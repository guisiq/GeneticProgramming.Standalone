using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Standalone.Expressions;
using GeneticProgramming.Standalone.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using Xunit;

namespace GeneticProgramming.Standalone.Tests.Unit
{
    public class MultiSymbolicExpressionTreeTests
    {
        [Fact]
        public void EvaluateAll_ReturnsCorrectOutputs_ForSimpleAddition()
        {
            // Arrange
            var tree = new MultiSymbolicExpressionTree<double>(2);
            
            // Output 0: x + 1
            var node0 = CreateAdditionNode(
                CreateVariableNode("x"),
                CreateConstantNode(1.0)
            );
            
            // Output 1: x + 2
            var node1 = CreateAdditionNode(
                CreateVariableNode("x"),
                CreateConstantNode(2.0)
            );
            
            tree.SetOutputNode(0, node0);
            tree.SetOutputNode(1, node1);
            var variables = new Dictionary<string, double> { { "x", 3.0 } };

            // Act
            var result = tree.EvaluateAll(variables);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(4.0, result[0]); // 3 + 1
            Assert.Equal(5.0, result[1]); // 3 + 2
        }

        [Fact]
        public void GetSharedNodes_ReturnsSharedNodes_WhenNodesAreShared()
        {
            // Arrange
            var tree = new MultiSymbolicExpressionTree<double>(2);
            var shared = CreateVariableNode("x");
            tree.SetOutputNode(0, shared);
            tree.SetOutputNode(1, shared);

            // Act
            var sharedNodes = tree.GetSharedNodes();

            // Assert
            Assert.Single(sharedNodes);
            Assert.Same(shared, sharedNodes[0]);
        }

        [Fact]
        public void EvaluateAll_HandlesMultipleVariables_Correctly()
        {
            // Arrange
            var tree = new MultiSymbolicExpressionTree<double>(3);
            
            // Output 0: x + y
            var node0 = CreateAdditionNode(
                CreateVariableNode("x"),
                CreateVariableNode("y")
            );
            
            // Output 1: x * 2
            var node1 = CreateMultiplicationNode(
                CreateVariableNode("x"),
                CreateConstantNode(2.0)
            );
            
            // Output 2: y - 1
            var node2 = CreateSubtractionNode(
                CreateVariableNode("y"),
                CreateConstantNode(1.0)
            );
            
            tree.SetOutputNode(0, node0);
            tree.SetOutputNode(1, node1);
            tree.SetOutputNode(2, node2);
            
            var variables = new Dictionary<string, double> { { "x", 5.0 }, { "y", 3.0 } };

            // Act
            var result = tree.EvaluateAll(variables);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(8.0, result[0]);  // 5 + 3
            Assert.Equal(10.0, result[1]); // 5 * 2
            Assert.Equal(2.0, result[2]);  // 3 - 1
        }

        // Helper methods to create nodes using real framework implementations
        private ISymbolicExpressionTreeNode<double> CreateAdditionNode(
            ISymbolicExpressionTreeNode<double> left,
            ISymbolicExpressionTreeNode<double> right)
        {
            var node = new SymbolicExpressionTreeNode<double>(MathematicalSymbols.Addition);
            node.AddSubtree(left);
            node.AddSubtree(right);
            return node;
        }

        private ISymbolicExpressionTreeNode<double> CreateMultiplicationNode(
            ISymbolicExpressionTreeNode<double> left,
            ISymbolicExpressionTreeNode<double> right)
        {
            var node = new SymbolicExpressionTreeNode<double>(MathematicalSymbols.Multiplication);
            node.AddSubtree(left);
            node.AddSubtree(right);
            return node;
        }

        private ISymbolicExpressionTreeNode<double> CreateSubtractionNode(
            ISymbolicExpressionTreeNode<double> left,
            ISymbolicExpressionTreeNode<double> right)
        {
            var node = new SymbolicExpressionTreeNode<double>(MathematicalSymbols.Subtraction);
            node.AddSubtree(left);
            node.AddSubtree(right);
            return node;
        }

        private ISymbolicExpressionTreeNode<double> CreateVariableNode(string variableName)
        {
            var variableSymbol = new Variable<double> { Name = variableName };
            return new VariableTreeNode<double>(variableSymbol, variableName);
        }

        private ISymbolicExpressionTreeNode<double> CreateConstantNode(double value)
        {
            var constantSymbol = new Constant<double>();
            return new ConstantTreeNode<double>(constantSymbol, value);
        }
    }
}
