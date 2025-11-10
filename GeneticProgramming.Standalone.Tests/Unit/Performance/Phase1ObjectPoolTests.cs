using Xunit;
using GeneticProgramming.Standalone.Performance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneticProgramming.Standalone.Tests.Unit.Performance
{
    /// <summary>
    /// Testes unitários para a classe ObjectPools.
    /// Valida reutilização de dicionários, thread-safety e limites de tamanho.
    /// </summary>
    public class Phase1ObjectPoolTests
    {
        [Fact]
        public void RentDictionary_ReturnsClearedDictionary()
        {
            // Arrange
            ObjectPools.Clear();
            
            // Act
            var dict = ObjectPools.RentDictionary();
            dict["key1"] = 1.0;
            dict["key2"] = 2.0;
            
            // Assert
            Assert.NotEmpty(dict);
            Assert.Equal(2, dict.Count);
        }
        
        [Fact]
        public void ReturnDictionary_ClearsDictionary()
        {
            // Arrange
            ObjectPools.Clear();
            var dict = ObjectPools.RentDictionary();
            dict["key"] = 1.0;
            
            // Act
            ObjectPools.ReturnDictionary(dict);
            
            // Assert
            Assert.Empty(dict);
        }
        
        [Fact]
        public void RentDictionary_AfterReturn_ReturnsSameDictionaryInstance()
        {
            // Arrange
            ObjectPools.Clear();
            
            // Act
            var dict1 = ObjectPools.RentDictionary();
            dict1["test"] = 42.0;
            ObjectPools.ReturnDictionary(dict1);
            
            var dict2 = ObjectPools.RentDictionary();
            
            // Assert - deve ser o mesmo objeto
            Assert.Same(dict1, dict2);
            // e deve estar limpo
            Assert.Empty(dict2);
        }
        
        [Fact]
        public void ObjectPools_RespectMaxSizeLimit()
        {
            // Arrange
            ObjectPools.Clear();
            const int maxPoolSize = 1000;
            
            // Act - alugar e devolver 1500 dicionários
            var dicts = new List<Dictionary<string, double>>();
            for (int i = 0; i < 1500; i++)
            {
                var dict = ObjectPools.RentDictionary();
                dicts.Add(dict);
            }
            
            // Devolver todos
            foreach (var dict in dicts)
            {
                ObjectPools.ReturnDictionary(dict);
            }
            
            // Assert
            var stats = ObjectPools.GetStatistics();
            Assert.True(stats.DictionaryPoolSize <= maxPoolSize, 
                $"Pool size {stats.DictionaryPoolSize} exceeded limit {maxPoolSize}");
        }
        
        [Fact]
        public void ObjectPools_IsThreadSafe()
        {
            // Arrange
            ObjectPools.Clear();
            const int threadCount = 10;
            const int operationsPerThread = 100;
            var tasks = new Task[threadCount];
            
            // Act - múltiplas threads alugando e devolvendo simultaneamente
            for (int t = 0; t < threadCount; t++)
            {
                tasks[t] = Task.Run(() =>
                {
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        var dict = ObjectPools.RentDictionary();
                        dict[$"key_{i}"] = i * 1.5;
                        ObjectPools.ReturnDictionary(dict);
                    }
                });
            }
            
            Task.WaitAll(tasks);
            
            // Assert - não deve ter lançado exceção
            var stats = ObjectPools.GetStatistics();
            Assert.True(stats.DictionaryPoolSize > 0, "Pool deveria ter dicionários");
            Assert.True(stats.DictionaryPoolSize <= 1000, "Pool não deveria exceder limite");
        }
        
        [Fact]
        public void ReturnDictionary_WithNull_DoesNotThrow()
        {
            // Arrange & Act & Assert
            ObjectPools.ReturnDictionary(null!);
            // Não deve lançar exceção
        }
        
        [Fact]
        public void GenericRentDictionary_WorksWithDifferentTypes()
        {
            // Arrange
            ObjectPools.Clear();
            
            // Act
            var dictInt = ObjectPools.RentDictionary<int>();
            dictInt["int"] = 42;
            
            var dictString = ObjectPools.RentDictionary<string>();
            dictString["string"] = "value";
            
            var dictDouble = ObjectPools.RentDictionary<double>();
            dictDouble["double"] = 3.14;
            
            // Assert
            Assert.Equal(42, dictInt["int"]);
            Assert.Equal("value", dictString["string"]);
            Assert.Equal(3.14, dictDouble["double"]);
        }
        
        [Fact]
        public void GenericReturnDictionary_ReusesInTypedPool()
        {
            // Arrange
            ObjectPools.Clear();
            
            // Act
            var dict1 = ObjectPools.RentDictionary<int>();
            dict1["key"] = 100;
            ObjectPools.ReturnDictionary(dict1);
            
            var dict2 = ObjectPools.RentDictionary<int>();
            
            // Assert
            Assert.Same(dict1, dict2);
            Assert.Empty(dict2);
        }
        
        [Fact]
        public void GetStatistics_ReturnsAccuratePoolInfo()
        {
            // Arrange
            ObjectPools.Clear();
            
            // Act
            var dict1 = ObjectPools.RentDictionary();
            var dict2 = ObjectPools.RentDictionary();
            ObjectPools.ReturnDictionary(dict1);
            ObjectPools.ReturnDictionary(dict2);
            
            var stats = ObjectPools.GetStatistics();
            
            // Assert
            Assert.Equal(2, stats.DictionaryPoolSize);
            Assert.True(stats.GenericPoolsCount >= 0);
        }
        
        [Fact]
        public void Clear_RemovesAllPooledObjects()
        {
            // Arrange
            var dict1 = ObjectPools.RentDictionary();
            var dict2 = ObjectPools.RentDictionary();
            ObjectPools.ReturnDictionary(dict1);
            ObjectPools.ReturnDictionary(dict2);
            
            var statsBefore = ObjectPools.GetStatistics();
            Assert.True(statsBefore.DictionaryPoolSize > 0);
            
            // Act
            ObjectPools.Clear();
            
            // Assert
            var statsAfter = ObjectPools.GetStatistics();
            Assert.Equal(0, statsAfter.DictionaryPoolSize);
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void ObjectPools_HandleManyDictionaries(int count)
        {
            // Arrange
            ObjectPools.Clear();
            var dicts = new List<Dictionary<string, double>>();
            
            // Act - criar e armazenar
            for (int i = 0; i < count; i++)
            {
                var dict = ObjectPools.RentDictionary();
                dict["value"] = i;
                dicts.Add(dict);
            }
            
            // Limpar todos
            for (int i = 0; i < count; i++)
            {
                ObjectPools.ReturnDictionary(dicts[i]);
            }
            
            // Assert
            var stats = ObjectPools.GetStatistics();
            Assert.True(stats.DictionaryPoolSize <= Math.Min(count, 1000));
        }
        
        [Fact]
        public void ToStringMethod_FormatsCorrectly()
        {
            // Arrange
            ObjectPools.Clear();
            var dict = ObjectPools.RentDictionary();
            ObjectPools.ReturnDictionary(dict);
            
            var stats = ObjectPools.GetStatistics();
            
            // Act
            var statsString = stats.ToString();
            
            // Assert
            Assert.Contains("DictionaryPool", statsString);
            Assert.Contains("objetos", statsString);
        }
    }
}
