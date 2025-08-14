// using System;
// using System.Collections.Generic;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using GeneticProgramming.Standalone.Expressions;
// using GeneticProgramming.Standalone.Expressions.Symbols;
// using GeneticProgramming.Standalone.Core;

// namespace GeneticProgramming.Standalone.Tests.Unit.Expressions;

// /// <summary>
// /// Basic unit tests for multi-output functionality.
// /// Tests creation, manipulation, and basic operations of multi-output trees.
// /// </summary>
// [TestClass]
// [TestCategory("BasicMultiOutput")]
// public class MultiOutputBasicTests
// {
//     /// <summary>
//     /// Tests that a multi-output tree can be created with valid output count.
//     /// </summary>
//     [TestMethod]
//     public void CreateMultiOutputTree_ValidOutputCount_Success()
//     {
//         // Arrange
//         const int outputCount = 3;

//         // Act
//         var tree = new MultiSymbolicExpressionTree<double>(outputCount);

//         // Assert
//         Assert.IsNotNull(tree);
//         Assert.AreEqual(outputCount, tree.OutputCount);
//         Assert.IsNotNull(tree.Root);
//         Assert.AreEqual(1, tree.Length); // Only root node
//         Assert.AreEqual(1, tree.Depth);  // Only root node
//     }

//     /// <summary>
//     /// Tests that creating a multi-output tree with invalid output count throws exception.
//     /// </summary>
//     [TestMethod]
//     [ExpectedException(typeof(ArgumentOutOfRangeException))]
//     public void CreateMultiOutputTree_InvalidOutputCount_ThrowsException()
//     {
//         // Act & Assert
//         var tree = new MultiSymbolicExpressionTree<double>(0);
//     }

//     /// <summary>
//     /// Tests that setting an output node at valid index succeeds.
//     /// </summary>
//     [TestMethod]
//     public void SetOutputNode_ValidIndex_Success()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);
//         var constantNode = CreateConstantNode(42.0);

//         // Act
//         tree.SetOutputNode(0, constantNode);

//         // Assert
//         var retrievedNode = tree.GetOutputNode(0);
//         Assert.IsNotNull(retrievedNode);
//         Assert.AreEqual(constantNode, retrievedNode);
//     }

//     /// <summary>
//     /// Tests that setting an output node at invalid index throws exception.
//     /// </summary>
//     [TestMethod]
//     [ExpectedException(typeof(ArgumentOutOfRangeException))]
//     public void SetOutputNode_InvalidIndex_ThrowsException()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);
//         var constantNode = CreateConstantNode(42.0);

//         // Act & Assert
//         tree.SetOutputNode(2, constantNode);
//     }

//     /// <summary>
//     /// Tests that setting a null output node throws exception.
//     /// </summary>
//     [TestMethod]
//     [ExpectedException(typeof(ArgumentNullException))]
//     public void SetOutputNode_NullNode_ThrowsException()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);

//         // Act & Assert
//         tree.SetOutputNode(0, null);
//     }

//     /// <summary>
//     /// Tests that getting an output node at valid index returns correct node.
//     /// </summary>
//     [TestMethod]
//     public void GetOutputNode_ValidIndex_ReturnsCorrectNode()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(3);
//         var node1 = CreateConstantNode(1.0);
//         var node2 = CreateConstantNode(2.0);
        
//         tree.SetOutputNode(0, node1);
//         tree.SetOutputNode(2, node2);

//         // Act & Assert
//         Assert.AreEqual(node1, tree.GetOutputNode(0));
//         Assert.IsNull(tree.GetOutputNode(1)); // Not set
//         Assert.AreEqual(node2, tree.GetOutputNode(2));
//     }

//     /// <summary>
//     /// Tests that getting an output node at invalid index throws exception.
//     /// </summary>
//     [TestMethod]
//     [ExpectedException(typeof(ArgumentOutOfRangeException))]
//     public void GetOutputNode_InvalidIndex_ThrowsException()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);

//         // Act & Assert
//         tree.GetOutputNode(-1);
//     }

//     /// <summary>
//     /// Tests that cloning a multi-output tree preserves structure.
//     /// </summary>
//     [TestMethod]
//     public void Clone_MultiOutputTree_PreservesStructure()
//     {
//         // Arrange
//         var originalTree = new MultiSymbolicExpressionTree<double>(2);
//         var node1 = CreateConstantNode(10.0);
//         var node2 = CreateConstantNode(20.0);
        
