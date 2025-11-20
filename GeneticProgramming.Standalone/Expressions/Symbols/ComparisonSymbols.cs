using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of comparison and logical functional symbols.
    /// Essential for building conditional trading rules.
    /// </summary>
    public static class ComparisonSymbols
    {
        /// <summary>
        /// Returns 1.0 if a > b, else 0.0
        /// </summary>
        public static readonly FunctionalSymbol<double> GreaterThan =
            SymbolFactory<double>.CreateBinary(
                "GreaterThan", "Returns 1 if a > b, else 0",
                (a, b) => a > b ? 1.0 : 0.0);

        /// <summary>
        /// Returns 1.0 if a < b, else 0.0
        /// </summary>
        public static readonly FunctionalSymbol<double> LessThan =
            SymbolFactory<double>.CreateBinary(
                "LessThan", "Returns 1 if a < b, else 0",
                (a, b) => a < b ? 1.0 : 0.0);

        /// <summary>
        /// Sign of number: -1, 0, or +1
        /// </summary>
        public static readonly FunctionalSymbol<double> Sign =
            SymbolFactory<double>.CreateUnary(
                "Sign", "Sign of number (-1, 0, +1)",
                x => Math.Sign(x));

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            GreaterThan,
            LessThan,
            Sign
        };
    }
}
