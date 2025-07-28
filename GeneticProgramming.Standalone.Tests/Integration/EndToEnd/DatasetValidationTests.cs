using System.Threading.Tasks;
using Xunit;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// Tests to validate that real datasets are being loaded correctly
    /// </summary>
    public class DatasetValidationTests
    {
        [Fact]
        public async Task RealDatasets_ShouldHaveExpectedSizes()
        {
            // Test Boston Housing
            var (bostonInputs, bostonTargets, bostonVars) = await DatasetManager.GetBostonHousingDatasetAsync();
            Assert.True(bostonInputs.Length >= 500, $"Boston Housing should have >= 500 samples, got {bostonInputs.Length}");
            Assert.Equal(13, bostonVars.Length);
            Assert.Equal(bostonInputs.Length, bostonTargets.Length);

            // Test Iris
            var (irisInputs, irisTargets, irisVars) = await DatasetManager.GetIrisDatasetAsync();
            Assert.Equal(150, irisInputs.Length); // Iris has exactly 150 samples
            Assert.Equal(4, irisVars.Length);
            Assert.Equal(irisInputs.Length, irisTargets.Length);

            // Test Diabetes
            var (diabetesInputs, diabetesTargets, diabetesVars) = await DatasetManager.GetDiabetesDatasetAsync();
            Assert.Equal(768, diabetesInputs.Length); // Pima Indians Diabetes has exactly 768 samples
            Assert.Equal(8, diabetesVars.Length);
            Assert.Equal(diabetesInputs.Length, diabetesTargets.Length);

            // Test Wine Quality
            var (wineInputs, wineTargets, wineVars) = await DatasetManager.GetWineQualityDatasetAsync();
            Assert.True(wineInputs.Length >= 1500, $"Wine Quality should have >= 1500 samples, got {wineInputs.Length}");
            Assert.Equal(11, wineVars.Length);
            Assert.Equal(wineInputs.Length, wineTargets.Length);

            // Test Handwritten Digits
            var (digitsInputs, digitsTargets, digitsVars) = await DatasetManager.GetDigitsDatasetAsync();
            Assert.True(digitsInputs.Length >= 200, $"Digits should have >= 200 samples, got {digitsInputs.Length}");
            Assert.Equal(64, digitsVars.Length); // 8x8 pixels = 64 features
            Assert.Equal(digitsInputs.Length, digitsTargets.Length);
        }

        [Fact]
        public async Task RealDatasets_ShouldHaveValidRanges()
        {
            // Test Iris - sepal and petal measurements should be reasonable
            var (irisInputs, irisTargets, _) = await DatasetManager.GetIrisDatasetAsync();
            
            // Check first sample (should be close to: 5.1,3.5,1.4,0.2)
            Assert.True(irisInputs[0][0] > 4.0 && irisInputs[0][0] < 6.0, "First Iris sepal length should be around 5.1");
            Assert.True(irisInputs[0][1] > 3.0 && irisInputs[0][1] < 4.0, "First Iris sepal width should be around 3.5");
            
            // Check target classes are 0, 1, or 2
            foreach (var target in irisTargets)
            {
                Assert.True(target >= 0 && target <= 2, $"Iris target should be 0, 1, or 2, got {target}");
            }

            // Test Handwritten Digits - pixel values should be 0-16, targets 0-9
            var (digitsInputs, digitsTargets, _) = await DatasetManager.GetDigitsDatasetAsync();
            
            // Check pixel values are in valid range (0-16)
            foreach (var sample in digitsInputs.Take(10)) // Check first 10 samples
            {
                foreach (var pixel in sample)
                {
                    Assert.True(pixel >= 0 && pixel <= 16, $"Digit pixel value should be 0-16, got {pixel}");
                }
            }
            
            // Check target classes are 0-9
            foreach (var target in digitsTargets)
            {
                Assert.True(target >= 0 && target <= 9, $"Digit target should be 0-9, got {target}");
            }
        }

        [Fact]
        public async Task RealDatasets_ShouldHaveCorrectVariableNames()
        {
            // Test Boston Housing variable names
            var (_, _, bostonVars) = await DatasetManager.GetBostonHousingDatasetAsync();
            Assert.Contains("CRIM", bostonVars);
            Assert.Contains("RM", bostonVars);
            Assert.Contains("LSTAT", bostonVars);

            // Test Iris variable names
            var (_, _, irisVars) = await DatasetManager.GetIrisDatasetAsync();
            Assert.Contains("SepalLength", irisVars);
            Assert.Contains("PetalLength", irisVars);

            // Test Diabetes variable names
            var (_, _, diabetesVars) = await DatasetManager.GetDiabetesDatasetAsync();
            Assert.Contains("Glucose", diabetesVars);
            Assert.Contains("BMI", diabetesVars);

            // Test Wine Quality variable names
            var (_, _, wineVars) = await DatasetManager.GetWineQualityDatasetAsync();
            Assert.Contains("FixedAcidity", wineVars);
            Assert.Contains("Alcohol", wineVars);

            // Test Handwritten Digits variable names
            var (_, _, digitsVars) = await DatasetManager.GetDigitsDatasetAsync();
            Assert.Contains("Pixel_0", digitsVars);
            Assert.Contains("Pixel_63", digitsVars);
            Assert.Equal(64, digitsVars.Length); // Should have exactly 64 pixel variables
        }
    }
}
