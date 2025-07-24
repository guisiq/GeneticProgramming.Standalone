using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Problems.Evaluators;

namespace GeneticProgramming.Standalone.Tests
{
    /// <summary>
    /// Integration tests for the complete FunctionalSymbol evaluation system
    /// </summary>
    public class FunctionalSymbolIntegrationTests
    {
        private readonly ExpressionInterpreter _interpreter;
        private readonly IDictionary<string, double> _variables;

        public FunctionalSymbolIntegrationTests()
        {
            _interpreter = new ExpressionInterpreter();
            _variables = new Dictionary<string, double>
            {
                { "X", 3.0 },
                { "Y", 4.0 },
                { "Z", 2.0 }
            };
        }

        [Fact]
        public void ComplexExpression_WithMixedSymbols_ShouldEvaluateCorrectly()
        {
            // Arrange: ((X + Y) * Z) / (X - 1) = ((3 + 4) * 2) / (3 - 1) = 14 / 2 = 7
            var tree = BuildComplexExpressionTree();

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(7.0, result, 2);
        }

        [Fact]
        public void StatisticsExpression_WithVariadicSymbols_ShouldEvaluateCorrectly()
        {
            // Arrange: Mean(X, Y, Z, 5, 1) = Mean(3, 4, 2, 5, 1) = 15/5 = 3
            var tree = BuildStatisticsExpressionTree();

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(3.0, result, 2);
        }

        [Fact]
        public void NestedExpression_WithDeepHierarchy_ShouldEvaluateCorrectly()
        {
            // Arrange: Variance(Mean(X, Y), Z, Sum(1, 2, 3))
            var tree = BuildNestedExpressionTree();

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            // Mean(3, 4) = 3.5, Z = 2, Sum(1, 2, 3) = 6
            // Variance(3.5, 2, 6) = Variance of [3.5, 2, 6]
            // Mean = (3.5 + 2 + 6) / 3 = 3.833...
            // Variance = ((3.5-3.833)² + (2-3.833)² + (6-3.833)²) / 3
            var expectedVariance = CalculateVariance(new[] { 3.5, 2.0, 6.0 });
            Assert.Equal(expectedVariance, result, 1);
        }

        [Fact]
        public void GrammarGeneratedTree_ShouldBeEvaluableByInterpreter()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            var tree = CreateTreeFromGrammarSymbols(grammar);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.True(double.IsFinite(result), "Result should be a finite number");
        }

        [Fact]
        public void MultipleEvaluations_SameTrees_ShouldReturnConsistentResults()
        {
            // Arrange
            var tree = BuildComplexExpressionTree();

            // Act
            var result1 = _interpreter.Evaluate(tree, _variables);
            var result2 = _interpreter.Evaluate(tree, _variables);
            var result3 = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(result1, result2, 10);
            Assert.Equal(result2, result3, 10);
        }

        [Fact]
        public void DifferentVariableValues_ShouldProduceDifferentResults()
        {
            // Arrange
            var tree = CreateSimpleTree(); // X + Y
            var variables1 = new Dictionary<string, double> { { "X", 1.0 }, { "Y", 2.0 } };
            var variables2 = new Dictionary<string, double> { { "X", 5.0 }, { "Y", 10.0 } };

            // Act
            var result1 = _interpreter.Evaluate(tree, variables1);
            var result2 = _interpreter.Evaluate(tree, variables2);

            // Assert
            Assert.Equal(3.0, result1, 2);
            Assert.Equal(15.0, result2, 2);
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void ErrorHandling_MissingVariable_ShouldThrowException()
        {
            // Arrange
            var tree = CreateSimpleTree(); // X + Y
            var incompleteVariables = new Dictionary<string, double> { { "X", 1.0 } }; // Missing Y

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _interpreter.Evaluate(tree, incompleteVariables));
        }

        [Fact]
        public void Performance_LargeExpression_ShouldEvaluateInReasonableTime()
        {
            // Arrange
            var tree = BuildLargeExpressionTree();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = _interpreter.Evaluate(tree, _variables);
            stopwatch.Stop();

            // Assert
            Assert.True(double.IsFinite(result), "Result should be finite");
            Assert.True(stopwatch.ElapsedMilliseconds < 100, "Evaluation should be fast (< 100ms)");
        }

        [Fact]
        public void CustomSymbols_ShouldIntegrateWithExistingSystem()
        {
            // Arrange - Create custom square root symbol
            var sqrtSymbol = new FunctionalSymbol<double>(
                "Sqrt", "Square root operation",
                args => Math.Sqrt(args[0]),
                1, 1);

            var tree = CreateTreeWithCustomSymbol(sqrtSymbol);

            // Act
            var result = _interpreter.Evaluate(tree, _variables);

            // Assert
            Assert.Equal(2.0, result, 2); // sqrt(4) = 2
        }

        #region Helper Methods

