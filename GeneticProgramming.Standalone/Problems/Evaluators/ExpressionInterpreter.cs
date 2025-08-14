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
            // Avalia recursivamente todos os filhos primeiro
            var childValues = node.Subtrees
                .Select(child => EvaluateNodeRecursively(child, variables))
                .ToArray();
                
            // Depois avalia o nó atual com os valores dos filhos
            return node.Evaluate(childValues, variables);
        }
    }
}
