using Xunit;
using GeneticProgramming.Core;
using GeneticProgramming.Abstractions.Parameters;
using System;

namespace GeneticProgramming.Standalone.UnitTests.Core
{
    [Trait("Category", "Unit")]
    [Trait("Target", "Cloner")]
    public class ClonerTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void Constructor_InitializesEmptyClonedObjectsMap()
        {
            // Act
            var cloner = new Cloner();

            // Assert
            Assert.NotNull(cloner);
        }

        [Fact]
        public void Clone_WithNullInput_ReturnsNull()
        {
            // Arrange
            var cloner = new Cloner();

            // Act
            var result = cloner.Clone<TestDeepCloneable?>(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Clone_WithIDeepCloneableObject_ReturnsClone()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new TestDeepCloneable { Value = "test" };

            // Act
            var clone = cloner.Clone(original);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.Equal(original.Value, clone.Value);
        }

        [Fact]
        public void Clone_WithNonIDeepCloneableObject_ThrowsNotSupportedException()
        {
            // Arrange
            var cloner = new Cloner();
            var nonCloneable = new NonCloneableClass();

            // Act & Assert
            var exception = Assert.Throws<NotSupportedException>(() => cloner.Clone(nonCloneable));
            Assert.Contains("does not implement IDeepCloneable", exception.Message);
        }

        #endregion

        #region Reference Management Tests

        [Fact]
        public void Clone_SameObjectTwice_ReturnsSameClone()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new TestDeepCloneable { Value = "test" };

            // Act
            var clone1 = cloner.Clone(original);
            var clone2 = cloner.Clone(original);

            // Assert
            Assert.Same(clone1, clone2);
        }

        [Fact]
        public void Clone_DifferentObjects_ReturnsDifferentClones()
        {
            // Arrange
            var cloner = new Cloner();
            var original1 = new TestDeepCloneable { Value = "test1" };
            var original2 = new TestDeepCloneable { Value = "test2" };

            // Act
            var clone1 = cloner.Clone(original1);
            var clone2 = cloner.Clone(original2);

            // Assert
            Assert.NotSame(clone1, clone2);
            Assert.Equal("test1", clone1.Value);
            Assert.Equal("test2", clone2.Value);
        }

        [Fact]
        public void Clone_ObjectWithCircularReference_HandlesCorrectly()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new CircularReferenceTest { Value = "root" };
            var child = new CircularReferenceTest { Value = "child" };
            original.Reference = child;
            child.Reference = original; // Circular reference

            // Act
            var clone = cloner.Clone(original);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.Equal("root", clone.Value);
            Assert.NotNull(clone.Reference);
            Assert.Equal("child", clone.Reference.Value);
            Assert.Same(clone, clone.Reference.Reference); // Circular reference preserved
        }

        #endregion

        #region ParameterCollection Integration Tests

        [Fact]
        public void Clone_ParameterCollection_ClonesSuccessfully()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new TestParameterCollection();
            var param1 = new Parameter("param1", "description1");
            var param2 = new Parameter("param2", "description2");
            original.Add(param1);
            original.Add(param2);

            // Act
            var clone = cloner.Clone(original);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.Equal(2, clone.Count);
            
            var clonedParam1 = clone.Get("param1");
            var clonedParam2 = clone.Get("param2");
            
            Assert.NotNull(clonedParam1);
            Assert.NotNull(clonedParam2);
            Assert.NotSame(param1, clonedParam1);
            Assert.NotSame(param2, clonedParam2);
            Assert.Equal("param1", clonedParam1.Name);
            Assert.Equal("param2", clonedParam2.Name);
        }

        [Fact]
        public void Clone_EmptyParameterCollection_ClonesSuccessfully()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new TestParameterCollection();

            // Act
            var clone = cloner.Clone(original);

            // Assert
            Assert.NotNull(clone);
            Assert.NotSame(original, clone);
            Assert.Equal(0, clone.Count);
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void Clone_ObjectThrowingExceptionInClone_ThrowsException()
        {
            // Arrange
            var cloner = new Cloner();
            var problematic = new ProblematicCloneable();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cloner.Clone(problematic));
        }

        [Fact]
        public void Clone_DifferentClonersDoNotShareState()
        {
            // Arrange
            var cloner1 = new Cloner();
            var cloner2 = new Cloner();
            var original = new TestDeepCloneable { Value = "test" };

            // Act
            var clone1 = cloner1.Clone(original);
            var clone2 = cloner2.Clone(original);

            // Assert
            Assert.NotSame(clone1, clone2);
        }

        #endregion

        #region Test Helper Classes

        private class TestDeepCloneable : IDeepCloneable
        {
            public string Value { get; set; } = string.Empty;

            public IDeepCloneable Clone(Cloner cloner)
            {
                var clone = new TestDeepCloneable { Value = this.Value };
                cloner.RegisterClonedObject(this, clone);
                return clone;
            }
        }

        private class CircularReferenceTest : IDeepCloneable
        {
            public string Value { get; set; } = string.Empty;
            public CircularReferenceTest? Reference { get; set; }

            public IDeepCloneable Clone(Cloner cloner)
            {
                var clone = new CircularReferenceTest { Value = this.Value };
                cloner.RegisterClonedObject(this, clone);
                
                if (Reference != null)
                {
                    clone.Reference = cloner.Clone(Reference);
                }
                
                return clone;
            }
        }

        private class NonCloneableClass
        {
            public string Value { get; set; } = "test";
        }

        private class ProblematicCloneable : IDeepCloneable
        {
            public IDeepCloneable Clone(Cloner cloner)
            {
                throw new InvalidOperationException("Clone operation failed");
            }
        }

        private class TestParameterCollection : IDeepCloneable
        {
            private readonly ParameterCollection _parameters = new ParameterCollection();

            public int Count => _parameters.Count();

            public void Add(IParameter parameter) => _parameters.Add(parameter);
            public IParameter? Get(string name) => _parameters.Get(name);

            public IDeepCloneable Clone(Cloner cloner)
            {
                var clone = new TestParameterCollection();
                foreach (var param in _parameters)
                {
                    clone.Add(cloner.Clone(param));
                }
                return clone;
            }
        }

        #endregion
    }
}
