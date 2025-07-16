using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System;
using Xunit;

//namespace GeneticProgramming.Standalone.Tests
namespace GeneticProgramming.Standalone.IntegrationTests.Algorithms

{
    public class GeneticProgrammingAlgorithmTests
    {
        [Fact]
        public void Constructor_InitializesDefaults()
        {
            var algorithm = new GeneticProgrammingAlgorithm();

            Assert.Equal(100, algorithm.PopulationSize);
            Assert.Equal(50, algorithm.MaxGenerations);
            Assert.Equal(25, algorithm.MaxTreeLength);
            Assert.Equal(10, algorithm.MaxTreeDepth);
            Assert.Equal(0.9, algorithm.CrossoverProbability);
            Assert.Equal(0.1, algorithm.MutationProbability);
            Assert.Null(algorithm.Grammar);
            Assert.Null(algorithm.TreeCreator);
            Assert.Null(algorithm.Crossover);
            Assert.Null(algorithm.Mutator);
            Assert.Null(algorithm.Random);
            Assert.Equal(0, algorithm.Generation);
            Assert.Empty(algorithm.Population);
            Assert.Null(algorithm.BestIndividual);
        }

        [Fact]
        public void Run_WithMinimalSetup_CompletesWithoutExceptions()
        {
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = 10,
                MaxGenerations = 2,
                MaxTreeDepth = 3,
                MaxTreeLength = 20
            };

            algorithm.Run();

            Assert.Equal(2, algorithm.Generation);
            Assert.Equal(10, algorithm.Population.Count);
            Assert.NotNull(algorithm.BestIndividual);
        }

        [Fact]
        public void PropertyChangedEvents_FireWhenParametersChange()
        {
            var algorithm = new GeneticProgrammingAlgorithm();
            int eventCount = 0;
            algorithm.PropertyChanged += (s, e) => eventCount++;

            algorithm.PopulationSize = 50;
            algorithm.MaxGenerations = 5;
            algorithm.MaxTreeLength = 10;
            algorithm.MaxTreeDepth = 5;
            algorithm.CrossoverProbability = 0.8;
            algorithm.MutationProbability = 0.2;
            algorithm.Random = new MersenneTwister(1);
            algorithm.Selector = new TournamentSelector();

            Assert.Equal(8, eventCount);
        }


        [Fact]
        public void Initialization_ConfiguresOperatorsCorrectly()
        {
            var grammar = new SymbolicRegressionGrammar();
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(1),
                PopulationSize = 5,
                MaxGenerations = 1
            };

            algorithm.Run();

            Assert.All(algorithm.Population, t => Assert.NotNull(t.Root));
            Assert.Equal(5, algorithm.Population.Count);
        }

        [Fact]
        public void Population_MaintainsSizeAcrossGenerations()
        {
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(2),
                PopulationSize = 6,
                MaxGenerations = 3,
                MaxTreeDepth = 3,
                MaxTreeLength = 15
            };

            algorithm.Run();

            Assert.Equal(3, algorithm.Generation);
            Assert.Equal(6, algorithm.Population.Count);
        }

        [Fact]
        public void GenerationCompletedEvent_IsRaisedDuringRun()
        {
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(3),
                PopulationSize = 5,
                MaxGenerations = 2
            };

            int eventCount = 0;
            algorithm.GenerationCompleted += (s, e) => eventCount++;

            algorithm.Run();

            Assert.Equal(2, eventCount);
        }

    }
}
