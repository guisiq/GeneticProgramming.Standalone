using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System;
using Xunit;

namespace GeneticProgramming.Standalone.Tests
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

            Assert.Equal(7, eventCount);
        }
    }
}
