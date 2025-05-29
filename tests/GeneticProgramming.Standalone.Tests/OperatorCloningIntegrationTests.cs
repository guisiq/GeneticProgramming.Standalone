using Xunit;
using GeneticProgramming.Operators;
using GeneticProgramming.Core;
using System.ComponentModel;
using System.Linq;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Integration tests for operator cloning functionality
    /// Tests the ability to clone operators and maintain parameter independence
    /// </summary>
    public class OperatorCloningIntegrationTests
    {
        [Fact]
        public void OperatorCloning_SubtreeCrossoverClonesParametersCorrectly()
        {
            var original = new SubtreeCrossover();
            original.Name = "Original Crossover";
            original.Description = "Original Description";
            original.InternalNodeProbability = 0.7;

            // Clone the operator
            var cloner = new Cloner();
            var clone = (SubtreeCrossover)original.Clone(cloner);

            // Verify clone is not the same instance
            Assert.NotSame(original, clone);

            // Verify properties are copied
            Assert.Equal(original.Name, clone.Name);
            Assert.Equal(original.Description, clone.Description);
            Assert.Equal(original.InternalNodeProbability, clone.InternalNodeProbability);

            // Verify parameters collection is cloned
            Assert.NotSame(original.Parameters, clone.Parameters);

            // Modify original and verify independence
            original.InternalNodeProbability = 0.3;
            original.Name = "Modified Original";
            original.Description = "Modified Description";

            // Clone should retain original values
            Assert.Equal(0.7, clone.InternalNodeProbability);
            Assert.Equal("Original Crossover", clone.Name);
            Assert.Equal("Original Description", clone.Description);

            // Modify clone and verify original is unaffected
            clone.InternalNodeProbability = 0.9;
            clone.Name = "Modified Clone";
            clone.Description = "Clone Description";

            Assert.Equal(0.3, original.InternalNodeProbability);
            Assert.Equal("Modified Original", original.Name);
            Assert.Equal("Modified Description", original.Description);
        }

        [Fact]
        public void OperatorCloning_SubtreeMutatorClonesParametersCorrectly()
        {
            var original = new SubtreeMutator();
            original.Name = "Original Mutator";
            original.Description = "Original Description";
            original.MaxTreeLength = 50;
            original.MaxTreeDepth = 15;

            // Clone the operator
            var cloner = new Cloner();
            var clone = (SubtreeMutator)original.Clone(cloner);

            // Verify clone is not the same instance
            Assert.NotSame(original, clone);

            // Verify properties are copied
            Assert.Equal(original.Name, clone.Name);
            Assert.Equal(original.Description, clone.Description);
            Assert.Equal(original.MaxTreeLength, clone.MaxTreeLength);
            Assert.Equal(original.MaxTreeDepth, clone.MaxTreeDepth);

            // Verify parameters collection is cloned
            Assert.NotSame(original.Parameters, clone.Parameters);

            // Modify original and verify independence
            original.MaxTreeLength = 100;
            original.MaxTreeDepth = 20;
            original.Name = "Modified Original";

            // Clone should retain original values
            Assert.Equal(50, clone.MaxTreeLength);
            Assert.Equal(15, clone.MaxTreeDepth);
            Assert.Equal("Original Mutator", clone.Name);

            // Modify clone and verify original is unaffected
            clone.MaxTreeLength = 30;
            clone.MaxTreeDepth = 8;
            clone.Name = "Modified Clone";

            Assert.Equal(100, original.MaxTreeLength);
            Assert.Equal(20, original.MaxTreeDepth);
            Assert.Equal("Modified Original", original.Name);
        }

        [Fact]
        public void OperatorCloning_AllOperatorTypesClonesSuccessfully()
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

            var cloner = new Cloner();

            foreach (var original in operators)
            {
                // Cast to IItem to access Name and Description properties
                var originalItem = (IItem)original;
                
                // Set some test values
                originalItem.Name = $"Original {original.GetType().Name}";
                originalItem.Description = "Original Description";

                // Clone the operator
                var clone = original.Clone(cloner);

                // Verify basic cloning requirements
                Assert.NotNull(clone);
                Assert.NotSame(original, clone);
                Assert.IsType(original.GetType(), clone);

                // Cast to correct interfaces to access properties
                var clonedOperator = (ISymbolicExpressionTreeOperator)clone;
                var clonedItem = (IItem)clone;

                // Verify properties are copied
                Assert.Equal(originalItem.ItemName, clonedItem.ItemName);
                Assert.Equal(originalItem.ItemDescription, clonedItem.ItemDescription);

                // Verify parameters collection exists and is cloned
                Assert.NotNull(clonedOperator.Parameters);
                Assert.NotSame(original.Parameters, clonedOperator.Parameters);
            }
        }

        [Fact]
        public void OperatorCloning_PropertyChangedEventsWorkOnClonedOperators()
        {
            var original = new SubtreeCrossover();
            original.InternalNodeProbability = 0.6;

            var cloner = new Cloner();
            var clone = (SubtreeCrossover)original.Clone(cloner);

            var eventFired = false;
            string? propertyName = null;

            // Subscribe to PropertyChanged event on clone
            clone.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                propertyName = e.PropertyName;
            };

            // Modify clone property
            clone.InternalNodeProbability = 0.8;

            // Verify event was fired on clone
            Assert.True(eventFired);
            Assert.Equal(nameof(SubtreeCrossover.InternalNodeProbability), propertyName);

            // Verify the change
            Assert.Equal(0.8, clone.InternalNodeProbability);
            Assert.Equal(0.6, original.InternalNodeProbability); // Original unchanged
        }

        [Fact]
        public void OperatorCloning_ClonedOperatorsAreIndependent()
        {
            var original = new SubtreeMutator();
            original.MaxTreeLength = 25;
            original.MaxTreeDepth = 10;

            // Use different cloners to ensure we get independent clones
            var cloner1 = new Cloner();
            var cloner2 = new Cloner();
            var clone1 = (SubtreeMutator)original.Clone(cloner1);
            var clone2 = (SubtreeMutator)original.Clone(cloner2);

            // Verify all are independent instances
            Assert.NotSame(original, clone1);
            Assert.NotSame(original, clone2);
            Assert.NotSame(clone1, clone2);

            // Modify each instance differently
            original.MaxTreeLength = 50;
            clone1.MaxTreeLength = 75;
            clone2.MaxTreeLength = 100;

            // Verify independence
            Assert.Equal(50, original.MaxTreeLength);
            Assert.Equal(75, clone1.MaxTreeLength);
            Assert.Equal(100, clone2.MaxTreeLength);

            // Verify parameters are independent
            Assert.NotSame(original.Parameters, clone1.Parameters);
            Assert.NotSame(original.Parameters, clone2.Parameters);
            Assert.NotSame(clone1.Parameters, clone2.Parameters);
        }

        [Fact]
        public void OperatorCloning_ClonePreservesEventHandling()
        {
            var original = new SubtreeCrossover();
            
            var originalEventCount = 0;
            original.PropertyChanged += (sender, e) => originalEventCount++;

            // Clone the operator
            var cloner = new Cloner();
            var clone = (SubtreeCrossover)original.Clone(cloner);

            var cloneEventCount = 0;
            clone.PropertyChanged += (sender, e) => cloneEventCount++;

            // Modify both operators
            original.InternalNodeProbability = 0.3;
            clone.InternalNodeProbability = 0.7;

            // Verify both can handle events independently
            Assert.Equal(1, originalEventCount);
            Assert.Equal(1, cloneEventCount);

            // Verify they have different event handler collections
            // (events should not be shared between original and clone)
            original.InternalNodeProbability = 0.4;
            Assert.Equal(2, originalEventCount);
            Assert.Equal(1, cloneEventCount); // Clone count unchanged
        }

        [Fact]
        public void OperatorCloning_ClonerHandlesCircularReferencesCorrectly()
        {
            var cloner = new Cloner();
            var original = new SubtreeCrossover();

            // Clone the same operator multiple times with same cloner
            var clone1 = original.Clone(cloner);
            var clone2 = original.Clone(cloner);

            // Should get the same clone instance for the same original
            // (this tests the cloner's object tracking functionality)
            Assert.Same(clone1, clone2);

            // Use a new cloner for a fresh clone
            var newCloner = new Cloner();
            var clone3 = original.Clone(newCloner);

            // This should be a different instance
            Assert.NotSame(clone1, clone3);
        }
    }
}
