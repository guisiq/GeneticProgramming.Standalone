using GeneticProgramming.Abstractions.Operators;
using GeneticProgramming.Abstractions.Parameters;
using GeneticProgramming.Abstractions.Optimization;
using GeneticProgramming.Expressions; // Corrected namespace

namespace GeneticProgramming.Abstractions.Optimization
{
    /// <summary>
    /// Interface for genetic programming specific optimization.
    /// </summary>
    public interface IGeneticProgrammingAlgorithm : IOptimizationAlgorithm
    {
        int PopulationSize { get; set; }
        int MaxGenerations { get; set; }
        double CrossoverProbability { get; set; }
        double MutationProbability { get; set; }

        /// <summary>
        /// Evaluates fitness of an individual solution.
        /// </summary>
        double EvaluateFitness(object individual);
    }
}
