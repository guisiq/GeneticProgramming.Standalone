using System;

namespace GeneticProgramming.Abstractions.Optimization
{
    /// <summary>
    /// Represents a generic optimization algorithm.
    /// </summary>
    public interface IOptimizationAlgorithm
    {
        /// <summary>
        /// Executes the optimization process.
        /// </summary>
        void Run();

        /// <summary>
        /// Requests termination of the optimization.
        /// </summary>
        void Stop();

        /// <summary>
        /// Event fired when an iteration or generation completes.
        /// </summary>
        event EventHandler? IterationCompleted;
    }
}
