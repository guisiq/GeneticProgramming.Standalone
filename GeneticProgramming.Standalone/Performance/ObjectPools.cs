using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GeneticProgramming.Standalone.Performance
{
    /// <summary>
    /// Pool de objetos reutilizáveis para reduzir alocações durante avaliação.
    /// Thread-safe para uso em contextos paralelos.
    /// </summary>
    public static class ObjectPools
    {
        /// <summary>
        /// Pool de dicionários para variáveis de avaliação.
        /// </summary>
        private static readonly ConcurrentBag<Dictionary<string, double>> _dictionaryPool = new();
        
        /// <summary>
        /// Aluga um dicionário do pool. Retorna um novo se o pool estiver vazio.
        /// </summary>
        /// <returns>Dicionário limpo e pronto para uso</returns>
        public static Dictionary<string, double> RentDictionary()
        {
            if (_dictionaryPool.TryTake(out var dictionary))
            {
                dictionary.Clear();
                return dictionary;
            }
            
            return new Dictionary<string, double>();
        }
        
        /// <summary>
        /// Devolve um dicionário ao pool para reutilização.
        /// </summary>
        /// <param name="dictionary">Dicionário a ser devolvido</param>
        public static void ReturnDictionary(Dictionary<string, double> dictionary)
        {
            if (dictionary == null) return;
            
            // Limpar o dicionário antes de devolver ao pool
            dictionary.Clear();
            
            // Limitar o tamanho do pool para evitar crescimento infinito
            if (_dictionaryPool.Count < 1000)
            {
                _dictionaryPool.Add(dictionary);
            }
        }
        
        /// <summary>
        /// Pool genérico para dicionários de qualquer tipo.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> _genericPools = new();
        
        /// <summary>
        /// Aluga um dicionário genérico do pool.
        /// </summary>
        public static Dictionary<string, T> RentDictionary<T>() where T : notnull
        {
            var poolKey = typeof(Dictionary<string, T>);
            var pool = (ConcurrentBag<Dictionary<string, T>>)_genericPools.GetOrAdd(
                poolKey, 
                _ => new ConcurrentBag<Dictionary<string, T>>()
            );
            
            if (pool.TryTake(out var dictionary))
            {
                dictionary.Clear();
                return dictionary;
            }
            
            return new Dictionary<string, T>();
        }
        
        /// <summary>
        /// Devolve um dicionário genérico ao pool.
        /// </summary>
        public static void ReturnDictionary<T>(Dictionary<string, T> dictionary) where T : notnull
        {
            if (dictionary == null) return;
            
            var poolKey = typeof(Dictionary<string, T>);
            var pool = (ConcurrentBag<Dictionary<string, T>>)_genericPools.GetOrAdd(
                poolKey, 
                _ => new ConcurrentBag<Dictionary<string, T>>()
            );
            
            dictionary.Clear();
            
            if (pool.Count < 1000)
            {
                pool.Add(dictionary);
            }
        }
        
        /// <summary>
        /// Aluga um dicionário genérico com chave e valor customizados.
        /// </summary>
        public static Dictionary<TKey, TValue> RentDictionary<TKey, TValue>() where TKey : notnull
        {
            var poolKey = typeof(Dictionary<TKey, TValue>);
            var pool = (ConcurrentBag<Dictionary<TKey, TValue>>)_genericPools.GetOrAdd(
                poolKey, 
                _ => new ConcurrentBag<Dictionary<TKey, TValue>>()
            );
            
            if (pool.TryTake(out var dictionary))
            {
                dictionary.Clear();
                return dictionary;
            }
            
            return new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Devolve um dicionário genérico com chave e valor customizados.
        /// </summary>
        public static void ReturnDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            if (dictionary == null) return;
            
            var poolKey = typeof(Dictionary<TKey, TValue>);
            var pool = (ConcurrentBag<Dictionary<TKey, TValue>>)_genericPools.GetOrAdd(
                poolKey, 
                _ => new ConcurrentBag<Dictionary<TKey, TValue>>()
            );
            
            dictionary.Clear();
            
            if (pool.Count < 1000)
            {
                pool.Add(dictionary);
            }
        }
        
        /// <summary>
        /// Obtém estatísticas do pool para monitoramento.
        /// </summary>
        public static PoolStatistics GetStatistics()
        {
            return new PoolStatistics
            {
                DictionaryPoolSize = _dictionaryPool.Count,
                GenericPoolsCount = _genericPools.Count
            };
        }
        
        /// <summary>
        /// Limpa todos os pools (útil para testes).
        /// </summary>
        public static void Clear()
        {
            _dictionaryPool.Clear();
            _genericPools.Clear();
        }
    }
    
    /// <summary>
    /// Estatísticas dos pools de objetos.
    /// </summary>
    public class PoolStatistics
    {
        public int DictionaryPoolSize { get; set; }
        public int GenericPoolsCount { get; set; }
        
        public override string ToString()
        {
            return $"DictionaryPool: {DictionaryPoolSize} objetos, GenericPools: {GenericPoolsCount} tipos";
        }
    }
}
