using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Base class for terminal tree nodes (nodes with no children)
    /// </summary>
    public abstract class TerminalTreeNode : Item, ISymbolicExpressionTreeNode
    {
        private ISymbol symbol;
        private ISymbolicExpressionTreeNode? parent;

        public ISymbol Symbol
        {
            get { return symbol; }
            protected set { symbol = value; }
        }

        public ISymbolicExpressionTreeNode? Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual ISymbolicExpressionTreeGrammar? Grammar
        {
            get { return parent?.Grammar; }
        }

        public virtual bool HasLocalParameters => false;

        public IEnumerable<ISymbolicExpressionTreeNode> Subtrees
        {
            get { return Enumerable.Empty<ISymbolicExpressionTreeNode>(); }
        }

        public int SubtreeCount => 0;

        protected TerminalTreeNode() : base()
        {
        }

        protected TerminalTreeNode(ISymbol symbol) : base()
        {
            this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        protected TerminalTreeNode(TerminalTreeNode original, Cloner cloner) : base(original, cloner)
        {
            symbol = original.symbol; // Symbols are reused
        }

        #region Tree Structure Operations

        public int GetLength() => 1;
        public int GetDepth() => 1;

        public int GetBranchLevel(ISymbolicExpressionTreeNode child) => int.MaxValue;

        #endregion

        #region Subtree Management (Not Supported)

        public int IndexOfSubtree(ISymbolicExpressionTreeNode tree) => -1;

        public ISymbolicExpressionTreeNode GetSubtree(int index)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void AddSubtree(ISymbolicExpressionTreeNode tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void InsertSubtree(int index, ISymbolicExpressionTreeNode tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void RemoveSubtree(int index)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void ReplaceSubtree(int index, ISymbolicExpressionTreeNode tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void ReplaceSubtree(ISymbolicExpressionTreeNode old, ISymbolicExpressionTreeNode tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        #endregion

        #region Tree Iteration

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth()
        {
            yield return this;
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix()
        {
            yield return this;
        }

        public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            action(this);
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix()
        {
            yield return this;
        }

        public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            action(this);
        }

        #endregion

        #region Parameter Operations

        public virtual void ResetLocalParameters(IRandom random)
        {
            // Default implementation does nothing
        }

        public virtual void ShakeLocalParameters(IRandom random, double shakingFactor)
        {
            // Default implementation does nothing
        }

        #endregion

        public override string ToString()
        {
            return Symbol?.Name ?? "TerminalTreeNode";
        }
    }

    /// <summary>
    /// Tree node for constant values with a numeric value
    /// </summary>
    public class ConstantTreeNode : TerminalTreeNode
    {
        private double value;

        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public override bool HasLocalParameters => true;

        public ConstantTreeNode() : base()
        {
        }

        public ConstantTreeNode(ISymbol symbol) : base(symbol)
        {
            value = 1.0; // Default value
        }

        public ConstantTreeNode(ISymbol symbol, double value) : base(symbol)
        {
            this.value = value;
        }

        private ConstantTreeNode(ConstantTreeNode original, Cloner cloner) : base(original, cloner)
        {
            value = original.value;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new ConstantTreeNode(this, cloner);
        }

        public override void ResetLocalParameters(IRandom random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            
            // Generate random value between -10 and 10
            Value = random.NextDouble() * 20.0 - 10.0;
        }

        public override void ShakeLocalParameters(IRandom random, double shakingFactor)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));
            
            // Add noise to the current value
            double noise = (random.NextDouble() * 2.0 - 1.0) * shakingFactor;
            Value += noise;
        }

        public override string ToString()
        {
            return value.ToString("F3");
        }
    }

    /// <summary>
    /// Tree node for variable references
    /// </summary>
    public class VariableTreeNode : TerminalTreeNode
    {
        private string variableName;

        public string VariableName
        {
            get { return variableName; }
            set
            {
                variableName = value ?? throw new ArgumentNullException(nameof(value));
                OnPropertyChanged(nameof(VariableName));
            }
        }

        public VariableTreeNode() : base()
        {
            variableName = "X0";
        }

        public VariableTreeNode(ISymbol symbol) : base(symbol)
        {
            variableName = "X0";
        }

        public VariableTreeNode(ISymbol symbol, string variableName) : base(symbol)
        {
            this.variableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
        }

        private VariableTreeNode(VariableTreeNode original, Cloner cloner) : base(original, cloner)
        {
            variableName = original.variableName;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new VariableTreeNode(this, cloner);
        }

        public override string ToString()
        {
            return variableName;
        }
    }
}
