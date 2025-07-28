using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System.Collections.Generic;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Abstractions.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Algorithms
{
    /// <summary>
    /// Tests for selection operators.
    /// </summary>
    public class SelectionLogicTests
    {
        [Fact]
        public void TournamentSelector_ReturnsMemberOfPopulation()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var random = new MersenneTwister(7);

            var population = new List<ISymbolicExpressionTree>();
            for (int i = 0; i < 4; i++)
                population.Add(creator.CreateTree(random, grammar, 5, 3));

            var selector = new TournamentSelector { TournamentSize = 2 };
            var selected = selector.Select(random, population, t => -t.Length);

            Assert.Contains(population, t => t.Root.Symbol.GetType() == selected.Root.Symbol.GetType());
        }

        private class CountingSelector : ISymbolicExpressionTreeSelector
        {
            public int CallCount { get; private set; }
            public GeneticProgramming.Abstractions.Parameters.IParameterCollection? Parameters => null;
            public ISymbolicExpressionTree Select(IRandom random, IList<ISymbolicExpressionTree> population, System.Func<ISymbolicExpressionTree, double> fitness)
            {
                CallCount++;
                return population[0];
            }
        }

        [Fact]
        public void Algorithm_UsesProvidedSelector()
        {
            var selector = new CountingSelector();
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = selector,
                Random = new MersenneTwister(5),
                PopulationSize = 5,
                MaxGenerations = 2
            };

            algorithm.Run();

            Assert.True(selector.CallCount > 0);
        }
    }
}
