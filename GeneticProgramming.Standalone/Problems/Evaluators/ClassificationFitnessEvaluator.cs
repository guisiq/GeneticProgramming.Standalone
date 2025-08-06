using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Evaluates trees for simple classification problems using accuracy.
    /// Predictions greater than 0.5 are treated as class 1.
    /// </summary>
    public class ClassificationFitnessEvaluator : IFitnessEvaluator<double>
    {
        private readonly double[][] _inputs;
        private readonly int[] _targets;
        private readonly string[] _variableNames;
        private readonly ExpressionInterpreter _interpreter = new();
        

        public ClassificationFitnessEvaluator(double[][] inputs, int[] targets, string[] variableNames)
        {
            _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            _targets = targets ?? throw new ArgumentNullException(nameof(targets));
            _variableNames = variableNames ?? throw new ArgumentNullException(nameof(variableNames));
            if (_inputs.Length != _targets.Length)
                throw new ArgumentException("Inputs and targets must have same length");
        }


        public double Evaluate(ISymbolicExpressionTree<double> tree)
        {
            int correct = 0;
            var vars = new Dictionary<string, double>();
            for (int i = 0; i < _inputs.Length; i++)
            {
                vars.Clear();
                for (int j = 0; j < _variableNames.Length; j++)
                    vars[_variableNames[j]] = _inputs[i][j];

                double prediction = _interpreter.Evaluate(tree, vars);
                int predClass = prediction >= 0.5 ? 1 : 0;
                if (predClass == _targets[i]) correct++;
            }
            return (double)correct / _inputs.Length;
        }
    }
}
