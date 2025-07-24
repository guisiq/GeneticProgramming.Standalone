using Microsoft.EntityFrameworkCore;
using GeneticProgramming.Playground.Data;

namespace GeneticProgramming.Playground.Services;

public class DatabaseConfigurationService
{
    private string _currentDatabasePath = @"C:\GP_Experiments\experiments.db";
    private readonly IServiceProvider _serviceProvider;

    public DatabaseConfigurationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string CurrentDatabasePath => _currentDatabasePath;

    public event Action<string>? DatabasePathChanged;

    /// <summary>
    /// Sets a new database path and ensures the directory exists
    /// </summary>
    public async Task<bool> SetDatabasePathAsync(string newPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newPath))
                return false;

            // Ensure directory exists
            var directory = Path.GetDirectoryName(newPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _currentDatabasePath = newPath;
            
            // Create new DbContext with the new path
            var connectionString = $"Data Source={newPath}";
            var optionsBuilder = new DbContextOptionsBuilder<ExperimentDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            using var context = new ExperimentDbContext(optionsBuilder.Options);
            await context.Database.EnsureCreatedAsync();

            DatabasePathChanged?.Invoke(_currentDatabasePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Tests if the current database path is accessible
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var connectionString = $"Data Source={_currentDatabasePath}";
            var optionsBuilder = new DbContextOptionsBuilder<ExperimentDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            using var context = new ExperimentDbContext(optionsBuilder.Options);
            return await context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new DbContext with the current database path
    /// </summary>
    public ExperimentDbContext CreateDbContext()
    {
        var connectionString = $"Data Source={_currentDatabasePath}";
        var optionsBuilder = new DbContextOptionsBuilder<ExperimentDbContext>();
        optionsBuilder.UseSqlite(connectionString);
        
        return new ExperimentDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Initializes the database with the current path
    /// </summary>
    public async Task<bool> InitializeDatabaseAsync()
    {
        try
        {
            using var context = CreateDbContext();
            await context.Database.EnsureCreatedAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets information about the current database
    /// </summary>
    public async Task<DatabaseInfo> GetDatabaseInfoAsync()
    {
        try
        {
            using var context = CreateDbContext();
            var experimentCount = await context.Experiments.CountAsync();
            var fileInfo = new FileInfo(_currentDatabasePath);
            
            return new DatabaseInfo
            {
                Path = _currentDatabasePath,
                Exists = fileInfo.Exists,
                Size = fileInfo.Exists ? fileInfo.Length : 0,
                ExperimentCount = experimentCount,
                LastModified = fileInfo.Exists ? fileInfo.LastWriteTime : null
            };
        }
        catch
        {
            return new DatabaseInfo
            {
                Path = _currentDatabasePath,
                Exists = false,
                Size = 0,
                ExperimentCount = 0,
                LastModified = null
            };
        }
    }
}

public class DatabaseInfo
{
    public string Path { get; set; } = "";
    public bool Exists { get; set; }
    public long Size { get; set; }
    public int ExperimentCount { get; set; }
    public DateTime? LastModified { get; set; }
}
