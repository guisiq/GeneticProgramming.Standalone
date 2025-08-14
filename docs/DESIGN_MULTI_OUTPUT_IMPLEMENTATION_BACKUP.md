# Design e Arquitetura - Etapa 2: Suporte a Múltiplas Saídas Homogêneas

## 📋 Visão Geral

Este documento apresenta o design completo para implementar suporte a múltiplas saídas homogêneas no sistema de Programação Genética. A implementação permite que uma única árvore de expressão simbólica produza múltiplas saídas do mesmo tipo, funcionando como uma função vetorial.

## 🎯 Conceito Central

Uma `MultiSymbolicExpressionTree<T>` representa uma função vetorial:

```
f: ℝⁿ → ℝᵏ
f(x₁, x₂, ..., xₙ) = [y₁, y₂, ..., yₖ]

Onde:
- n = número de variáveis de entrada
- k = número de saídas (OutputCount)
- T = tipo das saídas (homogêneo)
```

## 🏗️ Arquitetura do Sistema

### Diagrama ASCII da Estrutura

```
MultiSymbolicExpressionTree<T>
│
├── Root: MultiOutputRootNode<T>
│   │
│   ├── Subtree[0] → Output₁: ISymbolicExpressionTreeNode<T>
│   │   │
│   │   ├── Add(x, y)
│   │   │   ├── Variable(x)
│   │   │   └── Variable(y)
│   │   
│   ├── Subtree[1] → Output₂: ISymbolicExpressionTreeNode<T>
│   │   │
│   │   ├── Multiply
│   │   │   ├── [COMPARTILHADO] Add(x, y) ←─── MESMO NÓ!
│   │   │   └── Constant(2.0)
│   │
│   └── Subtree[2] → Output₃: ISymbolicExpressionTreeNode<T>
│       │
│       ├── Subtract
│       │   ├── [COMPARTILHADO] Add(x, y) ←─── MESMO NÓ!
│       │   └── Constant(1.0)

Resultado: f(x,y) = [x+y, 2*(x+y), (x+y)-1]
```

### Hierarquia de Classes

```
ISymbolicExpressionTree<T>
    ↑
ISymbolicExpressionTree<IReadOnlyList<T>>
    ↑
IMultiSymbolicExpressionTree<T>
    ↑
MultiSymbolicExpressionTree<T>

ISymbolicExpressionTreeNode<T>
    ↑
ISymbolicExpressionTreeNode<IReadOnlyList<T>>
    ↑
IMultiOutputNode<T>
    ↑
MultiOutputRootNode<T>
```

## 📚 Interfaces Principais

### 1. IMultiSymbolicExpressionTree<T>

```csharp
/// <summary>
/// Interface para árvores de expressão simbólica com múltiplas saídas homogêneas
/// </summary>
public interface IMultiSymbolicExpressionTree<T> : ISymbolicExpressionTree<IReadOnlyList<T>> 
    where T : struct
{
    /// <summary>
    /// Número de saídas que esta árvore produz
    /// </summary>
    int OutputCount { get; }
    
    /// <summary>
    /// Acesso direto aos nós que representam cada saída
    /// </summary>
    ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex);
    
    /// <summary>
    /// Define o nó que representa uma saída específica
    /// </summary>
    void SetOutputNode(int outputIndex, ISymbolicExpressionTreeNode<T> outputNode);
    
    /// <summary>
    /// Avalia todas as saídas da árvore em uma única passada
    /// </summary>
    IReadOnlyList<T> EvaluateAll(IDictionary<string, T> variables);
    
    /// <summary>
    /// Identifica nós compartilhados entre múltiplas saídas
    /// </summary>
    IReadOnlyList<ISymbolicExpressionTreeNode<T>> GetSharedNodes();
}
```

### 2. IMultiOutputNode<T>

```csharp
/// <summary>
/// Interface para nós raiz que agregam múltiplas saídas
/// </summary>
public interface IMultiOutputNode<T> : ISymbolicExpressionTreeNode<IReadOnlyList<T>> 
    where T : struct
{
    /// <summary>
    /// Número de saídas gerenciadas por este nó
    /// </summary>
    int OutputCount { get; }
    
    /// <summary>
    /// Estratégia de como as saídas são organizadas
    /// </summary>
    MultiOutputStrategy Strategy { get; set; }
}
```

## 🔧 Implementações Core

### 1. MultiSymbolicExpressionTree<T>

