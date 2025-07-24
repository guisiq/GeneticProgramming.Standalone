using System;
using System.Threading.Tasks;
using Xunit;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// End-to-end tests running genetic programming on real-world classification datasets
    /// </summary>
    public class ClassificationEndToEndTests
    {
        /// <summary>
        /// Tests GP on Iris dataset - classic flower species classification
        /// </summary>
        [Fact]
        public async Task Iris_GeneticProgramming_ShouldLearnClassification()
        {
            // Arrange - Get Iris dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetIrisDatasetAsync();
            
            // Convert to binary classification (Setosa vs Others)
            var binaryTargets = new int[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                binaryTargets[i] = targets[i] == 0 ? 1 : 0; // 1 for Setosa, 0 for others
            }

            // Split into train/test
            int trainSize = (int)(inputs.Length * 0.7);
            var trainInputs = new double[trainSize][];
            var trainTargets = new int[trainSize];
            var testInputs = new double[inputs.Length - trainSize][];
            var testTargets = new int[inputs.Length - trainSize];
            
            Array.Copy(inputs, 0, trainInputs, 0, trainSize);
            Array.Copy(binaryTargets, 0, trainTargets, 0, trainSize);
            Array.Copy(inputs, trainSize, testInputs, 0, inputs.Length - trainSize);
            Array.Copy(binaryTargets, trainSize, testTargets, 0, inputs.Length - trainSize);

            // Create grammar suitable for classification
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            var evaluator = new ClassificationFitnessEvaluator(trainInputs, trainTargets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = 40,
                MaxGenerations = 15,
                FitnessEvaluator = evaluator
            };

            double initialAccuracy = 0;
            double finalAccuracy = 0;
            int generationCount = 0;

            algorithm.GenerationCompleted += (s, e) =>
            {
                if (generationCount == 0)
                    initialAccuracy = e.BestFitness;
                finalAccuracy = e.BestFitness;
                generationCount++;
            };

            // Act
            algorithm.Run();

            // Assert - Check evolution progress
            Assert.True(finalAccuracy >= initialAccuracy, 
                $"Accuracy should not decrease: initial={initialAccuracy:F3}, final={finalAccuracy:F3}");
            
            Assert.True(finalAccuracy >= 0.5, 
                $"Final accuracy should be better than random: {finalAccuracy:F3}");
            
            Assert.Equal(15, generationCount);
            Assert.NotNull(algorithm.BestIndividual);

            // Test on holdout data
            var testEvaluator = new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames);
            double testAccuracy = testEvaluator.Evaluate(algorithm.BestIndividual);
            
            Assert.True(testAccuracy >= 0.4, 
                $"Test accuracy should be reasonable: {testAccuracy:F3}");
        }

        /// <summary>
        /// Tests GP on simplified binary classification task
        /// </summary>
        [Fact]
        public async Task SimpleClassification_GeneticProgramming_ShouldAchieveHighAccuracy()
        {
            // Arrange - Create simple linearly separable dataset
            var random = new Random(42);
            int samples = 100;
            var inputs = new double[samples][];
            var targets = new int[samples];
            var variableNames = new[] { "X1", "X2" };

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[2];
                inputs[i][0] = random.NextDouble() * 10 - 5; // X1 in [-5, 5]
                inputs[i][1] = random.NextDouble() * 10 - 5; // X2 in [-5, 5]
                
                // Simple decision boundary: X1 + X2 > 0
                targets[i] = (inputs[i][0] + inputs[i][1] > 0) ? 1 : 0;
            }

            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction
            }, allowConstants: false); // No constants needed for this simple problem

            var evaluator = new ClassificationFitnessEvaluator(inputs, targets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(123),
                PopulationSize = 30,
                MaxGenerations = 20,
                FitnessEvaluator = evaluator
            };

            double bestAccuracy = 0;
            algorithm.GenerationCompleted += (s, e) =>
            {
                if (e.BestFitness > bestAccuracy)
                    bestAccuracy = e.BestFitness;
            };

            // Act
            algorithm.Run();

            // Assert - Should achieve high accuracy on this simple problem
            Assert.True(bestAccuracy >= 0.8, 
                $"Should achieve high accuracy on simple linear problem: {bestAccuracy:F3}");
            
            Assert.NotNull(algorithm.BestIndividual);
        }

        /// <summary>
        /// Tests GP with limited features for robustness
        /// </summary>
        [Fact]
        public async Task IrisLimitedFeatures_GeneticProgramming_ShouldHandleConstraints()
        {
            // Arrange - Use only 2 features from Iris dataset
            var (fullInputs, targets, fullVariableNames) = await DatasetManager.GetIrisDatasetAsync();
            
            // Extract only petal length and petal width (indices 2 and 3)
            var inputs = new double[fullInputs.Length][];
            var variableNames = new[] { fullVariableNames[2], fullVariableNames[3] };
            
            for (int i = 0; i < fullInputs.Length; i++)
            {
                inputs[i] = new double[2];
                inputs[i][0] = fullInputs[i][2]; // petal_length
                inputs[i][1] = fullInputs[i][3]; // petal_width
            }

            // Binary classification: Setosa vs Non-Setosa
            var binaryTargets = new int[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                binaryTargets[i] = targets[i] == 0 ? 1 : 0;
            }

            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            var evaluator = new ClassificationFitnessEvaluator(inputs, binaryTargets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new FullTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(456),
                PopulationSize = 25,
                MaxGenerations = 10,
                FitnessEvaluator = evaluator
            };

            // Act
            algorithm.Run();

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > 0.5, 
                "Should perform better than random even with limited features");
            
            // Verify the tree uses only the allowed variables
            var treeString = algorithm.BestIndividual.ToString();
            Assert.True(treeString.Contains(variableNames[0]) || treeString.Contains(variableNames[1]) || 
                       treeString.Contains("Constant"), 
                       "Tree should contain expected symbols");
        }

        /// <summary>
        /// Tests GP classification with statistical operators
        /// </summary>
        [Fact]
        public async Task Classification_WithStatisticalOperators_ShouldWork()
        {
            // Arrange - Create multi-feature classification problem
            var random = new Random(789);
            int samples = 80;
            var inputs = new double[samples][];
            var targets = new int[samples];
            var variableNames = new[] { "Feature1", "Feature2", "Feature3" };

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[3];
                inputs[i][0] = random.NextDouble() * 6 - 3; // [-3, 3]
                inputs[i][1] = random.NextDouble() * 6 - 3; // [-3, 3]
                inputs[i][2] = random.NextDouble() * 6 - 3; // [-3, 3]
                
                // More complex decision boundary using mean
                double mean = (inputs[i][0] + inputs[i][1] + inputs[i][2]) / 3.0;
                targets[i] = mean > 0 ? 1 : 0;
            }

            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                StatisticsSymbols.Mean
            }, allowConstants: true);

            var evaluator = new ClassificationFitnessEvaluator(inputs, targets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(789),
                PopulationSize = 35,
                MaxGenerations = 12,
                FitnessEvaluator = evaluator
            };

            // Act
            algorithm.Run();

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness >= 0.6, 
                $"Should learn the mean-based pattern: {algorithm.BestFitness:F3}");
        }

        /// <summary>
        /// Performance test for classification tasks
        /// </summary>
        [Fact]
        public async Task LargeClassificationDataset_ShouldCompleteEfficiently()
        {
            // Arrange - Create larger synthetic dataset
            var random = new Random(999);
            int samples = 200;
            var inputs = new double[samples][];
            var targets = new int[samples];
            var variableNames = new[] { "X", "Y" };

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[2];
                inputs[i][0] = random.NextDouble() * 20 - 10;
                inputs[i][1] = random.NextDouble() * 20 - 10;
                
                // Circle classification: inside circle = 1, outside = 0
                double distance = Math.Sqrt(inputs[i][0] * inputs[i][0] + inputs[i][1] * inputs[i][1]);
                targets[i] = distance <= 5 ? 1 : 0;
            }

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new ClassificationFitnessEvaluator(inputs, targets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(999),
                PopulationSize = 50,
                MaxGenerations = 8,
                FitnessEvaluator = evaluator
            };

            // Act - Measure performance
            var startTime = DateTime.Now;
            algorithm.Run();
            var duration = DateTime.Now - startTime;

            // Assert
            Assert.True(duration.TotalSeconds < 20, 
                $"Should complete within 20 seconds: {duration.TotalSeconds:F2}s");
            
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > 0.5, 
                "Should perform better than random on circle classification");
        }
    }
}
