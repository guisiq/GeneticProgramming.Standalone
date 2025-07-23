using System;
using System.Collections.Generic;
using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Problems.Evaluators;

namespace GeneticProgramming.Standalone.Tests
{
    /// <summary>
    /// Tests for FunctionalSymbol evaluation in ExpressionInterpreter
    /// </summary>
    public class FunctionalSymbolEvaluationTests
    {
        private readonly ExpressionInterpreter _interpreter;
        private readonly IDictionary<string, double> _variables;

        public FunctionalSymbolEvaluationTests()
        {
            _interpreter = new ExpressionInterpreter();
            _variables = new Dictionary<string, double>
            {
                { "X", 5.0 },
                { "Y", 3.0 },
                { "Z", 2.0 }
            };
        }

        [Fact]
        public void MathematicalSymbols_Addition_ShouldEvaluateCorrectly()
        {
            // Arrange
            var tree = CreateBinaryTree(MathematicalSymbols.Addition, 5.0, 3.0);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(8.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Subtraction_ShouldEvaluateCorrectly()
        {
            // Arrange
            var tree = CreateBinaryTree(MathematicalSymbols.Subtraction, 5.0, 3.0);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(2.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Multiplication_ShouldEvaluateCorrectly()
        {
            // Arrange
            var tree = CreateBinaryTree(MathematicalSymbols.Multiplication, 5.0, 3.0);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(15.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Division_ShouldEvaluateCorrectly()
        {
            // Arrange
            var tree = CreateBinaryTree(MathematicalSymbols.ProtectedDivision, 15.0, 3.0);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(5.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Division_ByZero_ShouldThrowException()
        {
            // Arrange
            var tree = CreateBinaryTree(MathematicalSymbols.ProtectedDivision, 5.0, 0.0);

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _interpreter.Evaluate(tree, _variables));
        }

        [Fact]
        public void StatisticsSymbols_Mean_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var tree = CreateVariadicTree(StatisticsSymbols.Mean, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(3.0, result, 2);
        }

        [Fact]
        public void StatisticsSymbols_Variance_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var tree = CreateVariadicTree(StatisticsSymbols.Variance, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(2.0, result, 2); // Variance of [1,2,3,4,5] is 2.0
        }

        [Fact]
        public void StatisticsSymbols_Median_OddCount_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 1.0, 3.0, 2.0, 5.0, 4.0 };
            var tree = CreateVariadicTree(StatisticsSymbols.Median, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(3.0, result, 2);
        }

        [Fact]
        public void StatisticsSymbols_Median_EvenCount_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 1.0, 2.0, 3.0, 4.0 };
            var tree = CreateVariadicTree(StatisticsSymbols.Median, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(2.5, result, 2); // (2+3)/2 = 2.5
        }

        [Fact]
        public void ArraySymbols_Sum_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var tree = CreateVariadicTree(ArraySymbols.Sum, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(15.0, result, 2);
        }

        [Fact]
        public void ArraySymbols_Multiply_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 2.0, 3.0, 4.0 };
            var tree = CreateVariadicTree(ArraySymbols.Multiply, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(24.0, result, 2); // 2*3*4 = 24
        }

        [Fact]
        public void ListSymbols_Sum_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 10.0, 20.0, 30.0 };
            var tree = CreateVariadicTree(ListSymbols.Sum, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(60.0, result, 2);
        }

        [Fact]
        public void ListSymbols_Average_ShouldEvaluateCorrectly()
        {
            // Arrange
            var values = new[] { 10.0, 20.0, 30.0 };
            var tree = CreateVariadicTree(ListSymbols.Average, values);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(20.0, result, 2);
        }

        [Fact]
        public void VariableSymbol_ShouldEvaluateFromVariablesDictionary()
        {
            // Arrange
            var tree = CreateVariableTree("X");

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(5.0, result, 2);
        }

        [Fact]
        public void ConstantSymbol_ShouldEvaluateToConstantValue()
        {
            // Arrange
            var tree = CreateConstantTree(42.0);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(42.0, result, 2);
        }

        [Fact]
        public void ComplexExpression_ShouldEvaluateCorrectly()
        {
            // Arrange: (X + Y) * Z = (5 + 3) * 2 = 16
            var additionTree = CreateBinaryTree(MathematicalSymbols.Addition, 
                CreateVariableTree("X"), CreateVariableTree("Y"));
            var tree = CreateBinaryTree(MathematicalSymbols.Multiplication,
                additionTree, CreateVariableTree("Z"));

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(16.0, result, 2);
        }

        #region Helper Methods

        private ISymbolicExpressionTree CreateBinaryTree(FunctionalSymbol<double> symbol, double value1, double value2)
        {
            return CreateBinaryTree(symbol, CreateConstantTree(value1), CreateConstantTree(value2));
        }

        private ISymbolicExpressionTree CreateBinaryTree(FunctionalSymbol<double> symbol, 
            ISymbolicExpressionTree left, ISymbolicExpressionTree right)
        {
            var root = new SymbolicExpressionTreeNode(symbol);
            root.AddSubtree(left.Root);
            root.AddSubtree(right.Root);
            
            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree CreateVariadicTree(FunctionalSymbol<double> symbol, double[] values)
        {
            var root = new SymbolicExpressionTreeNode(symbol);
            
            foreach (var value in values)
            {
                root.AddSubtree(CreateConstantTree(value).Root);
            }
            
            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree CreateConstantTree(double value)
        {
            var constantSymbol = new Constant();
            var constantNode = new ConstantTreeNode(constantSymbol) { Value = value };
            return new SymbolicExpressionTree(constantNode);
        }

        private ISymbolicExpressionTree CreateVariableTree(string variableName)
        {
            var variableSymbol = new Variable { Name = variableName };
            var variableNode = new VariableTreeNode(variableSymbol) { VariableName = variableName };
            return new SymbolicExpressionTree(variableNode);
        }

        #endregion
    }
}
