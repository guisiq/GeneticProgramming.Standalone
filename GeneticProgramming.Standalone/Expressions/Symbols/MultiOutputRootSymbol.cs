using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;
using GeneticProgramming.Core;
using GeneticProgramming.Standalone.Core;

namespace GeneticProgramming.Standalone.Expressions.Symbols;

/// <summary>
/// Symbol for multi-output root nodes in symbolic expression trees.
/// This symbol represents a node that can manage multiple outputs of the same type.
/// </summary>
/// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
public class MultiOutputRootSymbol<T> : Symbol<IReadOnlyList<T>> where T : notnull
{
    private readonly int _outputCount;

    /// <summary>
    /// Initializes a new instance of the MultiOutputRootSymbol class.
    /// </summary>
    /// <param name="outputCount">Number of outputs this symbol will manage</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when outputCount is less than 1
    /// </exception>
    public MultiOutputRootSymbol(int outputCount) : base($"MultiOutput{outputCount}", $"Multi-output root with {outputCount} outputs")
    {
        if (outputCount < 1)
            throw new ArgumentOutOfRangeException(nameof(outputCount), "Output count must be at least 1");

        _outputCount = outputCount;
    }

    /// <summary>
    /// Copy constructor for cloning.
    /// </summary>
    /// <param name="original">Original symbol to clone</param>
    /// <param name="cloner">Cloner instance</param>
    protected MultiOutputRootSymbol(MultiOutputRootSymbol<T> original, Cloner cloner) : base(original, cloner)
    {
        _outputCount = original._outputCount;
    }

    /// <summary>
    /// Gets the number of outputs this symbol manages.
    /// </summary>
    public int OutputCount => _outputCount;

    /// <summary>
    /// Gets the input types for this symbol. Multi-output root accepts multiple T inputs.
    /// </summary>
    public override Type[] InputTypes
    {
        get
        {
            var inputTypes = new Type[_outputCount];
            for (int i = 0; i < _outputCount; i++)
            {
                inputTypes[i] = typeof(T);
            }
            return inputTypes;
        }
    }

    /// <summary>
    /// Gets the output type for this symbol (IReadOnlyList&lt;T&gt;).
    /// </summary>
    public override Type OutputType => typeof(IReadOnlyList<T>);

    /// <summary>
    /// Gets the minimum arity (number of children) for this symbol.
    /// </summary>
    public override int MinimumArity => _outputCount;

    /// <summary>
    /// Gets the maximum arity (number of children) for this symbol.
    /// </summary>
    public override int MaximumArity => _outputCount;

    /// <summary>
    /// Creates a tree node for this symbol.
    /// </summary>
    /// <returns>A new MultiOutputRootNode instance</returns>
    public override ISymbolicExpressionTreeNode<IReadOnlyList<T>> CreateTreeNode()
    {
        return new MultiOutputRootNode<T>(_outputCount);
    }

    /// <summary>
    /// Checks if the given child type is compatible at the specified index.
    /// </summary>
    /// <param name="childType">Type of the child</param>
    /// <param name="argumentIndex">Index of the argument</param>
    /// <returns>True if the child type is compatible</returns>
    public override bool IsCompatibleChildType(Type childType, int argumentIndex)
    {
        if (argumentIndex < 0 || argumentIndex >= _outputCount)
            return false;

        return childType == typeof(T) || childType.IsAssignableFrom(typeof(T));
    }

    /// <summary>
    /// Gets the format string for this symbol.
    /// </summary>
    /// <returns>Format string representation</returns>
    public override string GetFormatString()
    {
        return $"MultiOutput({_outputCount})";
    }

    /// <summary>
    /// Creates a clone instance of this symbol.
    /// </summary>
    /// <param name="cloner">Cloner instance</param>
    /// <returns>Cloned symbol</returns>
    protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
    {
        return new MultiOutputRootSymbol<T>(this, cloner);
    }

    /// <summary>
    /// Gets a string representation of this symbol.
    /// </summary>
    /// <returns>String representation</returns>
    public override string ToString()
    {
        return $"MultiOutput({_outputCount})";
    }

    /// <summary>
    /// Checks if this symbol equals another object.
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <returns>True if objects are equal</returns>
    public override bool Equals(object? obj)
    {
        if (obj is MultiOutputRootSymbol<T> other)
        {
            return _outputCount == other._outputCount && base.Equals(obj);
        }
        return false;
    }

    /// <summary>
    /// Gets the hash code for this symbol.
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), _outputCount);
    }
}
