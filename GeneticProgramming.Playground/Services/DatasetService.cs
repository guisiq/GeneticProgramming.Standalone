using GeneticProgramming.Standalone.Tests.Integration.EndToEnd;

namespace GeneticProgramming.Playground.Services;

/// <summary>
/// Service for managing datasets used in experiments
/// </summary>
public class DatasetService
{
    /// <summary>
    /// Gets available datasets
    /// </summary>
    public List<DatasetInfo> GetAvailableDatasets()
    {
        return new List<DatasetInfo>
        {
            new DatasetInfo
            {
                Name = "Iris",
                Description = "Classic flower species classification dataset (4 features, 3 classes)",
                Features = 4,
                Samples = 150,
                Classes = 3,
                ProblemType = ProblemType.Classification
            },
            new DatasetInfo
            {
                Name = "Handwritten Digits",
                Description = "Optical recognition of handwritten digits (64 features, 10 classes)",
                Features = 64,
                Samples = 5620,
                Classes = 10,
                ProblemType = ProblemType.Classification
            },
            new DatasetInfo
            {
                Name = "Simple Linear",
                Description = "Synthetic linearly separable dataset (2 features, 2 classes)",
                Features = 2,
                Samples = 100,
                Classes = 2,
                ProblemType = ProblemType.Classification
            },
            new DatasetInfo
            {
                Name = "Circle Classification",
                Description = "Synthetic circular decision boundary (2 features, 2 classes)",
                Features = 2,
                Samples = 200,
                Classes = 2,
                ProblemType = ProblemType.Classification
            },
            new DatasetInfo
            {
                Name = "Multi-feature Statistical",
                Description = "Mean-based classification problem (3 features, 2 classes)",
                Features = 3,
                Samples = 80,
                Classes = 2,
                ProblemType = ProblemType.Classification
            }
        };
    }

    /// <summary>
    /// Loads a dataset by name
    /// </summary>
    public async Task<(double[][] inputs, int[] targets, string[] variableNames)> LoadDatasetAsync(string datasetName)
    {
        return datasetName switch
        {
            "Iris" => await DatasetManager.GetIrisDatasetAsync(),
            "Handwritten Digits" => await DatasetManager.GetDigitsDatasetAsync(),
            "Simple Linear" => GenerateSimpleLinearDataset(),
            "Circle Classification" => GenerateCircleDataset(),
            "Multi-feature Statistical" => GenerateStatisticalDataset(),
            _ => throw new ArgumentException($"Unknown dataset: {datasetName}")
        };
    }

    private (double[][] inputs, int[] targets, string[] variableNames) GenerateSimpleLinearDataset()
    {
        var random = new Random(42);
        int samples = 100;
        var inputs = new double[samples][];
        var targets = new int[samples];
        var variableNames = new[] { "X1", "X2" };

        for (int i = 0; i < samples; i++)
        {
            inputs[i] = new double[2];
            inputs[i][0] = random.NextDouble() * 10 - 5; // X1 in [-5, 5]
            inputs[i][1] = random.NextDouble() * 10 - 5; // X2 in [-5, 5]
            
            // Simple decision boundary: X1 + X2 > 0
            targets[i] = (inputs[i][0] + inputs[i][1] > 0) ? 1 : 0;
        }

        return (inputs, targets, variableNames);
    }

    private (double[][] inputs, int[] targets, string[] variableNames) GenerateCircleDataset()
    {
        var random = new Random(999);
        int samples = 200;
        var inputs = new double[samples][];
        var targets = new int[samples];
        var variableNames = new[] { "X", "Y" };

        for (int i = 0; i < samples; i++)
        {
            inputs[i] = new double[2];
            inputs[i][0] = random.NextDouble() * 20 - 10;
            inputs[i][1] = random.NextDouble() * 20 - 10;
            
            // Circle classification: inside circle = 1, outside = 0
            double distance = Math.Sqrt(inputs[i][0] * inputs[i][0] + inputs[i][1] * inputs[i][1]);
            targets[i] = distance <= 5 ? 1 : 0;
        }

        return (inputs, targets, variableNames);
    }

    private (double[][] inputs, int[] targets, string[] variableNames) GenerateStatisticalDataset()
    {
        var random = new Random(789);
        int samples = 80;
        var inputs = new double[samples][];
        var targets = new int[samples];
        var variableNames = new[] { "Feature1", "Feature2", "Feature3" };

        for (int i = 0; i < samples; i++)
        {
            inputs[i] = new double[3];
            inputs[i][0] = random.NextDouble() * 6 - 3; // [-3, 3]
            inputs[i][1] = random.NextDouble() * 6 - 3; // [-3, 3]
            inputs[i][2] = random.NextDouble() * 6 - 3; // [-3, 3]
            
            // More complex decision boundary using mean
            double mean = (inputs[i][0] + inputs[i][1] + inputs[i][2]) / 3.0;
            targets[i] = mean > 0 ? 1 : 0;
        }

        return (inputs, targets, variableNames);
    }
}

/// <summary>
/// Information about a dataset
/// </summary>
public class DatasetInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Features { get; set; }
    public int Samples { get; set; }
    public int Classes { get; set; }
    public ProblemType ProblemType { get; set; }
}
