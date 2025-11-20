using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of statistical functional symbols.
    /// </summary>
    public static class StatisticsSymbols
    {
        // OPTIMIZATION: Replaced variadic Mean with fixed-arity versions to avoid array allocations
        public static readonly FunctionalSymbol<double> Mean2 =
            SymbolFactory<double>.CreateBinary(
                "Mean2", "Mean of 2 values",
                (a, b) => (a + b) * 0.5);

        public static readonly FunctionalSymbol<double> Mean3 =
            SymbolFactory<double>.CreateVariadic(
                "Mean3", "Mean of 3 values",
                args => (args[0] + args[1] + args[2]) * 0.3333333333333333,
                3, 3);

        public static readonly FunctionalSymbol<double> Mean4 =
            SymbolFactory<double>.CreateVariadic(
                "Mean4", "Mean of 4 values",
                args => (args[0] + args[1] + args[2] + args[3]) * 0.25,
                4, 4);

        public static readonly FunctionalSymbol<double> Mean5 =
            SymbolFactory<double>.CreateVariadic(
                "Mean5", "Mean of 5 values",
                args => (args[0] + args[1] + args[2] + args[3] + args[4]) * 0.2,
                5, 5);

        public static readonly FunctionalSymbol<double> Mean6 =
            SymbolFactory<double>.CreateVariadic(
                "Mean6", "Mean of 6 values",
                args => (args[0] + args[1] + args[2] + args[3] + args[4] + args[5]) / 6.0,
                6, 6);

        public static readonly FunctionalSymbol<double> Max2 =
            SymbolFactory<double>.CreateBinary(
                "Max2", "Max of 2 values",
                Math.Max);

        public static readonly FunctionalSymbol<double> Max3 =
            SymbolFactory<double>.CreateVariadic(
                "Max3", "Max of 3 values",
                args => Math.Max(args[0], Math.Max(args[1], args[2])),
                3, 3);

        public static readonly FunctionalSymbol<double> Max4 =
            SymbolFactory<double>.CreateVariadic(
                "Max4", "Max of 4 values",
                args => Math.Max(Math.Max(args[0], args[1]), Math.Max(args[2], args[3])),
                4, 4);

        public static readonly FunctionalSymbol<double> Max5 =
            SymbolFactory<double>.CreateVariadic(
                "Max5", "Max of 5 values",
                args => Math.Max(Math.Max(Math.Max(args[0], args[1]), Math.Max(args[2], args[3])), args[4]),
                5, 5);

        public static readonly FunctionalSymbol<double> Max6 =
            SymbolFactory<double>.CreateVariadic(
                "Max6", "Max of 6 values",
                args => Math.Max(Math.Max(Math.Max(args[0], args[1]), Math.Max(args[2], args[3])), Math.Max(args[4], args[5])),
                6, 6);

        public static readonly FunctionalSymbol<double> Min2 =
            SymbolFactory<double>.CreateBinary(
                "Min2", "Min of 2 values",
                Math.Min);

        public static readonly FunctionalSymbol<double> Min3 =
            SymbolFactory<double>.CreateVariadic(
                "Min3", "Min of 3 values",
                args => Math.Min(args[0], Math.Min(args[1], args[2])),
                3, 3);

        public static readonly FunctionalSymbol<double> Min4 =
            SymbolFactory<double>.CreateVariadic(
                "Min4", "Min of 4 values",
                args => Math.Min(Math.Min(args[0], args[1]), Math.Min(args[2], args[3])),
                4, 4);

        public static readonly FunctionalSymbol<double> Min5 =
            SymbolFactory<double>.CreateVariadic(
                "Min5", "Min of 5 values",
                args => Math.Min(Math.Min(Math.Min(args[0], args[1]), Math.Min(args[2], args[3])), args[4]),
                5, 5);

        public static readonly FunctionalSymbol<double> Min6 =
            SymbolFactory<double>.CreateVariadic(
                "Min6", "Min of 6 values",
                args => Math.Min(Math.Min(Math.Min(args[0], args[1]), Math.Min(args[2], args[3])), Math.Min(args[4], args[5])),
                6, 6);

        public static readonly FunctionalSymbol<double> IfElse =
            SymbolFactory<double>.CreateVariadic(
                "IfElse", "Soft If-Else",
                args => args[0] < 0 ? args[1] : args[2],
                3, 3);

        // Legacy variadic symbols kept but not recommended for high-performance loops
        public static readonly FunctionalSymbol<double> Mean =
            SymbolFactory<double>.CreateVariadic(
                "Mean", "Média de sequência",
                args =>
                {
                    if (args == null || args.Length == 0)
                        throw new ArgumentException("Dados não podem ser nulos ou vazios.");
                    return args.Average();
                },
                1, int.MaxValue);

        public static readonly FunctionalSymbol<double> Variance =
            SymbolFactory<double>.CreateVariadic(
                "Variance", "Variância de sequência",
                args =>
                {
                    if (args == null || args.Length == 0)
                        throw new ArgumentException("Dados não podem ser nulos ou vazios.");
                    var mean = args.Average();
                    return args.Sum(d => (d - mean) * (d - mean)) / args.Length;
                },
                1, int.MaxValue);

        public static readonly FunctionalSymbol<double> Median =
            SymbolFactory<double>.CreateVariadic(
                "Median", "Mediana de sequência",
                args =>
                {
                    if (args == null || args.Length == 0)
                        throw new ArgumentException("Dados não podem ser nulos ou vazios.");
                    var sorted = args.OrderBy(x => x).ToArray();
                    int n = sorted.Length;
                    return n % 2 == 0
                        ? (sorted[n/2 - 1] + sorted[n/2]) / 2.0
                        : sorted[n/2];

                },
                1, int.MaxValue);
            
            public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
            {
                Mean2,
                Mean3,
                Mean4,
                Mean5,
                Mean6,
                Max2,
                Max3,
                Max4,
                Max5,
                Max6,
                Min2,
                Min3,
                Min4,
                Min5,
                Min6,
                IfElse,
                // Variadic symbols included for compatibility but should be used with caution
                Mean,
                Variance,
                Median
            };
    }
}
