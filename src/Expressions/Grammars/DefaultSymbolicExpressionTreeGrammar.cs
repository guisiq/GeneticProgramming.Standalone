using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Expressions.Grammars
{
    /// <summary>
    /// A default grammar for symbolic regression problems with basic mathematical operations.
    /// </summary>
    [Serializable]
    public class DefaultSymbolicExpressionTreeGrammar : SymbolicExpressionTreeGrammar
    {
        /// <summary>
        /// Initializes a new instance of the DefaultSymbolicExpressionTreeGrammar class.
        /// </summary>
        public DefaultSymbolicExpressionTreeGrammar()
        {
            Initialize();
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        /// <param name="original">The original grammar to copy.</param>
        protected DefaultSymbolicExpressionTreeGrammar(DefaultSymbolicExpressionTreeGrammar original) : base(original)
        {
        }

        /// <summary>
        /// Creates a deep clone of this grammar.
        /// </summary>
        /// <returns>A cloned instance of the grammar.</returns>
        public override Core.IDeepCloneable Clone()
        {
            return new DefaultSymbolicExpressionTreeGrammar(this);
        }

        /// <summary>
        /// Initializes the grammar with default symbols and rules.
        /// </summary>
        private void Initialize()
        {
            // Add mathematical operation symbols
            var addition = new Addition();
            var subtraction = new Subtraction();
            var multiplication = new Multiplication();
            var division = new Division();

            AddSymbol(addition);
            AddSymbol(subtraction);
            AddSymbol(multiplication);
            AddSymbol(division);

            // Add terminal symbols
            var variable = new Variable();
            var constant = new Constant();

            AddSymbol(variable);
            AddSymbol(constant);

            // Set up grammar rules
            ConfigureSymbolRules();

            // Set reasonable defaults for expression constraints
            MaximumExpressionLength = 100;
            MaximumExpressionDepth = 10;
            MinimumExpressionLength = 3;
            MinimumExpressionDepth = 1;
        }

        /// <summary>
        /// Configures the rules for which symbols can be used where.
        /// </summary>
        private void ConfigureSymbolRules()
        {
            var functionSymbols = Symbols.Where(s => s.MinimumArity > 0).ToList();
            var terminalSymbols = Symbols.Where(s => s.MaximumArity == 0).ToList();
            var allSymbols = Symbols.ToList();

            // Function symbols can have any symbol as children
            foreach (var functionSymbol in functionSymbols)
            {
                SetAllowedChildSymbols(functionSymbol, allSymbols);
            }

            // All function symbols can be start symbols
            foreach (var functionSymbol in functionSymbols)
            {
                AddStartSymbol(functionSymbol);
            }

            // Terminal symbols can also be start symbols (for simple expressions)
            foreach (var terminalSymbol in terminalSymbols)
            {
                AddStartSymbol(terminalSymbol);
            }
        }

        /// <summary>
        /// Creates a grammar with specific variable names.
        /// </summary>
        /// <param name="variableNames">The names of variables to include in the grammar.</param>
        /// <returns>A new grammar instance with the specified variables.</returns>
        public static DefaultSymbolicExpressionTreeGrammar CreateGrammarWithVariables(IEnumerable<string> variableNames)
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            
            // Remove the default variable symbol
            var defaultVariable = grammar.GetSymbol("Variable");
            if (defaultVariable != null)
                grammar.RemoveSymbol(defaultVariable);

            // Add specific variables
            foreach (var variableName in variableNames)
            {
                var variable = new Variable { Name = variableName };
                grammar.AddSymbol(variable);
                grammar.AddStartSymbol(variable);
            }

            // Reconfigure rules after adding new variables
            grammar.ConfigureSymbolRules();

            return grammar;
        }

        /// <summary>
        /// Creates a minimal grammar with only basic operations.
        /// </summary>
        /// <returns>A new minimal grammar instance.</returns>
        public static DefaultSymbolicExpressionTreeGrammar CreateMinimalGrammar()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            
            // Keep only addition, subtraction, and terminals
            var symbolsToRemove = grammar.Symbols
                .Where(s => s.Name == "Multiplication" || s.Name == "Division")
                .ToList();

            foreach (var symbol in symbolsToRemove)
            {
                grammar.RemoveSymbol(symbol);
            }

            return grammar;
        }

        /// <summary>
        /// Creates an extended grammar with additional mathematical functions.
        /// </summary>
        /// <returns>A new extended grammar instance.</returns>
        public static DefaultSymbolicExpressionTreeGrammar CreateExtendedGrammar()
        {
            var grammar = new DefaultSymbolicExpressionTreeGrammar();
            
            // Add more mathematical operations (these would need to be implemented)
            // For now, this is a placeholder for future extensions
            // Examples: Sin, Cos, Log, Exp, Power, etc.
            
            return grammar;
        }

        /// <summary>
        /// Validates that the grammar is properly configured.
        /// </summary>
        /// <returns>True if the grammar is valid, false otherwise.</returns>
        public bool Validate()
        {
            // Check that there are start symbols
            if (!StartSymbols.Any())
                return false;

            // Check that there are terminal symbols
            if (!Symbols.Any(s => s.MaximumArity == 0))
                return false;

            // Check that constraints are valid
            if (MaximumExpressionLength < MinimumExpressionLength)
                return false;

            if (MaximumExpressionDepth < MinimumExpressionDepth)
                return false;

            // Check that all function symbols have allowed children
            foreach (var functionSymbol in Symbols.Where(s => s.MinimumArity > 0))
            {
                var allowedChildren = GetAllowedChildSymbols(functionSymbol);
                if (!allowedChildren.Any())
                    return false;
            }

            return true;
        }
    }
}
