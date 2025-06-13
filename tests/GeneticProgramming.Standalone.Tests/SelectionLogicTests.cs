using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System.Reflection;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Algorithms
{
    /// <summary>
    /// Unit tests covering the tournament selection logic in the algorithm.
    /// </summary>
    public class SelectionLogicTests
    {
        private class TestAlgorithm : GeneticProgrammingAlgorithm
        {
            public void Prepare(ISymbolicExpressionTreeGrammar grammar, IRandom random)
            {
                Grammar = grammar;
                TreeCreator = new GrowTreeCreator();
                Crossover = new SubtreeCrossover();
                Mutator = new SubtreeMutator();
                Random = random;
                PopulationSize = 4;
                MaxGenerations = 1;
            }

            public void InvokeInitialize() =>
                typeof(GeneticProgrammingAlgorithm)
                    .GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(this, null);

            public ISymbolicExpressionTree InvokeTournamentSelection(int size) =>
                (ISymbolicExpressionTree)typeof(GeneticProgrammingAlgorithm)
                    .GetMethod("TournamentSelection", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(this, new object[] { size })!;
        }

        /// <summary>
        /// Tournament selection should return a tree taken from the current population.
        /// </summary>
        [Fact]
        public void TournamentSelection_ReturnsMemberOfPopulation()
        {
            var grammar = new SymbolicRegressionGrammar();
            var alg = new TestAlgorithm();
            alg.Prepare(grammar, new MersenneTwister(7));
            alg.InvokeInitialize();

            var selected = alg.InvokeTournamentSelection(2);

            Assert.Contains(alg.Population, t => t.Root.Symbol.GetType() == selected.Root!.Symbol.GetType());
        }
    }
}
