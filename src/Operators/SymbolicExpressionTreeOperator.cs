using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Abstractions.Operators; // Required for IOperator
using System.ComponentModel; // Required for INotifyPropertyChanged

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Base class for all symbolic expression tree operators
    /// </summary>
    public abstract class SymbolicExpressionTreeOperator : Item, ISymbolicExpressionTreeOperator // Item already implements IOperator, IDeepCloneable, INotifyPropertyChanged via IItem
    {
        private ISymbolicExpressionTreeGrammar? _symbolicExpressionTreeGrammar;

        /// <summary>
        /// Gets or sets the symbolic expression tree grammar used by this operator
        /// </summary>
        public ISymbolicExpressionTreeGrammar? SymbolicExpressionTreeGrammar
        {
            get => _symbolicExpressionTreeGrammar;
            set
            {
                if (_symbolicExpressionTreeGrammar != value)
                {
                    _symbolicExpressionTreeGrammar = value;
                    OnPropertyChanged(nameof(SymbolicExpressionTreeGrammar));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SymbolicExpressionTreeOperator class
        /// </summary>
        protected SymbolicExpressionTreeOperator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SymbolicExpressionTreeOperator class
        /// </summary>
        /// <param name="original">The original operator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected SymbolicExpressionTreeOperator(SymbolicExpressionTreeOperator original, Cloner cloner) : base(original, cloner)
        {
            _symbolicExpressionTreeGrammar = cloner.Clone(original._symbolicExpressionTreeGrammar);
        }

        /// <summary>
        /// Creates a deep clone of this operator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this operator</returns>
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return base.Clone(cloner);
        }

        /// <summary>
        /// Creates a clone instance, to be implemented by derived classes.
        /// </summary>
        /// <param name="cloner">The cloner to use for cloning.</param>
        /// <returns>A new instance of the derived type.</returns>
        protected abstract override IDeepCloneable CreateCloneInstance(Core.Cloner cloner);

    }
}
