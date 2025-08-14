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
    public class FullTreeCreator<T> : SymbolicExpressionTreeOperator<T>, ISymbolicExpressionTreeCreator<T> where T : notnull
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
        protected FullTreeCreator(FullTreeCreator<T> original, Cloner cloner) : base(original, cloner)
        {
        }

        /// <summary>
        /// Creates a deep clone of this creator
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this creator</returns>
        protected override Item CreateCloneInstance(Cloner cloner)
        {
            return new FullTreeCreator<T>(this, cloner);
        }

        /// <summary>
        /// Creates a new symbolic expression tree using the full method
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
            var rootNode = CreateFullNode(random, grammar, maxTreeDepth, maxTreeLength);
            resultTree.Root = rootNode;
            return resultTree;
        }

        private ISymbolicExpressionTreeNode<T> CreateFullNode(IRandom random, ISymbolicExpressionTreeGrammar<T> grammar, int depth, int maxLength)
        {
            if (depth <= 1 || maxLength <= 1)
            {
                // Force terminal selection at leaf level or when maxLength constraint is hit
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
            var node = (ISymbolicExpressionTreeNode<T>)selectedSymbol.CreateTreeNode();

            // Check if we have enough remaining length for minimum arity
            var remainingLength = maxLength - 1; // Subtract 1 for current node
            if (remainingLength < selectedSymbol.MinimumArity)
            {
                // If we can't satisfy minimum arity, fall back to terminal
                return CreateTerminalNode(random, grammar);
            }

            // Add maximum arity children but respect length constraints
            var maxArity = selectedSymbol.MaximumArity == int.MaxValue ?
                Math.Min(5, remainingLength) : selectedSymbol.MaximumArity; // Limit variadic symbols
            var arity = Math.Min(maxArity, remainingLength);
            var childLength = remainingLength / arity;

            for (int i = 0; i < arity; i++)
            {
                var actualChildLength = Math.Max(1, childLength);
                var child = CreateFullNode(random, grammar, depth - 1, actualChildLength);
                node.AddSubtree(child);
                remainingLength -= child.GetLength();

                // Stop if we're running out of length budget
                if (remainingLength <= 0) break;
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
            return (ISymbolicExpressionTreeNode<T>)selectedTerminal.CreateTreeNode();
        }

        private ISymbol SelectSymbolByFrequency(IRandom random, IList<ISymbol<T>> symbols)
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
