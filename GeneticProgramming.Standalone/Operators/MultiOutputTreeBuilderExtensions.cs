using System;
using System.Collections.Generic;
using System.Linq;
using GeneticProgramming.Expressions.Symbols;
using GeneticProgramming.Standalone.Core;
using GeneticProgramming.Standalone.Expressions;

namespace GeneticProgramming.Operators
{
    public static class MultiSymbolicExpressionTreeExtensions
    {
        /// <summary>
        /// Preenche um MultiSymbolicExpressionTree com subárvores aleatórias para múltiplas saídas, usando as configurações fornecidas.
        /// Usa o Random do próprio MultiSymbolicExpressionTree (via Cloner ou campo interno, se disponível).
        /// </summary>
        public static void BuildMultiOutputRandom<T>(
            this MultiSymbolicExpressionTree<T> tree,
            IEnumerable<ISymbol<T>> symbols,
            string[] variableNames,
            int treeDepth = 5,
            int treeLength = 10,
            TreeCreationMode creationMode = TreeCreationMode.Random,
            MultiOutputStrategy strategy = MultiOutputStrategy.Shared)
            where T : notnull
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            if (symbols == null || !symbols.Any()) throw new ArgumentException("At least one symbol must be provided");
            if (variableNames == null || variableNames.Length == 0) throw new ArgumentException("At least one variable name must be provided");

