using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    /// Generic interface for symbolic expression tree grammars that define the allowed symbols and their relationships.
    /// </summary>
    /// <typeparam name="T">The value type that the grammar supports (must be a struct)</typeparam>
    public interface ISymbolicExpressionTreeGrammar<T> : ISymbolicExpressionTreeGrammar where T : struct
    {
        /// <summary>
        /// Gets all symbols available in this grammar.
        /// </summary>
        new IEnumerable<ISymbol<T>> Symbols { get; }
        IEnumerable<ISymbol> ISymbolicExpressionTreeGrammar.Symbols => Symbols.Cast<ISymbol>();

        /// <summary>
        /// Gets all symbols that can be used as root symbols.
        /// </summary>
        new IEnumerable<ISymbol<T>> StartSymbols { get; }
        IEnumerable<ISymbol> ISymbolicExpressionTreeGrammar.StartSymbols => StartSymbols.Cast<ISymbol>();

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <returns>Collection of allowed child symbols.</returns>
        IEnumerable<ISymbol<T>> GetAllowedChildSymbols(ISymbol<T> parent);
        IEnumerable<ISymbol> ISymbolicExpressionTreeGrammar.GetAllowedChildSymbols(ISymbol parent) => GetAllowedChildSymbols((ISymbol<T>)parent).Cast<ISymbol>();

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>Collection of allowed child symbols for the specified position.</returns>
        IEnumerable<ISymbol<T>> GetAllowedChildSymbols(ISymbol<T> parent, int childIndex);
        IEnumerable<ISymbol> ISymbolicExpressionTreeGrammar.GetAllowedChildSymbols(ISymbol parent, int childIndex) => GetAllowedChildSymbols((ISymbol<T>)parent, childIndex).Cast<ISymbol>();

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <returns>True if the child is allowed, false otherwise.</returns>
        bool IsAllowedChildSymbol(ISymbol<T> parent, ISymbol<T> child);
        bool ISymbolicExpressionTreeGrammar.IsAllowedChildSymbol(ISymbol parent, ISymbol child) => IsAllowedChildSymbol((ISymbol<T>)parent, (ISymbol<T>)child);

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>True if the child is allowed at the specified position, false otherwise.</returns>
        bool IsAllowedChildSymbol(ISymbol<T> parent, ISymbol<T> child, int childIndex);
        bool ISymbolicExpressionTreeGrammar.IsAllowedChildSymbol(ISymbol parent, ISymbol child, int childIndex) => IsAllowedChildSymbol((ISymbol<T>)parent, (ISymbol<T>)child, childIndex);

        /// <summary>
        /// Gets the maximum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Maximum allowed subtree count.</returns>
        int GetMaximumSubtreeCount(ISymbol<T> symbol);
        int ISymbolicExpressionTreeGrammar.GetMaximumSubtreeCount(ISymbol symbol) => GetMaximumSubtreeCount((ISymbol<T>)symbol);

        /// <summary>
        /// Gets the minimum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Minimum allowed subtree count.</returns>
        int GetMinimumSubtreeCount(ISymbol<T> symbol);
        int ISymbolicExpressionTreeGrammar.GetMinimumSubtreeCount(ISymbol symbol) => GetMinimumSubtreeCount((ISymbol<T>)symbol);

        /// <summary>
        /// Adds a symbol to the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        void AddSymbol(ISymbol<T> symbol);
        void ISymbolicExpressionTreeGrammar.AddSymbol(ISymbol symbol) => AddSymbol((ISymbol<T>)symbol);

        /// <summary>
        /// Removes a symbol from the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to remove.</param>
        void RemoveSymbol(ISymbol<T> symbol);
        void ISymbolicExpressionTreeGrammar.RemoveSymbol(ISymbol symbol) => RemoveSymbol((ISymbol<T>)symbol);

        /// <summary>
        /// Checks if the grammar contains the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>True if the symbol exists in the grammar, false otherwise.</returns>
        bool ContainsSymbol(ISymbol<T> symbol);
        bool ISymbolicExpressionTreeGrammar.ContainsSymbol(ISymbol symbol) => ContainsSymbol((ISymbol<T>)symbol);

        /// <summary>
        /// Gets a symbol by its name.
        /// </summary>
        /// <param name="symbolName">The name of the symbol.</param>
        /// <returns>The symbol with the specified name, or null if not found.</returns>
        new ISymbol<T>? GetSymbol(string symbolName);
        ISymbol? ISymbolicExpressionTreeGrammar.GetSymbol(string symbolName) => GetSymbol(symbolName);

        /// <summary>
        /// Gets symbols that are compatible with the specified type
        /// </summary>
        /// <param name="outputType">The required output type</param>
        /// <returns>Collection of symbols that produce the specified output type</returns>
        IEnumerable<ISymbol<T>> GetSymbolsByOutputType(Type outputType);
        

        /// <summary>
        /// Gets functional symbols that accept specific input types
        /// </summary>
        /// <param name="inputTypes">The required input types</param>
        /// <returns>Collection of functional symbols that accept the specified input types</returns>
        IEnumerable<ISymbol<T>> GetFunctionalSymbols(params Type[] inputTypes);
    }
}
