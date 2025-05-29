using System;
using System.ComponentModel;
using Xunit;
using GeneticProgramming.Core;
using GeneticProgramming.Abstractions.Parameters;

namespace GeneticProgramming.Standalone.UnitTests.Core
{
    public class ItemTests
    {
        // Classe de teste concreta que herda de Item
        private class TestItem : Item
        {
            public TestItem() : base() { }
            
            public TestItem(TestItem original, Cloner cloner) : base(original, cloner) { }

            public override IDeepCloneable Clone(Cloner cloner)
            {
                return new TestItem(this, cloner);
            }
        }

        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var item = new TestItem();

            // Assert
            Assert.NotNull(item.Name);
            Assert.NotNull(item.Description);
            Assert.NotNull(item.Parameters);
            Assert.NotNull(item.ItemName);
            Assert.NotNull(item.ItemDescription);
            Assert.NotNull(item.ItemVersion);
            Assert.NotNull(item.ItemImage);
        }

        [Fact]
        public void Name_PropertyChanged_EventFired()
        {
            // Arrange
            var item = new TestItem();
            bool eventFired = false;
            string? changedPropertyName = null;

            item.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            item.Name = "Test Name";

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(Item.Name), changedPropertyName);
            Assert.Equal("Test Name", item.Name);
        }

        [Fact]
        public void Name_SameValue_EventNotFired()
        {
            // Arrange
            var item = new TestItem();
            item.Name = "Test Name";
            bool eventFired = false;

            item.PropertyChanged += (sender, e) => eventFired = true;

            // Act
            item.Name = "Test Name"; // Same value

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void Description_PropertyChanged_EventFired()
        {
            // Arrange
            var item = new TestItem();
            bool eventFired = false;
            string? changedPropertyName = null;

            item.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            item.Description = "Test Description";

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(Item.Description), changedPropertyName);
            Assert.Equal("Test Description", item.Description);
        }

        [Fact]
        public void Description_SameValue_EventNotFired()
        {
            // Arrange
            var item = new TestItem();
            item.Description = "Test Description";
            bool eventFired = false;

            item.PropertyChanged += (sender, e) => eventFired = true;

            // Act
            item.Description = "Test Description"; // Same value

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void Parameters_PropertyChanged_EventFired()
        {
            // Arrange
            var item = new TestItem();
            bool eventFired = false;
            string? changedPropertyName = null;

            item.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            var newParameters = new ParameterCollection();

            // Act
            item.Parameters = newParameters;

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(Item.Parameters), changedPropertyName);
            Assert.Same(newParameters, item.Parameters);
        }

        [Fact]
        public void Parameters_SameValue_EventNotFired()
        {
            // Arrange
            var item = new TestItem();
            var parameters = item.Parameters;
            bool eventFired = false;

            item.PropertyChanged += (sender, e) => eventFired = true;

            // Act
            item.Parameters = parameters; // Same value

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void OnPropertyChanged_FiresEvent()
        {
            // Arrange
            var item = new TestItem();
            bool eventFired = false;
            string? changedPropertyName = null;

            item.PropertyChanged += (sender, e) =>
            {
                eventFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            item.Name = "Trigger Event";

            // Assert
            Assert.True(eventFired);
            Assert.Equal(nameof(Item.Name), changedPropertyName);
        }

        [Fact]
        public void ToString_ReturnsName()
        {
            // Arrange
            var item = new TestItem();
            item.Name = "Test Item Name";

            // Act
            var result = item.ToString();

            // Assert
            Assert.Equal("Test Item Name", result);
        }

        [Fact]
        public void ItemName_ReturnsTypeName()
        {
            // Arrange
            var item = new TestItem();

            // Act
            var result = item.ItemName;

            // Assert
            Assert.Equal("TestItem", result);
        }

        [Fact]
        public void ItemDescription_ReturnsTypeName()
        {
            // Arrange
            var item = new TestItem();

            // Act
            var result = item.ItemDescription;

            // Assert
            Assert.Equal("TestItem", result);
        }

        [Fact]
        public void ItemVersion_ReturnsValidVersion()
        {
            // Arrange
            var item = new TestItem();

            // Act
            var result = item.ItemVersion;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new Version(1, 0), result);
        }

        [Fact]
        public void ItemImage_ReturnsEmptyString()
        {
            // Arrange
            var item = new TestItem();

            // Act
            var result = item.ItemImage;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Parameters_InitializedOnConstruction()
        {
            // Arrange & Act
            var item = new TestItem();

            // Assert
            Assert.NotNull(item.Parameters);
            Assert.IsType<ParameterCollection>(item.Parameters);
        }
    }
}
