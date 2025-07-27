using GeneticProgramming.Expressions;
using System;

namespace GeneticProgramming.Algorithms
{
    /// <summary>
    /// Event arguments for generation completed events
    /// </summary>
    public class GenerationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the generation number
        /// </summary>
        public int Generation { get; }

        /// <summary>
        /// Gets the best fitness in this generation
        /// </summary>
        public double BestFitness { get; }

        /// <summary>
        /// Gets the average fitness in this generation
        /// </summary>
        public double AverageFitness { get; }

        /// <summary>
        /// Gets the best individual in this generation
        /// </summary>
        public ISymbolicExpressionTree BestIndividual { get; }

        /// <summary>
        /// Initializes a new instance of the GenerationEventArgs class
        /// </summary>
        /// <param name="generation">The generation number</param>
        /// <param name="bestFitness">The best fitness</param>
        /// <param name="averageFitness">The average fitness</param>
        /// <param name="bestIndividual">The best individual</param>
        public GenerationEventArgs(int generation, double bestFitness, double averageFitness, ISymbolicExpressionTree bestIndividual)
        {
            Generation = generation;
            BestFitness = bestFitness;
            AverageFitness = averageFitness;
            BestIndividual = bestIndividual;
        }
    }
}
