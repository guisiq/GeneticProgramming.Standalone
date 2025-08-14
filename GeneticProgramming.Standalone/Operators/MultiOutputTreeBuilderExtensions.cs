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

            var functionalSymbols = symbols.Where(s => s is FunctionalSymbol<T>).ToList();
            switch (creationMode)
            {
                case TreeCreationMode.Random:
                    for (int i = 0; i < tree.OutputCount; i++)
                        tree.SetOutputNode(i, CreateRandomTreeNode(symbols, variableNames, treeDepth, treeLength, random));
                    break;
                case TreeCreationMode.SharedBase:
                    var sharedBase = CreateRandomTreeNode(symbols, variableNames, treeDepth - 1, treeLength / 2, random);
                    // busca todos os sybolos funcionais
                    for (int i = 0; i < tree.OutputCount; i++)
                    {
                        var specialized = CreateRandomTreeNode(symbols, variableNames, 2, treeLength / 2, random);
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
                        var node = CreateRandomTreeNode(symbols, variableNames, treeDepth, treeLength, random);
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

        private static ISymbolicExpressionTreeNode<T> CreateRandomTreeNode<T>(
            IEnumerable<ISymbol<T>> symbols,
            string[] variableNames,
            int depth,
            int length,
            Random random) where T : notnull
        {
            if (depth <= 1 || length <= 1)
            {
                // Terminal: variable ou constante
                if (random.NextDouble() < 0.5)
                {
                    var varSymbol = symbols.OfType<Variable<T>>().FirstOrDefault();
                    return new VariableTreeNode<T>(varSymbol ?? throw new InvalidOperationException("No variable symbol"))
                    { VariableName = variableNames[random.Next(variableNames.Length)] };
                }
                else
                {
                    var constSymbol = symbols.OfType<Constant<T>>().FirstOrDefault();
                    return new ConstantTreeNode<T>(constSymbol ?? throw new InvalidOperationException("No constant symbol"), (T)Convert.ChangeType(random.NextDouble(), typeof(T)));
                }
            }
            // Non-terminal: pick a random operator
            var opSymbols = symbols.Where(s => s.MaximumArity > 0).ToList();
            var op = opSymbols[random.Next(opSymbols.Count)];
            var node = op.CreateTreeNode();
            int arity = op.MinimumArity == op.MaximumArity ? op.MinimumArity : random.Next(op.MinimumArity, op.MaximumArity + 1);
            for (int i = 0; i < arity; i++)
            {
                node.AddSubtree(CreateRandomTreeNode(symbols, variableNames, depth - 1, length - 1, random));
            }
            return node;
        }
    }
}
