using GeneticProgramming.Algorithms;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using Xunit;

namespace GeneticProgramming.Standalone.IntegrationTests.Algorithms
{
    /// <summary>
    /// Integration tests covering algorithm validation and stopping behavior.
    /// </summary>
    public class GeneticProgrammingAlgorithmValidationTests
    {
        /// <summary>
        /// Run should throw InvalidOperationException when required components are missing.
        /// </summary>
        [Fact]
        public void Run_MissingConfiguration_ThrowsInvalidOperationException()
        {
            var algorithm = new GeneticProgrammingAlgorithm();
            Assert.Throws<InvalidOperationException>(() => algorithm.Run());
        }

        /// <summary>
        /// Calling Stop during execution should halt the algorithm before reaching MaxGenerations.
        /// </summary>
        [Fact]
        public void Stop_HaltsExecutionMidRun()
        {
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = new SymbolicRegressionGrammar(),
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(1),
                PopulationSize = 5,
                MaxGenerations = 5,
                MaxTreeLength = 10,
                MaxTreeDepth = 3
            };

            int eventCount = 0;
            algorithm.GenerationCompleted += (s, e) =>
            {
                eventCount++;
                algorithm.Stop();
            };

            algorithm.Run();

            Assert.Equal(1, eventCount);
            Assert.Equal(1, algorithm.Generation);
        }
    }
}
