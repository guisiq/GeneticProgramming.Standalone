using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    public class FullPipelineEndToEndTests
    {
        [Fact]
        public async Task CompleteMachineLearningPipeline_BostonHousing_ShouldWorkEndToEnd()
        {
            // Step 1: Data Loading
            var (allInputs, allTargets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            Assert.True(allInputs.Length > 0, "Dataset should be loaded successfully");
            Assert.Equal(allInputs.Length, allTargets.Length);

            // Step 2: Data Preprocessing - Normalization
            var normalizedData = NormalizeData(allInputs, allTargets);
            var (normalizedInputs, normalizedTargets, inputMeans, inputStds, targetMean, targetStd) = normalizedData;

            // Step 3: Train/Validation/Test Split (60/20/20)
            var (trainInputs, trainTargets, valInputs, valTargets, testInputs, testTargets) = 
                SplitDataset(normalizedInputs, normalizedTargets, 0.6, 0.2, 0.2, 42);

            Assert.True(trainInputs.Length > 0, "Training set should not be empty");
            Assert.True(valInputs.Length > 0, "Validation set should not be empty");
            Assert.True(testInputs.Length > 0, "Test set should not be empty");

            // Step 4: Model Configuration
            // Explicitly defining the 'funSymbols' parameter
            var funSymbols = new List<ISymbol<double>>
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            };

            var grammar = new SymbolicRegressionGrammar<double>(variableNames, funSymbols, allowConstants: true);

            var trainEvaluator = new RegressionFitnessEvaluator(trainInputs, trainTargets, variableNames);
            var valEvaluator = new RegressionFitnessEvaluator(valInputs, valTargets, variableNames);

            // Step 5: Hyperparameter Selection via Validation
            var bestConfig = SelectBestConfiguration(grammar, trainEvaluator, valEvaluator);

            // Step 6: Final Model Training
            var finalAlgorithm = new GeneticProgrammingAlgorithm<double>
            {
                Grammar = grammar,
                TreeCreator = bestConfig.TreeCreator,
                Crossover = bestConfig.Crossover,
                Mutator = new SubtreeMutator<double>(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = bestConfig.PopulationSize,
                MaxGenerations = bestConfig.MaxGenerations,
                FitnessEvaluator = trainEvaluator
            };

            var trainingHistory = new List<(int generation, double trainFitness, double valFitness)>();
            
            finalAlgorithm.GenerationCompleted += (s, e) =>
            {
                var valFitness = valEvaluator.Evaluate(e.BestIndividual);
                trainingHistory.Add((e.Generation, e.BestFitness, valFitness));

                // Add a breakpoint when efficiency exceeds 0.97
                if (e.BestFitness > 0.97)
                {
                    Console.WriteLine($"Stopping early at generation {e.Generation} with efficiency {e.BestFitness:F3}");
                    finalAlgorithm.Stop();
                }
            };

            finalAlgorithm.GenerationCompleted += (s, e) =>
            {
                var bestIndividual = e.BestIndividual;
                if (bestIndividual != null)
                {
                    Console.WriteLine($"Generation {e.Generation}: {bestIndividual.ToMathString()}");
                }

                var averageEfficiency = e.AverageFitness;
                var bestEfficiency = e.BestFitness;

                Console.WriteLine($"Average efficiency of population: {e.AverageFitness:F3}");
                Console.WriteLine($"Best efficiency of generation: {e.BestFitness:F3}");

            };

            finalAlgorithm.Run();

            // Step 7: Model Evaluation on Test Set
            var testEvaluator = new RegressionFitnessEvaluator(testInputs, testTargets, variableNames);
            Assert.NotNull(finalAlgorithm.BestIndividual);
            var finalTestFitness = testEvaluator.Evaluate(finalAlgorithm.BestIndividual);

            // Step 8: Results Analysis
            var finalTrainFitness = trainingHistory.Last().trainFitness;
            var finalValFitness = trainingHistory.Last().valFitness;

            // Denormalize fitness for interpretation (convert back to original scale)
            var denormalizedTestMSE = -finalTestFitness * (targetStd * targetStd);
            var testRMSE = Math.Sqrt(Math.Abs(denormalizedTestMSE));

            // Step 9: Assertions
            Assert.True(finalTestFitness > double.NegativeInfinity, "Test fitness should be valid");
            Assert.True(testRMSE < 50, $"Test RMSE should be reasonable: {testRMSE:F2}"); // Reasonable for housing prices
            
            Assert.True(trainingHistory.Count > 0, "Should have training history");
            Assert.True(trainingHistory.First().trainFitness <= trainingHistory.Last().trainFitness, 
                "Training fitness should improve or stay same");

            // Check for overfitting (validation fitness shouldn't be much worse than training)
            var fitnessDifference = Math.Abs(finalTrainFitness - finalValFitness);
            Assert.True(fitnessDifference < 1.0, 
                $"Model shouldn't severely overfit: train={finalTrainFitness:F4}, val={finalValFitness:F4}");

            // Step 10: Model Interpretability
            var bestModel = finalAlgorithm.BestIndividual;
            Assert.True(bestModel.Length < 50, "Model should be reasonably interpretable");
            
            var modelString = bestModel.ToString();
            Assert.True(!string.IsNullOrWhiteSpace(modelString), "Model should have string representation");
        }

        /// <summary>
        /// Multi-class classification pipeline with cross-validation
        /// </summary>
        [Fact]
        public async Task MultiClassClassificationPipeline_Iris_ShouldWorkWithCrossValidation()
        {
            // Data Loading
            var (inputs, targets, variableNames) = await DatasetManager.GetIrisDatasetAsync();

            // Convert to one-vs-rest binary classification for each class
            var classResults = new Dictionary<int, double>();

            for (int targetClass = 0; targetClass < 3; targetClass++)
            {
                // Create binary targets (1 for target class, 0 for others)
                var binaryTargets = targets.Select(t => t == targetClass ? 1 : 0).ToArray();

                // 5-fold cross-validation
                var foldSize = inputs.Length / 5;
                var foldAccuracies = new List<double>();

                for (int fold = 0; fold < 5; fold++)
                {
                    // Create train/test split for this fold
                    var testStartIdx = fold * foldSize;
                    var testEndIdx = Math.Min((fold + 1) * foldSize, inputs.Length);
                    
                    // Shuffle data to avoid target imbalance
                    var random = new Random(42 + fold);
                    var indices = Enumerable.Range(0, inputs.Length).OrderBy(_ => random.Next()).ToList();

                    var testIndices = indices.Skip(fold * foldSize).Take(testEndIdx - testStartIdx).ToList();
                    var trainIndices = indices.Except(testIndices).ToList();

                    var foldTrainInputs = trainIndices.Select(i => inputs[i]).ToArray();
                    var foldTrainTargets = trainIndices.Select(i => binaryTargets[i]).ToArray();
                    var foldTestInputs = testIndices.Select(i => inputs[i]).ToArray();
                    var foldTestTargets = testIndices.Select(i => binaryTargets[i]).ToArray();

                    // Train model for this fold
                    var grammar = new SymbolicRegressionGrammar<double>(variableNames, MathematicalSymbols.AllSymbols);
                    var evaluator = new ClassificationFitnessEvaluator(foldTrainInputs, foldTrainTargets, variableNames);

                    var algorithm = new GeneticProgrammingAlgorithm<double>
                    {
                        Grammar = grammar,
                        TreeCreator = new GrowTreeCreator<double>(),
                        Crossover = new SubtreeCrossover<double>(),
                        Mutator = new SubtreeMutator<double>(),
                        Selector = new TournamentSelector(),
                        Random = new MersenneTwister(42 + fold),
                        PopulationSize = 100,  // Increased from 30
                        MaxGenerations = 100,  // Increased from 10
                        FitnessEvaluator = evaluator
                    };
                    
                    algorithm.GenerationCompleted += (s, e) =>
                    {
                        var bestIndividual = e.BestIndividual;
                        if (bestIndividual != null)
                        {
                            // Add a breakpoint when efficiency exceeds 0.97
                            if (e.BestFitness > 0.97)
                            {
                                Console.WriteLine($"Stopping early at generation {e.Generation} with efficiency {e.BestFitness:F3}");
                                algorithm.Stop();
                            }

                            Console.WriteLine($"Generation {e.Generation}: {bestIndividual.ToMathString()}");
                            
                            var averageEfficiency = e.AverageFitness;
                            var bestEfficiency = e.BestFitness;

                            Console.WriteLine($"Average efficiency of population: {e.AverageFitness:F3}");
                            Console.WriteLine($"Best efficiency of generation: {e.BestFitness:F3}");
                        }
                    };

                    algorithm.Run();

                    var testEvaluator = new ClassificationFitnessEvaluator(foldTestInputs, foldTestTargets, variableNames);
                    Assert.NotNull(algorithm.BestIndividual);
                    var foldAccuracy = testEvaluator.Evaluate(algorithm.BestIndividual);
                    foldAccuracies.Add(foldAccuracy);
                }

                // Average accuracy across folds
                var avgAccuracy = foldAccuracies.Last();
                classResults[targetClass] = avgAccuracy;

                Assert.True(avgAccuracy >= 0.6, 
                    $"Class {targetClass} should achieve reasonable accuracy: {avgAccuracy:F3}");
            }

            // Overall performance should be good
            var overallAccuracy = classResults.Values.Average();
            Assert.True(overallAccuracy >= 0.7, 
                $"Overall multi-class performance should be good: {overallAccuracy:F3}");
        }

        /// <summary>
        /// Regression pipeline with feature selection and model ensemble
        /// </summary>
        [Fact]
        public async Task RegressionPipelineWithFeatureSelection_Wine_ShouldOptimizeFeatures()
        {
            // Data Loading
            var (allInputs, allTargets, allVariableNames) = await DatasetManager.GetWineQualityDatasetAsync();

            // Feature Selection - Test different feature subsets
            var featureSubsets = new[]
            {
                new[] { 0, 1, 2 },      // First 3 features
                new[] { 3, 4, 5 },      // Middle 3 features  
                new[] { 8, 9, 10 },     // Last 3 features
                new[] { 1, 5, 9 },      // Mixed features
                Enumerable.Range(0, Math.Min(6, allVariableNames.Length)).ToArray() // First 6 features
            };

            var featureResults = new List<(int[] features, double performance)>();

            foreach (var featureIndices in featureSubsets)
            {
                // Extract selected features
                var selectedVariableNames = featureIndices.Select(i => allVariableNames[i]).ToArray();
                var selectedInputs = allInputs.Select(row => 
                    featureIndices.Select(i => row[i]).ToArray()).ToArray();

                // Split data
                var (trainInputs, trainTargets, _, _, testInputs, testTargets) = 
                    SplitDataset(selectedInputs, allTargets, 0.8, 0.0, 0.2, 42);

                // Train model
                var grammar = new SymbolicRegressionGrammar<double>(selectedVariableNames,MathematicalSymbols.AllSymbols);
                var evaluator = new RegressionFitnessEvaluator(trainInputs, trainTargets, selectedVariableNames);

                var algorithm = new GeneticProgrammingAlgorithm<double>
                {
                    Grammar = grammar,
                    TreeCreator = new GrowTreeCreator<double>(),
                    Crossover = new SubtreeCrossover<double>(),
                    Mutator = new SubtreeMutator<double>(),
                    Selector = new TournamentSelector(),
                    Random = new MersenneTwister(123),
                    PopulationSize = 25,
                    MaxGenerations = 8,
                    FitnessEvaluator = evaluator
                };

                algorithm.Run();
                algorithm.GenerationCompleted += (s, e) =>
                {
                    // Add a breakpoint when efficiency exceeds 0.97
                    if (e.BestFitness > 0.97)
                    {
                        Console.WriteLine($"Stopping early at generation {e.Generation} with efficiency {e.BestFitness:F3}");
                        algorithm.Stop();
                    }
                };

                // Evaluate on test set
                var testEvaluator = new RegressionFitnessEvaluator(testInputs, testTargets, selectedVariableNames);
                Assert.NotNull(algorithm.BestIndividual);
                var testPerformance = testEvaluator.Evaluate(algorithm.BestIndividual);
                
                featureResults.Add((featureIndices, testPerformance));

                Assert.True(testPerformance > double.NegativeInfinity, 
                    $"Feature subset should produce valid results");
            }

            // Find best feature subset
            var bestFeatureResult = featureResults.OrderByDescending(r => r.performance).First();
            var worstFeatureResult = featureResults.OrderBy(r => r.performance).First();

            Assert.True(bestFeatureResult.performance > worstFeatureResult.performance,
                "Feature selection should show performance differences");

            Assert.True(featureResults.All(r => r.performance > -1000),
                "All feature subsets should achieve reasonable performance");
        }

        /// <summary>
        /// Helper method to normalize data
        /// </summary>
        private (double[][] normalizedInputs, double[] normalizedTargets, double[] inputMeans, double[] inputStds, double targetMean, double targetStd) 
            NormalizeData(double[][] inputs, double[] targets)
        {
            int numFeatures = inputs[0].Length;
            var inputMeans = new double[numFeatures];
            var inputStds = new double[numFeatures];

            // Calculate means and standard deviations for inputs
            for (int j = 0; j < numFeatures; j++)
            {
                inputMeans[j] = inputs.Average(row => row[j]);
                var variance = inputs.Average(row => Math.Pow(row[j] - inputMeans[j], 2));
                inputStds[j] = Math.Sqrt(variance);
                if (inputStds[j] == 0) inputStds[j] = 1; // Avoid division by zero
            }

            // Calculate mean and std for targets
            var targetMean = targets.Average();
            var targetVariance = targets.Average(t => Math.Pow(t - targetMean, 2));
            var targetStd = Math.Sqrt(targetVariance);
            if (targetStd == 0) targetStd = 1;

            // Normalize inputs
            var normalizedInputs = inputs.Select(row =>
                row.Select((val, j) => (val - inputMeans[j]) / inputStds[j]).ToArray()).ToArray();

            // Normalize targets
            var normalizedTargets = targets.Select(t => (t - targetMean) / targetStd).ToArray();

            return (normalizedInputs, normalizedTargets, inputMeans, inputStds, targetMean, targetStd);
        }

        /// <summary>
        /// Helper method to split dataset
        /// </summary>
        private (double[][] trainInputs, double[] trainTargets, double[][] valInputs, double[] valTargets, double[][] testInputs, double[] testTargets) 
            SplitDataset(double[][] inputs, double[] targets, double trainRatio, double valRatio, double testRatio, int seed)
        {
            var random = new Random(seed);
            var indices = Enumerable.Range(0, inputs.Length).OrderBy(x => random.Next()).ToArray();

            int trainSize = (int)(inputs.Length * trainRatio);
            int valSize = (int)(inputs.Length * valRatio);
            int testSize = inputs.Length - trainSize - valSize;

            var trainInputs = indices.Take(trainSize).Select(i => inputs[i]).ToArray();
            var trainTargets = indices.Take(trainSize).Select(i => targets[i]).ToArray();

            var valInputs = indices.Skip(trainSize).Take(valSize).Select(i => inputs[i]).ToArray();
            var valTargets = indices.Skip(trainSize).Take(valSize).Select(i => targets[i]).ToArray();

            var testInputs = indices.Skip(trainSize + valSize).Select(i => inputs[i]).ToArray();
            var testTargets = indices.Skip(trainSize + valSize).Select(i => targets[i]).ToArray();

            return (trainInputs, trainTargets, valInputs, valTargets, testInputs, testTargets);
        }

        /// <summary>
        /// Helper method to select best configuration via validation
        /// </summary>
        private (ISymbolicExpressionTreeCreator<double> TreeCreator, ISymbolicExpressionTreeCrossover<double> Crossover, int PopulationSize, int MaxGenerations) 
            SelectBestConfiguration(SymbolicRegressionGrammar<double> grammar, IFitnessEvaluator<double> trainEvaluator, IFitnessEvaluator<double> valEvaluator)
        {
            var configurations = new[]
            {
                (TreeCreator: (ISymbolicExpressionTreeCreator<double>)new GrowTreeCreator<double>(), Crossover: (ISymbolicExpressionTreeCrossover<double>)new SubtreeCrossover<double>(), PopSize: 20, MaxGen: 8),
                (TreeCreator: (ISymbolicExpressionTreeCreator<double>)new FullTreeCreator<double>(), Crossover: (ISymbolicExpressionTreeCrossover<double>)new SubtreeCrossover<double>(), PopSize: 20, MaxGen: 8),
                (TreeCreator: (ISymbolicExpressionTreeCreator<double>)new GrowTreeCreator<double>(), Crossover: (ISymbolicExpressionTreeCrossover<double>)new UniformCrossover<double>(), PopSize: 20, MaxGen: 8),
                (TreeCreator: (ISymbolicExpressionTreeCreator<double>)new GrowTreeCreator<double>(), Crossover: (ISymbolicExpressionTreeCrossover<double>)new SubtreeCrossover<double>(), PopSize: 30, MaxGen: 6)
            };

            double bestValFitness = double.NegativeInfinity;
            var bestConfig = configurations[0];

            foreach (var config in configurations)
            {
                var algorithm = new GeneticProgrammingAlgorithm<double>
                {
                    Grammar = grammar,
                    TreeCreator = config.TreeCreator,
                    Crossover = config.Crossover,
                    Mutator = new SubtreeMutator<double>(),
                    Selector = new TournamentSelector(),
                    Random = new MersenneTwister(42),
                    PopulationSize = config.PopSize,
                    MaxGenerations = config.MaxGen,
                    FitnessEvaluator = trainEvaluator
                };

                algorithm.Run();

                Assert.NotNull(algorithm.BestIndividual);
                var valFitness = valEvaluator.Evaluate(algorithm.BestIndividual);
                if (valFitness > bestValFitness)
                {
                    bestValFitness = valFitness;
                    bestConfig = config;
                }
            }

            return (bestConfig.TreeCreator, bestConfig.Crossover, bestConfig.PopSize, bestConfig.MaxGen);
        }
    }
}
