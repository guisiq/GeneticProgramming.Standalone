using System;
using System.Collections.Generic;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Interface for symbolic expression tree nodes (non-generic for backward compatibility)
    /// </summary>
    public interface ISymbolicExpressionTreeNode : IDeepCloneable
    {
        ISymbolicExpressionTreeGrammar? Grammar { get; }
        ISymbolicExpressionTreeNode? Parent { get; set; }
        ISymbol Symbol { get; }
        bool HasLocalParameters { get; }

        int GetDepth();
        int GetLength();
        int GetBranchLevel(ISymbolicExpressionTreeNode child);

        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth();
        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix();
        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix();
        void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> action);
        void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> action);

        IEnumerable<ISymbolicExpressionTreeNode> Subtrees { get; }
        int SubtreeCount { get; }
        ISymbolicExpressionTreeNode GetSubtree(int index);
        int IndexOfSubtree(ISymbolicExpressionTreeNode tree);
        void AddSubtree(ISymbolicExpressionTreeNode tree);
        void InsertSubtree(int index, ISymbolicExpressionTreeNode tree);
        void RemoveSubtree(int index);
        void ReplaceSubtree(int index, ISymbolicExpressionTreeNode tree);
        void ReplaceSubtree(ISymbolicExpressionTreeNode original, ISymbolicExpressionTreeNode replacement);

        void ResetLocalParameters(IRandom random);
        void ShakeLocalParameters(IRandom random, double shakingFactor);
    }

    /// <summary>
    /// Generic interface for symbolic expression tree nodes
    /// </summary>
    /// <typeparam name="T">The value type that the node evaluates to (must be a struct)</typeparam>
    public interface ISymbolicExpressionTreeNode<T> : ISymbolicExpressionTreeNode where T : struct
    {
        new ISymbolicExpressionTreeGrammar<T>? Grammar { get; }
        new ISymbolicExpressionTreeNode<T>? Parent { get; set; }
        new ISymbol<T> Symbol { get; }

        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesBreadth();
        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix();
        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPrefix();
        void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode<T>> action);
        void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode<T>> action);

        new IEnumerable<ISymbolicExpressionTreeNode<T>> Subtrees { get; }
        new ISymbolicExpressionTreeNode<T> GetSubtree(int index);
        int IndexOfSubtree(ISymbolicExpressionTreeNode<T> tree);
        void AddSubtree(ISymbolicExpressionTreeNode<T> tree);
        void InsertSubtree(int index, ISymbolicExpressionTreeNode<T> tree);
        void ReplaceSubtree(int index, ISymbolicExpressionTreeNode<T> tree);
        void ReplaceSubtree(ISymbolicExpressionTreeNode<T> original, ISymbolicExpressionTreeNode<T> replacement);

        /// <summary>
        /// Gets the output type of this node
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// Validates type compatibility when adding a subtree
        /// </summary>
        /// <param name="child">The child node to validate</param>
        /// <returns>True if the child is compatible, false otherwise</returns>
        bool IsCompatibleChild(ISymbolicExpressionTreeNode<T> child);
    }

    /// <summary>
    /// Interface for symbolic expression trees (non-generic for backward compatibility)
    /// </summary>
    public interface ISymbolicExpressionTree : IItem
    {
        ISymbolicExpressionTreeNode Root { get; set; }
        int Length { get; }
        int Depth { get; }

        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth();
        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix();
        IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix();
        
        // Métodos de visualização
        string ToTreeString();
        string ToMathString();
        void PrintTree();
    }

    /// <summary>
    /// Generic interface for symbolic expression trees
    /// </summary>
    /// <typeparam name="T">The value type that the tree evaluates to (must be a struct)</typeparam>
    public interface ISymbolicExpressionTree<T> : ISymbolicExpressionTree where T : struct
    {
        new ISymbolicExpressionTreeNode<T> Root { get; set; }

        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesBreadth();
        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPrefix();
        new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix();

        /// <summary>
        /// Gets the output type of this tree
        /// </summary>
        Type OutputType { get; }
    }

    /// <summary>
    /// Interface for random number generation
    /// </summary>
    public interface IRandom
    {
        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
        double NextDouble();
        void Reset();
        void Reset(int seed);
    }
}
