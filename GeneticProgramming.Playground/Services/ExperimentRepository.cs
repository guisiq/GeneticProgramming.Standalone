using Microsoft.EntityFrameworkCore;
using GeneticProgramming.Playground.Data;
using GeneticProgramming.Playground.Data.Models;
using GeneticProgramming.Playground.Services;

namespace GeneticProgramming.Playground.Services;

public class ExperimentRepository
{
    private readonly ExperimentDbContext _context;
    
    public ExperimentRepository(ExperimentDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Saves an experiment result to the database
    /// </summary>
    public async Task<int> SaveExperimentAsync(ExperimentResult result)
    {
        var entity = MapToEntity(result);
        
        _context.Experiments.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity.Id;
    }
    
    /// <summary>
    /// Gets all experiments ordered by start time (newest first)
    /// </summary>
    public async Task<List<ExperimentEntity>> GetAllExperimentsAsync()
    {
        return await _context.Experiments
            .Include(e => e.Configuration)
            .OrderByDescending(e => e.StartTime)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets all experiments as ExperimentResult objects
    /// </summary>
    public async Task<List<ExperimentResult>> GetAllExperimentResultsAsync()
    {
        var entities = await _context.Experiments
            .Include(e => e.Configuration)
            .Include(e => e.GenerationHistory.OrderBy(g => g.Generation))
            .OrderByDescending(e => e.StartTime)
            .ToListAsync();
            
        return entities.Select(MapFromEntity).ToList();
    }
    
    /// <summary>
    /// Gets a specific experiment by ID with all related data
    /// </summary>
    public async Task<ExperimentEntity?> GetExperimentByIdAsync(int id)
    {
        return await _context.Experiments
            .Include(e => e.Configuration)
            .Include(e => e.GenerationHistory.OrderBy(g => g.Generation))
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    /// <summary>
    /// Gets experiments for comparison
    /// </summary>
    public async Task<List<ExperimentEntity>> GetExperimentsForComparisonAsync(List<int> experimentIds)
    {
        return await _context.Experiments
            .Include(e => e.Configuration)
            .Include(e => e.GenerationHistory.OrderBy(g => g.Generation))
            .Where(e => experimentIds.Contains(e.Id))
            .ToListAsync();
    }
    
    /// <summary>
    /// Deletes an experiment and all related data
    /// </summary>
    public async Task DeleteExperimentAsync(int id)
    {
        var experiment = await _context.Experiments.FindAsync(id);
        if (experiment != null)
        {
            _context.Experiments.Remove(experiment);
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Gets experiments filtered by dataset name
    /// </summary>
    public async Task<List<ExperimentEntity>> GetExperimentsByDatasetAsync(string datasetName)
    {
        return await _context.Experiments
            .Include(e => e.Configuration)
            .Where(e => e.DatasetName == datasetName)
            .OrderByDescending(e => e.BestFitness)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets basic statistics about experiments
    /// </summary>
    public async Task<ExperimentStatistics> GetExperimentStatisticsAsync()
    {
        var totalExperiments = await _context.Experiments.CountAsync();
        var avgBestFitness = await _context.Experiments.AverageAsync(e => e.BestFitness);
        var bestExperiment = await _context.Experiments
            .OrderByDescending(e => e.BestFitness)
            .FirstOrDefaultAsync();
            
        var uniqueDatasets = await _context.Experiments
            .Select(e => e.DatasetName)
            .Distinct()
            .CountAsync();
            
        return new ExperimentStatistics
        {
            TotalExperiments = totalExperiments,
            AverageBestFitness = avgBestFitness,
            BestExperimentId = bestExperiment?.Id,
            BestFitness = bestExperiment?.BestFitness ?? 0,
            UniqueDatasets = uniqueDatasets
        };
    }
    
    private ExperimentResult MapFromEntity(ExperimentEntity entity)
    {
        return new ExperimentResult
        {
            ExperimentName = entity.Name,
            DatasetName = entity.DatasetName,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            BestFitness = entity.BestFitness,
            FinalFitness = entity.FinalFitness,
            InitialFitness = entity.InitialFitness,
            TestAccuracy = entity.TestAccuracy,
            GenerationsCompleted = entity.GenerationsCompleted,
            TreeComplexity = entity.TreeComplexity,
            BestIndividualString = entity.BestIndividualString,
            Configuration = new ExperimentConfiguration
            {
                Description = entity.Configuration.Description,
                PopulationSize = entity.Configuration.PopulationSize,
                MaxGenerations = entity.Configuration.MaxGenerations,
                MaxTreeDepth = entity.Configuration.MaxTreeDepth,
                MaxTreeLength = entity.Configuration.MaxTreeLength,
                RandomSeed = entity.Configuration.RandomSeed,
                CrossoverProbability = entity.Configuration.CrossoverProbability,
                MutationProbability = entity.Configuration.MutationProbability,
                UseBasicMath = entity.Configuration.UseBasicMath,
                UseAdvancedMath = entity.Configuration.UseAdvancedMath,
                UseStatistics = entity.Configuration.UseStatistics,
                AllowConstants = entity.Configuration.AllowConstants,
                ProblemType = Enum.Parse<ProblemType>(entity.Configuration.ProblemType),
                ClassificationFitnessType = Enum.Parse<ClassificationFitnessType>(entity.Configuration.ClassificationFitnessType),
                RegressionFitnessType = Enum.Parse<RegressionFitnessType>(entity.Configuration.RegressionFitnessType),
                ParsimonyPressure = entity.Configuration.ParsimonyPressure,
                TreeCreationMethod = Enum.Parse<TreeCreationMethod>(entity.Configuration.TreeCreationMethod),
                MutationType = Enum.Parse<MutationType>(entity.Configuration.MutationType),
                CrossoverType = Enum.Parse<CrossoverType>(entity.Configuration.CrossoverType)
            },
            GenerationHistory = entity.GenerationHistory.Select(g => new GenerationMetrics
            {
                Generation = g.Generation,
                BestFitness = g.BestFitness,
                AverageFitness = g.AverageFitness,
                WorstFitness = g.WorstFitness,
                Diversity = g.Diversity,
                BestTreeSize = g.BestTreeSize,
                Timestamp = g.Timestamp
            }).ToList()
        };
    }

    private ExperimentEntity MapToEntity(ExperimentResult result)
    {
        var entity = new ExperimentEntity
        {
            Name = result.ExperimentName,
            Description = result.Configuration.Description,
            DatasetName = result.DatasetName,
            StartTime = result.StartTime,
            EndTime = result.EndTime,
            BestFitness = result.BestFitness,
            FinalFitness = result.FinalFitness,
            InitialFitness = result.InitialFitness,
            TestAccuracy = result.TestAccuracy,
            GenerationsCompleted = result.GenerationsCompleted,
            TreeComplexity = result.TreeComplexity,
            BestIndividualString = result.BestIndividualString,
            Configuration = new ExperimentConfigurationEntity
            {
                Description = result.Configuration.Description,
                PopulationSize = result.Configuration.PopulationSize,
                MaxGenerations = result.Configuration.MaxGenerations,
                MaxTreeDepth = result.Configuration.MaxTreeDepth,
                MaxTreeLength = result.Configuration.MaxTreeLength,
                RandomSeed = result.Configuration.RandomSeed,
                CrossoverProbability = result.Configuration.CrossoverProbability,
                MutationProbability = result.Configuration.MutationProbability,
                UseBasicMath = result.Configuration.UseBasicMath,
                UseAdvancedMath = result.Configuration.UseAdvancedMath,
                UseStatistics = result.Configuration.UseStatistics,
                AllowConstants = result.Configuration.AllowConstants,
                ProblemType = result.Configuration.ProblemType.ToString(),
                ClassificationFitnessType = result.Configuration.ClassificationFitnessType.ToString(),
                RegressionFitnessType = result.Configuration.RegressionFitnessType.ToString(),
                ParsimonyPressure = result.Configuration.ParsimonyPressure,
                TreeCreationMethod = result.Configuration.TreeCreationMethod.ToString(),
                MutationType = result.Configuration.MutationType.ToString(),
                CrossoverType = result.Configuration.CrossoverType.ToString()
            },
            GenerationHistory = result.GenerationHistory.Select(g => new GenerationMetricsEntity
            {
                Generation = g.Generation,
                BestFitness = g.BestFitness,
                AverageFitness = g.AverageFitness,
                WorstFitness = g.WorstFitness,
                Diversity = g.Diversity,
                BestTreeSize = g.BestTreeSize,
                Timestamp = g.Timestamp
            }).ToList()
        };
        
        return entity;
    }
}

public class ExperimentStatistics
{
    public int TotalExperiments { get; set; }
    public double AverageBestFitness { get; set; }
    public int? BestExperimentId { get; set; }
    public double BestFitness { get; set; }
    public int UniqueDatasets { get; set; }
}