```csharp
/// <summary>
/// Implementação de árvore com múltiplas saídas homogêneas
/// Uma única árvore que funciona como função vetorial f: Rⁿ → Rᵏ
/// </summary>
[Item("MultiSymbolicExpressionTree", "Multi-output symbolic expression tree")]
public class MultiSymbolicExpressionTree<T> : SymbolicExpressionTree<IReadOnlyList<T>>, 
                                              IMultiSymbolicExpressionTree<T> 
    where T : struct
{
    private readonly int _outputCount;
    private readonly MultiOutputRootNode<T> _multiRoot;
    
    #region Properties
    
    public int OutputCount => _outputCount;
    
    /// <summary>
    /// Override Root para garantir tipo correto
    /// </summary>
    public new MultiOutputRootNode<T> Root 
    { 
        get => _multiRoot; 
        set => throw new InvalidOperationException("Cannot change root of multi-output tree"); 
    }
    
    #endregion
    
    #region Constructors
    
    public MultiSymbolicExpressionTree(int outputCount) : base()
    {
        if (outputCount <= 0)
            throw new ArgumentException("Output count must be positive", nameof(outputCount));
            
        _outputCount = outputCount;
        _multiRoot = new MultiOutputRootNode<T>(outputCount);
        base.Root = _multiRoot; // Chama o setter da base
    }
    
    protected MultiSymbolicExpressionTree(MultiSymbolicExpressionTree<T> original, Cloner cloner) 
        : base(original, cloner)
    {
        _outputCount = original._outputCount;
        _multiRoot = (MultiOutputRootNode<T>)base.Root;
    }
    
    #endregion
    
    #region Multi-Output Operations
    
    public ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex)
    {
        ValidateOutputIndex(outputIndex);
        return _multiRoot.GetOutputNode(outputIndex);
    }
    
    public void SetOutputNode(int outputIndex, ISymbolicExpressionTreeNode<T> outputNode)
    {
        ValidateOutputIndex(outputIndex);
        if (outputNode == null)
            throw new ArgumentNullException(nameof(outputNode));
            
        _multiRoot.SetOutputNode(outputIndex, outputNode);
    }
    
    public IReadOnlyList<T> EvaluateAll(IDictionary<string, T> variables)
    {
        var interpreter = new MultiOutputExpressionInterpreter();
        return interpreter.Evaluate(this, variables);
    }
    
    public IReadOnlyList<ISymbolicExpressionTreeNode<T>> GetSharedNodes()
    {
        var nodeReferences = new Dictionary<ISymbolicExpressionTreeNode<T>, int>();
        var sharedNodes = new List<ISymbolicExpressionTreeNode<T>>();
        
        // Conta referências a cada nó
        for (int i = 0; i < OutputCount; i++)
        {
            var outputNode = GetOutputNode(i);
            if (outputNode != null)
            {
                foreach (var node in outputNode.IterateNodesPostfix().Cast<ISymbolicExpressionTreeNode<T>>())
                {
                    nodeReferences[node] = nodeReferences.GetValueOrDefault(node, 0) + 1;
                }
            }
        }
        
        // Identifica nós com múltiplas referências
        foreach (var kvp in nodeReferences)
        {
            if (kvp.Value > 1)
            {
                sharedNodes.Add(kvp.Key);
            }
        }
        
        return sharedNodes.AsReadOnly();
    }
    
    #endregion
    
    #region Validation
    
    private void ValidateOutputIndex(int outputIndex)
    {
        if (outputIndex < 0 || outputIndex >= OutputCount)
            throw new ArgumentOutOfRangeException(nameof(outputIndex), 
                $"Output index must be between 0 and {OutputCount - 1}");
    }
    
    #endregion
    
    #region Cloning
    
    protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
    {
        return new MultiSymbolicExpressionTree<T>(this, cloner);
    }
    
    #endregion
    
    #region Visualization
    
    public override string ToMathString()
    {
        var outputs = new List<string>();
        for (int i = 0; i < OutputCount; i++)
        {
            var outputNode = GetOutputNode(i);
            var outputStr = outputNode?.ToString() ?? "null";
            outputs.Add($"y{i + 1} = {outputStr}");
        }
        
        return $"f(x) = [{string.Join(", ", outputs)}]";
    }
    
    #endregion
}
```

### 2. MultiOutputRootNode<T>

```csharp
/// <summary>
/// Nó raiz especializado que gerencia múltiplas saídas
/// Cada subárvore representa uma saída da função vetorial
/// </summary>
public class MultiOutputRootNode<T> : SymbolicExpressionTreeNode<IReadOnlyList<T>>, 
                                      IMultiOutputNode<T> 
    where T : struct
{
    private readonly int _outputCount;
    private readonly ISymbolicExpressionTreeNode<T>[] _outputNodes;
    
    #region Properties
    
    public int OutputCount => _outputCount;
    
    public MultiOutputStrategy Strategy { get; set; } = MultiOutputStrategy.Independent;
    
    #endregion
    
    #region Constructors
    
    public MultiOutputRootNode(int outputCount) 
        : base(new MultiOutputRootSymbol<T>(outputCount))
    {
        if (outputCount <= 0)
            throw new ArgumentException("Output count must be positive", nameof(outputCount));
            
        _outputCount = outputCount;
        _outputNodes = new ISymbolicExpressionTreeNode<T>[outputCount];
    }
    
    protected MultiOutputRootNode(MultiOutputRootNode<T> original, Cloner cloner) 
        : base(original, cloner)
    {
        _outputCount = original._outputCount;
        _outputNodes = new ISymbolicExpressionTreeNode<T>[_outputCount];
        
        // Clone dos nós de saída
        for (int i = 0; i < _outputCount; i++)
        {
            if (original._outputNodes[i] != null)
            {
                _outputNodes[i] = (ISymbolicExpressionTreeNode<T>)cloner.Clone(original._outputNodes[i]);
            }
        }
        
        Strategy = original.Strategy;
    }
    
    #endregion
    
    #region Output Management
    
    public ISymbolicExpressionTreeNode<T> GetOutputNode(int outputIndex)
    {
        ValidateOutputIndex(outputIndex);
        return _outputNodes[outputIndex];
    }
    
    public void SetOutputNode(int outputIndex, ISymbolicExpressionTreeNode<T> outputNode)
    {
        ValidateOutputIndex(outputIndex);
        
        // Remove nó anterior se existir
        if (_outputNodes[outputIndex] != null)
        {
            var oldNode = _outputNodes[outputIndex];
            var index = IndexOfSubtree(oldNode as ISymbolicExpressionTreeNode);
            if (index >= 0)
            {
                RemoveSubtree(index);
            }
        }
        
        // Define novo nó
        _outputNodes[outputIndex] = outputNode;
        
        if (outputNode != null)
        {
            AddSubtree(outputNode as ISymbolicExpressionTreeNode);
        }
    }
    
    #endregion
    
    #region Validation
    
    private void ValidateOutputIndex(int outputIndex)
    {
        if (outputIndex < 0 || outputIndex >= OutputCount)
            throw new ArgumentOutOfRangeException(nameof(outputIndex));
    }
    
    #endregion
    
    #region Cloning
    
    protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
    {
        return new MultiOutputRootNode<T>(this, cloner);
    }
    
    #endregion
}
```

