using Xunit;
using GeneticProgramming.Abstractions.Parameters;
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.UnitTests.Core
{
    [Trait("Category", "Unit")]
    [Trait("Target", "ParameterCollection")]
    public class ParameterCollectionTests
    {
        // [Fact]
        // public void Add_AddsParameterToCollection()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();
        //     var parameter = new Parameter("TestParam", "Test Description");

        //     // Act
        //     collection.Add(parameter);

        //     // Assert
        //     Assert.Single(collection);
        //     Assert.Equal(parameter, collection.First());
        // }

        // [Fact]
        // public void Add_ThrowsArgumentNullException_WhenParameterIsNull()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();

        //     // Act & Assert
        //     Assert.Throws<ArgumentNullException>(() => collection.Add(null!));
        // }

        // [Fact]
        // public void Remove_RemovesParameterFromCollection_ByName()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();
        //     var parameter = new Parameter("TestParam", "Test Description");
        //     collection.Add(parameter);

        //     // Act
        //     var result = collection.Remove("TestParam");

        //     // Assert
        //     Assert.True(result);
        //     Assert.Empty(collection);
        // }

        // [Fact]
        // public void Remove_ReturnsFalse_WhenParameterNotInCollection()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();

        //     // Act
        //     var result = collection.Remove("NonExistentParam");

        //     // Assert
        //     Assert.False(result);
        // }

        // [Fact]
        // public void Get_ReturnsCorrectParameter_WhenExists()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();
        //     var parameter = new Parameter("TestParam", "Test Description");
        //     collection.Add(parameter);

        //     // Act
        //     var retrievedParameter = collection.Get("TestParam");

        //     // Assert
        //     Assert.Equal(parameter, retrievedParameter);
        // }

        // [Fact]
        // public void Get_ReturnsNull_WhenNotExists()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();

        //     // Act
        //     var retrievedParameter = collection.Get("NonExistentParam");

        //     // Assert
        //     Assert.Null(retrievedParameter);
        // }

        // [Fact]
        // public void Enumeration_WorksCorrectly()
        // {
        //     // Arrange
        //     var collection = new ParameterCollection();
        //     var p1 = new Parameter("P1", "D1");
        //     var p2 = new Parameter("P2", "D2");
        //     collection.Add(p1);
        //     collection.Add(p2);

        //     // Act & Assert
        //     Assert.Equal(2, collection.Count());
        //     Assert.Contains(p1, collection);
        //     Assert.Contains(p2, collection);
        // }
    }
}
