using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticProgramming.Abstractions.Operators; // For IOperator

namespace GeneticProgramming.Core
{
    /// <summary>
    /// Base interface for all deep cloneable objects
    /// </summary>
    public interface IDeepCloneable
    {
        IDeepCloneable Clone(Cloner cloner);
    }

    /// <summary>
    /// Interface for items that can be named and described
    /// </summary>
    public interface INamedItem : INotifyPropertyChanged
    {
        string Name { get; set; }
        string Description { get; set; }
    }

    /// <summary>
    /// Base interface for all framework items
    /// </summary>
    public interface IItem : IDeepCloneable, INamedItem, IOperator // Added IOperator
    {
        string ItemName { get; }
        string ItemDescription { get; }
        Version ItemVersion { get; }
        string ItemImage { get; }
    }

    /// <summary>
    /// Cloner class responsible for deep cloning operations
    /// </summary>
    public class Cloner
    {
        private readonly Dictionary<object, object> clonedObjectsMap;

        public Cloner()
        {
            clonedObjectsMap = new Dictionary<object, object>();
        }

        /// <summary>
        /// Registers a clone for an object to handle circular references
        /// </summary>
        public void RegisterClone(object original, object clone)
        {
            clonedObjectsMap[original] = clone;
        }

        public T Clone<T>(T original) where T : class
        {
            if (original == null) return null!;

            if (clonedObjectsMap.TryGetValue(original, out var existing))
                return (T)existing;

            if (original is IDeepCloneable cloneable)
            {
                // Create a placeholder first to handle circular references
                var clone = cloneable.Clone(this);
                clonedObjectsMap[original] = clone;
                return (T)clone;
            }

            throw new NotSupportedException($"Type {typeof(T)} does not implement IDeepCloneable");
        }
    }

    /// <summary>
    /// Attribute to mark items as creatable
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CreatableAttribute : Attribute
    {
        public static class Categories
        {
            public const string PopulationBasedAlgorithms = "Population Based Algorithms";
            public const string GeneticProgramming = "Genetic Programming";
        }

        public string Category { get; }
        public int Priority { get; set; } = 100;

        public CreatableAttribute(string category)
        {
            Category = category;
        }
    }

    /// <summary>
    /// Attribute to mark classes as items
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ItemAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ItemAttribute(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}
