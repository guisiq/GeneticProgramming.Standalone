using System;
using System.Collections.Generic;
using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Tests
{
    /// <summary>
    /// Tests specifically for the IEvaluable interface implementation
    /// </summary>
    public class IEvaluableInterfaceTests
    {
        private readonly IDictionary<string, double> _variables;

        public IEvaluableInterfaceTests()
        {
            _variables = new Dictionary<string, double>
            {
                { "X", 7.0 },
                { "Y", 4.0 },
                { "TestVar", 12.0 }
            };
        }

        [Fact]
        public void FunctionalSymbol_ShouldImplementIEvaluable()
        {
            // Arrange
            var symbol = MathematicalSymbols.Addition;

            // Act & Assert
            Assert.True(symbol is IEvaluable<double>);
        }

        [Fact]
        public void Variable_ShouldImplementIEvaluable()
        {
            // Arrange
            var variable = new Variable();

            // Act & Assert
            Assert.True(variable is IEvaluable<double>);
        }

        [Fact]
        public void Constant_ShouldImplementIEvaluable()
        {
            // Arrange
            var constant = new Constant();

            // Act & Assert
            Assert.True(constant is IEvaluable<double>);
        }

        [Fact]
        public void FunctionalSymbol_Evaluate_ShouldUseInternalOperation()
        {
            // Arrange
            var symbol = MathematicalSymbols.Addition;
            var childValues = new[] { 10.0, 5.0 };

            // Act
            var result = symbol.Evaluate(childValues, _variables);

            // Assert
            Assert.Equal(15.0, result, 2);
        }

        [Fact]
        public void Variable_Evaluate_ShouldLookupInVariablesDictionary()
        {
            // Arrange
            var variable = new Variable { Name = "TestVar" };

            // Act
            var result = variable.Evaluate(new double[0], _variables);

            // Assert
            Assert.Equal(12.0, result, 2);
        }

        [Fact]
        public void Variable_Evaluate_MissingVariable_ShouldThrowException()
        {
            // Arrange
            var variable = new Variable { Name = "MissingVar" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => variable.Evaluate(new double[0], _variables));
        }

        [Fact]
        public void Constant_Evaluate_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var constant = new Constant();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => constant.Evaluate(new double[0], _variables));
        }

        [Fact]
        public void CustomFunctionalSymbol_ShouldWorkWithIEvaluable()
        {
            // Arrange - Create custom power function: x^y
            var powerSymbol = new FunctionalSymbol<double>(
                "Power", "Power operation (x^y)",
                args => Math.Pow(args[0], args[1]),
                2, 2);

            var childValues = new[] { 2.0, 3.0 };

            // Act
            var result = powerSymbol.Evaluate(childValues, _variables);

            // Assert
            Assert.Equal(8.0, result, 2); // 2^3 = 8
        }

        [Fact]
        public void CustomVariadicFunctionalSymbol_ShouldWorkWithIEvaluable()
        {
            // Arrange - Create custom max function
            var maxSymbol = new FunctionalSymbol<double>(
                "Max", "Maximum value operation",
                args => 
                {
                    if (args.Length == 0) throw new ArgumentException("Max requires at least one argument");
                    var max = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (args[i] > max) max = args[i];
                    }
                    return max;
                },
                1, int.MaxValue);

            var childValues = new[] { 3.0, 7.0, 2.0, 9.0, 1.0 };

            // Act
            var result = maxSymbol.Evaluate(childValues, _variables);

            // Assert
            Assert.Equal(9.0, result, 2);
        }

        [Fact]
        public void StatisticsSymbols_ShouldAllImplementIEvaluable()
        {
            // Arrange
            var symbols = new ISymbol[]
            {
                StatisticsSymbols.Mean,
                StatisticsSymbols.Variance,
                StatisticsSymbols.Median
            };

            // Act & Assert
            foreach (var symbol in symbols)
            {
                Assert.True(symbol is IEvaluable<double>, $"Symbol {symbol.Name} should implement IEvaluable<double>");
            }
        }

        [Fact]
        public void ArraySymbols_ShouldAllImplementIEvaluable()
        {
            // Arrange
            var symbols = new ISymbol[]
            {
                ArraySymbols.Sum,
                ArraySymbols.Multiply
            };

            // Act & Assert
            foreach (var symbol in symbols)
            {
                Assert.True(symbol is IEvaluable<double>, $"Symbol {symbol.Name} should implement IEvaluable<double>");
            }
        }

        [Fact]
        public void ListSymbols_ShouldAllImplementIEvaluable()
        {
            // Arrange
            var symbols = new ISymbol[]
            {
                ListSymbols.Sum,
                ListSymbols.Average
            };

            // Act & Assert
            foreach (var symbol in symbols)
            {
                Assert.True(symbol is IEvaluable<double>, $"Symbol {symbol.Name} should implement IEvaluable<double>");
            }
        }

        [Fact]
        public void MathematicalSymbols_ShouldAllImplementIEvaluable()
        {
            // Arrange
            var symbols = new ISymbol[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            };

            // Act & Assert
            foreach (var symbol in symbols)
            {
                Assert.True(symbol is IEvaluable<double>, $"Symbol {symbol.Name} should implement IEvaluable<double>");
            }
        }

        [Theory]
        [InlineData(new double[] { 2.0, 3.0 }, 5.0)]
        [InlineData(new double[] { -1.0, 1.0 }, 0.0)]
        [InlineData(new double[] { 0.0, 0.0 }, 0.0)]
        [InlineData(new double[] { 10.5, 2.3 }, 12.8)]
        public void Addition_Evaluate_ShouldReturnCorrectSum(double[] inputs, double expected)
        {
            // Arrange
            var symbol = MathematicalSymbols.Addition;

            // Act
            var result = symbol.Evaluate(inputs, _variables);

            // Assert
            Assert.Equal(expected, result, 2);
        }

        [Theory]
        [InlineData(new double[] { 5.0, 3.0 }, 2.0)]
        [InlineData(new double[] { 1.0, 1.0 }, 0.0)]
        [InlineData(new double[] { 0.0, 5.0 }, -5.0)]
        [InlineData(new double[] { 10.7, 2.2 }, 8.5)]
        public void Subtraction_Evaluate_ShouldReturnCorrectDifference(double[] inputs, double expected)
        {
            // Arrange
            var symbol = MathematicalSymbols.Subtraction;

            // Act
            var result = symbol.Evaluate(inputs, _variables);

            // Assert
            Assert.Equal(expected, result, 2);
        }

        [Theory]
        [InlineData(new double[] { 3.0, 4.0 }, 12.0)]
        [InlineData(new double[] { 0.0, 5.0 }, 0.0)]
        [InlineData(new double[] { -2.0, 3.0 }, -6.0)]
        [InlineData(new double[] { 2.5, 2.0 }, 5.0)]
        public void Multiplication_Evaluate_ShouldReturnCorrectProduct(double[] inputs, double expected)
        {
            // Arrange
            var symbol = MathematicalSymbols.Multiplication;

            // Act
            var result = symbol.Evaluate(inputs, _variables);

            // Assert
            Assert.Equal(expected, result, 2);
        }

        [Theory]
        [InlineData(new double[] { 12.0, 3.0 }, 4.0)]
        [InlineData(new double[] { 7.0, 1.0 }, 7.0)]
        [InlineData(new double[] { -6.0, 2.0 }, -3.0)]
        [InlineData(new double[] { 5.0, 2.0 }, 2.5)]
        public void Division_Evaluate_ShouldReturnCorrectQuotient(double[] inputs, double expected)
        {
            // Arrange
            var symbol = MathematicalSymbols.ProtectedDivision;

            // Act
            var result = symbol.Evaluate(inputs, _variables);

            // Assert
            Assert.Equal(expected, result, 2);
        }
    }
}
