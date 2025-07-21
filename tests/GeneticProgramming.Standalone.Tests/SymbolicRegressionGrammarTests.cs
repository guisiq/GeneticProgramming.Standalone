using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols; // Added for concrete Symbol access
using GeneticProgramming.Expressions; // Added for ISymbol and Symbol
using System;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Grammars
{
    public class SymbolicRegressionGrammarTests
    {
        [Fact]
        public void Constructor_Default_InitializesCorrectly()
        {
            // Arrange & Act
            var grammar = new SymbolicRegressionGrammar();

            // Assert
            Assert.Equal("SymbolicRegressionGrammar", grammar.Name);
            Assert.Contains("X", grammar.VariableNames);
            Assert.True(grammar.AllowConstants);
            Assert.True(grammar.AllowDivision);
            Assert.NotNull(grammar.GetSymbol("Add"));
            Assert.NotNull(grammar.GetSymbol("Subtract"));
            Assert.NotNull(grammar.GetSymbol("Multiply"));
            Assert.NotNull(grammar.GetSymbol("Divide"));
            Assert.NotNull(grammar.GetSymbol("Constant"));
            Assert.NotNull(grammar.GetSymbol("X"));
            Assert.Contains(grammar.StartSymbols, s => s.Name == "X");
            Assert.Contains(grammar.StartSymbols, s => s.Name == "Add");
        }

        [Fact]
        public void Constructor_WithSpecificVariables_InitializesCorrectly()
        {
            // Arrange
            var variables = new[] { "Var1", "Var2" };

            // Act
            var grammar = new SymbolicRegressionGrammar(variables, allowConstants: false, allowDivision: false);

            // Assert
            Assert.Equal(variables.Length, grammar.VariableNames.Count());
            Assert.Contains("Var1", grammar.VariableNames);
            Assert.Contains("Var2", grammar.VariableNames);
            Assert.False(grammar.AllowConstants);
            Assert.False(grammar.AllowDivision);
            Assert.Null(grammar.GetSymbol("Constant"));
            Assert.Null(grammar.GetSymbol("Divide"));
            Assert.NotNull(grammar.GetSymbol("Var1"));
            Assert.NotNull(grammar.GetSymbol("Var2"));
        }

        [Fact]
        public void Constructor_NullVariableNames_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SymbolicRegressionGrammar(null));
        }

        [Fact]
        public void Constructor_EmptyVariableNames_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new SymbolicRegressionGrammar(Array.Empty<string>()));
        }

        [Fact]
        public void AllowConstants_SetToFalse_RemovesConstantSymbol()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(); // Constants allowed by default
            Assert.NotNull(grammar.GetSymbol("Constant"));

            // Act
            grammar.AllowConstants = false;

            // Assert
            Assert.False(grammar.AllowConstants);
            Assert.Null(grammar.GetSymbol("Constant"));
        }

        [Fact]
        public void AllowConstants_SetToTrue_AddsConstantSymbol()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" }, allowConstants: false);
            Assert.Null(grammar.GetSymbol("Constant"));

            // Act
            grammar.AllowConstants = true;

            // Assert
            Assert.True(grammar.AllowConstants);
            Assert.NotNull(grammar.GetSymbol("Constant"));
        }

        [Fact]
        public void AllowDivision_SetToFalse_RemovesDivisionSymbol()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(); // Division allowed by default
            Assert.NotNull(grammar.GetSymbol("Divide"));

            // Act
            grammar.AllowDivision = false;

            // Assert
            Assert.False(grammar.AllowDivision);
            Assert.Null(grammar.GetSymbol("Divide"));
            // Ensure other function symbols are still present
            Assert.NotNull(grammar.GetSymbol("Add"));
        }

        [Fact]
        public void AllowDivision_SetToTrue_AddsDivisionSymbol()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" }, allowDivision: false);
            Assert.Null(grammar.GetSymbol("Divide"));

            // Act
            grammar.AllowDivision = true;

            // Assert
            Assert.True(grammar.AllowDivision);
            Assert.NotNull(grammar.GetSymbol("Divide"));
        }

        [Fact]
        public void AddVariable_NewVariable_AddsToGrammar()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" });

            // Act
            grammar.AddVariable("Y");

            // Assert
            Assert.NotNull(grammar.GetSymbol("Y"));
            Assert.Contains(grammar.StartSymbols, s => s.Name == "Y");
            Assert.Contains("Y", grammar.VariableNames);
        }

        [Fact]
        public void AddVariable_ExistingVariable_DoesNotDuplicate()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" });
            var initialSymbolCount = grammar.Symbols.Count();
            var initialStartSymbolCount = grammar.StartSymbols.Count();

            // Act
            grammar.AddVariable("X");

            // Assert
            Assert.Equal(initialSymbolCount, grammar.Symbols.Count());
            Assert.Equal(initialStartSymbolCount, grammar.StartSymbols.Count());
        }

        [Fact]
        public void AddVariable_NullOrEmpty_ThrowsArgumentException()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => grammar.AddVariable(null));
            Assert.Throws<ArgumentException>(() => grammar.AddVariable(""));
            Assert.Throws<ArgumentException>(() => grammar.AddVariable("   "));
        }

        [Fact]
        public void RemoveVariable_ExistingVariable_RemovesFromGrammar()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            Assert.NotNull(grammar.GetSymbol("Y"));

            // Act
            grammar.RemoveVariable("Y");

            // Assert
            Assert.DoesNotContain("Y", grammar.VariableNames);
            Assert.Null(grammar.GetSymbol("Y"));
            Assert.DoesNotContain(grammar.StartSymbols, s => s.Name == "Y");
        }

        [Fact]
        public void RemoveVariable_LastVariable_ThrowsInvalidOperationException()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => grammar.RemoveVariable("X"));
        }

        [Fact]
        public void RemoveVariable_NonExistingVariable_DoesNothing()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            var initialSymbolCount = grammar.Symbols.Count();
            var initialVariableCount = grammar.VariableNames.Count();

            // Act
            grammar.RemoveVariable("Z");

            // Assert
            Assert.Equal(initialSymbolCount, grammar.Symbols.Count());
            Assert.Equal(initialVariableCount, grammar.VariableNames.Count());
        }
        
        [Fact]
        public void Clone_CreatesDeepCopy()
        {
            // Arrange
            var originalGrammar = new SymbolicRegressionGrammar(new[] { "X1", "X2" }, allowConstants: true, allowDivision: true);
            originalGrammar.MaximumExpressionDepth = 10;

            // Act
            var cloner = new Core.Cloner();
            var clonedGrammar = originalGrammar.Clone(cloner) as SymbolicRegressionGrammar;

            // Assert
            Assert.NotNull(clonedGrammar);
            Assert.NotSame(originalGrammar, clonedGrammar);
            Assert.Equal(originalGrammar.Name, clonedGrammar.Name);
            Assert.Equal(originalGrammar.Description, clonedGrammar.Description);
            Assert.Equal(originalGrammar.MaximumExpressionDepth, clonedGrammar.MaximumExpressionDepth);
            Assert.Equal(originalGrammar.AllowConstants, clonedGrammar.AllowConstants);
            Assert.Equal(originalGrammar.AllowDivision, clonedGrammar.AllowDivision);

            Assert.Equal(originalGrammar.VariableNames.Count(), clonedGrammar.VariableNames.Count());
            foreach (var varName in originalGrammar.VariableNames)
            {
                Assert.Contains(varName, clonedGrammar.VariableNames);
            }
            Assert.NotSame(originalGrammar.VariableNames, clonedGrammar.VariableNames); // Ensure collection is new

            Assert.Equal(originalGrammar.Symbols.Count(), clonedGrammar.Symbols.Count());
            foreach (var symbol in originalGrammar.Symbols)
            {
                var clonedSymbol = clonedGrammar.GetSymbol(symbol.Name);
                Assert.NotNull(clonedSymbol);
                Assert.NotSame(symbol, clonedSymbol); // Symbols should be cloned
                // Corrected to use MinimumArity and MaximumArity from ISymbol
                Assert.Equal(symbol.MinimumArity, clonedSymbol.MinimumArity);
                Assert.Equal(symbol.MaximumArity, clonedSymbol.MaximumArity);
            }
            
            // Modify original and check clone is unaffected
            originalGrammar.AllowConstants = false;
            originalGrammar.AddVariable("X3");
            originalGrammar.MaximumExpressionDepth = 12;

            Assert.True(clonedGrammar.AllowConstants); // Should remain true
            Assert.DoesNotContain("X3", clonedGrammar.VariableNames); // Should not have X3
            Assert.Equal(10, clonedGrammar.MaximumExpressionDepth); // Should remain 10
        }

        [Fact]
        public void CreateSimpleGrammar_CreatesCorrectGrammar()
        {
            // Arrange
            var variables = new[] { "A", "B" };

            // Act
            var grammar = SymbolicRegressionGrammar.CreateSimpleGrammar(variables);

            // Assert
            Assert.False(grammar.AllowConstants);
            Assert.False(grammar.AllowDivision);
            Assert.Contains("A", grammar.VariableNames);
            Assert.Contains("B", grammar.VariableNames);
            Assert.NotNull(grammar.GetSymbol("Add"));
            Assert.NotNull(grammar.GetSymbol("Subtract"));
            Assert.NotNull(grammar.GetSymbol("Multiply"));
            Assert.Null(grammar.GetSymbol("Divide"));
            Assert.Null(grammar.GetSymbol("Constant"));
        }

        [Fact]
        public void CreateStandardGrammar_CreatesCorrectGrammar()
        {
            // Arrange
            var variables = new[] { "X" };

            // Act
            var grammar = SymbolicRegressionGrammar.CreateStandardGrammar(variables);

            // Assert
            Assert.True(grammar.AllowConstants);
            Assert.True(grammar.AllowDivision);
            Assert.Contains("X", grammar.VariableNames);
            Assert.NotNull(grammar.GetSymbol("Add"));
            Assert.NotNull(grammar.GetSymbol("Subtract"));
            Assert.NotNull(grammar.GetSymbol("Multiply"));
            Assert.NotNull(grammar.GetSymbol("Divide"));
            Assert.NotNull(grammar.GetSymbol("Constant"));
        }

        [Fact]
        public void ValidateForRegression_ValidGrammar_ReturnsTrue()
        {
            // Arrange
            var grammar = new SymbolicRegressionGrammar(new[] { "X" });
            // Basic validation: has start symbols, function symbols, terminal symbols
            Assert.NotEmpty(grammar.StartSymbols);
            // Corrected to use Assert.Contains with a predicate
            Assert.Contains(grammar.Symbols, s => s.MinimumArity > 0); // Function symbols
            Assert.Contains(grammar.Symbols, s => s.MaximumArity == 0); // Terminal symbols

            // Act
            var isValid = grammar.ValidateForRegression();

            // Assert
            Assert.True(isValid);
        }
    }
}