## 🔄 Operadores Especializados

### 1. MultiOutputTreeCreator<T>

```csharp
/// <summary>
/// Criador de árvores multi-output
/// Gera árvores onde cada saída é uma expressão independente ou compartilhada
/// </summary>
public class MultiOutputTreeCreator<T> : SymbolicExpressionTreeOperator<IReadOnlyList<T>>, 
                                         ISymbolicExpressionTreeCreator<IReadOnlyList<T>> 
    where T : struct
{
    #region Parameters
    
    private int _outputCount = 2;
    private int _maxTreeLength = 25;
    private int _maxTreeDepth = 10;
    private double _sharingProbability = 0.3;
    
    public int OutputCount
    {
        get => _outputCount;
        set
        {
            if (_outputCount != value)
            {
                _outputCount = Math.Max(1, value);
                OnPropertyChanged(nameof(OutputCount));
            }
        }
    }
    
    public int MaxTreeLength
    {
        get => _maxTreeLength;
        set
        {
            if (_maxTreeLength != value)
            {
                _maxTreeLength = Math.Max(1, value);
                OnPropertyChanged(nameof(MaxTreeLength));
            }
        }
    }
    
    public int MaxTreeDepth
    {
        get => _maxTreeDepth;
        set
        {
            if (_maxTreeDepth != value)
            {
                _maxTreeDepth = Math.Max(1, value);
                OnPropertyChanged(nameof(MaxTreeDepth));
            }
        }
    }
    
    /// <summary>
    /// Probabilidade de compartilhar subexpressões entre saídas
    /// </summary>
    public double SharingProbability
    {
        get => _sharingProbability;
        set
        {
            if (_sharingProbability != value)
            {
                _sharingProbability = Math.Max(0.0, Math.Min(1.0, value));
                OnPropertyChanged(nameof(SharingProbability));
            }
        }
    }
    
    #endregion
    
    #region Constructors
    
    public MultiOutputTreeCreator() : base() { }
    
    protected MultiOutputTreeCreator(MultiOutputTreeCreator<T> original, Cloner cloner) 
        : base(original, cloner)
    {
        _outputCount = original._outputCount;
        _maxTreeLength = original._maxTreeLength;
        _maxTreeDepth = original._maxTreeDepth;
        _sharingProbability = original._sharingProbability;
    }
    
    #endregion
    
    #region Tree Creation
    
    public ISymbolicExpressionTree<IReadOnlyList<T>> CreateTree(IRandom random, 
        ISymbolicExpressionTreeGrammar<IReadOnlyList<T>> grammar, 
        int maxTreeLength, int maxTreeDepth)
    {
        if (SymbolicExpressionTreeGrammar == null)
            throw new InvalidOperationException("Grammar must be set");
            
        var multiTree = new MultiSymbolicExpressionTree<T>(OutputCount);
        var singleGrammar = ExtractSingleOutputGrammar(grammar);
        var singleCreator = new GrowTreeCreator<T> 
        { 
            SymbolicExpressionTreeGrammar = singleGrammar 
        };
        
        // Cria árvores para cada saída
        var createdTrees = new List<ISymbolicExpressionTree<T>>();
        for (int i = 0; i < OutputCount; i++)
        {
            var tree = singleCreator.CreateTree(random, singleGrammar, maxTreeLength, maxTreeDepth);
            createdTrees.Add(tree);
        }
        
        // Aplica estratégia de compartilhamento se configurada
        if (SharingProbability > 0)
        {
            ApplySharing(random, createdTrees);
        }
        
        // Define as saídas na árvore multi-output
        for (int i = 0; i < OutputCount; i++)
        {
            multiTree.SetOutputNode(i, createdTrees[i].Root);
        }
        
        return multiTree;
    }
    
    private void ApplySharing(IRandom random, List<ISymbolicExpressionTree<T>> trees)
    {
        // Estratégia simples: tenta compartilhar subárvores entre pares de árvores
        for (int i = 0; i < trees.Count - 1; i++)
        {
            if (random.NextDouble() < SharingProbability)
            {
                TryShareSubtree(random, trees[i], trees[i + 1]);
            }
        }
    }
    
    private void TryShareSubtree(IRandom random, ISymbolicExpressionTree<T> tree1, ISymbolicExpressionTree<T> tree2)
    {
        var nodes1 = tree1.IterateNodesPostfix().ToList();
        var nodes2 = tree2.IterateNodesPostfix().ToList();
        
        if (nodes1.Count > 1 && nodes2.Count > 1)
        {
            var sourceNode = nodes1[random.Next(1, nodes1.Count)]; // Não inclui root
            var targetIndex = random.Next(1, nodes2.Count); // Não inclui root
            var targetParent = nodes2[targetIndex].Parent;
            
            if (targetParent != null)
            {
                var childIndex = targetParent.IndexOfSubtree(nodes2[targetIndex]);
                if (childIndex >= 0)
                {
                    targetParent.ReplaceSubtree(childIndex, sourceNode); // COMPARTILHAMENTO!
                }
            }
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    private ISymbolicExpressionTreeGrammar<T> ExtractSingleOutputGrammar(
        ISymbolicExpressionTreeGrammar<IReadOnlyList<T>> multiGrammar)
    {
        // Converte gramática multi-output para single-output
        // Esta é uma simplificação - na implementação real seria mais complexa
        return SymbolicExpressionTreeGrammar as ISymbolicExpressionTreeGrammar<T>;
    }
    
    #endregion
    
    #region Cloning
    
    protected override Item CreateCloneInstance(Cloner cloner)
    {
        return new MultiOutputTreeCreator<T>(this, cloner);
    }
    
    #endregion
}
```

