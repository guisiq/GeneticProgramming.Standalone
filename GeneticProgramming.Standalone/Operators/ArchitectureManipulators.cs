using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Inserts a new node into a symbolic expression tree by wrapping an existing subtree.
    /// </summary>
    public class NodeInsertionManipulator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeMutator
    {
        private int _maxTreeLength = 25;
        private int _maxTreeDepth = 10;

        /// <summary>
        /// Gets or sets the maximum length for newly generated subtrees.
        /// </summary>
        public int MaxTreeLength
        {
            get => _maxTreeLength;
            set
            {
                if (_maxTreeLength != value)
                {
                    _maxTreeLength = Math.Max(1, value);
                    OnPropertyChanged(nameof(MaxTreeLength));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum depth for newly generated subtrees.
        /// </summary>
        public int MaxTreeDepth
        {
            get => _maxTreeDepth;
            set
            {
                if (_maxTreeDepth != value)
                {
                    _maxTreeDepth = Math.Max(1, value);
                    OnPropertyChanged(nameof(MaxTreeDepth));
                }
            }
        }

        public NodeInsertionManipulator() : base() { }

        protected NodeInsertionManipulator(NodeInsertionManipulator original, Cloner cloner) : base(original, cloner)
        {
            _maxTreeLength = original._maxTreeLength;
            _maxTreeDepth = original._maxTreeDepth;
        }

        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new NodeInsertionManipulator(this, cloner);
        }

        public ISymbolicExpressionTree Mutate(IRandom random, ISymbolicExpressionTree tree)
        {
            if (tree?.Root == null)
                throw new ArgumentException("Tree must have a valid root node");
            if (SymbolicExpressionTreeGrammar == null)
                throw new InvalidOperationException("Grammar must be set before mutation");

            var result = (ISymbolicExpressionTree)tree.Clone(new Cloner());
            var nodes = result.IterateNodesPostfix().ToList();
            if (!nodes.Any()) return result;

            var target = nodes[random.Next(nodes.Count)];

            var functionSymbols = SymbolicExpressionTreeGrammar.Symbols
                .Where(s => s.Enabled && s.MaximumArity > 0)
                .ToList();
            if (!functionSymbols.Any()) return result;

            var chosen = functionSymbols[random.Next(functionSymbols.Count)];
            int arity = chosen.MinimumArity == chosen.MaximumArity
                ? chosen.MinimumArity
                : random.Next(chosen.MinimumArity, chosen.MaximumArity + 1);

            var newNode = chosen.CreateTreeNode();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = SymbolicExpressionTreeGrammar };

            ISymbolicExpressionTreeNode? parent = null;
            int index = -1;
            var replacingRoot = target == result.Root;
            if (!replacingRoot)
            {
                parent = target.Parent!;
                index = parent.IndexOfSubtree(target);
                parent.RemoveSubtree(index);
            }

            newNode.AddSubtree(target);
            for (int i = 1; i < arity; i++)
            {
                var subtree = creator.CreateTree(random, SymbolicExpressionTreeGrammar, _maxTreeLength, _maxTreeDepth);
                newNode.AddSubtree(subtree.Root);
            }

            if (replacingRoot)
            {
                result.Root = newNode;
                newNode.Parent = null;
            }
            else if (parent != null)
            {
                parent.InsertSubtree(index, newNode);
            }

            return result;
        }
    }

    /// <summary>
    /// Removes a node from the tree. Children of the removed node are discarded.
    /// </summary>
    public class NodeRemovalManipulator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeMutator
    {
        public NodeRemovalManipulator() : base() { }
        protected NodeRemovalManipulator(NodeRemovalManipulator original, Cloner cloner) : base(original, cloner) { }

        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new NodeRemovalManipulator(this, cloner);
        }

        public ISymbolicExpressionTree Mutate(IRandom random, ISymbolicExpressionTree tree)
        {
            if (tree?.Root == null)
                throw new ArgumentException("Tree must have a valid root node");

            var result = (ISymbolicExpressionTree)tree.Clone(new Cloner());
            var nodes = result.IterateNodesPostfix().Where(n => n != result.Root).ToList();
            if (!nodes.Any()) return result;

            var nodeToRemove = nodes[random.Next(nodes.Count)];
            var parent = nodeToRemove.Parent!;
            var index = parent.IndexOfSubtree(nodeToRemove);
            if (index >= 0)
                parent.RemoveSubtree(index);

            return result;
        }
    }
}
