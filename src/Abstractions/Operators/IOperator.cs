using GeneticProgramming.Abstractions.Parameters;

namespace GeneticProgramming.Abstractions.Operators
{
    /// <summary>
    /// Represents a generic operator with configurable parameters.
    /// </summary>
    public interface IOperator
    {
        IParameterCollection? Parameters { get; } // Made nullable
    }
}
