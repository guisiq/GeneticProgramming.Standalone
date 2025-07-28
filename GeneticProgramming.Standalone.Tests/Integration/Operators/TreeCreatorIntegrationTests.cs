using Xunit;
using GeneticProgramming.Operators;
using GeneticProgramming.Expressions;
using GeneticProgramming.Expressions.Grammars;
using GeneticProgramming.Core;
using System;
using System.Linq;

namespace GeneticProgramming.Standalone.IntegrationTests.Operators
{
    /// <summary>
    /// Testes de integração para criadores de árvores
    /// Testa a capacidade dos criadores de produzir árvores válidas com diferentes gramáticas e configurações
    /// </summary>
    public class TreeCreatorIntegrationTests
    {
        [Fact]
        public void GrowTreeCreator_CreateTree_ProducesValidTree()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            var random = new MersenneTwister(12345);
            var grammar = new SymbolicRegressionGrammar();
            const int maxTreeLength = 20;
            const int maxTreeDepth = 5;

            // Act
            var tree = creator.CreateTree(random, grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.True(tree.Length <= maxTreeLength, $"Tree length {tree.Length} exceeds max length {maxTreeLength}");
            Assert.True(tree.Depth <= maxTreeDepth, $"Tree depth {tree.Depth} exceeds max depth {maxTreeDepth}");
            Assert.True(tree.Length > 0, "Tree should have at least one node");
            Assert.Contains(tree.Root.Symbol, grammar.Symbols);
        }

        [Fact]
        public void FullTreeCreator_CreateTree_ProducesValidTree()
        {
            // Arrange
            var creator = new FullTreeCreator();
            var random = new MersenneTwister(12345);
            var grammar = new SymbolicRegressionGrammar();
            const int maxTreeLength = 20;
            const int maxTreeDepth = 4;

            // Act
            var tree = creator.CreateTree(random, grammar, maxTreeLength, maxTreeDepth);

            // Assert
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.True(tree.Length <= maxTreeLength, $"Tree length {tree.Length} exceeds max length {maxTreeLength}");
            Assert.True(tree.Depth <= maxTreeDepth, $"Tree depth {tree.Depth} exceeds max depth {maxTreeDepth}");
            Assert.True(tree.Length > 0, "Tree should have at least one node");
            Assert.Contains(tree.Root.Symbol, grammar.Symbols);
        }

        [Fact]
        public void TreeCreators_ProduceDifferentTrees_WithDifferentSeeds()
        {
            // Arrange
            var growCreator = new GrowTreeCreator();
            var fullCreator = new FullTreeCreator();
            var grammar = new SymbolicRegressionGrammar();
            var random1 = new MersenneTwister(12345);
            var random2 = new MersenneTwister(67890);
            var random3 = new MersenneTwister(11111);
            var random4 = new MersenneTwister(22222);
            const int maxTreeLength = 15;
            const int maxTreeDepth = 4;

            // Act
            var growTree1 = growCreator.CreateTree(random1, grammar, maxTreeLength, maxTreeDepth);
            var growTree2 = growCreator.CreateTree(random2, grammar, maxTreeLength, maxTreeDepth);
            var fullTree1 = fullCreator.CreateTree(random3, grammar, maxTreeLength, maxTreeDepth);
            var fullTree2 = fullCreator.CreateTree(random4, grammar, maxTreeLength, maxTreeDepth);

            // Assert - Trees should be different (not identical structure)
            // Note: We compare actual tree structure, not just ToString()
            var tree1Nodes = string.Join(",", growTree1.Root.IterateNodesPrefix().Select(n => n.Symbol.Name));
            var tree2Nodes = string.Join(",", growTree2.Root.IterateNodesPrefix().Select(n => n.Symbol.Name));
            var tree3Nodes = string.Join(",", fullTree1.Root.IterateNodesPrefix().Select(n => n.Symbol.Name));
            var tree4Nodes = string.Join(",", fullTree2.Root.IterateNodesPrefix().Select(n => n.Symbol.Name));
            
            // At least some should be different (allowing for some chance of similarity)
            var allSame = tree1Nodes == tree2Nodes && tree2Nodes == tree3Nodes && tree3Nodes == tree4Nodes;
            Assert.False(allSame, "All trees have identical structure, which is statistically unlikely");
            
            // All trees should be valid
            ValidateTreeStructure(growTree1, grammar, maxTreeLength, maxTreeDepth);
            ValidateTreeStructure(growTree2, grammar, maxTreeLength, maxTreeDepth);
            ValidateTreeStructure(fullTree1, grammar, maxTreeLength, maxTreeDepth);
            ValidateTreeStructure(fullTree2, grammar, maxTreeLength, maxTreeDepth);
        }

        [Fact]
        public void TreeCreators_RespectMaxDepthConstraints()
        {
            // Arrange
            var creators = new ISymbolicExpressionTreeCreator[]
            {
                new GrowTreeCreator(),
                new FullTreeCreator()
            };
            var grammar = new SymbolicRegressionGrammar();
            var random = new MersenneTwister(12345);
            var maxDepths = new[] { 2, 3, 4, 5 };
            const int maxTreeLength = 50;

            foreach (var creator in creators)
            {
                foreach (var maxDepth in maxDepths)
                {
                    // Act
                    var tree = creator.CreateTree(random, grammar, maxTreeLength, maxDepth);

                    // Assert
                    Assert.True(tree.Depth <= maxDepth, 
                        $"{creator.GetType().Name} produced tree with depth {tree.Depth}, max allowed was {maxDepth}");
                    ValidateTreeStructure(tree, grammar, maxTreeLength, maxDepth);
                }
            }
        }

