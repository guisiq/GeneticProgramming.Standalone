using System;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Debug Cloner Behavior ===");
        
        var cloner = new Cloner();
        var original = new SubtreeCrossover();
        
        Console.WriteLine($"Original: {original.GetHashCode()}");
        Console.WriteLine($"Cloner map count before first clone: {GetClonedObjectsMapCount(cloner)}");
        
        // First clone
        var clone1 = original.Clone(cloner);
        Console.WriteLine($"Clone1: {clone1.GetHashCode()}");
        Console.WriteLine($"Cloner map count after first clone: {GetClonedObjectsMapCount(cloner)}");
        Console.WriteLine($"Are they the same? {object.ReferenceEquals(original, clone1)}");
        
        // Check if original is registered
        bool isRegistered = cloner.ClonedObjectRegistered(original);
        Console.WriteLine($"Is original registered in cloner? {isRegistered}");
        
        // Second clone (should return same instance as clone1)
        var clone2 = original.Clone(cloner);
        Console.WriteLine($"Clone2: {clone2.GetHashCode()}");
        Console.WriteLine($"Cloner map count after second clone: {GetClonedObjectsMapCount(cloner)}");
        Console.WriteLine($"Are clone1 and clone2 the same? {object.ReferenceEquals(clone1, clone2)}");
        
        // Check what GetClone returns
        var getCloneResult = cloner.GetClone(original);
        Console.WriteLine($"GetClone result: {getCloneResult?.GetHashCode()}");
        Console.WriteLine($"GetClone equals clone1? {object.ReferenceEquals(getCloneResult, clone1)}");
        Console.WriteLine($"GetClone equals clone2? {object.ReferenceEquals(getCloneResult, clone2)}");
    }
    
    // Use reflection to access private field for debugging
    static int GetClonedObjectsMapCount(Cloner cloner)
    {
        var field = typeof(Cloner).GetField("clonedObjectsMap", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dict = field?.GetValue(cloner) as System.Collections.IDictionary;
        return dict?.Count ?? -1;
    }
}
