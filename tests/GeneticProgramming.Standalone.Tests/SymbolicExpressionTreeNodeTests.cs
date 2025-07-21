using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Core;
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.UnitTests.Expressions
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

        /// <summary>
        /// Tests that the SymbolicExpressionTreeNode constructor throws ArgumentNullException when the symbol is null.
        /// </summary>
        [Fact]
        public void Constructor_NullSymbol_ThrowsArgumentNullException()
        {
            // Arrange
            ISymbol? symbol = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new SymbolicExpressionTreeNode(symbol!));
            Assert.Equal("symbol", exception.ParamName);
        }

        /// <summary>
        /// Tests that AddSubtree correctly adds a child node to the subtrees collection and sets parent relationship.
        /// </summary>
        [Fact]
        public void AddSubtree_ValidChild_AddsToSubtrees()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);

            // Act
            parentNode.AddSubtree(childNode);

            // Assert
            Assert.Single(parentNode.Subtrees);
            Assert.Contains(childNode, parentNode.Subtrees);
            Assert.Same(parentNode, childNode.Parent);
        }        /// <summary>
        /// Tests that AddSubtree throws ArgumentNullException when a null child node is provided.
        /// </summary>
        [Fact]
        public void AddSubtree_NullChild_ThrowsArgumentNullException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            ISymbolicExpressionTreeNode? nullChild = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => parentNode.AddSubtree(nullChild!));
            Assert.Equal("tree", exception.ParamName);
        }/// <summary>
        /// Tests that RemoveSubtree correctly removes an existing child node from the subtrees collection and clears parent relationship.
        /// </summary>
        [Fact]
        public void RemoveSubtree_ExistingChild_RemovesFromSubtrees()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);
            
            parentNode.AddSubtree(childNode);
            Assert.Single(parentNode.Subtrees); // Verify setup

            // Act
            parentNode.RemoveSubtree(0); // Remove the first (and only) child at index 0

            // Assert
            Assert.Empty(parentNode.Subtrees);
            Assert.DoesNotContain(childNode, parentNode.Subtrees);
            Assert.Null(childNode.Parent);
        }

        /// <summary>
        /// Tests that InsertSubtree correctly inserts a child node at the specified index and sets parent relationship.
        /// </summary>
        [Fact]
        public void InsertSubtree_ValidChild_InsertsAtIndex()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var firstChildSymbol = new Variable { Name = "X" };
            var firstChildNode = new SymbolicExpressionTreeNode(firstChildSymbol);
            
            var secondChildSymbol = new Variable { Name = "Y" };
            var secondChildNode = new SymbolicExpressionTreeNode(secondChildSymbol);
            
            // Add first child
            parentNode.AddSubtree(firstChildNode);
            Assert.Single(parentNode.Subtrees); // Verify setup

            // Act - Insert second child at index 0 (before the first child)
            parentNode.InsertSubtree(0, secondChildNode);

            // Assert
            Assert.Equal(2, parentNode.Subtrees.Count());
            Assert.Same(secondChildNode, parentNode.GetSubtree(0)); // Second child should be at index 0
            Assert.Same(firstChildNode, parentNode.GetSubtree(1)); // First child should be moved to index 1
            Assert.Same(parentNode, secondChildNode.Parent);
            Assert.Same(parentNode, firstChildNode.Parent); // First child parent should remain the same
        }

        /// <summary>
        /// Tests that InsertSubtree throws ArgumentNullException when a null child node is provided.
        /// </summary>
        [Fact]
        public void InsertSubtree_NullChild_ThrowsArgumentNullException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            ISymbolicExpressionTreeNode? nullChild = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => parentNode.InsertSubtree(0, nullChild!));
            Assert.Equal("tree", exception.ParamName);
        }

        /// <summary>
        /// Tests that ReplaceSubtree correctly replaces a child node at the specified index and updates parent relationships.
        /// </summary>
        [Fact]
        public void ReplaceSubtree_ValidReplacement_ReplacesCorrectly()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var originalChildSymbol = new Variable { Name = "X" };
            var originalChildNode = new SymbolicExpressionTreeNode(originalChildSymbol);
            
            var replacementChildSymbol = new Variable { Name = "Y" };
            var replacementChildNode = new SymbolicExpressionTreeNode(replacementChildSymbol);
            
            // Add original child
            parentNode.AddSubtree(originalChildNode);
            Assert.Single(parentNode.Subtrees); // Verify setup
            Assert.Same(originalChildNode, parentNode.GetSubtree(0));

            // Act - Replace child at index 0
            parentNode.ReplaceSubtree(0, replacementChildNode);

            // Assert
            Assert.Single(parentNode.Subtrees); // Should still have one child
            Assert.Same(replacementChildNode, parentNode.GetSubtree(0)); // New child should be at index 0
            Assert.DoesNotContain(originalChildNode, parentNode.Subtrees); // Original child should be removed
            Assert.Same(parentNode, replacementChildNode.Parent); // New child should have correct parent
            Assert.Null(originalChildNode.Parent); // Original child parent should be cleared
        }

        /// <summary>
        /// Tests that ReplaceSubtree throws ArgumentNullException when replacement node is null.
        /// </summary>
        [Fact]
        public void ReplaceSubtree_NullReplacement_ThrowsArgumentNullException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);
            
            parentNode.AddSubtree(childNode);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => parentNode.ReplaceSubtree(0, null!));
        }

        /// <summary>
        /// Tests that ReplaceSubtree throws ArgumentOutOfRangeException when index is invalid.
        /// </summary>
        [Fact]
        public void ReplaceSubtree_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var replacementSymbol = new Variable { Name = "Y" };
            var replacementNode = new SymbolicExpressionTreeNode(replacementSymbol);

            // Act & Assert - Index too high
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.ReplaceSubtree(0, replacementNode));
            
            // Add a child and test negative index
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);
            parentNode.AddSubtree(childNode);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.ReplaceSubtree(-1, replacementNode));
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.ReplaceSubtree(1, replacementNode));
        }

        /// <summary>
        /// Tests IndexOfSubtree method returns correct index for existing subtree.
        /// </summary>
        [Fact]
        public void IndexOfSubtree_ExistingChild_ReturnsCorrectIndex()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var firstChildSymbol = new Variable { Name = "X" };
            var firstChildNode = new SymbolicExpressionTreeNode(firstChildSymbol);
            
            var secondChildSymbol = new Variable { Name = "Y" };
            var secondChildNode = new SymbolicExpressionTreeNode(secondChildSymbol);
            
            parentNode.AddSubtree(firstChildNode);
            parentNode.AddSubtree(secondChildNode);

            // Act & Assert
            Assert.Equal(0, parentNode.IndexOfSubtree(firstChildNode));
            Assert.Equal(1, parentNode.IndexOfSubtree(secondChildNode));
        }

        /// <summary>
        /// Tests IndexOfSubtree method returns -1 for non-existing subtree.
        /// </summary>
        [Fact]
        public void IndexOfSubtree_NonExistingChild_ReturnsMinusOne()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var nonChildSymbol = new Variable { Name = "Z" };
            var nonChildNode = new SymbolicExpressionTreeNode(nonChildSymbol);

            // Act & Assert
            Assert.Equal(-1, parentNode.IndexOfSubtree(nonChildNode));
        }

        /// <summary>
        /// Tests GetLength method calculates correct tree length.
        /// </summary>
        [Fact]
        public void GetLength_SimpleTree_ReturnsCorrectLength()
        {
            // Arrange - Create tree: Addition(X, Y)
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Variable { Name = "Y" };
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);

            // Act & Assert
            Assert.Equal(3, rootNode.GetLength()); // Root + 2 children = 3
            Assert.Equal(1, leftNode.GetLength());
            Assert.Equal(1, rightNode.GetLength());
        }

        /// <summary>
        /// Tests GetDepth method calculates correct tree depth.
        /// </summary>
        [Fact]
        public void GetDepth_SimpleTree_ReturnsCorrectDepth()
        {
            // Arrange - Create tree: Addition(X, Y)
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Variable { Name = "Y" };
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);

            // Act & Assert
            Assert.Equal(2, rootNode.GetDepth()); // Root level 1 + child level 1 = depth 2
            Assert.Equal(1, leftNode.GetDepth());
            Assert.Equal(1, rightNode.GetDepth());
        }

        /// <summary>
        /// Tests GetBranchLevel method calculates correct branch level for child nodes.
        /// </summary>
        [Fact]
        public void GetBranchLevel_ValidChild_ReturnsCorrectLevel()
        {
            // Arrange - Create tree: Addition(X, Multiplication(Y, Z))
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Multiplication();
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            var grandChildSymbol1 = new Variable { Name = "Y" };
            var grandChild1 = new SymbolicExpressionTreeNode(grandChildSymbol1);
            
            var grandChildSymbol2 = new Variable { Name = "Z" };
            var grandChild2 = new SymbolicExpressionTreeNode(grandChildSymbol2);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);
            rightNode.AddSubtree(grandChild1);
            rightNode.AddSubtree(grandChild2);

            // Act & Assert
            Assert.Equal(0, rootNode.GetBranchLevel(rootNode)); // Self = 0
            Assert.Equal(1, rootNode.GetBranchLevel(leftNode));  // Direct child = 1
            Assert.Equal(1, rootNode.GetBranchLevel(rightNode)); // Direct child = 1
            Assert.Equal(2, rootNode.GetBranchLevel(grandChild1)); // Grandchild = 2
            Assert.Equal(2, rootNode.GetBranchLevel(grandChild2)); // Grandchild = 2
        }

        /// <summary>
        /// Tests GetBranchLevel method returns MaxValue for non-descendant nodes.
        /// </summary>
        [Fact]
        public void GetBranchLevel_NonDescendant_ReturnsMaxValue()
        {
            // Arrange
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var nonDescendantSymbol = new Variable { Name = "X" };
            var nonDescendantNode = new SymbolicExpressionTreeNode(nonDescendantSymbol);

            // Act & Assert
            Assert.Equal(int.MaxValue, rootNode.GetBranchLevel(nonDescendantNode));
        }

        /// <summary>
        /// Tests IterateNodesBreadth method returns nodes in breadth-first order.
        /// </summary>
        [Fact]
        public void IterateNodesBreadth_SimpleTree_ReturnsNodesInBreadthFirstOrder()
        {
            // Arrange - Create tree: Addition(X, Multiplication(Y, Z))
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Multiplication();
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            var grandChildSymbol1 = new Variable { Name = "Y" };
            var grandChild1 = new SymbolicExpressionTreeNode(grandChildSymbol1);
            
            var grandChildSymbol2 = new Variable { Name = "Z" };
            var grandChild2 = new SymbolicExpressionTreeNode(grandChildSymbol2);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);
            rightNode.AddSubtree(grandChild1);
            rightNode.AddSubtree(grandChild2);

            // Act
            var nodes = rootNode.IterateNodesBreadth().ToList();

            // Assert - Should be: root, left, right, grandChild1, grandChild2
            Assert.Equal(5, nodes.Count);
            Assert.Same(rootNode, nodes[0]);
            Assert.Same(leftNode, nodes[1]);
            Assert.Same(rightNode, nodes[2]);
            Assert.Same(grandChild1, nodes[3]);
            Assert.Same(grandChild2, nodes[4]);
        }

        /// <summary>
        /// Tests IterateNodesPrefix method returns nodes in prefix order.
        /// </summary>
        [Fact]
        public void IterateNodesPrefix_SimpleTree_ReturnsNodesInPrefixOrder()
        {
            // Arrange - Create tree: Addition(X, Multiplication(Y, Z))
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Multiplication();
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            var grandChildSymbol1 = new Variable { Name = "Y" };
            var grandChild1 = new SymbolicExpressionTreeNode(grandChildSymbol1);
            
            var grandChildSymbol2 = new Variable { Name = "Z" };
            var grandChild2 = new SymbolicExpressionTreeNode(grandChildSymbol2);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);
            rightNode.AddSubtree(grandChild1);
            rightNode.AddSubtree(grandChild2);

            // Act
            var nodes = rootNode.IterateNodesPrefix().ToList();

            // Assert - Should be: root, left, right, grandChild1, grandChild2
            Assert.Equal(5, nodes.Count);
            Assert.Same(rootNode, nodes[0]);
            Assert.Same(leftNode, nodes[1]);
            Assert.Same(rightNode, nodes[2]);
            Assert.Same(grandChild1, nodes[3]);
            Assert.Same(grandChild2, nodes[4]);
        }

        /// <summary>
        /// Tests IterateNodesPostfix method returns nodes in postfix order.
        /// </summary>
        [Fact]
        public void IterateNodesPostfix_SimpleTree_ReturnsNodesInPostfixOrder()
        {
            // Arrange - Create tree: Addition(X, Multiplication(Y, Z))
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Multiplication();
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            var grandChildSymbol1 = new Variable { Name = "Y" };
            var grandChild1 = new SymbolicExpressionTreeNode(grandChildSymbol1);
            
            var grandChildSymbol2 = new Variable { Name = "Z" };
            var grandChild2 = new SymbolicExpressionTreeNode(grandChildSymbol2);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);
            rightNode.AddSubtree(grandChild1);
            rightNode.AddSubtree(grandChild2);

            // Act
            var nodes = rootNode.IterateNodesPostfix().ToList();

            // Assert - Should be: left, grandChild1, grandChild2, right, root
            Assert.Equal(5, nodes.Count);
            Assert.Same(leftNode, nodes[0]);
            Assert.Same(grandChild1, nodes[1]);
            Assert.Same(grandChild2, nodes[2]);
            Assert.Same(rightNode, nodes[3]);
            Assert.Same(rootNode, nodes[4]);
        }

        /// <summary>
        /// Tests RemoveSubtree throws ArgumentOutOfRangeException when index is invalid.
        /// </summary>
        [Fact]
        public void RemoveSubtree_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);

            // Act & Assert - No children
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.RemoveSubtree(0));
            
            // Add one child and test out of bounds
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);
            parentNode.AddSubtree(childNode);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.RemoveSubtree(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.RemoveSubtree(1));
        }

        /// <summary>
        /// Tests InsertSubtree throws ArgumentOutOfRangeException when index is invalid.
        /// </summary>
        [Fact]
        public void InsertSubtree_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);

            // Act & Assert - Negative index should throw
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.InsertSubtree(-1, childNode));
        }

        /// <summary>
        /// Tests GetSubtree throws ArgumentOutOfRangeException when index is invalid.
        /// </summary>
        [Fact]
        public void GetSubtree_InvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);

            // Act & Assert - No children
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.GetSubtree(0));
            
            // Add one child and test out of bounds
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);
            parentNode.AddSubtree(childNode);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.GetSubtree(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => parentNode.GetSubtree(1));
        }

        /// <summary>
        /// Tests GetSubtree returns the correct child node at the specified index.
        /// </summary>
        [Fact]
        public void GetSubtree_ValidIndex_ReturnsCorrectChild()
        {
            // Arrange
            var parentSymbol = new Addition();
            var parentNode = new SymbolicExpressionTreeNode(parentSymbol);
            
            var firstChildSymbol = new Variable { Name = "X" };
            var firstChildNode = new SymbolicExpressionTreeNode(firstChildSymbol);
            
            var secondChildSymbol = new Variable { Name = "Y" };
            var secondChildNode = new SymbolicExpressionTreeNode(secondChildSymbol);
            
            parentNode.AddSubtree(firstChildNode);
            parentNode.AddSubtree(secondChildNode);

            // Act & Assert
            Assert.Same(firstChildNode, parentNode.GetSubtree(0));
            Assert.Same(secondChildNode, parentNode.GetSubtree(1));
        }

        /// <summary>
        /// Tests that cached length and depth values are correctly reset when tree structure changes.
        /// </summary>
        [Fact]
        public void CachedValues_TreeStructureChanges_ValuesAreRecalculated()
        {
            // Arrange
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var childSymbol = new Variable { Name = "X" };
            var childNode = new SymbolicExpressionTreeNode(childSymbol);

            // Initial state
            Assert.Equal(1, rootNode.GetLength());
            Assert.Equal(1, rootNode.GetDepth());

            // Add child
            rootNode.AddSubtree(childNode);
            Assert.Equal(2, rootNode.GetLength()); // Should recalculate
            Assert.Equal(2, rootNode.GetDepth()); // Should recalculate

            // Remove child
            rootNode.RemoveSubtree(0);
            Assert.Equal(1, rootNode.GetLength()); // Should recalculate
            Assert.Equal(1, rootNode.GetDepth()); // Should recalculate
        }

        /// <summary>
        /// Tests that cloning creates a deep copy of the tree structure with correct parent relationships.
        /// </summary>
        [Fact]
        public void Clone_ComplexTree_CreatesDeepCopyWithCorrectParentRelationships()
        {
            // Arrange - Create tree: Addition(X, Multiplication(Y, Z))
            var rootSymbol = new Addition();
            var originalRoot = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Multiplication();
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            var grandChildSymbol1 = new Variable { Name = "Y" };
            var grandChild1 = new SymbolicExpressionTreeNode(grandChildSymbol1);
            
            var grandChildSymbol2 = new Variable { Name = "Z" };
            var grandChild2 = new SymbolicExpressionTreeNode(grandChildSymbol2);
            
            originalRoot.AddSubtree(leftNode);
            originalRoot.AddSubtree(rightNode);
            rightNode.AddSubtree(grandChild1);
            rightNode.AddSubtree(grandChild2);            // Act
            var cloner = new Cloner();
            var clonedRoot = cloner.Clone(originalRoot);

            // Assert - Structure should be identical but objects should be different
            Assert.NotNull(clonedRoot);
            Assert.NotSame(originalRoot, clonedRoot);
            Assert.Equal(originalRoot.SubtreeCount, clonedRoot.SubtreeCount);
            Assert.Equal(originalRoot.GetLength(), clonedRoot.GetLength());
            Assert.Equal(originalRoot.GetDepth(), clonedRoot.GetDepth());
            
            // Check symbols are the same (they should be reused)
            Assert.Same(originalRoot.Symbol, clonedRoot.Symbol);
            
            // Check subtree structure
            var clonedLeft = clonedRoot.GetSubtree(0);
            var clonedRight = clonedRoot.GetSubtree(1);
            
            Assert.NotSame(leftNode, clonedLeft);
            Assert.NotSame(rightNode, clonedRight);
            Assert.Same(leftNode.Symbol, clonedLeft.Symbol);
            Assert.Same(rightNode.Symbol, clonedRight.Symbol);
            
            // Check parent relationships are correct
            Assert.Same(clonedRoot, clonedLeft.Parent);
            Assert.Same(clonedRoot, clonedRight.Parent);
            
            // Check grandchildren
            Assert.Equal(2, clonedRight.SubtreeCount);
            var clonedGrandChild1 = clonedRight.GetSubtree(0);
            var clonedGrandChild2 = clonedRight.GetSubtree(1);
            
            Assert.NotSame(grandChild1, clonedGrandChild1);
            Assert.NotSame(grandChild2, clonedGrandChild2);
            Assert.Same(clonedRight, clonedGrandChild1.Parent);
            Assert.Same(clonedRight, clonedGrandChild2.Parent);
        }        /// <summary>
        /// Tests that ResetLocalParameters doesn't throw for nodes without parameters.
        /// </summary>
        [Fact]
        public void ResetLocalParameters_NodeWithoutParameters_DoesNotThrow()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);
            var random = new MersenneTwister(42);

            // Act & Assert - Should not throw
            node.ResetLocalParameters(random);
        }        /// <summary>
        /// Tests that ShakeLocalParameters doesn't throw for nodes without parameters.
        /// </summary>
        [Fact]
        public void ShakeLocalParameters_NodeWithoutParameters_DoesNotThrow()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);
            var random = new MersenneTwister(42);

            // Act & Assert - Should not throw
            node.ShakeLocalParameters(random, 0.1);
        }

        /// <summary>
        /// Tests HasLocalParameters property returns false for basic nodes.
        /// </summary>
        [Fact]
        public void HasLocalParameters_BasicNode_ReturnsFalse()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act & Assert
            Assert.False(node.HasLocalParameters);
        }

        /// <summary>
        /// Tests Grammar property returns null when no parent is set.
        /// </summary>
        [Fact]
        public void Grammar_NoParent_ReturnsNull()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act & Assert
            Assert.Null(node.Grammar);
        }

        /// <summary>
        /// Tests ToString method returns symbol name.
        /// </summary>
        [Fact]
        public void ToString_ValidSymbol_ReturnsSymbolName()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act
            var result = node.ToString();

            // Assert
            Assert.Equal("Addition", result);
        }

        /// <summary>
        /// Tests that Subtrees property returns empty collection when no children.
        /// </summary>
        [Fact]
        public void Subtrees_NoChildren_ReturnsEmptyCollection()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act
            var subtrees = node.Subtrees.ToList();

            // Assert
            Assert.Empty(subtrees);
        }

        /// <summary>
        /// Tests that SubtreeCount property returns 0 when no children.
        /// </summary>
        [Fact]
        public void SubtreeCount_NoChildren_ReturnsZero()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act & Assert
            Assert.Equal(0, node.SubtreeCount);
        }

        /// <summary>
        /// Tests ForEachNodePrefix action execution with null action.
        /// </summary>
        [Fact]
        public void ForEachNodePrefix_NullAction_ThrowsArgumentNullException()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => node.ForEachNodePrefix(null!));
        }

        /// <summary>
        /// Tests ForEachNodePostfix action execution with null action.
        /// </summary>
        [Fact]
        public void ForEachNodePostfix_NullAction_ThrowsArgumentNullException()
        {
            // Arrange
            var symbol = new Addition();
            var node = new SymbolicExpressionTreeNode(symbol);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => node.ForEachNodePostfix(null!));
        }

        /// <summary>
        /// Tests ForEachNodePrefix executes action on all nodes in correct order.
        /// </summary>
        [Fact]
        public void ForEachNodePrefix_SimpleTree_ExecutesActionInCorrectOrder()
        {
            // Arrange - Create tree: Addition(X, Y)
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Variable { Name = "Y" };
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);

            var visitedNodes = new List<ISymbolicExpressionTreeNode>();

            // Act
            rootNode.ForEachNodePrefix(node => visitedNodes.Add(node));

            // Assert - Should visit root, left, right
            Assert.Equal(3, visitedNodes.Count);
            Assert.Same(rootNode, visitedNodes[0]);
            Assert.Same(leftNode, visitedNodes[1]);
            Assert.Same(rightNode, visitedNodes[2]);
        }

        /// <summary>
        /// Tests ForEachNodePostfix executes action on all nodes in correct order.
        /// </summary>
        [Fact]
        public void ForEachNodePostfix_SimpleTree_ExecutesActionInCorrectOrder()
        {
            // Arrange - Create tree: Addition(X, Y)
            var rootSymbol = new Addition();
            var rootNode = new SymbolicExpressionTreeNode(rootSymbol);
            
            var leftSymbol = new Variable { Name = "X" };
            var leftNode = new SymbolicExpressionTreeNode(leftSymbol);
            
            var rightSymbol = new Variable { Name = "Y" };
            var rightNode = new SymbolicExpressionTreeNode(rightSymbol);
            
            rootNode.AddSubtree(leftNode);
            rootNode.AddSubtree(rightNode);

            var visitedNodes = new List<ISymbolicExpressionTreeNode>();

            // Act
            rootNode.ForEachNodePostfix(node => visitedNodes.Add(node));

            // Assert - Should visit left, right, root
            Assert.Equal(3, visitedNodes.Count);
            Assert.Same(leftNode, visitedNodes[0]);
            Assert.Same(rightNode, visitedNodes[1]);
            Assert.Same(rootNode, visitedNodes[2]);
        }
    }
}
