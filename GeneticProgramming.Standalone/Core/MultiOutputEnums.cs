namespace GeneticProgramming.Standalone.Core;

/// <summary>
/// Defines strategies for managing multiple outputs in symbolic expression trees.
/// </summary>
public enum MultiOutputStrategy
{
    /// <summary>
    /// Each output is independent with no sharing of subtrees.
    /// This strategy creates completely separate expression trees for each output.
    /// </summary>
    Independent,

    /// <summary>
    /// Outputs can share subtrees for improved efficiency and compactness.
    /// This is the default strategy as it allows for more efficient evaluation
    /// and can lead to more compact representations.
    /// </summary>
    Shared
}

/// <summary>
/// Defines how multi-output trees should be created during initialization.
/// </summary>
public enum TreeCreationMode
{
    /// <summary>
    /// Create completely random trees for each output.
    /// Each output gets an independently generated expression tree.
    /// </summary>
    Random,

    /// <summary>
    /// Create trees with a shared base and specialized outputs.
    /// A common subtree is created and then extended differently for each output.
    /// </summary>
    SharedBase,

    /// <summary>
    /// Create hierarchical outputs where some outputs depend on others.
    /// Later outputs can reference and build upon earlier outputs.
    /// </summary>
    Hierarchical
}
