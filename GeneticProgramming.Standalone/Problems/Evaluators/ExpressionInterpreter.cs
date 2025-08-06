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
        public T Evaluate<T>(ISymbolicExpressionTree<T> tree, IDictionary<string, T> variables) where T : struct, IConvertible
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            if (variables == null) throw new ArgumentNullException(nameof(variables));
            return EvaluateNode(tree.Root, variables);
        }

        private T EvaluateNode<T>(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> vars) where T : struct, IConvertible
        {
            switch (node)
            {
                case ConstantTreeNode<T> c:
                    return c.Value;
                case VariableTreeNode<T> v:
                    if (vars.TryGetValue(v.Symbol.Name, out T valBySymbol))
                        return valBySymbol;
                    if (vars.TryGetValue(v.VariableName, out T valByName))
                        return valByName;
                    throw new ArgumentException($"Variable '{v.Symbol.Name}' not provided.");
                case SymbolicExpressionTreeNode<T> internalNode:
                    if (internalNode.Symbol is IEvaluable<T> evaluableSymbol)
                    {
                        var childValues = internalNode.Subtrees
                            .Select(child => EvaluateNode(child, vars))
                            .ToArray();

                        return evaluableSymbol.Evaluate(childValues, vars);
                    }

                    break;
            }
            throw new NotSupportedException($"Symbol '{node.Symbol.SymbolName}' not supported in interpreter.");
        }
    }
}
