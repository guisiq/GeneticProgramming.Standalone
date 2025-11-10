using System;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Interface para evaluators de fitness que suportam processamento paralelo de amostras.
    /// Estende IFitnessEvaluator com controles de paralelização.
    /// </summary>
    /// <typeparam name="T">Tipo de valor do fitness</typeparam>
    public interface IParallelizableFitnessEvaluator<T> : IFitnessEvaluator<T> 
        where T : notnull, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets whether parallel evaluation is enabled for sample processing.
        /// When true, samples are processed in parallel using multiple threads.
        /// When false, samples are processed sequentially.
        /// </summary>
        bool EnableParallelEvaluation { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum number of samples required to enable parallel processing.
        /// If the dataset has fewer samples than this threshold, sequential processing is used
        /// to avoid thread overhead. Default is typically 100 samples.
        /// </summary>
        int ParallelThreshold { get; set; }
        
        /// <summary>
        /// Gets statistics about the last evaluation performed.
        /// Useful for monitoring and debugging parallel performance.
        /// </summary>
        EvaluationStatistics GetLastEvaluationStatistics();
    }
    
    /// <summary>
    /// Estatísticas sobre a execução de uma avaliação.
    /// </summary>
    public class EvaluationStatistics
    {
        /// <summary>
        /// Indica se a última avaliação usou processamento paralelo.
        /// </summary>
        public bool UsedParallelProcessing { get; set; }
        
        /// <summary>
        /// Número de amostras avaliadas.
        /// </summary>
        public int SamplesEvaluated { get; set; }
        
        /// <summary>
        /// Tempo total de avaliação.
        /// </summary>
        public TimeSpan EvaluationTime { get; set; }
        
        /// <summary>
        /// Número de threads utilizadas (se paralelo).
        /// </summary>
        public int ThreadsUsed { get; set; }
        
        /// <summary>
        /// Taxa de avaliações por segundo.
        /// </summary>
        public double SamplesPerSecond => 
            EvaluationTime.TotalSeconds > 0 
                ? SamplesEvaluated / EvaluationTime.TotalSeconds 
                : 0;
        
        public override string ToString()
        {
            var mode = UsedParallelProcessing ? "Parallel" : "Sequential";
            return $"{mode}: {SamplesEvaluated} samples in {EvaluationTime.TotalMilliseconds:F2}ms " +
                   $"({SamplesPerSecond:F0} samples/sec, {ThreadsUsed} threads)";
        }
    }
}
