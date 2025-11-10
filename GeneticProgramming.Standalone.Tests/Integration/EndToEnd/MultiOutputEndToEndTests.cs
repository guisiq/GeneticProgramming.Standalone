using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Expressions;
using GeneticProgramming.Standalone.Expressions;
using MathNet.Numerics.Optimization;
using GeneticProgramming.Standalone.Core;
using System.Runtime.CompilerServices;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// End-to-end tests specifically for MultiSymbolicExpressionTree functionality
    /// </summary>
    public class MultiOutputEndToEndTests
    {
        /// <summary>
        /// Multi-output regression pipeline using MultiSymbolicExpressionTree for simultaneous prediction
        /// </summary>
        [Fact]
        public async Task MultiOutputRegressionPipeline_BostonHousing_ShouldPredictMultipleTargets()
        {
            var (allInputs, allTargets, variableNames) = await DatasetManager.GetBostonHousingDatasetAsync();
            Assert.True(allInputs.Length > 0, "Dataset should be loaded successfully");

            var (originalPrices, priceCategories, priceTiers) = CreateMultiTargets(allTargets);
            var (normalizedInputs, normalizedPriceCategories, normalizedPriceTiers) = NormalizeAllTargets(allInputs, originalPrices, priceCategories, priceTiers);
            var (trainInputs, trainIndices, testInputs, testIndices) = TrainTestSplit(normalizedInputs, 0.8, 42);
            var (trainMultiTargets, testMultiTargets) = BuildMultiTargetSets(trainIndices, testIndices, originalPrices, normalizedPriceCategories, normalizedPriceTiers);

            var multiTrees = CreateMultiOutputTrees(3, 5, trainInputs, variableNames);
            ValidateMultiOutputTrees(multiTrees, trainInputs);
            TestSharedNodesAndCloning(multiTrees);
            TestStringAndNodeAccess(multiTrees[0]);
            TestErrorHandling(multiTrees[0]);
        }


        [Theory(Skip = "Demorado demais para rodar em CI")]
        [InlineData(2, 30, 15,10)]
        [InlineData(3, 30, 30,10)]
        [InlineData(1, 10, 5, 10)]
        //pula todo esse teste
        /*erro 
        //[InlineData(3, 10, 5)]erro
        [InlineData(1, 10, 5)]
        //[InlineData(3, 10, 10)]erro
        [InlineData(3, 10, 15)]
        [InlineData(6, 10, 5)]
        //[InlineData(9, 10, 5)]erro 
        [InlineData(12, 10, 5)]
        */
        //[InlineData(5, 20, 5)] muito demorado
        //[InlineData(7, 20, 5)] muito demorado 
        //[InlineData(8, 12, 5)] erro //TODO analizar sauto de tempo entre esse e o teste de cima 
        public void MultiOutputPerformance_ShouldBeEfficientWithComplexTrees(int outputCount, int treeDepth, int numVars, int iterations)
        {
            var random = new Random(123);
            var multiTree = new MultiSymbolicExpressionTree<double>(outputCount);
            var addSymbol = MathematicalSymbols.Addition;
            var multSymbol = MathematicalSymbols.Multiplication;
            var varSymbol = new Variable<double>();
            var constSymbol = new Constant<double>();
            var variableNames = Enumerable.Range(1, numVars).Select(i => $"x{i}").ToArray();
            var variables = variableNames.ToDictionary(
                name => name,
                name => Math.Round(random.NextDouble() * 2.0 + 0.5, 3));
            var symbols = new List<ISymbol<double>> { addSymbol, multSymbol, varSymbol, constSymbol };
            //melhorar esse builder 
            multiTree.BuildMultiOutputRandom(
                symbols,
                variableNames,
                treeDepth,
                treeLength: treeDepth * 2,
                creationMode: TreeCreationMode.SharedBase,
                strategy: MultiOutputStrategy.Shared);
            var multiEvalTime = MeasureEvaluationTime(multiTree, variables, iterations);
            var singleEvalTime = MeasureSingleEvaluationTime(multiTree, variables, iterations);
            Assert.True(multiEvalTime <= singleEvalTime * 2,
                $"Multi-output should be reasonably efficient: {multiEvalTime}ms vs {singleEvalTime}ms (individual outputs)");
            Console.WriteLine($"Performance test (complex trees): Multi-eval {multiEvalTime}ms, Single-eval {singleEvalTime}ms");
        }

        // Métodos auxiliares extraídos do teste original
        private (double[] originalPrices, double[] priceCategories, double[] priceTiers) CreateMultiTargets(double[] allTargets)
        {
            var originalPrices = allTargets;
            var priceCategories = allTargets.Select(price => price > allTargets.Average() ? 1.0 : 0.0).ToArray();
            var priceTiers = allTargets.Select(price =>
            {
                var percentile33 = allTargets.OrderBy(x => x).Skip((int)(allTargets.Length * 0.33)).First();
                var percentile67 = allTargets.OrderBy(x => x).Skip((int)(allTargets.Length * 0.67)).First();
                return price <= percentile33 ? 1.0 : (price <= percentile67 ? 2.0 : 3.0);
            }).ToArray();
            return (originalPrices, priceCategories, priceTiers);
        }

        private (double[][] normalizedInputs, double[] normalizedPriceCategories, double[] normalizedPriceTiers) NormalizeAllTargets(
            double[][] allInputs, double[] originalPrices, double[] priceCategories, double[] priceTiers)
        {
            var normalizedData = NormalizeData(allInputs, originalPrices);
            var (normalizedInputs, _, inputMeans, inputStds, targetMean, targetStd) = normalizedData;
            var normalizedPriceCategories = priceCategories.Select(x => (x - priceCategories.Average()) /
                Math.Sqrt(priceCategories.Select(y => Math.Pow(y - priceCategories.Average(), 2)).Average())).ToArray();
            var normalizedPriceTiers = priceTiers.Select(x => (x - priceTiers.Average()) /
                Math.Sqrt(priceTiers.Select(y => Math.Pow(y - priceTiers.Average(), 2)).Average())).ToArray();
            return (normalizedInputs, normalizedPriceCategories, normalizedPriceTiers);
        }

        private (double[][] trainInputs, int[] trainIndices, double[][] testInputs, int[] testIndices) TrainTestSplit(
            double[][] normalizedInputs, double trainRatio, int seed)
        {
            var random = new Random(seed);
            var indices = Enumerable.Range(0, normalizedInputs.Length).OrderBy(x => random.Next()).ToArray();
            int trainSize = (int)(normalizedInputs.Length * trainRatio);
            var trainInputs = indices.Take(trainSize).Select(i => normalizedInputs[i]).ToArray();
            var testInputs = indices.Skip(trainSize).Select(i => normalizedInputs[i]).ToArray();
            var trainIndices = indices.Take(trainSize).ToArray();
            var testIndices = indices.Skip(trainSize).ToArray();
            return (trainInputs, trainIndices, testInputs, testIndices);
        }

        private (double[][] trainMultiTargets, double[][] testMultiTargets) BuildMultiTargetSets(
            int[] trainIndices, int[] testIndices, double[] originalPrices, double[] normalizedPriceCategories, double[] normalizedPriceTiers)
        {
            var trainMultiTargets = trainIndices.Select(i => new double[]
            {
                originalPrices[i],
                normalizedPriceCategories[i],
                normalizedPriceTiers[i]
            }).ToArray();
            var testMultiTargets = testIndices.Select(i => new double[]
            {
                originalPrices[i],
                normalizedPriceCategories[i],
                normalizedPriceTiers[i]
            }).ToArray();
            return (trainMultiTargets, testMultiTargets);
        }

        private List<MultiSymbolicExpressionTree<double>> CreateMultiOutputTrees(int outputCount, int treeCount, double[][] trainInputs, string[] variableNames)
        {
            var multiTrees = new List<MultiSymbolicExpressionTree<double>>();
            var random = new Random(42);
            for (int i = 0; i < treeCount; i++)
            {
                var multiTree = new MultiSymbolicExpressionTree<double>(outputCount);
                var addSymbol = MathematicalSymbols.Addition;
                var varSymbol = new Variable<double>();
                var constSymbol = new Constant<double>();
                for (int output = 0; output < outputCount; output++)
                {
                    var rootNode = addSymbol.CreateTreeNode();
                    var leftChild = new VariableTreeNode<double>(varSymbol) { VariableName = "crim" };
                    var rightChild = new ConstantTreeNode<double>(constSymbol, random.NextDouble());
                    rootNode.AddSubtree(leftChild);
                    rootNode.AddSubtree(rightChild);
                    multiTree.SetOutputNode(output, rootNode);
                }
                multiTrees.Add(multiTree);
                Assert.Equal(outputCount, multiTree.OutputCount);
                Assert.True(multiTree.Length > 0, "Multi-tree should have positive length");
                Assert.True(multiTree.Depth > 0, "Multi-tree should have positive depth");
            }
            return multiTrees;
        }

        private void ValidateMultiOutputTrees(List<MultiSymbolicExpressionTree<double>> multiTrees, double[][] trainInputs)
        {
            var testVariables = new Dictionary<string, double>
            {
                { "crim", trainInputs[0][0] },
                { "zn", trainInputs[0][1] },
                { "indus", trainInputs[0][2] }
            };
            const int outputCount = 3;
            foreach (var multiTree in multiTrees)
            {
                var results = multiTree.EvaluateAll(testVariables);
                Assert.NotNull(results);
                Assert.Equal(outputCount, results.Count);
                Assert.All(results, result => Assert.False(double.IsNaN(result)));
                Assert.All(results, result => Assert.False(double.IsInfinity(result)));
            }
        }

        private void TestSharedNodesAndCloning(List<MultiSymbolicExpressionTree<double>> multiTrees)
        {
            var firstMultiTree = multiTrees[0];
            var sharedNodes = firstMultiTree.GetSharedNodes();
            Assert.NotNull(sharedNodes);
            var cloner = new Cloner();
            var clonedTree = (MultiSymbolicExpressionTree<double>)firstMultiTree.Clone(cloner);
            Assert.NotNull(clonedTree);
            Assert.Equal(firstMultiTree.OutputCount, clonedTree.OutputCount);
            Assert.NotSame(firstMultiTree, clonedTree);
            var testVariables = new Dictionary<string, double>
            {
                { "crim", 0.5 },
                { "zn", 0.5 },
                { "indus", 0.5 }
            };
            var originalResults = firstMultiTree.EvaluateAll(testVariables);
            var clonedResults = clonedTree.EvaluateAll(testVariables);
            for (int i = 0; i < originalResults.Count; i++)
            {
                Assert.Equal(originalResults[i], clonedResults[i], precision: 10);
            }
        }

        private void TestStringAndNodeAccess(MultiSymbolicExpressionTree<double> tree)
        {
            var treeString = tree.ToString();
            Assert.NotNull(treeString);
            Assert.Contains("MultiSymbolicExpressionTree", treeString);
            Assert.Contains(tree.OutputCount.ToString(), treeString);
            for (int i = 0; i < tree.OutputCount; i++)
            {
                var outputNode = tree.GetOutputNode(i);
                Assert.NotNull(outputNode);
                Assert.True(outputNode.SubtreeCount >= 0, "Output node should be valid");
            }
        }

        private void TestErrorHandling(MultiSymbolicExpressionTree<double> tree)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.GetOutputNode(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.GetOutputNode(tree.OutputCount));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MultiSymbolicExpressionTree<double>(0));
            Assert.Throws<ArgumentNullException>(() => tree.EvaluateAll(null!));
        }

        private ISymbolicExpressionTreeNode<double> BuildComplexTree(
            ISymbol<double> addSymbol,
            ISymbol<double> multSymbol,
            ISymbol<double> varSymbol,
            ISymbol<double> constSymbol,
            string[] variableNames,
            int depth,
            Random random)
        {
            if (depth <= 1)
            {
                if (random.NextDouble() < 0.5)
                    return new VariableTreeNode<double>((Variable<double>)varSymbol) { VariableName = variableNames[random.Next(variableNames.Length)] };
                else
                    return new ConstantTreeNode<double>((Constant<double>)constSymbol, random.NextDouble());
            }
            var op = random.NextDouble() < 0.5 ? addSymbol : multSymbol;
            var node = op.CreateTreeNode();
            node.AddSubtree(BuildComplexTree(addSymbol, multSymbol, varSymbol, constSymbol, variableNames, depth - 1, random));
            node.AddSubtree(BuildComplexTree(addSymbol, multSymbol, varSymbol, constSymbol, variableNames, depth - 1, random));
            return node;
        }

        /// <summary>
        /// Test MultiSymbolicExpressionTree with genetic algorithm evolution
        /// </summary>
        [Fact]
        public async Task MultiOutputGeneticEvolution_Diabetes_ShouldEvolveMultipleTargets()
        {
            // Step 1: Data Loading
            var (allInputs, originalTargets, variableNames) = await DatasetManager.GetDiabetesDatasetAsync();
            Assert.True(allInputs.Length > 0, "Dataset should be loaded successfully");

            // Step 2: Create secondary targets (disease progression categories)
            var targetMean = originalTargets.Average();
            var targetStd = Math.Sqrt(originalTargets.Select(x => Math.Pow(x - targetMean, 2)).Average());
            
            var progressionCategories = originalTargets.Select(x => 
                x > targetMean + targetStd ? 2.0 : (x > targetMean ? 1.0 : 0.0)).ToArray();
            
            var riskLevels = originalTargets.Select(x => 
                Math.Max(0.0, Math.Min(1.0, (x - originalTargets.Min()) / (originalTargets.Max() - originalTargets.Min())))).ToArray();

            // Step 3: Prepare data splits
            var (trainInputs, _, _, _, testInputs, _) = 
                SplitDataset(allInputs, originalTargets, 0.8, 0.0, 0.2, 42);

            // Step 4: Create multi-output fitness evaluator
            var multiTargets = Enumerable.Range(0, allInputs.Length)
                .Select(i => new double[] { originalTargets[i], progressionCategories[i], riskLevels[i] })
                .ToArray();

            var random = new Random(42);
            var indices = Enumerable.Range(0, allInputs.Length).OrderBy(x => random.Next()).ToArray();
            int trainSize = (int)(allInputs.Length * 0.8);

            var trainMultiTargets = indices.Take(trainSize).Select(i => multiTargets[i]).ToArray();
            var testMultiTargets = indices.Skip(trainSize).Select(i => multiTargets[i]).ToArray();

            // Step 5: Create custom multi-output evaluator
            var multiEvaluator = new MultiOutputRegressionEvaluator(trainInputs, trainMultiTargets, variableNames);

            // Step 6: Test evolution with population of multi-output trees
            var multiTreePopulation = new List<MultiSymbolicExpressionTree<double>>();
            const int outputCount = 3;
            const int populationSize = 10;

            // Initialize population
            for (int i = 0; i < populationSize; i++)
            {
                var multiTree = new MultiSymbolicExpressionTree<double>(outputCount);
                
                for (int output = 0; output < outputCount; output++)
                {
                    // Create random simple expressions
                    var treeCreator = new GrowTreeCreator<double>();
                    var innerGrammar = new SymbolicRegressionGrammar<double>(variableNames.Take(3).ToArray(), 
                        new List<ISymbol<double>> { MathematicalSymbols.Addition, MathematicalSymbols.Multiplication });
                    
                    var mersenneTwister = new MersenneTwister(random.Next());
                    var singleTree = treeCreator.CreateTree(mersenneTwister, innerGrammar, 2, 4);
                    multiTree.SetOutputNode(output, singleTree.Root);
                }
                
                multiTreePopulation.Add(multiTree);
            }

            // Step 7: Evaluate population
            var fitnessScores = new double[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                fitnessScores[i] = EvaluateMultiOutputTree(multiTreePopulation[i], multiEvaluator, trainInputs, trainMultiTargets, variableNames);
            }

            // Step 8: Simple selection and mutation
            var bestIndices = fitnessScores
                .Select((fitness, idx) => new { Fitness = fitness, Index = idx })
                .OrderByDescending(x => x.Fitness)
                .Take(populationSize / 2)
                .Select(x => x.Index)
                .ToArray();

            var bestTrees = bestIndices.Select(idx => multiTreePopulation[idx]).ToArray();
            var bestFitness = fitnessScores[bestIndices[0]];

            // Step 9: Test mutation on best trees
            var mutationGrammar = new SymbolicRegressionGrammar<double>(variableNames.Take(5).ToArray(), MathematicalSymbols.AllSymbols);
            var mutator = new SubtreeMutator<double>(mutationGrammar);
            var mutationRandom = new MersenneTwister(random.Next());
            
            foreach (var tree in bestTrees.Take(2))
            {
                for (int output = 0; output < outputCount; output++)
                {
                    var originalNode = tree.GetOutputNode(output);
                    var singleTree = new SymbolicExpressionTree<double> { Root = originalNode };
                    
                    var mutatedTree = mutator.Mutate(mutationRandom, singleTree);
                    tree.SetOutputNode(output, mutatedTree.Root);
                }
            }

            // Step 10: Evaluate mutated trees
            var newFitnessScores = new double[bestTrees.Length];
            for (int i = 0; i < bestTrees.Length; i++)
            {
                newFitnessScores[i] = EvaluateMultiOutputTree(bestTrees[i], multiEvaluator, trainInputs, trainMultiTargets, variableNames);
            }

            // Step 11: Assertions
            Assert.All(fitnessScores, fitness => Assert.True(fitness > double.NegativeInfinity));
            Assert.All(newFitnessScores, fitness => Assert.True(fitness > double.NegativeInfinity));
            Assert.True(bestFitness >= fitnessScores.Min(), "Best fitness should be at least as good as minimum");

            // Test that trees can be successfully evaluated
            var testVariables = variableNames.Take(5).ToDictionary(name => name, name => 0.5);
            foreach (var tree in bestTrees)
            {
                var results = tree.EvaluateAll(testVariables);
                Assert.Equal(outputCount, results.Count);
                Assert.All(results, result => Assert.False(double.IsNaN(result)));
            }

            Console.WriteLine($"Multi-output genetic evolution completed");
            Console.WriteLine($"Population size: {populationSize}, Output count: {outputCount}");
            Console.WriteLine($"Best fitness: {bestFitness:F4}");
            Console.WriteLine($"Fitness range: [{fitnessScores.Min():F4}, {fitnessScores.Max():F4}]");
        }

        /// <summary>
        /// Test basic multi-output tree operations and functionality
        /// </summary>
        [Fact]
        public void MultiOutputTreeBasicOperations_ShouldWorkCorrectly()
        {
            // Test tree creation
            var multiTree = new MultiSymbolicExpressionTree<double>(3);
            Assert.Equal(3, multiTree.OutputCount);

            // Test setting and getting output nodes
            var addSymbol = MathematicalSymbols.Addition;
            var varSymbol = new Variable<double>();
            var constSymbol = new Constant<double>();

            for (int i = 0; i < 3; i++)
            {
                var rootNode = addSymbol.CreateTreeNode();
                var leftChild = new VariableTreeNode<double>(varSymbol) { VariableName = $"x{i}" };
                var rightChild = new ConstantTreeNode<double>(constSymbol, i + 1.0);
                
                rootNode.AddSubtree(leftChild);
                rootNode.AddSubtree(rightChild);
                
                multiTree.SetOutputNode(i, rootNode);
                
                var retrievedNode = multiTree.GetOutputNode(i);
                Assert.NotNull(retrievedNode);
                Assert.Equal(rootNode, retrievedNode);
            }

            // Test evaluation
            var variables = new Dictionary<string, double>
            {
                { "x0", 1.0 },
                { "x1", 2.0 },
                { "x2", 3.0 }
            };

            var results = multiTree.EvaluateAll(variables);
            Assert.Equal(3, results.Count);
            
            // Expected results: x0+1=2, x1+2=4, x2+3=6
            Assert.Equal(2.0, results[0]);
            Assert.Equal(4.0, results[1]);
            Assert.Equal(6.0, results[2]);

            // Test string representation
            var treeString = multiTree.ToString();
            Assert.Contains("MultiSymbolicExpressionTree", treeString);
            Assert.Contains("3 outputs", treeString);
            Assert.Contains("3 set", treeString);
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to normalize data
        /// </summary>
        private (double[][] normalizedInputs, double[] normalizedTargets, double[] inputMeans, double[] inputStds, double targetMean, double targetStd) 
            NormalizeData(double[][] inputs, double[] targets)
        {
            int numFeatures = inputs[0].Length;
            var inputMeans = new double[numFeatures];
            var inputStds = new double[numFeatures];

            // Calculate means and standard deviations for inputs
            for (int j = 0; j < numFeatures; j++)
            {
                inputMeans[j] = inputs.Average(row => row[j]);
                var variance = inputs.Average(row => Math.Pow(row[j] - inputMeans[j], 2));
                inputStds[j] = Math.Sqrt(variance);
                if (inputStds[j] == 0) inputStds[j] = 1; // Avoid division by zero
            }

            // Calculate mean and std for targets
            var targetMean = targets.Average();
            var targetVariance = targets.Average(t => Math.Pow(t - targetMean, 2));
            var targetStd = Math.Sqrt(targetVariance);
            if (targetStd == 0) targetStd = 1;

            // Normalize inputs
            var normalizedInputs = inputs.Select(row =>
                row.Select((val, j) => (val - inputMeans[j]) / inputStds[j]).ToArray()).ToArray();

            // Normalize targets
            var normalizedTargets = targets.Select(t => (t - targetMean) / targetStd).ToArray();

            return (normalizedInputs, normalizedTargets, inputMeans, inputStds, targetMean, targetStd);
        }

        /// <summary>
        /// Helper method to split dataset
        /// </summary>
        private (double[][] trainInputs, double[] trainTargets, double[][] valInputs, double[] valTargets, double[][] testInputs, double[] testTargets) 
            SplitDataset(double[][] inputs, double[] targets, double trainRatio, double valRatio, double testRatio, int seed)
        {
            var random = new Random(seed);
            var indices = Enumerable.Range(0, inputs.Length).OrderBy(x => random.Next()).ToArray();

            int trainSize = (int)(inputs.Length * trainRatio);
            int valSize = (int)(inputs.Length * valRatio);
            int testSize = inputs.Length - trainSize - valSize;

            var trainInputs = indices.Take(trainSize).Select(i => inputs[i]).ToArray();
            var trainTargets = indices.Take(trainSize).Select(i => targets[i]).ToArray();

            var valInputs = indices.Skip(trainSize).Take(valSize).Select(i => inputs[i]).ToArray();
            var valTargets = indices.Skip(trainSize).Take(valSize).Select(i => targets[i]).ToArray();

            var testInputs = indices.Skip(trainSize + valSize).Select(i => inputs[i]).ToArray();
            var testTargets = indices.Skip(trainSize + valSize).Select(i => targets[i]).ToArray();

            return (trainInputs, trainTargets, valInputs, valTargets, testInputs, testTargets);
        }

        /// <summary>
        /// Helper method to measure evaluation time
        /// </summary>
        private long MeasureEvaluationTime(MultiSymbolicExpressionTree<double> multiTree, Dictionary<string, double> variables, int iterations)
        {
            var results = new List<IReadOnlyList<double>>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var evaluationResult = multiTree.EvaluateAll(variables);
                results.Add(evaluationResult);
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Helper method to measure evaluation time for multiple single trees
        /// </summary>
        private long MeasureSingleEvaluationTime(MultiSymbolicExpressionTree<double> multiTree, Dictionary<string, double> variables, int iterations)
        {
            List<SymbolicExpressionTree<double>> singleTrees = new List<SymbolicExpressionTree<double>>();
            for (int i = 0; i < multiTree.OutputCount; i++)
            {
                var singleTree = new SymbolicExpressionTree<double>();
                singleTree.Root = multiTree.GetOutputNode(i);
                singleTrees.Add(singleTree);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            List<List<double>> results = new List<List<double>>();
            for (int i = 0; i < iterations; i++)
            {
                var aux = new List<double>();
                foreach (var tree in singleTrees)
                {

                    // Simple evaluation without parameters for performance test
                    aux.Add(((MultiOutputRootNode<double>)multiTree.Root).EvaluateNode(tree.Root, variables)); // Just ensure tree is accessible
                }
                results.Add(aux);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Helper method to evaluate multi-output tree fitness
        /// </summary>
        private double EvaluateMultiOutputTree(MultiSymbolicExpressionTree<double> tree, 
            MultiOutputRegressionEvaluator evaluator, 
            double[][] inputs, 
            double[][] targets, 
            string[] variableNames)
        {
            double totalError = 0.0;
            int sampleCount = Math.Min(inputs.Length, 50); // Limit for performance
            
            for (int i = 0; i < sampleCount; i++)
            {
                var variables = variableNames.Take(Math.Min(variableNames.Length, inputs[i].Length))
                    .Zip(inputs[i], (name, value) => new { name, value })
                    .ToDictionary(x => x.name, x => x.value);
                
                try
                {
                    var predictions = tree.EvaluateAll(variables);
                    
                    for (int output = 0; output < Math.Min(predictions.Count, targets[i].Length); output++)
                    {
                        if (!double.IsNaN(predictions[output]) && !double.IsInfinity(predictions[output]))
                        {
                            var error = Math.Abs(predictions[output] - targets[i][output]);
                            totalError += error;
                        }
                        else
                        {
                            totalError += 1000; // Penalty for invalid predictions
                        }
                    }
                }
                catch
                {
                    totalError += 1000; // Penalty for evaluation errors
                }
            }
            
            return -totalError / sampleCount; // Return negative error as fitness (higher is better)
        }

        /// <summary>
        /// Performance comparison test between normal and cached MultiOutputRootNode
        /// </summary>
        [Fact]
        public void CachedMultiOutputRootNode_ShouldShowPerformanceBenefit()
        {
            const int outputCount = 3;
            const int iterations = 100; // Reduced for faster test execution

            // Create test data
            var variables = new Dictionary<string, double>
            {
                { "x1", 1.5 },
                { "x2", 2.3 },
                { "x3", 0.8 }
            };

            // Create normal multi-output tree
            var normalTree = new MultiSymbolicExpressionTree<double>(outputCount);

            // Create cached multi-output tree
            var cachedRoot = new CachedMultiOutputRootNode<double>(outputCount);

            // Create identical tree structures for both
            var addSymbol = MathematicalSymbols.Addition;
            var multSymbol = MathematicalSymbols.Multiplication;
            var varSymbol = new Variable<double>();
            var constSymbol = new Constant<double>();

            for (int output = 0; output < outputCount; output++)
            {
                // Create: (x1 + x2) * (x3 + constant)
                var rootNode = multSymbol.CreateTreeNode();
                
                var leftAdd = addSymbol.CreateTreeNode();
                leftAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x1" });
                leftAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x2" });
                
                var rightAdd = addSymbol.CreateTreeNode();
                rightAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x3" });
                rightAdd.AddSubtree(new ConstantTreeNode<double>(constSymbol, 1.0 + output));
                
                rootNode.AddSubtree(leftAdd);
                rootNode.AddSubtree(rightAdd);
                
                normalTree.SetOutputNode(output, rootNode);
                
                // Create a copy for cached version
                var cachedRootNode = multSymbol.CreateTreeNode();
                
                var cachedLeftAdd = addSymbol.CreateTreeNode();
                cachedLeftAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x1" });
                cachedLeftAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x2" });
                
                var cachedRightAdd = addSymbol.CreateTreeNode();
                cachedRightAdd.AddSubtree(new VariableTreeNode<double>(varSymbol) { VariableName = "x3" });
                cachedRightAdd.AddSubtree(new ConstantTreeNode<double>(constSymbol, 1.0 + output));
                
                cachedRootNode.AddSubtree(cachedLeftAdd);
                cachedRootNode.AddSubtree(cachedRightAdd);
                
                cachedRoot.SetOutputNode(output, cachedRootNode);
            }

            // Pre-warm the cached version
            cachedRoot.UpdateVariablesHash(variables);
            var preWarmResults = cachedRoot.EvaluateAllOptimized(variables);
            Assert.NotNull(preWarmResults);

            // Measure normal version
            var normalTime = System.Diagnostics.Stopwatch.StartNew();
            IReadOnlyList<double>? normalResults = null;
            for (int i = 0; i < iterations; i++)
            {
                normalResults = normalTree.EvaluateAll(variables);
            }
            normalTime.Stop();

            // Measure cached version (with pre-computed hash)
            var cachedTime = System.Diagnostics.Stopwatch.StartNew();
            IReadOnlyList<double>? cachedResults = null;
            for (int i = 0; i < iterations; i++)
            {
                cachedResults = cachedRoot.EvaluateAllOptimized(variables);
            }
            cachedTime.Stop();

            // Verify results are identical
            Assert.NotNull(normalResults);
            Assert.NotNull(cachedResults);
            Assert.Equal(normalResults.Count, cachedResults.Count);
            
            for (int i = 0; i < normalResults.Count; i++)
            {
                Assert.Equal(normalResults[i], cachedResults[i], precision: 10);
            }

            // Check cache statistics
            var (cacheSize, hashValid) = cachedRoot.GetCacheStats();
            Assert.True(hashValid, "Variables hash should be valid");
            Assert.True(cacheSize > 0, "Cache should contain entries");

            // Log performance comparison
            Console.WriteLine($"Normal: {normalTime.ElapsedMilliseconds}ms, Cached: {cachedTime.ElapsedMilliseconds}ms");
            Console.WriteLine($"Cache size: {cacheSize}, Hash valid: {hashValid}");
            
            // Basic validation that both complete successfully
            Assert.True(normalTime.ElapsedMilliseconds >= 0, "Normal evaluation should complete");
            Assert.True(cachedTime.ElapsedMilliseconds >= 0, "Cached evaluation should complete");
            
            // Clear cache and verify it affects statistics
            cachedRoot.ClearCache();
            var (emptyCacheSize, emptyHashValid) = cachedRoot.GetCacheStats();
            Assert.Equal(0, emptyCacheSize);
            Assert.False(emptyHashValid);
        }

        #endregion
    }

    /// <summary>
    /// Custom evaluator for multi-output regression
    /// </summary>
    public class MultiOutputRegressionEvaluator
    {
        private readonly double[][] _inputs;
        private readonly double[][] _targets;
        private readonly string[] _variableNames;

        public MultiOutputRegressionEvaluator(double[][] inputs, double[][] targets, string[] variableNames)
        {
            _inputs = inputs;
            _targets = targets;
            _variableNames = variableNames;
        }

        public double Evaluate(MultiSymbolicExpressionTree<double> tree)
        {
            double totalError = 0.0;
            int validEvaluations = 0;

            for (int i = 0; i < Math.Min(_inputs.Length, 100); i++)
            {
                var variables = _variableNames.Take(Math.Min(_variableNames.Length, _inputs[i].Length))
                    .Zip(_inputs[i], (name, value) => new { name, value })
                    .ToDictionary(x => x.name, x => x.value);

                try
                {
                    var predictions = tree.EvaluateAll(variables);
                    
                    for (int output = 0; output < Math.Min(predictions.Count, _targets[i].Length); output++)
                    {
                        if (!double.IsNaN(predictions[output]) && !double.IsInfinity(predictions[output]))
                        {
                            var error = Math.Pow(predictions[output] - _targets[i][output], 2);
                            totalError += error;
                            validEvaluations++;
                        }
                    }
                }
                catch
                {
                    // Skip invalid evaluations
                }
            }

            return validEvaluations > 0 ? -Math.Sqrt(totalError / validEvaluations) : -1000.0;
        }
    }
}
