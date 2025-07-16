using System;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Problems.Samples;

namespace GeneticProgramming.Examples
{
    /// <summary>
    /// Demonstrates running the GP algorithm on a simple regression problem.
    /// </summary>
    public static class RegressionExample
    {
        public static void Run()
        {
            var grammar = new SymbolicRegressionGrammar(SimpleRegressionProblem.VariableNames);
            var evaluator = new RegressionFitnessEvaluator(SimpleRegressionProblem.Inputs, SimpleRegressionProblem.Targets, SimpleRegressionProblem.VariableNames);

            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Random = new MersenneTwister(42),
                PopulationSize = 20,
                MaxGenerations = 5,
                FitnessEvaluator = evaluator
            };

            algorithm.GenerationCompleted += (s, e) =>
            {
                Console.WriteLine($"Generation {e.Generation}: bestFitness={e.BestFitness:F4}");
            };

            algorithm.Run();
            Console.WriteLine($"Best fitness: {algorithm.BestFitness:F4}");
        }
    }
}
