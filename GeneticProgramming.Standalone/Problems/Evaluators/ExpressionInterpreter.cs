using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using static GeneticProgramming.Expressions.Symbols.MathematicalSymbols;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Simple interpreter for evaluating symbolic expression trees.
    /// Supports basic arithmetic operations, variables and constants.
    /// </summary>
    public class ExpressionInterpreter
    {
        public double Evaluate(ISymbolicExpressionTree tree, IDictionary<string, double> variables)
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            return EvaluateNode(tree.Root, variables);
        }

        private double EvaluateNode(ISymbolicExpressionTreeNode node, IDictionary<string, double> vars)
        {
            switch (node)
            {
                case ConstantTreeNode c:
                    return c.Value;
                case VariableTreeNode v:
                    if (vars.TryGetValue(v.Symbol.Name, out double valBySymbol))
                        return valBySymbol;
                    if (vars.TryGetValue(v.VariableName, out double valByName))
                        return valByName;
                    throw new ArgumentException($"Variable '{v.Symbol.Name}' not provided.");
                case SymbolicExpressionTreeNode internalNode:
                    // Use the new IEvaluable interface if the symbol implements it
                    if (internalNode.Symbol is IEvaluable<double> evaluableSymbol)
                    {
                        // Evaluate all children and collect their values
                        var childValues = internalNode.Subtrees
                            .Select(child => EvaluateNode(child, vars))
                            .ToArray();
                        
                        // Use the symbol's own evaluation method
                        return evaluableSymbol.Evaluate(childValues, vars);
                    }
 
                    break;
            }
            throw new NotSupportedException($"Symbol '{node.Symbol.SymbolName}' not supported in interpreter.");
        }
    }
}
