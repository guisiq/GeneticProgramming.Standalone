using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Compilation;
using GeneticProgramming.Standalone.Performance;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Evaluates trees for regression problems using mean squared error.
    /// Supports parallel evaluation for large datasets.
    /// </summary>
    public class RegressionFitnessEvaluator : IParallelizableFitnessEvaluator<double>
    {
        private readonly double[][] _inputs;
        private readonly double[] _targets;
        private readonly ExpressionInterpreter _interpreter = new();
    private readonly TreeCompiler<double> _treeCompiler = new();
        private readonly string[] _variableNames;
        
        // Controlar paralelização
        private bool _enableParallelEvaluation = true;
        private int _parallelThreshold = 100;
        private EvaluationStatistics _lastStatistics = new();
        
        /// <summary>
        /// Gets or sets whether parallel evaluation is enabled for sample processing.
        /// Default is true. Automatically disabled for small datasets (&lt; 100 samples).
        /// </summary>
        public bool EnableParallelEvaluation
        {
            get => _enableParallelEvaluation;
            set => _enableParallelEvaluation = value;
        }
        
        /// <summary>
        /// Gets or sets the minimum number of samples required to enable parallel processing.
        /// Default is 100. Below this threshold, sequential processing is used.
        /// </summary>
        public int ParallelThreshold
        {
            get => _parallelThreshold;
            set => _parallelThreshold = Math.Max(1, value);
        }
        
        /// <summary>
        /// Gets statistics about the last evaluation performed.
        /// </summary>
        public EvaluationStatistics GetLastEvaluationStatistics()
        {
            return _lastStatistics;
        }

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
            var sw = Stopwatch.StartNew();
            
            // Decidir se usa paralelização baseado no tamanho do dataset
            bool useParallel = _enableParallelEvaluation && _inputs.Length >= _parallelThreshold;
            
            double result;
            if (useParallel)
            {
                result = EvaluateParallel(tree);
            }
            else
            {
                result = EvaluateSequential(tree);
            }
            
            sw.Stop();
            
            // Atualizar estatísticas
            _lastStatistics = new EvaluationStatistics
            {
                UsedParallelProcessing = useParallel,
                SamplesEvaluated = _inputs.Length,
                EvaluationTime = sw.Elapsed,
                ThreadsUsed = useParallel ? Environment.ProcessorCount : 1
            };
            
            return result;
        }

        /// <summary>
        /// Avaliação sequencial (original) - usada para datasets pequenos.
        /// </summary>
        private double EvaluateSequential(ISymbolicExpressionTree<double> tree)
        {
            double mse = 0.0;
            var vars = ObjectPools.RentDictionary();
            var compiled = GetCompiledEvaluator(tree);
            
            try
            {
                for (int i = 0; i < _inputs.Length; i++)
                {
                    vars.Clear();
                    for (int j = 0; j < _variableNames.Length; j++)
                        vars[_variableNames[j]] = _inputs[i][j];
                    
                    double prediction = compiled(vars);
                    double error = prediction - _targets[i];
                    mse += error * error;
                }
                
                mse /= _inputs.Length;
                return -mse;
            }
            finally
            {
                ObjectPools.ReturnDictionary(vars);
            }
        }

        /// <summary>
        /// Avaliação paralela - usada para datasets grandes.
        /// Processa múltiplas amostras simultaneamente em threads diferentes.
        /// </summary>
        private double EvaluateParallel(ISymbolicExpressionTree<double> tree)
        {
            // Array para armazenar erros de cada amostra
            var errors = new double[_inputs.Length];
            var compiled = GetCompiledEvaluator(tree);
            
            // Processar amostras em paralelo
            Parallel.For(0, _inputs.Length, i =>
            {
                var vars = ObjectPools.RentDictionary();
                try
                {
                    for (int j = 0; j < _variableNames.Length; j++)
                        vars[_variableNames[j]] = _inputs[i][j];
                    
                    double prediction = compiled(vars);
                    errors[i] = prediction - _targets[i];
                }
                finally
                {
                    ObjectPools.ReturnDictionary(vars);
                }
            });
            
            // Calcular MSE a partir dos erros
            double sumSquaredErrors = 0.0;
            for (int i = 0; i < errors.Length; i++)
            {
                sumSquaredErrors += errors[i] * errors[i];
            }
            
            double mse = sumSquaredErrors / _inputs.Length;
            return -mse;
        }

        private Func<IDictionary<string, double>, double> GetCompiledEvaluator(ISymbolicExpressionTree<double> tree)
        {
            try
            {
                return _treeCompiler.Compile(tree);
            }
            catch
            {
                return vars => _interpreter.Evaluate(tree, vars);
            }
        }
    }
}
