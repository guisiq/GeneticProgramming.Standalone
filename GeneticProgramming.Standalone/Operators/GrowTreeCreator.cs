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
    public class GrowTreeCreator<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeCreator<T> where T : struct
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
        protected GrowTreeCreator(GrowTreeCreator<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        /// <summary>
        /// Creates a deep clone of this creator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this creator</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new GrowTreeCreator<T>(this, cloner);
        }

        /// <summary>
        /// Creates a new symbolic expression tree using the grow method
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <param name="grammar">Grammar to use for tree creation</param>
        /// <param name="maxTreeLength">Maximum length of the created tree</param>
        /// <param name="maxTreeDepth">Maximum depth of the created tree</param>
        /// <returns>A new symbolic expression tree</returns>
        public ISymbolicExpressionTree<T> CreateTree(IRandom random, ISymbolicExpressionTreeGrammar<T> grammar, int maxTreeLength, int maxTreeDepth)
        {
            // Validate parameters
            if (random == null) throw new ArgumentNullException(nameof(random));
            if (grammar == null) throw new ArgumentNullException(nameof(grammar));
            if (maxTreeLength < 0) throw new ArgumentOutOfRangeException(nameof(maxTreeLength), "Maximum tree length must be greater than or equal to 0");
            if (maxTreeDepth < 0) throw new ArgumentOutOfRangeException(nameof(maxTreeDepth), "Maximum tree depth must be greater than or equal to 0");
            
            // If max length or depth is 0, or max length is 1, must create terminal
            if (maxTreeLength <= 1 || maxTreeDepth <= 1)
            {
                var tree = new SymbolicExpressionTree<T>();
                tree.Root = CreateTerminalNode(random, grammar);
                return tree;
            }

            var resultTree = new SymbolicExpressionTree<T>();
            var rootNode = CreateNode(random, grammar, maxTreeDepth, maxTreeLength);
            resultTree.Root = rootNode;
            return resultTree;
        }

        private ISymbolicExpressionTreeNode<T> CreateNode(IRandom random, ISymbolicExpressionTreeGrammar<T> grammar, int maxDepth, int maxLength)
        {
            if (maxDepth <= 1 || maxLength <= 1)
            {
                // Force terminal selection
                return CreateTerminalNode(random, grammar);
            }

            // Get all allowed symbols
            // If we're at max depth, must use a terminal
            if (maxDepth <= 1)
            {
                return CreateTerminalNode(random, grammar);
            }

            var allowedSymbols = grammar.Symbols.Where(s => s.Enabled).ToList();
            if (!allowedSymbols.Any())
            {
                throw new InvalidOperationException("No enabled symbols found in grammar");
            }

            // Select a symbol based on frequency
            var selectedSymbol = SelectSymbolByFrequency(random, allowedSymbols);
            var node = selectedSymbol.CreateTreeNode();

            // If it's a terminal, return as is
            if (selectedSymbol.MaximumArity == 0)
            {
                return node;
            }

            // Add children for non-terminals
            var remainingLength = maxLength - 1; // Account for current node
            
            // For binary operators, always use exactly 2 children
            var arity = selectedSymbol.MinimumArity == selectedSymbol.MaximumArity ? 
                selectedSymbol.MinimumArity : 
                random.Next(selectedSymbol.MinimumArity, Math.Min(selectedSymbol.MaximumArity, Math.Min(5, remainingLength)) + 1);
            
            // Ensure we don't violate minimum arity constraints
            if (arity < selectedSymbol.MinimumArity)
            {
                arity = selectedSymbol.MinimumArity;
            }
            
            // Check if we have enough remaining length to satisfy arity
            if (remainingLength < arity)
            {
                // If we can't satisfy arity requirements, fall back to terminal
                return CreateTerminalNode(random, grammar);
            }
            
            var childrenPerSubtree = remainingLength / Math.Max(1, arity);

            for (int i = 0; i < arity; i++)
            {
                var childDepth = maxDepth - 1;
                var childLength = Math.Max(1, Math.Min(childrenPerSubtree, remainingLength));
                var child = CreateNode(random, grammar, childDepth, childLength);
                node.AddSubtree(child);
                remainingLength -= child.GetLength();
            }

            return node;
        }

        private ISymbolicExpressionTreeNode<T> CreateTerminalNode(IRandom random, ISymbolicExpressionTreeGrammar<T> grammar)
        {
            var terminals = grammar.Symbols.Where(s => s.Enabled && s.MaximumArity == 0).ToList();
            if (!terminals.Any())
            {
                throw new InvalidOperationException("No terminal symbols found in grammar");
            }

            var selectedTerminal = SelectSymbolByFrequency(random, terminals);
            return selectedTerminal.CreateTreeNode();
        }

        private ISymbol<T> SelectSymbolByFrequency(IRandom random, IList<ISymbol<T>> symbols)
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
