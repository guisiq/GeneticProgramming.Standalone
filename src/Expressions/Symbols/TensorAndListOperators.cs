using System;
using System.Collections.Generic;
using System.Linq;
// Importa a biblioteca MathNet.Numerics para operações com tensores
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Operações básicas com arrays
    /// </summary>
    public static class ArrayOperations
    {
        /// <summary>
        /// Soma todos os elementos de um array de doubles.
        /// </summary>
        public static double Sum(double[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array não pode ser nulo ou vazio.");
            return array.Sum();
        }

        /// <summary>
        /// Multiplica todos os elementos de um array de doubles.
        /// </summary>
        public static double Multiply(double[] array)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Array não pode ser nulo ou vazio.");
            double result = 1;
            foreach (var item in array)
                result *= item;
            return result;
        }
    }

    /// <summary>
    /// Operadores para listas
    /// </summary>
    public static class ListOperators
    {
        /// <summary>
        /// Retorna a soma dos elementos de uma lista de doubles.
        /// </summary>
        public static double Sum(List<double> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Lista não pode ser nula ou vazia.");
            return list.Sum();
        }

        /// <summary>
        /// Retorna a média dos elementos de uma lista de doubles.
        /// </summary>
        public static double Average(List<double> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("Lista não pode ser nula ou vazia.");
            return list.Average();
        }
    }

    /// <summary>
    /// Operadores com tensores utilizando MathNet.Numerics
    /// </summary>
    public static class TensorOperators
    {
        /// <summary>
        /// Realiza a soma entre dois tensores (matrizes) de mesmo tamanho.
        /// </summary>
        public static Matrix<double> Add(Matrix<double> a, Matrix<double> b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrizes não podem ser nulas.");
            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("As matrizes devem ter o mesmo tamanho.");
            return a + b;
        }

        /// <summary>
        /// Realiza a multiplicação (produto escalar) entre duas matrizes.
        /// </summary>
        public static Matrix<double> Dot(Matrix<double> a, Matrix<double> b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrizes não podem ser nulas.");
            if (a.ColumnCount != b.RowCount)
                throw new ArgumentException("O número de colunas da primeira matriz deve ser igual ao número de linhas da segunda.");
            return a * b;
        }

        /// <summary>
        /// Realiza a subtração entre duas matrizes de mesmo tamanho.
        /// </summary>
        public static Matrix<double> Subtract(Matrix<double> a, Matrix<double> b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrizes não podem ser nulas.");
            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("As matrizes devem ter o mesmo tamanho.");
            return a - b;
        }

        /// <summary>
        /// Calcula o determinante de uma matriz quadrada.
        /// </summary>
        public static double Determinant(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException("A matriz deve ser quadrada para calcular o determinante.");
            return matrix.Determinant();
        }

        /// <summary>
        /// Calcula a transposta de uma matriz.
        /// </summary>
        public static Matrix<double> Transpose(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            return matrix.Transpose();
        }

        /// <summary>
        /// Calcula a inversa de uma matriz quadrada não-singular.
        /// </summary>
        public static Matrix<double> Inverse(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException("A matriz deve ser quadrada para calcular a inversa.");
            
            // Verifica se a matriz é singular (determinante próximo de zero)
            var determinant = matrix.Determinant();
            if (Math.Abs(determinant) < 1e-15)
                throw new InvalidOperationException("A matriz é singular e não possui inversa.");
            
            try
            {
                return matrix.Inverse();
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("A matriz é singular e não possui inversa.", ex);
            }
        }

        /// <summary>
        /// Calcula o traço (trace) de uma matriz quadrada - soma dos elementos da diagonal principal.
        /// </summary>
        public static double Trace(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentException("A matriz deve ser quadrada para calcular o traço.");
            return matrix.Trace();
        }

        /// <summary>
        /// Calcula a norma Frobenius de uma matriz.
        /// </summary>
        public static double FrobeniusNorm(Matrix<double> matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            return matrix.FrobeniusNorm();
        }

        /// <summary>
        /// Multiplica uma matriz por um escalar.
        /// </summary>
        public static Matrix<double> ScalarMultiply(Matrix<double> matrix, double scalar)
        {
            if (matrix == null)
                throw new ArgumentNullException("Matriz não pode ser nula.");
            return matrix * scalar;
        }

        /// <summary>
        /// Cria uma matriz identidade de tamanho especificado.
        /// </summary>
        public static Matrix<double> Identity(int size)
        {
            if (size <= 0)
                throw new ArgumentException("O tamanho deve ser positivo.");
            return DenseMatrix.CreateIdentity(size);
        }

        /// <summary>
        /// Cria uma matriz de zeros com dimensões especificadas.
        /// </summary>
        public static Matrix<double> Zeros(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0)
                throw new ArgumentException("As dimensões devem ser positivas.");
            return DenseMatrix.Create(rows, columns, 0.0);
        }

        /// <summary>
        /// Cria uma matriz de uns com dimensões especificadas.
        /// </summary>
        public static Matrix<double> Ones(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0)
                throw new ArgumentException("As dimensões devem ser positivas.");
            return DenseMatrix.Create(rows, columns, 1.0);
        }

        /// <summary>
        /// Verifica se duas matrizes são aproximadamente iguais dentro de uma tolerância.
        /// </summary>
        public static bool AreEqual(Matrix<double> a, Matrix<double> b, double tolerance = 1e-10)
        {
            if (a == null || b == null)
                return false;
            if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                return false;
            
            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColumnCount; j++)
                {
                    if (Math.Abs(a[i, j] - b[i, j]) > tolerance)
                        return false;
                }
            }
            return true;
        }
    }
}
