using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of temporal operation symbols.
    /// Captures patterns over time (momentum, rate of change, etc.).
    /// </summary>
    public static class TemporalSymbols
    {
        /// <summary>
        /// Rate of change: (current - previous) / previous
        /// Protected against division by zero
        /// </summary>
        public static readonly FunctionalSymbol<double> RateOfChange =
            SymbolFactory<double>.CreateBinary(
                "RateOfChange", "Rate of change: (current - previous) / previous",
                (current, previous) => Math.Abs(previous) < double.Epsilon ? 0.0 : (current - previous) / previous);

        /// <summary>
        /// Momentum: current - previous (simple difference)
        /// </summary>
        public static readonly FunctionalSymbol<double> Momentum =
            SymbolFactory<double>.CreateBinary(
                "Momentum", "Momentum (difference)",
                (current, previous) => current - previous);

        /// <summary>
        /// Exponential decay: x * decay_factor
        /// Useful for time-weighted averaging
        /// </summary>
        public static readonly FunctionalSymbol<double> Decay =
            SymbolFactory<double>.CreateBinary(
                "Decay", "Exponential decay: x * factor",
                (value, factor) => value * factor);

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            RateOfChange,
            Momentum,
            Decay
        };
    }
}
