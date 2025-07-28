using GeneticProgramming.Core;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests that SubtreeMutator generates subtrees respecting limits.
    /// </summary>
    public class SubtreeGenerationTests
    {
        /// <summary>
        /// Generated subtree must not exceed specified depth and length.
        /// </summary>
        [Fact]
        public void Mutate_RespectsGenerationLimits()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var tree = creator.CreateTree(new MersenneTwister(5), grammar, 10, 3);

            var mutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = grammar, MaxTreeLength = 4, MaxTreeDepth = 2 };
            var mutated = mutator.Mutate(new MersenneTwister(6), tree);

            Assert.True(mutated.Length <= 4);
            Assert.True(mutated.Depth <= 2);
        }
    }
}
