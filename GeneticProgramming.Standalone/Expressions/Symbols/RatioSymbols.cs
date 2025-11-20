using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of ratio and normalization functional symbols.
    /// Core symbols for technical indicators (momentum, RSI, etc.).
    /// </summary>
    public static class RatioSymbols
    {
        /// <summary>
        /// Percentage change: (new - old) / old
        /// Protected against division by zero
        /// </summary>
        public static readonly FunctionalSymbol<double> PercentChange =
            SymbolFactory<double>.CreateBinary(
                "PercentChange", "Percentage change: (new - old) / old",
                (newVal, oldVal) => Math.Abs(oldVal) < double.Epsilon ? 0.0 : (newVal - oldVal) / oldVal);

        /// <summary>
        /// Min-Max normalization to [0,1]
        /// </summary>
        public static readonly FunctionalSymbol<double> Normalize =
            SymbolFactory<double>.CreateVariadic(
                "Normalize", "Min-Max normalization: (x - min) / (max - min)",
                args =>
                {
                    var range = args[2] - args[1];
                    return Math.Abs(range) < double.Epsilon ? 0.5 : (args[0] - args[1]) / range;
                },
                3, 3); // value, min, max

        /// <summary>
        /// Z-Score: (x - mean) / std
        /// Protected against division by zero
        /// </summary>
        public static readonly FunctionalSymbol<double> ZScore =
            SymbolFactory<double>.CreateVariadic(
                "ZScore", "Z-score normalization: (x - mean) / std",
                args => Math.Abs(args[2]) < double.Epsilon ? 0.0 : (args[0] - args[1]) / args[2],
                3, 3); // value, mean, std

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            PercentChange,
            Normalize,
            ZScore
        };
    }
}
