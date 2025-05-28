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
        ISymbolicExpressionTreeGrammar Grammar { get; }
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
    }

    /// <summary>
    /// Interface for symbols in the expression tree
    /// </summary>
    public interface ISymbol : IItem
    {
        int MinimumArity { get; }
        int MaximumArity { get; }
        bool Enabled { get; set; }
        double Weight { get; set; }
        string SymbolName { get; }
    }

    /// <summary>
    /// Interface for grammars that define allowed symbols and rules
    /// </summary>
    public interface ISymbolicExpressionTreeGrammar : IItem
    {
        IEnumerable<ISymbol> Symbols { get; }
        IEnumerable<ISymbol> AllowedSymbols { get; }
        ISymbol ProgramRootSymbol { get; }
        ISymbol DefunSymbol { get; }

        bool ContainsSymbol(ISymbol symbol);
        void AddSymbol(ISymbol symbol);
        void RemoveSymbol(ISymbol symbol);

        IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent);
        IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int argumentIndex);

        int GetMinimumSubtreeCount(ISymbol symbol);
        int GetMaximumSubtreeCount(ISymbol symbol);

        void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount);
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