        [Fact]
        public void TreeCreators_RespectMaxLengthConstraints()
        {
            // Arrange
            var creators = new ISymbolicExpressionTreeCreator[]
            {
                new GrowTreeCreator(),
                new FullTreeCreator()
            };
            var grammar = new SymbolicRegressionGrammar();
            var random = new MersenneTwister(12345);
            var maxLengths = new[] { 5, 10, 15, 20 };
            const int maxTreeDepth = 6;

            foreach (var creator in creators)
            {
                foreach (var maxLength in maxLengths)
                {
                    // Act
                    var tree = creator.CreateTree(random, grammar, maxLength, maxTreeDepth);

                    // Assert
                    Assert.True(tree.Length <= maxLength, 
                        $"{creator.GetType().Name} produced tree with length {tree.Length}, max allowed was {maxLength}");
                    ValidateTreeStructure(tree, grammar, maxLength, maxTreeDepth);
                }
            }
        }

        [Fact]
        public void TreeCreators_CreateValidTreesWithMinimalConstraints()
        {
            // Arrange
            var creators = new ISymbolicExpressionTreeCreator[]
            {
                new GrowTreeCreator(),
                new FullTreeCreator()
            };
            var grammar = new SymbolicRegressionGrammar();
            var random = new MersenneTwister(12345);
            const int minTreeLength = 1;
            const int minTreeDepth = 1;

            foreach (var creator in creators)
            {
                // Act
                var tree = creator.CreateTree(random, grammar, minTreeLength, minTreeDepth);

                // Assert
                Assert.NotNull(tree);
                Assert.NotNull(tree.Root);
                Assert.True(tree.Length >= 1, "Tree should have at least one node");
                Assert.True(tree.Depth >= 1, "Tree should have depth of at least 1");
                
                // When length is 1, tree should be just a terminal
                if (minTreeLength == 1)
                {
                    Assert.Equal(1, tree.Length);
                    Assert.Equal(0, tree.Root.Symbol.MinimumArity);
                    Assert.Equal(0, tree.Root.Symbol.MaximumArity);
                }
            }
        }

        [Fact]
        public void TreeCreators_WithDifferentGrammars_ProduceValidTrees()
        {
            // Arrange
            var creator = new GrowTreeCreator();
            var random = new MersenneTwister(12345);
            var grammars = new ISymbolicExpressionTreeGrammar[]
            {
                new SymbolicRegressionGrammar(),
                new DefaultSymbolicExpressionTreeGrammar()
            };
            const int maxTreeLength = 15;
            const int maxTreeDepth = 4;

            foreach (var grammar in grammars)
            {
                // Act
                var tree = creator.CreateTree(random, grammar, maxTreeLength, maxTreeDepth);

                // Assert
                ValidateTreeStructure(tree, grammar, maxTreeLength, maxTreeDepth);
                
                // All symbols in tree should be from the grammar
                foreach (var node in tree.Root.IterateNodesPrefix())
                {
                    Assert.Contains(node.Symbol, grammar.Symbols);
                }
            }
        }

        [Fact]
        public void TreeCreators_HandleEdgeCases_Gracefully()
        {
            // Arrange
            var creators = new ISymbolicExpressionTreeCreator[]
            {
                new GrowTreeCreator(),
                new FullTreeCreator()
            };
            var grammar = new SymbolicRegressionGrammar();
            var random = new MersenneTwister(12345);

            foreach (var creator in creators)
            {
                // Test with max depth = 1 should create terminal only
                var tree1 = creator.CreateTree(random, grammar, 10, 1);
                Assert.Equal(1, tree1.Length);
                Assert.Equal(1, tree1.Depth);
                Assert.Equal(0, tree1.Root.Symbol.MinimumArity);
                Assert.Equal(0, tree1.Root.Symbol.MaximumArity);

                // Test with max length = 1 should create terminal only  
                var tree2 = creator.CreateTree(random, grammar, 1, 5);
                Assert.Equal(1, tree2.Length);
                Assert.Equal(0, tree2.Root.Symbol.MinimumArity);
                Assert.Equal(0, tree2.Root.Symbol.MaximumArity);
            }
        }

        /// <summary>
        /// Helper method to validate tree structure and constraints
        /// </summary>
        private static void ValidateTreeStructure(ISymbolicExpressionTree tree, ISymbolicExpressionTreeGrammar grammar, 
            int maxLength, int maxDepth)
        {
            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.True(tree.Length <= maxLength, $"Tree length {tree.Length} exceeds max {maxLength}");
            Assert.True(tree.Depth <= maxDepth, $"Tree depth {tree.Depth} exceeds max {maxDepth}");
            Assert.True(tree.Length > 0, "Tree should have at least one node");
            
            // Validate all symbols are from grammar
            foreach (var node in tree.Root.IterateNodesPrefix())
            {
                Assert.Contains(node.Symbol, grammar.Symbols);
                
                // Validate arity constraints
                Assert.True(node.SubtreeCount >= node.Symbol.MinimumArity, 
                    $"Node {node.Symbol.Name} has {node.SubtreeCount} children, minimum is {node.Symbol.MinimumArity}");
                Assert.True(node.SubtreeCount <= node.Symbol.MaximumArity, 
                    $"Node {node.Symbol.Name} has {node.SubtreeCount} children, maximum is {node.Symbol.MaximumArity}");
            }
        }
    }
}
