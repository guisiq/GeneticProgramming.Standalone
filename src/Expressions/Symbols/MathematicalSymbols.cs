using System;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of mathematical functional symbols.
    /// </summary>
    public static class MathematicalSymbols
    {
        public static readonly FunctionalSymbol<double> Addition =
            SymbolFactory<double>.CreateBinary(
                "Addition", "Addition operation (+)",
                (a, b) => a + b);

        public static readonly FunctionalSymbol<double> Subtraction =
            SymbolFactory<double>.CreateBinary(
                "Subtraction", "Subtraction operation (-)",
                (a, b) => a - b);

        public static readonly FunctionalSymbol<double> Multiplication =
            SymbolFactory<double>.CreateBinary(
                "Multiplication", "Multiplication operation (*)",
                (a, b) => a * b);

        public static readonly FunctionalSymbol<double> Division =
            SymbolFactory<double>.CreateBinary(
                "Division", "Division operation (/)",
                (a, b) =>
                {
                    if (b == 0)
                        throw new DivideByZeroException("Divisão por zero.");
                    return a / b;
                });
    }
}
