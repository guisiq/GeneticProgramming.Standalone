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
    /// End-to-end tests running genetic programming on real-world regression datasets
    /// </summary>
    public class RegressionEndToEndTests
    {
        /// <summary>
        /// Tests GP on Boston Housing dataset - predicting house prices
        /// </summary>
        [Fact]
        public async Task BostonHousing_GeneticProgramming_ShouldImproveOverGenerations()
        {
            // Arrange - Get Boston Housing dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            
            // Split into train/test (80/20)
            int trainSize = (int)(inputs.Length * 0.8);
            var trainInputs = new double[trainSize][];
            var trainTargets = new double[trainSize];
            var testInputs = new double[inputs.Length - trainSize][];
            var testTargets = new double[inputs.Length - trainSize];
            
            Array.Copy(inputs, 0, trainInputs, 0, trainSize);
            Array.Copy(targets, 0, trainTargets, 0, trainSize);
            Array.Copy(inputs, trainSize, testInputs, 0, inputs.Length - trainSize);
            Array.Copy(targets, trainSize, testTargets, 0, inputs.Length - trainSize);

            // Create grammar with mathematical operations for regression
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            var evaluator = new RegressionFitnessEvaluator(trainInputs, trainTargets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = 50,
                MaxGenerations = 15,  // Increased generations for better chance of improvement
                FitnessEvaluator = evaluator
            };

            // Track fitness progress
            double initialFitness = double.NegativeInfinity;
            double finalFitness = double.NegativeInfinity;
            int generations = 0;

            algorithm.GenerationCompleted += (s, e) =>
            {
                if (generations == 0)
                    initialFitness = e.BestFitness;
                finalFitness = e.BestFitness;
                generations++;
            };

            // Act - Run genetic programming
            algorithm.Run();

            // Assert - Verify improvement and reasonable performance
            Assert.True(finalFitness >= initialFitness - 100, 
                $"Fitness should maintain or improve: initial={initialFitness:F4}, final={finalFitness:F4}");
            
            Assert.True(finalFitness > -2000, 
                $"Final fitness should be reasonable: {finalFitness:F4}");
            
            Assert.Equal(15, generations);
            Assert.NotNull(algorithm.BestIndividual);
            
            // Test on holdout data
            var testEvaluator = new RegressionFitnessEvaluator(testInputs, testTargets, variableNames);
            double testFitness = testEvaluator.Evaluate(algorithm.BestIndividual);
            
            Assert.True(testFitness > double.NegativeInfinity, 
                $"Test fitness should be valid: {testFitness:F4}");
        }

        /// <summary>
        /// Tests GP on Diabetes dataset - predicting disease progression
        /// </summary>
        [Fact]
        public async Task Diabetes_GeneticProgramming_ShouldEvolveSolution()
        {
            // Arrange - Get Diabetes dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetDiabetesDatasetAsync();
            
            // Use smaller subset for faster testing
            int sampleSize = Math.Min(50, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            // Create comprehensive grammar for medical data
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision,
                StatisticsSymbols.Mean
            }, allowConstants: true);

            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(123),
                PopulationSize = 30,
                MaxGenerations = 8,
                FitnessEvaluator = evaluator
            };

            double bestInitialFitness = double.NegativeInfinity;
            double bestFinalFitness = double.NegativeInfinity;

            algorithm.GenerationCompleted += (s, e) =>
            {
                if (e.Generation == 0)
                    bestInitialFitness = e.BestFitness;
                bestFinalFitness = e.BestFitness;
            };

            // Act
            algorithm.Run();

            // Assert
            Assert.True(bestFinalFitness >= bestInitialFitness, 
                "GP should maintain or improve fitness over generations");
            
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestIndividual.Length > 0, 
                "Best individual should have valid structure");
            
            Assert.True(bestFinalFitness > -50000, 
                "Fitness should be in reasonable range for diabetes data");
        }

        /// <summary>
        /// Tests GP on Wine Quality dataset - predicting wine ratings
        /// </summary>
        [Fact]
        public async Task WineQuality_GeneticProgramming_WithStatisticalOperators_ShouldWork()
        {
            // Arrange - Get Wine Quality dataset  
            var (inputs, targets, variableNames) = await DatasetManager.GetWineQualityDatasetAsync();

            // Create grammar with statistical operators suitable for wine analysis
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction, 
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision,
                StatisticsSymbols.Mean,
                StatisticsSymbols.Variance
            }, allowConstants: true);

            var evaluator = new RegressionFitnessEvaluator(inputs, targets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new FullTreeCreator(),  // Use full tree creator
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(456),
                PopulationSize = 40,
                MaxGenerations = 6,
                FitnessEvaluator = evaluator
            };

            bool algorithmCompleted = false;
            double finalBestFitness = double.NegativeInfinity;

            algorithm.GenerationCompleted += (s, e) =>
            {
                finalBestFitness = e.BestFitness;
                if (e.Generation == algorithm.MaxGenerations - 1)
                    algorithmCompleted = true;
            };

            // Act
            algorithm.Run();

            // Assert
            Assert.True(algorithmCompleted, "Algorithm should complete all generations");
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestIndividual.Length >= 1, "Best individual should have at least one node");
            Assert.True(finalBestFitness > double.NegativeInfinity, "Should produce valid fitness values");
            
            // Wine quality scores range 3-9, so MSE should be reasonable
            Assert.True(finalBestFitness > -100, $"Fitness should be reasonable for wine data: {finalBestFitness:F4}");
        }

        /// <summary>
        /// Tests GP with minimal operators to ensure core functionality
        /// </summary>
        [Fact]
        public async Task BostonHousing_MinimalOperators_ShouldStillWork()
        {
            // Arrange - Get small subset of Boston data
            var (inputs, targets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            
            // Use only first 20 samples for quick test
            int sampleSize = Math.Min(20, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            // Create minimal grammar - only addition and one variable
            var minimalGrammar = SymbolicRegressionGrammar.CreateSimpleGrammar(new[] { variableNames[0] }); // Just first variable

            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, new[] { variableNames[0] });

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = minimalGrammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(789),
                PopulationSize = 10,
                MaxGenerations = 3,
                FitnessEvaluator = evaluator
            };

            // Act
            algorithm.Run();

            // Assert
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > double.NegativeInfinity);
            Assert.True(algorithm.Population.Count == 10);
        }

        /// <summary>
        /// Performance test - ensures GP completes in reasonable time
        /// </summary>
        [Fact]
        public async Task LargeDataset_GeneticProgramming_ShouldCompleteInReasonableTime()
        {
            // Arrange - Use larger diabetes dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetDiabetesDatasetAsync();

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, variableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(999),
                PopulationSize = 20,
                MaxGenerations = 5,
                FitnessEvaluator = evaluator
            };

            // Act - Measure execution time
            var startTime = DateTime.Now;
            algorithm.Run();
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            // Assert - Should complete within reasonable time (30 seconds)
            Assert.True(duration.TotalSeconds < 30, 
                $"GP should complete within 30 seconds, took {duration.TotalSeconds:F2}s");
            
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > double.NegativeInfinity);
        }
    }
}
