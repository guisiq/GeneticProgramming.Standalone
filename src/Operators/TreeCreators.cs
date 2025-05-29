using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Creates random symbolic expression trees using the grow method
    /// </summary>
    public class GrowTreeCreator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCreator
    {
        /// <summary>
        /// Initializes a new instance of the GrowTreeCreator class
        /// </summary>
        public GrowTreeCreator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GrowTreeCreator class
        /// </summary>
        /// <param name="original">The original creator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected GrowTreeCreator(GrowTreeCreator original, Cloner cloner) : base(original, cloner)
        {
        }

        /// <summary>
        /// Creates a deep clone of this creator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this creator</returns>
        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new GrowTreeCreator(this, cloner);
        }

        /// <summary>
        /// Creates a new symbolic expression tree using the grow method
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="grammar">Grammar to use for tree creation</param>
        /// <param name="maxTreeLength">Maximum length of the created tree</param>
        /// <param name="maxTreeDepth">Maximum depth of the created tree</param>
        /// <returns>A new symbolic expression tree</returns>
        public ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionTreeGrammar grammar, int maxTreeLength, int maxTreeDepth)
        {
            var tree = new SymbolicExpressionTree();
            var rootNode = CreateNode(random, grammar, maxTreeDepth, maxTreeLength);
            tree.Root = rootNode;
            return tree;
        }

        private ISymbolicExpressionTreeNode CreateNode(IRandom random, ISymbolicExpressionTreeGrammar grammar, int maxDepth, int maxLength)
        {
            if (maxDepth <= 1 || maxLength <= 1)
            {
                // Force terminal selection
                return CreateTerminalNode(random, grammar);
            }

            // Get all allowed symbols
            var allowedSymbols = grammar.Symbols.Where(s => s.Enabled).ToList();
            if (!allowedSymbols.Any())
            {
                throw new InvalidOperationException("No enabled symbols found in grammar");
            }

            // Select a symbol based on frequency
            var selectedSymbol = SelectSymbolByFrequency(random, allowedSymbols);
            var node = selectedSymbol.CreateTreeNode();

            // If it's a terminal or we're at max depth, return as is
            if (selectedSymbol.MaximumArity == 0 || maxDepth <= 1)
            {
                return node;
            }

            // Add children for non-terminals
            var arity = random.Next(selectedSymbol.MinimumArity, selectedSymbol.MaximumArity + 1);
            var remainingLength = maxLength - 1; // Subtract 1 for current node
            var childrenPerSubtree = remainingLength / Math.Max(1, arity);

            for (int i = 0; i < arity; i++)
            {
                var childDepth = maxDepth - 1;
                var childLength = Math.Min(childrenPerSubtree, remainingLength);
                var child = CreateNode(random, grammar, childDepth, childLength);
                node.AddSubtree(child);
                remainingLength -= child.GetLength();
                
                if (remainingLength <= 0) break;
            }

            return node;
        }

        private ISymbolicExpressionTreeNode CreateTerminalNode(IRandom random, ISymbolicExpressionTreeGrammar grammar)
        {
            var terminals = grammar.Symbols.Where(s => s.Enabled && s.MaximumArity == 0).ToList();
            if (!terminals.Any())
            {
                throw new InvalidOperationException("No terminal symbols found in grammar");
            }

            var selectedTerminal = SelectSymbolByFrequency(random, terminals);
            return selectedTerminal.CreateTreeNode();
        }

        private ISymbol SelectSymbolByFrequency(IRandom random, IList<ISymbol> symbols)
        {
            if (!symbols.Any())
            {
                throw new ArgumentException("Symbol list cannot be empty");
            }

            if (symbols.Count == 1)
            {
                return symbols[0];
            }

            // Calculate total frequency
            double totalFrequency = symbols.Sum(s => s.InitialFrequency);
            if (totalFrequency <= 0)
            {
                // If no frequencies set, select randomly
                return symbols[random.Next(symbols.Count)];
            }

            // Roulette wheel selection
            double randomValue = random.NextDouble() * totalFrequency;
            double currentSum = 0;

            foreach (var symbol in symbols)
            {
                currentSum += symbol.InitialFrequency;
                if (randomValue <= currentSum)
                {
                    return symbol;
                }
            }

            // Fallback to last symbol
            return symbols[symbols.Count - 1];
        }
    }

    /// <summary>
    /// Creates random symbolic expression trees using the full method
    /// </summary>
    public class FullTreeCreator : SymbolicExpressionTreeOperator, ISymbolicExpressionTreeCreator
    {
        /// <summary>
        /// Initializes a new instance of the FullTreeCreator class
        /// </summary>
        public FullTreeCreator() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FullTreeCreator class
        /// </summary>
        /// <param name="original">The original creator to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected FullTreeCreator(FullTreeCreator original, Cloner cloner) : base(original, cloner)
        {
        }

        /// <summary>
        /// Creates a deep clone of this creator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this creator</returns>
        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new FullTreeCreator(this, cloner);
        }

        /// <summary>
        /// Creates a new symbolic expression tree using the full method
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="grammar">Grammar to use for tree creation</param>
        /// <param name="maxTreeLength">Maximum length of the created tree</param>
        /// <param name="maxTreeDepth">Maximum depth of the created tree</param>
        /// <returns>A new symbolic expression tree</returns>
        public ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionTreeGrammar grammar, int maxTreeLength, int maxTreeDepth)
        {
            var tree = new SymbolicExpressionTree();
            var rootNode = CreateFullNode(random, grammar, maxTreeDepth);
            tree.Root = rootNode;
            return tree;
        }

        private ISymbolicExpressionTreeNode CreateFullNode(IRandom random, ISymbolicExpressionTreeGrammar grammar, int depth)
        {
            if (depth <= 1)
            {
                // Force terminal selection at leaf level
                return CreateTerminalNode(random, grammar);
            }

            // Force non-terminal selection at internal levels
            var nonTerminals = grammar.Symbols.Where(s => s.Enabled && s.MaximumArity > 0).ToList();
            if (!nonTerminals.Any())
            {
                // Fallback to terminals if no non-terminals available
                return CreateTerminalNode(random, grammar);
            }

            var selectedSymbol = SelectSymbolByFrequency(random, nonTerminals);
            var node = selectedSymbol.CreateTreeNode();

            // Add maximum arity children
            var arity = selectedSymbol.MaximumArity;
            for (int i = 0; i < arity; i++)
            {
                var child = CreateFullNode(random, grammar, depth - 1);
                node.AddSubtree(child);
            }

            return node;
        }

        private ISymbolicExpressionTreeNode CreateTerminalNode(IRandom random, ISymbolicExpressionTreeGrammar grammar)
        {
            var terminals = grammar.Symbols.Where(s => s.Enabled && s.MaximumArity == 0).ToList();
            if (!terminals.Any())
            {
                throw new InvalidOperationException("No terminal symbols found in grammar");
            }

            var selectedTerminal = SelectSymbolByFrequency(random, terminals);
            return selectedTerminal.CreateTreeNode();
        }

        private ISymbol SelectSymbolByFrequency(IRandom random, IList<ISymbol> symbols)
        {
            if (!symbols.Any())
            {
                throw new ArgumentException("Symbol list cannot be empty");
            }

            if (symbols.Count == 1)
            {
                return symbols[0];
            }

            // Calculate total frequency
            double totalFrequency = symbols.Sum(s => s.InitialFrequency);
            if (totalFrequency <= 0)
            {
                // If no frequencies set, select randomly
                return symbols[random.Next(symbols.Count)];
            }

            // Roulette wheel selection
            double randomValue = random.NextDouble() * totalFrequency;
            double currentSum = 0;

            foreach (var symbol in symbols)
            {
                currentSum += symbol.InitialFrequency;
                if (randomValue <= currentSum)
                {
                    return symbol;
                }
            }

            // Fallback to last symbol
            return symbols[symbols.Count - 1];
        }
    }
}
