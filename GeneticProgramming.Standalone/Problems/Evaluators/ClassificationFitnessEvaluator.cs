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
    /// Evaluates trees for simple classification problems using accuracy.
    /// Predictions greater than 0.5 are treated as class 1.
    /// Supports parallel evaluation for large datasets.
    /// </summary>
    public class ClassificationFitnessEvaluator : IParallelizableFitnessEvaluator<double>
    {
        private readonly double[][] _inputs;
        private readonly int[] _targets;
        private readonly string[] _variableNames;
        private readonly ExpressionInterpreter _interpreter = new();
    private readonly TreeCompiler<double> _treeCompiler = new();
        
        // Controlar paralelização
        private bool _enableParallelEvaluation = true;
        private int _parallelThreshold = 100;
        private EvaluationStatistics _lastStatistics = new();
        
        /// <summary>
        /// Gets or sets whether parallel evaluation is enabled for sample processing.
        /// </summary>
        public bool EnableParallelEvaluation
        {
            get => _enableParallelEvaluation;
            set => _enableParallelEvaluation = value;
        }
        
        /// <summary>
        /// Gets or sets the minimum number of samples required to enable parallel processing.
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
            var sw = Stopwatch.StartNew();
            
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
        /// Avaliação sequencial - usada para datasets pequenos.
        /// </summary>
        private double EvaluateSequential(ISymbolicExpressionTree<double> tree)
        {
            int correct = 0;
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
                    int predClass = prediction >= 0.5 ? 1 : 0;
                    if (predClass == _targets[i]) correct++;
                }
                
                return (double)correct / _inputs.Length;
            }
            finally
            {
                ObjectPools.ReturnDictionary(vars);
            }
        }

        /// <summary>
        /// Avaliação paralela - usada para datasets grandes.
        /// </summary>
        private double EvaluateParallel(ISymbolicExpressionTree<double> tree)
        {
            var predictions = new int[_inputs.Length];
            var compiled = GetCompiledEvaluator(tree);
            
            Parallel.For(0, _inputs.Length, i =>
            {
                var vars = ObjectPools.RentDictionary();
                try
                {
                    for (int j = 0; j < _variableNames.Length; j++)
                        vars[_variableNames[j]] = _inputs[i][j];

                    double prediction = compiled(vars);
                    predictions[i] = prediction >= 0.5 ? 1 : 0;
                }
                finally
                {
                    ObjectPools.ReturnDictionary(vars);
                }
            });
            
            int correct = 0;
            for (int i = 0; i < predictions.Length; i++)
            {
                if (predictions[i] == _targets[i]) correct++;
            }
            
            return (double)correct / _inputs.Length;
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
