using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Grammars
{
    public class GrammarTests
    {
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
