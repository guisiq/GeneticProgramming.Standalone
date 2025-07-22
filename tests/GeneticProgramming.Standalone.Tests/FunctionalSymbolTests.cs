using System;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Expressions
{
    public class FunctionalSymbolTests
    {
        [Fact]
        public void OperationDelegate_ExecutesCorrectly()
        {
            var sym = new FunctionalSymbol("Double","", a => a[0] * 2, 1, 1);
            double result = sym.Operation(new double[] {3});
            Assert.Equal(6, result, 6);
        }

        [Fact]
        public void CreateTreeNode_ReturnsNodeWithSymbol()
        {
            var sym = new FunctionalSymbol("Sum","", a => a[0] + a[1], 2, 2);
            var node = sym.CreateTreeNode();
            Assert.IsType<SymbolicExpressionTreeNode>(node);
            Assert.Same(sym, node.Symbol);
        }

        [Fact]
        public void Clone_ReturnsDistinctCopy()
        {
            var sym = new FunctionalSymbol("Double","", a => a[0] * 2, 1, 1);
            var clone = (FunctionalSymbol)sym.Clone(new Cloner());
            Assert.NotSame(sym, clone);
            Assert.Equal(sym.Operation(new double[]{2}), clone.Operation(new double[]{2}), 6);
            Assert.Equal(sym.MinimumArity, clone.MinimumArity);
            Assert.Equal(sym.MaximumArity, clone.MaximumArity);
        }
    }
}