        private ISymbolicExpressionTree BuildComplexExpressionTree()
        {
            // ((X + Y) * Z) / (X - 1)
            var root = new SymbolicExpressionTreeNode(MathematicalSymbols.ProtectedDivision);

            // Left side: (X + Y) * Z
            var multiplication = new SymbolicExpressionTreeNode(MathematicalSymbols.Multiplication);
            var addition = new SymbolicExpressionTreeNode(MathematicalSymbols.Addition);
            addition.AddSubtree(CreateVariableNode("X"));
            addition.AddSubtree(CreateVariableNode("Y"));
            multiplication.AddSubtree(addition);
            multiplication.AddSubtree(CreateVariableNode("Z"));

            // Right side: X - 1
            var subtraction = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
            subtraction.AddSubtree(CreateVariableNode("X"));
            subtraction.AddSubtree(CreateConstantNode(1.0));

            root.AddSubtree(multiplication);
            root.AddSubtree(subtraction);

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree BuildStatisticsExpressionTree()
        {
            // Mean(X, Y, Z, 5, 1)
            var root = new SymbolicExpressionTreeNode(StatisticsSymbols.Mean);
            root.AddSubtree(CreateVariableNode("X"));
            root.AddSubtree(CreateVariableNode("Y"));
            root.AddSubtree(CreateVariableNode("Z"));
            root.AddSubtree(CreateConstantNode(5.0));
            root.AddSubtree(CreateConstantNode(1.0));

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree BuildNestedExpressionTree()
        {
            // Variance(Mean(X, Y), Z, Sum(1, 2, 3))
            var root = new SymbolicExpressionTreeNode(StatisticsSymbols.Variance);

            // Mean(X, Y)
            var mean = new SymbolicExpressionTreeNode(StatisticsSymbols.Mean);
            mean.AddSubtree(CreateVariableNode("X"));
            mean.AddSubtree(CreateVariableNode("Y"));

            // Sum(1, 2, 3)
            var sum = new SymbolicExpressionTreeNode(ArraySymbols.Sum);
            sum.AddSubtree(CreateConstantNode(1.0));
            sum.AddSubtree(CreateConstantNode(2.0));
            sum.AddSubtree(CreateConstantNode(3.0));

            root.AddSubtree(mean);
            root.AddSubtree(CreateVariableNode("Z"));
            root.AddSubtree(sum);

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree CreateTreeFromGrammarSymbols(SymbolicRegressionGrammar grammar)
        {
            // Simple expression using grammar symbols: X + Y
            var additionSymbol = grammar.Symbols.First(s => s.Name == "Addition") as FunctionalSymbol<double>;
            var root = new SymbolicExpressionTreeNode(additionSymbol);
            root.AddSubtree(CreateVariableNode("X"));
            root.AddSubtree(CreateVariableNode("Y"));

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree CreateSimpleTree()
        {
            // X + Y
            var root = new SymbolicExpressionTreeNode(MathematicalSymbols.Addition);
            root.AddSubtree(CreateVariableNode("X"));
            root.AddSubtree(CreateVariableNode("Y"));

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree BuildDivisionByZeroTree()
        {
            // X / (Y - 4) where Y = 4
            var root = new SymbolicExpressionTreeNode(MathematicalSymbols.ProtectedDivision);
            root.AddSubtree(CreateVariableNode("X"));

            var subtraction = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
            subtraction.AddSubtree(CreateVariableNode("Y"));
            subtraction.AddSubtree(CreateConstantNode(4.0));

            root.AddSubtree(subtraction);
            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree BuildLargeExpressionTree()
        {
            // Create a deep nested expression for performance testing
            var root = new SymbolicExpressionTreeNode(MathematicalSymbols.Addition);
            var current = root;

            // Build a chain of additions: X + Y + Z + X + Y + Z + ...
            for (int i = 0; i < 20; i++)
            {
                if (i == 19) // Last iteration
                {
                    current.AddSubtree(CreateVariableNode("X"));
                    current.AddSubtree(CreateVariableNode("Y"));
                }
                else
                {
                    var next = new SymbolicExpressionTreeNode(MathematicalSymbols.Addition);
                    current.AddSubtree(CreateVariableNode(i % 3 == 0 ? "X" : i % 3 == 1 ? "Y" : "Z"));
                    current.AddSubtree(next);
                    current = next;
                }
            }

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTree CreateTreeWithCustomSymbol(FunctionalSymbol<double> customSymbol)
        {
            // sqrt(4) = 2
            var root = new SymbolicExpressionTreeNode(customSymbol);
            root.AddSubtree(CreateConstantNode(4.0));

            return new SymbolicExpressionTree(root);
        }

        private ISymbolicExpressionTreeNode CreateVariableNode(string name)
        {
            var variable = new Variable { Name = name };
            return new VariableTreeNode(variable) { VariableName = name };
        }

        private ISymbolicExpressionTreeNode CreateConstantNode(double value)
        {
            var constant = new Constant();
            return new ConstantTreeNode(constant) { Value = value };
        }

        private double CalculateVariance(double[] values)
        {
            var mean = values.Average();
            return values.Sum(v => (v - mean) * (v - mean)) / values.Length;
        }

        #endregion
    }
}
