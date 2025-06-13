using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests selection logic for nodes mutated by SubtreeMutator.
    /// </summary>
    public class MutationNodeSelectionTests
    {
        /// <summary>
        /// When the tree contains only a root node it should be replaced entirely.
        /// </summary>
        [Fact]
        public void Mutate_SingleNodeTree_ReplacesRoot()
        {
            var grammar = new SymbolicRegressionGrammar();
            var constSym = (Constant)grammar.GetSymbol("Constant")!;
            var tree = new SymbolicExpressionTree(new ConstantTreeNode(constSym, 1));

            var mutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = grammar, MaxTreeDepth = 3, MaxTreeLength = 5 };
            var mutated = mutator.Mutate(new MersenneTwister(4), tree);

            Assert.NotSame(tree.Root, mutated.Root);
            Assert.True(mutated.Length <= 5);
            Assert.True(mutated.Depth <= 3);
        }
    }
}
