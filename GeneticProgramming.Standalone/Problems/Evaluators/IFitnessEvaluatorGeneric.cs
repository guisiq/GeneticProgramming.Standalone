using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Problems.Evaluators
{
    /// <summary>
    /// Generic interface for evaluating the fitness of a symbolic expression tree.
    /// </summary>
    /// <typeparam name="T">The data type used in the expressions</typeparam>
    public interface IFitnessEvaluator<T> where T : struct
    {
        /// <summary>
        /// Gets the average calculator function for the specified type.
        /// Uses the cached implementation from AverageCalculatorCache by default.
        /// </summary>
        Func<IEnumerable<T>, T> AverageCalculator => AverageCalculatorCache.Instance<T>();

        /// <summary>
        /// Computes the fitness of the given tree. Higher is better.
        /// </summary>
        T Evaluate(ISymbolicExpressionTree<T> tree);
    }

    /// <summary>
    /// Cache estático por tipo para calculadores de média
    /// </summary>
    /// <typeparam name="T">Tipo para o qual criar o calculador</typeparam>
    internal static class AverageCalculatorCache
    {
        // cache único de Type -> boxed Func<IEnumerable<T>, T>
        private static readonly ConcurrentDictionary<Type, object> _cache 
            = new ConcurrentDictionary<Type, object>();

        static AverageCalculatorCache()
        {
            // registrar os calculadores built-in
            Register<double>(values => values.Average());
            Register<float>(values => values.Average());
            Register<decimal>(values => values.Average());
            Register<int>(values => (int)values.Average(v => v));
            Register<long>(values => (long)values.Average(v => v));
            Register<short>(values => (short)values.Average(v => (double)v));
            Register<byte>(values => (byte)values.Average(v => (double)v));
            Register<uint>(values => (uint)values.Average(v => (double)v));
            Register<ulong>(values => (ulong)values.Average(v => (double)v));
            Register<ushort>(values => (ushort)values.Average(v => (double)v));
            Register<sbyte>(values => (sbyte)values.Average(v => (double)v));
        }

        /// <summary>
        /// Retorna (ou cria por fallback) o calculator para T.
        /// </summary>
        public static Func<IEnumerable<T>, T> Instance<T>() where T : struct
        {
            var type = typeof(T);
            if (_cache.TryGetValue(type, out var boxed))
                return (Func<IEnumerable<T>, T>)boxed;

            // fallback: retorna primeiro elemento (ou default)
            Func<IEnumerable<T>, T> def = vals => vals.FirstOrDefault();
            _cache[type] = def;
            return def;
        }

        /// <summary>
        /// Permite adicionar/override de calculadores para um tipo específico.
        /// </summary>
        public static void Register<T>(Func<IEnumerable<T>, T> calculator) where T : struct
        {
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            _cache[typeof(T)] = calculator;
        }
    }
}
