using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Problems.Evaluators;

namespace GeneticProgramming.Standalone.Tests
{
    /// <summary>
    /// Debug tests to check symbol names
    /// </summary>
    public class DebugSymbolNamesTests
    {
        [Fact]
        public void DebugGrammarSymbolNames()
        {
            var grammar = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            
            // Print all symbol names for debugging
            var symbolNames = grammar.Symbols.Select(s => s.Name).ToList();
            
            // This will always pass but show us the actual names
            Assert.True(symbolNames.Count > 0);
            
            // Let's check each expected name
            Assert.Contains("Addition", symbolNames);
            Assert.Contains("Subtraction", symbolNames);
            Assert.Contains("Multiplication", symbolNames);
            Assert.Contains("Division", symbolNames);
            
            // Check variable name (should be X since we passed "X")
            Assert.Contains("X", symbolNames);
            
            // Check constant
            Assert.Contains("Constant", symbolNames);
        }
    }
}
