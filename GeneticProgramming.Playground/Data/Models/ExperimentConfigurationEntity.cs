using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneticProgramming.Playground.Data.Models;

public class ExperimentConfigurationEntity
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Experiment")]
    public int ExperimentId { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; } = "";
    
    public int PopulationSize { get; set; }
    public int MaxGenerations { get; set; }
    public int MaxTreeDepth { get; set; }
    public int MaxTreeLength { get; set; }
    public int RandomSeed { get; set; }
    
    public double CrossoverProbability { get; set; }
    public double MutationProbability { get; set; }
    
    public bool UseBasicMath { get; set; }
    public bool UseAdvancedMath { get; set; }
    public bool UseStatistics { get; set; }
    public bool AllowConstants { get; set; }
    
    [MaxLength(50)]
    public string ProblemType { get; set; } = "";
    
    [MaxLength(50)]
    public string ClassificationFitnessType { get; set; } = "";
    
    [MaxLength(50)]
    public string RegressionFitnessType { get; set; } = "";
    
    public double ParsimonyPressure { get; set; }
    
    [MaxLength(50)]
    public string TreeCreationMethod { get; set; } = "";
    
    [MaxLength(50)]
    public string MutationType { get; set; } = "";
    
    [MaxLength(50)]
    public string CrossoverType { get; set; } = "";
    
    // Navigation property
    public ExperimentEntity Experiment { get; set; } = null!;
}
