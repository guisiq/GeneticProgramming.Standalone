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
    /// Tests for FunctionalSymbol integration with grammars
    /// </summary>
    public class GrammarFunctionalSymbolTests
    {
        private readonly ExpressionInterpreter _interpreter;

        public GrammarFunctionalSymbolTests()
        {
            _interpreter = new ExpressionInterpreter();
        }

        [Fact]
        public void SymbolicRegressionGrammar_ShouldContainMathematicalSymbols()
        {
            // Arrange
            var variableNames = new[] { "X", "Y" };
            var grammar = new SymbolicRegressionGrammar(variableNames);

            // Act
            var symbols = grammar.Symbols.ToList();

            // Assert
            Assert.Contains(symbols, s => s.Name == "Addition");
            Assert.Contains(symbols, s => s.Name == "Subtraction");
            Assert.Contains(symbols, s => s.Name == "Multiplication");
            Assert.Contains(symbols, s => s.Name == "Division");
        }

        [Fact]
        public void SymbolicRegressionGrammar_ShouldContainVariables()
        {
            // Arrange
            var variableNames = new[] { "X", "Y", "Z" };
            var grammar = new SymbolicRegressionGrammar(variableNames);

            // Act
            var symbols = grammar.Symbols.ToList();

            // Assert
            foreach (var varName in variableNames)
            {
                Assert.Contains(symbols, s => s.Name == varName);
            }
        }

        [Fact]
        public void SymbolicRegressionGrammar_WithConstants_ShouldContainConstantSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowConstants: true);

            // Act
            var symbols = grammar.Symbols.ToList();

            // Assert
            Assert.Contains(symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void SymbolicRegressionGrammar_WithoutConstants_ShouldNotContainConstantSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowConstants: false);

            // Act
            var symbols = grammar.Symbols.ToList();

            // Assert
            Assert.DoesNotContain(symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void SymbolicRegressionGrammar_WithoutDivision_ShouldNotContainDivisionSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowDivision: false);

            // Act
            var symbols = grammar.Symbols.ToList();

            // Assert
            Assert.DoesNotContain(symbols, s => s.Name == "Division");
        }

        [Fact]
        public void SymbolicRegressionGrammar_AllowDivisionProperty_ShouldToggleDivisionSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowDivision: true);

            // Act & Assert - Initially should contain division
            Assert.Contains(grammar.Symbols, s => s.Name == "Division");

            // Disable division
            grammar.AllowDivision = false;
            Assert.DoesNotContain(grammar.Symbols, s => s.Name == "Division");

            // Re-enable division
            grammar.AllowDivision = true;
            Assert.Contains(grammar.Symbols, s => s.Name == "Division");
        }

        [Fact]
        public void SymbolicRegressionGrammar_AllowConstantsProperty_ShouldToggleConstantSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowConstants: true);

            // Act & Assert - Initially should contain constant
            Assert.Contains(grammar.Symbols, s => s.Name == "Constant");

            // Disable constants
            grammar.AllowConstants = false;
            Assert.DoesNotContain(grammar.Symbols, s => s.Name == "Constant");

            // Re-enable constants
            grammar.AllowConstants = true;
            Assert.Contains(grammar.Symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void SymbolicRegressionGrammar_AddVariable_ShouldAddNewVariableSymbol()
        {
            // Arrange
            var variableNames = new[] { "X" };
            var grammar = new SymbolicRegressionGrammar(variableNames);

            // Act
            grammar.AddVariable("NewVar");

            // Assert
            Assert.Contains(grammar.Symbols, s => s.Name == "NewVar");
            Assert.Contains(grammar.VariableNames, name => name == "NewVar");
        }

        [Fact]
        public void SymbolicRegressionGrammar_RemoveVariable_ShouldRemoveVariableSymbol()
        {
            // Arrange
            var variableNames = new[] { "X", "Y" };
            var grammar = new SymbolicRegressionGrammar(variableNames);

            // Act
            grammar.RemoveVariable("Y");

            // Assert
            Assert.DoesNotContain(grammar.Symbols, s => s.Name == "Y");
            Assert.DoesNotContain(grammar.VariableNames, name => name == "Y");
        }

        [Fact]
        public void SymbolicRegressionGrammar_CreateSimpleGrammar_ShouldContainOnlyBasicOperations()
        {
            // Arrange & Act
            var grammar = SymbolicRegressionGrammar.CreateSimpleGrammar(new[] { "X", "Y" });

            // Assert
            var symbols = grammar.Symbols.ToList();
            Assert.Contains(symbols, s => s.Name == "Addition");
            Assert.Contains(symbols, s => s.Name == "Subtraction");
            Assert.Contains(symbols, s => s.Name == "Multiplication");
            Assert.DoesNotContain(symbols, s => s.Name == "Division");
            Assert.DoesNotContain(symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void SymbolicRegressionGrammar_CreateStandardGrammar_ShouldContainAllOperations()
        {
            // Arrange & Act
            var grammar = SymbolicRegressionGrammar.CreateStandardGrammar(new[] { "X", "Y" });

            // Assert
            var symbols = grammar.Symbols.ToList();
            Assert.Contains(symbols, s => s.Name == "Addition");
            Assert.Contains(symbols, s => s.Name == "Subtraction");
            Assert.Contains(symbols, s => s.Name == "Multiplication");
            Assert.Contains(symbols, s => s.Name == "Division");
            Assert.Contains(symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void DefaultSymbolicExpressionTreeGrammar_ShouldContainMathematicalSymbols()
        {
            // Arrange & Act
            var grammar = new DefaultSymbolicExpressionTreeGrammar();

            // Assert
            var symbols = grammar.Symbols.ToList();
            Assert.Contains(symbols, s => s.Name == "Addition");
            Assert.Contains(symbols, s => s.Name == "Subtraction");
            Assert.Contains(symbols, s => s.Name == "Multiplication");
            Assert.Contains(symbols, s => s.Name == "Division");
        }

        [Fact]
        public void GrammarSymbols_ShouldBeEvaluableByInterpreter()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" });
            var variables = new Dictionary<string, double> { { "X", 10.0 } };

            // Find the addition symbol from grammar
            var additionSymbol = grammar.Symbols.FirstOrDefault(s => s.Name == "Addition") as FunctionalSymbol<double>;
            Assert.NotNull(additionSymbol);

            // Create a simple tree: X + 5
            var tree = CreateSimpleAdditionTree(additionSymbol, variables);

            // Act
            var result = _interpreter.Evaluate(tree, variables);

            // Assert
            Assert.Equal(15.0, result, 2); // 10 + 5 = 15
        }

        [Fact]
        public void GrammarValidation_ShouldPassForWellFormedGrammars()
        {
            // Arrange
            var grammar1 = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            var grammar2 = SymbolicRegressionGrammar.CreateSimpleGrammar(new[] { "A" });
            var grammar3 = new DefaultSymbolicExpressionTreeGrammar();

            // Act & Assert
            Assert.True(grammar1.ValidateForRegression());
            Assert.True(grammar2.Validate());
            Assert.True(grammar3.Validate());
        }

        #region Helper Methods

        private ISymbolicExpressionTree CreateSimpleAdditionTree(FunctionalSymbol<double> additionSymbol, 
            IDictionary<string, double> variables)
        {
            var root = new SymbolicExpressionTreeNode(additionSymbol);
            
            // Add variable X
            var variableSymbol = new Variable { Name = "X" };
            var variableNode = new VariableTreeNode(variableSymbol) { VariableName = "X" };
            root.AddSubtree(variableNode);
            
            // Add constant 5
            var constantSymbol = new Constant();
            var constantNode = new ConstantTreeNode(constantSymbol) { Value = 5.0 };
            root.AddSubtree(constantNode);
            
            return new SymbolicExpressionTree(root);
        }

        #endregion
    }
}
