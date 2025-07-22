using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Symbol that executes a provided delegate when evaluated.
    /// </summary>
    public class FunctionalSymbol : Symbol
    {
        /// <summary>
        /// Delegate representing the operation of this symbol.
        /// The array of doubles are the evaluated child node values.
        /// </summary>
        public Func<double[], double> Operation { get; }

        /// <summary>
        /// Minimum number of child nodes required.
        /// </summary>
        public int MinArity { get; }

        /// <summary>
        /// Maximum number of child nodes allowed.
        /// </summary>
        public int MaxArity { get; }

        public override int MinimumArity => MinArity;
        public override int MaximumArity => MaxArity;

        /// <summary>
        /// Creates a new functional symbol.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="operation">Delegate implementing the operation.</param>
        /// <param name="minArity">Minimum number of arguments.</param>
        /// <param name="maxArity">Maximum number of arguments.</param>
        public FunctionalSymbol(string name, string description,
            Func<double[], double> operation, int minArity, int maxArity)
            : base(name, description)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            MinArity = minArity;
            MaxArity = maxArity;
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        private FunctionalSymbol(FunctionalSymbol original, Cloner cloner)
            : base(original, cloner)
        {
            Operation = original.Operation;
            MinArity = original.MinArity;
            MaxArity = original.MaxArity;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new FunctionalSymbol(this, cloner);
        }

        public override string GetFormatString()
        {
            return Name;
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new SymbolicExpressionTreeNode(this);
        }
    }
}
