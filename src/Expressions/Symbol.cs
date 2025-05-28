using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Base class for all symbols in symbolic expression trees
    /// </summary>
    public abstract class Symbol : Item, ISymbol
    {
        private double initialFrequency = 1.0;
        private double weight = 1.0;
        private bool enabled = true;

        /// <summary>
        /// Gets the minimum number of arguments this symbol requires
        /// </summary>
        public abstract int MinimumArity { get; }

        /// <summary>
        /// Gets the maximum number of arguments this symbol can accept
        /// </summary>
        public abstract int MaximumArity { get; }

        /// <summary>
        /// Gets the name of this symbol
        /// </summary>
        public virtual string SymbolName => GetType().Name;

        /// <summary>
        /// Gets or sets the initial frequency of this symbol in random generation
        /// </summary>
        public virtual double InitialFrequency
        {
            get { return initialFrequency; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "InitialFrequency must be >= 0.0");                initialFrequency = value;
                OnPropertyChanged(nameof(InitialFrequency));
            }
        }

        /// <summary>
        /// Gets or sets the weight of this symbol for selection
        /// </summary>
        public virtual double Weight
        {
            get { return weight; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Weight must be >= 0.0");                weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        /// <summary>
        /// Gets or sets whether this symbol is enabled in the grammar
        /// </summary>
        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {                enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }        /// <summary>
        /// Creates a new symbol with the specified name and description
        /// </summary>
        /// <param name="name">Symbol name</param>
        /// <param name="description">Symbol description</param>
        protected Symbol(string name, string description) : base()
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Copy constructor for cloning
        /// </summary>
        /// <param name="original">Original symbol to clone</param>
        /// <param name="cloner">Cloner instance</param>
        protected Symbol(Symbol original, Cloner cloner) : base(original, cloner)
        {
            initialFrequency = original.initialFrequency;
            enabled = original.enabled;
        }

        /// <summary>
        /// Creates a tree node for this symbol
        /// </summary>
        /// <returns>A new tree node instance</returns>
        public virtual ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new SymbolicExpressionTreeNode(this);
        }

        /// <summary>
        /// Gets the format string for displaying this symbol
        /// </summary>
        /// <returns>Format string</returns>
        public virtual string GetFormatString()
        {
            return Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
