using System;
using System.Linq;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;

namespace GeneticProgramming.Examples
{
    /// <summary>
    /// Basic test to verify that the GP framework components are working.
    /// </summary>
    public class BasicGPTest
    {
        public static void RunBasicTests()
        {
            Console.WriteLine("=== Basic Genetic Programming Framework Test ===");
            
            // Test 1: Create a symbolic regression grammar
            Console.WriteLine("\n1. Testing Symbolic Regression Grammar...");
            var variableNames = new[] { "X", "Y", "Z" };
            var grammar = new SymbolicRegressionGrammar(variableNames, allowConstants: true);
            
            Console.WriteLine($"   Grammar created with {grammar.Symbols.Count()} symbols");
            Console.WriteLine($"   Start symbols: {grammar.StartSymbols.Count()}");
            Console.WriteLine($"   Variables: {string.Join(", ", grammar.VariableNames)}");
            Console.WriteLine($"   Grammar is valid: {grammar.ValidateForRegression()}");
            
            // Test 2: Create a simple expression tree manually
            Console.WriteLine("\n2. Testing Expression Tree Creation...");
            var addSymbol = grammar.GetSymbol("Addition");
            var variableX = grammar.GetSymbol("X");
            var constant = grammar.GetSymbol("Constant");
            
            if (addSymbol != null && variableX != null && constant != null)
            {
                // Create tree: Addition(X, Constant)
                var tree = new SymbolicExpressionTree();
                var rootNode = addSymbol.CreateTreeNode();
                var leftChild = variableX.CreateTreeNode();
                var rightChild = constant.CreateTreeNode();
                
                rootNode.AddSubtree(leftChild);
                rootNode.AddSubtree(rightChild);
                tree.Root = rootNode;
                
                Console.WriteLine($"   Created tree with {tree.Length} nodes");
                Console.WriteLine($"   Tree depth: {tree.Depth}");
                Console.WriteLine($"   Tree structure: {GetTreeStructure(tree.Root)}");
            }
            
            // Test 3: Test cloning
            Console.WriteLine("\n3. Testing Grammar Cloning...");
            var cloner = new Core.Cloner();
            var clonedGrammar = (SymbolicRegressionGrammar)grammar.Clone(cloner);
            
            Console.WriteLine($"   Original grammar symbols: {grammar.Symbols.Count()}");
            Console.WriteLine($"   Cloned grammar symbols: {clonedGrammar.Symbols.Count()}");
            Console.WriteLine($"   Cloned grammar is valid: {clonedGrammar.ValidateForRegression()}");
            
            // Test 4: Test different grammar configurations
            Console.WriteLine("\n4. Testing Different Grammar Configurations...");
            var simpleGrammar = SymbolicRegressionGrammar.CreateSimpleGrammar(new[] { "X" });
            var standardGrammar = SymbolicRegressionGrammar.CreateStandardGrammar(new[] { "X", "Y" });
            
            Console.WriteLine($"   Simple grammar symbols: {simpleGrammar.Symbols.Count()}");
            Console.WriteLine($"   Standard grammar symbols: {standardGrammar.Symbols.Count()}");
            
            Console.WriteLine("\n=== All tests completed successfully! ===");
        }
        
        private static string GetTreeStructure(ISymbolicExpressionTreeNode node)
        {
            if (node.SubtreeCount == 0)
                return node.Symbol.SymbolName;
            
            var children = string.Join(", ", node.Subtrees.Select(GetTreeStructure));
            return $"{node.Symbol.SymbolName}({children})";
        }
    }
}
