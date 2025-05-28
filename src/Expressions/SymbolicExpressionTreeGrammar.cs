using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Base implementation of a symbolic expression tree grammar.
    /// </summary>
    [Serializable]
    public class SymbolicExpressionTreeGrammar : Core.Item, ISymbolicExpressionTreeGrammar
    {
        private readonly Dictionary<ISymbol, ReadOnlyCollection<ISymbol>> _allowedChildSymbols;
        private readonly Dictionary<string, ISymbol> _symbolsByName;
        private readonly List<ISymbol> _symbols;
        private readonly List<ISymbol> _startSymbols;

        private int _maximumExpressionLength = 100;
        private int _maximumExpressionDepth = 10;
        private int _minimumExpressionLength = 1;
        private int _minimumExpressionDepth = 1;

        /// <summary>
        /// Gets all symbols available in this grammar.
        /// </summary>
        public IEnumerable<ISymbol> Symbols => _symbols.AsReadOnly();

        /// <summary>
        /// Gets all symbols that can be used as root symbols.
        /// </summary>
        public IEnumerable<ISymbol> StartSymbols => _startSymbols.AsReadOnly();

        /// <summary>
        /// Gets or sets the maximum allowed expression length.
        /// </summary>
        public int MaximumExpressionLength
        {
            get => _maximumExpressionLength;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum expression length must be at least 1.");
                if (value < _minimumExpressionLength)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum expression length must be greater than or equal to minimum expression length.");
                
                _maximumExpressionLength = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed expression depth.
        /// </summary>
        public int MaximumExpressionDepth
        {
            get => _maximumExpressionDepth;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum expression depth must be at least 1.");
                if (value < _minimumExpressionDepth)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum expression depth must be greater than or equal to minimum expression depth.");
                
                _maximumExpressionDepth = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed expression length.
        /// </summary>
        public int MinimumExpressionLength
        {
            get => _minimumExpressionLength;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minimum expression length must be at least 1.");
                if (value > _maximumExpressionLength)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minimum expression length must be less than or equal to maximum expression length.");
                
                _minimumExpressionLength = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed expression depth.
        /// </summary>
        public int MinimumExpressionDepth
        {
            get => _minimumExpressionDepth;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minimum expression depth must be at least 1.");
                if (value > _maximumExpressionDepth)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minimum expression depth must be less than or equal to maximum expression depth.");
                
                _minimumExpressionDepth = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event that is raised when symbols are added or removed from the grammar.
        /// </summary>
        public event EventHandler? Changed;

        /// <summary>
        /// Initializes a new instance of the SymbolicExpressionTreeGrammar class.
        /// </summary>
        public SymbolicExpressionTreeGrammar()
        {
            _allowedChildSymbols = new Dictionary<ISymbol, ReadOnlyCollection<ISymbol>>();
            _symbolsByName = new Dictionary<string, ISymbol>();
            _symbols = new List<ISymbol>();
            _startSymbols = new List<ISymbol>();
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        /// <param name="original">The original grammar to copy.</param>
        protected SymbolicExpressionTreeGrammar(SymbolicExpressionTreeGrammar original) : base(original)
        {
            _allowedChildSymbols = new Dictionary<ISymbol, ReadOnlyCollection<ISymbol>>();
            _symbolsByName = new Dictionary<string, ISymbol>();
            _symbols = new List<ISymbol>();
            _startSymbols = new List<ISymbol>();

            _maximumExpressionLength = original._maximumExpressionLength;
            _maximumExpressionDepth = original._maximumExpressionDepth;
            _minimumExpressionLength = original._minimumExpressionLength;
            _minimumExpressionDepth = original._minimumExpressionDepth;

            // Deep copy symbols
            foreach (var symbol in original._symbols)
            {
                var clonedSymbol = (ISymbol)symbol.Clone();
                AddSymbol(clonedSymbol);
            }

            // Rebuild start symbols list
            foreach (var startSymbol in original._startSymbols)
            {
                var clonedStartSymbol = GetSymbol(startSymbol.Name);
                if (clonedStartSymbol != null)
                    _startSymbols.Add(clonedStartSymbol);
            }

            // Rebuild allowed child symbols relationships
            foreach (var kvp in original._allowedChildSymbols)
            {
                var parentSymbol = GetSymbol(kvp.Key.Name);
                if (parentSymbol != null)
                {
                    var allowedChildren = new List<ISymbol>();
                    foreach (var childSymbol in kvp.Value)
                    {
                        var clonedChild = GetSymbol(childSymbol.Name);
                        if (clonedChild != null)
                            allowedChildren.Add(clonedChild);
                    }
                    _allowedChildSymbols[parentSymbol] = new ReadOnlyCollection<ISymbol>(allowedChildren);
                }
            }
        }

        /// <summary>
        /// Creates a deep clone of this grammar.
        /// </summary>
        /// <returns>A cloned instance of the grammar.</returns>
        public override Core.IDeepCloneable Clone()
        {
            return new SymbolicExpressionTreeGrammar(this);
        }

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <returns>Collection of allowed child symbols.</returns>
        public virtual IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent)
        {
            if (_allowedChildSymbols.TryGetValue(parent, out var allowedSymbols))
                return allowedSymbols;
            
            // Default: allow all symbols except the parent itself to prevent infinite recursion
            return _symbols.Where(s => s != parent);
        }

        /// <summary>
        /// Gets symbols that are allowed as children of the specified parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>Collection of allowed child symbols for the specified position.</returns>
        public virtual IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int childIndex)
        {
            // Default implementation: same allowed symbols for all child positions
            return GetAllowedChildSymbols(parent);
        }

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <returns>True if the child is allowed, false otherwise.</returns>
        public virtual bool IsAllowedChildSymbol(ISymbol parent, ISymbol child)
        {
            return GetAllowedChildSymbols(parent).Contains(child);
        }

        /// <summary>
        /// Checks if the specified symbol is allowed as a child of the parent symbol at a specific child index.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="child">The child symbol to check.</param>
        /// <param name="childIndex">The index of the child position.</param>
        /// <returns>True if the child is allowed at the specified position, false otherwise.</returns>
        public virtual bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int childIndex)
        {
            return GetAllowedChildSymbols(parent, childIndex).Contains(child);
        }

        /// <summary>
        /// Gets the maximum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Maximum allowed subtree count.</returns>
        public virtual int GetMaximumSubtreeCount(ISymbol symbol)
        {
            return symbol.MaximumArity;
        }

        /// <summary>
        /// Gets the minimum allowed subtree count for the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>Minimum allowed subtree count.</returns>
        public virtual int GetMinimumSubtreeCount(ISymbol symbol)
        {
            return symbol.MinimumArity;
        }

        /// <summary>
        /// Adds a symbol to the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to add.</param>
        public virtual void AddSymbol(ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (_symbolsByName.ContainsKey(symbol.Name))
                throw new ArgumentException($"A symbol with the name '{symbol.Name}' already exists in the grammar.");

            _symbols.Add(symbol);
            _symbolsByName[symbol.Name] = symbol;

            // By default, all symbols can be start symbols unless they require children
            if (symbol.MinimumArity == 0)
                _startSymbols.Add(symbol);

            OnChanged();
        }

        /// <summary>
        /// Removes a symbol from the grammar.
        /// </summary>
        /// <param name="symbol">The symbol to remove.</param>
        public virtual void RemoveSymbol(ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (_symbols.Remove(symbol))
            {
                _symbolsByName.Remove(symbol.Name);
                _startSymbols.Remove(symbol);
                _allowedChildSymbols.Remove(symbol);

                // Remove this symbol from allowed children of other symbols
                var keysToUpdate = _allowedChildSymbols.Keys.ToList();
                foreach (var parentSymbol in keysToUpdate)
                {
                    var currentAllowed = _allowedChildSymbols[parentSymbol];
                    if (currentAllowed.Contains(symbol))
                    {
                        var newAllowed = currentAllowed.Where(s => s != symbol).ToList();
                        _allowedChildSymbols[parentSymbol] = new ReadOnlyCollection<ISymbol>(newAllowed);
                    }
                }

                OnChanged();
            }
        }

        /// <summary>
        /// Checks if the grammar contains the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>True if the symbol exists in the grammar, false otherwise.</returns>
        public bool ContainsSymbol(ISymbol symbol)
        {
            return _symbols.Contains(symbol);
        }

        /// <summary>
        /// Gets a symbol by its name.
        /// </summary>
        /// <param name="symbolName">The name of the symbol.</param>
        /// <returns>The symbol with the specified name, or null if not found.</returns>
        public ISymbol? GetSymbol(string symbolName)
        {
            _symbolsByName.TryGetValue(symbolName, out var symbol);
            return symbol;
        }

        /// <summary>
        /// Sets allowed child symbols for a parent symbol.
        /// </summary>
        /// <param name="parent">The parent symbol.</param>
        /// <param name="allowedChildren">The allowed child symbols.</param>
        protected void SetAllowedChildSymbols(ISymbol parent, IEnumerable<ISymbol> allowedChildren)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (allowedChildren == null)
                throw new ArgumentNullException(nameof(allowedChildren));

            _allowedChildSymbols[parent] = new ReadOnlyCollection<ISymbol>(allowedChildren.ToList());
        }

        /// <summary>
        /// Adds a symbol as a start symbol.
        /// </summary>
        /// <param name="symbol">The symbol to add as a start symbol.</param>
        protected void AddStartSymbol(ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (ContainsSymbol(symbol) && !_startSymbols.Contains(symbol))
            {
                _startSymbols.Add(symbol);
                OnChanged();
            }
        }

        /// <summary>
        /// Removes a symbol from start symbols.
        /// </summary>
        /// <param name="symbol">The symbol to remove from start symbols.</param>
        protected void RemoveStartSymbol(ISymbol symbol)
        {
            if (_startSymbols.Remove(symbol))
                OnChanged();
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        protected virtual void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
