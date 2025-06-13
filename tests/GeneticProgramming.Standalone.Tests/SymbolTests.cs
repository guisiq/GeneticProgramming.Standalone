using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Symbols;
using Xunit;

namespace GeneticProgramming.Standalone.UnitTests.Expressions
{
    /// <summary>
    /// Unit tests for the base Symbol class using a concrete symbol implementation.
    /// </summary>
    public class SymbolTests
    {
        [Fact]
        public void DefaultProperties_AreInitializedCorrectly()
        {
            var symbol = new Addition();

            Assert.Equal("Addition", symbol.Name);
            Assert.Equal("Addition operation (+)", symbol.Description);
            Assert.Equal(1.0, symbol.InitialFrequency);
            Assert.Equal(1.0, symbol.Weight);
            Assert.True(symbol.Enabled);
        }

        [Fact]
        public void InitialFrequency_NegativeValue_Throws()
        {
            var symbol = new Addition();

            Assert.Throws<ArgumentOutOfRangeException>(() => symbol.InitialFrequency = -0.1);
        }

        [Fact]
        public void Weight_NegativeValue_Throws()
        {
            var symbol = new Addition();

            Assert.Throws<ArgumentOutOfRangeException>(() => symbol.Weight = -0.5);
        }

        [Fact]
        public void Enabled_PropertyChanged_FiresEvent()
        {
            var symbol = new Addition();
            bool eventFired = false;
            string? propertyName = null;

            symbol.PropertyChanged += (s, e) => { eventFired = true; propertyName = e.PropertyName; };

            symbol.Enabled = false;

            Assert.True(eventFired);
            Assert.Equal(nameof(Symbol.Enabled), propertyName);
            Assert.False(symbol.Enabled);
        }
    }
}
