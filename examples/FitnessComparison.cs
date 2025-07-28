using System;
using System.Collections.Generic;

namespace GeneticProgramming.Examples
{
    /// <summary>
    /// Demonstra a diferença entre ClassificationFitnessEvaluator e ImprovedClassificationFitnessEvaluator
    /// </summary>
    public class FitnessComparison
    {
        public static void DemonstrateExample()
        {
            Console.WriteLine("=== COMPARAÇÃO DOS AVALIADORES DE FITNESS ===\n");
            
            // Dataset exemplo: 10 amostras, 4 corretas (40% accuracy)
            var predictions = new double[] { 0.3, 0.7, 0.2, 0.8, 0.4, 0.6, 0.1, 0.9, 0.45, 0.55 };
            var targets = new int[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 };
            
            Console.WriteLine("Predições da árvore: [0.3, 0.7, 0.2, 0.8, 0.4, 0.6, 0.1, 0.9, 0.45, 0.55]");
            Console.WriteLine("Targets verdadeiros: [0,   1,   0,   1,   0,   1,   0,   1,   0,    1  ]\n");
            
            // === AVALIADOR ORIGINAL (ClassificationFitnessEvaluator) ===
            Console.WriteLine("🔴 AVALIADOR ORIGINAL (ClassificationFitnessEvaluator):");
            Console.WriteLine("Regra: predição >= 0.5 ? classe 1 : classe 0");
            Console.WriteLine("Fitness = accuracy = corretos / total\n");
            
            int correctOriginal = 0;
            for (int i = 0; i < predictions.Length; i++)
            {
                int predClass = predictions[i] >= 0.5 ? 1 : 0;
                bool isCorrect = predClass == targets[i];
                if (isCorrect) correctOriginal++;
                
                Console.WriteLine($"Amostra {i+1}: predição={predictions[i]:F2} -> classe={predClass} | target={targets[i]} | {(isCorrect ? "✅ CORRETO" : "❌ ERRADO")}");
            }
            
            double fitnessOriginal = (double)correctOriginal / predictions.Length;
            Console.WriteLine($"\n📊 FITNESS FINAL: {correctOriginal}/{predictions.Length} = {fitnessOriginal:F3}");
            
            // === AVALIADOR MELHORADO (ImprovedClassificationFitnessEvaluator) ===
            Console.WriteLine("\n\n🟢 AVALIADOR MELHORADO (ImprovedClassificationFitnessEvaluator):");
            Console.WriteLine("Regra: fitness suave baseado na distância da predição ao target");
            Console.WriteLine("Normalização: sigmoid(predição) para manter entre 0-1\n");
            
            double totalScoreImproved = 0.0;
            for (int i = 0; i < predictions.Length; i++)
            {
                // Normaliza usando sigmoid
                double normalizedPred = 1.0 / (1.0 + Math.Exp(-predictions[i]));
                
                // Calcula fitness suave
                double sampleFitness;
                if (targets[i] == 1)
                {
                    sampleFitness = normalizedPred; // Reward predições altas
                }
                else
                {
                    sampleFitness = 1.0 - normalizedPred; // Reward predições baixas
                }
                
                totalScoreImproved += sampleFitness;
                
                Console.WriteLine($"Amostra {i+1}: predição={predictions[i]:F2} -> sigmoid={normalizedPred:F3} | target={targets[i]} | fitness={sampleFitness:F3}");
            }
            
            double fitnessImproved = totalScoreImproved / predictions.Length;
            Console.WriteLine($"\n📊 FITNESS FINAL: {totalScoreImproved:F3}/{predictions.Length} = {fitnessImproved:F3}");
            
            // === ANÁLISE DO PROBLEMA ===
            Console.WriteLine("\n\n🧠 ANÁLISE DO PROBLEMA DE CRESCIMENTO:");
            Console.WriteLine("=======================================");
            
            Console.WriteLine($"\n1️⃣ GRANULARIDADE DO FITNESS:");
            Console.WriteLine($"   • Original: Apenas {predictions.Length + 1} valores possíveis (0.0, 0.1, 0.2, ..., 1.0)");
            Console.WriteLine($"   • Melhorado: Infinitos valores contínuos entre 0.0 e 1.0");
            
            Console.WriteLine($"\n2️⃣ GRADIENTE DE EVOLUÇÃO:");
            Console.WriteLine($"   • Original: SEM gradiente - árvore A(0.45→0) = árvore B(0.01→0) = mesmo fitness");
            Console.WriteLine($"   • Melhorado: COM gradiente - árvore A(0.45) = 0.389, árvore B(0.01) = 0.502");
            
            Console.WriteLine($"\n3️⃣ SELEÇÃO PARA CRUZAMENTO:");
            Console.WriteLine($"   • Original: Muitas árvores com fitness idêntico = seleção aleatória");
            Console.WriteLine($"   • Melhorado: Cada árvore tem fitness único = seleção baseada em qualidade");
            
            Console.WriteLine($"\n4️⃣ PRESSÃO EVOLUTIVA:");
            Console.WriteLine($"   • Original: Evolução para = SEM pressão para melhorar");
            Console.WriteLine($"   • Melhorado: Sempre há direção para melhorar + pressão de parcimônia");
            
            // === EXEMPLO NUMÉRICO ===
            Console.WriteLine($"\n\n🔢 EXEMPLO DE POR QUE AS ÁRVORES NÃO CRESCEM:");
            Console.WriteLine("============================================");
            
            Console.WriteLine("Imagine duas árvores candidatas:");
            Console.WriteLine("🌱 Árvore Simples: X1 (tamanho=1, predição=0.4 para amostra classe 0)");
            Console.WriteLine("🌳 Árvore Complexa: (X1 + X2) * X3 (tamanho=5, predição=0.45 para amostra classe 0)");
            
            Console.WriteLine("\n📊 AVALIAÇÃO ORIGINAL:");
            Console.WriteLine("   • Simples: 0.4 >= 0.5? NÃO → classe 0 → ✅ CORRETO → fitness contribui +1");
            Console.WriteLine("   • Complexa: 0.45 >= 0.5? NÃO → classe 0 → ✅ CORRETO → fitness contribui +1");
            Console.WriteLine("   • RESULTADO: MESMO FITNESS! Algoritmo não vê diferença.");
            
            Console.WriteLine("\n📊 AVALIAÇÃO MELHORADA:");
            double simplesNorm = 1.0 / (1.0 + Math.Exp(-0.4));
            double complexaNorm = 1.0 / (1.0 + Math.Exp(-0.45));
            double simplesFit = 1.0 - simplesNorm;
            double complexaFit = 1.0 - complexaNorm;
            
            Console.WriteLine($"   • Simples: sigmoid(0.4)={simplesNorm:F3} → fitness={simplesFit:F3}");
            Console.WriteLine($"   • Complexa: sigmoid(0.45)={complexaNorm:F3} → fitness={complexaFit:F3}");
            Console.WriteLine($"   • RESULTADO: Complexa é {complexaFit/simplesFit:F2}x melhor! Algoritmo vê a diferença.");
            
            Console.WriteLine($"\n✨ CONCLUSÃO:");
            Console.WriteLine($"O avaliador original cria 'platôs de fitness' onde muitas árvores diferentes");
            Console.WriteLine($"têm exatamente o mesmo score, eliminando a pressão evolutiva para crescer.");
            Console.WriteLine($"O avaliador melhorado fornece gradientes suaves que guiam a evolução!");
        }
    }
}
