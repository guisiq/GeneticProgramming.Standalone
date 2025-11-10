using System.Collections.Generic;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Compilation;
using GeneticProgramming.Expressions.Symbols;
using Xunit;

namespace GeneticProgramming.Standalone.Tests.Unit.Compilation
{
    public class TreeCompilerTests
    {
        [Fact]
        public void Compile_AdditionTree_ReturnsExpectedSum()
        {
            var tree = BuildAdditionTree();
            var compiler = new TreeCompiler<double>();
            var func = compiler.Compile(tree);

            var variables = new Dictionary<string, double>
            {
                ["x0"] = 3.0,
                ["x1"] = 5.0
            };

            var result = func(variables);

            Assert.Equal(8.0, result, 5);
        }

        [Fact]
        public void Compile_NestedOperations_ReturnsExpectedValue()
        {
            var addSymbol = MathematicalSymbols.Addition;
            var multSymbol = MathematicalSymbols.Multiplication;
            var variable = new Variable<double>();
            var constant = new Constant<double>();

            var innerAdd = new SymbolicExpressionTreeNode<double>(addSymbol);
            innerAdd.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x0" });
            innerAdd.AddSubtree(new ConstantTreeNode<double>(constant, 2.0));

            var root = new SymbolicExpressionTreeNode<double>(multSymbol);
            root.AddSubtree(innerAdd);
            root.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x1" });

            var tree = new SymbolicExpressionTree<double> { Root = root };

            var compiler = new TreeCompiler<double>();
            var func = compiler.Compile(tree);

            var variables = new Dictionary<string, double>
            {
                ["x0"] = 4.0,
                ["x1"] = 3.0
            };

            var result = func(variables);

            // (x0 + 2) * x1 => (4 + 2) * 3 = 18
            Assert.Equal(18.0, result, 5);
        }

        [Fact]
        public void Compile_NodeWithoutCompilableSymbol_UsesEvaluateMethod()
        {
            var countingSymbol = new CountingTerminalSymbol();
            var countingNode = (CountingTerminalNode)countingSymbol.CreateTreeNode();

            var tree = new SymbolicExpressionTree<double> { Root = countingNode };
            CountingTerminalNode.CallCount = 0;

            var compiler = new TreeCompiler<double>();
            var func = compiler.Compile(tree);

            var result = func(new Dictionary<string, double>());

            Assert.Equal(CountingTerminalNode.ExpectedValue, result, 5);
            Assert.Equal(1, CountingTerminalNode.CallCount);
        }

        private static SymbolicExpressionTree<double> BuildAdditionTree()
        {
            var addSymbol = MathematicalSymbols.Addition;
            var variable = new Variable<double>();

            var root = new SymbolicExpressionTreeNode<double>(addSymbol);
            root.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x0" });
            root.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x1" });

            return new SymbolicExpressionTree<double> { Root = root };
        }

        private sealed class CountingTerminalSymbol : TerminalSymbol<double>
        {
            public CountingTerminalSymbol() : base("Counting", "Counts Evaluate invocations")
            {
            }

            private CountingTerminalSymbol(CountingTerminalSymbol original, Cloner cloner) : base(original, cloner)
            {
            }

            protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
            {
                return new CountingTerminalSymbol(this, cloner);
            }

            public override string GetFormatString() => "Count";

            public override ISymbolicExpressionTreeNode<double> CreateTreeNode()
            {
                return new CountingTerminalNode(this);
            }
        }

        private sealed class CountingTerminalNode : TerminalTreeNode<double>
        {
            public const double ExpectedValue = 7.0;
            public static int CallCount;

            public CountingTerminalNode(ISymbol<double> symbol) : base(symbol)
            {
            }

            private CountingTerminalNode(CountingTerminalNode original, Cloner cloner) : base(original, cloner)
            {
            }

            protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
            {
                return new CountingTerminalNode(this, cloner);
            }

            public override double Evaluate(double[] arguments, IDictionary<string, double> variables)
            {
                CallCount++;
                return ExpectedValue;
            }
        }
    }
}
