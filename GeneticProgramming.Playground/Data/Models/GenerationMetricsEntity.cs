using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneticProgramming.Playground.Data.Models;

public class GenerationMetricsEntity
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Experiment")]
    public int ExperimentId { get; set; }
    
    public int Generation { get; set; }
    public double BestFitness { get; set; }
    public double AverageFitness { get; set; }
    public double WorstFitness { get; set; }
    public double Diversity { get; set; }
    public int BestTreeSize { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Navigation property
    public ExperimentEntity Experiment { get; set; } = null!;
}
