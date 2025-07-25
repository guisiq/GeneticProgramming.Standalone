using System;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using GeneticProgramming.Core;

class Program
{
    static void Main()
    {
        // Test symbol names
        Console.WriteLine("Testing symbol names:");
        Console.WriteLine($"Addition: {MathematicalSymbols.Addition.SymbolName}");
        Console.WriteLine($"Subtraction: {MathematicalSymbols.Subtraction.SymbolName}");
        Console.WriteLine($"Multiplication: {MathematicalSymbols.Multiplication.SymbolName}");
        Console.WriteLine($"ProtectedDivision: {MathematicalSymbols.ProtectedDivision.SymbolName}");
        
        // Create a simple tree and test ToTreeString
        var variableNames = new[] { "x", "y" };
        var symbols = new[] { 
            MathematicalSymbols.Addition,
            MathematicalSymbols.Multiplication
        };
        
        var grammar = new SymbolicRegressionGrammar(variableNames, symbols, true);
        var creator = new GrowTreeCreator();
        var random = new MersenneTwister(42);
        
        var tree = creator.CreateTree(random, grammar, 20, 3);
        
        Console.WriteLine("\nTree structure:");
        Console.WriteLine(tree.ToTreeString());
    }
}
