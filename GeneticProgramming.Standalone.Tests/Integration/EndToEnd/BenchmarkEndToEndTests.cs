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

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// Benchmark and comparison tests for different GP configurations
    /// </summary>
    public class BenchmarkEndToEndTests
    {
        /// <summary>
        /// Compares different crossover operators on Boston Housing dataset
        /// </summary>
        [Fact]
        public async Task BostonHousing_CrossoverComparison_ShouldShowDifferences()
        {
            // Arrange - Get dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            
            // Use subset for faster testing
            int sampleSize = Math.Min(50, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);

            // Test different crossover operators
            var crossoverResults = new Dictionary<string, double>();

            // Test SubtreeCrossover
            var subtreeAlgorithm = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 42);
            subtreeAlgorithm.Run();
            crossoverResults["SubtreeCrossover"] = subtreeAlgorithm.BestFitness;

            // Test UniformCrossover  
            var uniformAlgorithm = CreateStandardAlgorithm(grammar, evaluator, new UniformCrossover(), 42);
            uniformAlgorithm.Run();
            crossoverResults["UniformCrossover"] = uniformAlgorithm.BestFitness;

            // Assert - All crossover types should produce valid results
            foreach (var result in crossoverResults)
            {
                Assert.True(result.Value > double.NegativeInfinity, 
                    $"{result.Key} should produce valid fitness: {result.Value:F4}");
                
                Assert.True(result.Value > -1000, 
                    $"{result.Key} should achieve reasonable performance: {result.Value:F4}");
            }

            // At least one should achieve decent performance
            var bestFitness = crossoverResults.Values.Max();
            Assert.True(bestFitness > -100, 
                $"Best crossover should achieve good fitness: {bestFitness:F4}");
        }

        /// <summary>
        /// Compares different tree creation methods
        /// </summary>
        [Fact]
        public async Task Diabetes_TreeCreatorComparison_ShouldShowVariation()
        {
            // Arrange
            var (inputs, targets, variableNames) = await DatasetManager.GetDiabetesDatasetAsync();
            
            int sampleSize = Math.Min(40, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);

            var creatorResults = new Dictionary<string, (double fitness, int avgTreeSize)>();

            // Test GrowTreeCreator
            var growAlgorithm = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 123);
            growAlgorithm.TreeCreator = new GrowTreeCreator();
            growAlgorithm.Run();
            var growAvgSize = growAlgorithm.Population.Average(ind => ind.Length);
            creatorResults["GrowTreeCreator"] = (growAlgorithm.BestFitness, (int)growAvgSize);

            // Test FullTreeCreator
            var fullAlgorithm = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 123);
            fullAlgorithm.TreeCreator = new FullTreeCreator();
            fullAlgorithm.Run();
            var fullAvgSize = fullAlgorithm.Population.Average(ind => ind.Length);
            creatorResults["FullTreeCreator"] = (fullAlgorithm.BestFitness, (int)fullAvgSize);

            // Assert - Both should work but may have different characteristics
            foreach (var result in creatorResults)
            {
                Assert.True(result.Value.fitness > double.NegativeInfinity, 
                    $"{result.Key} should produce valid fitness");
                
                Assert.True(result.Value.avgTreeSize > 0, 
                    $"{result.Key} should create non-empty trees");
                
                Assert.True(result.Value.avgTreeSize < 100, 
                    $"{result.Key} should create reasonable sized trees: {result.Value.avgTreeSize}");
            }

            // FullTreeCreator typically creates larger initial trees
            Assert.True(creatorResults["FullTreeCreator"].avgTreeSize >= creatorResults["GrowTreeCreator"].avgTreeSize,
                "FullTreeCreator should typically create larger or equal sized trees");
        }

        /// <summary>
        /// Tests population size effects on algorithm performance
        /// </summary>
        [Fact]
        public async Task PopulationSizeComparison_ShouldShowScalability()
        {
            // Arrange
            var (inputs, targets, variableNames) = await DatasetManager.GetWineQualityDatasetAsync();
            
            int sampleSize = Math.Min(30, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);

            var populationResults = new Dictionary<int, (double fitness, TimeSpan duration)>();
            var populationSizes = new[] { 10, 20, 30 };

            foreach (var popSize in populationSizes)
            {
                var algorithm = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 456);
                algorithm.PopulationSize = popSize;
                algorithm.MaxGenerations = 5; // Keep generations low for speed

                var startTime = DateTime.Now;
                algorithm.Run();
                var duration = DateTime.Now - startTime;

                populationResults[popSize] = (algorithm.BestFitness, duration);
            }

            // Assert - All population sizes should work
            foreach (var result in populationResults)
            {
                Assert.True(result.Value.fitness > double.NegativeInfinity, 
                    $"Population size {result.Key} should produce valid results");
                
                Assert.True(result.Value.duration.TotalSeconds < 30, 
                    $"Population size {result.Key} should complete in reasonable time: {result.Value.duration.TotalSeconds:F2}s");
            }

            // Larger populations should generally take longer
            Assert.True(populationResults[30].duration >= populationResults[10].duration,
                "Larger populations should take same or more time");
        }

        /// <summary>
        /// Tests different grammar configurations for expressiveness
        /// </summary>
        [Fact]
        public async Task GrammarComplexityComparison_ShouldShowExpressiveness()
        {
            // Arrange
            var (inputs, targets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            
            int sampleSize = Math.Min(25, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);
            var grammarResults = new Dictionary<string, double>();

            // Simple grammar - only addition and subtraction
            var simpleGrammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction
            }, allowConstants: false);

            var simpleAlgorithm = CreateStandardAlgorithm(simpleGrammar, evaluator, new SubtreeCrossover(), 789);
            simpleAlgorithm.Run();
            grammarResults["Simple"] = simpleAlgorithm.BestFitness;

            // Standard grammar - basic math operations
            var standardGrammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            var standardAlgorithm = CreateStandardAlgorithm(standardGrammar, evaluator, new SubtreeCrossover(), 789);
            standardAlgorithm.Run();
            grammarResults["Standard"] = standardAlgorithm.BestFitness;

            // Complex grammar - includes statistical operations
            var complexGrammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision,
                StatisticsSymbols.Mean,
                StatisticsSymbols.Variance
            }, allowConstants: true);

            var complexAlgorithm = CreateStandardAlgorithm(complexGrammar, evaluator, new SubtreeCrossover(), 789);
            complexAlgorithm.Run();
            grammarResults["Complex"] = complexAlgorithm.BestFitness;

            // Assert - All grammars should produce valid results
            foreach (var result in grammarResults)
            {
                Assert.True(result.Value > double.NegativeInfinity, 
                    $"{result.Key} grammar should produce valid fitness: {result.Value:F4}");
            }

            // More complex grammars should generally perform same or better
            Assert.True(grammarResults["Standard"] >= grammarResults["Simple"], 
                "Standard grammar should perform at least as well as simple grammar");
        }

        /// <summary>
        /// Stress test with maximum configuration
        /// </summary>
        [Fact]
        public async Task MaximumConfiguration_StressTest_ShouldHandleLargeScale()
        {
            // Arrange - Use largest available dataset
            var (inputs, targets, variableNames) = await DatasetManager.GetWineQualityDatasetAsync();

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
                PopulationSize = 100,  // Large population
                MaxGenerations = 20,   // Many generations
                FitnessEvaluator = evaluator
            };

            int generationsCompleted = 0;
            algorithm.GenerationCompleted += (s, e) => generationsCompleted++;

            // Act
            var startTime = DateTime.Now;
            algorithm.Run();
            var duration = DateTime.Now - startTime;

            // Assert - Should complete successfully even with large configuration
            Assert.Equal(20, generationsCompleted);
            Assert.NotNull(algorithm.BestIndividual);
            Assert.True(algorithm.BestFitness > double.NegativeInfinity);
            Assert.Equal(100, algorithm.Population.Count);
            
            // Should complete within reasonable time (2 minutes)
            Assert.True(duration.TotalMinutes < 2, 
                $"Stress test should complete within 2 minutes: {duration.TotalMinutes:F2}m");
        }

        /// <summary>
        /// Helper method to create standardized algorithm configuration
        /// </summary>
        private GeneticProgrammingAlgorithm CreateStandardAlgorithm(
            SymbolicRegressionGrammar grammar, 
            IFitnessEvaluator evaluator, 
            ISymbolicExpressionTreeCrossover crossover,
            int seed)
        {
            return new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = crossover,
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(seed),
                PopulationSize = 20,
                MaxGenerations = 6,
                FitnessEvaluator = evaluator
            };
        }

        /// <summary>
        /// Test random seed reproducibility
        /// </summary>
        [Fact]
        public async Task SameRandomSeed_ShouldProduceIdenticalResults()
        {
            // Arrange
            var (inputs, targets, variableNames) = await DatasetManager.GetDiabetesDatasetAsync();
            
            int sampleSize = Math.Min(20, inputs.Length);
            var sampleInputs = new double[sampleSize][];
            var sampleTargets = new double[sampleSize];
            Array.Copy(inputs, sampleInputs, sampleSize);
            Array.Copy(targets, sampleTargets, sampleSize);

            var grammar = new SymbolicRegressionGrammar(variableNames);
            var evaluator = new RegressionFitnessEvaluator(sampleInputs, sampleTargets, variableNames);

            // Run 1
            var algorithm1 = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 12345);
            algorithm1.Run();

            // Run 2 with same seed
            var algorithm2 = CreateStandardAlgorithm(grammar, evaluator, new SubtreeCrossover(), 12345);
            algorithm2.Run();

            // Assert - Should produce identical results
            Assert.Equal(algorithm1.BestFitness, algorithm2.BestFitness);
            Assert.Equal(algorithm1.BestIndividual.Length, algorithm2.BestIndividual.Length);
        }
    }
}
