using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Evaluates trees for regression problems using mean squared error.
    /// </summary>
    public class RegressionFitnessEvaluator : IFitnessEvaluator<double>
    {
        private readonly double[][] _inputs;
        private readonly double[] _targets;
        private readonly ExpressionInterpreter _interpreter = new();
        private readonly string[] _variableNames;

        public RegressionFitnessEvaluator(double[][] inputs, double[] targets, string[] variableNames)
        {
            _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            _targets = targets ?? throw new ArgumentNullException(nameof(targets));
            _variableNames = variableNames ?? throw new ArgumentNullException(nameof(variableNames));
            if (_inputs.Length != _targets.Length)
                throw new ArgumentException("Inputs and targets must have same length");
        }
        /// <summary>
        /// Avalia o fitness de uma árvore para problemas de regressão usando erro quadrático médio.
        /// Agora suporta avaliação dinâmica quando a árvore implementa IEvaluable.
        /// </summary>
        /// <param name="tree">Árvore a ser avaliada</param>
        /// <returns>Fitness da árvore (erro negativo para maximização)</returns>
        public double Evaluate(ISymbolicExpressionTree<double> tree)
        {
            double mse = 0.0;
            var vars = new Dictionary<string, double>();
            
            for (int i = 0; i < _inputs.Length; i++)
            {
                vars.Clear();
                for (int j = 0; j < _variableNames.Length; j++)
                    vars[_variableNames[j]] = _inputs[i][j];
                double prediction = _interpreter.Evaluate(tree, vars);
                double error = prediction - _targets[i];
                mse += error * error;
            }
            
            mse /= _inputs.Length;
            return -mse; // minimize error => maximize negative error
        }


    }
}
