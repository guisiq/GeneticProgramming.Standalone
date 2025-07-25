using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneticProgramming.Tests.Integration.EndToEnd;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("🔍 DEBUGGING FITNESS 0.991 ISSUE...\n");
        
        // Load normalized dataset
        var (inputs, originalTargets, variableNames) = await DatasetManager.GetDigitsDatasetAsync();
        
        // Convert to binary (even vs odd)
        var targets = new int[originalTargets.Length];
        for (int i = 0; i < originalTargets.Length; i++)
        {
            targets[i] = originalTargets[i] % 2;
        }
        
        Console.WriteLine($"Dataset Info:");
        Console.WriteLine($"- Total samples: {inputs.Length}");
        Console.WriteLine($"- Features: {inputs[0].Length}");
        Console.WriteLine($"- Pixel range: [{inputs.SelectMany(x => x).Min():F3}, {inputs.SelectMany(x => x).Max():F3}]");
        
        var class0 = targets.Count(t => t == 0);
        var class1 = targets.Count(t => t == 1);
        Console.WriteLine($"- Class 0: {class0} ({(double)class0/targets.Length:P})");
        Console.WriteLine($"- Class 1: {class1} ({(double)class1/targets.Length:P})");
        
        // Create the suspicious tree structure manually
        var grammar = new SymbolicRegressionGrammar(variableNames, new[] { MathematicalSymbols.Subtraction }, false);
        var tree = CreateSuspiciousTree();
        
        Console.WriteLine($"\n🌳 Suspicious Tree:");
        Console.WriteLine(tree.ToTreeString());
        
        // Test with ImprovedClassificationFitnessEvaluator
        var evaluator = new ImprovedClassificationFitnessEvaluator(inputs, targets, variableNames, 0.001);
        var fitness = evaluator.Evaluate(tree);
        
        Console.WriteLine($"\n📊 Fitness: {fitness:F6}");
        
        // Manual evaluation on samples to see what's happening
        Console.WriteLine($"\n🔬 Manual Analysis (first 20 samples):");
        var interpreter = new ExpressionInterpreter();
        var vars = new Dictionary<string, double>();
        
        double totalCorrect = 0;
        var predictions = new List<double>();
        var normalizedPreds = new List<double>();
        
        for (int i = 0; i < Math.Min(20, inputs.Length); i++)
        {
            vars.Clear();
            for (int j = 0; j < variableNames.Length; j++)
                vars[variableNames[j]] = inputs[i][j];
            
            try
            {
                // Calculate raw prediction: ((Pixel_60 - Pixel_13) - Pixel_15) - Pixel_46 - Pixel_48
                double pixel60 = inputs[i][60];
                double pixel13 = inputs[i][13];
                double pixel15 = inputs[i][15];
                double pixel46 = inputs[i][46];
                double pixel48 = inputs[i][48];
                
                double manualPred = ((pixel60 - pixel13) - pixel15) - pixel46 - pixel48;
                double treePred = interpreter.Evaluate(tree, vars);
                
                // Apply clamping and sigmoid like in evaluator
                double clampedPred = Math.Max(-10.0, Math.Min(10.0, treePred));
                double sigmoidPred = 1.0 / (1.0 + Math.Exp(-clampedPred));
                
                int predictedClass = sigmoidPred > 0.5 ? 1 : 0;
                bool correct = predictedClass == targets[i];
                if (correct) totalCorrect++;
                
                predictions.Add(treePred);
                normalizedPreds.Add(sigmoidPred);
                
                // Calculate fitness contribution
                double fitnessContrib = targets[i] == 1 ? sigmoidPred : (1.0 - sigmoidPred);
                
                Console.WriteLine($"Sample {i:D2}: " +
                    $"pixels=[{pixel60:F2},{pixel13:F2},{pixel15:F2},{pixel46:F2},{pixel48:F2}] " +
                    $"raw={treePred:F3} sigmoid={sigmoidPred:F3} " +
                    $"pred={predictedClass} target={targets[i]} " +
                    $"correct={correct} fitness_contrib={fitnessContrib:F3}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sample {i:D2}: ERROR - {ex.Message}");
            }
        }
        
        double accuracy = totalCorrect / Math.Min(20, inputs.Length);
        Console.WriteLine($"\n📈 Quick Stats (first 20 samples):");
        Console.WriteLine($"- Accuracy: {accuracy:P}");
        Console.WriteLine($"- Raw prediction range: [{predictions.Min():F3}, {predictions.Max():F3}]");
        Console.WriteLine($"- Sigmoid range: [{normalizedPreds.Min():F3}, {normalizedPreds.Max():F3}]");
        
        // Test baseline predictions
        Console.WriteLine($"\n🎯 Baseline Comparisons:");
        
        // Always predict majority class
        int majorityClass = class0 > class1 ? 0 : 1;
        double majorityFitness = majorityClass == 0 ? (double)class0/targets.Length : (double)class1/targets.Length;
        Console.WriteLine($"- Always predict class {majorityClass}: {majorityFitness:F3} accuracy");
        
        // Test constant tree (always output 0)
        var constantTree = CreateConstantTree(0.0);
        var constantFitness = evaluator.Evaluate(constantTree);
        Console.WriteLine($"- Constant 0 tree fitness: {constantFitness:F6}");
        
        // Test constant tree (always output +5)
        var constantTree2 = CreateConstantTree(5.0);
        var constantFitness2 = evaluator.Evaluate(constantTree2);
        Console.WriteLine($"- Constant +5 tree fitness: {constantFitness2:F6}");
        
        // Test constant tree (always output -5)
        var constantTree3 = CreateConstantTree(-5.0);
        var constantFitness3 = evaluator.Evaluate(constantTree3);
        Console.WriteLine($"- Constant -5 tree fitness: {constantFitness3:F6}");
        
        // Check if there's a pattern in the suspicious pixels
        Console.WriteLine($"\n🔍 Pixel Analysis:");
        var pixel60Values = inputs.Take(100).Select(x => x[60]).ToArray();
        var pixel13Values = inputs.Take(100).Select(x => x[13]).ToArray();
        var pixel15Values = inputs.Take(100).Select(x => x[15]).ToArray();
        var pixel46Values = inputs.Take(100).Select(x => x[46]).ToArray();
        var pixel48Values = inputs.Take(100).Select(x => x[48]).ToArray();
        
        Console.WriteLine($"- Pixel_60: avg={pixel60Values.Average():F3}, std={StandardDeviation(pixel60Values):F3}");
        Console.WriteLine($"- Pixel_13: avg={pixel13Values.Average():F3}, std={StandardDeviation(pixel13Values):F3}");
        Console.WriteLine($"- Pixel_15: avg={pixel15Values.Average():F3}, std={StandardDeviation(pixel15Values):F3}");
        Console.WriteLine($"- Pixel_46: avg={pixel46Values.Average():F3}, std={StandardDeviation(pixel46Values):F3}");
        Console.WriteLine($"- Pixel_48: avg={pixel48Values.Average():F3}, std={StandardDeviation(pixel48Values):F3}");
    }
    
    static double StandardDeviation(double[] values)
    {
        double avg = values.Average();
        double sumSquaredDiffs = values.Sum(x => (x - avg) * (x - avg));
        return Math.Sqrt(sumSquaredDiffs / values.Length);
    }
    
    static ISymbolicExpressionTree CreateSuspiciousTree()
    {
        // From the image: Subtração -> Subtração -> Subtração -> Subtração -> Pixel_60, Pixel_13, Pixel_15, Pixel_46, Pixel_48
        // Structure: ((((Pixel_60 - Pixel_13) - Pixel_15) - Pixel_46) - Pixel_48)
        
        var tree = new SymbolicExpressionTree();
        
        // Root: Subtraction
        var root = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        tree.Root = root;
        
        // Left: Subtraction
        var sub1 = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        root.AddSubtree(sub1);
        
        // Left.Left: Subtraction  
        var sub2 = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        sub1.AddSubtree(sub2);
        
        // Left.Left.Left: Subtraction
        var sub3 = new SymbolicExpressionTreeNode(MathematicalSymbols.Subtraction);
        sub2.AddSubtree(sub3);
        
        // Pixels
        var pixel60 = new VariableTreeNode("Pixel_60");
        var pixel13 = new VariableTreeNode("Pixel_13");
        var pixel15 = new VariableTreeNode("Pixel_15");
        var pixel46 = new VariableTreeNode("Pixel_46");
        var pixel48 = new VariableTreeNode("Pixel_48");
        
        sub3.AddSubtree(pixel60);
        sub3.AddSubtree(pixel13);
        sub2.AddSubtree(pixel15);
        sub1.AddSubtree(pixel46);
        root.AddSubtree(pixel48);
        
        return tree;
    }
    
    static ISymbolicExpressionTree CreateConstantTree(double value)
    {
        var tree = new SymbolicExpressionTree();
        var root = new ConstantTreeNode(value);
        tree.Root = root;
        return tree;
    }
}
