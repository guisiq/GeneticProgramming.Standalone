using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Interface for symbols that represent operations or values in symbolic expression trees
    /// </summary>
    public interface ISymbol : IItem
    {
        /// <summary>
        /// Gets the minimum number of arguments (subtrees) this symbol can have
        /// </summary>
        int MinimumArity { get; }

        /// <summary>
        /// Gets the maximum number of arguments (subtrees) this symbol can have
        /// </summary>
        int MaximumArity { get; }

        /// <summary>
        /// Gets or sets the initial frequency of this symbol in the grammar
        /// Used for random generation of trees
        /// </summary>
        double InitialFrequency { get; set; }

        /// <summary>
        /// Gets or sets whether this symbol is enabled in the grammar
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Creates a tree node for this symbol
        /// </summary>
        /// <returns>A new tree node instance</returns>
        ISymbolicExpressionTreeNode CreateTreeNode();

        /// <summary>
        /// Gets the formatter for displaying this symbol
        /// </summary>
        string GetFormatString();
    }

    /// <summary>
    /// Interface for read-only symbols that cannot be modified after creation
    /// </summary>
    public interface IReadOnlySymbol : ISymbol
    {
        // Marker interface for symbols that should not be modified
    }
}
