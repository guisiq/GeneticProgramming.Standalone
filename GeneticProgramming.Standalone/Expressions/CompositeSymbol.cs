using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Symbol that generates a predefined subtree structure as a shortcut for composite mathematical operations.
    /// This allows complex mathematical expressions to be represented as reusable building blocks while maintaining
    /// the ability for genetic operators to mutate and crossover within the generated subtree.
    /// </summary>
    /// <typeparam name="T">The data type this symbol operates on.</typeparam>
    public class CompositeSymbol<T> : Symbol
    
    {
        /// <summary>
        /// Delegate that builds the subtree structure when CreateTreeNode is called.
        /// The parameter array represents placeholder nodes that will be replaced with actual child nodes.
        /// </summary>
        public Func<ISymbolicExpressionTreeNode[], ISymbolicExpressionTreeNode> SubtreeBuilder { get; }

        public int _MinimumArity;
        public int _MaximumArity;
        public override int MinimumArity => _MinimumArity;

        public override int MaximumArity => _MaximumArity;

        /// <summary>
        /// Creates a new composite symbol that generates predefined subtree structures.
        /// </summary>
        /// <param name="name">Symbol name.</param>
        /// <param name="description">Symbol description.</param>
        /// <param name="arity">Number of input parameters this composite symbol requires.</param>
        /// <param name="subtreeBuilder">Delegate that builds the subtree structure using placeholder nodes.</param>
        /// <param name="operation">Operation to execute when evaluating (for compatibility with FunctionalSymbol).</param>
        public CompositeSymbol(string name, string description, 
            Func<ISymbolicExpressionTreeNode[], ISymbolicExpressionTreeNode> subtreeBuilder,int arity)
            : base(name, description)
        {
            _MinimumArity = arity;
            _MaximumArity = arity;

            SubtreeBuilder = subtreeBuilder ?? throw new ArgumentNullException(nameof(subtreeBuilder));
        }
        public CompositeSymbol(string name, string description, 
            Func<ISymbolicExpressionTreeNode[], ISymbolicExpressionTreeNode> subtreeBuilder,int minarity, int? maxarity = null)
            : base(name, description)
        {
            _MinimumArity = minarity;
            _MaximumArity = maxarity??minarity;

            SubtreeBuilder = subtreeBuilder ?? throw new ArgumentNullException(nameof(subtreeBuilder));
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        protected CompositeSymbol(CompositeSymbol<T> original, Cloner cloner)
            : base(original, cloner)
        {
            SubtreeBuilder = original.SubtreeBuilder;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new CompositeSymbol<T>(this, cloner);
        }

        /// <summary>
        /// Creates a tree node by generating the predefined subtree structure.
        /// This method builds the composite structure using placeholder nodes that will be
        /// replaced with actual child nodes during tree construction.
        /// </summary>
        /// <returns>The root node of the generated subtree.</returns>
        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            // Create placeholder nodes for each input parameter
            var placeholders = new ISymbolicExpressionTreeNode[_MinimumArity];
            for (int i = 0; i < _MinimumArity; i++)
            {
                placeholders[i] = new ParameterPlaceholderNode<T>(i);
            }

            // Build the subtree using the placeholders
            return SubtreeBuilder(placeholders);
        }

        public override string GetFormatString()
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Placeholder node used during subtree construction to represent input parameters.
    /// These nodes will be replaced with actual child nodes during tree building.
    /// </summary>
    /// <typeparam name="T">The data type this node operates on.</typeparam>
    internal class ParameterPlaceholderNode<T> : SymbolicExpressionTreeNode, IEvaluable<T>
    {
        /// <summary>
        /// The parameter index this placeholder represents.
        /// </summary>
        public int ParameterIndex { get; }

        /// <summary>
        /// Creates a new parameter placeholder node.
        /// </summary>
        /// <param name="parameterIndex">The index of the parameter this placeholder represents.</param>
        public ParameterPlaceholderNode(int parameterIndex) 
            : base(new ParameterPlaceholderSymbol<T>(parameterIndex))
        {
            ParameterIndex = parameterIndex;
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        protected ParameterPlaceholderNode(ParameterPlaceholderNode<T> original, Cloner cloner)
            : base(original, cloner)
        {
            ParameterIndex = original.ParameterIndex;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new ParameterPlaceholderNode<T>(this, cloner);
        }

        /// <summary>
        /// Evaluates this placeholder by returning the corresponding parameter value.
        /// </summary>
        /// <param name="childValues">Not used for placeholder nodes.</param>
        /// <param name="variables">Variable context containing parameter values.</param>
        /// <returns>The parameter value from the variables dictionary.</returns>
        public T Evaluate(T[] childValues, IDictionary<string, T> variables)
        {
            var paramKey = $"param_{ParameterIndex}";
            if (variables.TryGetValue(paramKey, out var value))
                return value;
            
            
            throw new InvalidOperationException($"Parameter {ParameterIndex} not found in evaluation context.");
        }
    }

    /// <summary>
    /// Symbol representing a parameter placeholder in composite symbol subtrees.
    /// </summary>
    /// <typeparam name="T">The data type this symbol operates on.</typeparam>
    internal class ParameterPlaceholderSymbol<T> : Symbol
    {
        /// <summary>
        /// The parameter index this symbol represents.
        /// </summary>
        public int ParameterIndex { get; }

        /// <summary>
        /// Creates a new parameter placeholder symbol.
        /// </summary>
        /// <param name="parameterIndex">The index of the parameter this symbol represents.</param>
        public ParameterPlaceholderSymbol(int parameterIndex) 
            : base($"Param{parameterIndex}", $"Parameter placeholder for index {parameterIndex}")
        {
            ParameterIndex = parameterIndex;
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        protected ParameterPlaceholderSymbol(ParameterPlaceholderSymbol<T> original, Cloner cloner)
            : base(original, cloner)
        {
            ParameterIndex = original.ParameterIndex;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new ParameterPlaceholderSymbol<T>(this, cloner);
        }

        public override int MinimumArity => 0;
        public override int MaximumArity => 0;

        public override string GetFormatString()
        {
            return $"${ParameterIndex}";
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new ParameterPlaceholderNode<T>(ParameterIndex);
        }
    }
}
