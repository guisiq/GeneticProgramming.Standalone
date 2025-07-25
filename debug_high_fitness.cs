using System;
using System.Linq;
using GeneticProgramming.Playground.Services;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("🔍 Debugging High Fitness for Simple Tree...\n");
        
        // Load dataset
        var datasetService = new DatasetService();
        var (inputs, targets, variableNames) = await datasetService.LoadDatasetAsync("Handwritten Digits");
        
        // Show dataset stats
        Console.WriteLine($"Dataset Stats:");
        Console.WriteLine($"- Samples: {inputs.Length}");
        Console.WriteLine($"- Features: {inputs[0].Length}");
        Console.WriteLine($"- Variables: {string.Join(", ", variableNames.Take(5))}...");
        
        // Check class distribution
        var class0Count = targets.Count(t => t == 0);
        var class1Count = targets.Count(t => t == 1);
        Console.WriteLine($"- Class 0 (even): {class0Count} ({(double)class0Count/targets.Length:P})");
        Console.WriteLine($"- Class 1 (odd): {class1Count} ({(double)class1Count/targets.Length:P})");
        
        // Test the suspicious tree: Subtraction(Subtraction(Subtraction(Pixel_11, Pixel_1), Pixel_50), Pixel_10)
        Console.WriteLine($"\n🌳 Testing Suspicious Tree...");
        
        var evaluator = new ImprovedClassificationFitnessEvaluator(inputs, targets, variableNames, 0.001);
        
        // Create the tree manually
        var grammar = new SymbolicRegressionGrammar(variableNames, new[] { MathematicalSymbols.Subtraction }, false);
        var tree = CreateSuspiciousTree(grammar);
        
        Console.WriteLine($"Tree: {tree}");
        Console.WriteLine($"Tree String: {tree.ToTreeString()}");
        
        var fitness = evaluator.Evaluate(tree);
        Console.WriteLine($"Fitness: {fitness:F6}");
        
        // Manual evaluation on first few samples
        Console.WriteLine($"\n📊 Manual Predictions (first 10 samples):");
        var interpreter = new ExpressionInterpreter();
        var vars = new Dictionary<string, double>();
        
        for (int i = 0; i < Math.Min(10, inputs.Length); i++)
        {
            vars.Clear();
            for (int j = 0; j < variableNames.Length; j++)
                vars[variableNames[j]] = inputs[i][j];
            
            try
            {
                double prediction = interpreter.Evaluate(tree, vars);
                double normalized = 1.0 / (1.0 + Math.Exp(-prediction));
                int predictedClass = normalized > 0.5 ? 1 : 0;
                bool correct = predictedClass == targets[i];
                
                Console.WriteLine($"Sample {i}: pred={prediction:F3}, norm={normalized:F3}, class={predictedClass}, target={targets[i]}, correct={correct}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sample {i}: ERROR - {ex.Message}");
            }
        }
        
        // Test baseline - always predict majority class
        Console.WriteLine($"\n📏 Baseline Comparison:");
        int majorityClass = class0Count > class1Count ? 0 : 1;
        double baselineAccuracy = (double)Math.Max(class0Count, class1Count) / targets.Length;
        Console.WriteLine($"Always predict class {majorityClass}: {baselineAccuracy:P} accuracy");
        
        // Test simple constant predictions
        var constantTree = CreateConstantTree(grammar, 0.0); // Always predict 0
        var constantFitness = evaluator.Evaluate(constantTree);
        Console.WriteLine($"Constant 0 tree fitness: {constantFitness:F6}");
        
        var constantTree2 = CreateConstantTree(grammar, 10.0); // Always predict high value
        var constantFitness2 = evaluator.Evaluate(constantTree2);
        Console.WriteLine($"Constant 10 tree fitness: {constantFitness2:F6}");
    }
    
    static ISymbolicExpressionTree CreateSuspiciousTree(ISymbolicExpressionTreeGrammar grammar)
    {
        // Subtraction(Subtraction(Subtraction(Pixel_11, Pixel_1), Pixel_50), Pixel_10)
        var tree = new SymbolicExpressionTree();
        
        // Root: Subtraction
        var root = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        tree.Root = root;
        
        // Left child: Subtraction(Subtraction(Pixel_11, Pixel_1), Pixel_50)
        var sub1 = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        root.AddSubtree(sub1);
        
        // Left child of sub1: Subtraction(Pixel_11, Pixel_1)
        var sub2 = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        sub1.AddSubtree(sub2);
        
        // Pixel_11 and Pixel_1
        var pixel11 = new VariableTreeNode(grammar.GetVariableSymbol("Pixel_11"));
        var pixel1 = new VariableTreeNode(grammar.GetVariableSymbol("Pixel_1"));
        sub2.AddSubtree(pixel11);
        sub2.AddSubtree(pixel1);
        
        // Right child of sub1: Pixel_50
        var pixel50 = new VariableTreeNode(grammar.GetVariableSymbol("Pixel_50"));
        sub1.AddSubtree(pixel50);
        
        // Right child of root: Pixel_10
        var pixel10 = new VariableTreeNode(grammar.GetVariableSymbol("Pixel_10"));
        root.AddSubtree(pixel10);
        
        return tree;
    }
    
    static ISymbolicExpressionTree CreateConstantTree(ISymbolicExpressionTreeGrammar grammar, double value)
    {
        var tree = new SymbolicExpressionTree();
        var root = new ConstantTreeNode(value);
        tree.Root = root;
        return tree;
    }
}
