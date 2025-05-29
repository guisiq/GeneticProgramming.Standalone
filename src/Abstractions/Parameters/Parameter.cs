using GeneticProgramming.Abstractions.Parameters;
using GeneticProgramming.Core;

namespace GeneticProgramming.Abstractions.Parameters
{
    /// <summary>
    /// Base implementation of a configurable parameter.
    /// </summary>
    public class Parameter : IParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Parameter(string name, string description)
        {
            Name = name;
            Description = description;
        }

        protected Parameter(Parameter original, Cloner cloner)
        {
            Name = original.Name;
            Description = original.Description;
        }

        public virtual IDeepCloneable Clone(Cloner cloner)
        {
            return new Parameter(this, cloner);
        }
    }
}
