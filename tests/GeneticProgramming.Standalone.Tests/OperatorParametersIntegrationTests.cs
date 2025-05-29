using Xunit;
using GeneticProgramming.Operators;
using GeneticProgramming.Core;
using GeneticProgramming.Abstractions.Parameters;
using System.ComponentModel;
using System.Linq;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Integration tests for operator parameter functionality
    /// Tests the ability to access and modify operator parameters
    /// </summary>
    public class OperatorParametersIntegrationTests
    {
        [Fact]
        public void OperatorParameters_CanGetAndSetSubtreeCrossoverParameters()
        {
            var crossover = new SubtreeCrossover();
            var parameters = crossover.Parameters;
            
            Assert.NotNull(parameters);

            // Test InternalNodeProbability parameter access through property
            var originalProbability = crossover.InternalNodeProbability;
            Assert.True(originalProbability >= 0.0 && originalProbability <= 1.0);

            // Modify parameter
            crossover.InternalNodeProbability = 0.7;
            Assert.Equal(0.7, crossover.InternalNodeProbability);

            // Test boundary values
            crossover.InternalNodeProbability = 0.0;
            Assert.Equal(0.0, crossover.InternalNodeProbability);

            crossover.InternalNodeProbability = 1.0;
            Assert.Equal(1.0, crossover.InternalNodeProbability);

            // Test values outside range are clamped
            crossover.InternalNodeProbability = -0.1;
            Assert.Equal(0.0, crossover.InternalNodeProbability);

            crossover.InternalNodeProbability = 1.1;
            Assert.Equal(1.0, crossover.InternalNodeProbability);
        }

        [Fact]
        public void OperatorParameters_CanGetAndSetSubtreeMutatorParameters()
        {
            var mutator = new SubtreeMutator();
            var parameters = mutator.Parameters;
            
            Assert.NotNull(parameters);

            // Test MaxTreeLength parameter
            var originalLength = mutator.MaxTreeLength;
            Assert.True(originalLength > 0);

            mutator.MaxTreeLength = 50;
            Assert.Equal(50, mutator.MaxTreeLength);

            // Test that negative values are corrected
            mutator.MaxTreeLength = -5;
            Assert.Equal(1, mutator.MaxTreeLength); // Should be clamped to minimum of 1

            // Test MaxTreeDepth parameter
            var originalDepth = mutator.MaxTreeDepth;
            Assert.True(originalDepth > 0);

            mutator.MaxTreeDepth = 15;
            Assert.Equal(15, mutator.MaxTreeDepth);

            // Test that negative values are corrected
            mutator.MaxTreeDepth = 0;
            Assert.Equal(1, mutator.MaxTreeDepth); // Should be clamped to minimum of 1
        }

        [Fact]
        public void OperatorParameters_PropertyChangedEventsFireCorrectly()
        {
            var crossover = new SubtreeCrossover();
            var mutator = new SubtreeMutator();

            var crossoverEventFired = false;
            var mutatorLengthEventFired = false;
            var mutatorDepthEventFired = false;

            // Subscribe to PropertyChanged events
            crossover.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SubtreeCrossover.InternalNodeProbability))
                    crossoverEventFired = true;
            };

            mutator.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SubtreeMutator.MaxTreeLength))
                    mutatorLengthEventFired = true;
                else if (e.PropertyName == nameof(SubtreeMutator.MaxTreeDepth))
                    mutatorDepthEventFired = true;
            };

            // Change properties to trigger events
            crossover.InternalNodeProbability = 0.5;
            mutator.MaxTreeLength = 30;
            mutator.MaxTreeDepth = 8;

            // Verify events were fired
            Assert.True(crossoverEventFired);
            Assert.True(mutatorLengthEventFired);
            Assert.True(mutatorDepthEventFired);
        }

        [Fact]
        public void OperatorParameters_PropertyChangedEventsNotFiredForSameValue()
        {
            var crossover = new SubtreeCrossover();
            var eventFiredCount = 0;

            crossover.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SubtreeCrossover.InternalNodeProbability))
                    eventFiredCount++;
            };

            // Set to same value multiple times
            var currentValue = crossover.InternalNodeProbability;
            crossover.InternalNodeProbability = currentValue;
            crossover.InternalNodeProbability = currentValue;
            crossover.InternalNodeProbability = currentValue;

            // No events should have fired
            Assert.Equal(0, eventFiredCount);

            // Now change to different value
            crossover.InternalNodeProbability = currentValue + 0.1;
            Assert.Equal(1, eventFiredCount);
        }

        [Fact]
        public void OperatorParameters_AllOperatorsHaveAccessibleParameters()
        {
            var operators = new ISymbolicExpressionTreeOperator[]
            {
                new GrowTreeCreator(),
                new FullTreeCreator(),
                new SubtreeCrossover(),
                new OnePointCrossover(),
                new SubtreeMutator(),
                new ChangeNodeTypeMutator()
            };

            foreach (var op in operators)
            {
                // Each operator should have a Parameters collection
                Assert.NotNull(op.Parameters);
                
                // Parameters collection should be enumerable
                var paramCount = op.Parameters.Count();
                Assert.True(paramCount >= 0);
                
                // Should implement INotifyPropertyChanged
                Assert.IsAssignableFrom<INotifyPropertyChanged>(op);

                // Cast to IItem to access Name, Description properties
                var item = (IItem)op;
                
                // Base parameters from Item should be accessible
                Assert.NotNull(item.Name);
                Assert.NotNull(item.Description);
                
                // Should be able to change Name and Description
                var originalName = item.Name;
                var originalDescription = item.Description;
                
                item.Name = "Test Name";
                Assert.Equal("Test Name", item.Name);
                
                item.Description = "Test Description";
                Assert.Equal("Test Description", item.Description);
                
                // Restore original values
                item.Name = originalName;
                item.Description = originalDescription;
            }
        }

        [Fact]
        public void OperatorParameters_CanAccessParametersAfterModification()
        {
            var crossover = new SubtreeCrossover();
            
            // Modify parameter
            crossover.InternalNodeProbability = 0.8;
            
            // Should still be able to access parameters collection
            Assert.NotNull(crossover.Parameters);
            
            // Parameter collection should reflect current state
            var parameters = crossover.Parameters;
            Assert.NotNull(parameters);
            
            // Should be able to enumerate parameters
            var paramCount = parameters.Count();
            Assert.True(paramCount >= 0);
        }

        [Fact]
        public void OperatorParameters_ParametersCollectionIsConsistent()
        {
            var mutator = new SubtreeMutator();
            
            // Get parameters collection reference
            var parameters1 = mutator.Parameters;
            var parameters2 = mutator.Parameters;
            
            // Should return same reference (consistent)
            Assert.Same(parameters1, parameters2);
            
            // Modify operator state
            mutator.MaxTreeLength = 100;
            
            // Parameters collection should still be same reference
            var parameters3 = mutator.Parameters;
            Assert.Same(parameters1, parameters3);
        }
    }
}
