using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Base class for terminal symbols (symbols with no children)
    /// </summary>
    public abstract class TerminalSymbol : Symbol
    {
        public override int MinimumArity => 0;
        public override int MaximumArity => 0;

        protected TerminalSymbol(string name, string description) : base(name, description)
        {
        }

        protected TerminalSymbol(TerminalSymbol original, Cloner cloner) : base(original, cloner)
        {
        }
        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new SymbolicExpressionTreeNode(this);
        }
    }

    /// <summary>
    /// Symbol representing a variable in the expression
    /// </summary>
    public sealed class Variable : TerminalSymbol
    {
        public Variable() : base("Variable", "A variable symbol")
        {
        }

        private Variable(Variable original, Cloner cloner) : base(original, cloner)
        {
        }        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Variable(this, cloner);
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new VariableTreeNode(this);
        }

        public override string GetFormatString()
        {
            return "X";
        }

        protected override Core.Item CreateCloneInstance(Core.Cloner cloner)
        {
            return new Variable(this, cloner);
        }
    }

    /// <summary>
    /// Symbol representing a numeric constant in the expression
    /// </summary>
    public sealed class Constant : TerminalSymbol
    {
        public Constant() : base("Constant", "A numeric constant symbol")
        {
        }

        private Constant(Constant original, Cloner cloner) : base(original, cloner)
        {
        }        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Constant(this, cloner);
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new ConstantTreeNode(this);
        }        public override string GetFormatString()
        {
            return "C";
        }
        protected override Core.Item CreateCloneInstance(Core.Cloner cloner)
        {
            return new Constant(this, cloner);
        }
    }
}
