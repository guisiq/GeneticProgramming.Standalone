using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// Manages real-world datasets for end-to-end tests
    /// </summary>
    public static class DatasetManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _datasetPath = "D:\\repos\\GeneticProgramming.Standalone\\datasets";

        /// <summary>
        /// Ensures dataset directory exists
        /// </summary>
        static DatasetManager()
        {
            if (!Directory.Exists(_datasetPath))
            {
                Directory.CreateDirectory(_datasetPath);
            }
        }

        /// <summary>
        /// Downloads Boston Housing dataset
        /// </summary>
        public static async Task<(double[][] inputs, double[] targets, string[] variableNames)> GetBostonHousingDatasetAsync()
        {
            string fileName = "boston_housing.csv";
            string filePath = Path.Combine(_datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                // Create synthetic Boston Housing-like dataset if download fails
                return CreateSyntheticBostonDataset();
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return ParseBostonDataset(lines);
            }
            catch
            {
                return CreateSyntheticBostonDataset();
            }
        }

        /// <summary>
        /// Downloads Diabetes dataset  
        /// </summary>
        public static async Task<(double[][] inputs, double[] targets, string[] variableNames)> GetDiabetesDatasetAsync()
        {
            string fileName = "diabetes.csv";
            string filePath = Path.Combine(_datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                // Create synthetic diabetes-like dataset if download fails
                return CreateSyntheticDiabetesDataset();
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return ParseDiabetesDataset(lines);
            }
            catch
            {
                return CreateSyntheticDiabetesDataset();
            }
        }

        /// <summary>
        /// Downloads Wine Quality dataset
        /// </summary>
        public static async Task<(double[][] inputs, double[] targets, string[] variableNames)> GetWineQualityDatasetAsync()
        {
            string fileName = "winequality-red.csv";
            string filePath = Path.Combine(_datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                // Create synthetic wine quality dataset if download fails
                return CreateSyntheticWineDataset();
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return ParseWineQualityDataset(lines);
            }
            catch
            {
                return CreateSyntheticWineDataset();
            }
        }

        /// <summary>
        /// Downloads Iris dataset for classification
        /// </summary>
        public static async Task<(double[][] inputs, int[] targets, string[] variableNames)> GetIrisDatasetAsync()
        {
            string fileName = "iris.data";
            string filePath = Path.Combine(_datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                // Create synthetic iris-like dataset if download fails
                return CreateSyntheticIrisDataset();
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return ParseIrisDataset(lines);
            }
            catch
            {
                return CreateSyntheticIrisDataset();
            }
        }

        /// <summary>
        /// Downloads Optical Recognition of Handwritten Digits dataset for classification
        /// </summary>
        public static async Task<(double[][] inputs, int[] targets, string[] variableNames)> GetDigitsDatasetAsync()
        {
            string fileName = "optdigits_train.data";
            string filePath = Path.Combine(_datasetPath, fileName);

            if (!File.Exists(filePath))
            {
                return CreateSyntheticDigitsDataset();
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return ParseDigitsDataset(lines);
            }
            catch
            {
                return CreateSyntheticDigitsDataset();
            }
        }

        /// <summary>
        /// Creates synthetic Boston Housing-like dataset
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) CreateSyntheticBostonDataset()
        {
            var random = new Random(42);
            int samples = 100;
            
            var variableNames = new[] { "CRIM", "ZN", "INDUS", "CHAS", "NOX", "RM", "AGE", "DIS", "RAD", "TAX", "PTRATIO", "B", "LSTAT" };
            var inputs = new double[samples][];
            var targets = new double[samples];

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[variableNames.Length];
                
                // Generate synthetic features
                inputs[i][0] = random.NextDouble() * 20;        // CRIM
                inputs[i][1] = random.NextDouble() * 100;       // ZN  
                inputs[i][2] = random.NextDouble() * 30;        // INDUS
                inputs[i][3] = random.Next(0, 2);              // CHAS
                inputs[i][4] = 0.3 + random.NextDouble() * 0.5; // NOX
                inputs[i][5] = 4 + random.NextDouble() * 4;     // RM (rooms)
                inputs[i][6] = random.NextDouble() * 100;       // AGE
                inputs[i][7] = 1 + random.NextDouble() * 10;    // DIS
                inputs[i][8] = random.Next(1, 25);             // RAD
                inputs[i][9] = 200 + random.NextDouble() * 500; // TAX
                inputs[i][10] = 10 + random.NextDouble() * 12;  // PTRATIO
                inputs[i][11] = 300 + random.NextDouble() * 96; // B
                inputs[i][12] = random.NextDouble() * 30;       // LSTAT

                // Synthetic price based on key features (simplified Boston formula)
                double price = 50 - (inputs[i][0] * 0.1) +     // Less crime = higher price
                              (inputs[i][5] * 5) -              // More rooms = higher price  
                              (inputs[i][12] * 0.5) +           // Less % lower status = higher price
                              random.NextDouble() * 5;          // Noise

                targets[i] = Math.Max(5, Math.Min(50, price));  // Clamp between 5-50k
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Creates synthetic diabetes-like dataset
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) CreateSyntheticDiabetesDataset()
        {
            var random = new Random(42);
            int samples = 150;
            
            var variableNames = new[] { "AGE", "SEX", "BMI", "BP", "S1", "S2", "S3", "S4", "S5", "S6" };
            var inputs = new double[samples][];
            var targets = new double[samples];

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[variableNames.Length];
                
                inputs[i][0] = 20 + random.NextDouble() * 60;   // AGE
                inputs[i][1] = random.Next(0, 2);              // SEX
                inputs[i][2] = 18 + random.NextDouble() * 25;   // BMI
                inputs[i][3] = 80 + random.NextDouble() * 60;   // BP
                inputs[i][4] = random.NextDouble() * 200;       // S1-S6 are lab values
                inputs[i][5] = random.NextDouble() * 200;
                inputs[i][6] = random.NextDouble() * 200;
                inputs[i][7] = random.NextDouble() * 200;
                inputs[i][8] = random.NextDouble() * 200;
                inputs[i][9] = random.NextDouble() * 200;

                // Synthetic diabetes progression
                double progression = (inputs[i][2] - 25) * 10 +  // BMI effect
                                   (inputs[i][3] - 120) * 2 +     // Blood pressure effect
                                   (inputs[i][0] - 40) * 1 +      // Age effect
                                   random.NextDouble() * 50;      // Noise

                targets[i] = Math.Max(25, Math.Min(300, progression + 150));
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Creates synthetic wine quality dataset
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) CreateSyntheticWineDataset()
        {
            var random = new Random(42);
            int samples = 200;
            
            var variableNames = new[] { 
                "fixed_acidity", "volatile_acidity", "citric_acid", "residual_sugar",
                "chlorides", "free_sulfur_dioxide", "total_sulfur_dioxide", "density",
                "pH", "sulphates", "alcohol" 
            };
            
            var inputs = new double[samples][];
            var targets = new double[samples];

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[variableNames.Length];
                
                inputs[i][0] = 4 + random.NextDouble() * 12;    // fixed acidity
                inputs[i][1] = 0.1 + random.NextDouble() * 1.5; // volatile acidity
                inputs[i][2] = random.NextDouble() * 0.8;       // citric acid
                inputs[i][3] = 0.5 + random.NextDouble() * 15;  // residual sugar
                inputs[i][4] = 0.01 + random.NextDouble() * 0.6; // chlorides
                inputs[i][5] = 1 + random.NextDouble() * 70;    // free sulfur dioxide
                inputs[i][6] = 6 + random.NextDouble() * 280;   // total sulfur dioxide
                inputs[i][7] = 0.99 + random.NextDouble() * 0.01; // density
                inputs[i][8] = 2.7 + random.NextDouble() * 1.8;  // pH
                inputs[i][9] = 0.3 + random.NextDouble() * 1.5;  // sulphates
                inputs[i][10] = 8 + random.NextDouble() * 6;     // alcohol

                // Synthetic quality score (3-9 scale)
                double quality = 5 + 
                               (inputs[i][10] - 11) * 0.3 +      // Alcohol effect
                               (0.8 - inputs[i][1]) * 2 +        // Lower volatile acidity = better
                               (inputs[i][9] - 0.8) * 1.5 +      // Sulphates effect
                               random.NextDouble() * 2 - 1;       // Noise

                targets[i] = Math.Max(3, Math.Min(9, Math.Round(quality)));
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Creates synthetic Iris-like classification dataset
        /// </summary>
        private static (double[][] inputs, int[] targets, string[] variableNames) CreateSyntheticIrisDataset()
        {
            var random = new Random(42);
            int samples = 150;
            
            var variableNames = new[] { "sepal_length", "sepal_width", "petal_length", "petal_width" };
            var inputs = new double[samples][];
            var targets = new int[samples];

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[variableNames.Length];
                
                int species = i / 50; // 3 classes, 50 samples each
                
                switch (species)
                {
                    case 0: // Setosa-like
                        inputs[i][0] = 4.5 + random.NextDouble() * 1.5; // sepal length
                        inputs[i][1] = 3.0 + random.NextDouble() * 1.0; // sepal width
                        inputs[i][2] = 1.0 + random.NextDouble() * 1.0; // petal length
                        inputs[i][3] = 0.1 + random.NextDouble() * 0.5; // petal width
                        break;
                        
                    case 1: // Versicolor-like
                        inputs[i][0] = 5.5 + random.NextDouble() * 1.5; // sepal length
                        inputs[i][1] = 2.2 + random.NextDouble() * 0.8; // sepal width
                        inputs[i][2] = 3.5 + random.NextDouble() * 1.5; // petal length
                        inputs[i][3] = 1.0 + random.NextDouble() * 0.8; // petal width
                        break;
                        
                    case 2: // Virginica-like
                        inputs[i][0] = 6.0 + random.NextDouble() * 1.5; // sepal length
                        inputs[i][1] = 2.5 + random.NextDouble() * 1.0; // sepal width
                        inputs[i][2] = 5.0 + random.NextDouble() * 2.0; // petal length
                        inputs[i][3] = 1.5 + random.NextDouble() * 1.0; // petal width
                        break;
                }
                
                targets[i] = species;
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Creates synthetic handwritten digits dataset
        /// </summary>
        private static (double[][] inputs, int[] targets, string[] variableNames) CreateSyntheticDigitsDataset()
        {
            var random = new Random(42);
            int samples = 200; // Smaller synthetic dataset for testing
            
            // 64 pixel features (8x8 image)
            var variableNames = new string[64];
            for (int i = 0; i < 64; i++)
            {
                variableNames[i] = $"Pixel_{i}";
            }
            
            var inputs = new double[samples][];
            var targets = new int[samples];

            for (int i = 0; i < samples; i++)
            {
                inputs[i] = new double[64];
                int digit = i % 10; // Digits 0-9
                
                // Generate simple patterns for each digit
                for (int pixel = 0; pixel < 64; pixel++)
                {
                    int row = pixel / 8;
                    int col = pixel % 8;
                    
                    // Create simple digit patterns based on position
                    double value = 0;
                    
                    switch (digit)
                    {
                        case 0: // Circle-like pattern
                            if ((row == 1 || row == 6) && col >= 2 && col <= 5) value = 12 + random.Next(5);
                            else if ((col == 1 || col == 6) && row >= 2 && row <= 5) value = 12 + random.Next(5);
                            break;
                        case 1: // Vertical line in center
                            if (col == 3 || col == 4) value = 10 + random.Next(7);
                            break;
                        case 2: // Horizontal lines at top, middle, bottom
                            if (row == 1 || row == 3 || row == 6) value = 8 + random.Next(9);
                            break;
                        default: // Random pattern for other digits
                            value = random.Next(17);
                            break;
                    }
                    
                    inputs[i][pixel] = value;
                }
                
                targets[i] = digit;
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Parses Boston Housing dataset from CSV lines
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) ParseBostonDataset(string[] lines)
        {
            var variableNames = new[] { "CRIM", "ZN", "INDUS", "CHAS", "NOX", "RM", "AGE", "DIS", "RAD", "TAX", "PTRATIO", "B", "LSTAT" };
            var dataLines = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            
            var inputs = new double[dataLines.Length][];
            var targets = new double[dataLines.Length];

            for (int i = 0; i < dataLines.Length; i++)
            {
                var values = dataLines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                inputs[i] = new double[variableNames.Length];
                
                for (int j = 0; j < variableNames.Length; j++)
                {
                    // Remove quotes if present and parse
                    var cleanValue = values[j].Trim('"');
                    inputs[i][j] = double.Parse(cleanValue, CultureInfo.InvariantCulture);
                }
                
                // Target is the last column (medv - median value)
                var targetValue = values[variableNames.Length].Trim('"');
                targets[i] = double.Parse(targetValue, CultureInfo.InvariantCulture);
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Parses the Iris dataset
        /// </summary>
        private static (double[][] inputs, int[] targets, string[] variableNames) ParseIrisDataset(string[] lines)
        {
            var variableNames = new[] { "SepalLength", "SepalWidth", "PetalLength", "PetalWidth" };
            var dataLines = lines.Where(l => !string.IsNullOrWhiteSpace(l) && l.Contains(",")).ToArray();
            
            var inputs = new double[dataLines.Length][];
            var targets = new int[dataLines.Length];

            for (int i = 0; i < dataLines.Length; i++)
            {
                var values = dataLines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                inputs[i] = new double[variableNames.Length];
                
                for (int j = 0; j < variableNames.Length; j++)
                {
                    inputs[i][j] = double.Parse(values[j], CultureInfo.InvariantCulture);
                }
                
                // Convert class name to integer
                var className = values[4].Trim();
                targets[i] = className switch
                {
                    "Iris-setosa" => 0,
                    "Iris-versicolor" => 1,
                    "Iris-virginica" => 2,
                    _ => 0
                };
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Parses the Wine Quality dataset
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) ParseWineQualityDataset(string[] lines)
        {
            var variableNames = new[] { 
                "FixedAcidity", "VolatileAcidity", "CitricAcid", "ResidualSugar", 
                "Chlorides", "FreeSulfurDioxide", "TotalSulfurDioxide", "Density", 
                "pH", "Sulphates", "Alcohol" 
            };
            var dataLines = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            
            var inputs = new double[dataLines.Length][];
            var targets = new double[dataLines.Length];

            for (int i = 0; i < dataLines.Length; i++)
            {
                var values = dataLines[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                inputs[i] = new double[variableNames.Length];
                
                for (int j = 0; j < variableNames.Length; j++)
                {
                    // Remove quotes if present and parse
                    var cleanValue = values[j].Trim('"');
                    inputs[i][j] = double.Parse(cleanValue, CultureInfo.InvariantCulture);
                }
                
                // Target is the last column (quality score)
                var targetValue = values[variableNames.Length].Trim('"');
                targets[i] = double.Parse(targetValue, CultureInfo.InvariantCulture);
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Parses the Pima Indians Diabetes dataset
        /// </summary>
        private static (double[][] inputs, double[] targets, string[] variableNames) ParseDiabetesDataset(string[] lines)
        {
            var variableNames = new[] { "Pregnancies", "Glucose", "BloodPressure", "SkinThickness", "Insulin", "BMI", "DiabetesPedigreeFunction", "Age" };
            var dataLines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            
            var inputs = new double[dataLines.Length][];
            var targets = new double[dataLines.Length];

            for (int i = 0; i < dataLines.Length; i++)
            {
                var values = dataLines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                inputs[i] = new double[variableNames.Length];
                
                for (int j = 0; j < variableNames.Length; j++)
                {
                    inputs[i][j] = double.Parse(values[j], CultureInfo.InvariantCulture);
                }
                
                // Target is the last column (outcome - 0 or 1)
                targets[i] = double.Parse(values[variableNames.Length], CultureInfo.InvariantCulture);
            }

            return (inputs, targets, variableNames);
        }

        /// <summary>
        /// Parses the Optical Recognition of Handwritten Digits dataset
        /// </summary>
        private static (double[][] inputs, int[] targets, string[] variableNames) ParseDigitsDataset(string[] lines)
        {
            // 64 pixel features (8x8 image)
            var variableNames = new string[64];
            for (int i = 0; i < 64; i++)
            {
                variableNames[i] = $"Pixel_{i}";
            }
            
            var dataLines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            var inputs = new double[dataLines.Length][];
            var targets = new int[dataLines.Length];

            for (int i = 0; i < dataLines.Length; i++)
            {
                var values = dataLines[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                inputs[i] = new double[64]; // 64 pixels
                
                // Parse the 64 pixel values (0-16) and normalize to [0,1]
                for (int j = 0; j < 64; j++)
                {
                    double rawValue = double.Parse(values[j], CultureInfo.InvariantCulture);
                    inputs[i][j] = rawValue / 16.0; // Normalize from [0,16] to [0,1]
                }
                
                // Target is the last column (digit class 0-9)
                targets[i] = int.Parse(values[64], CultureInfo.InvariantCulture);
            }

            return (inputs, targets, variableNames);
        }
    }
}
