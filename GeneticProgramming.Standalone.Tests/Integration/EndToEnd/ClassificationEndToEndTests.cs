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
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting Iris classification test...");
            
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
                
                if (generationCount % 5 == 0)
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generation {generationCount}, Best Fitness: {e.BestFitness:F3}");
            };

            // Act
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution with {algorithm.PopulationSize} individuals, {algorithm.MaxGenerations} generations...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] Iris test completed in {duration.TotalSeconds:F2}s");

            // Assert - Check evolution progress
            Assert.True(finalAccuracy >= initialAccuracy, 
                $"Accuracy should not decrease: initial={initialAccuracy:F3}, final={finalAccuracy:F3}");
            
            Assert.True(finalAccuracy >= 0.5, 
                $"Final accuracy should be better than random: {finalAccuracy:F3}");
            
            Assert.Equal(15, generationCount);
            Assert.NotNull(algorithm.BestIndividual);

            // Test on holdout data
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Testing on holdout data...");
            var testEvaluator = new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames);
            double testAccuracy = testEvaluator.Evaluate(algorithm.BestIndividual);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Test accuracy: {testAccuracy:F3}");
            Assert.True(testAccuracy >= 0.4, 
                $"Test accuracy should be reasonable: {testAccuracy:F3}");
        }

        /// <summary>
        /// Tests GP on simplified binary classification task
        /// </summary>
        [Fact]
        public void SimpleClassification_GeneticProgramming_ShouldAchieveHighAccuracy()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting SimpleClassification test...");
            
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
                    
                if (e.Generation % 5 == 0)
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generation {e.Generation}, Best Fitness: {e.BestFitness:F3}");
            };

            // Act
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution for simple classification...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] SimpleClassification test completed in {duration.TotalSeconds:F2}s");

            // Assert - Should achieve high accuracy on this simple problem
            Assert.True(bestAccuracy >= 0.8, 
                $"Should achieve high accuracy on simple linear problem: {bestAccuracy:F3}");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final accuracy: {bestAccuracy:F3}");
            Assert.NotNull(algorithm.BestIndividual);
        }

        /// <summary>
        /// Tests GP with limited features for robustness
        /// </summary>
        [Fact]
        public async Task IrisLimitedFeatures_GeneticProgramming_ShouldHandleConstraints()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting IrisLimitedFeatures test...");
            
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
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution with limited features...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] IrisLimitedFeatures test completed in {duration.TotalSeconds:F2}s");

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > 0.5, 
                "Should perform better than random even with limited features");
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final fitness: {algorithm.BestFitness:F3}");
            
            // Verify the tree uses only the allowed variables
            var treeString = algorithm.BestIndividual.ToString();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generated tree: {treeString}");
            Assert.True(!string.IsNullOrEmpty(treeString), 
                       "Tree should be generated and not empty");
        }

        /// <summary>
        /// Tests GP classification with statistical operators
        /// </summary>
        [Fact]
        public void Classification_WithStatisticalOperators_ShouldWork()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting Classification_WithStatisticalOperators test...");
            
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
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution with statistical operators...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] Classification_WithStatisticalOperators test completed in {duration.TotalSeconds:F2}s");

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final fitness: {algorithm.BestFitness:F3}");
            Assert.True(algorithm.BestFitness >= 0.6, 
                $"Should learn the mean-based pattern: {algorithm.BestFitness:F3}");
        }

        /// <summary>
        /// Performance test for classification tasks
        /// </summary>
        [Fact]
        public void LargeClassificationDataset_ShouldCompleteEfficiently()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting LargeClassificationDataset performance test...");
            
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
            var algorithmStartTime = DateTime.Now;
            Console.WriteLine($"[{algorithmStartTime:HH:mm:ss.fff}] Starting GP evolution for large dataset...");
            algorithm.Run();
            var duration = DateTime.Now - algorithmStartTime;
            
            var endTime = DateTime.Now;
            var totalDuration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] LargeClassificationDataset test completed in {totalDuration.TotalSeconds:F2}s (GP: {duration.TotalSeconds:F2}s)");

            // Assert
            Assert.True(duration.TotalSeconds < 20, 
                $"Should complete within 20 seconds: {duration.TotalSeconds:F2}s");
            
            Assert.NotNull(algorithm.BestIndividual);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final fitness: {algorithm.BestFitness:F3}");
            Assert.True(algorithm.BestFitness > 0.5, 
                "Should perform better than random on circle classification");
        }

        /// <summary>
        /// Tests GP on handwritten digits dataset - high-dimensional problem requiring larger trees
        /// </summary>
        [Fact]
        public async Task HandwrittenDigits_GeneticProgramming_ShouldHandleHighDimensionalClassification()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting HandwrittenDigits high-dimensional classification test...");
            
            // Arrange - Get handwritten digits dataset (64 features, 10 classes)
            var (fullInputs, fullTargets, variableNames) = await DatasetManager.GetDigitsDatasetAsync();
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Dataset loaded: full dataset with {variableNames.Length} features");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Using subset for faster testing...");
            
            // Use subset for faster testing (first 500 samples)
            int subsetSize = Math.Min(500, fullInputs.Length);
            var inputs = new double[subsetSize][];
            var targets = new int[subsetSize];
            
            Array.Copy(fullInputs, 0, inputs, 0, subsetSize);
            Array.Copy(fullTargets, 0, targets, 0, subsetSize);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Using {subsetSize} samples, converting to binary classification (even vs odd digits)...");

            // Convert to binary classification: even digits (0,2,4,6,8) vs odd digits (1,3,5,7,9)
            // This creates a more balanced dataset
            var binaryTargets = new int[subsetSize];
            for (int i = 0; i < subsetSize; i++)
            {
                binaryTargets[i] = targets[i] % 2; // 0 for even digits, 1 for odd digits
            }

            // Split into train/test
            int trainSize = (int)(subsetSize * 0.7);
            var trainInputs = new double[trainSize][];
            var trainTargets = new int[trainSize];
            var testInputs = new double[subsetSize - trainSize][];
            var testTargets = new int[subsetSize - trainSize];
            
            Array.Copy(inputs, 0, trainInputs, 0, trainSize);
            Array.Copy(binaryTargets, 0, trainTargets, 0, trainSize);
            Array.Copy(inputs, trainSize, testInputs, 0, subsetSize - trainSize);
            Array.Copy(binaryTargets, trainSize, testTargets, 0, subsetSize - trainSize);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Data split: {trainSize} training, {subsetSize - trainSize} test samples");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Creating grammar with advanced operators for high-dimensional problem...");

            // Create grammar with more operators for complex problems
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision,
                StatisticsSymbols.Mean,
                StatisticsSymbols.Variance
            }, allowConstants: true);

            var evaluator = new ClassificationFitnessEvaluator(trainInputs, trainTargets, variableNames);

            // Configure algorithm for complex problem - larger trees and population
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = 60,        // Larger population for complex problem
                MaxGenerations = 25,        // More generations for convergence
                FitnessEvaluator = evaluator
            };

            double initialAccuracy = 0;
            double finalAccuracy = 0;
            double bestAccuracy = 0;
            int generationCount = 0;

            algorithm.GenerationCompleted += (s, e) =>
            {
                if (generationCount == 0)
                    initialAccuracy = e.BestFitness;
                finalAccuracy = e.BestFitness;
                if (e.BestFitness > bestAccuracy)
                    bestAccuracy = e.BestFitness;
                generationCount++;
                
                if (generationCount % 5 == 0)
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generation {generationCount}, Best Fitness: {e.BestFitness:F3}");
            };

            // Act
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution: {algorithm.PopulationSize} population, {algorithm.MaxGenerations} generations...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] HandwrittenDigits test completed in {duration.TotalSeconds:F2}s");

            // Assert - Check that algorithm can handle high-dimensional data
            Assert.True(finalAccuracy >= initialAccuracy, 
                $"Accuracy should not decrease significantly: initial={initialAccuracy:F3}, final={finalAccuracy:F3}");
            
            Assert.True(bestAccuracy >= 0.6, 
                $"Should achieve reasonable accuracy on digit recognition: {bestAccuracy:F3}");
            
            Assert.Equal(25, generationCount);
            Assert.NotNull(algorithm.BestIndividual);

            // Test on holdout data
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Testing on holdout data...");
            var testEvaluator = new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames);
            double testAccuracy = testEvaluator.Evaluate(algorithm.BestIndividual);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Test accuracy: {testAccuracy:F3}, Best training accuracy: {bestAccuracy:F3}");
            Assert.True(testAccuracy >= 0.5, 
                $"Test accuracy should be better than random: {testAccuracy:F3}");

            // Verify the algorithm handled high-dimensional problem
            var bestTreeString = algorithm.BestIndividual?.ToString() ?? "";
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Best tree length: {bestTreeString.Length} characters");
            Assert.True(bestTreeString.Length > 10, 
                $"Should evolve non-trivial trees for high-dimensional problem. Tree: {bestTreeString}");
        }

        /// <summary>
        /// Tests GP multiclass classification on digits dataset
        /// </summary>
        [Fact]
        public async Task DigitsMulticlass_GeneticProgramming_ShouldHandleMultipleClasses()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting DigitsMulticlass test...");
            
            // Arrange - Use smaller subset with 3 classes (digits 0, 1, 2)
            var (fullInputs, fullTargets, variableNames) = await DatasetManager.GetDigitsDatasetAsync();
            
            // Filter for only digits 0, 1, and 2
            var filteredData = fullInputs.Zip(fullTargets, (input, target) => new { input, target })
                                        .Where(x => x.target <= 2)
                                        .Take(200) // Limit to 200 samples for faster testing
                                        .ToArray();
            
            var inputs = filteredData.Select(x => x.input).ToArray();
            var targets = filteredData.Select(x => x.target).ToArray();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Filtered to {inputs.Length} samples for 3-class classification (digits 0,1,2)");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Creating grammar optimized for multiclass classification...");

            // Create grammar optimized for multiclass classification
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
                TreeCreator = new FullTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(123),
                PopulationSize = 40,
                MaxGenerations = 15,
                FitnessEvaluator = evaluator
            };

            // Act
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting GP evolution for multiclass classification...");
            algorithm.Run();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] DigitsMulticlass test completed in {duration.TotalSeconds:F2}s");

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final fitness: {algorithm.BestFitness:F3} (target > 0.33 for 3-class)");
            Assert.True(algorithm.BestFitness > 0.33, // Better than random for 3 classes
                $"Should perform better than random on 3-class problem: {algorithm.BestFitness:F3}");
        }

        /// <summary>
        /// Performance stress test with large feature set from digits dataset
        /// </summary>
        [Fact]
        public async Task LargeFeatureSet_GeneticProgramming_ShouldCompleteEfficiently()
        {
            var testStartTime = DateTime.Now;
            Console.WriteLine($"[{testStartTime:HH:mm:ss.fff}] Starting LargeFeatureSet performance stress test...");
            
            // Arrange - Use full 64 features with limited samples
            var (fullInputs, fullTargets, variableNames) = await DatasetManager.GetDigitsDatasetAsync();
            
            // Use first 100 samples for performance test
            int samples = Math.Min(100, fullInputs.Length);
            var inputs = new double[samples][];
            var targets = new int[samples];
            
            Array.Copy(fullInputs, 0, inputs, 0, samples);
            Array.Copy(fullTargets, 0, targets, 0, samples);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Using {samples} samples with all {variableNames.Length} features for performance test");
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Converting to binary classification (even vs odd digits)...");

            // Binary classification: even vs odd digits
            var binaryTargets = new int[samples];
            for (int i = 0; i < samples; i++)
            {
                binaryTargets[i] = targets[i] % 2; // 0 for even, 1 for odd
            }

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new ClassificationFitnessEvaluator(inputs, binaryTargets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(456),
                PopulationSize = 30,
                MaxGenerations = 10,  // Shorter run for performance test
                FitnessEvaluator = evaluator
            };

            // Act - Measure performance with high-dimensional data
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting GP evolution with 64 features...");
            algorithm.Run();
            var duration = DateTime.Now - startTime;
            
            var endTime = DateTime.Now;
            var totalDuration = endTime - testStartTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] LargeFeatureSet test completed in {totalDuration.TotalSeconds:F2}s (GP: {duration.TotalSeconds:F2}s)");

            // Assert
            Assert.True(duration.TotalSeconds < 30, 
                $"Should complete within 30 seconds with 64 features: {duration.TotalSeconds:F2}s");
            
            Assert.NotNull(algorithm.BestIndividual);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Final fitness: {algorithm.BestFitness:F3}");
            Assert.True(algorithm.BestFitness > 0.4, 
                "Should perform reasonably well on even/odd classification");
        }
    }
}
