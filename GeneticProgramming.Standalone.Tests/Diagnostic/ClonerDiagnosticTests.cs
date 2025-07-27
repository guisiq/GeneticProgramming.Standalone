using System;
using System.Collections;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using GeneticProgramming.Core;
using GeneticProgramming.Operators;

namespace GeneticProgramming.Standalone.DiagnosticTests.Core
{
    /// <summary>
    /// Testes diagnósticos para investigar problemas específicos do sistema de clonagem.
    /// Este namespace separado permite isolamento e análise detalhada de comportamentos.
    /// </summary>
    public class ClonerDiagnosticTests
    {
        private readonly ITestOutputHelper output;

        public ClonerDiagnosticTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CloneDiagnostic_SingleObjectClonedTwice_ShouldReturnSameInstance()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new SubtreeCrossover();
            
            output.WriteLine("=== Diagnóstico: Clonagem Dupla do Mesmo Objeto ===");
            output.WriteLine($"Original HashCode: {original.GetHashCode()}");
            output.WriteLine($"Mapa de objetos clonados (inicial): {GetClonedObjectsMapCount(cloner)}");
            
            // Act - Primeira clonagem
            output.WriteLine("\n--- Primeira Clonagem ---");
            var clone1 = original.Clone(cloner);
            output.WriteLine($"Clone1 HashCode: {clone1.GetHashCode()}");
            output.WriteLine($"Mapa de objetos clonados (após 1ª clonagem): {GetClonedObjectsMapCount(cloner)}");
            output.WriteLine($"Original == Clone1? {object.ReferenceEquals(original, clone1)}");
            
            // Verificação intermediária
            bool isRegisteredAfterFirst = cloner.ClonedObjectRegistered(original);
            output.WriteLine($"Original registrado no cloner? {isRegisteredAfterFirst}");
            
            var getCloneAfterFirst = cloner.GetClone(original);
            output.WriteLine($"GetClone retorna? {getCloneAfterFirst?.GetHashCode()}");
            output.WriteLine($"GetClone == Clone1? {object.ReferenceEquals(getCloneAfterFirst, clone1)}");
            
            // Act - Segunda clonagem (deve retornar a mesma instância)
            output.WriteLine("\n--- Segunda Clonagem ---");
            var clone2 = original.Clone(cloner);
            output.WriteLine($"Clone2 HashCode: {clone2.GetHashCode()}");
            output.WriteLine($"Mapa de objetos clonados (após 2ª clonagem): {GetClonedObjectsMapCount(cloner)}");
            
            // Diagnóstico detalhado
            output.WriteLine("\n--- Diagnóstico Final ---");
            output.WriteLine($"Clone1 == Clone2? {object.ReferenceEquals(clone1, clone2)}");
            output.WriteLine($"GetClone == Clone2? {object.ReferenceEquals(getCloneAfterFirst, clone2)}");
            
            // Verificação final
            var getCloneAfterSecond = cloner.GetClone(original);
            output.WriteLine($"GetClone final: {getCloneAfterSecond?.GetHashCode()}");
            
            // Assert
            Assert.True(isRegisteredAfterFirst, "Original deve estar registrado após primeira clonagem");
            Assert.NotNull(getCloneAfterFirst);
            Assert.Same(clone1, clone2); // Esta é a asserção que está falhando
        }

        [Fact]
        public void CloneDiagnostic_ClonerInternalState_InvestigateRegistration()
        {
            // Arrange
            var cloner = new Cloner();
            var original = new SubtreeCrossover();
            
            output.WriteLine("=== Diagnóstico: Estado Interno do Cloner ===");
            
            // Act & Diagnose
            output.WriteLine($"Estado inicial - Mapa: {GetClonedObjectsMapCount(cloner)}");
            
            // Simular o que acontece dentro do Clone
            output.WriteLine("\n--- Simulando processo de clonagem ---");
            
            // Verificar se já está registrado (deve ser false)
            bool alreadyRegistered = cloner.ClonedObjectRegistered(original);
            output.WriteLine($"1. Já registrado inicialmente? {alreadyRegistered}");
            
            // Primeira clonagem
            var clone1 = original.Clone(cloner);
            output.WriteLine($"2. Após primeira clonagem - Mapa: {GetClonedObjectsMapCount(cloner)}");
            output.WriteLine($"3. Registrado após 1ª clonagem? {cloner.ClonedObjectRegistered(original)}");
            
            // Tentar obter o clone
            var retrievedClone = cloner.GetClone(original);
            output.WriteLine($"4. GetClone retorna algo? {retrievedClone != null}");
            output.WriteLine($"5. GetClone == Clone1? {object.ReferenceEquals(retrievedClone, clone1)}");
            
            // Segunda clonagem
            var clone2 = original.Clone(cloner);
            output.WriteLine($"6. Após segunda clonagem - Mapa: {GetClonedObjectsMapCount(cloner)}");
            output.WriteLine($"7. Clone1 == Clone2? {object.ReferenceEquals(clone1, clone2)}");
            
            // Assert
            Assert.True(cloner.ClonedObjectRegistered(original), "Original deve estar registrado");
            Assert.NotNull(retrievedClone);
        }

        /// <summary>
        /// Usa reflexão para acessar o campo privado clonedObjectsMap do Cloner
        /// </summary>
        private static int GetClonedObjectsMapCount(Cloner cloner)
        {
            var field = typeof(Cloner).GetField("clonedObjectsMap", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var dict = field?.GetValue(cloner) as IDictionary;
            return dict?.Count ?? -1;
        }

        /// <summary>
        /// Classe simples para testes de clonagem
        /// </summary>
        private class TestDeepCloneable : IDeepCloneable
        {
            public string Value { get; }

            public TestDeepCloneable(string value)
            {
                Value = value;
            }

            protected TestDeepCloneable(TestDeepCloneable original, Cloner cloner) : this(original.Value)
            {
                cloner.RegisterClonedObject(original, this);
            }

            public IDeepCloneable Clone(Cloner cloner)
            {
                return new TestDeepCloneable(this, cloner);
            }
        }
    }
}
