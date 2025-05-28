using System;
using System.Collections.Generic;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Interface for symbolic expression tree grammars that define the allowed symbols and their relationships.
    /// </summary>
    public interface ISymbolicExpressionTreeGrammar : Core.IItem
    {
        /// <summary>
        /// Gets all symbols available in this grammar.
        /// </summary>
        IEnumerable<ISymbol> Symbols { get; }

        /// <summary>
        /// Gets all symbols that can be used as root symbols.
        /// </summary>
        IEnumerable<ISymbol> StartSymbols { get; }

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <returns>Collection of allowed child symbols.</returns>
        IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent);

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>Collection of allowed child symbols for the specified position.</returns>
        IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int childIndex);

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <returns>True if the child is allowed, false otherwise.</returns>
        bool IsAllowedChildSymbol(ISymbol parent, ISymbol child);

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>True if the child is allowed at the specified position, false otherwise.</returns>
        bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int childIndex);

        /// <summary>
        /// Gets the maximum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Maximum allowed subtree count.</returns>
        int GetMaximumSubtreeCount(ISymbol symbol);

        /// <summary>
        /// Gets the minimum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Minimum allowed subtree count.</returns>
        int GetMinimumSubtreeCount(ISymbol symbol);

        /// <summary>
        /// Gets the maximum allowed expression length.
        /// </summary>
        int MaximumExpressionLength { get; set; }

        /// <summary>
        /// Gets the maximum allowed expression depth.
        /// </summary>
        int MaximumExpressionDepth { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed expression length.
        /// </summary>
        int MinimumExpressionLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed expression depth.
        /// </summary>
        int MinimumExpressionDepth { get; set; }

        /// <summary>
        /// Adds a symbol to the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        void AddSymbol(ISymbol symbol);

        /// <summary>
        /// Removes a symbol from the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to remove.</param>
        void RemoveSymbol(ISymbol symbol);

        /// <summary>
        /// Checks if the grammar contains the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>True if the symbol exists in the grammar, false otherwise.</returns>
        bool ContainsSymbol(ISymbol symbol);

        /// <summary>
        /// Gets a symbol by its name.
        /// </summary>
        /// <param name="symbolName">The name of the symbol.</param>
        /// <returns>The symbol with the specified name, or null if not found.</returns>
        ISymbol? GetSymbol(string symbolName);

        /// <summary>
        /// Event that is raised when symbols are added or removed from the grammar.
        /// </summary>
        event EventHandler? Changed;
    }
}
