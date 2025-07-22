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
    }
}
