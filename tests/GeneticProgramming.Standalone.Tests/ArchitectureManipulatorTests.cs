using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests for node insertion and removal manipulators.
    /// </summary>
    public class ArchitectureManipulatorTests
    {
        [Fact]
        public void NodeInsertion_IncreasesTreeLength()
        {
            var grammar = new SymbolicRegressionGrammar();
            var add = (Addition)grammar.GetSymbol("Add")!;
            var constSym = (Constant)grammar.GetSymbol("Constant")!;
            var root = new SymbolicExpressionTreeNode(add);
            root.AddSubtree(new ConstantTreeNode(constSym, 1));
            root.AddSubtree(new ConstantTreeNode(constSym, 2));
            var tree = new SymbolicExpressionTree(root);

            var inserter = new NodeInsertionManipulator
            {
                SymbolicExpressionTreeGrammar = grammar,
                MaxTreeLength = 3,
                MaxTreeDepth = 2
            };

            var mutated = inserter.Mutate(new MersenneTwister(1), tree);

            Assert.True(mutated.Length > tree.Length);
            Assert.NotNull(mutated.Root);
        }

        [Fact]
        public void NodeRemoval_DecreasesTreeLength()
        {
            var grammar = new SymbolicRegressionGrammar();
            var add = (Addition)grammar.GetSymbol("Add")!;
            var constSym = (Constant)grammar.GetSymbol("Constant")!;
            var root = new SymbolicExpressionTreeNode(add);
            root.AddSubtree(new ConstantTreeNode(constSym, 1));
            root.AddSubtree(new ConstantTreeNode(constSym, 2));
            var tree = new SymbolicExpressionTree(root);

            var remover = new NodeRemovalManipulator();
            var mutated = remover.Mutate(new MersenneTwister(2), tree);

            Assert.True(mutated.Length < tree.Length);
        }
    }
}