### 2. MultiOutputCrossover<T>

```csharp
/// <summary>
/// Operador de crossover para árvores multi-output
/// Implementa múltiplas estratégias de crossover considerando compartilhamento
/// </summary>
public class MultiOutputCrossover<T> : SymbolicExpressionTreeOperator<IReadOnlyList<T>>, 
                                       ISymbolicExpressionTreeCrossover<IReadOnlyList<T>> 
    where T : struct
{
    #region Parameters
    
    private MultiOutputCrossoverStrategy _strategy = MultiOutputCrossoverStrategy.Mixed;
    private double _outputCrossoverProbability = 0.7;
    private double _structuralCrossoverProbability = 0.3;
    
    public MultiOutputCrossoverStrategy Strategy
    {
        get => _strategy;
        set
        {
            if (_strategy != value)
            {
                _strategy = value;
                OnPropertyChanged(nameof(Strategy));
            }
        }
    }
    
    public double OutputCrossoverProbability
    {
        get => _outputCrossoverProbability;
        set
        {
            if (_outputCrossoverProbability != value)
            {
                _outputCrossoverProbability = Math.Max(0.0, Math.Min(1.0, value));
                OnPropertyChanged(nameof(OutputCrossoverProbability));
            }
        }
    }
    
    public double StructuralCrossoverProbability
    {
        get => _structuralCrossoverProbability;
        set
        {
            if (_structuralCrossoverProbability != value)
            {
                _structuralCrossoverProbability = Math.Max(0.0, Math.Min(1.0, value));
                OnPropertyChanged(nameof(StructuralCrossoverProbability));
            }
        }
    }
    
    #endregion
    
    #region Constructors
    
    public MultiOutputCrossover() : base() { }
    
    protected MultiOutputCrossover(MultiOutputCrossover<T> original, Cloner cloner) 
        : base(original, cloner)
    {
        _strategy = original._strategy;
        _outputCrossoverProbability = original._outputCrossoverProbability;
        _structuralCrossoverProbability = original._structuralCrossoverProbability;
    }
    
    #endregion
    
    #region Crossover Implementation
    
    public ISymbolicExpressionTree<IReadOnlyList<T>> Crossover(IRandom random, 
        ISymbolicExpressionTree<IReadOnlyList<T>> parent0, 
        ISymbolicExpressionTree<IReadOnlyList<T>> parent1)
    {
        if (parent0 == null || parent1 == null)
            throw new ArgumentNullException("Parents cannot be null");
            
        var multiParent0 = parent0 as IMultiSymbolicExpressionTree<T> ??
            throw new ArgumentException("Parent0 must be multi-output tree");
        var multiParent1 = parent1 as IMultiSymbolicExpressionTree<T> ??
            throw new ArgumentException("Parent1 must be multi-output tree");
        
        // Clone primeiro pai
        var offspring = (IMultiSymbolicExpressionTree<T>)multiParent0.Clone(new Cloner());
        
        return Strategy switch
        {
            MultiOutputCrossoverStrategy.OutputLevel => 
                CrossoverAtOutputLevel(random, offspring, multiParent1),
            MultiOutputCrossoverStrategy.Structural => 
                CrossoverStructural(random, offspring, multiParent1),
            MultiOutputCrossoverStrategy.Mixed => 
                CrossoverMixed(random, offspring, multiParent1),
            _ => throw new ArgumentException($"Unknown strategy: {Strategy}")
        };
    }
    
    #endregion
    
    #region Crossover Strategies
    
    /// <summary>
    /// Crossover em nível de saída - substitui árvore inteira de uma saída
    /// </summary>
    private IMultiSymbolicExpressionTree<T> CrossoverAtOutputLevel(IRandom random, 
        IMultiSymbolicExpressionTree<T> offspring, IMultiSymbolicExpressionTree<T> donor)
    {
        // Seleciona saída aleatória para substituir
        int outputIndex = random.Next(offspring.OutputCount);
        int donorOutputIndex = random.Next(donor.OutputCount);
        
        var donorOutputNode = donor.GetOutputNode(donorOutputIndex);
        if (donorOutputNode != null)
        {
            var clonedDonorNode = (ISymbolicExpressionTreeNode<T>)donorOutputNode.Clone(new Cloner());
            offspring.SetOutputNode(outputIndex, clonedDonorNode);
        }
        
        return offspring;
    }
    
    /// <summary>
    /// Crossover estrutural - pode afetar múltiplas saídas se nó for compartilhado
    /// </summary>
    private IMultiSymbolicExpressionTree<T> CrossoverStructural(IRandom random, 
        IMultiSymbolicExpressionTree<T> offspring, IMultiSymbolicExpressionTree<T> donor)
    {
        var singleCrossover = new SubtreeCrossover<T>
        {
            SymbolicExpressionTreeGrammar = ExtractSingleGrammar()
        };
        
        // Coleta todos os nós de ambas as árvores
        var offspringNodes = new List<ISymbolicExpressionTreeNode<T>>();
        var donorNodes = new List<ISymbolicExpressionTreeNode<T>>();
        
        for (int i = 0; i < offspring.OutputCount; i++)
        {
            var outputNode = offspring.GetOutputNode(i);
            if (outputNode != null)
            {
                offspringNodes.AddRange(outputNode.IterateNodesPostfix().Cast<ISymbolicExpressionTreeNode<T>>());
            }
        }
        
        for (int i = 0; i < donor.OutputCount; i++)
        {
            var outputNode = donor.GetOutputNode(i);
            if (outputNode != null)
            {
                donorNodes.AddRange(outputNode.IterateNodesPostfix().Cast<ISymbolicExpressionTreeNode<T>>());
            }
        }
        
        if (offspringNodes.Count > 0 && donorNodes.Count > 0)
        {
            // Seleciona nós para crossover
            var offspringNode = offspringNodes[random.Next(offspringNodes.Count)];
            var donorNode = donorNodes[random.Next(donorNodes.Count)];
            
            // Realiza crossover estrutural
            if (offspringNode.Parent != null)
            {
                var parent = offspringNode.Parent;
                var index = parent.IndexOfSubtree(offspringNode);
                if (index >= 0)
                {
                    var clonedDonor = (ISymbolicExpressionTreeNode)donorNode.Clone(new Cloner());
                    parent.ReplaceSubtree(index, clonedDonor);
                }
            }
        }
        
        return offspring;
    }
    
    /// <summary>
    /// Estratégia mista - combina crossover de saída e estrutural
    /// </summary>
    private IMultiSymbolicExpressionTree<T> CrossoverMixed(IRandom random, 
        IMultiSymbolicExpressionTree<T> offspring, IMultiSymbolicExpressionTree<T> donor)
    {
        if (random.NextDouble() < OutputCrossoverProbability)
        {
            return CrossoverAtOutputLevel(random, offspring, donor);
        }
        else
        {
            return CrossoverStructural(random, offspring, donor);
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    private ISymbolicExpressionTreeGrammar<T> ExtractSingleGrammar()
    {
        return SymbolicExpressionTreeGrammar as ISymbolicExpressionTreeGrammar<T>;
    }
    
    #endregion
    
    #region Cloning
    
    protected override Item CreateCloneInstance(Cloner cloner)
    {
        return new MultiOutputCrossover<T>(this, cloner);
    }
    
    #endregion
}
```

