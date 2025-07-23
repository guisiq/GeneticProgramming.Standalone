using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Collection of mathematical functional symbols.
    /// </summary>
    public static class MathematicalSymbols
    {
        public static readonly FunctionalSymbol<double> Addition =
            SymbolFactory<double>.CreateBinary(
                "Addition", "Addition operation (+)",
                (a, b) => a + b);

        public static readonly FunctionalSymbol<double> Subtraction =
            SymbolFactory<double>.CreateBinary(
                "Subtraction", "Subtraction operation (-)",
                (a, b) => a - b);

        public static readonly FunctionalSymbol<double> Multiplication =
            SymbolFactory<double>.CreateBinary(
                "Multiplication", "Multiplication operation (*)",
                (a, b) => a * b);

        /// <summary>
        /// Alternative protected division that returns the numerator when dividing by zero
        /// </summary>
        public static readonly FunctionalSymbol<double> ProtectedDivision = SymbolFactory<double>.CreateBinary(
            "ProtectedDivision", "pdiv", 
            (a, b) => Math.Abs(b) < double.Epsilon ? a : a / b);

        /// <summary>
        /// Safe division that returns zero when dividing by zero
        /// </summary>
        public static readonly FunctionalSymbol<double> SafeDivision = SymbolFactory<double>.CreateBinary(
            "SafeDivision", "sdiv", 
            (a, b) => Math.Abs(b) < double.Epsilon ? double.MaxValue : a / b);
        public static readonly List<ISymbol> AllSymbols = new List<ISymbol>
        {
            Addition,
            Subtraction,
            Multiplication,
            ProtectedDivision
        };
    }

    public static class MathematicalLogarithmicSymbols
    {


        public static readonly FunctionalSymbol<double> Logarithm =
            SymbolFactory<double>.CreateBinary(
                "Logarithm", "Logarithm operation (log)",
                (a, b) =>
                {
                    if (a <= 0 || b <= 0 || a == 1)
                        throw new ArgumentException("Base and value must be positive and base cannot be 1.");
                    return Math.Log(b, a);
                });
        //loga ritimo de base e
        public static readonly FunctionalSymbol<double> NaturalLogarithm =
            SymbolFactory<double>.CreateUnary(
                "NaturalLogarithm", "Natural logarithm operation (ln)",
                Math.Log);

        public static readonly FunctionalSymbol<double> Exponential =
            SymbolFactory<double>.CreateUnary(
                "Exponential", "Exponential operation (exp)",
                Math.Exp);
        public static readonly List<ISymbol> AllSymbols = new List<ISymbol>
        {
            Logarithm,
            Exponential
        };
    }
    public static class MathematicalTrigonometricSymbols
    {

        public static readonly FunctionalSymbol<double> Sine =
            SymbolFactory<double>.CreateUnary(
                "Sine", "Sine operation (sin)",
                Math.Sin);

        public static readonly FunctionalSymbol<double> Cosine =
            SymbolFactory<double>.CreateUnary(
                "Cosine", "Cosine operation (cos)",
                Math.Cos);

        public static readonly FunctionalSymbol<double> Tangent =
            SymbolFactory<double>.CreateUnary(
                "Tangent", "Tangent operation (tan)",
                Math.Tan);
        public static readonly List<ISymbol> AllSymbols = new List<ISymbol>
        {
            Sine,
            Cosine,
            Tangent
        };
    }
    public static class MathematicalHyperbolicSymbols
    {

        public static readonly FunctionalSymbol<double> HyperbolicSine =
            SymbolFactory<double>.CreateUnary(
                "HyperbolicSine", "Hyperbolic sine operation (sinh)",
                Math.Sinh);

        public static readonly FunctionalSymbol<double> HyperbolicCosine =
            SymbolFactory<double>.CreateUnary(
                "HyperbolicCosine", "Hyperbolic cosine operation (cosh)",
                Math.Cosh);

        public static readonly FunctionalSymbol<double> HyperbolicTangent =
            SymbolFactory<double>.CreateUnary(
                "HyperbolicTangent", "Hyperbolic tangent operation (tanh)",
                Math.Tanh);
        public static readonly List<ISymbol> AllSymbols = new List<ISymbol>
        {
            HyperbolicSine,
            HyperbolicCosine,
            HyperbolicTangent
        };
    }
    //mathematica discreta 
    public static class MathematicalDiscreteSymbols
    {
        public static readonly FunctionalSymbol<int> Factorial =
            SymbolFactory<int>.CreateUnary(
                "Factorial", "Factorial operation (n!)",
                n =>
                {
                    if (n < 0) throw new ArgumentException("Negative numbers do not have factorials.");
                    return n == 0 ? 1 : Enumerable.Range(1, n).Aggregate(1, (acc, x) => acc * x);
                });

        public static readonly CompositeSymbol<int> Permutation =
            SymbolFactory<int>.CreateComposite(
                "Permutation", "Permutation operation (nPr)", 2,
                placeholders =>
                {
                    // Create P(n,r) node
                    var permutationNode = new SymbolicExpressionTreeNode(Factorial);
                    permutationNode.AddSubtree(placeholders[0]); // n
                    permutationNode.AddSubtree(placeholders[1]); // r

                    // Create r! node
                    var factorialNode = new SymbolicExpressionTreeNode(Factorial);
                    factorialNode.AddSubtree(placeholders[1]); // r

                    // Create division node: P(n,r) / r!
                    var divisionNode = new SymbolicExpressionTreeNode(
                        SymbolFactory<int>.CreateBinary("IntDivision", "Integer division", (a, b) => 
                        {
                            if (b == 0) throw new DivideByZeroException("Division by zero.");
                            return a / b;
                        }));
                    divisionNode.AddSubtree(permutationNode);
                    divisionNode.AddSubtree(factorialNode);

                    return divisionNode;
                },
                // Operation for evaluation (matches the subtree logic)
                args =>
                {
                    var n = args[0];
                    var r = args[1];
                    if (n < 0 || r < 0 || r > n)
                        throw new ArgumentException("Invalid values for permutation.");
                    
                    // P(n, r) = n! / (n - r)!
                    var numerator = Enumerable.Range(n - r + 1, r).Aggregate(1, (acc, x) => acc * x);
                    return numerator;
                });
        /// <summary>
        /// Combination symbol that generates a subtree representing P(n,r) / r!
        /// This creates a composite structure that can be mutated and crossed over at the component level.
        /// </summary>
        public static readonly CompositeSymbol<int> Combination =
            SymbolFactory<int>.CreateComposite(
                "Combination", "Combination operation (nCr) as P(n,r) / r!", 2,
                placeholders =>
                {
                    // Create P(n,r) node
                    var permutationNode = new SymbolicExpressionTreeNode(Permutation);
                    permutationNode.AddSubtree(placeholders[0]); // n
                    permutationNode.AddSubtree(placeholders[1]); // r

                    // Create r! node
                    var factorialNode = new SymbolicExpressionTreeNode(Factorial);
                    factorialNode.AddSubtree(placeholders[1]); // r

                    // Create division node: P(n,r) / r!
                    var divisionNode = new SymbolicExpressionTreeNode(
                        SymbolFactory<int>.CreateBinary("IntDivision", "Integer division", (a, b) => 
                        {
                            if (b == 0) throw new DivideByZeroException("Division by zero.");
                            return a / b;
                        }));
                    divisionNode.AddSubtree(permutationNode);
                    divisionNode.AddSubtree(factorialNode);

                    return divisionNode;
                },
                // Operation for evaluation (matches the subtree logic)
                args =>
                {
                    var n = args[0];
                    var r = args[1];
                    if (n < 0 || r < 0 || r > n)
                        throw new ArgumentException("Invalid values for combination.");
                    
                    // C(n, r) = P(n, r) / r!
                    var permutationValue = Enumerable.Range(n - r + 1, r).Aggregate(1, (acc, x) => acc * x);
                    var factorialValue = r == 0 ? 1 : Enumerable.Range(1, r).Aggregate(1, (acc, x) => acc * x);
                    return permutationValue / factorialValue;
                });

        public static readonly List<ISymbol> AllSymbols = new List<ISymbol>
        {
            Factorial,
            Permutation,
            Combination
        };
    }
}
