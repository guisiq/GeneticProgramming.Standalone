using GeneticProgramming.Core;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Operators;
using GeneticProgramming.Algorithms;
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.Examples
{
    /// <summary>
    /// Comprehensive test class demonstrating all GP framework functionality
    /// </summary>
    public class BasicGPTest
    {
        private readonly IRandom random;

        public BasicGPTest()
        {
            random = new MersenneTwister(42); // Fixed seed for reproducible results
        }

        public void RunAllTests()
        {
            Console.WriteLine("=== Genetic Programming Framework Test ===\n");

            try
            {
                TestCoreFramework();
                TestExpressionTrees();
                TestGrammars();
                TestOperators();
                TestCompleteGPAlgorithm();
                
                Console.WriteLine("\n=== ALL TESTS COMPLETED SUCCESSFULLY ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void TestCoreFramework()
        {
            Console.WriteLine("1. Testing Core Framework...");
            
            // Test data types
            var intVal = new IntValue(42);
            var doubleVal = new DoubleValue(3.14);
            var boolVal = new BoolValue(true);
            var stringVal = new StringValue("test");
            
            Console.WriteLine($"   IntValue: {intVal.Value}");
            Console.WriteLine($"   DoubleValue: {doubleVal.Value}");
            Console.WriteLine($"   BoolValue: {boolVal.Value}");
            Console.WriteLine($"   StringValue: {stringVal.Value}");
            
            // Test cloning
            var cloner = new Cloner();
            var clonedInt = cloner.Clone(intVal);
            Console.WriteLine($"   Cloned IntValue: {clonedInt.Value}");
            
            // Test random number generator
            Console.WriteLine($"   Random double: {random.NextDouble():F4}");
            Console.WriteLine($"   Random int (1-10): {random.Next(1, 11)}");
            
            Console.WriteLine("   ✓ Core Framework tests passed\n");
        }

        private void TestExpressionTrees()
        {
            Console.WriteLine("2. Testing Expression Trees...");
            
            // Create a simple tree: (X + 5)
            var tree = new SymbolicExpressionTree();
            var addNode = new Addition().CreateTreeNode();
            var varNode = new VariableTreeNode(new Variable(), "X");
            var constNode = new ConstantTreeNode(new Constant(), 5.0);
            
            addNode.AddSubtree(varNode);
            addNode.AddSubtree(constNode);
            tree.Root = addNode;
            
            Console.WriteLine($"   Tree size: {tree.Length}");
            Console.WriteLine($"   Tree depth: {tree.Root.GetDepth()}");
            Console.WriteLine($"   Tree structure: {GetTreeString(tree.Root)}");
            
            // Test iterators
            var prefixNodes = tree.IterateNodesPrefix().ToList();
            var postfixNodes = tree.IterateNodesPostfix().ToList();
            var breadthFirstNodes = tree.IterateNodesBreadthFirst().ToList();
            
            Console.WriteLine($"   Prefix iteration: {prefixNodes.Count} nodes");
            Console.WriteLine($"   Postfix iteration: {postfixNodes.Count} nodes");
            Console.WriteLine($"   Breadth-first iteration: {breadthFirstNodes.Count} nodes");
            
            // Test cloning
            var clonedTree = (ISymbolicExpressionTree)tree.Clone(new Cloner());
            Console.WriteLine($"   Cloned tree size: {clonedTree.Length}");
            
            Console.WriteLine("   ✓ Expression Tree tests passed\n");
        }

        private void TestGrammars()
        {
            Console.WriteLine("3. Testing Grammars...");
            
            // Test default grammar
            var defaultGrammar = new DefaultSymbolicExpressionTreeGrammar();
            Console.WriteLine($"   Default grammar symbols: {defaultGrammar.Symbols.Count()}");
            
            foreach (var symbol in defaultGrammar.Symbols)
            {
                Console.WriteLine($"     - {symbol.SymbolName} (arity: {symbol.MinimumArity}-{symbol.MaximumArity})");
            }
            
            // Test symbolic regression grammar
            var regressionGrammar = new SymbolicRegressionGrammar();
            Console.WriteLine($"   Regression grammar symbols: {regressionGrammar.Symbols.Count()}");
            
            // Test grammar validation
            var isValid = regressionGrammar.Validate();
            Console.WriteLine($"   Grammar validation: {(isValid ? "✓ Valid" : "✗ Invalid")}");
            
            Console.WriteLine("   ✓ Grammar tests passed\n");
        }

        private void TestOperators()
        {
            Console.WriteLine("4. Testing Genetic Operators...");
            
            var grammar = new SymbolicRegressionGrammar();
            
            // Test tree creators
            Console.WriteLine("   Testing Tree Creators:");
            var growCreator = new GrowTreeCreator();
            var fullCreator = new FullTreeCreator();
            
            var grownTree = growCreator.CreateTree(random, grammar, 15, 5);
            var fullTree = fullCreator.CreateTree(random, grammar, 15, 5);
            
            Console.WriteLine($"     Grown tree: size={grownTree.Length}, depth={grownTree.Root.GetDepth()}");
            Console.WriteLine($"     Full tree: size={fullTree.Length}, depth={fullTree.Root.GetDepth()}");
            
            // Test crossover operators
            Console.WriteLine("   Testing Crossover:");
            var subtreeCrossover = new SubtreeCrossover();
            var onePointCrossover = new OnePointCrossover();
            
            var offspring1 = subtreeCrossover.Crossover(random, grownTree, fullTree);
            var offspring2 = onePointCrossover.Crossover(random, grownTree, fullTree);
            
            Console.WriteLine($"     Subtree crossover result: size={offspring1.Length}");
            Console.WriteLine($"     One-point crossover result: size={offspring2.Length}");
            
            // Test mutation operators
            Console.WriteLine("   Testing Mutation:");
            var subtreeMutator = new SubtreeMutator { SymbolicExpressionTreeGrammar = grammar };
            var nodeTypeMutator = new ChangeNodeTypeMutator { SymbolicExpressionTreeGrammar = grammar };
            var terminalMutator = new ChangeTerminalMutator { SymbolicExpressionTreeGrammar = grammar };
            
            var mutated1 = subtreeMutator.Mutate(random, grownTree);
            var mutated2 = nodeTypeMutator.Mutate(random, fullTree);
            var mutated3 = terminalMutator.Mutate(random, grownTree);
            
            Console.WriteLine($"     Subtree mutation result: size={mutated1.Length}");
            Console.WriteLine($"     Node type mutation result: size={mutated2.Length}");
            Console.WriteLine($"     Terminal mutation result: size={mutated3.Length}");
            
            Console.WriteLine("   ✓ Operator tests passed\n");
        }

        private void TestCompleteGPAlgorithm()
        {
            Console.WriteLine("5. Testing Complete GP Algorithm...");
            
            // Create a simple symbolic regression problem: f(x) = x^2 + 2x + 1
            var algorithm = new TestGPAlgorithm();
            
            // Configure algorithm
            algorithm.PopulationSize = 20;
            algorithm.MaxGenerations = 10;
            algorithm.MaxTreeLength = 15;
            algorithm.MaxTreeDepth = 5;
            algorithm.CrossoverProbability = 0.9;
            algorithm.MutationProbability = 0.1;
            
            // Set operators and grammar
            algorithm.Grammar = new SymbolicRegressionGrammar();
            algorithm.TreeCreator = new GrowTreeCreator();
            algorithm.Crossover = new SubtreeCrossover();
            algorithm.Mutator = new SubtreeMutator();
            algorithm.Random = random;
            
            // Subscribe to generation events
            algorithm.GenerationCompleted += (sender, e) =>
            {
                Console.WriteLine($"     Generation {e.Generation}: Best={e.BestFitness:F4}, Avg={e.AverageFitness:F4}");
            };
            
            Console.WriteLine("   Running GP algorithm...");
            algorithm.Run();
            
            Console.WriteLine($"   Final best fitness: {algorithm.BestFitness:F4}");
            Console.WriteLine($"   Best individual size: {algorithm.BestIndividual?.Length ?? 0}");
            if (algorithm.BestIndividual != null)
            {
                Console.WriteLine($"   Best individual: {GetTreeString(algorithm.BestIndividual.Root)}");
            }
            
            Console.WriteLine("   ✓ Complete GP Algorithm test passed\n");
        }

        private string GetTreeString(ISymbolicExpressionTreeNode node)
        {
            if (node.SubtreeCount == 0)
            {
                if (node is ConstantTreeNode constNode)
                    return constNode.Value.ToString("F2");
                else if (node is VariableTreeNode varNode)
                    return varNode.VariableName;
                else
                    return node.Symbol.SymbolName;
            }
            else
            {
                var children = string.Join(", ", node.Subtrees.Select(GetTreeString));
                return $"{node.Symbol.SymbolName}({children})";
            }
        }
    }

    /// <summary>
    /// Test GP algorithm with a simple fitness function
    /// </summary>
    public class TestGPAlgorithm : GeneticProgrammingAlgorithm
    {
        // Simple test data for symbolic regression: f(x) = x^2 + 2x + 1
        private readonly double[] testInputs = { -2, -1, 0, 1, 2, 3 };
        private readonly double[] expectedOutputs = { 1, 0, 1, 4, 9, 16 };

        public override double EvaluateFitness(ISymbolicExpressionTree individual)
        {
            try
            {
                double totalError = 0;
                
                for (int i = 0; i < testInputs.Length; i++)
                {
                    var predicted = EvaluateTree(individual.Root, testInputs[i]);
                    var error = Math.Abs(predicted - expectedOutputs[i]);
                    totalError += error;
                }
                
                // Return negative error (higher fitness is better)
                var avgError = totalError / testInputs.Length;
                var fitness = 1.0 / (1.0 + avgError);
                
                // Add parsimony pressure (prefer smaller trees)
                var sizePenalty = individual.Length * 0.001;
                return fitness - sizePenalty;
            }
            catch
            {
                // Return very low fitness for invalid expressions
                return -1000;
            }
        }

        private double EvaluateTree(ISymbolicExpressionTreeNode node, double x)
        {
            if (node is ConstantTreeNode constNode)
            {
                return constNode.Value;
            }
            else if (node is VariableTreeNode varNode)
            {
                return x; // For simplicity, all variables evaluate to x
            }
            else if (node.Symbol is Addition && node.SubtreeCount == 2)
            {
                return EvaluateTree(node.Subtrees.ElementAt(0), x) + EvaluateTree(node.Subtrees.ElementAt(1), x);
            }
            else if (node.Symbol is Subtraction && node.SubtreeCount == 2)
            {
                return EvaluateTree(node.Subtrees.ElementAt(0), x) - EvaluateTree(node.Subtrees.ElementAt(1), x);
            }
            else if (node.Symbol is Multiplication && node.SubtreeCount == 2)
            {
                return EvaluateTree(node.Subtrees.ElementAt(0), x) * EvaluateTree(node.Subtrees.ElementAt(1), x);
            }
            else if (node.Symbol is Division && node.SubtreeCount == 2)
            {
                var denominator = EvaluateTree(node.Subtrees.ElementAt(1), x);
                if (Math.Abs(denominator) < 1e-10) return 0; // Avoid division by zero
                return EvaluateTree(node.Subtrees.ElementAt(0), x) / denominator;
            }
            else
            {
                throw new InvalidOperationException($"Unknown symbol type: {node.Symbol.GetType()}");
            }
        }
    }
}
