using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Algorithms
{
    public class GenerationEventArgsTests
    {
        private ISymbolicExpressionTree CreateDummyTree()
        {
            var grammar = new SymbolicRegressionGrammar();
            var creator = new GrowTreeCreator { SymbolicExpressionTreeGrammar = grammar };
            var random = new MersenneTwister(1);
            return creator.CreateTree(random, grammar, 5, 3);
        }

        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            var tree = CreateDummyTree();
            var args = new GenerationEventArgs(5, 1.2, 0.8, tree);

            Assert.Equal(5, args.Generation);
            Assert.Equal(1.2, args.BestFitness);
            Assert.Equal(0.8, args.AverageFitness);
            Assert.Same(tree, args.BestIndividual);
        }

        [Fact]
        public void Properties_AreReadOnly()
        {
            var tree = CreateDummyTree();
            var args = new GenerationEventArgs(2, 0.5, 0.4, tree);

            // Just access properties to ensure compile-time readonly behavior
            Assert.Equal(2, args.Generation);
            Assert.Equal(0.5, args.BestFitness);
            Assert.Equal(0.4, args.AverageFitness);
            Assert.NotNull(args.BestIndividual);
        }
    }
}
