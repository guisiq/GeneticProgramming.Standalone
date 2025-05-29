using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Core;
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.Tests
{
    public class SymbolicExpressionTreeNodeTests
    {
        /// <summary>
        /// Tests that the SymbolicExpressionTreeNode constructor initializes correctly with a valid symbol.
        /// </summary>
        [Fact]
        public void Constructor_WithValidSymbol_InitializesCorrectly()
        {
            // Arrange
            var symbol = new Addition(); // Using a concrete, non-terminal symbol for testing

            // Act
            var node = new SymbolicExpressionTreeNode(symbol);

            // Assert
            Assert.Same(symbol, node.Symbol);
            Assert.NotNull(node.Subtrees);
            Assert.Empty(node.Subtrees);
            Assert.NotNull(node.Id); // Ensure an ID is generated (from base Item class)
            Assert.Null(node.Parent); // Parent should be null initially
        }
    }
}
