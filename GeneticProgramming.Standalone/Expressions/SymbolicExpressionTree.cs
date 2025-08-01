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

    /// <summary>
    /// Generic implementation of a symbolic expression tree with type safety
    /// </summary>
    /// <typeparam name="T">The value type that the tree evaluates to (must be a struct)</typeparam>
    [Item("SymbolicExpressionTree<T>", "Represents a generic symbolic expression tree")]
    public class SymbolicExpressionTree<T> : SymbolicExpressionTree, ISymbolicExpressionTree<T> where T : struct
    {
        private ISymbolicExpressionTreeNode<T>? genericRoot;

        /// <summary>
        /// Gets or sets the generic root node
        /// </summary>
        public new ISymbolicExpressionTreeNode<T> Root
        {
            get => genericRoot ?? throw new InvalidOperationException("Root node is not set");
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                
                if (value != genericRoot)
                {
                    genericRoot = value;
                    base.Root = value; // Also set base root for compatibility
                    OnPropertyChanged(nameof(Root));
                }
            }
        }

        /// <summary>
        /// Gets the output type of this tree
        /// </summary>
        public Type OutputType => typeof(T);

        /// <summary>
        /// Default constructor
        /// </summary>
        public SymbolicExpressionTree() : base() { }

        /// <summary>
        /// Creates a new tree with the specified root
        /// </summary>
        /// <param name="root">The generic root node</param>
        public SymbolicExpressionTree(ISymbolicExpressionTreeNode<T> root) : base()
        {
            Root = root;
        }

        /// <summary>
        /// Copy constructor for cloning
        /// </summary>
        /// <param name="original">Original tree to clone</param>
        /// <param name="cloner">Cloner instance</param>
        protected SymbolicExpressionTree(SymbolicExpressionTree<T> original, Cloner cloner) : base(original, cloner)
        {
            if (original.genericRoot != null)
            {
                var clonedRoot = cloner.Clone(original.genericRoot) as ISymbolicExpressionTreeNode<T>;
                genericRoot = clonedRoot ?? throw new InvalidOperationException("Failed to clone root node");
            }
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new SymbolicExpressionTree<T>(this, cloner);
        }

        /// <summary>
        /// Generic iteration methods
        /// </summary>
        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesBreadth()
        {
            if (genericRoot == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode<T>>();
            return genericRoot.IterateNodesBreadth();
        }

        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPrefix()
        {
            if (genericRoot == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode<T>>();
            return genericRoot.IterateNodesPrefix();
        }

        public new IEnumerable<ISymbolicExpressionTreeNode<T>> IterateNodesPostfix()
        {
            if (genericRoot == null)
                return Enumerable.Empty<ISymbolicExpressionTreeNode<T>>();
            return genericRoot.IterateNodesPostfix();
        }
    }
}
