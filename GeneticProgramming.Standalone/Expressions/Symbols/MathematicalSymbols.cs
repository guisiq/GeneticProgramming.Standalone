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

        public static readonly FunctionalSymbol<double> Abs =
            SymbolFactory<double>.CreateUnary(
                "Abs", "Absolute value",
                Math.Abs);

        public static readonly FunctionalSymbol<double> Square =
            SymbolFactory<double>.CreateUnary(
                "Square", "Square (x²)",
                x => x * x);

        public static readonly FunctionalSymbol<double> SquareRoot =
            SymbolFactory<double>.CreateUnary(
                "SquareRoot", "Protected square root",
                x => x < 0 ? 0.0 : Math.Sqrt(x));

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            Addition,
            Subtraction,
            Multiplication,
            ProtectedDivision,
            Abs,
            Square,
            SquareRoot
        };
    }

    public static class MathematicalLogarithmicSymbols
    {


        public static readonly FunctionalSymbol<double> Logarithm =
            SymbolFactory<double>.CreateBinary(
                "Logarithm", "Protected logarithm operation (log)",
                (a, b) =>
                {
                    // domínio inválido? retorna 0 em vez de exception
                    if (a <= 0 || b <= 0 || Math.Abs(a - 1.0) < double.Epsilon)
                        return 0.0;
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
        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
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
        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
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

        public static readonly FunctionalSymbol<double> Sigmoid =
            SymbolFactory<double>.CreateUnary(
                "Sigmoid", "Sigmoid function (1 / (1 + exp(-x)))",
                x => 1.0 / (1.0 + Math.Exp(-x)));

        public static readonly List<ISymbol<double>> AllSymbols = new List<ISymbol<double>>
        {
            HyperbolicSine,
            HyperbolicCosine,
            HyperbolicTangent,
            Sigmoid
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
                    var permutationNode = new SymbolicExpressionTreeNode<int>(Factorial);
                    permutationNode.AddSubtree(placeholders[0]); // n
                    permutationNode.AddSubtree(placeholders[1]); // r

                    // Create r! node
                    var factorialNode = new SymbolicExpressionTreeNode<int>(Factorial);
                    factorialNode.AddSubtree(placeholders[1]); // r

                    // Create division node: P(n,r) / r!
                    var divisionNode = new SymbolicExpressionTreeNode<int>(
                        SymbolFactory<int>.CreateBinary("IntDivision", "Integer division", (a, b) => 
                        {
                            if (b == 0) throw new DivideByZeroException("Division by zero.");
                            return a / b;
                        }));
                    divisionNode.AddSubtree(permutationNode);
                    divisionNode.AddSubtree(factorialNode);

                    return divisionNode;
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
                    var permutationNode = new SymbolicExpressionTreeNode<int>(Permutation);
                    permutationNode.AddSubtree(placeholders[0]); // n
                    permutationNode.AddSubtree(placeholders[1]); // r

                    // Create r! node
                    var factorialNode = new SymbolicExpressionTreeNode<int>(Factorial);
                    factorialNode.AddSubtree(placeholders[1]); // r

                    // Create division node: P(n,r) / r!
                    var divisionNode = new SymbolicExpressionTreeNode<int>(
                        SymbolFactory<int>.CreateBinary("IntDivision", "Integer division", (a, b) => 
                        {
                            if (b == 0) throw new DivideByZeroException("Division by zero.");
                            return a / b;
                        }));
                    divisionNode.AddSubtree(permutationNode);
                    divisionNode.AddSubtree(factorialNode);

                    return divisionNode;
                });

        public static readonly List<ISymbol<int>> AllSymbols = new List<ISymbol<int>>
        {
            Factorial,
            Permutation,
            Combination
        };
    }
}
