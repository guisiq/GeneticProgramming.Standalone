using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Core;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Represents a symbolic expression tree
    /// </summary>
    [Item("SymbolicExpressionTree", "Represents a symbolic expression tree")]
    public class SymbolicExpressionTree : Item, ISymbolicExpressionTree
    {
        private ISymbolicExpressionTreeNode? root;

        public ISymbolicExpressionTreeNode Root
        {
            get => root ?? throw new InvalidOperationException("Root node is not set");
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                
                if (value != root)
                {
                    root = value;
                    OnPropertyChanged(nameof(Root));
                }
            }
        }

        public int Length
        {
            get
            {
                if (root == null)
                    return 0;
                return root.GetLength();
            }
        }

        public int Depth
        {
            get
            {
                if (root == null)
                    return 0;
                return root.GetDepth();
            }
        }

        public SymbolicExpressionTree() : base() { }

        public SymbolicExpressionTree(ISymbolicExpressionTreeNode root) : base()
        {
            Root = root;
        }

        protected SymbolicExpressionTree(SymbolicExpressionTree original, Cloner cloner) : base(original, cloner)
        {
            if (original.root != null)
                root = cloner.Clone(original.root);
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesBreadth();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadthFirst()
        {
            return IterateNodesBreadth();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesPrefix();
        }

        public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix()
        {
            if (root == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode>();
            return root.IterateNodesPostfix();
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new SymbolicExpressionTree(this, cloner);
        }

        public override string ToString()
        {
            if (root == null)
                return "Empty Tree";
            
            return $"Tree (Length: {Length}, Depth: {Depth})";
        }

        /// <summary>
        /// Retorna uma representação textual da árvore em formato hierárquico visual
        /// </summary>
        /// <returns>String representando a estrutura da árvore</returns>
        public string ToTreeString()
        {
            if (root == null)
                return "Empty Tree";

            return ToTreeStringRecursive(root, "", true);
        }

        /// <summary>
        /// Método recursivo para construir a representação visual da árvore
        /// </summary>
        private string ToTreeStringRecursive(ISymbolicExpressionTreeNode node, string indent, bool isLast)
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

            var subtrees = node.Subtrees.ToList();
            for (int i = 0; i < subtrees.Count; i++)
            {
                bool isLastChild = (i == subtrees.Count - 1);
                result += ToTreeStringRecursive(subtrees[i], indent, isLastChild);
            }

            return result;
        }

        /// <summary>
        /// Retorna uma representação textual da árvore em formato de expressão matemática (notação infixa)
        /// </summary>
        /// <returns>String representando a expressão matemática</returns>
        public string ToMathString()
        {
            if (root == null)
                return "Empty";

            return ToMathStringRecursive(root);
        }

        /// <summary>
        /// Método recursivo para construir a representação matemática da árvore
        /// </summary>
        private string ToMathStringRecursive(ISymbolicExpressionTreeNode node)
        {
            var symbolName = node.Symbol.SymbolName;
            var subtrees = node.Subtrees.ToList();

            // Nó terminal (sem filhos)
            if (subtrees.Count == 0)
            {
                return symbolName;
            }

            // Operadores binários comuns
            if (subtrees.Count == 2 && IsCommonBinaryOperator(symbolName))
            {
                var left = ToMathStringRecursive(subtrees[0]);
                var right = ToMathStringRecursive(subtrees[1]);
                return $"({left} {GetOperatorSymbol(symbolName)} {right})";
            }

            // Operadores unários
            if (subtrees.Count == 1 && IsCommonUnaryOperator(symbolName))
            {
                var operand = ToMathStringRecursive(subtrees[0]);
                return $"{GetOperatorSymbol(symbolName)}({operand})";
            }

            // Funções genéricas
            var args = string.Join(", ", subtrees.Select(ToMathStringRecursive));
            return $"{symbolName}({args})";
        }

        /// <summary>
        /// Verifica se o símbolo é um operador binário comum
        /// </summary>
        private bool IsCommonBinaryOperator(string symbolName)
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

        /// <summary>
        /// Verifica se o símbolo é um operador unário comum
        /// </summary>
        private bool IsCommonUnaryOperator(string symbolName)
        {
            return symbolName switch
            {
                "Sin" or "Cos" or "Tan" or "Log" or "Exp" or "Sqrt" or "Abs" => true,
                _ => false
            };
        }

        /// <summary>
        /// Retorna o símbolo matemático correspondente ao nome do operador
        /// </summary>
        private string GetOperatorSymbol(string symbolName)
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

        /// <summary>
        /// Imprime a árvore no console em formato visual
        /// </summary>
        public void PrintTree()
        {
            Console.WriteLine("Árvore de Expressão Simbólica:");
            Console.WriteLine("═══════════════════════════════");
            Console.WriteLine(ToTreeString());
            Console.WriteLine();
            Console.WriteLine($"Expressão Matemática: {ToMathString()}");
            Console.WriteLine($"Comprimento: {Length} nós");
            Console.WriteLine($"Profundidade: {Depth} níveis");
        }
    }
}
