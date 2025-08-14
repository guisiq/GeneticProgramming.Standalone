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

        // Check if variables actually changed
        if (_variablesHashValid &&
            _lastVariables.Count == variables.Count &&
            _lastVariables.All(kvp => variables.TryGetValue(kvp.Key, out var value) && EqualityComparer<T>.Default.Equals(kvp.Value, value)))
        {
            return; // No change, keep existing hash
        }

        // Update cached variables and compute new hash
        _lastVariables.Clear();
        var hashCode = new HashCode();

        // Sort keys for consistent hashing
        foreach (var kvp in variables.OrderBy(x => x.Key))
        {
            _lastVariables[kvp.Key] = kvp.Value;
            hashCode.Add(kvp.Key);
            hashCode.Add(kvp.Value);
        }

        _variablesHash = hashCode.ToHashCode();
        _variablesHashValid = true;
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
