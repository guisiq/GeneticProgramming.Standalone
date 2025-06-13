using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests that crossover exchanges subtrees and keeps structure valid.
    /// </summary>
    public class SubtreeExchangeTests
    {
        /// <summary>
        /// Offspring should contain nodes originating from the second parent and maintain parent links.
        /// </summary>
        [Fact]
        public void Crossover_ExchangesSubtreesAndMaintainsParents()
        {
            var grammar = new SymbolicRegressionGrammar();
            var add = (Addition)grammar.GetSymbol("Add")!;
            var constSym = (Constant)grammar.GetSymbol("Constant")!;

            var p1Root = new SymbolicExpressionTreeNode(add);
            p1Root.AddSubtree(new ConstantTreeNode(constSym, 1));
            p1Root.AddSubtree(new ConstantTreeNode(constSym, 2));
            var parent1 = new SymbolicExpressionTree(p1Root);

            var p2Root = new SymbolicExpressionTreeNode(add);
            var unique = new ConstantTreeNode(constSym, 42);
            p2Root.AddSubtree(unique);
            p2Root.AddSubtree(new ConstantTreeNode(constSym, 5));
            var parent2 = new SymbolicExpressionTree(p2Root);

            var crossover = new SubtreeCrossover { SymbolicExpressionTreeGrammar = grammar, InternalNodeProbability = 0.0 };
            var offspring = crossover.Crossover(new MersenneTwister(3), parent1, parent2);

            // Offspring should contain the unique node value from parent2 somewhere
            var values = offspring.IterateNodesPostfix().OfType<ConstantTreeNode>().Select(n => n.Value).ToList();
            Assert.Contains(42, values);

            // Validate parent-child links
            foreach (var node in offspring.IterateNodesPostfix())
            {
                foreach (var child in node.Subtrees)
                {
                    Assert.Same(node, child.Parent);
                }
            }
        }
    }
}
