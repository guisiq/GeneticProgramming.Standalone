using System.ComponentModel.DataAnnotations;

namespace GeneticProgramming.Playground.Data.Models;

public class ExperimentEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";
    
    [MaxLength(1000)]
    public string Description { get; set; } = "";
    
    [Required]
    [MaxLength(100)]
    public string DatasetName { get; set; } = "";
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public double BestFitness { get; set; }
    public double FinalFitness { get; set; }
    public double InitialFitness { get; set; }
    public double TestAccuracy { get; set; }
    
    public int GenerationsCompleted { get; set; }
    public int TreeComplexity { get; set; }
    
    [MaxLength(2000)]
    public string BestIndividualString { get; set; } = "";
    
    // Navigation properties
    public ExperimentConfigurationEntity Configuration { get; set; } = null!;
    public List<GenerationMetricsEntity> GenerationHistory { get; set; } = new();
}
