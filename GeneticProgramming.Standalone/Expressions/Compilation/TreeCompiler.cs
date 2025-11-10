using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Abstractions;

namespace GeneticProgramming.Expressions.Compilation
{
    /// <summary>
    /// Compila árvores de expressão simbólica em delegados fortemente tipados.
    /// </summary>
    /// <typeparam name="T">Tipo de valor da árvore.</typeparam>
    public sealed class TreeCompiler<T> where T : notnull
    {
        private readonly ParameterExpression _variablesParameter = Expression.Parameter(typeof(IDictionary<string, T>), "variables");

        /// <summary>
        /// Compila uma árvore em um delegado reutilizável.
        /// </summary>
        public Func<IDictionary<string, T>, T> Compile(ISymbolicExpressionTree<T> tree)
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            if (tree.Root == null) throw new InvalidOperationException("Tree root cannot be null");

            var body = BuildNodeExpression(tree.Root);
            var lambda = Expression.Lambda<Func<IDictionary<string, T>, T>>(EnsureType(body, typeof(T)), _variablesParameter);
            return lambda.Compile();
        }

        private Expression BuildNodeExpression(ISymbolicExpressionTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            // Constantes podem ser tratadas diretamente
            if (node is ConstantTreeNode<T> constantNode)
            {
                return Expression.Constant(constantNode.Value, typeof(T));
            }

            var childExpressions = node.Subtrees
                .Select(BuildNodeExpression)
                .Select(expr => EnsureType(expr, typeof(T)))
                .ToArray();

            if (node.Symbol is ICompilableSymbol<T> compilableSymbol)
            {
                var compiled = compilableSymbol.BuildExpression(childExpressions, _variablesParameter);
                return EnsureType(compiled, typeof(T));
            }

            var evaluateMethod = node.GetType().GetMethod(
                nameof(ISymbolicExpressionTreeNode<T>.Evaluate),
                new[] { typeof(T[]), typeof(IDictionary<string, T>) });

            if (evaluateMethod == null)
            {
                throw new InvalidOperationException($"Node type {node.GetType().Name} does not expose an Evaluate method.");
            }

            var childArray = Expression.NewArrayInit(typeof(T), childExpressions);
            var nodeConstant = Expression.Constant(node);
            var call = Expression.Call(nodeConstant, evaluateMethod, childArray, _variablesParameter);
            return EnsureType(call, typeof(T));
        }

        private static Expression EnsureType(Expression expression, Type targetType)
        {
            if (expression.Type == targetType)
            {
                return expression;
            }

            return Expression.Convert(expression, targetType);
        }
    }
}
