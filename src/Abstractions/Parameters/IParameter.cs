using GeneticProgramming.Core;

namespace GeneticProgramming.Abstractions.Parameters
{
    /// <summary>
    /// Represents a configurable parameter with a name and description.
    /// </summary>
    public interface IParameter : IDeepCloneable
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
