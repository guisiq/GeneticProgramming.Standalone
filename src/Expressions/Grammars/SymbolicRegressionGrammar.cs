using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Expressions.Grammars
{
    /// <summary>
    /// A specialized grammar for symbolic regression problems.
    /// </summary>
    [Serializable]
    public class SymbolicRegressionGrammar : SymbolicExpressionTreeGrammar
    {
        private readonly List<string> _variableNames;
        private bool _allowConstants;
        private bool _allowDivision;

        /// <summary>
        /// Gets the variable names used in this grammar.
        /// </summary>
        public IEnumerable<string> VariableNames => _variableNames.AsReadOnly();

        /// <summary>
        /// Gets or sets whether constants are allowed in expressions.
        /// </summary>
        public bool AllowConstants
        {
            get => _allowConstants;
            set
            {
                if (_allowConstants != value)
                {                    _allowConstants = value;
                    UpdateConstants();
                    OnPropertyChanged(nameof(AllowConstants));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether division operations are allowed.
        /// </summary>
        public bool AllowDivision
        {
            get => _allowDivision;
            set
            {
                if (_allowDivision != value)
                {                    _allowDivision = value;
                    UpdateDivision();
                    OnPropertyChanged(nameof(AllowDivision));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SymbolicRegressionGrammar class.
        /// </summary>
        /// <param name="variableNames">The names of input variables.</param>
        /// <param name="allowConstants">Whether to allow constants in expressions.</param>
        /// <param name="allowDivision">Whether to allow division operations.</param>
        public SymbolicRegressionGrammar(IEnumerable<string> variableNames, bool allowConstants = true, bool allowDivision = true)
        {
            _variableNames = new List<string>(variableNames ?? throw new ArgumentNullException(nameof(variableNames)));
            _allowConstants = allowConstants;
            _allowDivision = allowDivision;

            if (!_variableNames.Any())
                throw new ArgumentException("At least one variable name must be provided.", nameof(variableNames));

            Initialize();
        }        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        /// <param name="original">The original grammar to copy.</param>
        /// <param name="cloner">The cloner to use for deep cloning.</param>
        private SymbolicRegressionGrammar(SymbolicRegressionGrammar original, Core.Cloner cloner)
            : base(original, cloner)
        {
            _variableNames = new List<string>(original._variableNames);
            _allowConstants = original._allowConstants;
            _allowDivision = original._allowDivision;
        }

        /// <summary>
        /// Creates a deep clone of this grammar using the specified cloner.
        /// </summary>
        /// <param name="cloner">The cloner to use for deep cloning.</param>
        /// <returns>A cloned instance of the grammar.</returns>
        public override Core.IDeepCloneable Clone(Core.Cloner cloner)
        {
            return new SymbolicRegressionGrammar(this, cloner);
        }

        /// <summary>
        /// Initializes the grammar with symbols appropriate for symbolic regression.
        /// </summary>
        private void Initialize()
        {
            // Add basic mathematical operations
            AddSymbol(new Symbols.Addition());
            AddSymbol(new Symbols.Subtraction());
            AddSymbol(new Symbols.Multiplication());

            if (_allowDivision)
                AddSymbol(new Symbols.Division());

            // Add variables
            foreach (var variableName in _variableNames)
            {
                var variable = new Symbols.Variable { Name = variableName };
                AddSymbol(variable);
            }

            // Add constant if allowed
            if (_allowConstants)
                AddSymbol(new Symbols.Constant());

            // Configure grammar rules
            ConfigureRegressionRules();

            // Set reasonable defaults for regression problems
            MaximumExpressionLength = 50;
            MaximumExpressionDepth = 8;
            MinimumExpressionLength = 3;
            MinimumExpressionDepth = 1;
        }

        /// <summary>
        /// Configures the grammar rules specific to symbolic regression.
        /// </summary>
        private void ConfigureRegressionRules()
        {
            var functionSymbols = Symbols.Where(s => s.MinimumArity > 0).ToList();
            var terminalSymbols = Symbols.Where(s => s.MaximumArity == 0).ToList();
            var allSymbols = Symbols.ToList();

            // Function symbols can have any symbol as children
            foreach (var functionSymbol in functionSymbols)
            {
                SetAllowedChildSymbols(functionSymbol, allSymbols);
                AddStartSymbol(functionSymbol);
            }

            // Variables can be start symbols for simple expressions
            foreach (var variable in terminalSymbols.Where(s => s.Name != "Constant"))
            {
                AddStartSymbol(variable);
            }

            // Constants should not typically be root nodes alone
            // (though they can be children of other nodes)
        }

        /// <summary>
        /// Adds a new variable to the grammar.
        /// </summary>
        /// <param name="variableName">The name of the variable to add.</param>
        public void AddVariable(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("Variable name cannot be null or empty.", nameof(variableName));

            if (_variableNames.Contains(variableName))
                return; // Variable already exists            _variableNames.Add(variableName);
            var variable = new Symbols.Variable { Name = variableName };
            AddSymbol(variable);
            AddStartSymbol(variable);

            ConfigureRegressionRules();
        }

        /// <summary>
        /// Removes a variable from the grammar.
        /// </summary>
        /// <param name="variableName">The name of the variable to remove.</param>
        public void RemoveVariable(string variableName)
        {
            if (_variableNames.Remove(variableName))
            {
                var variable = GetSymbol(variableName);
                if (variable != null)
                    RemoveSymbol(variable);

                if (!_variableNames.Any())
                    throw new InvalidOperationException("Cannot remove the last variable from the grammar.");
            }
        }

        /// <summary>
        /// Updates the constant symbol based on the AllowConstants setting.
        /// </summary>
        private void UpdateConstants()
        {
            var constant = GetSymbol("Constant");
              if (_allowConstants && constant == null)
            {
                AddSymbol(new Symbols.Constant());
                ConfigureRegressionRules();
            }
            else if (!_allowConstants && constant != null)
            {
                RemoveSymbol(constant);
            }
        }

        /// <summary>
        /// Updates the division symbol based on the AllowDivision setting.
        /// </summary>
        private void UpdateDivision()
        {
            var division = GetSymbol("Division");
              if (_allowDivision && division == null)
            {
                AddSymbol(new Symbols.Division());
                ConfigureRegressionRules();
            }
            else if (!_allowDivision && division != null)
            {
                RemoveSymbol(division);
            }
        }

        /// <summary>
        /// Creates a simple grammar with only addition, subtraction, and specified variables.
        /// </summary>
        /// <param name="variableNames">The names of variables to include.</param>
        /// <returns>A simple regression grammar.</returns>
        public static SymbolicRegressionGrammar CreateSimpleGrammar(IEnumerable<string> variableNames)
        {
            return new SymbolicRegressionGrammar(variableNames, allowConstants: false, allowDivision: false);
        }

        /// <summary>
        /// Creates a standard grammar with basic operations and constants.
        /// </summary>
        /// <param name="variableNames">The names of variables to include.</param>
        /// <returns>A standard regression grammar.</returns>
        public static SymbolicRegressionGrammar CreateStandardGrammar(IEnumerable<string> variableNames)
        {
            return new SymbolicRegressionGrammar(variableNames, allowConstants: true, allowDivision: true);
        }

        /// <summary>
        /// Gets the probability distribution for selecting terminals vs functions.
        /// </summary>
        /// <param name="currentDepth">The current depth in the tree.</param>
        /// <param name="maxDepth">The maximum allowed depth.</param>
        /// <returns>A tuple with (terminalProbability, functionProbability).</returns>
        public (double TerminalProbability, double FunctionProbability) GetSelectionProbabilities(int currentDepth, int maxDepth)
        {
            // Increase terminal probability as we approach maximum depth
            if (currentDepth >= maxDepth - 1)
                return (1.0, 0.0); // Force terminals at maximum depth

            var depthRatio = (double)currentDepth / maxDepth;
            var terminalProb = Math.Min(0.8, depthRatio + 0.2); // Gradually increase from 0.2 to 0.8
            var functionProb = 1.0 - terminalProb;

            return (terminalProb, functionProb);
        }

        /// <summary>
        /// Validates that the grammar is suitable for symbolic regression.
        /// </summary>
        /// <returns>True if valid for regression, false otherwise.</returns>
        public bool ValidateForRegression()
        {
            // Must have at least one variable
            if (!_variableNames.Any())
                return false;

            // Must have at least one mathematical operation
            var hasOperations = Symbols.Any(s => s.MinimumArity > 0);
            if (!hasOperations)
                return false;

            // Must have start symbols
            if (!StartSymbols.Any())
                return false;

            return Validate(); // Use base validation
        }
    }
}
