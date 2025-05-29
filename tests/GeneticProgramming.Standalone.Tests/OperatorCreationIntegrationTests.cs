using Xunit;
using GeneticProgramming.Operators;
using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using System;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Integration tests for operator creation functionality
    /// Tests the ability to create all types of genetic programming operators
    /// </summary>
    public class OperatorCreationIntegrationTests
    {
        [Fact]
        public void OperatorCreation_CanCreateAllTreeCreators()
        {
            // Test GrowTreeCreator
            var growCreator = new GrowTreeCreator();
            Assert.NotNull(growCreator);
            Assert.NotNull(growCreator.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeCreator>(growCreator);
            Assert.IsAssignableFrom<IDeepCloneable>(growCreator);

            // Test FullTreeCreator
            var fullCreator = new FullTreeCreator();
            Assert.NotNull(fullCreator);
            Assert.NotNull(fullCreator.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeCreator>(fullCreator);
            Assert.IsAssignableFrom<IDeepCloneable>(fullCreator);
        }

        [Fact]
        public void OperatorCreation_CanCreateAllCrossoverOperators()
        {
            // Test SubtreeCrossover
            var subtreeCrossover = new SubtreeCrossover();
            Assert.NotNull(subtreeCrossover);
            Assert.NotNull(subtreeCrossover.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeCrossover>(subtreeCrossover);
            Assert.IsAssignableFrom<IDeepCloneable>(subtreeCrossover);

            // Test OnePointCrossover
            var onePointCrossover = new OnePointCrossover();
            Assert.NotNull(onePointCrossover);
            Assert.NotNull(onePointCrossover.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeCrossover>(onePointCrossover);
            Assert.IsAssignableFrom<IDeepCloneable>(onePointCrossover);
        }

        [Fact]
        public void OperatorCreation_CanCreateAllMutationOperators()
        {
            // Test SubtreeMutator
            var subtreeMutator = new SubtreeMutator();
            Assert.NotNull(subtreeMutator);
            Assert.NotNull(subtreeMutator.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeMutator>(subtreeMutator);
            Assert.IsAssignableFrom<IDeepCloneable>(subtreeMutator);

            // Test ChangeNodeTypeMutator
            var changeNodeMutator = new ChangeNodeTypeMutator();
            Assert.NotNull(changeNodeMutator);
            Assert.NotNull(changeNodeMutator.Parameters);
            Assert.IsAssignableFrom<ISymbolicExpressionTreeMutator>(changeNodeMutator);
            Assert.IsAssignableFrom<IDeepCloneable>(changeNodeMutator);
        }

        [Fact]
        public void OperatorCreation_AllOperatorsHaveValidDefaultState()
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
                // Cast to IItem to access Name, Description, ItemName, ItemDescription, ItemVersion
                var item = (IItem)op;
                
                // Check basic properties
                Assert.NotNull(op);
                Assert.NotNull(op.Parameters);
                Assert.NotNull(item.Name);
                Assert.NotNull(item.Description);
                
                // Check that operator can be converted to string
                var stringResult = op.ToString();
                Assert.NotNull(stringResult);
                
                // Check ItemName and ItemDescription are accessible
                Assert.NotNull(item.ItemName);
                Assert.NotNull(item.ItemDescription);
                
                // Check ItemVersion is valid
                var version = item.ItemVersion;
                Assert.True(version.Major >= 0);
                Assert.True(version.Minor >= 0);
            }
        }

        [Fact]
        public void OperatorCreation_OperatorsCanBeInstantiatedMultipleTimes()
        {
            // Test that multiple instances can be created without issues
            var creators = new ISymbolicExpressionTreeCreator[10];
            var crossovers = new ISymbolicExpressionTreeCrossover[10];
            var mutators = new ISymbolicExpressionTreeMutator[10];

            // Create multiple instances
            for (int i = 0; i < 10; i++)
            {
                creators[i] = new GrowTreeCreator();
                crossovers[i] = new SubtreeCrossover();
                mutators[i] = new SubtreeMutator();
            }

            // Verify all instances are independent
            for (int i = 0; i < 10; i++)
            {
                Assert.NotNull(creators[i]);
                Assert.NotNull(crossovers[i]);
                Assert.NotNull(mutators[i]);

                // Verify they are different instances
                for (int j = i + 1; j < 10; j++)
                {
                    Assert.NotSame(creators[i], creators[j]);
                    Assert.NotSame(crossovers[i], crossovers[j]);
                    Assert.NotSame(mutators[i], mutators[j]);
                }
            }
        }

        [Fact]
        public void OperatorCreation_OperatorsInheritFromCorrectBaseClasses()
        {
            var growCreator = new GrowTreeCreator();
            var subtreeCrossover = new SubtreeCrossover();
            var subtreeMutator = new SubtreeMutator();

            // Check inheritance hierarchy
            Assert.IsAssignableFrom<SymbolicExpressionTreeOperator>(growCreator);
            Assert.IsAssignableFrom<Item>(growCreator);

            Assert.IsAssignableFrom<SymbolicExpressionTreeOperator>(subtreeCrossover);
            Assert.IsAssignableFrom<Item>(subtreeCrossover);

            Assert.IsAssignableFrom<SymbolicExpressionTreeOperator>(subtreeMutator);
            Assert.IsAssignableFrom<Item>(subtreeMutator);
        }
    }
}
