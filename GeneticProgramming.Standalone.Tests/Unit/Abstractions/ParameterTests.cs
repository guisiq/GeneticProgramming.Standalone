using System;
using System.ComponentModel;
using Xunit;
using GeneticProgramming.Abstractions.Parameters;
using GeneticProgramming.Core;

namespace GeneticProgramming.Standalone.UnitTests.Parameters
{
    public class ParameterTests
    {
        // Classe de teste que herda de Parameter para testar PropertyChanged
        private class TestParameter : Parameter, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            private string _name;
            private string _description;

            public new string Name
            {
                get => _name;
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public new string Description
            {
                get => _description;
                set
                {
                    if (_description != value)
                    {
                        _description = value;
                        OnPropertyChanged(nameof(Description));
                    }
                }
            }

            public TestParameter(string name, string description) : base(name, description)
            {
                _name = name;
                _description = description;
            }

            protected TestParameter(TestParameter original, Cloner cloner) : base(original, cloner)
            {
                _name = original._name;
                _description = original._description;
            }

            public override IDeepCloneable Clone(Cloner cloner)
            {
                return new TestParameter(this, cloner);
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Fact]
        public void Constructor_SetsNameAndDescription()
        {
            // Arrange
            const string name = "TestParameter";
            const string description = "Test parameter description";

            // Act
            var parameter = new Parameter(name, description);

            // Assert
            Assert.Equal(name, parameter.Name);
            Assert.Equal(description, parameter.Description);
        }

        [Fact]
        public void Constructor_WithNullName_SetsName()
        {
            // Arrange
            const string? name = null;
            const string description = "Test description";

            // Act
            var parameter = new Parameter(name!, description);

            // Assert
            Assert.Null(parameter.Name);
            Assert.Equal(description, parameter.Description);
        }

        [Fact]
        public void Constructor_WithNullDescription_SetsDescription()
        {
            // Arrange
            const string name = "TestParameter";
            const string? description = null;

            // Act
            var parameter = new Parameter(name, description!);

            // Assert
            Assert.Equal(name, parameter.Name);
            Assert.Null(parameter.Description);
        }

        [Fact]
        public void Clone_CreatesNewInstance()
        {
            // Arrange
            var original = new Parameter("Original", "Original Description");
            var cloner = new Cloner();

            // Act
            var cloned = original.Clone(cloner);

            // Assert
            Assert.NotNull(cloned);
            Assert.NotSame(original, cloned);
            Assert.IsType<Parameter>(cloned);
        }

        [Fact]
        public void Clone_CopiesProperties()
        {
            // Arrange
            var original = new Parameter("Original", "Original Description");
            var cloner = new Cloner();

            // Act
            var cloned = (Parameter)original.Clone(cloner);

            // Assert
            Assert.Equal(original.Name, cloned.Name);
            Assert.Equal(original.Description, cloned.Description);
        }

        [Fact]
        public void Clone_IndependentInstances()
        {
            // Arrange
            var original = new Parameter("Original", "Original Description");
            var cloner = new Cloner();

            // Act
            var cloned = (Parameter)original.Clone(cloner);
            
            // Modify original
            original.Name = "Modified Original";
            original.Description = "Modified Description";

            // Assert
            Assert.NotEqual(original.Name, cloned.Name);
            Assert.NotEqual(original.Description, cloned.Description);
            Assert.Equal("Original", cloned.Name);
            Assert.Equal("Original Description", cloned.Description);
        }

        [Fact]
        public void TestParameter_Name_PropertyChanged_EventFired()
        {
            // Arrange
            var parameter = new TestParameter("Initial", "Description");
            bool eventFired = false;
            string? changedPropertyName = null;

            parameter.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            parameter.Name = "Changed Name";

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(TestParameter.Name), changedPropertyName);
            Assert.Equal("Changed Name", parameter.Name);
        }

        [Fact]
        public void TestParameter_Name_SameValue_EventNotFired()
        {
            // Arrange
            var parameter = new TestParameter("Test Name", "Description");
            bool eventFired = false;

            parameter.PropertyChanged += (sender, e) => eventFired = true;

            // Act
            parameter.Name = "Test Name"; // Same value

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void TestParameter_Description_PropertyChanged_EventFired()
        {
            // Arrange
            var parameter = new TestParameter("Name", "Initial Description");
            bool eventFired = false;
            string? changedPropertyName = null;

            parameter.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            parameter.Description = "Changed Description";

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(TestParameter.Description), changedPropertyName);
            Assert.Equal("Changed Description", parameter.Description);
        }

        [Fact]
        public void TestParameter_Description_SameValue_EventNotFired()
        {
            // Arrange
            var parameter = new TestParameter("Name", "Test Description");
            bool eventFired = false;

            parameter.PropertyChanged += (sender, e) => eventFired = true;

            // Act
            parameter.Description = "Test Description"; // Same value

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void TestParameter_Clone_PreservesEventHandling()
        {
            // Arrange
            var original = new TestParameter("Original", "Description");
            var cloner = new Cloner();

            // Act
            var cloned = (TestParameter)original.Clone(cloner);
            
            // Verify that cloned instance can handle PropertyChanged events
            bool eventFired = false;
            cloned.PropertyChanged += (sender, e) => eventFired = true;
            cloned.Name = "Cloned Modified";

            // Assert
            Assert.True(eventFired);
            Assert.Equal("Cloned Modified", cloned.Name);
        }

        [Fact]
        public void TestParameter_MultiplePropertyChanges_AllEventsFired()
        {
            // Arrange
            var parameter = new TestParameter("Initial", "Initial Description");
            int eventCount = 0;
            
            parameter.PropertyChanged += (sender, e) => eventCount++;

            // Act
            parameter.Name = "Changed Name";
            parameter.Description = "Changed Description";
            parameter.Name = "Changed Again";

            // Assert
            Assert.Equal(3, eventCount);
        }
    }
}