## 📊 Sistema de Avaliação Multi-Output

### MultiOutputFitnessEvaluator<T>

```csharp
/// <summary>
/// Avaliador de fitness para árvores multi-output
/// Suporta múltiplas estratégias de agregação de fitness
/// </summary>
public abstract class MultiOutputFitnessEvaluator<T> : IFitnessEvaluator<IReadOnlyList<T>> 
    where T : struct, IComparable<T>
{
    #region Parameters
    
    private FitnessAggregationStrategy _aggregationStrategy = FitnessAggregationStrategy.WeightedAverage;
    private T[] _outputWeights;
    
    public FitnessAggregationStrategy AggregationStrategy
    {
        get => _aggregationStrategy;
        set => _aggregationStrategy = value;
    }
    
    public T[] OutputWeights
    {
        get => _outputWeights;
        set => _outputWeights = value;
    }
    
    #endregion
    
    #region Abstract Methods
    
    /// <summary>
    /// Avalia o fitness de cada saída individualmente
    /// </summary>
    protected abstract T[] EvaluateIndividualOutputs(IMultiSymbolicExpressionTree<T> tree);
    
    #endregion
    
    #region Fitness Evaluation
    
    public IReadOnlyList<T> Evaluate(ISymbolicExpressionTree<IReadOnlyList<T>> tree)
    {
        var multiTree = tree as IMultiSymbolicExpressionTree<T> ??
            throw new ArgumentException("Tree must be multi-output");
            
        var individualFitnesses = EvaluateIndividualOutputs(multiTree);
        var aggregatedFitness = AggregateFeatures(individualFitnesses);
        
        return new[] { aggregatedFitness };
    }
    
    protected T AggregateFeatures(T[] individualFitnesses)
    {
        return AggregationStrategy switch
        {
            FitnessAggregationStrategy.Average => CalculateAverage(individualFitnesses),
            FitnessAggregationStrategy.WeightedAverage => CalculateWeightedAverage(individualFitnesses),
            FitnessAggregationStrategy.Minimum => individualFitnesses.Min(),
            FitnessAggregationStrategy.Maximum => individualFitnesses.Max(),
            FitnessAggregationStrategy.Product => CalculateProduct(individualFitnesses),
            _ => CalculateAverage(individualFitnesses)
        };
    }
    
    #endregion
    
    #region Aggregation Methods
    
    protected abstract T CalculateAverage(T[] values);
    protected abstract T CalculateWeightedAverage(T[] values);
    protected abstract T CalculateProduct(T[] values);
    
    #endregion
}

/// <summary>
/// Implementação específica para regressão multivariada com double
/// </summary>
public class MultiOutputRegressionEvaluator : MultiOutputFitnessEvaluator<double>
{
    private readonly double[][] _inputs;
    private readonly double[][] _targets; // targets[sample][output]
    private readonly string[] _variableNames;
    private readonly MultiOutputExpressionInterpreter _interpreter;
    
    public MultiOutputRegressionEvaluator(double[][] inputs, double[][] targets, string[] variableNames)
    {
        _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
        _targets = targets ?? throw new ArgumentNullException(nameof(targets));
        _variableNames = variableNames ?? throw new ArgumentNullException(nameof(variableNames));
        _interpreter = new MultiOutputExpressionInterpreter();
        
        // Inicializa pesos uniformes
        if (targets.Length > 0)
        {
            OutputWeights = Enumerable.Repeat(1.0 / targets[0].Length, targets[0].Length).ToArray();
        }
    }
    
    protected override double[] EvaluateIndividualOutputs(IMultiSymbolicExpressionTree<double> tree)
    {
        var outputFitnesses = new double[tree.OutputCount];
        var variables = new Dictionary<string, double>();
        
        // Calcula MSE para cada saída
        for (int output = 0; output < tree.OutputCount; output++)
        {
            double mse = 0.0;
            
            for (int sample = 0; sample < _inputs.Length; sample++)
            {
                // Prepara variáveis
                variables.Clear();
                for (int var = 0; var < _variableNames.Length; var++)
                {
                    variables[_variableNames[var]] = _inputs[sample][var];
                }
                
                // Avalia árvore
                var predictions = tree.EvaluateAll(variables);
                var prediction = output < predictions.Count ? predictions[output] : 0.0;
                var target = output < _targets[sample].Length ? _targets[sample][output] : 0.0;
                
                var error = prediction - target;
                mse += error * error;
            }
            
            mse /= _inputs.Length;
            outputFitnesses[output] = -mse; // Fitness negativo (maximizar = minimizar erro)
        }
        
        return outputFitnesses;
    }
    
    protected override double CalculateAverage(double[] values)
    {
        return values.Average();
    }
    
    protected override double CalculateWeightedAverage(double[] values)
    {
        if (OutputWeights == null || OutputWeights.Length != values.Length)
            return CalculateAverage(values);
            
        double weightedSum = 0.0;
        double totalWeight = 0.0;
        
        for (int i = 0; i < values.Length; i++)
        {
            weightedSum += values[i] * OutputWeights[i];
            totalWeight += OutputWeights[i];
        }
        
        return totalWeight > 0 ? weightedSum / totalWeight : 0.0;
    }
    
    protected override double CalculateProduct(double[] values)
    {
        // Para fitness negativos, usa produto das probabilidades normalizadas
        var normalized = values.Select(v => 1.0 / (1.0 + Math.Exp(-v))).ToArray();
        return normalized.Aggregate(1.0, (acc, v) => acc * v);
    }
}
```

