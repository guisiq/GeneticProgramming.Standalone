using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Symbol that executes a provided delegate when evaluated.
    /// </summary>
/// <summary>
/// Symbol that executes a provided delegate when evaluated, operating on generic type T.
/// </summary>
public class FunctionalSymbol<T> : Symbol
    {
    /// <summary>
    /// Delegate representing the operation of this symbol.
    /// The array of T are the evaluated child node values.
    /// </summary>
    public Func<T[], T> Operation { get; }

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
        /// Creates a new functional symbol with generic operation.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="operation">Delegate implementing the operation.</param>
        /// <param name="minArity">Minimum number of arguments.</param>
        /// <param name="maxArity">Maximum number of arguments.</param>
        public FunctionalSymbol(string name, string description,
            Func<T[], T> operation, int minArity, int maxArity)
            : base(name, description)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            MinArity = minArity;
            MaxArity = maxArity;
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        protected FunctionalSymbol(FunctionalSymbol<T> original, Cloner cloner)
            : base(original, cloner)
        {
            Operation = original.Operation;
            MinArity = original.MinArity;
            MaxArity = original.MaxArity;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new FunctionalSymbol<T>(this, cloner);
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
