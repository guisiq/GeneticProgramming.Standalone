using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Abstractions;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Problems.Evaluators;


/// <summary>
/// Evaluates multi-output symbolic expression trees with intelligent caching for shared nodes.
/// This interpreter avoids redundant computations by caching the results of nodes that
/// appear in multiple output paths.
/// </summary>
public class MultiOutputExpressionInterpreter
{
    private readonly Dictionary<ISymbolicExpressionTreeNode, object> _evaluationCache = new();

    /// <summary>
    /// Evaluates a multi-output symbolic expression tree.
    /// </summary>
    /// <typeparam name="T">The base value type for each output (must be a struct and implement IConvertible)</typeparam>
    /// <param name="tree">The multi-output tree to evaluate</param>
    /// <param name="variables">Variable assignments for evaluation</param>
    /// <returns>A read-only list of evaluated values for each output</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when tree or variables are null
    /// </exception>
    public IReadOnlyList<T> Evaluate<T>(IMultiSymbolicExpressionTree<T> tree, IDictionary<string, T> variables) 
        where T : notnull
    {
        if (tree == null)
            throw new ArgumentNullException(nameof(tree));
        if (variables == null)
            throw new ArgumentNullException(nameof(variables));

        _evaluationCache.Clear();
        var results = new T[tree.OutputCount];

        for (int i = 0; i < tree.OutputCount; i++)
        {
            var outputNode = tree.GetOutputNode(i);
            if (outputNode != null)
            {
                results[i] = EvaluateWithCache(outputNode, variables);
            }
            else
            {
                // For value types like double, int, etc., this will be 0 or equivalent
                // For struct types that implement IConvertible, this is the appropriate fallback
                results[i] = (T)Convert.ChangeType(0, typeof(T));
            }
        }

        return results;
    }

    /// <summary>
    /// Recursively evaluates a node with caching.
    /// </summary>
    /// <typeparam name="T">The value type of the node</typeparam>
    /// <param name="node">The node to evaluate</param>
    /// <param name="variables">Variable assignments</param>
    /// <returns>The evaluated result of the node</returns>
    private T EvaluateWithCache<T>(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> variables) 
        where T : notnull
    {
        // If the node result is already in the cache, return it.
        if (_evaluationCache.TryGetValue(node, out var cachedResult))
        {
            return (T)cachedResult;
        }

        T result;
        var symbol = node.Symbol;

        if (symbol is FunctionalSymbol<T> functionalSymbol)
        {
            var arguments = new T[node.SubtreeCount];
            int i = 0;
            foreach (var subtree in node.Subtrees)
            {
                // This cast is safe as we know subtrees are of type ISymbolicExpressionTreeNode<T>
                arguments[i++] = EvaluateWithCache((ISymbolicExpressionTreeNode<T>)subtree, variables);
            }
            result = ((IEvaluable<T>)functionalSymbol).Evaluate(arguments, variables);
        }
        else if (symbol is Variable<T> variableSymbol)
        {
            if (variables.TryGetValue(variableSymbol.Name, out var value))
            {
                result = value;
            }
            else
            {
                throw new ArgumentException($"Variable '{variableSymbol.Name}' not found.");
            }
        }
        else if (symbol is Constant<T>)
        {
            // For constants, the value is stored in the tree node itself
            if (node is ConstantTreeNode<T> constantNode)
            {
                result = constantNode.Value;
            }
            else
            {
                throw new InvalidOperationException("Constant symbol must be used with ConstantTreeNode");
            }
        }
        else if (symbol is TerminalSymbol<T> && symbol is IEvaluable<T> evaluableTerminal)
        {
            // Other terminal symbols that implement IEvaluable
            result = evaluableTerminal.Evaluate(Array.Empty<T>(), variables);
        }
        else
        {
            throw new NotSupportedException($"Symbol type {symbol.GetType()} is not supported for evaluation by this interpreter.");
        }

        // Store the result in the cache and return it.
        _evaluationCache[node] = result;
        return result;
    }
}
