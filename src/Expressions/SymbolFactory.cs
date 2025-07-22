using System;
using System.Collections.Generic;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Factory for creating reusable functional symbols.
    /// </summary>
    public static class SymbolFactory
    {
        private static readonly Dictionary<string, ISymbol> _cache = new();

        /// <summary>
        /// Creates or retrieves a unary functional symbol.
        /// </summary>
        public static FunctionalSymbol CreateUnary(string name, string description, Func<double, double> op)
        {
            if (_cache.TryGetValue(name, out var existing))
                return (FunctionalSymbol)existing;

            double Wrapper(double[] args)
            {
                if (args.Length != 1)
                    throw new ArgumentException("Unary operator requires 1 argument");
                return op(args[0]);
            }

            var sym = new FunctionalSymbol(name, description, Wrapper, 1, 1);
            _cache[name] = sym;
            return sym;
        }

        /// <summary>
        /// Creates or retrieves a binary functional symbol.
        /// </summary>
        public static FunctionalSymbol CreateBinary(string name, string description, Func<double, double, double> op)
        {
            if (_cache.TryGetValue(name, out var existing))
                return (FunctionalSymbol)existing;

            double Wrapper(double[] args)
            {
                if (args.Length != 2)
                    throw new ArgumentException("Binary operator requires 2 arguments");
                return op(args[0], args[1]);
            }

            var sym = new FunctionalSymbol(name, description, Wrapper, 2, 2);
            _cache[name] = sym;
            return sym;
        }
    }
}
