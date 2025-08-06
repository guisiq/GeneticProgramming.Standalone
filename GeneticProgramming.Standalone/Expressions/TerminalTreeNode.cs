using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Base class for terminal tree nodes (nodes with no children)
    /// </summary>
    public abstract class TerminalTreeNode<T> : Item, ISymbolicExpressionTreeNode<T> where T : struct
    {
        private ISymbol<T> symbol;
        private ISymbolicExpressionTreeNode<T>? parent;

        public ISymbol<T> Symbol
        {
            get { return symbol; }
            protected set { symbol = value; }
        }

        public ISymbolicExpressionTreeNode<T>? Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual ISymbolicExpressionTreeGrammar<T>? Grammar
        {
            get { return parent?.Grammar; }
        }

        public virtual bool HasLocalParameters => false;

        public IEnumerable<ISymbolicExpressionTreeNode<T>> Subtrees
        {
            get { return Enumerable.Empty<ISymbolicExpressionTreeNode<T>>(); }
        }

        public int SubtreeCount => 0;

        protected TerminalTreeNode() : base()
        {
        }

        protected TerminalTreeNode(ISymbol<T> symbol) : base()
        {
            this.symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        protected TerminalTreeNode(TerminalTreeNode<T> original, Cloner cloner) : base(original, cloner)
        {
            symbol = original.symbol; // Symbols are reused
        }

        // Implementação adicional necessária para a interface ISymbolicExpressionTreeNode<T>
        public Type OutputType => typeof(T);

        public bool IsCompatibleChild(ISymbolicExpressionTreeNode<T> child)
        {
            // Terminal nodes não podem ter filhos
            return false;
        }

        #region Tree Structure Operations

        public int GetLength() => 1;
        public int GetDepth() => 1;

        public int GetBranchLevel(ISymbolicExpressionTreeNode child) => int.MaxValue;

        #endregion

        #region Subtree Management (Not Supported)

        public int IndexOfSubtree(ISymbolicExpressionTreeNode<T> tree) => -1;

        public ISymbolicExpressionTreeNode<T> GetSubtree(int index)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void AddSubtree(ISymbolicExpressionTreeNode<T> tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void InsertSubtree(int index, ISymbolicExpressionTreeNode<T> tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void RemoveSubtree(int index)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void ReplaceSubtree(int index, ISymbolicExpressionTreeNode<T> tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        public void ReplaceSubtree(ISymbolicExpressionTreeNode<T> old, ISymbolicExpressionTreeNode<T> tree)
        {
            throw new NotSupportedException("Terminal nodes cannot have subtrees");
        }

        #endregion

        #region Tree Iteration

        public IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesBreadth()
        {
            yield return this;
        }

        public IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPrefix()
        {
            yield return this;
        }

        public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode<T>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            action(this);
        }

        public IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix()
        {
            yield return this;
        }

        public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode<T>> action)
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
    public class ConstantTreeNode<T> : TerminalTreeNode<T> where T : struct
    {
        private T value;

        public T Value
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

        public ConstantTreeNode(ISymbol<T> symbol) : base(symbol)
        {
            value = default(T); 
        }

        public ConstantTreeNode(ISymbol<T> symbol, T value) : base(symbol)
        {
            this.value = value;
        }

        private ConstantTreeNode(ConstantTreeNode<T> original, Cloner cloner) : base(original, cloner)
        {
            value = original.value;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new ConstantTreeNode<T>(this, cloner);
        }

        public override void ResetLocalParameters(IRandom random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            // Generate random value between -10 and 10
            Value = default(T);//ToDo: Implement random value generation logic based on T
        }
        //Todo: Implement shaking logic based on T

        public override void ShakeLocalParameters(IRandom random, double shakingFactor)
        {
            //Todo implementar
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return value.ToString() ?? "0";
        }
    }

    /// <summary>
    /// Tree node for variable references
    /// </summary>
    public class VariableTreeNode<T> : TerminalTreeNode<T> where T : struct
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

        public VariableTreeNode(ISymbol<T> symbol) : base(symbol)
        {
            variableName = "X0";
        }

        public VariableTreeNode(ISymbol<T> symbol, string variableName) : base(symbol)
        {
            this.variableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
        }

        private VariableTreeNode(VariableTreeNode<T> original, Cloner cloner) : base(original, cloner)
        {
            variableName = original.variableName;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new VariableTreeNode<T>(this, cloner);
        }

        public override string ToString()
        {
            return variableName;
        }
    }
}
