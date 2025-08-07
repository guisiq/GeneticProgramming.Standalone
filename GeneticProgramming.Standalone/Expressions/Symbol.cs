using System;
using GeneticProgramming.Core;
using GeneticProgramming.Abstractions.Parameters; // For IParameterCollection

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Base class for all symbols in symbolic expression trees
    /// </summary>
    public abstract class Symbol<T> : Item, ISymbol<T> where T : notnull
    {
        private double initialFrequency = 1.0;
        private double weight = 1.0;
        private bool enabled = true;

        /// <summary>
        /// Gets the input types for this symbol.
        /// </summary>
        public abstract Type[] InputTypes { get; }

        /// <summary>
        /// Gets the output type for this symbol.
        /// </summary>
        public virtual Type OutputType => typeof(T);

        /// <summary>
        /// Checks if the given child type is compatible at the specified index.
        /// </summary>
        /// <param name="childType">Type of the child.</param>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns>True if compatible, otherwise false.</returns>
        public abstract bool IsCompatibleChildType(Type childType, int argumentIndex);

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
        public virtual string SymbolName => !string.IsNullOrEmpty(Name) ? Name : GetType().Name;

        /// <summary>
        /// Gets or sets the initial frequency of this symbol in random generation
        /// </summary>
        public virtual double InitialFrequency
        {
            get { return initialFrequency; }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "InitialFrequency must be >= 0.0");
                }
                initialFrequency = value;
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
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Weight must be >= 0.0");
                }
                weight = value;
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
            {
                enabled = value;
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
            // Parameters = new ParameterCollection(); // Already initialized in base Item constructor
        }

        /// <summary>
        /// Copy constructor for cloning
        /// </summary>
        /// <param name="original">Original symbol to clone</param>
        /// <param name="cloner">Cloner instance</param>
        protected Symbol(Symbol<T> original, Cloner cloner) : base(original, cloner)
        {
            initialFrequency = original.initialFrequency;
            enabled = original.enabled;
            weight = original.weight; // Adicionado para garantir que o peso seja clonado
        }

        protected abstract override IDeepCloneable CreateCloneInstance(Cloner cloner);

        /// <summary>
        /// Gets the format string for this symbol, used for display purposes.
        /// For example, an addition symbol might return "+".
        /// </summary>
        /// <returns>The format string.</returns>
        public abstract string GetFormatString(); // Moved from individual symbols to base Symbol class

        /// <summary>
        /// Creates a tree node for this symbol
        /// </summary>
        /// <returns>A new tree node instance</returns>
        public abstract ISymbolicExpressionTreeNode<T> CreateTreeNode();

        // Evaluate method remains abstract as its implementation is specific to each symbol type
        // public abstract object Evaluate(params object[] arguments); // Example, adjust as per your design
    }
}
