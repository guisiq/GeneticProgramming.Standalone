using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Interface for evaluating the fitness of a symbolic expression tree.
    /// </summary>
    public interface IFitnessEvaluator
    {
        /// <summary>
        /// Computes the fitness of the given tree. Higher is better.
        /// </summary>
        double Evaluate(ISymbolicExpressionTree tree);
    }
}
