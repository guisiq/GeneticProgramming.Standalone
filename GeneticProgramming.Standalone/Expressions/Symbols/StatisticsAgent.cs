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
                Mean,
                Variance,
                Median
            };
    }
}
