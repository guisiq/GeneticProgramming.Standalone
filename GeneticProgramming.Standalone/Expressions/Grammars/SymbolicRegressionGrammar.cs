using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Expressions.Grammars
{
    /// <summary>
    /// A specialized grammar for symbolic regression problems.
    /// </summary>
    [Serializable]
    /// <typeparam name="T">o tipo dos valores de entrada e saída que devem ser numéricos.</typeparam>
    public class SymbolicRegressionGrammar<T> : SymbolicExpressionTreeGrammar<T> where T : notnull, IComparable<T>, IEquatable<T>, IConvertible
    {
        private readonly List<string> _variableNames;
        private readonly List<ISymbol<T>> _funSymbols;
        private bool _allowConstants;

        /// <summary>
        /// Gets the variable names used in this grammar.
        /// </summary>
        public IEnumerable<string> VariableNames => _variableNames.AsReadOnly();

        /// <summary>
        /// Gets the functional symbols used in this grammar.
        /// </summary>
        public IEnumerable<ISymbol<T>> FunSymbols => _funSymbols.AsReadOnly();

        /// <summary>
        /// Gets or sets whether constants are allowed in expressions.
        /// </summary>
        public bool AllowConstants
        {
            get => _allowConstants;
            set
            {
                if (_allowConstants != value)
                {
                    _allowConstants = value;
                    UpdateConstants();
                    OnPropertyChanged(nameof(AllowConstants));
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the SymbolicRegressionGrammar class.
        /// </summary>
        /// <param name="variableNames">The names of input variables.</param>
        /// <param name="funSymbols">The functional symbols to include in the grammar.</param>
        /// <param name="allowConstants">Whether to allow constants in expressions.</param>
        public SymbolicRegressionGrammar(IEnumerable<string> variableNames, IEnumerable<ISymbol<T>> funSymbols , bool allowConstants = true)
            : base("SymbolicRegressionGrammar", "A grammar for symbolic regression with mathematical operations and variables")
        {
            _variableNames = new List<string>(variableNames ?? throw new ArgumentNullException(nameof(variableNames)));
            _funSymbols = new List<ISymbol<T>>(funSymbols);
            _allowConstants = allowConstants;

            if (!_variableNames.Any())
                throw new ArgumentException("At least one variable name must be provided.", nameof(variableNames));

            if (!_funSymbols.Any())
                throw new ArgumentException("At least one functional symbol must be provided.", nameof(funSymbols));

            Initialize();
        }

        /// <summary>
        /// Copy constructor for cloning.
        /// </summary>
        /// <param name="original">The original grammar to copy.</param>
        /// <param name="cloner">The cloner to use for deep cloning.</param>
        private SymbolicRegressionGrammar(SymbolicRegressionGrammar<T> original, Core.Cloner cloner)
            : base(original, cloner)
        {
            _variableNames = new List<string>(original._variableNames);
            _funSymbols = new List<ISymbol<T>>(original._funSymbols);
            _allowConstants = original._allowConstants;
        }

        /// <summary>
        /// Creates a deep clone of this grammar using the specified cloner.
        /// </summary>
        /// <param name="cloner">The cloner to use for deep cloning.</param>
        /// <returns>A cloned instance of the grammar.</returns>
        public override Core.IDeepCloneable Clone(Core.Cloner cloner)
        {
            return new SymbolicRegressionGrammar<T>(this, cloner);
        }

        /// <summary>
        /// Initializes the grammar with symbols appropriate for symbolic regression.
        /// </summary>
        private void Initialize()
        {
            // Add the functional symbols passed as parameters
            foreach (var funSymbol in _funSymbols)
            {
                AddSymbol(funSymbol);
            }

            // Add variables
            foreach (var variableName in _variableNames)
            {
                var variable = new Variable<T> { Name = variableName };
                AddSymbol(variable);
            }

            // Add constant if allowed
            if (_allowConstants)
                AddSymbol(new Constant<T> { Name = "Constant" });

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
                return; // Variable already exists
            _variableNames.Add(variableName); // This line is crucial for adding the name to VariableNames
            var variable = new Variable<T> { Name = variableName };
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
                AddSymbol(new Constant<T> { Name = "Constant" });
                ConfigureRegressionRules();
            }
            else if (!_allowConstants && constant != null)
            {
                RemoveSymbol(constant);
            }
        }

        /// <summary>
        /// Validates the grammar for regression problems.
        /// </summary>
        public bool ValidateForRegression()
        {
            return Validate();
        }
    }
}
