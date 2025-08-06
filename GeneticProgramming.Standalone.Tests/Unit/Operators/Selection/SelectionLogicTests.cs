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
            var creator = new GrowTreeCreator<double> { SymbolicExpressionTreeGrammar = grammar };
            var random = new MersenneTwister(7);

            var population = new List<ISymbolicExpressionTree<double>>();
            for (int i = 0; i < 4; i++)
                population.Add(creator.CreateTree(random, grammar, 5, 3));

            var selector = new TournamentSelector { TournamentSize = 2 };
            var selected = selector.Select<double>(random, population, t => -(double)t.Length);

            Assert.Contains(population, t => t.Root.Symbol.GetType() == selected.Root.Symbol.GetType());
        }

        private class CountingSelector : ISymbolicExpressionTreeSelector
        {
            public int CallCount { get; private set; }
            public GeneticProgramming.Abstractions.Parameters.IParameterCollection? Parameters => null;
            public ISymbolicExpressionTree<T> Select<T>(IRandom random, IList<ISymbolicExpressionTree<T>> population, System.Func<ISymbolicExpressionTree<T>, T> fitness)
                where T : struct, IComparable<T>, IEquatable<T>
            {
                CallCount++;
                return population[0];
            }
        }

        [Fact]
        public void Algorithm_UsesProvidedSelector()
        {
            var selector = new CountingSelector();
            var algorithm = new GeneticProgrammingAlgorithm<double>
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator<double>(),
                Crossover = new SubtreeCrossover<double>(),
                Mutator = new SubtreeMutator<double>(),
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
