using GeneticProgramming.Core;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Core
{
    public class MersenneTwisterTests
    {
        [Fact]
        public void Reset_WithNewSeed_ChangesSequence()
        {
            // Arrange
            var rng = new MersenneTwister(42);
            var beforeReset = rng.Next();

            // Act
            rng.Reset(100);
            var afterReset = rng.Next();

            // Assert - after resetting with new seed the next value should equal
            // the value produced by a new RNG initialized with the same seed.
            var expected = new MersenneTwister(100).Next();
            Assert.NotEqual(beforeReset, afterReset);
            Assert.Equal(expected, afterReset);
        }
    }
}
