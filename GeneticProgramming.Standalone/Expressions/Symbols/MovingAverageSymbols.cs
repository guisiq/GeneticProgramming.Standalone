using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of moving average and weighted combination symbols.
    /// Allows GP to learn adaptive filtering and smoothing.
    /// </summary>
    public static class MovingAverageSymbols
    {
        /// <summary>
        /// Exponential Moving Average approximation: alpha*current + (1-alpha)*previous
        /// GP can use this recursively to build EMA-like behaviors
        /// </summary>
        public static readonly FunctionalSymbol<double> EMA =
            SymbolFactory<double>.CreateVariadic(
                "EMA", "Exponential Moving Average: alpha*current + (1-alpha)*previous",
                args => args[0] * args[1] + (1.0 - args[0]) * args[2],
                3, 3); // alpha, current, previous

        /// <summary>
        /// Weighted average of 2 values: w*a + (1-w)*b
        /// </summary>
        public static readonly FunctionalSymbol<double> WeightedAvg2 =
            SymbolFactory<double>.CreateVariadic(
                "WeightedAvg2", "Weighted average: w*a + (1-w)*b",
                args => args[0] * args[1] + (1.0 - args[0]) * args[2],
                3, 3); // weight, a, b

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            EMA,
            WeightedAvg2
        };
    }
}
