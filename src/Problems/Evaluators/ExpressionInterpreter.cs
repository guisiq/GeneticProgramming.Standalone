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
                    // Se o símbolo é um FunctionalSymbol<double>, use sua operação
                    if (internalNode.Symbol is FunctionalSymbol<double> functionalSymbol)
                    {
                        // Avalie todos os filhos e colete os valores
                        var childValues = internalNode.Subtrees
                            .Select(child => EvaluateNode(child, vars))
                            .ToArray();
                        
                        // Execute a operação do símbolo funcional
                        return functionalSymbol.Operation(childValues);
                    }
                    
                    // Fallback para símbolos não-funcionais (compatibilidade)
                    var children = internalNode.Subtrees.ToList();
                    double c0 = children.Count > 0 ? EvaluateNode(children[0], vars) : 0.0;
                    double c1 = children.Count > 1 ? EvaluateNode(children[1], vars) : 0.0;
                    switch (internalNode.Symbol.Name)
                    {
                        case "Addition":
                            return c0 + c1;
                        case "Subtraction":
                            return c0 - c1;
                        case "Multiplication":
                            return c0 * c1;
                        case "Division":
                            if (Math.Abs(c1) < 1e-12) return 0.0;
                            return c0 / c1;
                    }
                    break;
            }
            throw new NotSupportedException($"Symbol '{node.Symbol.SymbolName}' not supported in interpreter.");
        }
    }
}
