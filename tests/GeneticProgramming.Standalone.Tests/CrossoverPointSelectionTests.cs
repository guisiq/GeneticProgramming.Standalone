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
    /// Tests internal crossover selection logic via observable effects on the offspring tree.
    /// </summary>
    public class CrossoverPointSelectionTests
    {
        /// <summary>
        /// With internal node probability set to 1.0 the root node should be replaced.
        /// </summary>
        [Fact]
        public void InternalNodeProbabilityOne_ReplacesRoot()
        {
            var grammar = new SymbolicRegressionGrammar();
            var add = grammar.GetSymbol("Add")!;
            var constSym = (Constant)grammar.GetSymbol("Constant")!;

            var parent1Root = new SymbolicExpressionTreeNode(add);
            parent1Root.AddSubtree(new ConstantTreeNode(constSym, 1));
            parent1Root.AddSubtree(new ConstantTreeNode(constSym, 2));
            var parent1 = new SymbolicExpressionTree(parent1Root);

            var parent2Root = new SymbolicExpressionTreeNode(add);
            parent2Root.AddSubtree(new ConstantTreeNode(constSym, 99));
            parent2Root.AddSubtree(new ConstantTreeNode(constSym, 100));
            var parent2 = new SymbolicExpressionTree(parent2Root);

            var crossover = new SubtreeCrossover
            {
                SymbolicExpressionTreeGrammar = grammar,
                InternalNodeProbability = 1.0
            };

            var offspring = crossover.Crossover(new MersenneTwister(1), parent1, parent2);

            var values = offspring.IterateNodesPostfix().OfType<ConstantTreeNode>().Select(n => n.Value).ToList();
            Assert.Contains(values, v => v == 99 || v == 100);
        }

        /// <summary>
        /// Even when a tree consists only of a terminal node, crossover should replace it with the donor root.
        /// </summary>
        [Fact]
        public void TerminalTree_StillReplaced()
        {
            var grammar = new SymbolicRegressionGrammar();
            var constSym = (Constant)grammar.GetSymbol("Constant")!;
            var tree1 = new SymbolicExpressionTree(new ConstantTreeNode(constSym, 1));
            var tree2 = new SymbolicExpressionTree(new ConstantTreeNode(constSym, 2));

            var crossover = new SubtreeCrossover
            {
                SymbolicExpressionTreeGrammar = grammar,
                InternalNodeProbability = 1.0
            };

            var offspring = crossover.Crossover(new MersenneTwister(2), tree1, tree2);

            Assert.NotSame(tree1.Root, offspring.Root);
            Assert.Equal(1, offspring.Length);
        }
    }
}
