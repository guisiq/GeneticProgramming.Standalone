using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;

namespace GeneticProgramming.Abstractions.Operators
{
    /// <summary>
    /// Interface for selection operators working on symbolic expression trees.
    /// </summary>
    public interface ISymbolicExpressionTreeSelector : IOperator
    {
        /// <summary>
        /// Selects an individual from the given population.
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="population">Available individuals</param>
        /// <param name="fitness">Fitness evaluation function</param>
        /// <returns>Selected individual</returns>
        ISymbolicExpressionTree Select(IRandom random, IList<ISymbolicExpressionTree> population, Func<ISymbolicExpressionTree, double> fitness);
    }
}
