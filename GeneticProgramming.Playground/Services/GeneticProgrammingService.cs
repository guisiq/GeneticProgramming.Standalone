using System.ComponentModel.DataAnnotations;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Playground.Services;

/// <summary>
/// Service for configuring and running genetic programming experiments
/// </summary>
public class GeneticProgrammingService
{
    /// <summary>
    /// Creates a configured genetic programming algorithm
    /// </summary>
    public GeneticProgrammingAlgorithm CreateAlgorithm(ExperimentConfiguration config, double[][] inputs, int[] targets, string[] variableNames)
    {
        // Create grammar based on configuration
        var symbols = new List<ISymbol>();
        
        // Always ensure we have at least basic math for meaningful trees
        if (config.UseBasicMath || (!config.UseAdvancedMath && !config.UseStatistics))
        {
            symbols.AddRange(new ISymbol[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            });
        }
        
        if (config.UseAdvancedMath)
        {
            symbols.AddRange(new ISymbol[]
            {
                MathematicalTrigonometricSymbols.Sine,
                MathematicalTrigonometricSymbols.Cosine,
                MathematicalLogarithmicSymbols.Exponential,
                MathematicalLogarithmicSymbols.Logarithm
            });
        }
        
        if (config.UseStatistics)
        {
            symbols.AddRange(new ISymbol[]
            {
                StatisticsSymbols.Mean,
                StatisticsSymbols.Variance
            });
        }

        var grammar = new SymbolicRegressionGrammar(variableNames, symbols.ToArray(), config.AllowConstants);
        
        // Validate grammar has sufficient symbols
        if (symbols.Count == 0)
        {
            throw new InvalidOperationException("Grammar must contain at least one functional symbol. Enable BasicMath, AdvancedMath, or Statistics.");
        }
        
        if (variableNames.Length == 0 && !config.AllowConstants)
        {
            throw new InvalidOperationException("Grammar must contain variables or constants for terminal symbols.");
        }
        
        // Create evaluator
        IFitnessEvaluator evaluator = config.ProblemType switch
        {
            ProblemType.Classification => config.ClassificationFitnessType switch
            {
                ClassificationFitnessType.StandardAccuracy => new ClassificationFitnessEvaluator(inputs, targets, variableNames),
                ClassificationFitnessType.ImprovedGradient => new ImprovedClassificationFitnessEvaluator(inputs, targets, variableNames, config.ParsimonyPressure),
                _ => new ImprovedClassificationFitnessEvaluator(inputs, targets, variableNames, config.ParsimonyPressure)
            },
            ProblemType.Regression => config.RegressionFitnessType switch
            {
                RegressionFitnessType.MeanSquaredError => new RegressionFitnessEvaluator(inputs, targets.Select(t => (double)t).ToArray(), variableNames),
                _ => new RegressionFitnessEvaluator(inputs, targets.Select(t => (double)t).ToArray(), variableNames)
            },
            _ => throw new ArgumentException($"Unsupported problem type: {config.ProblemType}")
        };

        // Create operators
        ISymbolicExpressionTreeCreator treeCreator = config.TreeCreationMethod switch
        {
            TreeCreationMethod.Grow => new GrowTreeCreator(),
            TreeCreationMethod.Full => new FullTreeCreator(),
            _ => new GrowTreeCreator()
        };

        ISymbolicExpressionTreeCrossover crossover = config.CrossoverType switch
        {
            CrossoverType.SubtreeCrossover => new SubtreeCrossover(),
            CrossoverType.OnePointCrossover => new SubtreeCrossover(), // Default to subtree for now
            CrossoverType.UniformCrossover => new SubtreeCrossover(),  // Default to subtree for now
            _ => new SubtreeCrossover()
        };

        ISymbolicExpressionTreeMutator mutator = config.MutationType switch
        {
            MutationType.SubtreeMutation => new SubtreeMutator(),
            MutationType.PointMutation => new SubtreeMutator(),    // Default to subtree for now
            MutationType.HoistMutation => new SubtreeMutator(),    // Default to subtree for now
            MutationType.ShrinkMutation => new SubtreeMutator(),   // Default to subtree for now
            _ => new SubtreeMutator()
        };

        // Create algorithm
        var algorithm = new GeneticProgrammingAlgorithm
        {
            Grammar = grammar,
            TreeCreator = treeCreator,
            Crossover = crossover,
            Mutator = mutator,
            Selector = new TournamentSelector(),
            Random = new MersenneTwister(config.RandomSeed),
            PopulationSize = config.PopulationSize,
            MaxGenerations = config.MaxGenerations,
            MaxTreeDepth = config.MaxTreeDepth,
            MaxTreeLength = config.MaxTreeLength,
            CrossoverProbability = config.CrossoverProbability,
            MutationProbability = config.MutationProbability,
            EliteCount = config.EliteCount,
            EliteBreedingRatio = config.EliteBreedingRatio,
            FitnessEvaluator = evaluator
        };

        return algorithm;
    }
}

