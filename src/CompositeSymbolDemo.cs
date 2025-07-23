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


            Console.WriteLine("\n=== Testes concluídos ===");
        }
    }
}
