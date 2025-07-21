using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions.Symbols;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Symbols
{
    /// <summary>
    /// Testes unitários para os novos operadores estatísticos e de tensores.
    /// </summary>
    public class StatisticsAndTensorOperatorsTests
    {
        #region StatisticsAgent Tests

        [Fact]
        public void StatisticsAgent_Mean_ValidData_ReturnsCorrectMean()
        {
            // Arrange
            var data = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var expected = 3.0;

            // Act
            var result = StatisticsAgent.Mean(data);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void StatisticsAgent_Mean_EmptyData_ThrowsArgumentException()
        {
            // Arrange
            var data = new double[] { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Mean(data));
        }

        [Fact]
        public void StatisticsAgent_Mean_NullData_ThrowsArgumentException()
        {
            // Arrange
            IEnumerable<double> data = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Mean(data));
        }

        [Fact]
        public void StatisticsAgent_Variance_ValidData_ReturnsCorrectVariance()
        {
            // Arrange
            var data = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var expected = 2.0; // Variance of [1,2,3,4,5] is 2.0

            // Act
            var result = StatisticsAgent.Variance(data);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void StatisticsAgent_Variance_EmptyData_ThrowsArgumentException()
        {
            // Arrange
            var data = new double[] { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Variance(data));
        }

        [Fact]
        public void StatisticsAgent_Variance_NullData_ThrowsArgumentException()
        {
            // Arrange
            IEnumerable<double> data = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Variance(data));
        }

        [Fact]
        public void StatisticsAgent_Median_OddCountData_ReturnsMiddleValue()
        {
            // Arrange
            var data = new double[] { 3.0, 1.0, 5.0, 2.0, 4.0 };
            var expected = 3.0;

            // Act
            var result = StatisticsAgent.Median(data);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void StatisticsAgent_Median_EvenCountData_ReturnsAverageOfMiddleTwoValues()
        {
            // Arrange
            var data = new double[] { 1.0, 2.0, 3.0, 4.0 };
            var expected = 2.5; // (2 + 3) / 2

            // Act
            var result = StatisticsAgent.Median(data);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void StatisticsAgent_Median_EmptyData_ThrowsArgumentException()
        {
            // Arrange
            var data = new double[] { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Median(data));
        }

        [Fact]
        public void StatisticsAgent_Median_NullData_ThrowsArgumentException()
        {
            // Arrange
            IEnumerable<double> data = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StatisticsAgent.Median(data));
        }

        #endregion

        #region ArrayOperations Tests

        [Fact]
        public void ArrayOperations_Sum_ValidArray_ReturnsCorrectSum()
        {
            // Arrange
            var array = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var expected = 15.0;

            // Act
            var result = ArrayOperations.Sum(array);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void ArrayOperations_Sum_EmptyArray_ThrowsArgumentException()
        {
            // Arrange
            var array = new double[] { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ArrayOperations.Sum(array));
        }

        [Fact]
        public void ArrayOperations_Sum_NullArray_ThrowsArgumentException()
        {
            // Arrange
            double[] array = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ArrayOperations.Sum(array));
        }

        [Fact]
        public void ArrayOperations_Multiply_ValidArray_ReturnsCorrectProduct()
        {
            // Arrange
            var array = new double[] { 1.0, 2.0, 3.0, 4.0 };
            var expected = 24.0; // 1 * 2 * 3 * 4

            // Act
            var result = ArrayOperations.Multiply(array);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void ArrayOperations_Multiply_EmptyArray_ThrowsArgumentException()
        {
            // Arrange
            var array = new double[] { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ArrayOperations.Multiply(array));
        }

        [Fact]
        public void ArrayOperations_Multiply_NullArray_ThrowsArgumentException()
        {
            // Arrange
            double[] array = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ArrayOperations.Multiply(array));
        }

        #endregion

        #region ListOperators Tests

        [Fact]
        public void ListOperators_Sum_ValidList_ReturnsCorrectSum()
        {
            // Arrange
            var list = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var expected = 15.0;

            // Act
            var result = ListOperators.Sum(list);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void ListOperators_Sum_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            var list = new List<double>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ListOperators.Sum(list));
        }

        [Fact]
        public void ListOperators_Sum_NullList_ThrowsArgumentException()
        {
            // Arrange
            List<double> list = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ListOperators.Sum(list));
        }

        [Fact]
        public void ListOperators_Average_ValidList_ReturnsCorrectAverage()
        {
            // Arrange
            var list = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var expected = 3.0;

            // Act
            var result = ListOperators.Average(list);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void ListOperators_Average_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            var list = new List<double>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ListOperators.Average(list));
        }

        [Fact]
        public void ListOperators_Average_NullList_ThrowsArgumentException()
        {
            // Arrange
            List<double> list = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ListOperators.Average(list));
        }

        #endregion

        #region TensorOperators Tests

        [Fact]
        public void TensorOperators_Add_ValidMatrices_ReturnsCorrectSum()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var matrixB = DenseMatrix.OfArray(new double[,] { { 5, 6 }, { 7, 8 } });
            var expected = DenseMatrix.OfArray(new double[,] { { 6, 8 }, { 10, 12 } });

            // Act
            var result = TensorOperators.Add(matrixA, matrixB);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Add_NullMatrices_ThrowsArgumentNullException()
        {
            // Arrange
            Matrix<double> matrixA = null;
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => TensorOperators.Add(matrixA, matrixB));
        }

        [Fact]
        public void TensorOperators_Add_DifferentSizes_ThrowsArgumentException()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2 } }); // 1x2
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1 }, { 2 } }); // 2x1

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Add(matrixA, matrixB));
        }

        [Fact]
        public void TensorOperators_Dot_ValidMatrices_ReturnsCorrectProduct()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } }); // 2x2
            var matrixB = DenseMatrix.OfArray(new double[,] { { 5, 6 }, { 7, 8 } }); // 2x2
            var expected = DenseMatrix.OfArray(new double[,] { { 19, 22 }, { 43, 50 } }); // (1*5+2*7, 1*6+2*8), (3*5+4*7, 3*6+4*8)

            // Act
            var result = TensorOperators.Dot(matrixA, matrixB);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Dot_NullMatrices_ThrowsArgumentNullException()
        {
            // Arrange
            Matrix<double> matrixA = null;
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => TensorOperators.Dot(matrixA, matrixB));
        }

        [Fact]
        public void TensorOperators_Dot_IncompatibleDimensions_ThrowsArgumentException()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 } }); // 1x3
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1 }, { 2 } }); // 2x1 (incompatível: 3 colunas x 2 linhas)

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Dot(matrixA, matrixB));
        }

        [Fact]
        public void TensorOperators_Dot_CompatibleDimensions_ReturnsCorrectResult()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 } }); // 1x3
            var matrixB = DenseMatrix.OfArray(new double[,] { { 4 }, { 5 }, { 6 } }); // 3x1
            var expected = DenseMatrix.OfArray(new double[,] { { 32 } }); // 1*4 + 2*5 + 3*6 = 32

            // Act
            var result = TensorOperators.Dot(matrixA, matrixB);

            // Assert
            Assert.True(result.Equals(expected));
        }

        #endregion

        #region Advanced TensorOperators Tests

        [Fact]
        public void TensorOperators_Subtract_ValidMatrices_ReturnsCorrectDifference()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 5, 6 }, { 7, 8 } });
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var expected = DenseMatrix.OfArray(new double[,] { { 4, 4 }, { 4, 4 } });

            // Act
            var result = TensorOperators.Subtract(matrixA, matrixB);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Determinant_ValidSquareMatrix_ReturnsCorrectDeterminant()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var expected = -2.0; // det([[1,2],[3,4]]) = 1*4 - 2*3 = -2

            // Act
            var result = TensorOperators.Determinant(matrix);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void TensorOperators_Determinant_NonSquareMatrix_ThrowsArgumentException()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Determinant(matrix));
        }

        [Fact]
        public void TensorOperators_Transpose_ValidMatrix_ReturnsCorrectTranspose()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3
            var expected = DenseMatrix.OfArray(new double[,] { { 1, 4 }, { 2, 5 }, { 3, 6 } }); // 3x2

            // Act
            var result = TensorOperators.Transpose(matrix);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Inverse_ValidInvertibleMatrix_ReturnsCorrectInverse()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var expected = DenseMatrix.OfArray(new double[,] { { -2, 1 }, { 1.5, -0.5 } });

            // Act
            var result = TensorOperators.Inverse(matrix);

            // Assert - Use custom comparison for numerical precision
            Assert.True(TensorOperators.AreEqual(result, expected, 1e-10));
        }

        [Fact]
        public void TensorOperators_Inverse_SingularMatrix_ThrowsInvalidOperationException()
        {
            // Arrange - singular matrix (determinant = 0)
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 2, 4 } });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => TensorOperators.Inverse(matrix));
        }

        [Fact]
        public void TensorOperators_Trace_ValidSquareMatrix_ReturnsCorrectTrace()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
            var expected = 15.0; // 1 + 5 + 9 = 15

            // Act
            var result = TensorOperators.Trace(matrix);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void TensorOperators_Trace_NonSquareMatrix_ThrowsArgumentException()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Trace(matrix));
        }

        [Fact]
        public void TensorOperators_FrobeniusNorm_ValidMatrix_ReturnsCorrectNorm()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 3, 4 }, { 0, 0 } });
            var expected = 5.0; // sqrt(3^2 + 4^2 + 0^2 + 0^2) = sqrt(25) = 5

            // Act
            var result = TensorOperators.FrobeniusNorm(matrix);

            // Assert
            Assert.Equal(expected, result, 6);
        }

        [Fact]
        public void TensorOperators_ScalarMultiply_ValidMatrix_ReturnsCorrectResult()
        {
            // Arrange
            var matrix = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var scalar = 2.5;
            var expected = DenseMatrix.OfArray(new double[,] { { 2.5, 5.0 }, { 7.5, 10.0 } });

            // Act
            var result = TensorOperators.ScalarMultiply(matrix, scalar);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Identity_ValidSize_ReturnsCorrectIdentityMatrix()
        {
            // Arrange
            var size = 3;
            var expected = DenseMatrix.OfArray(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });

            // Act
            var result = TensorOperators.Identity(size);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Identity_InvalidSize_ThrowsArgumentException()
        {
            // Arrange
            var size = 0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Identity(size));
        }

        [Fact]
        public void TensorOperators_Zeros_ValidDimensions_ReturnsCorrectZeroMatrix()
        {
            // Arrange
            var rows = 2;
            var columns = 3;
            var expected = DenseMatrix.OfArray(new double[,] { { 0, 0, 0 }, { 0, 0, 0 } });

            // Act
            var result = TensorOperators.Zeros(rows, columns);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_Zeros_InvalidDimensions_ThrowsArgumentException()
        {
            // Arrange
            var rows = 0;
            var columns = 3;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => TensorOperators.Zeros(rows, columns));
        }

        [Fact]
        public void TensorOperators_Ones_ValidDimensions_ReturnsCorrectOnesMatrix()
        {
            // Arrange
            var rows = 2;
            var columns = 2;
            var expected = DenseMatrix.OfArray(new double[,] { { 1, 1 }, { 1, 1 } });

            // Act
            var result = TensorOperators.Ones(rows, columns);

            // Assert
            Assert.True(result.Equals(expected));
        }

        [Fact]
        public void TensorOperators_AreEqual_IdenticalMatrices_ReturnsTrue()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });

            // Act
            var result = TensorOperators.AreEqual(matrixA, matrixB);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TensorOperators_AreEqual_DifferentMatrices_ReturnsFalse()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 4 } });
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1, 2 }, { 3, 5 } });

            // Act
            var result = TensorOperators.AreEqual(matrixA, matrixB);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TensorOperators_AreEqual_WithinTolerance_ReturnsTrue()
        {
            // Arrange
            var matrixA = DenseMatrix.OfArray(new double[,] { { 1.0000001, 2 }, { 3, 4 } });
            var matrixB = DenseMatrix.OfArray(new double[,] { { 1.0000002, 2 }, { 3, 4 } });
            var tolerance = 1e-6;

            // Act
            var result = TensorOperators.AreEqual(matrixA, matrixB, tolerance);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Edge Cases and Performance Tests

        [Fact]
        public void StatisticsAgent_SingleElement_HandlesCorrectly()
        {
            // Arrange
            var data = new double[] { 42.0 };

            // Act & Assert
            Assert.Equal(42.0, StatisticsAgent.Mean(data), 6);
            Assert.Equal(0.0, StatisticsAgent.Variance(data), 6);
            Assert.Equal(42.0, StatisticsAgent.Median(data), 6);
        }

        [Fact]
        public void ArrayOperations_SingleElement_HandlesCorrectly()
        {
            // Arrange
            var array = new double[] { 42.0 };

            // Act & Assert
            Assert.Equal(42.0, ArrayOperations.Sum(array), 6);
            Assert.Equal(42.0, ArrayOperations.Multiply(array), 6);
        }

        [Fact]
        public void ListOperators_SingleElement_HandlesCorrectly()
        {
            // Arrange
            var list = new List<double> { 42.0 };

            // Act & Assert
            Assert.Equal(42.0, ListOperators.Sum(list), 6);
            Assert.Equal(42.0, ListOperators.Average(list), 6);
        }

        [Fact]
        public void TensorOperators_IdentityMatrix_HandlesCorrectly()
        {
            // Arrange
            var identity = DenseMatrix.CreateIdentity(2);
            var matrix = DenseMatrix.OfArray(new double[,] { { 3, 4 }, { 5, 6 } });

            // Act
            var result = TensorOperators.Dot(identity, matrix);

            // Assert
            Assert.True(result.Equals(matrix));
        }

        #endregion
    }
}
