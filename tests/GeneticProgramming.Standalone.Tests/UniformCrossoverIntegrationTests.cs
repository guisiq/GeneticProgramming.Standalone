using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Integration tests for the UniformCrossover operator.
    /// </summary>
    public class UniformCrossoverIntegrationTests
    {
        private readonly SymbolicRegressionGrammar _grammar = new SymbolicRegressionGrammar();
        private readonly GrowTreeCreator _creator = new GrowTreeCreator();

        private static bool ConformsToGrammar(ISymbolicExpressionTree tree, ISymbolicExpressionTreeGrammar grammar)
        {
            if (!grammar.StartSymbols.Contains(tree.Root.Symbol))
                return false;

            foreach (var node in tree.IterateNodesPostfix())
            {
                if (node.Parent == null) continue;
                var parent = node.Parent;
                int index = parent.IndexOfSubtree(node);
                if (!grammar.IsAllowedChildSymbol(parent.Symbol, node.Symbol, index))
                    return false;
            }
            return true;
        }

        private static List<string> GetSymbolNames(ISymbolicExpressionTree tree)
            => tree.IterateNodesPostfix().Select(n => n.Symbol.Name).ToList();

        [Fact]
        public void UniformCrossover_ProducesValidOffspring()
        {
            var random = new MersenneTwister(5);
            var parent1 = _creator.CreateTree(random, _grammar, 20, 5);
            var parent2 = _creator.CreateTree(random, _grammar, 20, 5);
            var crossover = new UniformCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(random, parent1, parent2);

            Assert.True(ConformsToGrammar(offspring, _grammar));
        }

        [Fact]
        public void UniformCrossover_OffspringIsDifferentFromParents()
        {
            var random = new MersenneTwister(7);
            var parent1 = _creator.CreateTree(random, _grammar, 20, 5);
            var parent2 = _creator.CreateTree(random, _grammar, 20, 5);
            var crossover = new UniformCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(random, parent1, parent2);
            var symbols1 = GetSymbolNames(parent1);
            var symbols2 = GetSymbolNames(parent2);
            var offspringSymbols = GetSymbolNames(offspring);

            Assert.NotEqual(symbols1, offspringSymbols);
            Assert.NotEqual(symbols2, offspringSymbols);
        }
    }
}
