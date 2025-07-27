using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Debug version of classification fitness evaluator that provides detailed metrics
    /// </summary>
    public class DebugClassificationFitnessEvaluator : IFitnessEvaluator
    {
        private readonly double[][] _inputs;
        private readonly int[] _targets;
        private readonly string[] _variableNames;
        private readonly ExpressionInterpreter _interpreter = new();
        
        public DebugClassificationFitnessEvaluator(double[][] inputs, int[] targets, string[] variableNames)
        {
            _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            _targets = targets ?? throw new ArgumentNullException(nameof(targets));
            _variableNames = variableNames ?? throw new ArgumentNullException(nameof(variableNames));
            if (_inputs.Length != _targets.Length)
                throw new ArgumentException("Inputs and targets must have same length");
        }

        public double Evaluate(ISymbolicExpressionTree tree)
        {
            int correct = 0;
            int totalPredictions = 0;
            var predictions = new List<double>();
            var actualPredictions = new List<int>();
            var classDistribution = new Dictionary<int, int>();
            var predictionDistribution = new Dictionary<int, int>();
            
            var vars = new Dictionary<string, double>();
            
            for (int i = 0; i < _inputs.Length; i++)
            {
                vars.Clear();
                for (int j = 0; j < _variableNames.Length; j++)
                    vars[_variableNames[j]] = _inputs[i][j];

                try
                {
                    double prediction = _interpreter.Evaluate(tree, vars);
                    predictions.Add(prediction);
                    
                    int predClass = prediction >= 0.5 ? 1 : 0;
                    actualPredictions.Add(predClass);
                    
                    // Count class distributions
                    classDistribution[_targets[i]] = classDistribution.GetValueOrDefault(_targets[i], 0) + 1;
                    predictionDistribution[predClass] = predictionDistribution.GetValueOrDefault(predClass, 0) + 1;
                    
                    if (predClass == _targets[i]) correct++;
                    totalPredictions++;
                }
                catch
                {
                    // Handle evaluation errors
                    predictions.Add(0.0);
                    actualPredictions.Add(0);
                    predictionDistribution[0] = predictionDistribution.GetValueOrDefault(0, 0) + 1;
                    totalPredictions++;
                }
            }
            
            double accuracy = totalPredictions > 0 ? (double)correct / totalPredictions : 0.0;
            
            // Print debug information (can be removed in production)
            Console.WriteLine($"[DEBUG] Accuracy: {accuracy:F3} ({correct}/{totalPredictions})");
            Console.WriteLine($"[DEBUG] Prediction range: [{predictions.Min():F3}, {predictions.Max():F3}]");
            Console.WriteLine($"[DEBUG] Class distribution: {string.Join(", ", classDistribution.Select(kv => $"{kv.Key}:{kv.Value}"))}");
            Console.WriteLine($"[DEBUG] Prediction distribution: {string.Join(", ", predictionDistribution.Select(kv => $"{kv.Key}:{kv.Value}"))}");
            
            return accuracy;
        }
    }
}
