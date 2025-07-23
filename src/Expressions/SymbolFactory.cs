using System;
using System.Collections.Generic;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Factory for creating reusable functional symbols.
    /// </summary>
    /// <summary>
    /// Factory for creating reusable functional symbols of type T.
    /// </summary>
    public static class SymbolFactory<T>
    {
        private static readonly Dictionary<string, FunctionalSymbol<T>> _cache = new();

        /// <summary>
        /// Creates or retrieves a unary functional symbol.
        /// </summary>
        public static FunctionalSymbol<T> CreateUnary(string name, string description, Func<T, T> op)
        {
            if (_cache.TryGetValue(name, out var existing))
                return existing;

            T Wrapper(T[] args)
            {
                if (args.Length != 1)
                    throw new ArgumentException("Unary operator requires 1 argument");
                return op(args[0]);
            }

            var sym = new FunctionalSymbol<T>(name, description, Wrapper, 1, 1);
            _cache[name] = sym;
            return sym;
        }

        /// <summary>
        /// Creates or retrieves a binary functional symbol.
        /// </summary>
        public static FunctionalSymbol<T> CreateBinary(string name, string description, Func<T, T, T> op)
        {
            if (_cache.TryGetValue(name, out var existing))
                return existing;

            T Wrapper(T[] args)
            {
                if (args.Length != 2)
                    throw new ArgumentException("Binary operator requires 2 arguments");
                return op(args[0], args[1]);
            }

            var sym = new FunctionalSymbol<T>(name, description, Wrapper, 2, 2);
            _cache[name] = sym;
            return sym;
        }
        /// <summary>
        /// Creates or retrieves a variadic functional symbol.
        /// </summary>
        public static FunctionalSymbol<T> CreateVariadic(string name, string description, Func<T[], T> op, int minArgs, int maxArgs)
        {
            if (_cache.TryGetValue(name, out var existing))
                return existing;
            var sym = new FunctionalSymbol<T>(name, description, op, minArgs, maxArgs);
            _cache[name] = sym;
            return sym;
        }

        /// <summary>
        /// Creates a composite symbol that generates predefined subtree structures.
        /// Note: Composite symbols are not cached due to their complex subtree builders.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="arity">Number of input parameters.</param>
        /// <param name="subtreeBuilder">Delegate that builds the subtree structure.</param>
        /// <param name="operation">Operation to execute when evaluating.</param>
        /// <returns>A composite symbol that generates subtrees.</returns>
        public static CompositeSymbol<T> CreateComposite(string name, string description, int arity,
            Func<ISymbolicExpressionTreeNode[], ISymbolicExpressionTreeNode> subtreeBuilder)
        {
            return new CompositeSymbol<T>(name, description, subtreeBuilder, arity);
        }
        /// <summary>
        /// Creates a composite symbol that generates predefined subtree structures with specified min and max arity.   
        /// Note: Composite symbols are not cached due to their complex subtree builders.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="minarity">Minimum number of input parameters.</param>
        /// <param name="maxarity">Maximum number of input parameters (optional, defaults to minarity).</param>
        /// <param name="subtreeBuilder">Delegate that builds the subtree structure.</param>
        /// <returns>A composite symbol that generates subtrees.</returns>
        public static CompositeSymbol<T> CreateComposite(string name, string description, int minarity, int? maxarity,
            Func<ISymbolicExpressionTreeNode[], ISymbolicExpressionTreeNode> subtreeBuilder)
        {
            return new CompositeSymbol<T>(name, description, subtreeBuilder, minarity, maxarity);
        }
    }
}
