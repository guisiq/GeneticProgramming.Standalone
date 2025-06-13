using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    public class TreeCreatorNodeBuildingTests
    {
        private static void VerifyNode(ISymbolicExpressionTreeNode node)
        {
            Assert.InRange(node.SubtreeCount, node.Symbol.MinimumArity, node.Symbol.MaximumArity);
            foreach (var child in node.Subtrees)
            {
                Assert.Same(node, child.Parent);
                VerifyNode(child);
            }
        }

        /// <summary>
        /// GrowTreeCreator should build trees whose nodes respect the arity of their symbols.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GrowTreeCreator_RespectsArity(bool allowDivision)
        {
            var grammar = new SymbolicRegressionGrammar(new[] { "X" }, allowConstants: true, allowDivision: allowDivision);
            var creator = new GrowTreeCreator();
            var random = new MersenneTwister(42);

            var tree = creator.CreateTree(random, grammar, 30, 5);
            VerifyNode(tree.Root!);
        }

        /// <summary>
        /// FullTreeCreator must also honor symbol arity and maintain parent-child links.
        /// </summary>
        [Fact]
        public void FullTreeCreator_RespectsArity()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            var creator = new FullTreeCreator();
            var random = new MersenneTwister(99);

            var tree = creator.CreateTree(random, grammar, 20, 4);
            VerifyNode(tree.Root!);
        }
    }
}
