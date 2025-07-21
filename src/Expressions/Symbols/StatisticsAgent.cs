using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Expressions.Symbols
{
    /// <summary>
    /// Agente para operações estatísticas em coleções numéricas.
    /// </summary>
    public static class StatisticsAgent
    {
        /// <summary>
        /// Calcula a média de uma sequência de números.
        /// </summary>
        public static double Mean(IEnumerable<double> data)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Os dados não podem ser nulos ou vazios.");
            return data.Average();
        }

        /// <summary>
        /// Calcula a variância de uma sequência de números.
        /// </summary>
        public static double Variance(IEnumerable<double> data)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Os dados não podem ser nulos ou vazios.");
            double mean = data.Average();
            return data.Sum(d => (d - mean) * (d - mean)) / data.Count();
        }

        /// <summary>
        /// Calcula a mediana de uma sequência de números.
        /// </summary>
        public static double Median(IEnumerable<double> data)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Os dados não podem ser nulos ou vazios.");
            var sorted = data.OrderBy(x => x).ToArray();
            int count = sorted.Length;
            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            else
            {
                return sorted[count / 2];
            }
        }
    }
}
