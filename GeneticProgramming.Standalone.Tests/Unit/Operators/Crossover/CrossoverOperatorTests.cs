using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using System.Linq;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    public class CrossoverOperatorTests
    {
        private readonly SymbolicRegressionGrammar _grammar = new SymbolicRegressionGrammar();
        private readonly MersenneTwister _random = new MersenneTwister(42);
        private readonly GrowTreeCreator _creator = new GrowTreeCreator();

        [Fact]
        public void Crossover_ProducesValidOffspring()
        {
            var parent1 = _creator.CreateTree(_random, _grammar, 20, 5);
            var parent2 = _creator.CreateTree(_random, _grammar, 20, 5);
            var crossover = new SubtreeCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(_random, parent1, parent2);

            Assert.NotNull(offspring);
            Assert.NotNull(offspring.Root);
            Assert.True(offspring.Length > 0);
            Assert.True(offspring.Depth > 0);
        }

        [Fact]
        public void Crossover_OffspringAreDifferentFromParents()
        {
            var parent1 = _creator.CreateTree(_random, _grammar, 20, 5);
            var parent2 = _creator.CreateTree(_random, _grammar, 20, 5);
            var crossover = new SubtreeCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(_random, parent1, parent2);

            Assert.NotSame(parent1.Root, offspring.Root);
            Assert.NotSame(parent2.Root, offspring.Root);
        }

        [Fact]
        public void Crossover_HandlesEdgeCasesWithSmallTrees()
        {
            var parent1 = _creator.CreateTree(_random, _grammar, 1, 1);
            var parent2 = _creator.CreateTree(_random, _grammar, 1, 1);
            var crossover = new SubtreeCrossover { SymbolicExpressionTreeGrammar = _grammar };

            var offspring = crossover.Crossover(_random, parent1, parent2);

            Assert.NotNull(offspring);
            Assert.NotNull(offspring.Root);
            Assert.True(offspring.Length > 0);
        }
    }
}
