using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Abstractions;
using GeneticProgramming.Standalone.Core;
using GeneticProgramming.Core;

namespace GeneticProgramming.Standalone.Expressions;

/// <summary>
/// Implementation of a multi-output symbolic expression tree.
/// This tree can produce multiple outputs of the same type with potential node sharing.
/// </summary>
/// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
public class MultiSymbolicExpressionTree<T> : SymbolicExpressionTree<IReadOnlyList<T>>, IMultiSymbolicExpressionTree<T>
    where T : notnull
{
    private readonly int _outputCount;
    private readonly MultiOutputRootNode<T> _multiRoot;

    /// <summary>
    /// Initializes a new instance of the MultiSymbolicExpressionTree class.
    /// </summary>
    /// <param name="outputCount">Number of outputs this tree will produce</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputCount is less than 1
    /// </exception>
    public MultiSymbolicExpressionTree(int outputCount)
    {
        if (outputCount < 1)
            throw new ArgumentOutOfRangeException(nameof(outputCount), "Output count must be at least 1");

        _outputCount = outputCount;
        _multiRoot = new MultiOutputRootNode<T>(outputCount);
        
        // Set the root to our multi-output root node
        Root = _multiRoot;
    }

    /// <summary>
    /// Copy constructor for cloning.
    /// </summary>
    /// <param name="original">Original tree to clone</param>
    /// <param name="cloner">Cloner instance</param>
    protected MultiSymbolicExpressionTree(MultiSymbolicExpressionTree<T> original, Cloner cloner) : base(original, cloner)
    {
        _outputCount = original._outputCount;
        _multiRoot = (MultiOutputRootNode<T>)Root;
    }

    /// <summary>
    /// Gets the number of outputs this tree produces.
    /// </summary>
    public int OutputCount => _outputCount;

    /// <summary>
    /// Gets the output node at the specified index.
    /// </summary>
    /// <param name="outputIndex">Zero-based index of the output</param>
    /// <returns>The output node at the specified index</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputIndex is out of valid range
    /// </exception>
    public ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex)
    {
        return _multiRoot.GetOutputNode(outputIndex);
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
        _multiRoot.SetOutputNode(outputIndex, outputNode);
    }

    /// <summary>
    /// Evaluates all outputs with the given variables.
    /// This method efficiently evaluates all outputs, leveraging shared nodes when possible.
    /// </summary>
    /// <param name="variables">Variable assignments for evaluation</param>
    /// <returns>List of evaluated values for each output</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when variables is null
    /// </exception>
    public IReadOnlyList<T> EvaluateAll(IDictionary<string, T> variables)
    {
        return _multiRoot.Evaluate(variables);
    }

    /// <summary>
    /// Gets nodes that are shared between multiple outputs.
    /// Shared nodes are those that appear in the evaluation path of more than one output.
    /// </summary>
    /// <returns>Collection of shared nodes</returns>
    public IReadOnlyList<ISymbolicExpressionTreeNode<T>> GetSharedNodes()
    {
        return _multiRoot.GetSharedNodes();
    }

    /// <summary>
    /// Creates a compatible single-output tree view for interoperability with existing algorithms.
    /// This allows multi-output trees to work with existing operators and evaluators.
    /// </summary>
    /// <returns>A single-output tree that evaluates to IReadOnlyList&lt;T&gt;</returns>
    public ISymbolicExpressionTree<T> ToSingleOutputView()
    {
        throw new NotImplementedException("Single output view will be implemented in a future version");
    }

    /// <summary>
    /// Creates a clone instance of this tree.
    /// </summary>
    /// <param name="cloner">Cloner instance</param>
    /// <returns>Cloned tree</returns>
    protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
    {
        return new MultiSymbolicExpressionTree<T>(this, cloner);
    }

    /// <summary>
    /// Gets a string representation of this tree.
    /// </summary>
    /// <returns>String representation</returns>
    public override string ToString()
    {
        var setOutputs = 0;
        for (int i = 0; i < _outputCount; i++)
        {
            if (GetOutputNode(i) != null) setOutputs++;
        }
        
        return $"MultiSymbolicExpressionTree({_outputCount} outputs, {setOutputs} set, Length={Length}, Depth={Depth})";
    }
}
