using System;
using System.Threading.Tasks;
using Xunit;
using GeneticProgramming.Algorithms;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Expressions.Symbols;

namespace GeneticProgramming.Standalone.Tests.Integration.EndToEnd
{
    /// <summary>
    /// Testes específicos para verificar a correção do problema de fitness vs accuracy
    /// </summary>
    public class FitnessAccuracyVerificationTests
    {
        /// <summary>
        /// Verifica se o fitness de treinamento e teste são consistentes em classificação multiclasse (dígitos 0-9)
        /// </summary>
        [Fact]
        public async Task DigitsMulticlass_TrainingAndTestFitness_ShouldBeConsistent()
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Starting digits multiclass classification consistency test...");
            
            // Arrange - Get handwritten digits dataset
            var (fullInputs, fullTargets, variableNames) = await DatasetManager.GetDigitsDatasetAsync();
            
            // Use subset for faster testing
            int subsetSize = Math.Min(300, fullInputs.Length);
            var inputs = new double[subsetSize][];
            var targets = new int[subsetSize];
            
            Array.Copy(fullInputs, 0, inputs, 0, subsetSize);
            Array.Copy(fullTargets, 0, targets, 0, subsetSize);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Using {subsetSize} samples for multiclass classification (digits 0-9)...");

            // Use original multiclass targets (digits 0-9)
            var multiclassTargets = targets; // Keep original digit labels
            var classCounts = new int[10];
            for (int i = 0; i < subsetSize; i++)
            {
                classCounts[targets[i]]++;
            }
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Class distribution:");
            for (int digit = 0; digit < 10; digit++)
            {
                Console.WriteLine($"  Digit {digit}: {classCounts[digit]} samples");
            }

            // Split into train/test (70/30)
            int trainSize = (int)(subsetSize * 0.7);
            var trainInputs = new double[trainSize][];
            var trainTargets = new int[trainSize];
            var testInputs = new double[subsetSize - trainSize][];
            var testTargets = new int[subsetSize - trainSize];
            
            Array.Copy(inputs, 0, trainInputs, 0, trainSize);
            Array.Copy(multiclassTargets, 0, trainTargets, 0, trainSize);
            Array.Copy(inputs, trainSize, testInputs, 0, subsetSize - trainSize);
            Array.Copy(multiclassTargets, trainSize, testTargets, 0, subsetSize - trainSize);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Data split: {trainSize} training, {subsetSize - trainSize} test samples");

            // Test both evaluator types
            TestEvaluatorConsistency("Standard Accuracy", 
                new ClassificationFitnessEvaluator(trainInputs, trainTargets, variableNames),
                new ClassificationFitnessEvaluator(testInputs, testTargets, variableNames),
                variableNames);
                
            TestEvaluatorConsistency("Improved Gradient", 
                new ImprovedClassificationFitnessEvaluator(trainInputs, trainTargets, variableNames, 0.001),
                new ImprovedClassificationFitnessEvaluator(testInputs, testTargets, variableNames, 0.001),
                variableNames);
                
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] Multiclass fitness consistency test completed in {duration.TotalSeconds:F2}s");
        }
        
        private void TestEvaluatorConsistency(string evaluatorName, IFitnessEvaluator trainEvaluator, IFitnessEvaluator testEvaluator, string[] variableNames)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Testing {evaluatorName} evaluator...");
            
            // Create grammar with basic operators
            var grammar = new SymbolicRegressionGrammar(variableNames, new[]
            {
                MathematicalSymbols.Addition,
                MathematicalSymbols.Subtraction,
                MathematicalSymbols.Multiplication,
                MathematicalSymbols.ProtectedDivision
            }, allowConstants: true);

            // Configure algorithm for faster testing
            var algorithm = new GeneticProgrammingAlgorithm
            {
                Grammar = grammar,
                TreeCreator = new GrowTreeCreator(),
                Crossover = new SubtreeCrossover(),
                Mutator = new SubtreeMutator(),
                Selector = new TournamentSelector(),
                Random = new MersenneTwister(42),
                PopulationSize = 30,        // Small population for faster testing
                MaxGenerations = 10,        // Few generations for faster testing
                FitnessEvaluator = trainEvaluator
            };

            double finalTrainingFitness = 0;
            int generationCount = 0;
            
            algorithm.GenerationCompleted += (s, e) =>
            {
                finalTrainingFitness = e.BestFitness;
                generationCount++;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generation {generationCount}: {evaluatorName} training fitness = {e.BestFitness:F3}");
            };

            // Run algorithm
            algorithm.Run();
            
            // Test on holdout data with same evaluator type
            if (algorithm.BestIndividual != null)
            {
                double testFitness = testEvaluator.Evaluate(algorithm.BestIndividual);
                
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {evaluatorName} Results:");
                Console.WriteLine($"  Final Training Fitness: {finalTrainingFitness:F3}");
                Console.WriteLine($"  Test Fitness: {testFitness:F3}");
                Console.WriteLine($"  Difference: {Math.Abs(finalTrainingFitness - testFitness):F3}");
                
                // For multiclass (10 classes), adjust expectations
                double maxReasonableDifference = 0.4; // Allow more overfitting for harder problem
                
                Assert.True(Math.Abs(finalTrainingFitness - testFitness) <= maxReasonableDifference,
                    $"{evaluatorName}: Training fitness ({finalTrainingFitness:F3}) and test fitness ({testFitness:F3}) are too different. Difference: {Math.Abs(finalTrainingFitness - testFitness):F3}");
                    
                Assert.True(testFitness > 0.15, // Should be better than random (1/10 = 0.1) for 10-class classification
                    $"{evaluatorName}: Test fitness should be better than random: {testFitness:F3}");
                    
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✅ {evaluatorName} evaluator passed multiclass consistency test");
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ❌ {evaluatorName} failed to produce a best individual");
                Assert.Fail($"{evaluatorName}: Algorithm failed to produce a best individual");
            }
        }
    }
}
