using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Represents a symbolic expression tree
    /// </summary>
    [Item("SymbolicExpressionTree", "Represents a symbolic expression tree")]
    public class SymbolicExpressionTree : Item, ISymbolicExpressionTree
    {
        private ISymbolicExpressionTreeNode? root;

        public ISymbolicExpressionTreeNode Root
        {
            get => root ?? throw new InvalidOperationException("Root node is not set");
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                
                if (value != root)
                {
                    root = value;
                    OnPropertyChanged(nameof(Root));
                }
            }
        }

        public int Length
        {
            get
            {
                if (root == null)
                    return 0;
                return root.GetLength();
            }
        }

        public int Depth
        {
            get
            {
                if (root == null)
                    return 0;
                return root.GetDepth();
            }
        }

        public SymbolicExpressionTree() : base() { }

        public SymbolicExpressionTree(ISymbolicExpressionTreeNode root) : base()
        {
            Root = root;
        }

        protected SymbolicExpressionTree(SymbolicExpressionTree original, Cloner cloner) : base(original, cloner)
        {
            if (original.root != null)
                root = cloner.Clone(original.root);
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesBreadth();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadthFirst()
        {
            return IterateNodesBreadth();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesPrefix();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesPostfix();
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new SymbolicExpressionTree(this, cloner);
        }

        public override string ToString()
        {
            if (root == null)
                return "Empty Tree";
            
            return $"Tree (Length: {Length}, Depth: {Depth})";
        }
    }
}
