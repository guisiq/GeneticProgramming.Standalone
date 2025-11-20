using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using static GeneticProgramming.Expressions.Symbols.MathematicalSymbols;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Simple interpreter for evaluating symbolic expression trees.
    /// Uses the dynamic Evaluate method implemented in tree nodes.
    /// </summary>
    public class ExpressionInterpreter
    {
        /// <summary>
        /// Avalia uma árvore de expressão simbólica usando o método dinâmico do nó raiz.
        /// </summary>
        /// <typeparam name="T">Tipo de valor que a árvore retorna</typeparam>
        /// <param name="tree">Árvore a ser avaliada</param>
        /// <param name="variables">Variáveis para avaliação</param>
        /// <returns>Resultado da avaliação</returns>
        public T Evaluate<T>(ISymbolicExpressionTree<T> tree, IDictionary<string, T> variables) where T : notnull
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            
            // Cada nó é responsável por avaliar seus próprios filhos recursivamente
            // O método Evaluate do nó deve lidar com a avaliação completa da subárvore
            return EvaluateNodeRecursively(tree.Root, variables);
        }
        
        private static T EvaluateNodeRecursively<T>(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> variables) where T : notnull
        {
            // Optimization: Avoid LINQ allocations
            var subtrees = node.Subtrees;
            int count = node.SubtreeCount;
            
            // Use ArrayPool to avoid allocations for child values array
            var childValues = System.Buffers.ArrayPool<T>.Shared.Rent(count);
            
            try 
            {
                int i = 0;
                foreach (var child in subtrees)
                {
                     if (child is ISymbolicExpressionTreeNode<T> typedChild)
                     {
                         childValues[i] = EvaluateNodeRecursively(typedChild, variables);
                     }
                     i++;
                }
                
                // We need to pass exact array size to Evaluate because some implementations might iterate the whole array
                // If count is 0, we can pass empty array
                T[] exactChildValues;
                if (count == 0)
                {
                    exactChildValues = Array.Empty<T>();
                }
                else
                {
                    // Unfortunately we have to allocate here to be safe with the interface contract T[]
                    // unless we are sure implementations use Length or we change interface to Span/ArraySegment
                    // However, this is still better than LINQ Select + ToArray which allocates enumerators
                    exactChildValues = new T[count];
                    Array.Copy(childValues, exactChildValues, count);
                }
                
                return node.Evaluate(exactChildValues, variables);
            }
            finally
            {
                System.Buffers.ArrayPool<T>.Shared.Return(childValues);
            }
        }
    }
}
