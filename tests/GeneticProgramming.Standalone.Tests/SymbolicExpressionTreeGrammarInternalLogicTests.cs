using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Grammars
{
    /// <summary>
    /// Unit tests for internal logic of SymbolicExpressionTreeGrammar.
    /// </summary>
    public class SymbolicExpressionTreeGrammarInternalLogicTests
    {
        /// <summary>
        /// Adding a zero-arity symbol should automatically register it as a start symbol.
        /// </summary>
        [Fact]
        public void AddSymbol_StartSymbolAutomaticallyAdded_WhenZeroArity()
        {
            var grammar = new SymbolicExpressionTreeGrammar("Test", "Test grammar");
            var terminal = new Variable();

            grammar.AddSymbol(terminal);

            Assert.Contains(terminal, grammar.Symbols);
            Assert.Contains(terminal, grammar.StartSymbols);
        }

        /// <summary>
        /// Duplicate symbol names must cause an exception to be thrown.
        /// </summary>
        [Fact]
        public void AddSymbol_DuplicateName_ThrowsArgumentException()
        {
            var grammar = new SymbolicExpressionTreeGrammar("Test", "Test grammar");
            var add1 = new Addition();
            var add2 = new Addition();

            grammar.AddSymbol(add1);
            Assert.Throws<ArgumentException>(() => grammar.AddSymbol(add2));
        }

        /// <summary>
        /// Removing a symbol should also remove it from the start symbol collection.
        /// </summary>
        [Fact]
        public void RemoveSymbol_RemovesFromStartSymbolsAndSymbols()
        {
            var grammar = new SymbolicExpressionTreeGrammar("Test", "Test grammar");
            var terminal = new Variable();
            grammar.AddSymbol(terminal);

            grammar.RemoveSymbol(terminal);

            Assert.DoesNotContain(terminal, grammar.Symbols);
            Assert.DoesNotContain(terminal, grammar.StartSymbols);
        }

        /// <summary>
        /// GetSymbol should return the registered symbol when found or null otherwise.
        /// </summary>
        [Fact]
        public void GetSymbol_ReturnsSymbolByNameOrNull()
        {
            var grammar = new SymbolicExpressionTreeGrammar("Test", "Test grammar");
            var add = new Addition();
            grammar.AddSymbol(add);

            var fetched = grammar.GetSymbol(add.Name);
            var missing = grammar.GetSymbol("NonExistent");

            Assert.Same(add, fetched);
            Assert.Null(missing);
        }

        /// <summary>
        /// The Changed event should fire whenever symbols are added or removed.
        /// </summary>
        [Fact]
        public void ChangedEvent_FiresOnAddAndRemove()
        {
            var grammar = new SymbolicExpressionTreeGrammar("Test", "Test grammar");
            var terminal = new Variable();
            int changedCount = 0;
            grammar.Changed += (s, e) => changedCount++;

            grammar.AddSymbol(terminal);
            grammar.RemoveSymbol(terminal);

            Assert.Equal(2, changedCount);
        }
    }
}