## 🎯 Enumerações e Constantes

```csharp
/// <summary>
/// Estratégias de crossover para árvores multi-output
/// </summary>
public enum MultiOutputCrossoverStrategy
{
    /// <summary>
    /// Crossover em nível de saída (substitui árvore inteira de uma saída)
    /// </summary>
    OutputLevel,
    
    /// <summary>
    /// Crossover estrutural (pode afetar múltiplas saídas)
    /// </summary>
    Structural,
    
    /// <summary>
    /// Combinação das estratégias anteriores
    /// </summary>
    Mixed
}

/// <summary>
/// Estratégias de agregação de fitness
/// </summary>
public enum FitnessAggregationStrategy
{
    /// <summary>
    /// Média aritmética simples
    /// </summary>
    Average,
    
    /// <summary>
    /// Média ponderada pelos pesos definidos
    /// </summary>
    WeightedAverage,
    
    /// <summary>
    /// Mínimo fitness (estratégia conservadora)
    /// </summary>
    Minimum,
    
    /// <summary>
    /// Máximo fitness (estratégia otimista)
    /// </summary>
    Maximum,
    
    /// <summary>
    /// Produto dos fitness (para valores normalizados)
    /// </summary>
    Product
}

/// <summary>
/// Estratégias de organização das saídas
/// </summary>
public enum MultiOutputStrategy
{
    /// <summary>
    /// Saídas completamente independentes
    /// </summary>
    Independent,
    
    /// <summary>
    /// Saídas podem compartilhar subexpressões
    /// </summary>
    Shared,
    
    /// <summary>
    /// Estrutura hierárquica entre saídas
    /// </summary>
    Hierarchical
}
```

