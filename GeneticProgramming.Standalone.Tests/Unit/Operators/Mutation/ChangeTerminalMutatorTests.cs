using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Operators
{
    /// <summary>
    /// Tests for ChangeTerminalMutator affecting constant and variable nodes.
    /// </summary>
    public class ChangeTerminalMutatorTests
    {
        /// <summary>
        /// Mutating a constant-only tree should modify the constant value.
        /// </summary>
        [Fact]
        public void Mutate_ChangesConstantValue()
        {
            var grammar = new SymbolicRegressionGrammar();
            var constSymbol = (Constant)grammar.GetSymbol("Constant")!;
            var constNode = (ConstantTreeNode)constSymbol.CreateTreeNode();
            constNode.Value = 1.0;
            var tree = new SymbolicExpressionTree(constNode);

            var mutator = new ChangeTerminalMutator { SymbolicExpressionTreeGrammar = grammar, ConstantMutationRange = 5.0 };
            var mutated = mutator.Mutate(new MersenneTwister(5), tree);

            var mutatedValue = ((ConstantTreeNode)mutated.Root).Value;
            Assert.NotEqual(1.0, mutatedValue);
        }

        /// <summary>
        /// Mutating a variable node should assign a different variable name.
        /// </summary>
        [Fact]
        public void Mutate_ChangesVariableName()
        {
            var grammar = new SymbolicRegressionGrammar(new[] { "X0", "X1" }, allowConstants: false);
            var variableSymbol = (Variable)grammar.GetSymbol("X0")!;
            var variableNode = new VariableTreeNode(variableSymbol, "X0");
            var tree = new SymbolicExpressionTree(variableNode);

            var mutator = new ChangeTerminalMutator { SymbolicExpressionTreeGrammar = grammar };
            var mutated = mutator.Mutate(new MersenneTwister(6), tree);

            var mutatedName = ((VariableTreeNode)mutated.Root).VariableName;
            Assert.NotEqual("X0", mutatedName);
        }
    }
}
