using System;
using System.Collections.Generic;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Melhorado avaliador de fitness para classificação com gradiente suave e pressão de parcimônia
    /// </summary>
    public class ImprovedClassificationFitnessEvaluator : IFitnessEvaluator<double>
    {
        private readonly double[][] _inputs;
        private readonly int[] _targets;
        private readonly string[] _variableNames;
        private readonly ExpressionInterpreter _interpreter = new();
        private readonly double _parsimonyPressure;
        
        public ImprovedClassificationFitnessEvaluator(double[][] inputs, int[] targets, string[] variableNames, double parsimonyPressure = 0.001)
        {
            _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
            _targets = targets ?? throw new ArgumentNullException(nameof(targets));
            _variableNames = variableNames ?? throw new ArgumentNullException(nameof(variableNames));
            _parsimonyPressure = parsimonyPressure;
            if (_inputs.Length != _targets.Length)
                throw new ArgumentException("Inputs and targets must have same length");
        }

        public double Evaluate(ISymbolicExpressionTree<double> tree)
        {
            double totalScore = 0.0;
            var vars = new Dictionary<string, double>();
            int validPredictions = 0;
            
            for (int i = 0; i < _inputs.Length; i++)
            {
                vars.Clear();
                for (int j = 0; j < _variableNames.Length; j++)
                    vars[_variableNames[j]] = _inputs[i][j];

                try
                {
                    double prediction = _interpreter.Evaluate(tree, vars);
                    
                    // Evita valores infinitos ou NaN
                    if (double.IsNaN(prediction) || double.IsInfinity(prediction))
                    {
                        prediction = 0.0;
                    }
                    
                    // Fitness suave baseado na distância da predição ao target
                    double fitness = CalculateSmoothFitness(prediction, _targets[i]);
                    totalScore += fitness;
                    validPredictions++;
                }
                catch
                {
                    // Penaliza árvores que causam exceções
                    totalScore += 0.0;
                    validPredictions++;
                }
            }
            
            // Fitness médio
            double averageFitness = validPredictions > 0 ? totalScore / validPredictions : 0.0;
            
            // Aplica pressão de parcimônia (penaliza árvores grandes)
            double sizePenalty = tree.Length * _parsimonyPressure;
            
            return averageFitness - sizePenalty;
        }
        
        /// <summary>
        /// Calcula fitness suave que oferece gradiente mesmo para classificações incorretas
        /// </summary>
        private double CalculateSmoothFitness(double prediction, int target)
        {
            // Clamp prediction para evitar valores extremos na sigmoid
            prediction = Math.Max(-10.0, Math.Min(10.0, prediction));
            
            // Normaliza a predição usando sigmoid para manter entre 0 e 1
            double normalizedPrediction = 1.0 / (1.0 + Math.Exp(-prediction));
            
            if (target == 1)
            {
                // Para classe positiva, reward predições altas
                return normalizedPrediction;
            }
            else
            {
                // Para classe negativa, reward predições baixas
                return 1.0 - normalizedPrediction;
            }
        }
    }
}