## 🧪 Exemplos de Funcionamento

### Exemplo 1: Sistema de Coordenadas 2D

```csharp
// Criar sistema que gera trajetória paramétrica
// f(t) = [x(t), y(t)] = [cos(t), sin(t)]

var grammar = new SymbolicRegressionGrammar(new[] { "t" });
var creator = new MultiOutputTreeCreator<double>
{
    SymbolicExpressionTreeGrammar = grammar,
    OutputCount = 2,
    MaxTreeDepth = 5,
    SharingProbability = 0.2
};

var tree = creator.CreateTree(new MersenneTwister(), grammar, 20, 5);
var multiTree = (MultiSymbolicExpressionTree<double>)tree;

// Definir saídas manualmente para exemplo
// Output 0: cos(t)
var cosSymbol = MathematicalSymbols.Cosine;
var cosNode = new SymbolicExpressionTreeNode<double>(cosSymbol);
cosNode.AddSubtree(new VariableTreeNode<double>(new Variable<double> { Name = "t" }));

// Output 1: sin(t)
var sinSymbol = MathematicalSymbols.Sine;
var sinNode = new SymbolicExpressionTreeNode<double>(sinSymbol);
sinNode.AddSubtree(new VariableTreeNode<double>(new Variable<double> { Name = "t" }));

multiTree.SetOutputNode(0, cosNode);
multiTree.SetOutputNode(1, sinNode);

// Avaliação
var variables = new Dictionary<string, double> { { "t", Math.PI / 4 } };
var results = multiTree.EvaluateAll(variables);

Console.WriteLine($"f(π/4) = [{results[0]:F3}, {results[1]:F3}]");
// Output: f(π/4) = [0.707, 0.707]
```

### Exemplo 2: Predição Financeira Multi-Asset

```csharp
// Sistema que prediz preços de múltiplos ativos
// f(volume, rsi, ma) = [price_AAPL, price_MSFT, price_GOOGL]

var variables = new[] { "volume", "rsi", "ma" };
var grammar = new SymbolicRegressionGrammar(variables);

// Dados de treinamento (samples x variables)
var inputs = new double[][]
{
    new double[] { 1000000, 45.5, 150.2 },
    new double[] { 1200000, 52.1, 148.7 },
    new double[] { 800000, 38.9, 151.8 }
};

// Targets (samples x outputs)  
var targets = new double[][]
{
    new double[] { 150.5, 280.2, 2800.1 }, // AAPL, MSFT, GOOGL
    new double[] { 149.8, 282.5, 2795.3 },
    new double[] { 151.2, 278.9, 2810.7 }
};

// Criador e avaliador
var creator = new MultiOutputTreeCreator<double>
{
    OutputCount = 3, // 3 assets
    SharingProbability = 0.4 // 40% chance de compartilhar subexpressões
};

var evaluator = new MultiOutputRegressionEvaluator(inputs, targets, variables)
{
    AggregationStrategy = FitnessAggregationStrategy.WeightedAverage,
    OutputWeights = new[] { 0.4, 0.3, 0.3 } // AAPL mais importante
};

// Criar população inicial
var population = new List<ISymbolicExpressionTree<IReadOnlyList<double>>>();
for (int i = 0; i < 100; i++)
{
    var tree = creator.CreateTree(new MersenneTwister(), grammar, 25, 6);
    population.Add(tree);
}

// Avaliar população
foreach (var tree in population)
{
    var fitness = evaluator.Evaluate(tree);
    Console.WriteLine($"Fitness: {fitness[0]:F4}");
}
```

### Exemplo 3: Compartilhamento de Subexpressões

