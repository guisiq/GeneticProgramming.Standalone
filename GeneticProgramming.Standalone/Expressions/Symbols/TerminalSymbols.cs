using System;
using System.Collections.Generic;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Base class for terminal symbols (symbols with no children)
    /// </summary>
    public abstract class TerminalSymbol<T> : Symbol<T> where T : notnull, IConvertible
    {
        public override int MinimumArity => 0;
        public override int MaximumArity => 0;
        public override Type[] InputTypes => Array.Empty<Type>();
        

        protected TerminalSymbol(string name, string description) : base(name, description)
        {
        }

        protected TerminalSymbol(TerminalSymbol<T> original, Cloner cloner) : base(original, cloner)
        {
        }
        public override ISymbolicExpressionTreeNode<T> CreateTreeNode()
        {
            return new SymbolicExpressionTreeNode<T>(this);
        }

        /// <summary>
        /// Validates if a child symbol type is compatible with this terminal symbol.
        /// Terminal symbols do not accept children, so this always returns false.
        /// </summary>
        /// <param name="childOutputType">The output type of the child symbol.</param>
        /// <param name="argumentIndex">The position where the child would be placed.</param>
        /// <returns>False, as terminal symbols do not accept children.</returns>
        public override bool IsCompatibleChildType(Type childOutputType, int argumentIndex)
        {
            return false; // Terminal symbols do not accept children
        }
    }

    /// <summary>
    /// Symbol representing a variable in the expression
    /// </summary>
    public sealed class Variable<T> : TerminalSymbol<T>, IEvaluable<T> where T : notnull , IConvertible
    {
        public Variable() : base("Variable", "A variable symbol")
        {
        }

        private Variable(Variable<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Variable<T>(this, cloner);
        }

        /// <summary>
        /// Evaluates this variable by looking up its value in the variables dictionary.
        /// </summary>
        /// <param name="childValues">Not used for terminal symbols.</param>
        /// <param name="variables">Dictionary containing variable values.</param>
        /// <returns>The value of this variable.</returns>
        public T Evaluate(T[] childValues, IDictionary<string,T> variables)
        {
            if (variables.TryGetValue(Name, out T value))
                return value;
            throw new ArgumentException($"Variable '{Name}' not found in variables dictionary.");
        }

        public override ISymbolicExpressionTreeNode<T> CreateTreeNode()
        {
            return new VariableTreeNode<T>(this);
        }

        public override string GetFormatString()
        {
            return "X";
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Variable<T>(this, cloner);
        }
    }

    /// <summary>
    /// Symbol representing a numeric constant in the expression
    /// </summary>
    public sealed class Constant<T> : TerminalSymbol<T>, IEvaluable<T> where T : notnull, IConvertible 
    {
        public Constant() : base("Constant", "A numeric constant symbol")
        {
        }

        private Constant(Constant<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Constant<T>(this, cloner);
        }

        /// <summary>
        /// Evaluates this constant. For constants, the actual value comes from ConstantTreeNode.
        /// This method is not directly used as constants are handled by their tree nodes.
        /// </summary>
        /// <param name="childValues">Not used for terminal symbols.</param>
        /// <param name="variables">Not used for constants.</param>
        /// <returns>Default value (should not be called directly).</returns>
        public T Evaluate(T[] childValues, IDictionary<string, T> variables)
        {

            throw new InvalidOperationException("Constant evaluation should be handled by ConstantTreeNode.");
        }

        public override ISymbolicExpressionTreeNode<T> CreateTreeNode()
        {
            return new ConstantTreeNode<T>(this);
        }

        public override string GetFormatString()
        {
            return "C";
        }
        
        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Constant<T>(this, cloner);
        }
    }
}
