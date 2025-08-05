using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions.Exceptions;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Strategy for handling arity violations when adding subtrees
    /// </summary>
    public enum ArityViolationStrategy
    {
        /// <summary>
        /// Throw exception immediately (default behavior)
        /// </summary>
        ThrowException,
        
        /// <summary>
        /// Replace one of the existing children with the new subtree
        /// </summary>
        ReplaceExistingChild,
        
        /// <summary>
        /// Skip adding the subtree and continue
        /// </summary>
        SkipAndContinue,
        
        /// <summary>
        /// Replace current node with a terminal that fits
        /// </summary>
        ReplaceWithTerminal
    }
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
        
        /// <summary>
        /// Strategy to use when arity violations occur
        /// </summary>
        public ArityViolationStrategy ArityStrategy { get; set; } = ArityViolationStrategy.ReplaceExistingChild;

        public ISymbol Symbol
        {
            get { return symbol ?? throw new InvalidOperationException("Symbol not set"); }
            protected set { symbol = value; }
        }
        private ISymbolicExpressionTreeNode? parent;
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

        protected virtual void ValidateSubtreeArity(int currentCount, int operationDelta = 0)
        {
            var futureCount = currentCount + operationDelta;
            
            // Always validate maximum arity to prevent exceeding capacity
            if (futureCount > Symbol.MaximumArity)
                throw new MaximumArityExceededException(Symbol.SymbolName, currentCount, futureCount, Symbol.MaximumArity);
                
            // For minimum arity, only validate removal operations that would violate it
            // During construction (adding children), allow intermediate states below minimum arity
            if (operationDelta < 0 && futureCount < Symbol.MinimumArity)
                throw new MinimumArityViolatedException(Symbol.SymbolName, currentCount, futureCount, Symbol.MinimumArity);
        }

        /// <summary>
        /// Validates current arity without considering future operations
        /// </summary>
        protected virtual void ValidateCurrentArity()
        {
            int currentCount = SubtreeCount;
            if (currentCount < Symbol.MinimumArity || currentCount > Symbol.MaximumArity)
                throw new InvalidArityException(Symbol.SymbolName, currentCount, Symbol.MinimumArity, Symbol.MaximumArity);
        }

        /// <summary>
        /// Creates a replacement terminal node when arity violations occur
        /// </summary>
        protected virtual ISymbolicExpressionTreeNode CreateReplacementTerminal()
        {
            var terminalSymbols = Grammar?.Symbols.Where(s => s.MaximumArity == 0).ToList();
            if (terminalSymbols != null && terminalSymbols.Any())
            {
                var random = new Random();
                var randomTerminal = terminalSymbols[random.Next(terminalSymbols.Count)];
                return randomTerminal.CreateTreeNode();
            }

            throw new InvalidOperationException("No terminal symbols available for replacement.");
        }

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
            
            try
            {
                // Validate arity constraints
                ValidateSubtreeArity(subtrees.Count, 1);
                
                subtrees.Add(tree);
                tree.Parent = this;
                ResetCachedValues();
            }
            catch (MaximumArityExceededException ex)
            {
                HandleArityViolation(ex, tree);
            }
        }

        /// <summary>
        /// Handles arity violations based on the configured strategy
        /// </summary>
        protected virtual void HandleArityViolation(MaximumArityExceededException ex, ISymbolicExpressionTreeNode newTree)
        {
            switch (ArityStrategy)
            {
                case ArityViolationStrategy.ThrowException:
                    throw ex; // Re-throw the original exception
                    
                case ArityViolationStrategy.ReplaceExistingChild:
                    HandleReplaceExistingChild(newTree);
                    break;
                    
                case ArityViolationStrategy.SkipAndContinue:
                    HandleSkipAndContinue(ex);
                    break;
                    
                case ArityViolationStrategy.ReplaceWithTerminal:
                    HandleReplaceWithTerminal(ex, newTree);
                    break;
                    
                default:
                    throw ex; // Fallback to throwing
            }
        }

        /// <summary>
        /// Strategy: Replace one of the existing children with the new subtree
        /// </summary>
        protected virtual void HandleReplaceExistingChild(ISymbolicExpressionTreeNode newTree)
        {
            if (subtrees != null && subtrees.Count > 0)
            {
                // Choose a random child to replace
                var random = new Random();
                var indexToReplace = random.Next(0, subtrees.Count);
                
                // Remove old child
                subtrees[indexToReplace].Parent = null;
                
                // Replace with new tree
                subtrees[indexToReplace] = newTree;
                newTree.Parent = this;
                
                ResetCachedValues();
                
                System.Diagnostics.Debug.WriteLine($"Replaced child at index {indexToReplace} in {Symbol.SymbolName}");
            }
        }

        /// <summary>
        /// Strategy: Skip adding the subtree and continue
        /// </summary>
        protected virtual void HandleSkipAndContinue(MaximumArityExceededException ex)
        {
            // Log the issue but continue without adding this child
            System.Diagnostics.Debug.WriteLine($"Skipped adding child to {ex.SymbolName}: {ex.Message}");
            
            // Ensure we meet minimum arity by adding terminals if needed
            while (subtrees != null && subtrees.Count < Symbol.MinimumArity)
            {
                try
                {
                    var terminal = CreateReplacementTerminal();
                    subtrees.Add(terminal);
                    terminal.Parent = this;
                }
                catch (InvalidOperationException)
                {
                    break; // No terminals available
                }
            }
            
            ResetCachedValues();
        }

        /// <summary>
        /// Strategy: Replace current node with a terminal (this would need to be handled at parent level)
        /// </summary>
        protected virtual void HandleReplaceWithTerminal(MaximumArityExceededException ex, ISymbolicExpressionTreeNode newTree)
        {
            // This strategy would typically be handled at a higher level (tree creator)
            // For now, we'll log and fall back to skip
            System.Diagnostics.Debug.WriteLine($"ReplaceWithTerminal strategy requested for {ex.SymbolName}, falling back to skip");
            HandleSkipAndContinue(ex);
        }

        public virtual void InsertSubtree(int index, ISymbolicExpressionTreeNode tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));
            
            if (subtrees == null)
                subtrees = new List<ISymbolicExpressionTreeNode>(3);
            
            try
            {
                // Validate arity constraints
                ValidateSubtreeArity(subtrees.Count, 1);
                
                subtrees.Insert(index, tree);
                tree.Parent = this;
                ResetCachedValues();
            }
            catch (MaximumArityExceededException ex)
            {
                HandleArityViolation(ex, tree);
            }
        }

        public virtual void RemoveSubtree(int index)
        {
            if (subtrees == null || index < 0 || index >= subtrees.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            try
            {
                // Validate arity constraints
                ValidateSubtreeArity(subtrees.Count, -1);
                
                subtrees[index].Parent = null;
                subtrees.RemoveAt(index);
                ResetCachedValues();
            }
            catch (MinimumArityViolatedException ex)
            {
                HandleMinimumArityViolation(ex, index);
            }
        }

        /// <summary>
        /// Handles minimum arity violations (when trying to remove too many children)
        /// </summary>
        protected virtual void HandleMinimumArityViolation(MinimumArityViolatedException ex, int indexToRemove)
        {
            switch (ArityStrategy)
            {
                case ArityViolationStrategy.ThrowException:
                    throw ex;
                    
                case ArityViolationStrategy.SkipAndContinue:
                    System.Diagnostics.Debug.WriteLine($"Skipped removing child at index {indexToRemove} from {ex.SymbolName}: {ex.Message}");
                    break;
                    
                default:
                    // For removal operations, most strategies fall back to skipping
                    System.Diagnostics.Debug.WriteLine($"Cannot remove child at index {indexToRemove} from {ex.SymbolName}, would violate minimum arity");
                    break;
            }
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
        /// Strategy to use when arity violations occur (inherits from base class but can be overridden)
        /// </summary>
        public new ArityViolationStrategy ArityStrategy 
        { 
            get => base.ArityStrategy; 
            set => base.ArityStrategy = value; 
        }

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
                    var clonedSubtree = cloner.Clone(subtree) as ISymbolicExpressionTreeNode<T>;
                    if (clonedSubtree != null)
                    {
                        genericSubtrees.Add(clonedSubtree);
                        clonedSubtree.Parent = this;
                    }
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
        /// Adds a generic subtree with type validation and arity handling
        /// </summary>
        public void AddSubtree(ISymbolicExpressionTreeNode<T> tree)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (!IsCompatibleChild(tree))
                throw new ArgumentException($"Child node output type {tree.OutputType} is not compatible with expected input type at position {SubtreeCount}");

            genericSubtrees ??= new List<ISymbolicExpressionTreeNode<T>>(3);
            
            try
            {
                // Validate arity constraints against the current total subtree count
                ValidateSubtreeArity(SubtreeCount, 1);
                
                genericSubtrees.Add(tree);
                tree.Parent = this;

                // Also add to base subtrees for compatibility
                // Use the base implementation but wrap it to handle any nested exceptions
                try
                {
                    base.AddSubtree(tree);
                }
                catch (MaximumArityExceededException)
                {
                    // This should not happen since we already validated, but if it does,
                    // we need to undo the generic addition and re-throw our original exception
                    genericSubtrees.Remove(tree);
                    tree.Parent = null;
                    throw;
                }
            }
            catch (MaximumArityExceededException ex)
            {
                HandleGenericArityViolation(ex, tree);
            }
        }

        /// <summary>
        /// Handles arity violations for generic nodes
        /// </summary>
        protected virtual void HandleGenericArityViolation(MaximumArityExceededException ex, ISymbolicExpressionTreeNode<T> newTree)
        {
            switch (ArityStrategy)
            {
                case ArityViolationStrategy.ThrowException:
                    throw ex;
                    
                case ArityViolationStrategy.ReplaceExistingChild:
                    HandleGenericReplaceExistingChild(newTree);
                    break;
                    
                case ArityViolationStrategy.SkipAndContinue:
                    HandleGenericSkipAndContinue(ex);
                    break;
                    
                case ArityViolationStrategy.ReplaceWithTerminal:
                    HandleGenericReplaceWithTerminal(ex, newTree);
                    break;
                    
                default:
                    throw ex;
            }
        }

        /// <summary>
        /// Generic version: Replace one of the existing children with the new subtree
        /// </summary>
        protected virtual void HandleGenericReplaceExistingChild(ISymbolicExpressionTreeNode<T> newTree)
        {
            if (genericSubtrees != null && genericSubtrees.Count > 0)
            {
                var random = new Random();
                var indexToReplace = random.Next(0, genericSubtrees.Count);
                
                // Remove old child
                var oldChild = genericSubtrees[indexToReplace];
                oldChild.Parent = null;
                
                // Replace with new tree
                genericSubtrees[indexToReplace] = newTree;
                newTree.Parent = this;
                
                // Also update base subtrees
                base.ReplaceSubtree(indexToReplace, newTree);
                
                System.Diagnostics.Debug.WriteLine($"Replaced generic child at index {indexToReplace} in {Symbol.SymbolName}");
            }
        }

        /// <summary>
        /// Generic version: Skip adding the subtree and continue
        /// </summary>
        protected virtual void HandleGenericSkipAndContinue(MaximumArityExceededException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Skipped adding generic child to {ex.SymbolName}: {ex.Message}");
            
            // Ensure minimum arity with terminals if needed
            while (genericSubtrees != null && genericSubtrees.Count < Symbol.MinimumArity)
            {
                try
                {
                    var terminalSymbols = Grammar?.Symbols.Where(s => s.MaximumArity == 0).ToList();
                    if (terminalSymbols != null && terminalSymbols.Any())
                    {
                        var random = new Random();
                        var randomTerminal = terminalSymbols[random.Next(terminalSymbols.Count)];
                        var terminal = randomTerminal.CreateTreeNode() as ISymbolicExpressionTreeNode<T>;
                        if (terminal != null)
                        {
                            genericSubtrees.Add(terminal);
                            terminal.Parent = this;
                            base.AddSubtree(terminal);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Generic version: Replace with terminal strategy
        /// </summary>
        protected virtual void HandleGenericReplaceWithTerminal(MaximumArityExceededException ex, ISymbolicExpressionTreeNode<T> newTree)
        {
            System.Diagnostics.Debug.WriteLine($"ReplaceWithTerminal strategy requested for generic {ex.SymbolName}, falling back to skip");
            HandleGenericSkipAndContinue(ex);
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

            // Para tipos genéricos iguais, sempre compatible durante crossover
            if (child.OutputType == typeof(T))
                return true;

            // Validação adicional apenas se necessário
            try
            {
                int currentChildCount = SubtreeCount;
                
                // Verificar se estamos dentro dos limites do array InputTypes
                if (currentChildCount < genericSymbol.InputTypes.Length)
                {
                    return genericSymbol.IsCompatibleChildType(child.OutputType, currentChildCount);
                }
                
                // Se estamos além dos tipos esperados, usar validação básica
                return child.OutputType == typeof(T);
            }
            catch (Exception ex)
            {
                // Durante crossover, ser permissivo com erros de validação
                System.Diagnostics.Debug.WriteLine($"Type compatibility check failed: {ex.Message}");
                return child.OutputType == typeof(T);
            }
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