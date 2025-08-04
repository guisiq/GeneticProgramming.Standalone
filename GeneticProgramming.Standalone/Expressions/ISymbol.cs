using GeneticProgramming.Core;
using System;

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
        int MaximumArity { get; }        /// <summary>
        /// Gets or sets the initial frequency of this symbol in the grammar
        /// Used for random generation of trees
        /// </summary>
        double InitialFrequency { get; set; }

        /// <summary>
        /// Gets or sets the weight of this symbol for selection
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Gets the name of this symbol
        /// </summary>
        string SymbolName { get; }

        /// <summary>
        /// Gets or sets whether this symbol is enabled in the grammar
        /// </summary>
        bool Enabled { get; set; }

        // /// <summary>
        // /// Creates a tree node for this symbol
        // /// </summary>
        // /// <returns>A new tree node instance</returns>
        ISymbolicExpressionTreeNode CreateTreeNode();

        /// <summary>
        /// Gets the formatter for displaying this symbol
        /// </summary>
        string GetFormatString();
    }

    /// <summary>
    /// Generic interface for symbols that represent operations or values in symbolic expression trees
    /// </summary>
    /// <typeparam name="T">The value type that the symbol operates on (must be a struct)</typeparam>
    public interface ISymbol<T> : ISymbol where T : struct
    {
        /// <summary>
        /// Gets the types that this symbol accepts as input
        /// </summary>
        Type[] InputTypes { get; }

        /// <summary>
        /// Gets the type that this symbol produces as output
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// Creates a generic tree node for this symbol
        /// </summary>
        /// <returns>A new generic tree node instance</returns>
        new ISymbolicExpressionTreeNode<T> CreateTreeNode();

        ISymbolicExpressionTreeNode ISymbol.CreateTreeNode()
        {
            return CreateTreeNode();
        }

        /// <summary>
        /// Validates if a child symbol type is compatible with this symbol at the given position
        /// </summary>
        /// <param name="childOutputType">The output type of the child symbol</param>
        /// <param name="argumentIndex">The position where the child would be placed</param>
        /// <returns>True if compatible, false otherwise</returns>
        bool IsCompatibleChildType(Type childOutputType, int argumentIndex);
    }

    /// <summary>
    /// Interface for read-only symbols that cannot be modified after creation
    /// </summary>
    public interface IReadOnlySymbol : ISymbol
    {
        // Marker interface for symbols that should not be modified
    }
}
