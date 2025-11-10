using System.Collections.Generic;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Problems.Evaluators;
using Xunit;

namespace GeneticProgramming.Standalone.Tests.Integration.Compilation
{
    public class CompiledFitnessEvaluatorTests
    {
        [Fact]
        public void RegressionEvaluator_SequentialCompiledEvaluation_MatchesExpectedFitness()
        {
            var (inputs, targets, tree) = BuildPerfectRegressionScenario();
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "x0", "x1" })
            {
                EnableParallelEvaluation = false
            };

            var fitness = evaluator.Evaluate(tree);

            Assert.Equal(0.0, fitness, 6);
        }

        [Fact]
        public void RegressionEvaluator_ParallelCompiledEvaluation_MatchesExpectedFitness()
        {
            var (inputs, targets, tree) = BuildPerfectRegressionScenario();
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "x0", "x1" })
            {
                EnableParallelEvaluation = true,
                ParallelThreshold = 1
            };

            var fitness = evaluator.Evaluate(tree);

            Assert.Equal(0.0, fitness, 6);
        }

        [Fact]
        public void ClassificationEvaluator_CompiledEvaluation_ProducesPerfectAccuracy()
        {
            var inputs = new[]
            {
                new[] { 0.2, 0.1 },
                new[] { 0.9, 0.4 },
                new[] { 0.7, 0.8 },
                new[] { 0.1, 0.9 }
            };

            var targets = new[] { 0, 1, 1, 0 };
            var variableNames = new[] { "x0", "x1" };
            var tree = BuildClassificationTree();

            var evaluator = new ClassificationFitnessEvaluator(inputs, targets, variableNames)
            {
                EnableParallelEvaluation = true,
                ParallelThreshold = 1
            };

            var accuracy = evaluator.Evaluate(tree);

            Assert.Equal(1.0, accuracy, 6);
        }

        private static (double[][] Inputs, double[] Targets, ISymbolicExpressionTree<double> Tree) BuildPerfectRegressionScenario()
        {
            var inputs = new[]
            {
                new[] { 1.0, 2.0 },
                new[] { 2.0, 3.0 },
                new[] { 3.0, 4.0 },
                new[] { 4.0, 5.0 }
            };

            var targets = new[]
            {
                3.0,
                5.0,
                7.0,
                9.0
            };

            var tree = BuildAdditionTree();
            return (inputs, targets, tree);
        }

        private static ISymbolicExpressionTree<double> BuildAdditionTree()
        {
            var addSymbol = MathematicalSymbols.Addition;
            var variable = new Variable<double>();

            var root = new SymbolicExpressionTreeNode<double>(addSymbol);
            root.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x0" });
            root.AddSubtree(new VariableTreeNode<double>(variable) { VariableName = "x1" });

            return new SymbolicExpressionTree<double> { Root = root };
        }

        private static ISymbolicExpressionTree<double> BuildClassificationTree()
        {
            var variable = new Variable<double>();
            var root = new VariableTreeNode<double>(variable) { VariableName = "x0" };
            return new SymbolicExpressionTree<double> { Root = root };
        }
    }
}
