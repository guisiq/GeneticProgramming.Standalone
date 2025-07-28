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
    /// Tests for the ChangeNodeTypeMutator verifying node replacement logic.
    /// </summary>
    public class NodeTypeChangeMutatorTests
    {
        private static System.Collections.Generic.List<System.Type> GetNodeTypes(ISymbolicExpressionTree tree)
        {
            return tree.IterateNodesPostfix().Select(n => n.Symbol.GetType()).ToList();
        }

        /// <summary>
        /// The mutator should replace at least one node with a symbol of a different type when compatible symbols exist.
        /// </summary>
        [Fact]
        public void Mutate_WithCompatibleSymbols_ReplacesNodeType()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var original = creator.CreateTree(new MersenneTwister(1), grammar, 15, 4);
            var mutator = new ChangeNodeTypeMutator { SymbolicExpressionTreeGrammar = grammar };

            // Try multiple mutations to ensure at least one change occurs
            ISymbolicExpressionTree mutated = original;
            var before = GetNodeTypes(original);
            System.Collections.Generic.List<System.Type> after = before;
            
            // Try up to 20 times to get a mutation that changes node types
            bool mutationOccurred = false;
            for (int attempt = 0; attempt < 20; attempt++)
            {
                mutated = mutator.Mutate(new MersenneTwister(2 + attempt), original);
                after = GetNodeTypes(mutated);
                
                if (!before.SequenceEqual(after))
                {
                    mutationOccurred = true;
                    break; // Found a successful mutation
                }
            }

            Assert.Equal(before.Count, after.Count);
            Assert.True(mutationOccurred, "Expected at least one mutation to occur within 20 attempts");
        }

        /// <summary>
        /// The mutator should return the original tree when no compatible symbols are available.
        /// </summary>
        [Fact]
        public void Mutate_WithNoCompatibleSymbols_ReturnsOriginal()
        {
            // Create a minimal grammar with only one symbol type
            var grammar = new SymbolicRegressionGrammar();
            
            // Disable all symbols except constants to force no compatible alternatives
            foreach (var symbol in grammar.Symbols.ToList())
            {
                if (!(symbol is Constant))
                {
                    symbol.Enabled = false;
                }
            }

            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            
            // Create a simple tree with only constants
            var original = new SymbolicExpressionTree(new ConstantTreeNode(new Constant(), 5.0));
            var mutator = new ChangeNodeTypeMutator { SymbolicExpressionTreeGrammar = grammar };

            var mutated = mutator.Mutate(new MersenneTwister(42), original);

            var before = GetNodeTypes(original);
            var after = GetNodeTypes(mutated);

            // Should be identical since no alternative symbols are available
            Assert.Equal(before.Count, after.Count);
            Assert.Equal(before, after);
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
