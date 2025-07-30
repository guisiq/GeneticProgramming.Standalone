// using System;
// using System.Collections.Generic;
// using Xunit;
// using GeneticProgramming.Expressions;
// using GeneticProgramming.Expressions.Symbols;

// namespace GeneticProgramming.Standalone.Tests
// {
//     /// <summary>
//     /// Tests for CompositeSymbol functionality and discrete mathematical operations.
//     /// </summary>
//     public class CompositeSymbolTests
//     {
//         [Fact]
//         public void Factorial_ShouldCalculateCorrectly()
//         {
//             // Arrange
//             var factorial = MathematicalDiscreteSymbols.Factorial;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Equal(1, factorial.Evaluate(new[] { 0 }, variables));
//             Assert.Equal(1, factorial.Evaluate(new[] { 1 }, variables));
//             Assert.Equal(2, factorial.Evaluate(new[] { 2 }, variables));
//             Assert.Equal(6, factorial.Evaluate(new[] { 3 }, variables));
//             Assert.Equal(24, factorial.Evaluate(new[] { 4 }, variables));
//             Assert.Equal(120, factorial.Evaluate(new[] { 5 }, variables));
//         }

//         [Fact]
//         public void Factorial_WithNegativeNumber_ShouldThrowException()
//         {
//             // Arrange
//             var factorial = MathematicalDiscreteSymbols.Factorial;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Throws<ArgumentException>(() => factorial.Evaluate(new[] { -1 }, variables));
//         }

//         [Fact]
//         public void Permutation_ShouldCalculateCorrectly()
//         {
//             // Arrange
//             var permutation = MathematicalDiscreteSymbols.Permutation;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Equal(1, permutation.Evaluate(new[] { 0, 0 }, variables)); // P(0,0) = 1
//             Assert.Equal(3, permutation.Evaluate(new[] { 3, 1 }, variables)); // P(3,1) = 3
//             Assert.Equal(6, permutation.Evaluate(new[] { 3, 2 }, variables)); // P(3,2) = 6
//             Assert.Equal(6, permutation.Evaluate(new[] { 3, 3 }, variables)); // P(3,3) = 6
//             Assert.Equal(20, permutation.Evaluate(new[] { 5, 2 }, variables)); // P(5,2) = 20
//             Assert.Equal(60, permutation.Evaluate(new[] { 5, 3 }, variables)); // P(5,3) = 60
//         }

//         [Fact]
//         public void Permutation_WithInvalidValues_ShouldThrowException()
//         {
//             // Arrange
//             var permutation = MathematicalDiscreteSymbols.Permutation;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Throws<ArgumentException>(() => permutation.Evaluate(new[] { -1, 0 }, variables));
//             Assert.Throws<ArgumentException>(() => permutation.Evaluate(new[] { 3, -1 }, variables));
//             Assert.Throws<ArgumentException>(() => permutation.Evaluate(new[] { 2, 3 }, variables)); // r > n
//         }

//         [Fact]
//         public void Combination_ShouldCalculateCorrectly()
//         {
//             // Arrange
//             var combination = MathematicalDiscreteSymbols.Combination;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Equal(1, combination.Evaluate(new[] { 0, 0 }, variables)); // C(0,0) = 1
//             Assert.Equal(1, combination.Evaluate(new[] { 3, 0 }, variables)); // C(3,0) = 1
//             Assert.Equal(3, combination.Evaluate(new[] { 3, 1 }, variables)); // C(3,1) = 3
//             Assert.Equal(3, combination.Evaluate(new[] { 3, 2 }, variables)); // C(3,2) = 3
//             Assert.Equal(1, combination.Evaluate(new[] { 3, 3 }, variables)); // C(3,3) = 1
//             Assert.Equal(10, combination.Evaluate(new[] { 5, 2 }, variables)); // C(5,2) = 10
//             Assert.Equal(10, combination.Evaluate(new[] { 5, 3 }, variables)); // C(5,3) = 10
//         }

//         [Fact]
//         public void Combination_WithInvalidValues_ShouldThrowException()
//         {
//             // Arrange
//             var combination = MathematicalDiscreteSymbols.Combination;
//             var variables = new Dictionary<string, int>();

//             // Act & Assert
//             Assert.Throws<ArgumentException>(() => combination.Evaluate(new[] { -1, 0 }, variables));
//             Assert.Throws<ArgumentException>(() => combination.Evaluate(new[] { 3, -1 }, variables));
//             Assert.Throws<ArgumentException>(() => combination.Evaluate(new[] { 2, 3 }, variables)); // r > n
//         }

//         [Fact]
//         public void CompositeSymbol_CreateTreeNode_ShouldGenerateSubtree()
//         {
//             // Arrange
//             var combination = MathematicalDiscreteSymbols.Combination;

//             // Act
//             var treeNode = combination.CreateTreeNode();

//             // Assert
//             Assert.NotNull(treeNode);
//             Assert.Equal("IntDivision", treeNode.Symbol.Name); // Root should be division
//             Assert.Equal(2, treeNode.SubtreeCount); // Division has 2 children

//             // The subtree should represent P(n,r) / r!
//             var leftChild = treeNode.GetSubtree(0); // P(n,r)
//             var rightChild = treeNode.GetSubtree(1); // r!

//             Assert.Equal("Permutation", leftChild.Symbol.Name);
//             Assert.Equal("Factorial", rightChild.Symbol.Name);
//         }

//         [Fact]
//         public void CompositeSymbol_Properties_ShouldBeCorrect()
//         {
//             // Arrange
//             var combination = MathematicalDiscreteSymbols.Combination;

//             // Act & Assert
//             Assert.Equal("Combination", combination.Name);
//             Assert.Equal("Combination operation (nCr) as P(n,r) / r!", combination.Description);
//             Assert.Equal(2, combination.MinimumArity);
//             Assert.Equal(2, combination.MaximumArity);
//         }

//         [Fact]
//         public void AllDiscreteSymbols_ShouldBeInCollection()
//         {
//             // Arrange
//             var allSymbols = MathematicalDiscreteSymbols.AllSymbols;

//             // Act & Assert
//             Assert.Contains(MathematicalDiscreteSymbols.Factorial, allSymbols);
//             Assert.Contains(MathematicalDiscreteSymbols.Permutation, allSymbols);
//             Assert.Contains(MathematicalDiscreteSymbols.Combination, allSymbols);
//             Assert.Equal(3, allSymbols.Count);
//         }

//         [Fact]
//         public void CompositeSymbol_Clone_ShouldWork()
//         {
//             // Arrange
//             var original = MathematicalDiscreteSymbols.Combination;
//             var cloner = new GeneticProgramming.Core.Cloner();

//             // Act
//             var cloned = cloner.Clone(original);

//             // Assert
//             Assert.NotNull(cloned);
//             Assert.Equal(original.Name, cloned.Name);
//             Assert.Equal(original.Description, cloned.Description);
//             Assert.Equal(original.MinimumArity, cloned.MinimumArity);
//             Assert.Equal(original.MaximumArity, cloned.MaximumArity);
//         }
//     }
// }
