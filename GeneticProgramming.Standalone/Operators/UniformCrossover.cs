using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Performs uniform crossover between two symbolic expression trees. Each node
    /// of the first parent has a chance to be replaced by a compatible node from
    /// the second parent.
    /// </summary>
    public class UniformCrossover<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeCrossover<T> where T : notnull
    {
        private double _swapProbability = 0.5;

        /// <summary>
        /// Gets or sets the probability of swapping a node with a donor node.
        /// </summary>
        public double SwapProbability
        {
            get => _swapProbability;
            set
            {
                if (_swapProbability != value)
                {
                    _swapProbability = Math.Max(0.0, Math.Min(1.0, value));
                    OnPropertyChanged(nameof(SwapProbability));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the UniformCrossover class.
        /// </summary>
        public UniformCrossover() : base()
        {
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        protected UniformCrossover(UniformCrossover<T> original, Cloner cloner) : base(original, cloner)
        {
            _swapProbability = original._swapProbability;
        }

        /// <inheritdoc/>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new UniformCrossover<T>(this, cloner);
        }

        /// <summary>
        /// Performs uniform crossover between two parent trees.
        /// </summary>
        public ISymbolicExpressionTree<T> Crossover(IRandom random, ISymbolicExpressionTree<T> parent0, ISymbolicExpressionTree<T> parent1)
        {
            if (parent0?.Root == null || parent1?.Root == null)
                throw new ArgumentException("Parent trees must have valid root nodes");
            if (SymbolicExpressionTreeGrammar == null)
                throw new InvalidOperationException("Grammar must be set before crossover");

            var offspring = (ISymbolicExpressionTree<T>)parent0.Clone(new Cloner());
            var donorNodes = parent1.IterateNodesPostfix().ToList();
            var nodes = offspring.IterateNodesPostfix().ToList();

            foreach (var node in nodes)
            {
                if (random.NextDouble() >= _swapProbability)
                    continue;

                if (node.Parent == null)
                {
                    var donor = SelectCompatibleRoot(random, donorNodes);
                    if (donor == null) continue;
                    offspring.Root = (ISymbolicExpressionTreeNode<T>)donor.Clone(new Cloner());
                }
                else
                {
                    var parent = node.Parent;
                    if (parent == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Node parent is null during crossover iteration");
                        continue;
                    }
                    
                    int index = parent.IndexOfSubtree(node);
                    if (index == -1 || index >= parent.SubtreeCount)
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid index {index} for node in parent with {parent.SubtreeCount} subtrees");
                        continue;
                    }
                    
                    // Double-check: verify the node at the found index is actually our node
                    if (parent.GetSubtree(index) != node)
                    {
                        System.Diagnostics.Debug.WriteLine($"Node mismatch at index {index}: expected {node}, found {parent.GetSubtree(index)}");
                        continue;
                    }
                    
                    var donor = SelectCompatibleDonor(random, donorNodes, parent.Symbol, index);
                    if (donor == null) continue;
                    
                    parent.RemoveSubtree(index);
                    parent.InsertSubtree(index, (ISymbolicExpressionTreeNode<T>)donor.Clone(new Cloner()));
                }
            }
            return offspring;
        }

        private ISymbolicExpressionTreeNode<T>? SelectCompatibleRoot(IRandom random, IList<ISymbolicExpressionTreeNode<T>> donors)
        {
            var grammar = SymbolicExpressionTreeGrammar;
            if (grammar == null || donors.Count == 0) return null;
            var compatible = donors.Where(d => grammar.StartSymbols.Contains(d.Symbol)).ToList();
            if (!compatible.Any()) return null;
            return compatible[random.Next(compatible.Count)];
        }

        private ISymbolicExpressionTreeNode<T>? SelectCompatibleDonor(IRandom random, IList<ISymbolicExpressionTreeNode<T>> donors, ISymbol parentSymbol, int index)
        {
            var grammar = SymbolicExpressionTreeGrammar;
            if (grammar == null || donors.Count == 0) return null;
            var compatible = donors.Where(d => grammar.IsAllowedChildSymbol(parentSymbol, d.Symbol, index)).ToList();
            if (!compatible.Any()) return null;
            return compatible[random.Next(compatible.Count)];
        }
    }
}