```csharp
// Exemplo onde múltiplas saídas compartilham cálculos intermediários
// f(x,y) = [x+y, 2*(x+y), (x+y)^2]

var grammar = new SymbolicRegressionGrammar(new[] { "x", "y" });
var multiTree = new MultiSymbolicExpressionTree<double>(3);

// Criar subexpressão compartilhada: x + y
var addSymbol = MathematicalSymbols.Addition;
var sharedAddNode = new SymbolicExpressionTreeNode<double>(addSymbol);
sharedAddNode.AddSubtree(new VariableTreeNode<double>(new Variable<double> { Name = "x" }));
sharedAddNode.AddSubtree(new VariableTreeNode<double>(new Variable<double> { Name = "y" }));

// Output 0: x + y (diretamente)
multiTree.SetOutputNode(0, sharedAddNode);

// Output 1: 2 * (x + y) (compartilha sharedAddNode)
var multiplySymbol = MathematicalSymbols.Multiplication;
var multiplyNode = new SymbolicExpressionTreeNode<double>(multiplySymbol);
multiplyNode.AddSubtree(new ConstantTreeNode<double>(new Constant<double>, 2.0));
multiplyNode.AddSubtree(sharedAddNode); // MESMO NÓ!
multiTree.SetOutputNode(1, multiplyNode);

// Output 2: (x + y)^2 (também compartilha sharedAddNode)
var powerSymbol = MathematicalSymbols.Power;
var powerNode = new SymbolicExpressionTreeNode<double>(powerSymbol);
powerNode.AddSubtree(sharedAddNode); // MESMO NÓ!
powerNode.AddSubtree(new ConstantTreeNode<double>(new Constant<double>, 2.0));
multiTree.SetOutputNode(2, powerNode);

// Verificar compartilhamento
var sharedNodes = multiTree.GetSharedNodes();
Console.WriteLine($"Nós compartilhados: {sharedNodes.Count}");
// Output: Nós compartilhados: 3 (sharedAddNode + suas variáveis)

// Avaliação
var variables = new Dictionary<string, double> { { "x", 3.0 }, { "y", 2.0 } };
var results = multiTree.EvaluateAll(variables);
Console.WriteLine($"f(3,2) = [{results[0]}, {results[1]}, {results[2]}]");
// Output: f(3,2) = [5, 10, 25]
```

## 🔄 Fluxo de Evolução

### Algoritmo Genético Multi-Output

```
1. INICIALIZAÇÃO
   ├── Criar população de MultiSymbolicExpressionTree<T>
   ├── Cada indivíduo tem N saídas (OutputCount)
   └── Aplicar SharingProbability para compartilhamento inicial

2. AVALIAÇÃO
   ├── Para cada árvore na população:
   │   ├── EvaluateAll() → calcula todas as saídas
   │   ├── EvaluateIndividualOutputs() → fitness por saída
   │   └── AggregateFeatures() → fitness final
   
3. SELEÇÃO
   ├── Usar fitness agregado para seleção
   └── Preservar informações de fitness por saída

4. CROSSOVER
   ├── 70% OutputLevel → substitui árvore de saída completa
   ├── 30% Structural → pode afetar múltiplas saídas
   └── Preservar compartilhamentos benéficos

5. MUTAÇÃO  
   ├── Mutação por saída (independente)
   ├── Mutação estrutural (afeta compartilhados)
   └── Probabilidade configurável por tipo

6. SUBSTITUIÇÃO
   ├── Manter diversidade de saídas
   └── Considerar fitness individual e agregado
```

## ⚠️ Considerações de Implementação

### 1. **Gerenciamento de Memória**
- Compartilhamento de nós reduz uso de memória
- Clonagem deve preservar referências compartilhadas
- Cuidado com vazamentos em estruturas circulares

### 2. **Paralelização**
- Avaliação de saídas pode ser paralelizada
- Crossover estrutural requer sincronização
- Fitness por saída calculado independentemente

### 3. **Validação de Tipos**
- Verificar compatibilidade durante crossover
- Validar gramática para tipo T
- Garantir homogeneidade das saídas

### 4. **Performance**
- Cache de avaliações de nós compartilhados
- Otimização de travessia de árvore  
- Reuso de interpretadores

## 🎯 Métricas e Monitoramento

### Métricas Específicas Multi-Output

```csharp
public class MultiOutputMetrics
{
    public double[] IndividualFitnesses { get; set; }
    public double AggregatedFitness { get; set; }
    public int SharedNodeCount { get; set; }
    public double SharingRatio { get; set; }
    public double[] OutputDiversity { get; set; }
    public double StructuralComplexity { get; set; }
    
    public double CalculateOverallQuality()
    {
        // Combina fitness, compartilhamento e diversidade
        var fitnessScore = AggregatedFitness;
        var sharingBonus = SharingRatio * 0.1; // 10% bonus por compartilhamento
        var complexityPenalty = StructuralComplexity * 0.05; // 5% penalty
        
        return fitnessScore + sharingBonus - complexityPenalty;
    }
}
```

---

## 📋 Resumo da Arquitetura

Esta implementação oferece:

✅ **Função Vetorial**: Uma árvore que produz múltiplas saídas homogêneas  
✅ **Compartilhamento Inteligente**: Nós podem ser reutilizados entre saídas  
✅ **Crossover Adaptativo**: Estratégias para diferentes níveis estruturais  
✅ **Avaliação Flexível**: Múltiplas formas de agregar fitness  
✅ **Escalabilidade**: Suporte a qualquer número de saídas  
✅ **Compatibilidade**: Integra com sistema existente  
✅ **Extensibilidade**: Base para Etapa 3 (tipos heterogêneos)

A arquitetura permite que crossover em nós compartilhados afete múltiplas saídas, exatamente como você solicitou, mantendo a eficiência e flexibilidade do sistema.
