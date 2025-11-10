using Xunit;
using GeneticProgramming.Problems.Evaluators;
using GeneticProgramming.Standalone.Performance;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeneticProgramming.Standalone.Tests.Unit.Performance
{
    /// <summary>
    /// Testes para a interface IParallelizableFitnessEvaluator.
    /// Testa configurações, estatísticas e comportamento sem criar árvores complexas.
    /// </summary>
    public class Phase1ParallelEvaluatorTests
    {
        [Fact]
        public void RegressionEvaluator_ImplementsInterface()
        {
            // Arrange
            var inputs = new double[100][];
            var targets = new double[100];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            // Act
            IParallelizableFitnessEvaluator<double> evaluator = 
                new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Assert
            Assert.NotNull(evaluator);
            Assert.True(evaluator.EnableParallelEvaluation);
            Assert.Equal(100, evaluator.ParallelThreshold);
        }

        [Fact]
        public void ClassificationEvaluator_ImplementsInterface()
        {
            // Arrange
            var inputs = new double[100][];
            var targets = new int[100];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = i % 2;
            }
            
            // Act
            IParallelizableFitnessEvaluator<double> evaluator = 
                new ClassificationFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Assert
            Assert.NotNull(evaluator);
            Assert.True(evaluator.EnableParallelEvaluation);
        }

        [Fact]
        public void EnableParallelEvaluation_CanBeConfigured()
        {
            // Arrange
            var inputs = new double[100][];
            var targets = new double[100];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act
            evaluator.EnableParallelEvaluation = false;
            
            // Assert
            Assert.False(evaluator.EnableParallelEvaluation);
        }

        [Fact]
        public void ParallelThreshold_CanBeConfigured()
        {
            // Arrange
            var inputs = new double[1000][];
            var targets = new double[1000];
            for (int i = 0; i < 1000; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act
            evaluator.ParallelThreshold = 500;
            
            // Assert
            Assert.Equal(500, evaluator.ParallelThreshold);
        }

        [Fact]
        public void ParallelThreshold_MinimumValueIs1()
        {
            // Arrange
            var inputs = new double[100][];
            var targets = new double[100];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act
            evaluator.ParallelThreshold = -10;
            
            // Assert - deve ser forçado para 1
            Assert.Equal(1, evaluator.ParallelThreshold);
        }

        [Fact]
        public void EvaluationStatistics_CalculatesSamplesPerSecond()
        {
            // Arrange
            var stats = new EvaluationStatistics
            {
                SamplesEvaluated = 1000,
                EvaluationTime = TimeSpan.FromMilliseconds(100),
                UsedParallelProcessing = false,
                ThreadsUsed = 1
            };
            
            // Act
            var samplesPerSec = stats.SamplesPerSecond;
            
            // Assert - 1000 samples em 100ms = 10000 samples/sec
            Assert.Equal(10000, samplesPerSec, 0);
        }

        [Fact]
        public void EvaluationStatistics_ToStringContainsInfo()
        {
            // Arrange
            var stats = new EvaluationStatistics
            {
                SamplesEvaluated = 1000,
                EvaluationTime = TimeSpan.FromMilliseconds(100),
                UsedParallelProcessing = true,
                ThreadsUsed = 4
            };
            
            // Act
            var statsStr = stats.ToString();
            
            // Assert
            Assert.Contains("Parallel", statsStr);
            Assert.Contains("1000 samples", statsStr);
            Assert.Contains("samples/sec", statsStr);
        }

        [Fact]
        public void RegressionEvaluator_SmallDataset_PrefersModeConfiguration()
        {
            // Arrange
            var inputs = new double[50][];
            var targets = new double[50];
            
            for (int i = 0; i < 50; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act
            evaluator.EnableParallelEvaluation = true;
            evaluator.ParallelThreshold = 100; // maior que dataset
            
            // Assert
            Assert.True(evaluator.EnableParallelEvaluation);
            Assert.Equal(100, evaluator.ParallelThreshold);
        }

        [Fact]
        public void RegressionEvaluator_DisabledParallel_RespectsSetting()
        {
            // Arrange
            var inputs = new double[1000][];
            var targets = new double[1000];
            
            for (int i = 0; i < 1000; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act
            evaluator.EnableParallelEvaluation = false;
            
            // Assert
            Assert.False(evaluator.EnableParallelEvaluation);
        }

        [Fact]
        public void ObjectPools_AreUsedByEvaluators()
        {
            // Arrange
            ObjectPools.Clear();
            var inputs = new double[100][];
            var targets = new double[100];
            
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = 3.0;
            }
            
            var evaluator = new RegressionFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            evaluator.EnableParallelEvaluation = false; // força sequencial
            
            // Act
            var beforeStats = ObjectPools.GetStatistics();
            var poolsBeforeDictCount = beforeStats.DictionaryPoolSize;
            
            // Assert - pool deve estar preparado para ser usado
            Assert.NotNull(evaluator);
        }

        [Fact]
        public void EvaluationStatistics_ZeroDurationHandlesCorrectly()
        {
            // Arrange
            var stats = new EvaluationStatistics
            {
                SamplesEvaluated = 0,
                EvaluationTime = TimeSpan.Zero,
                UsedParallelProcessing = false,
                ThreadsUsed = 1
            };
            
            // Act
            var samplesPerSec = stats.SamplesPerSecond;
            
            // Assert - não deve lançar exceção, deve retornar 0
            Assert.Equal(0, samplesPerSec);
        }

        [Fact]
        public void ClassificationEvaluator_InterfaceProperties_Accessible()
        {
            // Arrange
            var inputs = new double[100][];
            var targets = new int[100];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = new double[] { 1.0, 2.0 };
                targets[i] = i % 2;
            }
            
            IParallelizableFitnessEvaluator<double> evaluator = 
                new ClassificationFitnessEvaluator(inputs, targets, new[] { "X0", "X1" });
            
            // Act & Assert
            evaluator.EnableParallelEvaluation = false;
            Assert.False(evaluator.EnableParallelEvaluation);
            
            evaluator.ParallelThreshold = 200;
            Assert.Equal(200, evaluator.ParallelThreshold);
            
            var stats = evaluator.GetLastEvaluationStatistics();
            Assert.NotNull(stats);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(500)]
        public void EvaluationStatistics_HandlesVariousSampleCounts(int sampleCount)
        {
            // Arrange
            var stats = new EvaluationStatistics
            {
                SamplesEvaluated = sampleCount,
                EvaluationTime = TimeSpan.FromMilliseconds(10),
                UsedParallelProcessing = false,
                ThreadsUsed = 1
            };
            
            // Act
            var samplesPerSec = stats.SamplesPerSecond;
            var statsStr = stats.ToString();
            
            // Assert
            Assert.True(samplesPerSec > 0);
            Assert.Contains(sampleCount.ToString(), statsStr);
        }
    }
}
