using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone
{
    /// <summary>
    /// Demo program to test CompositeSymbol functionality
    /// </summary>
    public class CompositeSymbolDemo
    {
        public static void TestCompositeSymbol()
        {
            Console.WriteLine("=== Teste do CompositeSymbol ===");

            // Teste Factorial
            var factorial = MathematicalDiscreteSymbols.Factorial;
            var variables = new Dictionary<string, int>();

            Console.WriteLine("Teste Factorial:");
            Console.WriteLine($"5! = {factorial.Evaluate(new[] { 5 }, variables)}"); // Esperado: 120

            // Teste Permutation
            var permutation = MathematicalDiscreteSymbols.Permutation;
            Console.WriteLine("Teste Permutation:");
            Console.WriteLine($"P(5,2) = {permutation.Evaluate(new[] { 5, 2 }, variables)}"); // Esperado: 20

            // Teste Combination (CompositeSymbol)
            var combination = MathematicalDiscreteSymbols.Combination;
            Console.WriteLine("Teste Combination (CompositeSymbol):");
            Console.WriteLine($"C(5,2) = {combination.Evaluate(new[] { 5, 2 }, variables)}"); // Esperado: 10

            // Teste da estrutura da árvore gerada
            Console.WriteLine("\nTeste da estrutura da árvore:");
            var treeNode = combination.CreateTreeNode();
            Console.WriteLine($"Root symbol: {treeNode.Symbol.Name}"); // Esperado: IntDivision
            Console.WriteLine($"Subtree count: {treeNode.SubtreeCount}"); // Esperado: 2

            if (treeNode.SubtreeCount >= 2)
            {
                var leftChild = treeNode.GetSubtree(0);
                var rightChild = treeNode.GetSubtree(1);
                Console.WriteLine($"Left child: {leftChild.Symbol.Name}"); // Esperado: Permutation
                Console.WriteLine($"Right child: {rightChild.Symbol.Name}"); // Esperado: Factorial
            }

            Console.WriteLine("\n=== Testes concluídos ===");
        }
    }
}
