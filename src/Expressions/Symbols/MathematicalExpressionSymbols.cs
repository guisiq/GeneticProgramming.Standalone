using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Factory for creating mathematical symbols using System.Linq.Expressions.
    /// </summary>
    public static class ExpressionSymbolFactory<T>
    {
        private static readonly Dictionary<string, ExpressionSymbol<T>> _cache = new();

        /// <summary>
        /// Creates a unary mathematical symbol.
        /// </summary>
        public static ExpressionSymbol<T> CreateUnary(string name, string description, 
            Func<Expression, Expression> operation)
        {
            if (_cache.TryGetValue(name, out var existing))
                return existing;

            var symbol = new ExpressionSymbol<T>(name, description, 1, 
                parameters => operation(parameters[0]));
                
            _cache[name] = symbol;
            return symbol;
        }

        /// <summary>
        /// Creates a binary mathematical symbol.
        /// </summary>
        public static ExpressionSymbol<T> CreateBinary(string name, string description,
            Func<Expression, Expression, Expression> operation)
        {
            if (_cache.TryGetValue(name, out var existing))
                return existing;

            var symbol = new ExpressionSymbol<T>(name, description, 2,
                parameters => operation(parameters[0], parameters[1]));
                
            _cache[name] = symbol;
            return symbol;
        }

        /// <summary>
        /// Creates a constant symbol.
        /// </summary>
        public static ExpressionSymbol<T> CreateConstant(string name, T value)
        {
            return new ExpressionSymbol<T>(name, $"Constant value {value}", 0,
                _ => Expression.Constant(value, typeof(T)));
        }

        /// <summary>
        /// Creates a variable symbol.
        /// </summary>
        public static ExpressionSymbol<T> CreateVariable(string name)
        {
            return new ExpressionSymbol<T>(name, $"Variable {name}", 0,
                _ => Expression.Parameter(typeof(T), name));
        }

        /// <summary>
        /// Creates a composite symbol that represents a complex mathematical operation.
        /// </summary>
        public static ExpressionSymbol<T> CreateComposite(string name, string description, int arity,
            Func<ParameterExpression[], Expression> expressionBuilder)
        {
            return new ExpressionSymbol<T>(name, description, arity, expressionBuilder);
        }
    }

    /// <summary>
    /// Mathematical symbols implemented using System.Linq.Expressions.
    /// </summary>
    public static class MathematicalExpressionSymbols
    {
        public static readonly List<ISymbol> AllSymbols = new()
        {
            Addition,
            Subtraction,
            Multiplication,
            Division
        };

        public static readonly ExpressionSymbol<double> Addition =
            ExpressionSymbolFactory<double>.CreateBinary(
                "Addition", "Addition operation (+)",
                Expression.Add);

        public static readonly ExpressionSymbol<double> Subtraction =
            ExpressionSymbolFactory<double>.CreateBinary(
                "Subtraction", "Subtraction operation (-)",
                Expression.Subtract);

        public static readonly ExpressionSymbol<double> Multiplication =
            ExpressionSymbolFactory<double>.CreateBinary(
                "Multiplication", "Multiplication operation (*)",
                Expression.Multiply);

        public static readonly ExpressionSymbol<double> Division =
            ExpressionSymbolFactory<double>.CreateBinary(
                "Division", "Division operation (/)",
                (left, right) =>
                {
                    // Add division by zero check
                    var zero = Expression.Constant(0.0);
                    var condition = Expression.Equal(right, zero);
                    var exception = Expression.Throw(
                        Expression.New(typeof(DivideByZeroException).GetConstructor(Type.EmptyTypes)),
                        typeof(double));
                    
                    return Expression.Condition(condition, exception, Expression.Divide(left, right));
                });
    }

    /// <summary>
    /// Trigonometric symbols using System.Linq.Expressions.
    /// </summary>
    public static class TrigonometricExpressionSymbols
    {
        public static readonly List<ISymbol> AllSymbols = new()
        {
            Sine,
            Cosine,
            Tangent
        };

        public static readonly ExpressionSymbol<double> Sine =
            ExpressionSymbolFactory<double>.CreateUnary(
                "Sine", "Sine operation (sin)",
                arg => Expression.Call(typeof(Math).GetMethod(nameof(Math.Sin)), arg));

        public static readonly ExpressionSymbol<double> Cosine =
            ExpressionSymbolFactory<double>.CreateUnary(
                "Cosine", "Cosine operation (cos)",
                arg => Expression.Call(typeof(Math).GetMethod(nameof(Math.Cos)), arg));

        public static readonly ExpressionSymbol<double> Tangent =
            ExpressionSymbolFactory<double>.CreateUnary(
                "Tangent", "Tangent operation (tan)",
                arg => Expression.Call(typeof(Math).GetMethod(nameof(Math.Tan)), arg));
    }

    /// <summary>
    /// Discrete mathematical symbols using System.Linq.Expressions.
    /// </summary>
    public static class DiscreteExpressionSymbols
    {
        public static readonly List<ISymbol> AllSymbols = new()
        {
            Factorial,
            Permutation,
            Combination
        };

        public static readonly ExpressionSymbol<int> Factorial =
            ExpressionSymbolFactory<int>.CreateUnary(
                "Factorial", "Factorial operation (n!)",
                n => Expression.Call(typeof(DiscreteExpressionSymbols).GetMethod(nameof(FactorialImpl)), n));

        public static readonly ExpressionSymbol<int> Permutation =
            ExpressionSymbolFactory<int>.CreateBinary(
                "Permutation", "Permutation operation (nPr)",
                (n, r) => Expression.Call(typeof(DiscreteExpressionSymbols).GetMethod(nameof(PermutationImpl)), n, r));

        /// <summary>
        /// Combination as a composite symbol: P(n,r) / r!
        /// This demonstrates how to create complex expressions using System.Linq.Expressions.
        /// </summary>
        public static readonly ExpressionSymbol<int> Combination =
            ExpressionSymbolFactory<int>.CreateComposite(
                "Combination", "Combination operation (nCr) as P(n,r) / r!", 2,
                parameters =>
                {
                    var n = parameters[0];
                    var r = parameters[1];
                    
                    // P(n,r)
                    var permutation = Expression.Call(
                        typeof(DiscreteExpressionSymbols).GetMethod(nameof(PermutationImpl)), n, r);
                    
                    // r!
                    var factorial = Expression.Call(
                        typeof(DiscreteExpressionSymbols).GetMethod(nameof(FactorialImpl)), r);
                    
                    // P(n,r) / r!
                    return Expression.Divide(permutation, factorial);
                });

        /// <summary>
        /// Helper method for factorial calculation (called by expression).
        /// </summary>
        public static int FactorialImpl(int n)
        {
            if (n < 0) throw new ArgumentException("Negative numbers do not have factorials.");
            if (n == 0) return 1;
            
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        /// <summary>
        /// Helper method for permutation calculation (called by expression).
        /// </summary>
        public static int PermutationImpl(int n, int r)
        {
            if (n < 0 || r < 0 || r > n)
                throw new ArgumentException("Invalid values for permutation.");
            
            int result = 1;
            for (int i = n; i > n - r; i--)
            {
                result *= i;
            }
            return result;
        }
    }
}
