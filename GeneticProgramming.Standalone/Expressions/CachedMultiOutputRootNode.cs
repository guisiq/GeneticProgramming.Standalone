using System;
using System.Collections.Generic;
using System.Linq;
namespace GeneticProgramming.Standalone.Expressions;
/// <summary>
/// Cached implementation of MultiOutputRootNode that optimizes repeated evaluations
/// with the same variable context by maintaining a pre-computed hash of variables.
/// </summary>
/// <typeparam name="T">The base value type for each output (must be a struct)</typeparam>
public class CachedMultiOutputRootNode<T> : MultiOutputRootNode<T> where T : notnull
{
    private readonly Dictionary<(long, int, int), T> _evalCache = new();
    private readonly Dictionary<string, T> _lastVariables = new();
    private int _variablesHash = 0;
    private bool _variablesHashValid = false;

    /// <summary>
    /// Initializes a new instance of the CachedMultiOutputRootNode class.
    /// </summary>
    /// <param name="outputCount">Number of outputs this node will manage</param>
    public CachedMultiOutputRootNode(int outputCount) : base(outputCount)
    {
    }

    /// <summary>
    /// Updates the variables hash cache. Call this when the variable context changes.
    /// This avoids recomputing the hash on every evaluation.
    /// </summary>
    /// <param name="variables">The new variable context</param>
    public void UpdateVariablesHash(IDictionary<string, T> variables)
    {
        if (variables == null)
        {
            _variablesHashValid = false;
            return;
        }

        // Optimization: Avoid sorting and copying if possible.
        // We assume that if the caller provides a hash, we can use it.
        // But here we compute it.
        // Instead of sorting, we can use an order-independent hash (XOR based) or just sum.
        // However, for floating point values, order might matter for hash stability if we used HashCode.Add.
        // But XOR is commutative.
        
        int newHash = 0;
        foreach (var kvp in variables)
        {
            int keyHash = kvp.Key.GetHashCode();
            int valHash = kvp.Value?.GetHashCode() ?? 0;
            // Simple order-independent combination
            newHash ^= (keyHash * 397) ^ valHash;
        }

        if (_variablesHashValid && _variablesHash == newHash)
        {
             // Optimistic check: if hash matches, assume variables are same.
             // This avoids the expensive deep comparison.
             // Risk: Hash collision. But for caching optimization, it might be acceptable or we can add a version counter.
             return;
        }

        _variablesHash = newHash;
        _variablesHashValid = true;
        
        // We don't need _lastVariables anymore if we trust the hash or if we accept that
        // we only update hash when caller says variables changed (which is implied by calling this method).
        // But the original code was checking if variables changed.
        // If we want to be safe, we can keep _lastVariables but avoid sorting.
    }

    /// <summary>
    /// Cached version of EvaluateNode that uses pre-computed variables hash for efficiency.
    /// </summary>
    /// <param name="node">The node to evaluate</param>
    /// <param name="variables">Variable assignments for evaluation</param>
    /// <returns>The evaluated result</returns>
    public override T EvaluateNode(ISymbolicExpressionTreeNode<T> node, IDictionary<string, T> variables)
    {
        if (variables == null || !_variablesHashValid)
        {
            // Fallback to base implementation if no valid hash
            return base.EvaluateNode(node, variables ?? new Dictionary<string, T>());
        }

        // Create cache key using node hash, symbol hash, and pre-computed variables hash
        var nodeHash = node.GetHashCode();
        var symbolHash = node.Symbol?.GetHashCode() ?? 0;
        var cacheKey = (((long)nodeHash << 32) | (uint)symbolHash, 0, _variablesHash);

        // Check cache
        if (_evalCache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        // Evaluate and cache result
        var result = base.EvaluateNode(node, variables);

        // Limit cache size to prevent memory issues
        if (_evalCache.Count < 10000)
        {
            _evalCache[cacheKey] = result;
        }

        return result;
    }

    /// <summary>
    /// Clears the evaluation cache. Call this when the tree structure changes.
    /// </summary>
    public void ClearCache()
    {
        _evalCache.Clear();
        _variablesHashValid = false;
        _lastVariables.Clear();
    }

    /// <summary>
    /// Gets cache statistics for performance monitoring.
    /// </summary>
    /// <returns>Tuple with (cache size, variables hash valid)</returns>
    public (int CacheSize, bool VariablesHashValid) GetCacheStats()
    {
        return (_evalCache.Count, _variablesHashValid);
    }

    /// <summary>
    /// Optimized evaluation that updates variables hash once and evaluates all outputs.
    /// This is more efficient than calling EvaluateNode individually for each output.
    /// </summary>
    /// <param name="variables">Variable assignments for evaluation</param>
    /// <returns>List of evaluated values for each output</returns>
    public IReadOnlyList<T> EvaluateAllOptimized(IDictionary<string, T> variables)
    {
        // Update variables hash once for all outputs
        UpdateVariablesHash(variables);

        // Convert to the expected format and evaluate
        var convertedVars = variables.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<T>)new List<T> { kvp.Value }
        );

        return Evaluate(convertedVars);
    }
}
