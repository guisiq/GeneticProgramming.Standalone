using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Operators;
using System;
using System.Collections.Generic;

namespace GeneticProgramming.Algorithms
{
    /// <summary>
    /// Interface for genetic programming algorithms
    /// </summary>
    public interface IGeneticProgrammingAlgorithm : IItem
    {
        /// <summary>
        /// Gets or sets the population size
        /// </summary>
        int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of generations
        /// </summary>
        int MaxGenerations { get; set; }

        /// <summary>
        /// Gets or sets the maximum tree length
        /// </summary>
        int MaxTreeLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum tree depth
        /// </summary>
        int MaxTreeDepth { get; set; }

        /// <summary>
        /// Gets or sets the crossover probability
        /// </summary>
        double CrossoverProbability { get; set; }

        /// <summary>
        /// Gets or sets the mutation probability
        /// </summary>
        double MutationProbability { get; set; }

        /// <summary>
        /// Gets or sets the grammar used for tree creation
        /// </summary>
        ISymbolicExpressionTreeGrammar? Grammar { get; set; }

        /// <summary>
        /// Gets or sets the tree creator
        /// </summary>
        ISymbolicExpressionTreeCreator? TreeCreator { get; set; }

        /// <summary>
        /// Gets or sets the crossover operator
        /// </summary>
        ISymbolicExpressionTreeCrossover? Crossover { get; set; }

        /// <summary>
        /// Gets or sets the mutator
        /// </summary>
        ISymbolicExpressionTreeMutator? Mutator { get; set; }

        /// <summary>
        /// Gets or sets the random number generator
        /// </summary>
        IRandom? Random { get; set; }

        /// <summary>
        /// Gets the current generation
        /// </summary>
        int Generation { get; }

        /// <summary>
        /// Gets the current population
        /// </summary>
        IList<ISymbolicExpressionTree> Population { get; }

        /// <summary>
        /// Gets the best individual found so far
        /// </summary>
        ISymbolicExpressionTree? BestIndividual { get; }

        /// <summary>
        /// Gets the fitness of the best individual
        /// </summary>
        double BestFitness { get; }

        /// <summary>
        /// Event raised when a generation is completed
        /// </summary>
        event EventHandler<GenerationEventArgs>? GenerationCompleted;

        /// <summary>
        /// Runs the genetic programming algorithm
        /// </summary>
        void Run();

        /// <summary>
        /// Stops the algorithm execution
        /// </summary>
        void Stop();        /// <summary>
        /// Evaluates the fitness of an individual
        /// </summary>
        /// <param name="individual">The individual to evaluate</param>
        /// <returns>The fitness value (higher is better)</returns>
        double EvaluateFitness(ISymbolicExpressionTree individual);
    }
}
