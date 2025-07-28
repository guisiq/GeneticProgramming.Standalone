using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Abstractions.Parameters;
using GeneticProgramming.Core;

namespace GeneticProgramming.Abstractions.Parameters
{
    /// <summary>
    /// Basic implementation of a parameter collection.
    /// </summary>
    public class ParameterCollection : IParameterCollection, IDeepCloneable
    {
        private readonly List<IParameter> parameters = new List<IParameter>();

        public ParameterCollection()
        {
        }

        protected ParameterCollection(ParameterCollection original, Cloner cloner)
        {
            parameters = new List<IParameter>(original.parameters.Select(p => cloner.Clone(p)));
        }

        public IEnumerator<IParameter> GetEnumerator() => parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            
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

        public IDeepCloneable Clone(Cloner cloner)
        {
            return new ParameterCollection(this, cloner);
        }
    }
}
