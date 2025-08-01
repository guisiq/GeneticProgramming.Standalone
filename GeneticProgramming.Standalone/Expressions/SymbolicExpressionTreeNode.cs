using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Represents a node in a symbolic expression tree that can have child nodes.
    /// This is the main implementation for non-terminal nodes in the tree.
    /// </summary>
    public class SymbolicExpressionTreeNode : Item, ISymbolicExpressionTreeNode
    {
        private IList<ISymbolicExpressionTreeNode>? subtrees;
        private ISymbol? symbol;

        // Cached values to prevent unnecessary tree iterations
        private ushort length;
        private ushort depth;

        public ISymbol Symbol
        {
            get { return symbol ?? throw new InvalidOperationException("Symbol not set"); }
            protected set { symbol = value; }
        }        private ISymbolicExpressionTreeNode? parent;
        public ISymbolicExpressionTreeNode? Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual bool HasLocalParameters
        {
            get { return false; }
        }

        public virtual IEnumerable<ISymbolicExpressionTreeNode> Subtrees
        {
            get { return subtrees ?? Enumerable.Empty<ISymbolicExpressionTreeNode>(); }
        }

        public virtual ISymbolicExpressionTreeGrammar? Grammar
        {
            get { return parent?.Grammar; }
        }

        public int SubtreeCount
        {
            get { return subtrees?.Count ?? 0; }
        }

        /// <summary>
        /// Protected default constructor for cloning
        /// </summary>
        protected SymbolicExpressionTreeNode() : base()
        {
            // Don't allocate subtrees list here!
            // Because we don't want to allocate it in terminal nodes
        }

        /// <summary>
        /// Creates a new node with the specified symbol
        /// </summary>
        /// <param name="symbol">The symbol this node represents</param>
        public SymbolicExpressionTreeNode(ISymbol symbol) : base()
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            subtrees = new List<ISymbolicExpressionTreeNode>(3);
            this.symbol = symbol;
        }

        /// <summary>
        /// Copy constructor for cloning
        /// </summary>
        /// <param name="original">Original node to clone</param>
        /// <param name="cloner">Cloner instance</param>
        protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original, Cloner cloner)
            : base(original, cloner)
        {
            symbol = original.symbol; // Symbols are reused
            length = original.length;
            depth = original.depth;
            
            if (original.subtrees != null)
            {
                subtrees = new List<ISymbolicExpressionTreeNode>(original.subtrees.Count);
                foreach (var subtree in original.subtrees)
                {
                    var clonedSubtree = cloner.Clone(subtree) as ISymbolicExpressionTreeNode;
                    if (clonedSubtree != null)
                    {
                        subtrees.Add(clonedSubtree);
                        clonedSubtree.Parent = this;
                    }
                }
            }
        }
        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new SymbolicExpressionTreeNode(this, cloner);
        }

        /// <summary>
        /// Initialize parent relationships after deserialization
        /// </summary>
        public void InitializeParentRelationships()
        {
            // Ensure parent relationships are correct after deserialization
            if (subtrees != null)
            {
                foreach (var subtree in subtrees)
                {
                    if (subtree.Parent == null)
                        subtree.Parent = this;
                }
            }
        }

        #region Tree Structure Operations

        public int GetLength()
        {
            if (length > 0) return length;
            
            ushort l = 1;
            if (subtrees != null)
            {
                for (int i = 0; i < subtrees.Count; i++)
                {
                    checked { l += (ushort)subtrees[i].GetLength(); }
                }
            }
            length = l;
            return length;
        }

        public int GetDepth()
        {
            if (depth > 0) return depth;
            
            ushort d = 0;
            if (subtrees != null)
            {
                for (int i = 0; i < subtrees.Count; i++)
                {
                    d = Math.Max(d, (ushort)subtrees[i].GetDepth());
                }
            }
            d++;
            depth = d;
            return depth;
        }

        public int GetBranchLevel(ISymbolicExpressionTreeNode child)
        {
            return GetBranchLevel(this, child);
        }

        private static int GetBranchLevel(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode point)
        {
            if (root == point)
                return 0;
            
            foreach (var subtree in root.Subtrees)
            {
                int branchLevel = GetBranchLevel(subtree, point);
                if (branchLevel < int.MaxValue)
                    return 1 + branchLevel;
            }
            return int.MaxValue;
        }

        #endregion

        #region Subtree Management

        public virtual ISymbolicExpressionTreeNode GetSubtree(int index)
        {
            if (subtrees == null)
                throw new ArgumentOutOfRangeException(nameof(index), "Node has no subtrees");
            return subtrees[index];
        }

        public virtual int IndexOfSubtree(ISymbolicExpressionTreeNode tree)
        {
            if (subtrees == null)
                return -1;
            return subtrees.IndexOf(tree);
        }

        public virtual void AddSubtree(ISymbolicExpressionTreeNode tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            
            if (subtrees == null)
                subtrees = new List<ISymbolicExpressionTreeNode>(3);
            
            subtrees.Add(tree);
            tree.Parent = this;
            ResetCachedValues();
        }

        public virtual void InsertSubtree(int index, ISymbolicExpressionTreeNode tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            
            if (subtrees == null)
                subtrees = new List<ISymbolicExpressionTreeNode>(3);
            
            subtrees.Insert(index, tree);
            tree.Parent = this;
            ResetCachedValues();
        }

        public virtual void RemoveSubtree(int index)
        {
            if (subtrees == null || index < 0 || index >= subtrees.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            subtrees[index].Parent = null;
            subtrees.RemoveAt(index);
            ResetCachedValues();
        }

        public virtual void ReplaceSubtree(int index, ISymbolicExpressionTreeNode tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            if (subtrees == null || index < 0 || index >= subtrees.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            subtrees[index].Parent = null;
            subtrees[index] = tree;
            tree.Parent = this;
            ResetCachedValues();
        }

        public virtual void ReplaceSubtree(ISymbolicExpressionTreeNode old, ISymbolicExpressionTreeNode tree)
        {
            if (old == null)
                throw new ArgumentNullException(nameof(old));
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            
            var index = IndexOfSubtree(old);
            if (index == -1)
                throw new ArgumentException("Old subtree not found", nameof(old));
            
            ReplaceSubtree(index, tree);
        }

        #endregion

        #region Tree Iteration

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth()
        {
            var list = new List<ISymbolicExpressionTreeNode>(GetLength()) { this };
            int i = 0;
            while (i != list.Count)
            {
                for (int j = 0; j != list[i].SubtreeCount; ++j)
                    list.Add(list[i].GetSubtree(j));
                ++i;
            }
            return list;
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix()
        {
            var list = new List<ISymbolicExpressionTreeNode>();
            ForEachNodePrefix(n => list.Add(n));
            return list;
        }

        public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            action(this);
            if (subtrees != null)
            {
                // Avoid LINQ to reduce memory pressure
                for (int i = 0; i < subtrees.Count; i++)
                {
                    subtrees[i].ForEachNodePrefix(action);
                }
            }
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix()
        {
            var list = new List<ISymbolicExpressionTreeNode>();
            ForEachNodePostfix(n => list.Add(n));
            return list;
        }

        public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            if (subtrees != null)
            {
                // Avoid LINQ to reduce memory pressure
                for (int i = 0; i < subtrees.Count; i++)
                {
                    subtrees[i].ForEachNodePostfix(action);
                }
            }
            action(this);
        }

        #endregion

        #region Parameter Operations

        public virtual void ResetLocalParameters(IRandom random)
        {
            // Default implementation does nothing
            // Override in derived classes that have parameters
        }

        public virtual void ShakeLocalParameters(IRandom random, double shakingFactor)
        {
            // Default implementation does nothing
            // Override in derived classes that have parameters
        }

        #endregion

        public override string ToString()
        {
            return Symbol?.Name ?? "SymbolicExpressionTreeNode";
        }

        private void ResetCachedValues()
        {
            length = 0;
            depth = 0;
            
            if (parent is SymbolicExpressionTreeNode parentNode)
                parentNode.ResetCachedValues();
        }
    }

    /// <summary>
    /// Generic implementation of a symbolic expression tree node that can have child nodes.
    /// This is the main implementation for non-terminal nodes in the tree with type safety.
    /// </summary>
    /// <typeparam name="T">The value type that the node evaluates to (must be a struct)</typeparam>
    public class SymbolicExpressionTreeNode<T> : SymbolicExpressionTreeNode, ISymbolicExpressionTreeNode<T> where T : struct
    {
        private IList<ISymbolicExpressionTreeNode<T>>? genericSubtrees;
        private ISymbol<T> genericSymbol;

        /// <summary>
        /// Gets the generic symbol this node represents
        /// </summary>
        public new ISymbol<T> Symbol
        {
            get { return genericSymbol; }
            protected set { genericSymbol = value; }
        }

        /// <summary>
        /// Gets the output type of this node
        /// </summary>
        public Type OutputType => typeof(T);

        /// <summary>
        /// Gets the parent node as a generic type
        /// </summary>
        public new ISymbolicExpressionTreeNode<T>? Parent
        {
            get { return base.Parent as ISymbolicExpressionTreeNode<T>; }
            set { base.Parent = value; }
        }

        /// <summary>
        /// Gets the grammar as a generic type
        /// </summary>
        public new ISymbolicExpressionTreeGrammar<T>? Grammar
        {
            get { return Parent?.Grammar; }
        }

        /// <summary>
        /// Gets the generic subtrees
        /// </summary>
        public new IEnumerable<ISymbolicExpressionTreeNode<T>> Subtrees
        {
            get { return genericSubtrees ?? Enumerable.Empty<ISymbolicExpressionTreeNode<T>>(); }
        }

        /// <summary>
        /// Creates a new generic node with the specified symbol
        /// </summary>
        /// <param name="symbol">The generic symbol this node represents</param>
        public SymbolicExpressionTreeNode(ISymbol<T> symbol) : base(symbol)
        {
            genericSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            genericSubtrees = new List<ISymbolicExpressionTreeNode<T>>(3);
        }

        /// <summary>
        /// Copy constructor for cloning
        /// </summary>
        /// <param name="original">Original node to clone</param>
        /// <param name="cloner">Cloner instance</param>
        protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode<T> original, Cloner cloner)
            : base(original, cloner)
        {
            genericSymbol = original.genericSymbol;
            
            if (original.genericSubtrees != null)
            {
                genericSubtrees = new List<ISymbolicExpressionTreeNode<T>>(original.genericSubtrees.Count);
                foreach (var subtree in original.genericSubtrees)
                {
                    var clonedSubtree = (ISymbolicExpressionTreeNode<T>)cloner.Clone(subtree);
                    genericSubtrees.Add(clonedSubtree);
                    clonedSubtree.Parent = this;
                }
            }
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new SymbolicExpressionTreeNode<T>(this, cloner);
        }

        /// <summary>
        /// Gets a generic subtree at the specified index
        /// </summary>
        public new ISymbolicExpressionTreeNode<T> GetSubtree(int index)
        {
            if (genericSubtrees == null)
                throw new ArgumentOutOfRangeException(nameof(index));
            return genericSubtrees[index];
        }

        /// <summary>
        /// Gets the index of a generic subtree
        /// </summary>
        public int IndexOfSubtree(ISymbolicExpressionTreeNode<T> tree)
        {
            return genericSubtrees?.IndexOf(tree) ?? -1;
        }

        /// <summary>
        /// Adds a generic subtree with type validation
        /// </summary>
        public void AddSubtree(ISymbolicExpressionTreeNode<T> tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (!IsCompatibleChild(tree))
                throw new ArgumentException($"Child node output type {tree.OutputType} is not compatible with expected input type at position {SubtreeCount}");

            genericSubtrees ??= new List<ISymbolicExpressionTreeNode<T>>(3);
            genericSubtrees.Add(tree);
            tree.Parent = this;

            // Also add to base subtrees for compatibility
            base.AddSubtree(tree);
        }

        /// <summary>
        /// Inserts a generic subtree at the specified index with type validation
        /// </summary>
        public void InsertSubtree(int index, ISymbolicExpressionTreeNode<T> tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (!IsCompatibleChild(tree))
                throw new ArgumentException($"Child node output type {tree.OutputType} is not compatible with expected input type at position {index}");

            genericSubtrees ??= new List<ISymbolicExpressionTreeNode<T>>(3);
            genericSubtrees.Insert(index, tree);
            tree.Parent = this;

            // Also insert to base subtrees for compatibility
            base.InsertSubtree(index, tree);
        }

        /// <summary>
        /// Replaces a generic subtree at the specified index with type validation
        /// </summary>
        public void ReplaceSubtree(int index, ISymbolicExpressionTreeNode<T> tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (!IsCompatibleChild(tree))
                throw new ArgumentException($"Child node output type {tree.OutputType} is not compatible with expected input type at position {index}");

            if (genericSubtrees == null)
                throw new ArgumentOutOfRangeException(nameof(index));

            var oldTree = genericSubtrees[index];
            oldTree.Parent = null;
            
            genericSubtrees[index] = tree;
            tree.Parent = this;

            // Also replace in base subtrees for compatibility
            base.ReplaceSubtree(index, tree);
        }

        /// <summary>
        /// Replaces a generic subtree with another with type validation
        /// </summary>
        public void ReplaceSubtree(ISymbolicExpressionTreeNode<T> original, ISymbolicExpressionTreeNode<T> replacement)
        {
            int index = IndexOfSubtree(original);
            if (index >= 0)
                ReplaceSubtree(index, replacement);
        }

        /// <summary>
        /// Validates type compatibility when adding a subtree
        /// </summary>
        /// <param name="child">The child node to validate</param>
        /// <returns>True if the child is compatible, false otherwise</returns>
        public bool IsCompatibleChild(ISymbolicExpressionTreeNode<T> child)
        {
            if (child == null || genericSymbol == null)
                return false;

            int currentChildCount = SubtreeCount;
            return genericSymbol.IsCompatibleChildType(child.OutputType, currentChildCount);
        }

        /// <summary>
        /// Generic iteration methods
        /// </summary>
        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesBreadth()
        {
            var queue = new Queue<ISymbolicExpressionTreeNode<T>>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Subtrees)
                    queue.Enqueue(child);
            }
        }

        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix()
        {
            foreach (var child in Subtrees)
                foreach (var node in child.IterateNodesPostfix())
                    yield return node;

            yield return this;
        }

        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPrefix()
        {
            yield return this;

            foreach (var child in Subtrees)
                foreach (var node in child.IterateNodesPrefix())
                    yield return node;
        }

        public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode<T>> action)
        {
            foreach (var child in Subtrees)
                child.ForEachNodePostfix(action);

            action(this);
        }

        public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode<T>> action)
        {
            action(this);

            foreach (var child in Subtrees)
                child.ForEachNodePrefix(action);
        }
    }
}