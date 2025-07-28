using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticProgramming.Expressions
{
    /// <summary>
    /// Classe utilitária para visualização avançada de árvores de expressão simbólica
    /// </summary>
    public static class TreeVisualization
    {
        /// <summary>
        /// Exibe a árvore em formato de caixa ASCII com bordas decoradas
        /// </summary>
        /// <param name="tree">A árvore a ser visualizada</param>
        /// <returns>String com a representação visual da árvore</returns>
        public static string ToBoxedTreeString(ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "┌─────────────┐\n│ Empty Tree  │\n└─────────────┘";

            var result = new StringBuilder();
            result.AppendLine("┌─────────────────────────────────────────┐");
            result.AppendLine("│          Árvore de Expressão            │");
            result.AppendLine("├─────────────────────────────────────────┤");
            
            var treeString = tree.ToTreeString();
            var treeLines = treeString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in treeLines)
            {
                var paddedLine = line.Length > 39 ? line.Substring(0, 39) : line.PadRight(39);
                result.AppendLine($"│ {paddedLine} │");
            }
            
            result.AppendLine("├─────────────────────────────────────────┤");
            result.AppendLine($"│ Expressão: {tree.ToMathString().PadRight(28)} │");
            result.AppendLine($"│ Comprimento: {tree.Length.ToString().PadRight(26)} │");
            result.AppendLine($"│ Profundidade: {tree.Depth.ToString().PadRight(25)} │");
            result.AppendLine("└─────────────────────────────────────────┘");
            
            return result.ToString();
        }

        /// <summary>
        /// Gera uma representação da árvore em formato de grafo com coordenadas
        /// </summary>
        /// <param name="tree">A árvore a ser visualizada</param>
        /// <returns>String com informações de posicionamento dos nós</returns>
        public static string ToGraphString(ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "Árvore vazia";

            var result = new StringBuilder();
            result.AppendLine("Representação em Grafo:");
            result.AppendLine("═══════════════════════");
            
            var nodePositions = new Dictionary<ISymbolicExpressionTreeNode, (int level, int position)>();
            CalculateNodePositions(tree.Root, 0, 0, nodePositions);
            
            var maxLevel = nodePositions.Values.Max(p => p.level);
            
            for (int level = 0; level <= maxLevel; level++)
            {
                var nodesAtLevel = nodePositions
                    .Where(kvp => kvp.Value.level == level)
                    .OrderBy(kvp => kvp.Value.position)
                    .ToList();
                
                result.AppendLine($"Nível {level}:");
                foreach (var node in nodesAtLevel)
                {
                    result.AppendLine($"  [{node.Value.position}] {node.Key.Symbol.SymbolName}");
                }
                result.AppendLine();
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Calcula as posições dos nós na árvore para visualização em grafo
        /// </summary>
        private static int CalculateNodePositions(ISymbolicExpressionTreeNode node, int level, int position, 
            Dictionary<ISymbolicExpressionTreeNode, (int level, int position)> positions)
        {
            positions[node] = (level, position);
            
            int currentPos = position;
            foreach (var child in node.Subtrees)
            {
                currentPos = CalculateNodePositions(child, level + 1, currentPos, positions);
                currentPos++;
            }
            
            return currentPos;
        }

        /// <summary>
        /// Exibe estatísticas detalhadas da árvore
        /// </summary>
        /// <param name="tree">A árvore a ser analisada</param>
        /// <returns>String com estatísticas detalhadas</returns>
        public static string GetDetailedStatistics(ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "Árvore vazia - sem estatísticas disponíveis";

            var result = new StringBuilder();
            result.AppendLine("Estatísticas Detalhadas da Árvore:");
            result.AppendLine("═══════════════════════════════════");
            
            var allNodes = tree.IterateNodesPrefix().ToList();
            var terminalNodes = allNodes.Where(n => n.SubtreeCount == 0).ToList();
            var functionalNodes = allNodes.Where(n => n.SubtreeCount > 0).ToList();
            
            result.AppendLine($"Total de nós: {allNodes.Count}");
            result.AppendLine($"Nós terminais: {terminalNodes.Count}");
            result.AppendLine($"Nós funcionais: {functionalNodes.Count}");
            result.AppendLine($"Profundidade: {tree.Depth}");
            result.AppendLine($"Comprimento: {tree.Length}");
            result.AppendLine();
            
            // Contagem por tipo de símbolo
            var symbolCounts = allNodes
                .GroupBy(n => n.Symbol.SymbolName)
                .OrderByDescending(g => g.Count())
                .ToList();
            
            result.AppendLine("Distribuição de Símbolos:");
            foreach (var group in symbolCounts)
            {
                result.AppendLine($"  {group.Key}: {group.Count()} ocorrência(s)");
            }
            result.AppendLine();
            
            // Análise da profundidade por nível
            var nodesByDepth = new Dictionary<int, int>();
            foreach (var node in allNodes)
            {
                int depth = GetNodeDepth(node, tree.Root);
                nodesByDepth[depth] = nodesByDepth.GetValueOrDefault(depth, 0) + 1;
            }
            
            result.AppendLine("Distribuição por Profundidade:");
            for (int i = 0; i <= nodesByDepth.Keys.Max(); i++)
            {
                if (nodesByDepth.ContainsKey(i))
                {
                    result.AppendLine($"  Nível {i}: {nodesByDepth[i]} nó(s)");
                }
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Calcula a profundidade de um nó específico na árvore
        /// </summary>
        private static int GetNodeDepth(ISymbolicExpressionTreeNode targetNode, ISymbolicExpressionTreeNode root)
        {
            if (targetNode == root)
                return 0;

            foreach (var child in root.Subtrees)
            {
                int childDepth = GetNodeDepth(targetNode, child);
                if (childDepth >= 0)
                    return childDepth + 1;
            }

            return -1; // Nó não encontrado
        }

        /// <summary>
        /// Gera uma representação compacta da árvore em uma única linha
        /// </summary>
        /// <param name="tree">A árvore a ser representada</param>
        /// <returns>Representação compacta da árvore</returns>
        public static string ToCompactString(ISymbolicExpressionTree tree)
        {
            if (tree.Root == null)
                return "()";

            return ToCompactStringRecursive(tree.Root);
        }

        /// <summary>
        /// Método recursivo para gerar representação compacta
        /// </summary>
        private static string ToCompactStringRecursive(ISymbolicExpressionTreeNode node)
        {
            if (node.SubtreeCount == 0)
                return node.Symbol.SymbolName;

            var children = string.Join(" ", node.Subtrees.Select(ToCompactStringRecursive));
            return $"({node.Symbol.SymbolName} {children})";
        }

        /// <summary>
        /// Imprime uma visualização completa da árvore no console
        /// </summary>
        /// <param name="tree">A árvore a ser visualizada</param>
        public static void PrintCompleteVisualization(ISymbolicExpressionTree tree)
        {
            Console.WriteLine(ToBoxedTreeString(tree));
            Console.WriteLine();
            Console.WriteLine(GetDetailedStatistics(tree));
            Console.WriteLine();
            Console.WriteLine($"Representação Compacta: {ToCompactString(tree)}");
            Console.WriteLine();
            Console.WriteLine(ToGraphString(tree));
        }
    }
}
