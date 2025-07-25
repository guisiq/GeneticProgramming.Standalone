using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions;
using GeneticProgramming.Problems.Evaluators;

namespace GeneticProgramming.Playground.Services;

/// <summary>
/// Service for running experiments and collecting metrics
/// </summary>
public class ExperimentRunner
{
    private readonly GeneticProgrammingService _gpService;
    private readonly DatasetService _datasetService;
    private readonly ExperimentRepository _experimentRepository;

    public ExperimentRunner(GeneticProgrammingService gpService, DatasetService datasetService, ExperimentRepository experimentRepository)
    {
        _gpService = gpService;
        _datasetService = datasetService;
        _experimentRepository = experimentRepository;
    }

    /// <summary>
    /// Runs an experiment with the given configuration
    /// </summary>
    public async Task<ExperimentResult> RunExperimentAsync(ExperimentConfiguration config, string datasetName, 
        IProgress<GenerationMetrics>? progress = null, CancellationToken cancellationToken = default)
    {
        var result = new ExperimentResult
        {
            ExperimentName = config.Name,
            Configuration = config,
            DatasetName = datasetName,
            StartTime = DateTime.Now
        };

        try
        {
            // Load dataset
            var (inputs, targets, variableNames) = await _datasetService.LoadDatasetAsync(datasetName);
            
            // Split into train/test if it's a real dataset
            var (trainInputs, trainTargets, testInputs, testTargets) = SplitDataset(inputs, targets, 0.7);

            // Create and configure algorithm
            var algorithm = _gpService.CreateAlgorithm(config, trainInputs, trainTargets, variableNames);

            // Set up event handlers for tracking progress
            var generationMetrics = new List<GenerationMetrics>();
            double initialFitness = 0;
            double bestFitness = 0;

            algorithm.GenerationCompleted += (sender, e) =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (e.Generation == 0)
                    initialFitness = e.BestFitness;

                if (e.BestFitness > bestFitness)
                    bestFitness = e.BestFitness;

                var metrics = new GenerationMetrics
                {
                    Generation = e.Generation,
                    BestFitness = e.BestFitness,
                    AverageFitness = e.AverageFitness,
                    WorstFitness = 0, // Not available in GenerationEventArgs
                    Timestamp = DateTime.Now,
                    BestTreeSize = e.BestIndividual?.ToString()?.Length ?? 0,
                    Diversity = 0, // Population not available in GenerationEventArgs
                    BestTreeString = e.BestIndividual?.ToTreeString() ?? string.Empty
                };

                generationMetrics.Add(metrics);
                progress?.Report(metrics);
            };

            // Run the algorithm
            await Task.Run(() => algorithm.Run(), cancellationToken);

            // Calculate test metrics if we have test data
            double testAccuracy = 0;
            if (testInputs.Length > 0 && algorithm.BestIndividual != null)
            {
                // Use the same type of evaluator that was used for training
                IFitnessEvaluator testEvaluator = config.ProblemType switch
                {
                    ProblemType.Classification => config.ClassificationFitnessType switch
                    {
                        ClassificationFitnessType.StandardAccuracy => new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames),
                        ClassificationFitnessType.ImprovedGradient => new ImprovedClassificationFitnessEvaluator(testInputs, testTargets, variableNames, config.ParsimonyPressure),
                        _ => new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames)
                    },
                    ProblemType.Regression => new RegressionFitnessEvaluator(testInputs, testTargets.Select(t => (double)t).ToArray(), variableNames),
                    _ => new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames)
                };
                testAccuracy = testEvaluator.Evaluate(algorithm.BestIndividual);
            }

            // Fill in final results
            result.EndTime = DateTime.Now;
            result.FinalFitness = algorithm.BestFitness;
            result.BestFitness = bestFitness;
            result.InitialFitness = initialFitness;
            result.GenerationsCompleted = generationMetrics.Count;
            result.BestIndividualString = algorithm.BestIndividual?.ToString() ?? "";
            result.GenerationHistory = generationMetrics;
            result.TestAccuracy = testAccuracy;
            result.TreeComplexity = result.BestIndividualString.Length;

            // Save to database
            try
            {
                await _experimentRepository.SaveExperimentAsync(result);
            }
            catch (Exception dbEx)
            {
                // Log database error but don't fail the experiment
                Console.WriteLine($"Failed to save experiment to database: {dbEx.Message}");
            }

            return result;
        }
        catch (Exception ex)
        {
            result.EndTime = DateTime.Now;
            throw new InvalidOperationException($"Experiment failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Splits dataset into training and testing sets
    /// </summary>
    private (double[][] trainInputs, int[] trainTargets, double[][] testInputs, int[] testTargets) 
        SplitDataset(double[][] inputs, int[] targets, double trainRatio)
    {
        int trainSize = (int)(inputs.Length * trainRatio);
        
        var trainInputs = new double[trainSize][];
        var trainTargets = new int[trainSize];
        var testInputs = new double[inputs.Length - trainSize][];
        var testTargets = new int[inputs.Length - trainSize];
        
        Array.Copy(inputs, 0, trainInputs, 0, trainSize);
        Array.Copy(targets, 0, trainTargets, 0, trainSize);
        Array.Copy(inputs, trainSize, testInputs, 0, inputs.Length - trainSize);
        Array.Copy(targets, trainSize, testTargets, 0, inputs.Length - trainSize);

        return (trainInputs, trainTargets, testInputs, testTargets);
    }

    /// <summary>
    /// Calculates diversity of the population
    /// </summary>
    private double CalculateDiversity(IList<ISymbolicExpressionTree> population)
    {
        if (population.Count <= 1) return 0;

        var uniqueTrees = new HashSet<string>();
        foreach (var tree in population)
        {
            uniqueTrees.Add(tree?.ToString() ?? "");
        }

        return (double)uniqueTrees.Count / population.Count;
    }

    /// <summary>
    /// Runs multiple experiments for comparison
    /// </summary>
    public async Task<List<ExperimentResult>> RunBatchExperimentsAsync(
        List<(ExperimentConfiguration config, string datasetName)> experiments,
        IProgress<(int completed, int total, string currentExperiment)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ExperimentResult>();
        
        for (int i = 0; i < experiments.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var (config, datasetName) = experiments[i];
            progress?.Report((i, experiments.Count, config.Name));

            try
            {
                var result = await RunExperimentAsync(config, datasetName, null, cancellationToken);
                results.Add(result);
            }
            catch (Exception ex)
            {
                // Log error but continue with next experiment
                Console.WriteLine($"Experiment {config.Name} failed: {ex.Message}");
            }
        }

        progress?.Report((experiments.Count, experiments.Count, "Completed"));
        return results;
    }
}
