using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Operators;
using GeneticProgramming.Abstractions.Operators;
using AbstractionOptimization = GeneticProgramming.Abstractions.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticProgramming.Algorithms
{
    /// <summary>
    /// Basic genetic programming algorithm implementation
    /// </summary>
    public class GeneticProgrammingAlgorithm : Item,
        IGeneticProgrammingAlgorithm,
        Abstractions.Optimization.IGeneticProgrammingAlgorithm
    {
        private int _populationSize = 100;
        private int _maxGenerations = 50;
        private int _maxTreeLength = 25;
        private int _maxTreeDepth = 10;
        private double _crossoverProbability = 0.9;
        private double _mutationProbability = 0.1;
        private int _eliteCount = 1; // Número de elites a preservar
        private double _eliteBreedingRatio = 0.3; // % da população gerada a partir dos elites
        private ISymbolicExpressionTreeGrammar? _grammar;
        private ISymbolicExpressionTreeCreator? _treeCreator;
        private ISymbolicExpressionTreeCrossover? _crossover;
        private ISymbolicExpressionTreeMutator? _mutator;
        private IRandom? _random;
        private ISymbolicExpressionTreeSelector? _selector;
        private int _generation;
        private List<ISymbolicExpressionTree> _population;
        private ISymbolicExpressionTree? _bestIndividual;
        private double _bestFitness = double.NegativeInfinity;
        private bool _stopRequested;
        private GeneticProgramming.Problems.Evaluators.IFitnessEvaluator? _fitnessEvaluator;
        private bool _enableParallelEvaluation = true;

        /// <summary>
        /// Gets or sets the population size
        /// </summary>
        public int PopulationSize
        {
            get => _populationSize;
            set
            {
                if (_populationSize != value && value > 0)
                {
                    _populationSize = value;
                    OnPropertyChanged(nameof(PopulationSize));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of generations
        /// </summary>
        public int MaxGenerations
        {
            get => _maxGenerations;
            set
            {
                if (_maxGenerations != value && value > 0)
                {
                    _maxGenerations = value;
                    OnPropertyChanged(nameof(MaxGenerations));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum tree length
        /// </summary>
        public int MaxTreeLength
        {
            get => _maxTreeLength;
            set
            {
                if (_maxTreeLength != value && value > 0)
                {
                    _maxTreeLength = value;
                    OnPropertyChanged(nameof(MaxTreeLength));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum tree depth
        /// </summary>
        public int MaxTreeDepth
        {
            get => _maxTreeDepth;
            set
            {
                if (_maxTreeDepth != value && value > 0)
                {
                    _maxTreeDepth = value;
                    OnPropertyChanged(nameof(MaxTreeDepth));
                }
            }
        }

        /// <summary>
        /// Gets or sets the crossover probability
        /// </summary>
        public double CrossoverProbability
        {
            get => _crossoverProbability;
            set
            {
                if (_crossoverProbability != value && value >= 0.0 && value <= 1.0)
                {
                    _crossoverProbability = value;
                    OnPropertyChanged(nameof(CrossoverProbability));
                }
            }
        }

        /// <summary>
        /// Gets or sets the mutation probability
        /// </summary>
        public double MutationProbability
        {
            get => _mutationProbability;
            set
            {
                if (_mutationProbability != value && value >= 0.0 && value <= 1.0)
                {
                    _mutationProbability = value;
                    OnPropertyChanged(nameof(MutationProbability));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of elite individuals to preserve each generation
        /// </summary>
        public int EliteCount
        {
            get => _eliteCount;
            set
            {
                if (_eliteCount != value && value >= 0)
                {
                    _eliteCount = value;
                    OnPropertyChanged(nameof(EliteCount));
                }
            }
        }

        /// <summary>
        /// Gets or sets the ratio of population generated from elite breeding (0.0 to 1.0)
        /// </summary>
        public double EliteBreedingRatio
        {
            get => _eliteBreedingRatio;
            set
            {
                if (_eliteBreedingRatio != value && value >= 0.0 && value <= 1.0)
                {
                    _eliteBreedingRatio = value;
                    OnPropertyChanged(nameof(EliteBreedingRatio));
                }
            }
        }

        /// <summary>
        /// Gets or sets the grammar used for tree creation
        /// </summary>
        public ISymbolicExpressionTreeGrammar? Grammar
        {
            get => _grammar;
            set
            {
                if (_grammar != value)
                {
                    _grammar = value;
                    OnPropertyChanged(nameof(Grammar));
                }
            }
        }

        /// <summary>
        /// Gets or sets the tree creator
        /// </summary>
        public ISymbolicExpressionTreeCreator? TreeCreator
        {
            get => _treeCreator;
            set
            {
                if (_treeCreator != value)
                {
                    _treeCreator = value;
                    OnPropertyChanged(nameof(TreeCreator));
                }
            }
        }

        /// <summary>
        /// Gets or sets the crossover operator
        /// </summary>
        public ISymbolicExpressionTreeCrossover? Crossover
        {
            get => _crossover;
            set
            {
                if (_crossover != value)
                {
                    _crossover = value;
                    OnPropertyChanged(nameof(Crossover));
                }
            }
        }

        /// <summary>
        /// Gets or sets the mutator
        /// </summary>
        public ISymbolicExpressionTreeMutator? Mutator
        {
            get => _mutator;
            set
            {
                if (_mutator != value)
                {
                    _mutator = value;
                    OnPropertyChanged(nameof(Mutator));
                }
            }
        }

        /// <summary>
        /// Gets or sets the selection operator used to choose parents.
        /// </summary>
        public ISymbolicExpressionTreeSelector? Selector
        {
            get => _selector;
            set
            {
                if (_selector != value)
                {
                    _selector = value;
                    OnPropertyChanged(nameof(Selector));
                }
            }
        }

        /// <summary>
        /// Gets or sets the random number generator
        /// </summary>
        public IRandom? Random
        {
            get => _random;
            set
            {
                if (_random != value)
                {
                    _random = value;
                    OnPropertyChanged(nameof(Random));
                }
            }
        }

        /// <summary>
        /// Gets or sets the fitness evaluator used for individuals.
        /// </summary>
        public GeneticProgramming.Problems.Evaluators.IFitnessEvaluator? FitnessEvaluator
        {
            get => _fitnessEvaluator;
            set
            {
                if (_fitnessEvaluator != value)
                {
                    _fitnessEvaluator = value;
                    OnPropertyChanged(nameof(FitnessEvaluator));
                }
            }
        }

        /// <summary>
        /// Gets the current generation
        /// </summary>
        public int Generation => _generation;

        /// <summary>
        /// Gets the current population
        /// </summary>
        public IList<ISymbolicExpressionTree> Population => _population.AsReadOnly();

        /// <summary>
        /// Gets the best individual found so far
        /// </summary>
        public ISymbolicExpressionTree? BestIndividual => _bestIndividual;

        /// <summary>
        /// Gets the fitness of the best individual
        /// </summary>
        public double BestFitness => _bestFitness;

        /// <summary>
        /// Event raised when a generation is completed
        /// </summary>
        public event EventHandler<GenerationEventArgs>? GenerationCompleted;

        // Explicit abstraction event
        public event EventHandler? IterationCompleted;

        /// <summary>
        /// Gets or sets whether parallel evaluation is enabled for fitness calculations
        /// </summary>
        public bool EnableParallelEvaluation
        {
            get => _enableParallelEvaluation;
            set
            {
                if (_enableParallelEvaluation != value)
                {
                    _enableParallelEvaluation = value;
                    OnPropertyChanged(nameof(EnableParallelEvaluation));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the GeneticProgrammingAlgorithm class
        /// </summary>
        public GeneticProgrammingAlgorithm() : base()
        {
            _population = new List<ISymbolicExpressionTree>();
        }

        /// <summary>
        /// Initializes a new instance of the GeneticProgrammingAlgorithm class
        /// </summary>
        /// <param name="original">The original algorithm to copy from</param>
        /// <param name="cloner">The cloner to use for deep copying</param>
        protected GeneticProgrammingAlgorithm(GeneticProgrammingAlgorithm original, Cloner cloner) : base(original, cloner)
        {
            _populationSize = original._populationSize;
            _maxGenerations = original._maxGenerations;
            _maxTreeLength = original._maxTreeLength;
            _maxTreeDepth = original._maxTreeDepth;
            _crossoverProbability = original._crossoverProbability;
            _mutationProbability = original._mutationProbability;
            _grammar = cloner.Clone(original._grammar);
            _treeCreator = cloner.Clone(original._treeCreator);
            _crossover = cloner.Clone(original._crossover);
            _mutator = cloner.Clone(original._mutator);
            _random = cloner.Clone(original._random);
            _selector = cloner.Clone(original._selector);
            _generation = original._generation;
            _population = new List<ISymbolicExpressionTree>(original._population.Select(ind => cloner.Clone(ind)).Where(ind => ind != null)!);
            _bestIndividual = cloner.Clone(original._bestIndividual);
            _bestFitness = original._bestFitness;
            _stopRequested = original._stopRequested;
        }

        /// <summary>
        /// Creates a deep clone of this algorithm
        /// </summary>
        /// <param name="cloner">The cloner to use</param>
        /// <returns>A deep clone of this algorithm</returns>
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new GeneticProgrammingAlgorithm(this, cloner);
        }

        /// <summary>
        /// Runs the genetic programming algorithm
        /// </summary>
        public virtual void Run()
        {
            ValidateParameters();
            Initialize();

            while (_generation < _maxGenerations && !_stopRequested)
            {
                EvaluatePopulation();
                UpdateBestIndividual();

                // Raise generation completed event with conditional parallel average calculation
                var averageFitness = _enableParallelEvaluation 
                    ? _population.AsParallel().Average(EvaluateFitness)
                    : _population.Average(EvaluateFitness);
                GenerationCompleted?.Invoke(this, new GenerationEventArgs(_generation, _bestFitness, averageFitness, _bestIndividual!));
                IterationCompleted?.Invoke(this, EventArgs.Empty);

                if (_generation < _maxGenerations - 1)
                {
                    CreateNextGeneration();
                }

                _generation++;
            }
        }

        /// <summary>
        /// Stops the algorithm execution
        /// </summary>
        public virtual void Stop()
        {
            _stopRequested = true;
        }

        /// <summary>
        /// Evaluates the fitness of an individual
        /// </summary>
        /// <param name="individual">The individual to evaluate</param>
        /// <returns>The fitness value (higher is better)</returns>
        public virtual double EvaluateFitness(ISymbolicExpressionTree individual)
        {
            if (_fitnessEvaluator != null)
            {
                return _fitnessEvaluator.Evaluate(individual);
            }

            // Default implementation - just return negative tree size (for parsimony)
            return -individual.Length;
        }

        double AbstractionOptimization.IGeneticProgrammingAlgorithm.EvaluateFitness(object individual)
        {
            if (individual is ISymbolicExpressionTree tree)
                return EvaluateFitness(tree);
            throw new ArgumentException("Individual must be an ISymbolicExpressionTree", nameof(individual));
        }

        private void ValidateParameters()
        {
            if (_grammar == null) throw new InvalidOperationException("Grammar must be set");
            if (_treeCreator == null) throw new InvalidOperationException("TreeCreator must be set");
            if (_crossover == null) throw new InvalidOperationException("Crossover must be set");
            if (_mutator == null) throw new InvalidOperationException("Mutator must be set");
            if (_random == null) throw new InvalidOperationException("Random must be set");
            if (_selector == null) throw new InvalidOperationException("Selector must be set");
        }

        private void Initialize()
        {
            _generation = 0;
            _stopRequested = false;
            _bestFitness = double.NegativeInfinity;
            _bestIndividual = null;
            _population.Clear();

            // Set operator grammars
            _treeCreator!.SymbolicExpressionTreeGrammar = _grammar;
            _crossover!.SymbolicExpressionTreeGrammar = _grammar;
            _mutator!.SymbolicExpressionTreeGrammar = _grammar;

            // Create initial population
            for (int i = 0; i < _populationSize; i++)
            {
                var individual = _treeCreator.CreateTree(_random!, _grammar!, _maxTreeLength, _maxTreeDepth);
                _population.Add(individual);
            }
        }

        private void EvaluatePopulation()
        {
            if (_enableParallelEvaluation)
            {
                // Parallel evaluation for better performance
                _population.AsParallel().ForAll(individual =>
                {
                    // In a real implementation, you might want to cache fitness values
                    var fitness = EvaluateFitness(individual);
                });
            }
            else
            {
                // Sequential evaluation
                foreach (var individual in _population)
                {
                    var fitness = EvaluateFitness(individual);
                }
            }
        }

        private void UpdateBestIndividual()
        {
            // reset for this generation so BestFitness is the best individual in current generation
            _bestFitness = double.NegativeInfinity;
            
            if (_enableParallelEvaluation)
            {
                // Use parallel evaluation with thread-safe best tracking
                var bestResult = _population.AsParallel()
                    .Select(individual => new { Individual = individual, Fitness = EvaluateFitness(individual) })
                    .Aggregate((best, current) => current.Fitness > best.Fitness ? current : best);
                
                if (bestResult.Fitness > _bestFitness)
                {
                    _bestFitness = bestResult.Fitness;
                    _bestIndividual = (ISymbolicExpressionTree)bestResult.Individual.Clone(new Cloner());
                }
            }
            else
            {
                // Sequential evaluation
                foreach (var individual in _population)
                {
                    var fitness = EvaluateFitness(individual);
                    if (fitness > _bestFitness)
                    {
                        _bestFitness = fitness;
                        _bestIndividual = (ISymbolicExpressionTree)individual.Clone(new Cloner());
                    }
                }
            }
        }

        private void CreateNextGeneration()
        {
            var newPopulation = new List<ISymbolicExpressionTree>();

            // Evaluate all individuals and get the top performers
            var evaluatedPopulation = _population
                .Select(individual => new { Individual = individual, Fitness = EvaluateFitness(individual) })
                .OrderByDescending(x => x.Fitness)
                .ToList();

            // 1. ELITISMO: Preservar os N melhores indivíduos
            var elites = evaluatedPopulation.Take(_eliteCount).ToList();
            foreach (var elite in elites)
            {
                newPopulation.Add((ISymbolicExpressionTree)elite.Individual.Clone(new Cloner()));
            }

            // 2. ELITE BREEDING: Gerar parte da população cruzando elites entre si
            int eliteBreedingCount = (int)(_populationSize * _eliteBreedingRatio);
            for (int i = 0; i < eliteBreedingCount && newPopulation.Count < _populationSize; i++)
            {
                // Selecionar dois elites aleatoriamente para crossover
                var parent1 = elites[_random!.Next(elites.Count)].Individual;
                var parent2 = elites[_random!.Next(elites.Count)].Individual;
                
                var offspring = _crossover!.Crossover(_random, parent1, parent2);
                
                // Aplicar mutação com probabilidade menor nos filhos dos elites
                if (_random.NextDouble() < _mutationProbability * 0.5) // 50% da probabilidade normal
                {
                    offspring = _mutator!.Mutate(_random, offspring);
                }
                
                newPopulation.Add(offspring);
            }

            // 3. REPRODUÇÃO NORMAL: Preencher resto da população
            while (newPopulation.Count < _populationSize)
            {
                if (_random!.NextDouble() < _crossoverProbability && newPopulation.Count < _populationSize - 1)
                {
                    // Crossover normal com seleção por torneio
                    var parent1 = _selector!.Select(_random!, _population, EvaluateFitness);
                    var parent2 = _selector!.Select(_random!, _population, EvaluateFitness);
                    var offspring = _crossover!.Crossover(_random, parent1, parent2);
                    
                    if (_random.NextDouble() < _mutationProbability)
                    {
                        offspring = _mutator!.Mutate(_random, offspring);
                    }
                    
                    newPopulation.Add(offspring);
                }
                else
                {
                    // Mutação apenas
                    var parent = _selector!.Select(_random!, _population, EvaluateFitness);
                    var offspring = _mutator!.Mutate(_random, parent);
                    newPopulation.Add(offspring);
                }
            }

            // Garantir tamanho exato da população
            while (newPopulation.Count > _populationSize)
            {
                newPopulation.RemoveAt(newPopulation.Count - 1);
            }

            _population = newPopulation;
        }

        private ISymbolicExpressionTree TournamentSelection(int tournamentSize = 3)
        {
            ISymbolicExpressionTree? best = null;
            double bestFitness = double.NegativeInfinity;

            for (int i = 0; i < tournamentSize; i++)
            {
                var candidate = _population[_random!.Next(_population.Count)];
                var fitness = EvaluateFitness(candidate);
                
                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    best = candidate;
                }
            }

            return (ISymbolicExpressionTree)best!.Clone(new Cloner());
        }

        protected override IDeepCloneable CreateCloneInstance(Cloner cloner)
        {
            return new GeneticProgrammingAlgorithm(this, cloner);
        }
    }
}
