using GeneticProgramming.Abstractions.Parameters;

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
    }
}
