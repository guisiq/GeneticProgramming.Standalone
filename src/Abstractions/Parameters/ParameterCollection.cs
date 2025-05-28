using System.Collections;
using System.Collections.Generic;
using GeneticProgramming.Abstractions.Parameters;

namespace GeneticProgramming.Abstractions.Parameters
{
    /// <summary>
    /// Basic implementation of a parameter collection.
    /// </summary>
    public class ParameterCollection : IParameterCollection
    {
        private readonly List<IParameter> parameters = new List<IParameter>();

        public IEnumerator<IParameter> GetEnumerator() => parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IParameter parameter)
        {
            parameters.Add(parameter);
        }

        public bool Remove(string name)
        {
            var param = parameters.Find(p => p.Name == name);
            if (param != null)
            {
                parameters.Remove(param);
                return true;
            }
            return false;
        }

        public IParameter? Get(string name) => parameters.Find(p => p.Name == name);
    }
}
