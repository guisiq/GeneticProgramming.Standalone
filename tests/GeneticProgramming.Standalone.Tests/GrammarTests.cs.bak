using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Grammars
{
    public class GrammarTests
    {
        /// <summary>
        /// Ensures symbols can be added to and removed from a grammar.
        /// </summary>
        [Fact]
        public void Grammar_AddAndRemoveSymbols()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            var initialCount = grammar.Symbols.Count();

            var newSymbol = new Addition { Name = "Add2" };
            grammar.AddSymbol(newSymbol);

            Assert.True(grammar.ContainsSymbol(newSymbol));
            Assert.Equal(initialCount + 1, grammar.Symbols.Count());

            grammar.RemoveSymbol(newSymbol);
            Assert.False(grammar.ContainsSymbol(newSymbol));
            Assert.Equal(initialCount, grammar.Symbols.Count());
        }

        /// <summary>
        /// Checks that the grammar returns the correct allowed child symbols for a given parent.
        /// </summary>
        [Fact]
        public void Grammar_GetAllowedSymbols()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            var addition = grammar.GetSymbol("Addition")!;

            var allowed = grammar.GetAllowedChildSymbols(addition).ToList();
            Assert.NotEmpty(allowed);
            Assert.Contains(allowed, s => s.Name == "Variable");
            Assert.Contains(allowed, s => s.Name == "Constant");
        }

        /// <summary>
        /// Validates that a grammar reports its validity correctly before and after modifications.
        /// </summary>
        [Fact]
        public void Grammar_ValidateGrammar()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            Assert.True(grammar.Validate());

            // Remove all symbols to invalidate
            foreach (var symbol in grammar.Symbols.ToList())
                grammar.RemoveSymbol(symbol);

            Assert.False(grammar.Validate());
        }
    }
}
