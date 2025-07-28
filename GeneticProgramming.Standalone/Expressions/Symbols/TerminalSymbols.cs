using System;
using System.Collections.Generic;
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
    public sealed class Variable : TerminalSymbol, IEvaluable<double>
    {
        public Variable() : base("Variable", "A variable symbol")
        {
        }

        private Variable(Variable original, Cloner cloner) : base(original, cloner)
        {
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Variable(this, cloner);
        }

        /// <summary>
        /// Evaluates this variable by looking up its value in the variables dictionary.
        /// </summary>
        /// <param name="childValues">Not used for terminal symbols.</param>
        /// <param name="variables">Dictionary containing variable values.</param>
        /// <returns>The value of this variable.</returns>
        public double Evaluate(double[] childValues, IDictionary<string, double> variables)
        {
            if (variables.TryGetValue(Name, out double value))
                return value;
            throw new ArgumentException($"Variable '{Name}' not found in variables dictionary.");
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new VariableTreeNode(this);
        }

        public override string GetFormatString()
        {
            return "X";
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Variable(this, cloner);
        }
    }

    /// <summary>
    /// Symbol representing a numeric constant in the expression
    /// </summary>
    public sealed class Constant : TerminalSymbol, IEvaluable<double>
    {
        public Constant() : base("Constant", "A numeric constant symbol")
        {
        }

        private Constant(Constant original, Cloner cloner) : base(original, cloner)
        {
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new Constant(this, cloner);
        }

        /// <summary>
        /// Evaluates this constant. For constants, the actual value comes from ConstantTreeNode.
        /// This method is not directly used as constants are handled by their tree nodes.
        /// </summary>
        /// <param name="childValues">Not used for terminal symbols.</param>
        /// <param name="variables">Not used for constants.</param>
        /// <returns>Default value (should not be called directly).</returns>
        public double Evaluate(double[] childValues, IDictionary<string, double> variables)
        {

            throw new InvalidOperationException("Constant evaluation should be handled by ConstantTreeNode.");
        }

        public override ISymbolicExpressionTreeNode CreateTreeNode()
        {
            return new ConstantTreeNode(this);
        }

        public override string GetFormatString()
        {
            return "C";
        }
        
        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new Constant(this, cloner);
        }
    }
}
