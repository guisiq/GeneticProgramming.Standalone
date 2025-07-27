using GeneticProgramming.Core;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Integration tests for the OnePointCrossover operator.
    /// </summary>
    public class OnePointCrossoverIntegrationTests
    {
        private readonly SymbolicRegressionGrammar _grammar = new SymbolicRegressionGrammar();
        private readonly MersenneTwister _random = new MersenneTwister(21);
        private readonly GrowTreeCreator _creator = new GrowTreeCreator();

        /// <summary>
        /// OnePointCrossover should create a valid offspring tree.
        /// </summary>
        [Fact]
        public void OnePointCrossover_ProducesValidOffspring()
        {
            var parent1 = _creator.CreateTree(_random, _grammar, 15, 4);
            var parent2 = _creator.CreateTree(_random, _grammar, 15, 4);
            var crossover = new OnePointCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(_random, parent1, parent2);

            Assert.NotNull(offspring.Root);
            Assert.True(offspring.Length > 0);
        }

        /// <summary>
        /// Offspring generated should not share the same root node instance with parents.
        /// </summary>
        [Fact]
        public void OnePointCrossover_OffspringIsDifferentFromParents()
        {
            var parent1 = _creator.CreateTree(_random, _grammar, 15, 4);
            var parent2 = _creator.CreateTree(_random, _grammar, 15, 4);
            var crossover = new OnePointCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(_random, parent1, parent2);

            Assert.NotSame(parent1.Root, offspring.Root);
            Assert.NotSame(parent2.Root, offspring.Root);
        }
    }
}
