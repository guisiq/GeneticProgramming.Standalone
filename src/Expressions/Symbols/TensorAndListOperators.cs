using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using GeneticProgramming.Expressions;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions.Symbols
{

    #region FunctionalSymbol Definitions

    // Símbolos para operações em arrays de double
    public static class ArraySymbols
    {
        public static readonly FunctionalSymbol<double> Sum =
            SymbolFactory<double>.CreateVariadic(
                "ArraySum", "Soma de elementos de um array",
                args => args.Length > 0
                    ? args.Sum()
                    : throw new ArgumentException("Array não pode ser vazio."),
                1, int.MaxValue);

        public static readonly FunctionalSymbol<double> Multiply =
            SymbolFactory<double>.CreateVariadic(
                "ArrayMultiply", "Multiplica elementos de um array",
                args => args.Length > 0
                    ? args.Aggregate(1.0, (acc, v) => acc * v)
                    : throw new ArgumentException("Array não pode ser vazio."),
                1, int.MaxValue);
    }

    // Símbolos para operações em listas de double
    public static class ListSymbols
    {
        public static readonly FunctionalSymbol<double> Sum =
            SymbolFactory<double>.CreateVariadic(
                "ListSum", "Soma de elementos de uma lista",
                args => args.Length > 0
                    ? args.Sum()
                    : throw new ArgumentException("Lista não pode ser vazia."),
                1, int.MaxValue);

        public static readonly FunctionalSymbol<double> Average =
            SymbolFactory<double>.CreateVariadic(
                "ListAverage", "Média de elementos de uma lista",
                args => args.Length > 0
                    ? args.Average()
                    : throw new ArgumentException("Lista não pode ser vazia."),
                1, int.MaxValue);
    }

    // Símbolos para operações em matrizes
    public static class MatrixSymbols
    {
        public static readonly FunctionalSymbol<Matrix<double>> Add =
            SymbolFactory<Matrix<double>>.CreateBinary(
                "MatrixAdd", "Soma de duas matrizes",
                (a, b) => (a.RowCount == b.RowCount && a.ColumnCount == b.ColumnCount)
                    ? a + b
                    : throw new ArgumentException("Matrizes de tamanhos diferentes."));

        public static readonly FunctionalSymbol<Matrix<double>> Dot =
            SymbolFactory<Matrix<double>>.CreateBinary(
                "MatrixDot", "Produto escalar de matrizes",
                (a, b) => (a.ColumnCount == b.RowCount)
                    ? a * b
                    : throw new ArgumentException("Dimensões incompatíveis."));

        public static readonly FunctionalSymbol<Matrix<double>> Subtract =
            SymbolFactory<Matrix<double>>.CreateBinary(
                "MatrixSubtract", "Subtração de matrizes",
                (a, b) => (a.RowCount == b.RowCount && a.ColumnCount == b.ColumnCount)
                    ? a - b
                    : throw new ArgumentException("Matrizes de tamanhos diferentes."));

        public static readonly FunctionalSymbol<Matrix<double>> Transpose =
            SymbolFactory<Matrix<double>>.CreateUnary(
                "MatrixTranspose", "Transposta de matriz",
                m => m.Transpose());

        public static readonly FunctionalSymbol<Matrix<double>> Inverse =
            SymbolFactory<Matrix<double>>.CreateUnary(
                "MatrixInverse", "Inversa de matriz",
                m => (Math.Abs(m.Determinant()) > 1e-15)
                    ? m.Inverse()
                    : throw new InvalidOperationException("Matriz singular sem inversa."));
    }
    #endregion

    
}
