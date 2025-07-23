using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Grammars
{
    public class GrammarSymbolSelectionTests
    {
        private class TestGrammar : SymbolicExpressionTreeGrammar
        {
            public TestGrammar() : base("TestGrammar", "Test grammar") { }

            public void ConfigureAllowed(ISymbol parent, IEnumerable<ISymbol> children) => SetAllowedChildSymbols(parent, children);
            public void AddStart(ISymbol symbol) => AddStartSymbol(symbol);
            public void RemoveStart(ISymbol symbol) => RemoveStartSymbol(symbol);
        }

        /// <summary>
        /// Setting allowed child symbols should restrict children to the provided set.
        /// </summary>
        [Fact]
        public void SetAllowedChildSymbols_DefinesChildRestrictions()
        {
            var grammar = new TestGrammar();
            var add = new Addition();
            var variable = new Variable();

            grammar.AddSymbol(add);
            grammar.AddSymbol(variable);
            grammar.ConfigureAllowed(add, new[] { variable });

            var allowed = grammar.GetAllowedChildSymbols(add).ToList();
            Assert.Single(allowed);
            Assert.Contains(variable, allowed);
            Assert.True(grammar.IsAllowedChildSymbol(add, variable));
            Assert.False(grammar.IsAllowedChildSymbol(add, add));
        }

        /// <summary>
        /// By default, a symbol should not allow itself as a child when no explicit rule is set.
        /// </summary>
        [Fact]
        public void DefaultAllowedChildSymbols_ExcludeParentByDefault()
        {
            var grammar = new TestGrammar();
            var add = new Addition();
            var variable = new Variable();
            grammar.AddSymbol(add);
            grammar.AddSymbol(variable);

            var allowed = grammar.GetAllowedChildSymbols(add).ToList();
            Assert.DoesNotContain(add, allowed);
            Assert.Contains(variable, allowed);
        }

        /// <summary>
        /// Adding and removing start symbols should update the StartSymbols collection.
        /// </summary>
        [Fact]
        public void AddStartSymbolAndRemoveStartSymbol_ModifiesCollection()
        {
            var grammar = new TestGrammar();
            var add = new Addition();
            grammar.AddSymbol(add);

            Assert.DoesNotContain(add, grammar.StartSymbols);
            grammar.AddStart(add);
            Assert.Contains(add, grammar.StartSymbols);
            grammar.RemoveStart(add);
            Assert.DoesNotContain(add, grammar.StartSymbols);
        }
    }
}
