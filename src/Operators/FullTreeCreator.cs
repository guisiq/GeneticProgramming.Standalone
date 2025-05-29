using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Operators
{
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
        protected override Item CreateCloneInstance(Cloner cloner)
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