/// <summary>
/// Configuration for genetic programming experiments
/// </summary>
public class ExperimentConfiguration
{
    [Required]
    [Range(10, 1000)]
    public int PopulationSize { get; set; } = 50;

    [Required]
    [Range(5, 500)]
    public int MaxGenerations { get; set; } = 25;

    [Required]
    public int RandomSeed { get; set; } = 42;

    public bool UseBasicMath { get; set; } = true;
    public bool UseAdvancedMath { get; set; } = true;      // Habilitado para problemas complexos
    public bool UseStatistics { get; set; } = true;       // Habilitado para operações estatísticas
    public bool AllowConstants { get; set; } = true;

    public ProblemType ProblemType { get; set; } = ProblemType.Classification;
    public ClassificationFitnessType ClassificationFitnessType { get; set; } = ClassificationFitnessType.ImprovedGradient;
    public RegressionFitnessType RegressionFitnessType { get; set; } = RegressionFitnessType.MeanSquaredError;
    
    [Range(0.0, 0.01)]
    public double ParsimonyPressure { get; set; } = 0.001;  // Pressão de parcimônia para o avaliador melhorado
    
    public TreeCreationMethod TreeCreationMethod { get; set; } = TreeCreationMethod.Grow;
    public MutationType MutationType { get; set; } = MutationType.SubtreeMutation;
    public CrossoverType CrossoverType { get; set; } = CrossoverType.SubtreeCrossover;

    [Range(0.0, 1.0)]
    public double CrossoverProbability { get; set; } = 0.9;
    
    [Range(0.0, 1.0)]
    public double MutationProbability { get; set; } = 0.1;

    [Range(1, 10)]
    public int EliteCount { get; set; } = 1;
    
    [Range(0.0, 1.0)]
    public double EliteBreedingRatio { get; set; } = 0.2;

    public string Name { get; set; } = "New Experiment";
    public string Description { get; set; } = "";
    
    [Required]
    [Range(1, 20)]
    public int MaxTreeDepth { get; set; } = 12;        // Aumentado para problemas complexos
    
    [Required]
    [Range(1, 200)]
    public int MaxTreeLength { get; set; } = 100;      // Aumentado para árvores mais expressivas
}

public enum ProblemType
{
    Classification,
    Regression
}

public enum ClassificationFitnessType
{
    StandardAccuracy,      // ClassificationFitnessEvaluator original
    ImprovedGradient      // ImprovedClassificationFitnessEvaluator com gradiente suave
}

public enum RegressionFitnessType
{
    MeanSquaredError      // RegressionFitnessEvaluator padrão
}

public enum TreeCreationMethod
{
    Grow,
    Full
}

public enum MutationType
{
    SubtreeMutation,
    PointMutation,
    HoistMutation,
    ShrinkMutation
}

public enum CrossoverType
{
    SubtreeCrossover,
    OnePointCrossover,
    UniformCrossover
}

/// <summary>
/// Results from running an experiment
/// </summary>
public class ExperimentResult
{
    public string ExperimentName { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    
    public double FinalFitness { get; set; }
    public double BestFitness { get; set; }
    public double InitialFitness { get; set; }
    
    public int GenerationsCompleted { get; set; }
    public string BestIndividualString { get; set; } = "";
    
    public List<GenerationMetrics> GenerationHistory { get; set; } = new();
    
    public ExperimentConfiguration Configuration { get; set; } = new();
    public string DatasetName { get; set; } = "";
    
    public double TestAccuracy { get; set; }
    public int TreeComplexity { get; set; }
}

/// <summary>
/// Metrics for a single generation
/// </summary>
public class GenerationMetrics
{
    public int Generation { get; set; }
    public double BestFitness { get; set; }
    public double AverageFitness { get; set; }
    public double WorstFitness { get; set; }
    public DateTime Timestamp { get; set; }
    public int BestTreeSize { get; set; }
    public double Diversity { get; set; }
    // Representação em texto da melhor árvore nesta geração
    public string BestTreeString { get; set; } = "";
}
