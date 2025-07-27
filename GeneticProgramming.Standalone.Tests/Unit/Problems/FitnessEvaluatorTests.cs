using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Problems.Samples;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Problems
{
    /// <summary>
    /// Tests for the fitness evaluator classes and their integration with the algorithm.
    /// </summary>
    public class FitnessEvaluatorTests
    {
        [Fact]
        public void RegressionEvaluator_PerfectTree_YieldsZeroError()
        {
            var grammar = new SymbolicRegressionGrammar(SimpleRegressionProblem.VariableNames);
            var evaluator = new RegressionFitnessEvaluator(SimpleRegressionProblem.Inputs, SimpleRegressionProblem.Targets, SimpleRegressionProblem.VariableNames);

            var multiply = grammar.GetSymbol(MathematicalSymbols.Multiplication.Name)!;
            var variable = (Variable)grammar.GetSymbol("X")!;
            var constant = (Constant)grammar.GetSymbol("Constant")!;

            var tree = new SymbolicExpressionTree();
            var root = new SymbolicExpressionTreeNode(multiply);
            root.AddSubtree(new VariableTreeNode(variable));
            root.AddSubtree(new ConstantTreeNode(constant, 2.0));
            tree.Root = root;

            double fitness = evaluator.Evaluate(tree);

            Assert.Equal(0.0, -fitness, 6); // fitness is negative MSE
        }

        [Fact]
        public void Algorithm_UsesProvidedFitnessEvaluator()
        {
            var grammar = new SymbolicRegressionGrammar(SimpleRegressionProblem.VariableNames);
            var evaluator = new RegressionFitnessEvaluator(SimpleRegressionProblem.Inputs, SimpleRegressionProblem.Targets, SimpleRegressionProblem.VariableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(1),
                PopulationSize = 5,
                MaxGenerations = 1,
                FitnessEvaluator = evaluator
            };

            algorithm.Run();

            Assert.True(algorithm.BestFitness > double.NegativeInfinity);
        }
    }
}