//         originalTree.SetOutputNode(0, node1);
//         originalTree.SetOutputNode(1, node2);

//         // Act
//         var clonedTree = (MultiSymbolicExpressionTree<double>)originalTree.Clone();

//         // Assert
//         Assert.IsNotNull(clonedTree);
//         Assert.AreEqual(originalTree.OutputCount, clonedTree.OutputCount);
        
//         // Nodes should be different instances but equal values
//         var clonedNode1 = clonedTree.GetOutputNode(0);
//         var clonedNode2 = clonedTree.GetOutputNode(1);
        
//         Assert.IsNotNull(clonedNode1);
//         Assert.IsNotNull(clonedNode2);
//         Assert.AreNotSame(node1, clonedNode1); // Different instances
//         Assert.AreNotSame(node2, clonedNode2); // Different instances
//     }

//     /// <summary>
//     /// Tests that evaluating all outputs works correctly.
//     /// </summary>
//     [TestMethod]
//     public void EvaluateAll_WithConstantNodes_ReturnsCorrectValues()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(3);
//         tree.SetOutputNode(0, CreateConstantNode(1.0));
//         tree.SetOutputNode(1, CreateConstantNode(2.0));
//         tree.SetOutputNode(2, CreateConstantNode(3.0));

//         var variables = new Dictionary<string, double>();

//         // Act
//         var results = tree.EvaluateAll(variables);

//         // Assert
//         Assert.IsNotNull(results);
//         Assert.AreEqual(3, results.Count);
//         Assert.AreEqual(1.0, results[0]);
//         Assert.AreEqual(2.0, results[1]);
//         Assert.AreEqual(3.0, results[2]);
//     }

//     /// <summary>
//     /// Tests that evaluating with null variables throws exception.
//     /// </summary>
//     [TestMethod]
//     [ExpectedException(typeof(ArgumentNullException))]
//     public void EvaluateAll_NullVariables_ThrowsException()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(1);
//         tree.SetOutputNode(0, CreateConstantNode(1.0));

//         // Act & Assert
//         tree.EvaluateAll(null);
//     }

//     /// <summary>
//     /// Tests that ToString returns meaningful representation.
//     /// </summary>
//     [TestMethod]
//     public void ToString_MultiOutputTree_ReturnsMeaningfulString()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);
//         tree.SetOutputNode(0, CreateConstantNode(1.0));

//         // Act
//         var result = tree.ToString();

//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsTrue(result.Contains("MultiSymbolicExpressionTree"));
//         Assert.IsTrue(result.Contains("2 outputs"));
//         Assert.IsTrue(result.Contains("1 set"));
//     }

//     /// <summary>
//     /// Tests getting shared nodes when no nodes are shared.
//     /// </summary>
//     [TestMethod]
//     public void GetSharedNodes_IndependentOutputs_ReturnsEmpty()
//     {
//         // Arrange
//         var tree = new MultiSymbolicExpressionTree<double>(2);
//         tree.SetOutputNode(0, CreateConstantNode(1.0));
//         tree.SetOutputNode(1, CreateConstantNode(2.0));

//         // Act
//         var sharedNodes = tree.GetSharedNodes();

//         // Assert
//         Assert.IsNotNull(sharedNodes);
//         Assert.AreEqual(0, sharedNodes.Count);
//     }

//     /// <summary>
//     /// Helper method to create a constant node for testing.
//     /// Note: This is a simplified implementation for testing purposes.
//     /// </summary>
//     /// <param name="value">Constant value</param>
//     /// <returns>A constant node</returns>
//     private ISymbolicExpressionTreeNode<double> CreateConstantNode(double value)
//     {
//         // For now, create a simple mock node
//         // In a full implementation, this would use the actual constant symbol
//         return new TestConstantNode<double>(value);
//     }
// }

// /// <summary>
// /// Simple test implementation of a constant node for testing purposes.
// /// </summary>
// /// <typeparam name="T">Value type</typeparam>
// internal class TestConstantNode<T> : SymbolicExpressionTreeNode<T> where T : struct
// {
//     private readonly T _value;

//     public TestConstantNode(T value)
//     {
//         _value = value;
//     }

//     public override T Evaluate(IDictionary<string, T> variables)
//     {
//         return _value;
//     }

//     public override ISymbolicExpressionTreeNode Clone()
//     {
//         return new TestConstantNode<T>(_value);
//     }

//     public override string ToString()
//     {
//         return _value.ToString() ?? "0";
//     }
// }
