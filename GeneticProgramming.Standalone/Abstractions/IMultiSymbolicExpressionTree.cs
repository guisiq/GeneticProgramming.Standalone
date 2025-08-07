using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;
using GeneticProgramming.Core;

namespace GeneticProgramming.Abstractions
{
    /// <summary>
    /// Interface for multi-output symbolic expression trees that can produce multiple values
    /// of the same type. This interface extends the standard symbolic expression tree
    /// to support evaluating multiple outputs simultaneously with potential node sharing
    /// for efficiency.
    /// </summary>
    /// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
    public interface IMultiSymbolicExpressionTree<T> : ISymbolicExpressionTree<IReadOnlyList<T>>
        where T : notnull
    {
        /// <summary>
        /// Gets the number of outputs this tree produces.
        /// </summary>
        /// <value>The number of output values produced by this tree</value>
        int OutputCount { get; }
        
        /// <summary>
        /// Gets the total length (number of nodes) in the tree
        /// </summary>
        int Length { get; }
        
        /// <summary>
        /// Gets the depth of the tree
        /// </summary>
        int Depth { get; }
        
        /// <summary>
        /// Gets the output node at the specified index.
        /// </summary>
        /// <param name="outputIndex">Zero-based index of the output</param>
        /// <returns>The symbolic expression tree node for the specified output</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when outputIndex is less than 0 or greater than or equal to OutputCount
        /// </exception>
        ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex);
        
        /// <summary>
        /// Sets the output node at the specified index.
        /// </summary>
        /// <param name="outputIndex">Zero-based index of the output</param>
        /// <param name="outputNode">The node to set as output</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when outputIndex is less than 0 or greater than or equal to OutputCount
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when outputNode is null
        /// </exception>
        void SetOutputNode(int outputIndex, ISymbolicExpressionTreeNode<T> outputNode);
        
        /// <summary>
        /// Evaluates all outputs with the given variables.
        /// This method efficiently evaluates all outputs, leveraging shared nodes
        /// when possible to avoid redundant computations.
        /// </summary>
        /// <param name="variables">Variable assignments for evaluation</param>
        /// <returns>List of evaluated values for each output</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when variables is null
        /// </exception>
        IReadOnlyList<T> EvaluateAll(IDictionary<string, T> variables);
        
        /// <summary>
        /// Gets nodes that are shared between multiple outputs.
        /// Shared nodes are those that appear in the evaluation path of more than one output,
        /// which allows for optimization during evaluation by caching their results.
        /// </summary>
        /// <returns>Collection of shared nodes that appear in multiple output paths</returns>
        IReadOnlyList<ISymbolicExpressionTreeNode<T>> GetSharedNodes();
        
        /// <summary>
        /// Creates a compatible single-output tree view for interoperability with existing algorithms.
        /// This adapter allows multi-output trees to work with existing operators and evaluators
        /// that expect single-output trees.
        /// </summary>
        /// <returns>A single-output tree that evaluates to IReadOnlyList&lt;T&gt;</returns>
        ISymbolicExpressionTree<T> ToSingleOutputView();
        
        /// <summary>
        /// Gets a string representation of the tree structure for debugging
        /// </summary>
        string ToTreeString();
        
        /// <summary>
        /// Gets a mathematical representation of all outputs as a string
        /// </summary>
        string ToMathString();
    }
}
