using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// Debug tests to verify dataset loading paths
    /// </summary>
    public class DatasetDebugTests
    {
        private readonly ITestOutputHelper _output;

        public DatasetDebugTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DatasetFiles_ShouldExist()
        {
            var currentDir = System.Environment.CurrentDirectory;
            _output.WriteLine($"Current directory: {currentDir}");

            var datasetPaths = new[]
            {
                Path.Combine(currentDir, "datasets", "boston_housing.csv"),
                Path.Combine(currentDir, "datasets", "iris.data"),
                Path.Combine(currentDir, "datasets", "diabetes.csv"),
                Path.Combine(currentDir, "datasets", "winequality-red.csv"),
                Path.Combine(currentDir, "datasets", "optdigits_train.data"),
                Path.Combine(currentDir, "datasets", "optdigits_test.data")
            };

            foreach (var path in datasetPaths)
            {
                _output.WriteLine($"Checking: {path}");
                _output.WriteLine($"Exists: {File.Exists(path)}");
                if (File.Exists(path))
                {
                    var info = new FileInfo(path);
                    _output.WriteLine($"Size: {info.Length} bytes");
                }
            }
        }

        [Fact]
        public async Task BostonHousing_ShouldLoadFromRealFile()
        {
            var (inputs, targets, vars) = await DatasetManager.GetBostonHousingDatasetAsync();
            _output.WriteLine($"Boston Housing loaded: {inputs.Length} samples, {vars.Length} variables");
            _output.WriteLine($"Variables: {string.Join(", ", vars)}");
            
            if (inputs.Length > 0)
            {
                _output.WriteLine($"First sample: [{string.Join(", ", inputs[0])}] -> {targets[0]}");
            }
        }

        [Fact]
        public async Task Iris_ShouldLoadFromRealFile()
        {
            var (inputs, targets, vars) = await DatasetManager.GetIrisDatasetAsync();
            _output.WriteLine($"Iris loaded: {inputs.Length} samples, {vars.Length} variables");
            _output.WriteLine($"Variables: {string.Join(", ", vars)}");
            
            if (inputs.Length > 0)
            {
                _output.WriteLine($"First sample: [{string.Join(", ", inputs[0])}] -> {targets[0]}");
            }
        }

        [Fact]
        public async Task Digits_ShouldLoadFromRealFile()
        {
            var (inputs, targets, vars) = await DatasetManager.GetDigitsDatasetAsync();
            _output.WriteLine($"Handwritten Digits loaded: {inputs.Length} samples, {vars.Length} variables");
            _output.WriteLine($"Variables: {vars[0]}, {vars[1]}, ..., {vars[62]}, {vars[63]}");
            
            if (inputs.Length > 0)
            {
                // Show first few pixels and target
                var firstPixels = string.Join(", ", inputs[0][0..10]); // First 10 pixels
                _output.WriteLine($"First sample (first 10 pixels): [{firstPixels}...] -> digit {targets[0]}");
                
                // Show sample distribution
                var digitCounts = new int[10];
                foreach (var target in targets)
                {
                    if (target >= 0 && target <= 9)
                        digitCounts[target]++;
                }
                _output.WriteLine($"Digit distribution: {string.Join(", ", digitCounts.Select((count, digit) => $"{digit}:{count}"))}");
            }
        }
    }
}
