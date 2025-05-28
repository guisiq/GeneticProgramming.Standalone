using GeneticProgramming.Standalone.Core;

namespace GeneticProgramming.Standalone.Operators
{
    /// <summary>
    /// Base interface for all operators that work with symbolic expression trees
    /// </summary>
    public interface ISymbolicExpressionTreeOperator : IItem
    {
        /// <summary>
        /// Gets or sets the symbolic expression tree grammar used by this operator
        /// </summary>
        ISymbolicExpressionTreeGrammar? SymbolicExpressionTreeGrammar { get; set; }
    }

    /// <summary>
    /// Interface for operators that create new symbolic expression trees
    /// </summary>
    public interface ISymbolicExpressionTreeCreator : ISymbolicExpressionTreeOperator
    {
        /// <summary>
        /// Creates a new symbolic expression tree
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="grammar">Grammar to use for tree creation</param>
        /// <param name="maxTreeLength">Maximum length of the created tree</param>
        /// <param name="maxTreeDepth">Maximum depth of the created tree</param>
        /// <returns>A new symbolic expression tree</returns>
        ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionTreeGrammar grammar, int maxTreeLength, int maxTreeDepth);
    }

    /// <summary>
    /// Interface for operators that perform crossover between two parent trees
    /// </summary>
    public interface ISymbolicExpressionTreeCrossover : ISymbolicExpressionTreeOperator
    {
        /// <summary>
        /// Performs crossover between two parent trees to produce offspring
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="parent0">First parent tree</param>
        /// <param name="parent1">Second parent tree</param>
        /// <returns>Offspring tree created by crossover</returns>
        ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1);
    }

    /// <summary>
    /// Interface for operators that perform mutation on a single tree
    /// </summary>
    public interface ISymbolicExpressionTreeMutator : ISymbolicExpressionTreeOperator
    {
        /// <summary>
        /// Performs mutation on a tree
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="symbolicExpressionTree">Tree to mutate</param>
        /// <returns>Mutated tree</returns>
        ISymbolicExpressionTree Mutate(IRandom random, ISymbolicExpressionTree symbolicExpressionTree);
    }
}