            // Tenta obter o Random do Cloner, se possível
            var clonerField = tree.GetType().GetField("_cloner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Random random;
            random = null!;
            if (clonerField != null)
            {
                var cloner = clonerField.GetValue(tree);
                if (cloner != null)
                {
                    var randProp = cloner.GetType().GetProperty("Random", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (randProp != null)
                    {
                        var randObj = randProp.GetValue(cloner);
                        if (randObj is Random r)
                            random = r;
                    }
                }
            }
            if (random == null)
                random = new Random();

            // Cache symbol lists to avoid repeated LINQ queries
            var symbolList = symbols.ToList();
            var functionalSymbols = symbolList.Where(s => s is FunctionalSymbol<T>).ToList();
            var terminalSymbols = symbolList.Where(s => s.MaximumArity == 0).ToList();
            var variableSymbols = terminalSymbols.OfType<Variable<T>>().ToList();
            var constantSymbols = terminalSymbols.OfType<Constant<T>>().ToList();
            var operatorSymbols = symbolList.Where(s => s.MaximumArity > 0).ToList();

            // Helper to pass cached lists
            ISymbolicExpressionTreeNode<T> CreateNode(int depth, int length) => 
                CreateRandomTreeNodeOptimized(variableSymbols, constantSymbols, operatorSymbols, variableNames, depth, length, random);

            switch (creationMode)
            {
                case TreeCreationMode.Random:
                    for (int i = 0; i < tree.OutputCount; i++)
                        tree.SetOutputNode(i, CreateNode(treeDepth, treeLength));
                    break;
                case TreeCreationMode.SharedBase:
                    var sharedBase = CreateNode(treeDepth - 1, treeLength / 2);
                    // busca todos os sybolos funcionais
                    for (int i = 0; i < tree.OutputCount; i++)
                    {
                        var specialized = CreateNode(2, treeLength / 2);
                        // Cria um CompositeSymbol com aridade 2 e um builder trivial (apenas conecta os filhos)
                        // pega uma funcao aleatoria
                        var randomFunctionalSymbol = functionalSymbols[random.Next(functionalSymbols.Count)];
                        var compositeSymbol = GeneticProgramming.Expressions.SymbolFactory<T>.CreateComposite(
                            "Composite", "Composite symbol for multi-output trees", 2,
                            placeholders =>
                            {
                                var node = randomFunctionalSymbol.CreateTreeNode(); // Placeholder, será substituído
                                node.AddSubtree(placeholders[0]);
                                node.AddSubtree(placeholders[1]);
                                return node;
                            });
                        var root = compositeSymbol.CreateTreeNode();
                        root.AddSubtree(sharedBase);
                        root.AddSubtree(specialized);
                        tree.SetOutputNode(i, root);
                    }
                    break;
                case TreeCreationMode.Hierarchical:
                    ISymbolicExpressionTreeNode<T> prev = default!;
                    for (int i = 0; i < tree.OutputCount; i++)
                    {
                        var node = CreateNode(treeDepth, treeLength);
                        if (prev != null)
                        {
                            var randomFunctionalSymbol = functionalSymbols[random.Next(functionalSymbols.Count)];
                            var compositeSymbol = GeneticProgramming.Expressions.SymbolFactory<T>.CreateComposite(
                                "Composite", "Composite symbol for multi-output trees", 2,
                                placeholders => {
                                    var n = randomFunctionalSymbol.CreateTreeNode(); // Placeholder, será substituído
                                    n.AddSubtree(placeholders[0]);
                                    n.AddSubtree(placeholders[1]);
                                    return n;
                                });
                            var composite = compositeSymbol.CreateTreeNode();
                            composite.AddSubtree(prev);
                            composite.AddSubtree(node);
                            tree.SetOutputNode(i, composite);
                            prev = composite;
                        }
                        else
                        {
                            tree.SetOutputNode(i, node);
                            prev = node;
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException($"Creation mode {creationMode} not supported");
            }
        }

        private static ISymbolicExpressionTreeNode<T> CreateRandomTreeNodeOptimized<T>(
            List<Variable<T>> variableSymbols,
            List<Constant<T>> constantSymbols,
            List<ISymbol<T>> operatorSymbols,
            string[] variableNames,
            int depth,
            int length,
            Random random) where T : notnull
        {
            if (depth <= 1 || length <= 1)
            {
                // Terminal: variable ou constante
                // Check if we have variables or constants available
                bool hasVariables = variableSymbols.Count > 0;
                bool hasConstants = constantSymbols.Count > 0;
                
                if (!hasVariables && !hasConstants)
                    throw new InvalidOperationException("No terminal symbols available");

                bool pickVariable = hasVariables && (!hasConstants || random.NextDouble() < 0.5);

                if (pickVariable)
                {
                    var varSymbol = variableSymbols[random.Next(variableSymbols.Count)];
                    return new VariableTreeNode<T>(varSymbol)
                    { VariableName = variableNames[random.Next(variableNames.Length)] };
                }
                else
                {
                    var constSymbol = constantSymbols[random.Next(constantSymbols.Count)];
                    return new ConstantTreeNode<T>(constSymbol, (T)Convert.ChangeType(random.NextDouble(), typeof(T)));
                }
            }
            
            // Non-terminal: pick a random operator
            if (operatorSymbols.Count == 0)
                 throw new InvalidOperationException("No operator symbols available");
                 
            var op = operatorSymbols[random.Next(operatorSymbols.Count)];
            var node = op.CreateTreeNode();
            int arity = op.MinimumArity == op.MaximumArity ? op.MinimumArity : random.Next(op.MinimumArity, op.MaximumArity + 1);
            for (int i = 0; i < arity; i++)
            {
                node.AddSubtree(CreateRandomTreeNodeOptimized(variableSymbols, constantSymbols, operatorSymbols, variableNames, depth - 1, length - 1, random));
            }
            return node;
        }

        private static ISymbolicExpressionTreeNode<T> CreateRandomTreeNode<T>(
            IEnumerable<ISymbol<T>> symbols,
            string[] variableNames,
            int depth,
            int length,
            Random random) where T : notnull
        {
            // Fallback for backward compatibility or if called directly (though private)
            // Re-implement using optimized version by filtering symbols here
            var symbolList = symbols.ToList();
            var terminalSymbols = symbolList.Where(s => s.MaximumArity == 0).ToList();
            return CreateRandomTreeNodeOptimized(
                terminalSymbols.OfType<Variable<T>>().ToList(),
                terminalSymbols.OfType<Constant<T>>().ToList(),
                symbolList.Where(s => s.MaximumArity > 0).ToList(),
                variableNames, depth, length, random);
        }
    }
}
