namespace GeneticProgramming.Problems.Samples
{
    /// <summary>
    /// Dataset for a trivial classification task: output is 1 when X >= 1 else 0.
    /// </summary>
    public static class SimpleClassificationProblem
    {
        public static readonly double[][] Inputs =
        {
            new[] {0.0},
            new[] {1.0},
            new[] {2.0}
        };

        public static readonly int[] Targets = { 0, 1, 1 };
        public static readonly string[] VariableNames = { "X" };
    }
}
