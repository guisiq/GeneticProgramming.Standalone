using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    public class MutationOperatorTests
    {
        private readonly SymbolicRegressionGrammar _grammar = new SymbolicRegressionGrammar();
        private readonly MersenneTwister _random = new MersenneTwister(42);
        private readonly GrowTreeCreator _creator = new GrowTreeCreator();

        [Fact]
        public void Mutation_ProducesValidMutatedTrees()
        {
            var tree = _creator.CreateTree(_random, _grammar, 20, 5);
            var mutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = _grammar };

            var mutated = mutator.Mutate(_random, tree);

            Assert.NotNull(mutated);
            Assert.NotNull(mutated.Root);
            Assert.True(mutated.Length > 0);
            Assert.True(mutated.Depth > 0);
        }

        [Fact]
        public void Mutation_MutatedTreeIsDifferentFromOriginal()
        {
            var tree = _creator.CreateTree(_random, _grammar, 20, 5);
            var mutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = _grammar };

            var mutated = mutator.Mutate(_random, tree);

            Assert.NotEqual(tree.Length, mutated.Length);
        }

        [Fact]
        public void Mutation_HandlesEdgeCasesWithSmallTree()
        {
            var tree = _creator.CreateTree(_random, _grammar, 1, 1);
            var mutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = _grammar };

            var mutated = mutator.Mutate(_random, tree);

            Assert.NotNull(mutated.Root);
            Assert.True(mutated.Length > 0);
        }
    }
}
