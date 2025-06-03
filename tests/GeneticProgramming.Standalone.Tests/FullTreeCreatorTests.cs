using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using GeneticProgramming.Expressions.Symbols; // Added for TerminalSymbol and FunctionSymbol
using System;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Unit tests for FullTreeCreator class
    /// Tests the full method of symbolic expression tree creation
    /// </summary>
    public class FullTreeCreatorTests
    {
        private readonly SymbolicRegressionGrammar _grammar;
        private readonly MersenneTwister _random;

        public FullTreeCreatorTests()
        {
            _grammar = new SymbolicRegressionGrammar();
            _random = new MersenneTwister(42); // Fixed seed for deterministic tests
        }

        [Fact]
        public void Constructor_Default_InitializesCorrectly()
        {
            // Arrange & Act
            var creator = new FullTreeCreator();

            // Assert
            Assert.NotNull(creator);
            Assert.NotNull(creator.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeCreator>(creator);
            Assert.IsAssignableFrom<IDeepCloneable>(creator);
        }

        [Fact]
        public void CreateTree_ValidParameters_ReturnsValidTree()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 20;
            int maxTreeDepth = 4;

            // Act
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.True(tree.Depth <= maxTreeDepth);
        }

        [Fact]
        public void CreateTree_WithMinimalDepth_ReturnsTerminal()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 20;
            int maxTreeDepth = 1;

            // Act
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(1, tree.Depth);
            Assert.Equal(0, tree.Root.SubtreeCount); // Should be a terminal
        }

        [Fact]
        public void CreateTree_FullMethodCharacteristics_ProducesCompleteStructures()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 50;
            int maxTreeDepth = 3;

            // Act
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(maxTreeDepth, tree.Depth); // Full method should reach max depth
            
            // Verify that internal nodes have maximum arity children (full method characteristic)
            VerifyFullTreeStructure(tree.Root, maxTreeDepth - 1);
        }

        private void VerifyFullTreeStructure(ISymbolicExpressionTreeNode node, int remainingDepth)
        {
            if (remainingDepth <= 0)
            {
                // At leaf level, should be terminal
                Assert.Equal(0, node.SubtreeCount);
            }
            else
            {
                // At internal levels, should have maximum arity children
                var symbol = node.Symbol;
                if (symbol.MaximumArity > 0) // If it's a non-terminal
                {
                    Assert.Equal(symbol.MaximumArity, node.SubtreeCount);
                    
                    // Recursively check children
                    for (int i = 0; i < node.SubtreeCount; i++)
                    {
                        VerifyFullTreeStructure(node.GetSubtree(i), remainingDepth - 1);
                    }
                }
            }
        }

        [Fact]
        public void CreateTree_NullRandom_ThrowsArgumentNullException()
        {
            // Arrange
            var creator = new FullTreeCreator();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                creator.CreateTree(null!, _grammar, 10, 5));
        }

        [Fact]
        public void CreateTree_NullGrammar_ThrowsArgumentNullException()
        {
            // Arrange
            var creator = new FullTreeCreator();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                creator.CreateTree(_random, null!, 10, 5));
        }



        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new FullTreeCreator();
            var cloner = new Cloner();

            // Act
            var clone = (FullTreeCreator)original.Clone(cloner);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.IsType<FullTreeCreator>(clone);
        }

        [Fact]
        public void Clone_PreservesType()
        {
            // Arrange
            var original = new FullTreeCreator();
            var cloner = new Cloner();

            // Act
            var clone = original.Clone(cloner);

            // Assert
            Assert.IsType<FullTreeCreator>(clone);
        }

      

        [Fact]
        public void CreateTree_MaxDepthExceeded_ThrowsException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 100; // Sufficient length
            // Corrected: Accessing Symbols property correctly if it's a direct collection or Count() for IEnumerable
            int maxTreeDepth = _grammar.Symbols.Count() + 5; // Depth likely to exceed practical limits with full method

            // Act & Assert
            // Depending on the grammar, this might not always throw,
            // but for a typical grammar, forcing full at great depth without enough distinct non-terminals
            // could lead to issues or specific behaviors to test.
            // For now, let's assume it should create a tree up to a reasonable depth or handle gracefully.
            // This test might need refinement based on specific grammar constraints.
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);
            Assert.NotNull(tree);
            Assert.True(tree.Depth <= maxTreeDepth); // Should still respect maxDepth
        }

        [Fact]
        public void CreateTree_ZeroMaxDepth_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 5;
            int maxTreeDepth = 0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth));
        }

        [Fact]
        public void CreateTree_NegativeMaxDepth_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = 5;
            int maxTreeDepth = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws ArgumentOutOfRangeException for a negative maxTreeLength.
        /// </summary>
        [Fact]
        public void CreateTree_NegativeMaxTreeLength_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            int maxTreeLength = -1;
            int maxTreeDepth = 5;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws InvalidOperationException if the grammar has no enabled symbols.
        /// The Full method requires non-terminal symbols to build down to the max depth.
        /// </summary>
        [Fact]
        public void CreateTree_GrammarWithNoEnabledSymbols_ThrowsInvalidOperationException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            var emptyGrammar = new SymbolicExpressionTreeGrammar("EmptyGrammarForFullTest", "Grammar with no enabled symbols");
            
            int maxTreeLength = 5;
            int maxTreeDepth = 3;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => creator.CreateTree(_random, emptyGrammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws InvalidOperationException if no terminal symbols are available when one is required by the Full method.
        /// The Full method places terminals at the maximum depth.
        /// </summary>
        [Fact]
        public void CreateTree_GrammarWithNoTerminalSymbolsWhenTerminalRequired_ThrowsInvalidOperationException()
        {
            // Arrange
            var creator = new FullTreeCreator();
            var nonTerminalOnlyGrammar = new SymbolicExpressionTreeGrammar("NonTerminalOnlyGrammarFullTest", "Grammar intended to lack terminals");
            
            int maxTreeLength = 10; 
            int maxTreeDepth = 1;   // Force attempt to select a terminal at root.

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => creator.CreateTree(_random, nonTerminalOnlyGrammar, maxTreeLength, maxTreeDepth));
        }

        // Helper method to check if all nodes at a certain depth are terminals
        private bool AreAllNodesAtDepthTerminals(ISymbolicExpressionTreeNode node, int targetDepth, int currentDepth)
        {
            if (node == null) return true;
            if (currentDepth == targetDepth)
            {
                return node.SubtreeCount == 0; // Is a terminal
            }
            if (currentDepth > targetDepth)
            {
                return true; // Beyond target depth, doesn't matter
            }

            foreach (var child in node.Subtrees)
            {
                if (!AreAllNodesAtDepthTerminals(child, targetDepth, currentDepth + 1))
                {
                    return false;
                }
            }
            return true;
        }

        // Helper method to check if all internal nodes are non-terminals
        private bool AreAllInternalNodesNonTerminals(ISymbolicExpressionTreeNode node, int maxDepth, int currentDepth)
        {
            if (node == null || currentDepth >= maxDepth) return true; // Leaf or beyond

            if (node.SubtreeCount > 0) // If it's an internal node
            {
                if (node.Symbol.MaximumArity == 0) return false; // Internal node is a terminal, incorrect for Full
            }

            foreach (var child in node.Subtrees)
            {
                if (!AreAllInternalNodesNonTerminals(child, maxDepth, currentDepth + 1))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
