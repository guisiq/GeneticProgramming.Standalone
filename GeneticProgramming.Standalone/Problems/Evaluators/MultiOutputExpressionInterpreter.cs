using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Abstractions;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Problems.Evaluators;


/// <summary>
/// Evaluates multi-output symbolic expression trees using the new dynamic evaluation approach.
/// This interpreter now leverages the Evaluate methods implemented in the nodes themselves,
/// while maintaining intelligent caching for shared nodes when needed.
/// </summary>
public class MultiOutputExpressionInterpreter
{
    /// <summary>
    /// Avalia uma árvore de expressão multi-output usando o método dinâmico.
    /// </summary>
    /// <typeparam name="T">Tipo base de valor para cada saída</typeparam>
    /// <param name="tree">Árvore multi-output a ser avaliada</param>
    /// <param name="variables">Atribuições de variáveis para avaliação</param>
    /// <returns>Lista somente leitura de valores avaliados para cada saída</returns>
    public IReadOnlyList<T> Evaluate<T>(IMultiSymbolicExpressionTree<T> tree, IDictionary<string, T> variables) 
        where T : notnull
    {
        if (tree == null && variables == null)
            throw new ArgumentNullException("Tree and variables cannot be null.");


            return tree.Root.Evaluate(Array.Empty<IReadOnlyList<T>>(), 
                variables.ToDictionary(kvp => kvp.Key, kvp => new List<T> { kvp.Value } as IReadOnlyList<T>));

        // Fallback para avaliação manual (compatibilidade)
       // return EvaluateManually(tree, variables);
    }

    /// <summary>
    /// Método de fallback para avaliação manual quando o nó raiz não implementa IEvaluable.
    /// </summary>
    private IReadOnlyList<T> EvaluateManually<T>(IMultiSymbolicExpressionTree<T> tree, IDictionary<string, T> variables) 
        where T : notnull
    {
        var results = new T[tree.OutputCount];

        for (int i = 0; i < tree.OutputCount; i++)
        {
            var outputNode = tree.GetOutputNode(i);
            if (outputNode != null)
            {
                results[i] = outputNode.Evaluate(Array.Empty<T>(), variables);
            }
            else
            {
                results[i] = default(T)!;
                throw new ArgumentException($"Output node at index {i} is null or not set.");
            }
        }

        return results;
    }

    /// <summary>
    /// Método legado para avaliação de nós que ainda não implementam IEvaluable.
    /// </summary>
    private T EvaluateNodeLegacy<T>(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> variables) 
        where T : notnull
    {
        var symbol = node.Symbol;

        if (symbol is FunctionalSymbol<T> functionalSymbol)
        {
            var arguments = new T[node.SubtreeCount];
            int i = 0;
            foreach (var subtree in node.Subtrees)
            {
                arguments[i++] = EvaluateNodeLegacy((ISymbolicExpressionTreeNode<T>)subtree, variables);
            }
            return ((IEvaluable<T>)functionalSymbol).Evaluate(arguments, variables);
        }
        else if (symbol is Variable<T> variableSymbol)
        {
            if (variables.TryGetValue(variableSymbol.Name, out var value))
            {
                return value;
            }
            throw new ArgumentException($"Variable '{variableSymbol.Name}' not found.");
        }
        else if (symbol is Constant<T>)
        {
            if (node is ConstantTreeNode<T> constantNode)
            {
                return constantNode.Value;
            }
            throw new InvalidOperationException("Constant symbol must be used with ConstantTreeNode");
        }
        else if (symbol is TerminalSymbol<T> && symbol is IEvaluable<T> evaluableTerminal)
        {
            return evaluableTerminal.Evaluate(Array.Empty<T>(), variables);
        }

        throw new NotSupportedException($"Symbol type {symbol.GetType()} is not supported for evaluation by this interpreter.");
    }
}
