namespace GeneticProgramming.Problems.Samples
{
    /// <summary>
    /// Provides a tiny dataset for linear regression: y = 2 * x.
    /// </summary>
    public static class SimpleRegressionProblem
    {
        public static readonly double[][] Inputs =
        {
            new[] {0.0},
            new[] {1.0},
            new[] {2.0},
            new[] {3.0}
        };

        public static readonly double[] Targets = { 0.0, 2.0, 4.0, 6.0 };
        public static readonly string[] VariableNames = { "X" };
    }
}
