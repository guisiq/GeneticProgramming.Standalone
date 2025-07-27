using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Algorithms
{
    /// <summary>
    /// Tests the default fitness evaluation logic.
    /// </summary>
    public class EvaluationLogicTests
    {
        /// <summary>
        /// EvaluateFitness should return the negative tree length.
        /// </summary>
        [Fact]
        public void EvaluateFitness_ReturnsNegativeLength()
        {
            var grammar = new SymbolicRegressionGrammar();
            var constSym = (Constant)grammar.GetSymbol("Constant")!;
            var tree = new SymbolicExpressionTree(new ConstantTreeNode(constSym, 1));

            var alg = new GeneticProgrammingAlgorithm();
            var fitness = alg.EvaluateFitness(tree);

            Assert.Equal(-1, fitness);
        }
    }
}
