using Microsoft.EntityFrameworkCore;
using GeneticProgramming.Playground.Data.Models;

namespace GeneticProgramming.Playground.Data;

public class ExperimentDbContext : DbContext
{
    public ExperimentDbContext(DbContextOptions<ExperimentDbContext> options) : base(options)
    {
    }
    
    public DbSet<ExperimentEntity> Experiments { get; set; }
    public DbSet<ExperimentConfigurationEntity> ExperimentConfigurations { get; set; }
    public DbSet<GenerationMetricsEntity> GenerationMetrics { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<ExperimentEntity>()
            .HasOne(e => e.Configuration)
            .WithOne(c => c.Experiment)
            .HasForeignKey<ExperimentConfigurationEntity>(c => c.ExperimentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<ExperimentEntity>()
            .HasMany(e => e.GenerationHistory)
            .WithOne(g => g.Experiment)
            .HasForeignKey(g => g.ExperimentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure indexes for better performance
        modelBuilder.Entity<ExperimentEntity>()
            .HasIndex(e => e.StartTime);
            
        modelBuilder.Entity<GenerationMetricsEntity>()
            .HasIndex(g => new { g.ExperimentId, g.Generation });
    }
}
