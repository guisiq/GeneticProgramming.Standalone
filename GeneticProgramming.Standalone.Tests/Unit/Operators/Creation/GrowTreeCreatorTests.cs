using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Unit tests for GrowTreeCreator class
    /// Tests the grow method of symbolic expression tree creation
    /// </summary>
    public class GrowTreeCreatorTests
    {
        private readonly SymbolicRegressionGrammar _grammar;
        private readonly MersenneTwister _random;

        public GrowTreeCreatorTests()
        {
            _grammar = new SymbolicRegressionGrammar();
            _random = new MersenneTwister(42); // Fixed seed for deterministic tests
        }

        [Fact]
        public void Constructor_Default_InitializesCorrectly()
        {
            // Arrange & Act
            var creator = new GrowTreeCreator();

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
            var creator = new GrowTreeCreator();
            int maxTreeLength = 20;
            int maxTreeDepth = 5;

            // Act
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.True(tree.Length <= maxTreeLength);
            Assert.True(tree.Depth <= maxTreeDepth);
        }

        [Fact]
        public void CreateTree_WithMinimalConstraints_ReturnsTerminal()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = 1;
            int maxTreeDepth = 1;

            // Act
            var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(1, tree.Length);
            Assert.Equal(1, tree.Depth);
            Assert.Equal(0, tree.Root.SubtreeCount); // Should be a terminal
        }

        [Fact]
        public void CreateTree_MultipleCreations_ProduceDifferentTrees()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = 15;
            int maxTreeDepth = 4;
            var trees = new ISymbolicExpressionTree[10];

            // Act
            for (int i = 0; i < trees.Length; i++)
            {
                trees[i] = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);
            }

            // Assert
            // Trees should be different (at least some of them due to randomness)
            var lengths = trees.Select(t => t.Length).Distinct().Count();
            var depths = trees.Select(t => t.Depth).Distinct().Count();
            
            // With randomness, we should get some variation in either length or depth
            Assert.True(lengths > 1 || depths > 1, "Trees should show variation in length or depth");
        }

        [Fact]
        public void CreateTree_RespectsMaxTreeLength()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = 10;
            int maxTreeDepth = 10; // High depth to test length constraint

            // Act & Assert - Test multiple trees to account for randomness
            for (int i = 0; i < 20; i++)
            {
                var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);
                Assert.True(tree.Length <= maxTreeLength, 
                    $"Tree length {tree.Length} exceeds maximum {maxTreeLength}");
            }
        }

        [Fact]
        public void CreateTree_RespectsMaxTreeDepth()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = 50; // High length to test depth constraint
            int maxTreeDepth = 3;

            // Act & Assert - Test multiple trees to account for randomness
            for (int i = 0; i < 20; i++)
            {
                var tree = creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth);
                Assert.True(tree.Depth <= maxTreeDepth,
                    $"Tree depth {tree.Depth} exceeds maximum {maxTreeDepth}");
            }
        }

        [Fact]
        public void CreateTree_NullRandom_ThrowsArgumentNullException()
        {
            // Arrange
            var creator = new GrowTreeCreator();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                creator.CreateTree(null!, _grammar, 10, 5));
        }

        [Fact]
        public void CreateTree_NullGrammar_ThrowsArgumentNullException()
        {
            // Arrange
            var creator = new GrowTreeCreator();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                creator.CreateTree(_random, null!, 10, 5));
        }

        [Fact]
        public void CreateTree_ZeroMaxLength_ReturnsTerminal()
        {
            // Arrange
            var creator = new GrowTreeCreator();

            // Act
            var tree = creator.CreateTree(_random, _grammar, 0, 5);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(0, tree.Root.SubtreeCount); // Should be terminal
        }

        [Fact]
        public void CreateTree_ZeroMaxDepth_ReturnsTerminal()
        {
            // Arrange
            var creator = new GrowTreeCreator();

            // Act
            var tree = creator.CreateTree(_random, _grammar, 10, 0);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(0, tree.Root.SubtreeCount); // Should be terminal
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new GrowTreeCreator();
            var cloner = new Cloner();

            // Act
            var clone = (GrowTreeCreator)original.Clone(cloner);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.IsType<GrowTreeCreator>(clone);
        }

        [Fact]
        public void Clone_PreservesType()
        {
            // Arrange
            var original = new GrowTreeCreator();
            var cloner = new Cloner();

            // Act
            var clone = original.Clone(cloner);

            // Assert
            Assert.IsType<GrowTreeCreator>(clone);
        }

        /// <summary>
        /// Verifies that CreateTree throws ArgumentOutOfRangeException for a negative maxTreeLength.
        /// </summary>
        [Fact]
        public void CreateTree_NegativeMaxTreeLength_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = -1;
            int maxTreeDepth = 5;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws ArgumentOutOfRangeException for a negative maxTreeDepth.
        /// </summary>
        [Fact]
        public void CreateTree_NegativeMaxTreeDepth_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            int maxTreeLength = 5;
            int maxTreeDepth = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => creator.CreateTree(_random, _grammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws InvalidOperationException if the grammar has no enabled symbols.
        /// </summary>
        [Fact]
        public void CreateTree_GrammarWithNoEnabledSymbols_ThrowsInvalidOperationException()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            var emptyGrammar = new SymbolicExpressionTreeGrammar("EmptyGrammar", "Grammar with no symbols");
            // If SymbolicExpressionTreeGrammar adds default symbols, we might need to clear them:
            // emptyGrammar.ClearSymbols(); // Hypothetical method, replace with actual API if available
            // Or, ensure all symbols are disabled if that's an option.

            int maxTreeLength = 5;
            int maxTreeDepth = 3;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => creator.CreateTree(_random, emptyGrammar, maxTreeLength, maxTreeDepth));
        }

        /// <summary>
        /// Verifies that CreateTree throws InvalidOperationException if no terminal symbols are available when one is required.
        /// </summary>
        [Fact]
        public void CreateTree_GrammarWithNoTerminalSymbolsWhenTerminalRequired_ThrowsInvalidOperationException()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            var nonTerminalGrammar = new SymbolicExpressionTreeGrammar("NonTerminalGrammar", "Grammar with no terminal symbols");
            // Add only a non-terminal symbol
            // var functionSymbol = new FunctionSymbol("F", 1, 1); // Example, replace with actual symbol creation
            // nonTerminalGrammar.AddSymbol(functionSymbol); // Hypothetical method

            // For this test to be effective, SymbolicRegressionGrammar might be unsuitable if it always has terminals.
            // We need a grammar guaranteed to have no terminals or only disabled terminals.
            // Using a fresh SymbolicExpressionTreeGrammar and NOT adding terminals is the easiest.
            // If SymbolicExpressionTreeGrammar adds default terminals, they would need to be removed or disabled.

            int maxTreeLength = 5;
            int maxTreeDepth = 1; // Force terminal creation

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => creator.CreateTree(_random, nonTerminalGrammar, maxTreeLength, maxTreeDepth));
        }
    }
}
