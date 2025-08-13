using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Standalone.Abstractions;
using GeneticProgramming.Standalone.Core;
using GeneticProgramming.Standalone.Expressions.Symbols;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Expressions;

/// <summary>
/// Root node implementation that manages multiple outputs in symbolic expression trees.
/// This is a simplified implementation that provides basic multi-output functionality.
/// </summary>
/// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
public class MultiOutputRootNode<T> : IMultiOutputNode<T>, ISymbolicExpressionTreeNode<IReadOnlyList<T>>

    where T : notnull
{
    // Cache de avaliação: chave = (symbolId, outputIndex, hash das variáveis)
    private readonly Dictionary<(long, int, int), T> _evalCache = new();
    /// <summary>
    /// Implementa ISymbolicExpressionTreeNode<IReadOnlyList<T>>.Evaluate.
    /// Avalia usando variáveis multi-output, convertendo para variáveis simples quando necessário.
    /// </summary>
    /// <param name="ChildValues">Argumentos de entrada (não utilizado nesta implementação)</param>
    /// <param name="variables">Dicionário de variáveis multi-output</param>
    public IReadOnlyList<T> Evaluate(T[] ChildValues, IDictionary<string, T> variables)
    {
        var results = new T[_outputCount];
        for (int i = 0; i < _outputCount; i++)
        {
            var outputNode = _outputNodes[i];
            if (outputNode != null)
            {
                var symbolId = outputNode.Symbol is ISymbol s ? s.GetHashCode() : outputNode.GetHashCode();
                var varHash = ComputeVariablesHash(variables);
                var cacheKey = (symbolId, i, varHash);
                if (_evalCache.TryGetValue(cacheKey, out var cached))
                {
                    results[i] = cached;
                }
                else
                {
                    var value = EvaluateNode(outputNode, variables);
                    _evalCache[cacheKey] = value;
                    results[i] = value;
                }
            }
            else
            {
                results[i] = default(T)!;
            }
        }
        return results;

    }

    // Função utilitária para gerar um hash rápido das variáveis
    private static int ComputeVariablesHash(IDictionary<string, T> variables)
    {
        unchecked
        {
            int hash = 17;
            foreach (var kvp in variables.OrderBy(x => x.Key))
            {
                hash = hash * 23 + kvp.Key.GetHashCode();
                hash = hash * 23 + (kvp.Value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
    
    public IReadOnlyList<T> Evaluate(IReadOnlyList<T>[] arguments, IDictionary<string, IReadOnlyList<T>> variables)
    {
        // Converte variáveis multi-output para variáveis simples
        var convertedVars = new Dictionary<string, T>();
        if (variables != null)
        {
            foreach (var kvp in variables)
            {
                if (kvp.Value != null && kvp.Value.Count > 0)
                    convertedVars[kvp.Key] = kvp.Value[0];
            }
        }
        return Evaluate(Array.Empty<T>(), convertedVars);
    }
    /// <summary>
    /// Evaluates all outputs with the given variables using a simple interpreter.
    /// </summary>
    /// <param name="variables">Variable assignments for evaluation</param>
    /// <returns>List of evaluated values for each output</returns>
    /// <exception cref="ArgumentNullException">Thrown when variables is null</exception>
    public IReadOnlyList<T> Evaluate(IDictionary<string, IReadOnlyList<T>> variables)
    {
        return Evaluate(new IReadOnlyList<T>[0], variables);
    }
    private readonly int _outputCount;
    private readonly ISymbolicExpressionTreeNode<T>[] _outputNodes;
    private ISymbolicExpressionTreeNode<IReadOnlyList<T>>? _parent;

    /// <summary>
    /// Initializes a new instance of the MultiOutputRootNode class.
    /// </summary>
    /// <param name="outputCount">Number of outputs this node will manage</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputCount is less than 1
    /// </exception>
    public MultiOutputRootNode(int outputCount)
    {
        if (outputCount < 1)
            throw new ArgumentOutOfRangeException(nameof(outputCount), "Output count must be at least 1");

        _outputCount = outputCount;
        _outputNodes = new ISymbolicExpressionTreeNode<T>[outputCount];
        Symbol = new MultiOutputRootSymbol<T>(outputCount);
    }

    /// <summary>
    /// Gets the number of outputs this node manages.
    /// </summary>
    public int OutputCount => _outputCount;

    // Basic ISymbolicExpressionTreeNode<IReadOnlyList<T>> implementation
    public ISymbol<IReadOnlyList<T>> Symbol { get; private set; }
    public Type OutputType => typeof(IReadOnlyList<T>);
    public ISymbolicExpressionTreeGrammar<IReadOnlyList<T>>? Grammar => Parent?.Grammar;
    public ISymbolicExpressionTreeNode<IReadOnlyList<T>>? Parent 
    { 
        get => _parent; 
        set => _parent = value; 
    }
    public bool HasLocalParameters => false;
    public IEnumerable<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> Subtrees => 
        _outputNodes.Where(n => n != null).Cast<ISymbolicExpressionTreeNode<IReadOnlyList<T>>>();
    public int SubtreeCount => _outputNodes.Count(n => n != null);
    public bool IsCompatibleChild(ISymbolicExpressionTreeNode<IReadOnlyList<T>> child) => true;

    /// <summary>
    /// Gets the output node at the specified index.
    /// </summary>
    /// <param name="outputIndex">Zero-based index of the output</param>
    /// <returns>The output node at the specified index, or null if not set</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputIndex is out of valid range
    /// </exception>
    public ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex)
    {
        ValidateOutputIndex(outputIndex);
        return _outputNodes[outputIndex];
    }

    /// <summary>
    /// Sets the output node at the specified index.
    /// </summary>
    /// <param name="outputIndex">Zero-based index of the output</param>
    /// <param name="outputNode">The node to set as output</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputIndex is out of valid range
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when outputNode is null
    /// </exception>
    public void SetOutputNode(int outputIndex, ISymbolicExpressionTreeNode<T> outputNode)
    {
        ValidateOutputIndex(outputIndex);
        if (outputNode == null)
            throw new ArgumentNullException(nameof(outputNode));

        _outputNodes[outputIndex] = outputNode;
        // Note: Cannot set parent due to type mismatch - this is a simplified implementation
    }

    

    /// <summary>
    /// Gets nodes that are shared between multiple outputs.
    /// </summary>
    /// <returns>Collection of shared nodes</returns>
    public IReadOnlyList<ISymbolicExpressionTreeNode<T>> GetSharedNodes()
    {
        var nodeReferences = new Dictionary<ISymbolicExpressionTreeNode<T>, int>();

        // Count references in all outputs
        for (int i = 0; i < _outputCount; i++)
        {
            var outputNode = _outputNodes[i];
            if (outputNode != null)
            {
                foreach (var node in IterateNodesPostfix(outputNode))
                {
                    nodeReferences[node] = nodeReferences.GetValueOrDefault(node, 0) + 1;
                }
            }
        }

        // Return nodes with multiple references
        return nodeReferences.Where(kvp => kvp.Value > 1)
                           .Select(kvp => kvp.Key)
                           .ToList()
                           .AsReadOnly();
    }

    /// <summary>
    /// Creates a deep copy of this node.
    /// </summary>
    /// <param name="cloner">Cloner instance</param>
    /// <returns>A deep copy of this node</returns>
    public IDeepCloneable Clone(Cloner cloner)
    {
        var clone = new MultiOutputRootNode<T>(_outputCount);
        
        for (int i = 0; i < _outputCount; i++)
        {
            if (_outputNodes[i] != null)
            {
                var clonedOutputNode = cloner.Clone(_outputNodes[i]) as ISymbolicExpressionTreeNode<T>;
                if (clonedOutputNode != null)
                {
                    clone.SetOutputNode(i, clonedOutputNode);
                }
            }
        }

        return clone;
    }

    /// <summary>
    /// Gets a string representation of the node for debugging.
    /// </summary>
    /// <returns>String representation</returns>
    public override string ToString()
    {
        var setOutputs = _outputNodes.Count(n => n != null);
        return $"MultiOutput({_outputCount} outputs, {setOutputs} set)";
    }

    // Helper methods
    private void ValidateOutputIndex(int outputIndex)
    {
        if (outputIndex < 0 || outputIndex >= _outputCount)
        {
            throw new ArgumentOutOfRangeException(nameof(outputIndex), 
                $"Output index must be between 0 and {_outputCount - 1}");
        }
    }
    
    protected virtual T EvaluateNode(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> variables)
    {
        // Avalia recursivamente os filhos do nó
        var childValues = node.Subtrees
            .Select(child => EvaluateNode(child, variables))
            .ToArray();

        // Usa o método Evaluate do próprio nó que implementa IEvaluable<T>
        return node.Evaluate(childValues, variables);
    }

    private IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix(ISymbolicExpressionTreeNode<T> node)
    {
        // Recursively iterate through all subtrees first (postfix order)
        foreach (var child in node.Subtrees)
        {
            if (child is ISymbolicExpressionTreeNode<T> typedChild)
            {
                foreach (var descendant in IterateNodesPostfix(typedChild))
                {
                    yield return descendant;
                }
            }
        }
        
        // Return the node itself last (postfix)
        yield return node;
    }

    // Minimal implementations for interface compliance
    public int GetLength() => 1 + _outputNodes.Where(n => n != null).Sum(n => n.GetLength());
    public int GetDepth() => 1 + (_outputNodes.Where(n => n != null).Any() ? _outputNodes.Where(n => n != null).Max(n => n.GetDepth()) : 0);
    public int GetBranchLevel(ISymbolicExpressionTreeNode<IReadOnlyList<T>> child) => 0;
    public int GetBranchLevel(ISymbolicExpressionTreeNode child) => 0; // Non-generic overload
    public ISymbolicExpressionTreeNode<IReadOnlyList<T>> GetSubtree(int index) => throw new NotImplementedException("Simplified implementation");
    public int IndexOfSubtree(ISymbolicExpressionTreeNode<IReadOnlyList<T>> tree) => -1;
    public void AddSubtree(ISymbolicExpressionTreeNode<IReadOnlyList<T>> tree) => throw new NotImplementedException("Simplified implementation");
    public void InsertSubtree(int index, ISymbolicExpressionTreeNode<IReadOnlyList<T>> tree) => throw new NotImplementedException("Simplified implementation");
    public void RemoveSubtree(int index) => throw new NotImplementedException("Simplified implementation");
    public void ReplaceSubtree(int index, ISymbolicExpressionTreeNode<IReadOnlyList<T>> tree) => throw new NotImplementedException("Simplified implementation");
    public void ReplaceSubtree(ISymbolicExpressionTreeNode<IReadOnlyList<T>> old, ISymbolicExpressionTreeNode<IReadOnlyList<T>> tree) => throw new NotImplementedException("Simplified implementation");
    public IEnumerable<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> IterateNodesBreadth() => new[] { this };
    public IEnumerable<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> IterateNodesPostfix() => new[] { this };
    public IEnumerable<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> IterateNodesPrefix() => new[] { this };
    public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> action) => action(this);
    public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode<IReadOnlyList<T>>> action) => action(this);
    public void ResetLocalParameters(IRandom random) { }
    public void ShakeLocalParameters(IRandom random, double shakingFactor) { }
}
