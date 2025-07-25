using System;
using System.Collections.Generic;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Interface for symbolic expression tree nodes
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
    /// Interface for symbolic expression trees
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
