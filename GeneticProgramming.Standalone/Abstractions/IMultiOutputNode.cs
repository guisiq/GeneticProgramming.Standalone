using System.Collections.Generic;
using GeneticProgramming.Expressions;
using GeneticProgramming.Standalone.Core;

namespace GeneticProgramming.Standalone.Abstractions;

/// <summary>
/// Interface for root nodes that manage multiple outputs in symbolic expression trees.
/// This node serves as the container for multiple output nodes and manages their evaluation strategy.
/// </summary>
/// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
public interface IMultiOutputNode<T> : ISymbolicExpressionTreeNode<IReadOnlyList<T>>
    where T : notnull
{
    /// <summary>
    /// Gets the number of outputs this node manages.
    /// </summary>
    /// <value>The number of output values produced by this node</value>
    int OutputCount { get; }
}
