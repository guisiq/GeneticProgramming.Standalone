using System;
using System.Collections.Generic;

namespace GeneticProgramming.Core
{
    /// <summary>
    /// Cloner class responsible for deep cloning operations.
    /// It maintains a map of original objects to their cloned counterparts
    /// to handle circular references and ensure that objects are cloned only once
    /// within a single cloning operation (i.e., using the same Cloner instance).
    /// </summary>
    public class Cloner
    {
        private readonly Dictionary<object, object> clonedObjectsMap;

        public Cloner()
        {
            clonedObjectsMap = new Dictionary<object, object>(new ReferenceEqualityComparer());
        }

        /// <summary>
        /// Registers a clone for an original object.
        /// This method is typically called from within the copy constructor or
        /// Clone method of an IDeepCloneable object.
        /// If the original object is already registered, its existing clone mapping will be overwritten.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        public void RegisterClonedObject(object original, object clone)
        {
            clonedObjectsMap[original] = clone;
        }

        /// <summary>
        /// Checks if a clone is already registered for a given original object.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <returns>True if the object has already been cloned and registered in this Cloner instance, false otherwise.</returns>
        public bool ClonedObjectRegistered(object original)
        {
            return clonedObjectsMap.ContainsKey(original);
        }

        /// <summary>
        /// Retrieves the clone of an object, if it was already cloned and registered
        /// with this Cloner instance.
        /// </summary>
        /// <typeparam name="T">The type of the original object.</typeparam>
        /// <param name="original">The original object.</param>
        /// <returns>The cloned instance of type T if found, otherwise null.</returns>
        public T? GetClone<T>(T? original) where T : class
        {
            if (original == null) return null;
            if (clonedObjectsMap.TryGetValue(original, out var clone))
                return (T)clone;
            return null;
        }

        /// <summary>
        /// Performs a deep clone of the specified object.
        /// If the object has already been cloned using this Cloner instance,
        /// the existing clone is returned. Otherwise, if the object implements
        /// IDeepCloneable, its Clone method is invoked.
        /// </summary>
        /// <typeparam name="T">The type of the object to clone.</typeparam>
        /// <param name="original">The original object to clone.</param>
        /// <returns>The cloned object.</returns>
        /// <exception cref="NotSupportedException">Thrown if the object is not null,
        /// does not implement IDeepCloneable, and has not been previously cloned/registered.</exception>
        /// <remarks>
        /// For this mechanism to work correctly and avoid issues like the one observed in
        /// `ClonerDiagnosticTests.CloneDiagnostic_SingleObjectClonedTwice_ShouldReturnSameInstance`,
        /// implementations of `IDeepCloneable.Clone(Cloner cloner)` MUST first check if `cloner.ClonedObjectRegistered(this)`
        /// is true. If so, they should return `cloner.GetClone(this)`. Otherwise, they should proceed
                /// to create a new instance, and the new instance (usually in its copy constructor)
        /// should call `cloner.RegisterClonedObject(originalInstance, thisNewCloneInstance)`.
        /// </remarks>
        public T? Clone<T>(T? original) where T : class
        {
            if (original == null) return null;

            // Check if already cloned by this cloner instance
            if (clonedObjectsMap.TryGetValue(original, out var existingClone))
                return (T)existingClone;

            if (original is IDeepCloneable cloneable)
            {
                // The IDeepCloneable object's Clone method is responsible for:
                // 1. Checking if it's already registered with the cloner (though Cloner.Clone does it first here).
                // 2. Creating the new clone.
                // 3. Registering the new clone with the cloner via cloner.RegisterClonedObject(original, newClone).
                // This call to cloneable.Clone(this) will lead to the new clone registering itself.
                var newTypedClone = cloneable.Clone(this);
                return (T)newTypedClone;
            }

            // Fallback for types that are not IDeepCloneable but might have been registered manually (less common).
            // Or, more likely, this indicates an issue if we expect IDeepCloneable.
            throw new NotSupportedException($"Type {original.GetType().FullName} does not implement IDeepCloneable, and no pre-existing clone was found. " +
                                            "Ensure the object implements IDeepCloneable or is handled by a custom cloning process that registers it.");
        }
    }

    /// <summary>
    /// An IEqualityComparer that uses ReferenceEquals.
    /// Useful for dictionaries where keys are objects and you want to compare them by reference.
    /// </summary>
    internal class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
