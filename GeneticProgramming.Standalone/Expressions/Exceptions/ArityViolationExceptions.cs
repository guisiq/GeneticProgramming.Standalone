using System;

namespace GeneticProgramming.Expressions.Exceptions
{
    /// <summary>
    /// Base exception for arity constraint violations
    /// </summary>
    public abstract class ArityViolationException : InvalidOperationException
    {
        public string SymbolName { get; }
        public int CurrentCount { get; }
        public int MinimumArity { get; }
        public int MaximumArity { get; }

        protected ArityViolationException(string message, string symbolName, int currentCount, int minArity, int maxArity)
            : base(message)
        {
            SymbolName = symbolName;
            CurrentCount = currentCount;
            MinimumArity = minArity;
            MaximumArity = maxArity;
        }
    }

    /// <summary>
    /// Exception thrown when trying to add more subtrees than the maximum arity allows
    /// </summary>
    public class MaximumArityExceededException : ArityViolationException
    {
        public int AttemptedCount { get; }

        public MaximumArityExceededException(string symbolName, int currentCount, int attemptedCount, int maxArity)
            : base($"Cannot add more subtrees to '{symbolName}'. Maximum arity is {maxArity}, but would have {attemptedCount} subtrees.",
                   symbolName, currentCount, 0, maxArity)
        {
            AttemptedCount = attemptedCount;
        }
    }

    /// <summary>
    /// Exception thrown when trying to remove subtrees below the minimum arity
    /// </summary>
    public class MinimumArityViolatedException : ArityViolationException
    {
        public int AttemptedCount { get; }

        public MinimumArityViolatedException(string symbolName, int currentCount, int attemptedCount, int minArity)
            : base($"Cannot remove subtrees from '{symbolName}'. Minimum arity is {minArity}, but would have {attemptedCount} subtrees.",
                   symbolName, currentCount, minArity, 0)
        {
            AttemptedCount = attemptedCount;
        }
    }

    /// <summary>
    /// Exception thrown when the current subtree count is invalid for the symbol
    /// </summary>
    public class InvalidArityException : ArityViolationException
    {
        public InvalidArityException(string symbolName, int currentCount, int minArity, int maxArity)
            : base($"Symbol '{symbolName}' has invalid arity. Current: {currentCount}, Expected: {minArity}-{maxArity}.",
                   symbolName, currentCount, minArity, maxArity)
        {
        }
    }
}
