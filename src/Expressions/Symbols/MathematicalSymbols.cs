using System;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Symbol representing addition operation (+)
    /// </summary>
    public sealed class Addition : Symbol
    {
        private const int MinArity = 2;
        private const int MaxArity = 2;

        public override int MinimumArity => MinArity;
        public override int MaximumArity => MaxArity;

        public Addition() : base("Addition", "Addition operation (+)")
        {
        }

        private Addition(Addition original, Cloner cloner) : base(original, cloner)
        {
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Addition(this, cloner);
        }

        public override string GetFormatString()
        {
            return "+";
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            var node = new SymbolicExpressionTreeNode(this);
            return node;
        }
    }

    /// <summary>
    /// Symbol representing subtraction operation (-)
    /// </summary>
    public sealed class Subtraction : Symbol
    {
        private const int MinArity = 2;
        private const int MaxArity = 2;

        public override int MinimumArity => MinArity;
        public override int MaximumArity => MaxArity;

        public Subtraction() : base("Subtraction", "Subtraction operation (-)")
        {
        }

        private Subtraction(Subtraction original, Cloner cloner) : base(original, cloner)
        {
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Subtraction(this, cloner);
        }

        public override string GetFormatString()
        {
            return "-";
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            var node = new SymbolicExpressionTreeNode(this);
            return node;
        }
    }

    /// <summary>
    /// Symbol representing multiplication operation (*)
    /// </summary>
    public sealed class Multiplication : Symbol
    {
        private const int MinArity = 2;
        private const int MaxArity = 2;

        public override int MinimumArity => MinArity;
        public override int MaximumArity => MaxArity;

        public Multiplication() : base("Multiplication", "Multiplication operation (*)")
        {
        }

        private Multiplication(Multiplication original, Cloner cloner) : base(original, cloner)
        {
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Multiplication(this, cloner);
        }

        public override string GetFormatString()
        {
            return "*";
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            var node = new SymbolicExpressionTreeNode(this);
            return node;
        }
    }

    /// <summary>
    /// Symbol representing division operation (/)
    /// </summary>
    public sealed class Division : Symbol
    {
        private const int MinArity = 2;
        private const int MaxArity = 2;

        public override int MinimumArity => MinArity;
        public override int MaximumArity => MaxArity;

        public Division() : base("Division", "Division operation (/)")
        {
        }

        private Division(Division original, Cloner cloner) : base(original, cloner)
        {
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Division(this, cloner);
        }

        public override string GetFormatString()
        {
            return "/";
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            var node = new SymbolicExpressionTreeNode(this);
            return node;
        }
    }
}
