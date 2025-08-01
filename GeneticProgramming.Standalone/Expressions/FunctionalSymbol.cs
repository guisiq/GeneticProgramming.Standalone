using System;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Symbol that executes a provided delegate when evaluated, operating on generic type T.
    /// </summary>
    public class FunctionalSymbol<T> : Symbol, IEvaluable<T>, ISymbol<T> where T : struct
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
        /// Gets the types that this symbol accepts as input
        /// </summary>
        public Type[] InputTypes { get; }

        /// <summary>
        /// Gets the type that this symbol produces as output
        /// </summary>
        public Type OutputType => typeof(T);

        /// <summary>
        /// Creates a new functional symbol with generic operation.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="operation">Delegate implementing the operation.</param>
        /// <param name="minArity">Minimum number of arguments.</param>
        /// <param name="maxArity">Maximum number of arguments.</param>
        /// <param name="inputTypes">The types accepted as input (optional, defaults to all T)</param>
        public FunctionalSymbol(string name, string description,
            Func<T[], T> operation, int minArity, int maxArity, Type[]? inputTypes = null)
            : base(name, description)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            MinArity = minArity;
            MaxArity = maxArity;
            
            // Default input types to T for all arguments
            if (inputTypes == null)
            {
                InputTypes = new Type[maxArity];
                for (int i = 0; i < maxArity; i++)
                {
                    InputTypes[i] = typeof(T);
                }
            }
            else
            {
                InputTypes = inputTypes;
            }
            
            // Validate input types array length
            if (InputTypes.Length < maxArity)
            {
                throw new ArgumentException($"InputTypes array must have at least {maxArity} elements");
            }
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
            InputTypes = (Type[])original.InputTypes.Clone();
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
            return new SymbolicExpressionTreeNode<T>(this);
        }

        /// <summary>
        /// Creates a generic tree node for this symbol
        /// </summary>
        /// <returns>A new generic tree node instance</returns>
        ISymbolicExpressionTreeNode<T> ISymbol<T>.CreateTreeNode()
        {
            return new SymbolicExpressionTreeNode<T>(this);
        }

        /// <summary>
        /// Validates if a child symbol type is compatible with this symbol at the given position
        /// </summary>
        /// <param name="childOutputType">The output type of the child symbol</param>
        /// <param name="argumentIndex">The position where the child would be placed</param>
        /// <returns>True if compatible, false otherwise</returns>
        public bool IsCompatibleChildType(Type childOutputType, int argumentIndex)
        {
            if (argumentIndex < 0 || argumentIndex >= InputTypes.Length)
                return false;
                
            return InputTypes[argumentIndex] == childOutputType;
        }

        /// <summary>
        /// Evaluates this functional symbol by applying its operation to the child values.
        /// </summary>
        /// <param name="childValues">The evaluated values from child nodes.</param>
        /// <param name="variables">Variable context (not used by functional symbols).</param>
        /// <returns>The result of applying the operation to the child values.</returns>
        public T Evaluate(T[] childValues, System.Collections.Generic.IDictionary<string, T> variables)
        {
            return Operation(childValues);
        }
    }
}
