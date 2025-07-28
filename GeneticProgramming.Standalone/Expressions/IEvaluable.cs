using System.Collections.Generic;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Interface for symbols that can evaluate themselves given a context and child values.
    /// </summary>
    /// <typeparam name="T">The type of value this symbol evaluates to.</typeparam>
    public interface IEvaluable<T>
    {
        /// <summary>
        /// Evaluates this symbol given the evaluated values of its children and a variable context.
        /// </summary>
        /// <param name="childValues">The evaluated values from child nodes.</param>
        /// <param name="variables">Dictionary mapping variable names to their values.</param>
        /// <returns>The evaluated result of this symbol.</returns>
        T Evaluate(T[] childValues, IDictionary<string, T> variables);
    }
}
