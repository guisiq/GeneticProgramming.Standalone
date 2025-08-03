using GeneticProgramming.Abstractions.Operators;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;
using System.Collections.Generic;

namespace GeneticProgramming.Operators
{
    /// <summary>
    /// Selects individuals using tournament selection.
    /// </summary>
    public class TournamentSelector : Item, ISymbolicExpressionTreeSelector
    {
        private int _tournamentSize = 3;

        /// <summary>
        /// Number of individuals participating in each tournament.
        /// </summary>
        public int TournamentSize
        {
            get => _tournamentSize;
            set
            {
                if (_tournamentSize != value)
                {
                    _tournamentSize = Math.Max(1, value);
                    OnPropertyChanged(nameof(TournamentSize));
                }
            }
        }

        public TournamentSelector() : base() { }

        protected TournamentSelector(TournamentSelector original, Cloner cloner) : base(original, cloner)
        {
            _tournamentSize = original._tournamentSize;
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new TournamentSelector(this, cloner);
        }

        /// <inheritdoc />
        public ISymbolicExpressionTree<T> Select<T>(IRandom random, IList<ISymbolicExpressionTree<T>> population, Func<ISymbolicExpressionTree<T>, T> fitness)
            where T : struct, IComparable<T>, IEquatable<T>
        {
            if (population == null || population.Count == 0)
                throw new ArgumentException("Population cannot be empty", nameof(population));
            if (random == null) throw new ArgumentNullException(nameof(random));
            if (fitness == null) throw new ArgumentNullException(nameof(fitness));

            ISymbolicExpressionTree<T>? best = null;
            T bestFitness = default(T);
            bool firstCandidate = true;

            for (int i = 0; i < _tournamentSize; i++)
            {
                var candidate = population[random.Next(population.Count)];
                var fit = fitness(candidate);

                if (firstCandidate || fit.CompareTo(bestFitness) > 0)
                {
                    bestFitness = fit;
                    best = candidate;
                    firstCandidate = false;
                }
            }

            return (ISymbolicExpressionTree<T>)best!.Clone(new Cloner());
        }

    }
}
