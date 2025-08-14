using GeneticProgramming.Core; // For IDeepCloneable, INotifyPropertyChanged
using GeneticProgramming.Expressions;
using GeneticProgramming.Abstractions.Operators; // For IOperator
using System.ComponentModel; // For INotifyPropertyChanged

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Base interface for all operators that work with symbolic expression trees
    /// </summary>
    public interface ISymbolicExpressionTreeOperator<T> : IOperator, IDeepCloneable, INotifyPropertyChanged where T : notnull
    {
        /// <summary>
        /// Gets or sets the symbolic expression tree grammar used by this operator
        /// </summary>
        ISymbolicExpressionTreeGrammar<T> ? SymbolicExpressionTreeGrammar { get; set; }
    }

    /// <summary>
    /// Interface for operators that create new symbolic expression trees
    /// </summary>
    public interface ISymbolicExpressionTreeCreator<T> : ISymbolicExpressionTreeOperator<T> where T : notnull
    {
        /// <summary>
        /// Creates a new symbolic expression tree
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="grammar">Grammar to use for tree creation</param>
        /// <param name="maxTreeLength">Maximum length of the created tree</param>
        /// <param name="maxTreeDepth">Maximum depth of the created tree</param>
        /// <returns>A new symbolic expression tree</returns>
        ISymbolicExpressionTree<T> CreateTree(IRandom random, ISymbolicExpressionTreeGrammar<T> grammar, int maxTreeLength, int maxTreeDepth);
    }

    /// <summary>
    /// Interface for operators that perform crossover between two parent trees
    /// </summary>
    public interface ISymbolicExpressionTreeCrossover<T> : ISymbolicExpressionTreeOperator<T> where T : notnull
    {
        /// <summary>
        /// Performs crossover between two parent trees to produce offspring
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="parent0">First parent tree</param>
        /// <param name="parent1">Second parent tree</param>
        /// <returns>Offspring tree created by crossover</returns>
        ISymbolicExpressionTree<T> Crossover(IRandom random, ISymbolicExpressionTree<T> parent0, ISymbolicExpressionTree<T> parent1);
    }

    /// <summary>
    /// Interface for operators that perform mutation on a single tree
    /// </summary>
    public interface ISymbolicExpressionTreeMutator<T> : ISymbolicExpressionTreeOperator<T> where T : notnull
    {
        /// <summary>
        /// Performs mutation on a tree
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="symbolicExpressionTree">Tree to mutate</param>
        /// <returns>Mutated tree</returns>
        ISymbolicExpressionTree<T> Mutate(IRandom random, ISymbolicExpressionTree<T> symbolicExpressionTree);
    }
}
