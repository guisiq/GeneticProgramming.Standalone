using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Extension methods for visualizing symbolic expression trees.
    /// </summary>
    public static class SymbolicExpressionTreeExtensions
    {
        /// <summary>
        /// Returns a hierarchical ASCII representation of the tree.
        /// </summary>
        public static string ToTreeString(this ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "Empty Tree";
            return tree.Root.ToTreeString();
        }

        /// <summary>
        /// Returns a hierarchical ASCII representation of a subtree node.
        /// </summary>
        public static string ToTreeString(this ISymbolicExpressionTreeNode node)
        {
            return ToTreeStringRecursive(node, string.Empty, true);
        }

        private static string ToTreeStringRecursive(ISymbolicExpressionTreeNode node, string indent, bool isLast)
        {
            var result = indent;
            if (isLast)
            {
                result += "└── ";
                indent += "    ";
            }
            else
            {
                result += "├── ";
                indent += "│   ";
            }

            result += node.Symbol.SymbolName + Environment.NewLine;
            var children = node.Subtrees.ToList();
            for (int i = 0; i < children.Count; i++)
            {
                bool isLastChild = (i == children.Count - 1);
                result += ToTreeStringRecursive(children[i], indent, isLastChild);
            }
            return result;
        }

        /// <summary>
        /// Returns a mathematical (infix) string of the tree.
        /// </summary>
        public static string ToMathString(this ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "Empty";
            return tree.Root.ToMathString();
        }

        /// <summary>
        /// Returns a mathematical (infix) string of a subtree node.
        /// </summary>
        public static string ToMathString(this ISymbolicExpressionTreeNode node)
        {
            return ToMathStringRecursive(node);
        }

        private static string ToMathStringRecursive(ISymbolicExpressionTreeNode node)
        {
            var symbolName = node.Symbol.SymbolName;
            var children = node.Subtrees.ToList();
            if (children.Count == 0)
                return symbolName;

            if (children.Count == 2 && IsCommonBinaryOperator(symbolName))
            {
                var left = ToMathStringRecursive(children[0]);
                var right = ToMathStringRecursive(children[1]);
                return $"({left} {GetOperatorSymbol(symbolName)} {right})";
            }

            if (children.Count == 1 && IsCommonUnaryOperator(symbolName))
            {
                var operand = ToMathStringRecursive(children[0]);
                return $"{GetOperatorSymbol(symbolName)}({operand})";
            }

            var args = string.Join(", ", children.Select(ToMathStringRecursive));
            return $"{symbolName}({args})";
        }

        private static bool IsCommonBinaryOperator(string symbolName)
        {
            return symbolName switch
            {
                "Addition" or "Add" or "+" => true,
                "Subtraction" or "Sub" or "-" => true,
                "Multiplication" or "Mul" or "*" => true,
                "Division" or "Div" or "/" => true,
                "Power" or "Pow" or "^" => true,
                _ => false
            };
        }

        private static bool IsCommonUnaryOperator(string symbolName)
        {
            return symbolName switch
            {
                "Sin" or "Cos" or "Tan" or "Log" or "Exp" or "Sqrt" or "Abs" => true,
                _ => false
            };
        }

        private static string GetOperatorSymbol(string symbolName)
        {
            return symbolName switch
            {
                "Addition" or "Add" => "+",
                "Subtraction" or "Sub" => "-",
                "Multiplication" or "Mul" => "*",
                "Division" or "Div" => "/",
                "Power" or "Pow" => "^",
                "Sin" => "sin",
                "Cos" => "cos",
                "Tan" => "tan",
                "Log" => "log",
                "Exp" => "exp",
                "Sqrt" => "sqrt",
                "Abs" => "abs",
                _ => symbolName
            };
        }
    }
}
