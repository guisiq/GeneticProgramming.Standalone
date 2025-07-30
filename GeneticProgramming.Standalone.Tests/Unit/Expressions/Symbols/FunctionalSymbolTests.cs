using System;
using System.Collections.Generic;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Expressions
{
    public class FunctionalSymbolTests
    {
        [Fact]
        public void OperationDelegate_ExecutesCorrectly()
        {
            var sym = new FunctionalSymbol<double>("Double","", a => a[0] * 2, 1, 1);
            double result = sym.Operation(new double[] {3});
            Assert.Equal(6, result, 6);
        }

        [Fact]
        public void CreateTreeNode_ReturnsNodeWithSymbol()
        {
            var sym = new FunctionalSymbol<double>("Sum","", a => a[0] + a[1], 2, 2);
            var node = sym.CreateTreeNode();
            Assert.IsType<SymbolicExpressionTreeNode>(node);
            Assert.Same(sym, node.Symbol);
        }

        [Fact]
        public void Clone_ReturnsDistinctCopy()
        {
            var sym = new FunctionalSymbol<double>("Double","", a => a[0] * 2, 1, 1);
            var clone = (FunctionalSymbol<double>)sym.Clone(new Cloner());
            Assert.NotSame(sym, clone);
            Assert.Equal(sym.Operation(new double[]{2}), clone.Operation(new double[]{2}), 6);
            Assert.Equal(sym.MinimumArity, clone.MinimumArity);
            Assert.Equal(sym.MaximumArity, clone.MaximumArity);
        }

        [Fact]
        public void Evaluate_WithValidChildren_ReturnsCorrectResult()
        {
            var sym = new FunctionalSymbol<double>("Add","Addition", a => a[0] + a[1], 2, 2);
            var variables = new Dictionary<string, double>();
            var result = sym.Evaluate(new double[] { 3.0, 4.0 }, variables);
            Assert.Equal(7.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Addition_WorksCorrectly()
        {
            var addition = MathematicalSymbols.Addition;
            var variables = new Dictionary<string, double>();
            var result = addition.Evaluate(new double[] { 5.0, 3.0 }, variables);
            Assert.Equal(8.0, result, 2);
        }

        [Fact]
        public void MathematicalSymbols_Multiplication_WorksCorrectly()
        {
            var multiplication = MathematicalSymbols.Multiplication;
            var variables = new Dictionary<string, double>();
            var result = multiplication.Evaluate(new double[] { 4.0, 3.0 }, variables);
            Assert.Equal(12.0, result, 2);
        }
    }
}
