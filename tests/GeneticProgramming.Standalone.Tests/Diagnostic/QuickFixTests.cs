using System;
using System.Collections.Generic;
using Xunit;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Problems.Evaluators;

namespace GeneticProgramming.Standalone.Tests
{
    /// <summary>
    /// Quick integration tests to validate the refactored FunctionalSymbol system
    /// </summary>
    public class QuickFixTests
    {
        [Fact]
        public void MathematicalSymbols_AllOperationsWork()
        {
            // Test basic mathematical operations
            var variables = new Dictionary<string, double>();
            
            Assert.Equal(7.0, MathematicalSymbols.Addition.Evaluate(new[] { 3.0, 4.0 }, variables), 2);
            Assert.Equal(-1.0, MathematicalSymbols.Subtraction.Evaluate(new[] { 3.0, 4.0 }, variables), 2);
            Assert.Equal(12.0, MathematicalSymbols.Multiplication.Evaluate(new[] { 3.0, 4.0 }, variables), 2);
            Assert.Equal(0.75, MathematicalSymbols.Division.Evaluate(new[] { 3.0, 4.0 }, variables), 2);
        }

        [Fact]
        public void StatisticsSymbols_VariadicOperationsWork()
        {
            var variables = new Dictionary<string, double>();
            
            // Test Mean
            var mean = StatisticsSymbols.Mean.Evaluate(new[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, variables);
            Assert.Equal(3.0, mean, 2);
            
            // Test Sum (from ArraySymbols)
            var sum = ArraySymbols.Sum.Evaluate(new[] { 1.0, 2.0, 3.0, 4.0 }, variables);
            Assert.Equal(10.0, sum, 2);
        }

        [Fact]
        public void ExpressionInterpreter_EvaluatesCorrectly()
        {
            var interpreter = new ExpressionInterpreter();
            var variables = new Dictionary<string, double> { { "X", 3.0 }, { "Y", 4.0 } };
            
            // Create simple tree: X + Y
            var root = new SymbolicExpressionTreeNode(MathematicalSymbols.Addition);
            root.AddSubtree(CreateVariableNode("X"));
            root.AddSubtree(CreateVariableNode("Y"));
            
            var tree = new SymbolicExpressionTree(root);
            var result = interpreter.Evaluate(tree, variables);
            
            Assert.Equal(7.0, result, 2);
        }

        [Fact]
        public void SymbolicRegressionGrammar_ContainsCorrectSymbols()
        {
            var grammar = new SymbolicRegressionGrammar(new[] { "X", "Y" });
            
            // Check that the grammar contains our new symbols
            Assert.Contains(grammar.Symbols, s => s.Name == "Addition");
            Assert.Contains(grammar.Symbols, s => s.Name == "Multiplication");
            Assert.Contains(grammar.Symbols, s => s.Name == "X");  // Variable name should be exactly "X"
            Assert.Contains(grammar.Symbols, s => s.Name == "Constant");
        }

        [Fact]
        public void FunctionalSymbol_ImplementsIEvaluable()
        {
            // Test that our symbols implement IEvaluable
            Assert.IsAssignableFrom<IEvaluable<double>>(MathematicalSymbols.Addition);
            Assert.IsAssignableFrom<IEvaluable<double>>(StatisticsSymbols.Mean);
            Assert.IsAssignableFrom<IEvaluable<double>>(ArraySymbols.Sum);
        }

        private ISymbolicExpressionTreeNode CreateVariableNode(string name)
        {
            var variable = new Variable { Name = name };
            return new VariableTreeNode(variable) { VariableName = name };
        }
    }
}
