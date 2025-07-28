using System;
using System.Linq;
using System.Reflection;
using Xunit;
using GeneticProgramming.Abstractions.Parameters;
using GeneticProgramming.Abstractions.Operators;
using AbstractionOptimization = GeneticProgramming.Abstractions.Optimization;
using GeneticProgramming.Expressions;
using GeneticProgramming.Operators;
using GeneticProgramming.Algorithms;

namespace GeneticProgramming.Standalone.IntegrationTests.Abstractions
{
    /// <summary>
    /// Integration tests validating that core concrete classes implement the expected abstractions.
    /// </summary>
    public class AbstractionsIntegrationTests
    {
        [Fact]
        public void ParameterClasses_ImplementInterfaces()
        {
            Assert.True(typeof(IParameter).IsAssignableFrom(typeof(Parameter)));
            Assert.True(typeof(IParameterCollection).IsAssignableFrom(typeof(ParameterCollection)));
        }

        [Fact]
        public void GrammarClass_ImplementsInterface()
        {
            Assert.True(typeof(ISymbolicExpressionTreeGrammar).IsAssignableFrom(typeof(SymbolicExpressionTreeGrammar)));
        }

        [Fact]
        public void OperatorClasses_ImplementIOperator()
        {
            var operatorTypes = new[]
            {
                typeof(GrowTreeCreator),
                typeof(FullTreeCreator),
                typeof(SubtreeCrossover),
                typeof(OnePointCrossover),
                typeof(SubtreeMutator),
                typeof(ChangeNodeTypeMutator),
                typeof(NodeInsertionManipulator),
                typeof(NodeRemovalManipulator)
            };

            foreach (var type in operatorTypes)
            {
                Assert.True(typeof(ISymbolicExpressionTreeOperator).IsAssignableFrom(type));
                var parametersProp = type.GetProperty("Parameters", BindingFlags.Instance | BindingFlags.Public);
                Assert.NotNull(parametersProp);
                Assert.Equal(typeof(IParameterCollection), parametersProp!.PropertyType);
            }
        }

        [Fact]
        public void Algorithm_ImplementsOptimizationInterfaces()
        {
            Assert.True(typeof(AbstractionOptimization.IGeneticProgrammingAlgorithm).IsAssignableFrom(typeof(GeneticProgrammingAlgorithm)));
            Assert.True(typeof(AbstractionOptimization.IOptimizationAlgorithm).IsAssignableFrom(typeof(GeneticProgrammingAlgorithm)));
        }
    }
}
