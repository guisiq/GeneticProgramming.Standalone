using System.Collections.Generic;

namespace GeneticProgramming.Abstractions.Parameters
{
    /// <summary>
    /// Represents a collection of parameters.
    /// </summary>
    public interface IParameterCollection : IEnumerable<IParameter>
    {
        void Add(IParameter parameter);
        bool Remove(string name);
        IParameter? Get(string name);
    }
}
