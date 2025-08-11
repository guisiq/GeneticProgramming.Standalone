using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Mutates a tree by replacing a randomly selected subtree with a newly generated one
    /// </summary>
    public class SubtreeMutator<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeMutator<T> where T : notnull
    {
        private int _maxTreeLength = 25;
        private int _maxTreeDepth = 10;

        /// <summary>
        /// Gets or sets the maximum length for newly generated subtrees
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
        /// Gets or sets the maximum depth for newly generated subtrees
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

        /// <summary>
        /// Initializes a new instance of the SubtreeMutator class
        /// </summary>
        public SubtreeMutator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SubtreeMutator class
        /// </summary>
        /// <param name="original">The original mutator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected SubtreeMutator(SubtreeMutator<T> original, Cloner cloner) : base(original, cloner)
        {
            _maxTreeLength = original._maxTreeLength;
            _maxTreeDepth = original._maxTreeDepth;
        }

        /// <summary>
        /// Creates a deep clone of this mutator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this mutator</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new SubtreeMutator<T>(this, cloner);
        }

        /// <summary>
        /// Mutates a tree by replacing a random subtree
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="symbolicExpressionTree">Tree to mutate</param>
        /// <returns>Mutated tree</returns>
        public ISymbolicExpressionTree<T> Mutate(IRandom random, ISymbolicExpressionTree<T> symbolicExpressionTree)
        {
            if (symbolicExpressionTree?.Root == null)
            {
                throw new ArgumentException("Tree must have a valid root node");
            }

            if (SymbolicExpressionTreeGrammar == null)
            {
                throw new InvalidOperationException("Grammar must be set before mutation");
            }

            // Clone the tree
            var result = (ISymbolicExpressionTree<T>)symbolicExpressionTree.Clone(new Cloner());

            // Get all nodes
            var nodes = result.IterateNodesPostfix().ToList();
            if (!nodes.Any())
            {
                return result;
            }

            // Select a random node to replace
            var nodeToReplace = nodes[random.Next(nodes.Count)];

            // Create a new subtree using the grow method
            var creator = new GrowTreeCreator<T>();
            creator.SymbolicExpressionTreeGrammar = SymbolicExpressionTreeGrammar;
            var newSubtree = creator.CreateTree(random, SymbolicExpressionTreeGrammar, _maxTreeLength, _maxTreeDepth);

            // Replace the selected node
            if (nodeToReplace == result.Root)
            {
                // Replace the entire tree
                result.Root = newSubtree.Root;
            }
            else
            {
                // Replace a subtree
                var parent = nodeToReplace.Parent;
                if (parent != null)
                {
                    var childIndex = parent.IndexOfSubtree(nodeToReplace);
                    if (childIndex >= 0 && childIndex < parent.SubtreeCount)
                    {
                        parent.RemoveSubtree(childIndex);
                        parent.InsertSubtree(childIndex, newSubtree.Root);
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Mutates a tree by changing the symbol of a randomly selected node
    /// </summary>
    public class ChangeNodeTypeMutator<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeMutator<T> where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the ChangeNodeTypeMutator class
        /// </summary>
        public ChangeNodeTypeMutator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChangeNodeTypeMutator class
        /// </summary>
        /// <param name="original">The original mutator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected ChangeNodeTypeMutator(ChangeNodeTypeMutator<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        /// <summary>
        /// Creates a deep clone of this mutator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this mutator</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new ChangeNodeTypeMutator<T>(this, cloner);
        }

        /// <summary>
        /// Mutates a tree by changing the symbol of a random node
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="symbolicExpressionTree">Tree to mutate</param>
        /// <returns>Mutated tree</returns>
        public ISymbolicExpressionTree<T> Mutate(IRandom random, ISymbolicExpressionTree<T> symbolicExpressionTree)
        {
            if (symbolicExpressionTree?.Root == null)
            {
                throw new ArgumentException("Tree must have a valid root node");
            }

            if (SymbolicExpressionTreeGrammar == null)
            {
                throw new InvalidOperationException("Grammar must be set before mutation");
            }

            // Clone the tree
            var result = (ISymbolicExpressionTree<T>)symbolicExpressionTree.Clone(new Cloner());

            // Get all nodes
            var nodes = result.IterateNodesPostfix().ToList();
            if (!nodes.Any())
            {
                return result;
            }

            // Try to find a node that can be mutated by trying multiple random selections
            var maxAttempts = Math.Min(nodes.Count, 10); // Try up to 10 nodes or all nodes if fewer
            var nodesToTry = nodes.OrderBy(x => random.Next()).Take(maxAttempts).ToList();
            
            foreach (var nodeToChange in nodesToTry)
            {
                var currentArity = nodeToChange.SubtreeCount;

                // Find compatible symbols (same arity)
                var compatibleSymbols = SymbolicExpressionTreeGrammar.Symbols
                    .Where(s => s.Enabled && 
                               s.MinimumArity <= currentArity && 
                               s.MaximumArity >= currentArity &&
                               s.GetType() != nodeToChange.Symbol.GetType()) // Don't select the same symbol type
                    .ToList();

                if (compatibleSymbols.Any())
                {
                    // Found compatible symbols, perform the mutation
                    var newSymbol = compatibleSymbols[random.Next(compatibleSymbols.Count)];
                    var newNode = newSymbol.CreateTreeNode();

                    // Transfer children from old node to new node
                    var children = nodeToChange.Subtrees.ToList();
                    foreach (var child in children)
                    {
                        newNode.AddSubtree(child);
                    }

                    // Replace the node
                    if (nodeToChange == result.Root)
                    {
                        result.Root = newNode;
                    }
                    else
                    {
                        var parent = nodeToChange.Parent;
                        if (parent != null)
                        {
                            var childIndex = parent.IndexOfSubtree(nodeToChange);
                            if (childIndex >= 0)
                            {
                                parent.RemoveSubtree(childIndex);
                                parent.InsertSubtree(childIndex, newNode);
                            }
                        }
                    }

                    return result; // Mutation successful, return the mutated tree
                }
            }

            // If no compatible symbols found for any node, return the original tree
            return result;
        }
    }

    /// <summary>
    /// Mutates terminal nodes by changing their values
    /// </summary>
    public class ChangeTerminalMutator<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeMutator<T> where T : notnull
    {
        private double _constantMutationRange = 1.0;

        /// <summary>
        /// Gets or sets the range for constant mutations
        /// </summary>
        public double ConstantMutationRange
        {
            get => _constantMutationRange;
            set
            {
                if (_constantMutationRange != value)
                {
                    _constantMutationRange = Math.Max(0.0, value);
                    OnPropertyChanged(nameof(ConstantMutationRange));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ChangeTerminalMutator class
        /// </summary>
        public ChangeTerminalMutator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChangeTerminalMutator class
        /// </summary>
        /// <param name="original">The original mutator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected ChangeTerminalMutator(ChangeTerminalMutator<T> original, Cloner cloner) : base(original, cloner)
        {
            _constantMutationRange = original._constantMutationRange;
        }

        /// <summary>
        /// Creates a deep clone of this mutator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this mutator</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new ChangeTerminalMutator<T>(this, cloner);
        }

        /// <summary>
        /// Mutates terminal nodes by changing their values
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="symbolicExpressionTree">Tree to mutate</param>
        /// <returns>Mutated tree</returns>
        public ISymbolicExpressionTree<T> Mutate(IRandom random, ISymbolicExpressionTree<T> symbolicExpressionTree)
        {
            if (symbolicExpressionTree?.Root == null)
            {
                throw new ArgumentException("Tree must have a valid root node");
            }

            // Clone the tree
            var result = (ISymbolicExpressionTree<T>)symbolicExpressionTree.Clone(new Cloner());

            // Find terminal nodes
            var terminalNodes = result.IterateNodesPostfix()
                .Where(n => n.SubtreeCount == 0)
                .ToList();

            if (!terminalNodes.Any())
            {
                return result; // No terminal nodes to mutate
            }

            // Select a random terminal
            var terminalToMutate = terminalNodes[random.Next(terminalNodes.Count)];

            // Mutate based on terminal type
            if (terminalToMutate is ConstantTreeNode<T> constantNode)
            {
                if (MutateConstantValue != null)
                {
                    var currentValue = constantNode.Value;
                    constantNode.Value = MutateConstantValue(currentValue, _constantMutationRange);
                }
                else
                {
                    throw new InvalidOperationException("MutateConstantValue delegate must be set for constant mutation.");
                }
            }
            else if (terminalToMutate is VariableTreeNode<T> variableNode && SymbolicExpressionTreeGrammar != null)
            {
                var variableSymbols = SymbolicExpressionTreeGrammar.Symbols
                    .OfType<Variable<T>>()
                    .Where(v => v.Enabled)
                    .ToList();

                if (variableSymbols.Any())
                {
                    var newVariable = variableSymbols[random.Next(variableSymbols.Count)];
                    // Just change to a different variable name - for simplicity, use a random name
                    var variableNames = new[] { "X0", "X1", "X2", "X3", "X4" };
                    var currentName = variableNode.VariableName;
                    var availableNames = variableNames.Where(name => name != currentName).ToList();
                    if (availableNames.Any())
                    {
                        variableNode.VariableName = availableNames[random.Next(availableNames.Count)];
                    }
                }
            }

            return result;
        }

        private static readonly Dictionary<Type, Func<object, double, object>> DefaultMutateFunctions = new()
        {
            { typeof(int), (value, range) => (int)value + (int)(range * 10) },
            { typeof(double), (value, range) => (double)value + range },
            { typeof(float), (value, range) => (float)value + (float)range },
            { typeof(long), (value, range) => (long)value + (long)(range * 10) }
        };

        public Func<T, double, T>? MutateConstantValue { get; set; } = (currentValue, mutationRange) =>
        {
            if (DefaultMutateFunctions.TryGetValue(typeof(T), out var mutateFunc))
            {
                return (T)mutateFunc(currentValue, mutationRange);
            }
            throw new InvalidOperationException($"No default mutation function defined for type {typeof(T)}");
        };
    }
}
