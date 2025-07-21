using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols; // Assuming ISymbol is in this namespace
using GeneticProgramming.Core; // Added for Item class and Id property
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.UnitTests.Expressions
{
    public class SymbolicExpressionTreeTests
    {

        /// <summary>
        /// Tests that the SymbolicExpressionTree constructor initializes correctly with a valid root node.
        /// </summary>
        [Fact]
        public void Constructor_WithValidRoot_InitializesCorrectly()
        {
            // Arrange
            var rootSymbol = new Addition(); // Using a concrete symbol for testing
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            // Act
            var tree = new SymbolicExpressionTree(rootNode);

            // Assert
            Assert.Same(rootNode, tree.Root);
            Assert.NotNull(tree.Id); // Ensure an ID is generated (from base Item class)
        }

        /// <summary>
        /// Tests that the SymbolicExpressionTree constructor throws ArgumentNullException when the root node is null.
        /// </summary>
        [Fact]
        public void Constructor_NullRoot_ThrowsArgumentNullException()
        {
            // Arrange
            ISymbolicExpressionTreeNode? rootNode = null; // Use nullable reference type

            // Act & Assert
            // The constructor calls the Root setter, which throws ArgumentNullException for a null value.
            // The parameter name in the exception will be "value" because of `nameof(value)` in the Root setter.
            var exception = Assert.Throws<ArgumentNullException>(() => new SymbolicExpressionTree(rootNode!)); // Use ! to satisfy compiler for this specific test case
            Assert.Equal("value", exception.ParamName); 
        }
    }
}
