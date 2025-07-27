using System;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;

namespace GeneticProgramming.Core
{
    /// <summary>
    /// Random number generator implementation
    /// </summary>
    [Item("Random", "Random number generator")]
    public class MersenneTwister : Item, IRandom
    {
        private Random random;
        private int seed;

        public MersenneTwister() : base()
        {
            seed = Environment.TickCount;
            random = new Random(seed);
        }

        public MersenneTwister(int seed) : base()
        {
            this.seed = seed;
            random = new Random(seed);
        }

        protected MersenneTwister(MersenneTwister original, Cloner cloner) : base(original, cloner)
        {
            seed = original.seed;
            random = new Random(seed);
        }

        public int Next()
        {
            return random.Next();
        }

        public int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }

        public void Reset()
        {
            Reset(this.seed);
        }

        public void Reset(int newSeed)
        {
            seed = newSeed;
            random = new Random(seed);
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new MersenneTwister(this, cloner);
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new MersenneTwister(this, cloner);
        }
    }
}
