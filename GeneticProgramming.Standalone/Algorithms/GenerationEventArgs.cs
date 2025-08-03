using GeneticProgramming.Expressions;
using System;

namespace GeneticProgramming.Algorithms
{
    /// <summary>
    /// Event arguments for generation completed events
    /// </summary>
    public class GenerationEventArgs<T> : EventArgs where T : struct
    {
        /// <summary>
        /// Gets the generation number
        /// </summary>
        public int Generation { get; }

        /// <summary>
        /// Gets the best fitness in this generation
        /// </summary>
        public T BestFitness { get; }

        /// <summary>
        /// Gets the average fitness in this generation
        /// </summary>
        public T AverageFitness { get; }

        /// <summary>
        /// Gets the best individual in this generation
        /// </summary>
        public ISymbolicExpressionTree<T> BestIndividual { get; }

        /// <summary>
        /// Initializes a new instance of the GenerationEventArgs class
        /// </summary>
        /// <param name="generation">The generation number</param>
        /// <param name="bestFitness">The best fitness</param>
        /// <param name="averageFitness">The average fitness</param>
        /// <param name="bestIndividual">The best individual</param>
        public GenerationEventArgs(int generation, T bestFitness, T averageFitness, ISymbolicExpressionTree<T> bestIndividual)
        {
            Generation = generation;
            BestFitness = bestFitness;
            AverageFitness = averageFitness;
            BestIndividual = bestIndividual;
        }
    }
}
