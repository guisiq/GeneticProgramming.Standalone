using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests for the ChangeNodeTypeMutator verifying node replacement logic.
    /// </summary>
    public class NodeTypeChangeMutatorTests
    {
        private static System.Collections.Generic.List<System.Type> GetNodeTypes(ISymbolicExpressionTree tree)
        {
            return tree.IterateNodesPostfix().Select(n => n.Symbol.GetType()).ToList();
        }

        /// <summary>
        /// The mutator should replace at least one node with a symbol of a different type when possible.
        /// </summary>
        [Fact]
        public void Mutate_ReplacesNodeType()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var original = creator.CreateTree(new MersenneTwister(1), grammar, 10, 3);
            var mutator = new ChangeNodeTypeMutator { SymbolicExpressionTreeGrammar = grammar };

            var mutated = mutator.Mutate(new MersenneTwister(2), original);

            var before = GetNodeTypes(original);
            var after = GetNodeTypes(mutated);

            Assert.Equal(before.Count, after.Count);
            Assert.NotEqual(before, after);
        }

        /// <summary>
        /// Child subtrees should remain attached and arity preserved after mutation.
        /// </summary>
        [Fact]
        public void Mutate_PreservesStructure()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var original = creator.CreateTree(new MersenneTwister(3), grammar, 8, 4);
            var arities = original.IterateNodesPostfix().Select(n => n.SubtreeCount).ToList();
            var mutator = new ChangeNodeTypeMutator { SymbolicExpressionTreeGrammar = grammar };

            var mutated = mutator.Mutate(new MersenneTwister(4), original);
            var mutatedArities = mutated.IterateNodesPostfix().Select(n => n.SubtreeCount).ToList();

            Assert.Equal(arities, mutatedArities);
        }
    }
}
