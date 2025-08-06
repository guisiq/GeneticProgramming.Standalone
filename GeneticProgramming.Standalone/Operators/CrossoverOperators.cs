using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Performs subtree crossover between two symbolic expression trees
    /// </summary>
    public class SubtreeCrossover<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeCrossover<T> where T : struct
    {
        private double _internalNodeProbability = 0.9;

        /// <summary>
        /// Gets or sets the probability of selecting an internal node for crossover
        /// </summary>
        public double InternalNodeProbability
        {
            get => _internalNodeProbability;
            set
            {
                if (_internalNodeProbability != value)
                {
                    _internalNodeProbability = Math.Max(0.0, Math.Min(1.0, value));
                    OnPropertyChanged(nameof(InternalNodeProbability));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SubtreeCrossover class
        /// </summary>
        public SubtreeCrossover() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SubtreeCrossover class
        /// </summary>
        /// <param name="original">The original crossover operator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected SubtreeCrossover(SubtreeCrossover<T> original, Cloner cloner) : base(original, cloner)
        {
            _internalNodeProbability = original._internalNodeProbability;
        }

        // Removido o override de Clone(Cloner cloner) pois a lógica base em Item.cs será usada.
        // O método Clone de Item chamará CreateCloneInstance.

        /// <summary>
        /// Creates a new instance of the SubtreeCrossover for cloning.
        /// </summary>
        /// <param name="cloner">The cloner to use for the cloning process.</param>
        /// <returns>A new instance of SubtreeCrossover.</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new SubtreeCrossover<T>(this, cloner);
        }

        /// <summary>
        /// Performs subtree crossover between two parent trees
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="parent0">First parent tree</param>
        /// <param name="parent1">Second parent tree</param>
        /// <returns>Offspring tree created by crossover</returns>

        public  ISymbolicExpressionTree<T> Crossover(IRandom random, ISymbolicExpressionTree<T> parent0, ISymbolicExpressionTree<T> parent1)
        {
            if (parent0?.Root == null || parent1?.Root == null)
            {
                throw new ArgumentException("Parent trees must have valid root nodes");
            }

            // Clone the first parent to create offspring
            var offspring = (ISymbolicExpressionTree<T>)parent0.Clone(new Cloner());

            // Get all nodes from both trees
            var offspringNodes = offspring.IterateNodesPostfix().ToList();
            var donorNodes = parent1.IterateNodesPostfix().ToList();

            if (!offspringNodes.Any() || !donorNodes.Any())
            {
                return offspring;
            }

            // Select crossover point in offspring tree
            var crossoverPoint = SelectCrossoverPoint(random, offspringNodes);
            
            // Select compatible donor subtree
            var donorSubtree = SelectCompatibleDonor(random, donorNodes, crossoverPoint);
            if (donorSubtree == null)
            {
                return offspring; // No compatible donor found
            }

            // Perform the crossover
            var clonedDonorSubtree = (ISymbolicExpressionTreeNode<T>)donorSubtree.Clone(new Cloner());
            
            if (crossoverPoint == offspring.Root)
            {
                // Replace root
                offspring.Root = clonedDonorSubtree;
            }
            else
            {
                // Replace subtree
                var parent = crossoverPoint.Parent;
                if (parent != null)
                {
                    var childIndex = parent.IndexOfSubtree(crossoverPoint);
                    if (childIndex >= 0 && childIndex < parent.SubtreeCount)
                    {
                        parent.RemoveSubtree(childIndex);
                        parent.InsertSubtree(childIndex, clonedDonorSubtree);
                    }
                }
            }

            return offspring;
        }

        private ISymbolicExpressionTreeNode<T> SelectCrossoverPoint(IRandom random, IList<ISymbolicExpressionTreeNode<T>> nodes)
        {
            if (random.NextDouble() < _internalNodeProbability)
            {
                // Try to select an internal node
                var internalNodes = nodes.Where(n => n.SubtreeCount > 0).ToList();
                if (internalNodes.Any())
                {
                    return internalNodes[random.Next(internalNodes.Count)];
                }
            }

            // Select any node (fallback or terminal selection)
            return nodes[random.Next(nodes.Count)];
        }

        private ISymbolicExpressionTreeNode<T>? SelectCompatibleDonor(IRandom random, IList<ISymbolicExpressionTreeNode<T>> donorNodes, ISymbolicExpressionTreeNode<T> crossoverPoint)
        {
            // For basic implementation, any node is compatible
            // In more advanced implementations, this would check type constraints
            if (!donorNodes.Any())
            {
                return null;
            }

            return donorNodes[random.Next(donorNodes.Count)];
        }
    }

    /// <summary>
    /// Performs one-point crossover at the same depth level
    /// </summary>
    public class OnePointCrossover<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeCrossover<T> where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the OnePointCrossover class
        /// </summary>
        public OnePointCrossover() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the OnePointCrossover class
        /// </summary>
        /// <param name="original">The original crossover operator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected OnePointCrossover(OnePointCrossover<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        // Removido o override de Clone(Cloner cloner) pois a lógica base em Item.cs será usada.
        // O método Clone de Item chamará CreateCloneInstance.

        /// <summary>
        /// Creates a new instance of the OnePointCrossover for cloning.
        /// </summary>
        /// <param name="cloner">The cloner to use for the cloning process.</param>
        /// <returns>A new instance of OnePointCrossover.</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new OnePointCrossover<T>(this, cloner);
        }

        /// <summary>
        /// Performs one-point crossover between two parent trees
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="parent0">First parent tree</param>
        /// <param name="parent1">Second parent tree</param>
        /// <returns>Offspring tree created by crossover</returns>
        public ISymbolicExpressionTree<T> Crossover(IRandom random, ISymbolicExpressionTree<T> parent0, ISymbolicExpressionTree<T> parent1)
        {
            if (parent0?.Root == null || parent1?.Root == null)
            {
                throw new ArgumentException("Parent trees must have valid root nodes");
            }

            // Clone the first parent
            var offspring = (ISymbolicExpressionTree<T>)parent0.Clone(new Cloner());

            // Get the maximum common depth
            var depth0 = offspring.Root.GetDepth();
            var depth1 = parent1.Root.GetDepth();
            var maxCommonDepth = Math.Min(depth0, depth1);

            if (maxCommonDepth <= 1)
            {
                return offspring; // Cannot perform meaningful crossover
            }

            // Select a random depth level for crossover
            var crossoverDepth = random.Next(1, maxCommonDepth);

            // Get nodes at the selected depth from both trees
            var offspringNodesAtDepth = GetNodesAtDepth(offspring.Root, crossoverDepth).ToList();
            var donorNodesAtDepth = GetNodesAtDepth(parent1.Root, crossoverDepth).ToList();

            if (!offspringNodesAtDepth.Any() || !donorNodesAtDepth.Any())
            {
                return offspring;
            }

            // Select random nodes at the crossover depth
            var offspringNode = offspringNodesAtDepth[random.Next(offspringNodesAtDepth.Count)];
            var donorNode = donorNodesAtDepth[random.Next(donorNodesAtDepth.Count)];

            // Clone the donor subtree
            var clonedDonorSubtree = (ISymbolicExpressionTreeNode<T>)donorNode.Clone(new Cloner());

            // Replace the subtree
            var parent = offspringNode.Parent;
            if (parent != null)
            {
                var childIndex = parent.IndexOfSubtree(offspringNode);
                if (childIndex >= 0)
                {
                    parent.RemoveSubtree(childIndex);
                    parent.InsertSubtree(childIndex, clonedDonorSubtree);
                }
            }
            else
            {
                // Replace root
                offspring.Root = clonedDonorSubtree;
            }

            return offspring;
        }

        private IEnumerable<ISymbolicExpressionTreeNode<T>> GetNodesAtDepth(ISymbolicExpressionTreeNode<T> root, int targetDepth)
        {
            if (targetDepth == 0)
            {
                yield return root;
                yield break;
            }

            var queue = new Queue<(ISymbolicExpressionTreeNode<T> node, int depth)>();
            queue.Enqueue((root, 0));

            while (queue.Count > 0)
            {
                var (node, depth) = queue.Dequeue();

                if (depth == targetDepth)
                {
                    yield return node;
                }
                else if (depth < targetDepth)
                {
                    foreach (var child in node.Subtrees)
                    {
                        queue.Enqueue((child, depth + 1));
                    }
                }
            }
        }
    }
}
