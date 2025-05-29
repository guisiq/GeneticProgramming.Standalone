# Genetic Programming Standalone - Fase de Testes de Integração e Unitários

## 🎯 Objetivo da Etapa
Garantir a corretude e robustez da biblioteca de Programação Genética através da criação e execução de testes de integração e unitários abrangentes. Validar a interação entre os diferentes componentes e o comportamento individual de unidades de código críticas.

## 📝 Plano de Testes e Fluxos de Integração

### Fluxo 1: Criação, Parametrização e Clonagem de Operadores
- **Objetivo:** Verificar a correta instanciação de todos os tipos de operadores genéticos, a capacidade de acessar e modificar seus parâmetros (herdada via `IOperator`), e a funcionalidade de clonagem.
- **Testes de Integração:**
    - `OperatorCreation_CanCreateAllOperatorTypes`: Tenta instanciar cada classe concreta de operador.
    - `OperatorParameters_CanGetAndSetParameters`: Para cada tipo de operador, obtém a coleção de parâmetros, tenta modificar um parâmetro (se aplicável e se houver parâmetros expostos além dos de `Item`) e verifica a alteração.
    - `OperatorCloning_ClonesParametersCorrectly`: Para cada tipo de operador, clona o operador, modifica um parâmetro no original e no clone, e verifica se as alterações são independentes e se os valores iniciais foram clonados corretamente.
- **Testes Unitários Relevantes:**
    - Testes para `ParameterCollection`: Adicionar, remover, obter parâmetros.
    - Testes para a classe `Cloner` (se ainda não existirem ou não forem suficientes) focando na clonagem de `ParameterCollection` e tipos de dados básicos dos parâmetros.
    - Testes unitários para a lógica de `OnPropertyChanged` na classe `Item` e `Parameter`.

### Fluxo 2: Funcionalidade Básica dos Operadores Genéticos
- **Objetivo:** Validar que os operadores genéticos (criadores, cruzamento, mutação) funcionam conforme o esperado, produzindo resultados válidos e consistentes com as regras da programação genética e as gramáticas fornecidas.
- **Subfluxo 2.1: Criadores de Árvores (`FullTreeCreator`, `GrowTreeCreator`)**
    - **Testes de Integração:**
        - `TreeCreators_ProduceValidTrees`: Verifica se árvores criadas são válidas de acordo com uma gramática simples e respeitam limites de profundidade/tamanho.
        - `TreeCreators_RespectMaxDepthAndLength`: Testa especificamente se os criadores não excedem a profundidade e o tamanho máximos definidos.
    - **Testes Unitários Relevantes:**
        - Testes para a lógica de seleção de símbolos na gramática.
        - Testes para a lógica de construção de nós (terminais e não terminais) nos criadores.
- **Subfluxo 2.2: Operadores de Cruzamento (`SubtreeCrossover`, `OnePointCrossover`)**
    - **Testes de Integração:**
        - `Crossover_ProducesValidOffspring`: Verifica se os filhos gerados são árvores válidas.
        - `Crossover_OffspringAreDifferentFromParents`: Verifica se o cruzamento geralmente produz descendentes diferentes dos pais.
        - `Crossover_HandlesEdgeCases`: (e.g., árvores muito pequenas, árvores idênticas).
    - **Testes Unitários Relevantes:**
        - Testes para a lógica de seleção de pontos de cruzamento.
        - Testes para a lógica de troca de subárvores/segmentos.
- **Subfluxo 2.3: Operadores de Mutação (`SubtreeMutator`, `ChangeNodeTypeMutator`)**
    - **Testes de Integração:**
        - `Mutation_ProducesValidMutatedTrees`: Verifica se a árvore mutada é válida.
        - `Mutation_MutatedTreeIsDifferentFromOriginal`: Verifica se a mutação geralmente altera a árvore.
        - `Mutation_HandlesEdgeCases`: (e.g., tentar mudar tipo de nó raiz para terminal incompatível).
    - **Testes Unitários Relevantes:**
        - Testes para a lógica de seleção de nós para mutação.
        - Testes para a lógica de geração de novas subárvores (para `SubtreeMutator`).
        - Testes para a lógica de substituição de tipos de nós (para `ChangeNodeTypeMutator`), incluindo a verificação de aridade.

### Fluxo 3: Gramática e Símbolos (`SymbolicExpressionTreeGrammar`, `Symbol`)
- **Objetivo:** Testar a funcionalidade das gramáticas, incluindo adição/remoção de símbolos, recuperação de símbolos permitidos e validação de árvores.
- **Testes de Integração:**
    - `Grammar_AddAndRemoveSymbols`: Verifica se símbolos podem ser adicionados e removidos da gramática.
    - `Grammar_GetAllowedSymbols`: Verifica se a gramática retorna os símbolos corretos que podem seguir um determinado nó/símbolo.
    - `Grammar_ValidateTree`: Testa a funcionalidade de validação de árvores (positivos e negativos).
- **Testes Unitários Relevantes:**
    - Testes para a classe `Symbol`: Propriedades como `Arity`, `SymbolName`.
    - Testes para a lógica interna de `SymbolicExpressionTreeGrammar` para gerenciar coleções de símbolos e regras.

### Fluxo 4: Algoritmo de Programação Genética (Estrutura Básica) (`GeneticProgrammingAlgorithm`)
- **Objetivo:** Validar a estrutura básica e o fluxo de execução do `GeneticProgrammingAlgorithm`.
- **Testes de Integração:**
    - `GPAlgorithm_Initialization`: Verifica se o algoritmo pode ser inicializado com operadores e gramática.
    - `GPAlgorithm_RunShortEvolution`: Executa o algoritmo por um pequeno número de gerações e verifica se não ocorrem exceções e se os eventos de geração são disparados.
    - `GPAlgorithm_PopulationManagement`: Verifica se a população é inicializada e se seu tamanho é mantido (aproximadamente) ao longo das gerações.
- **Testes Unitários Relevantes:**
    - Testes para a lógica de seleção de indivíduos (se implementada separadamente).
    - Testes para a lógica de avaliação (usando uma função de fitness mock).
    - Testes para o disparo de `GenerationEventArgs`.

## 📈 Status da Implementação dos Testes

### Fluxo 1: Criação, Parametrização e Clonagem de Operadores
- **Testes Unitários:**
    - [x] `ParameterCollectionTests` ✅ **CONCLUÍDO** - Corrigidos erros de compilação e 8 testes passando
    - [ ] `ClonerTests` (foco em `ParameterCollection`)
    - [ ] `ItemTests` (foco em `OnPropertyChanged` para `Parameters`)
    - [ ] `ParameterTests` (foco em `OnPropertyChanged`)
- **Testes de Integração:**
    - [ ] `OperatorCreationTests`
    - [ ] `OperatorParametersTests`
    - [ ] `OperatorCloningTests`

### Fluxo 2: Funcionalidade Básica dos Operadores Genéticos
- **Subfluxo 2.1: Criadores de Árvores**
    - **Testes Unitários:**
        - [ ] `GrammarSymbolSelectionTests`
        - [ ] `TreeCreatorNodeBuildingTests`
    - **Testes de Integração:**
        - [ ] `TreeCreatorTests`
- **Subfluxo 2.2: Operadores de Cruzamento**
    - **Testes Unitários:**
        - [ ] `CrossoverPointSelectionTests`
        - [ ] `SubtreeExchangeTests`
    - **Testes de Integração:**
        - [ ] `CrossoverOperatorTests`
- **Subfluxo 2.3: Operadores de Mutação**
    - **Testes Unitários:**
        - [ ] `MutationNodeSelectionTests`
        - [ ] `SubtreeGenerationTests` (para `SubtreeMutator`)
        - [ ] `NodeTypeChangeTests` (para `ChangeNodeTypeMutator`)
    - **Testes de Integração:**
        - [ ] `MutationOperatorTests`

### Fluxo 3: Gramática e Símbolos
- **Testes Unitários:**
    - [ ] `SymbolTests`
    - [ ] `SymbolicExpressionTreeGrammarInternalLogicTests`
- **Testes de Integração:**
    - [ ] `GrammarTests`

### Fluxo 4: Algoritmo de Programação Genética
- **Testes Unitários:**
    - [ ] `SelectionLogicTests` (se aplicável)
    - [ ] `EvaluationLogicTests` (com mock fitness)
    - [ ] `GenerationEventArgsTests`
- **Testes de Integração:**
    - [ ] `GeneticProgrammingAlgorithmTests`

## 🧪 Testes de Diagnóstico
- (A serem adicionados conforme necessário durante a implementação dos testes de integração e unitários)

## 📅 Histórico de Alterações

### 29/05/2025 - Correção de Erros de Compilação nos Testes
**Status:** ✅ **CONCLUÍDO**

#### Problemas Identificados e Corrigidos:
1. **ParameterCollectionTests.cs:**
   - ❌ **Problema:** Construtor `Parameter` estava sendo chamado com 3 argumentos, mas a API real aceita apenas 2
   - ✅ **Solução:** Atualizado para `new Parameter("name", "description")`
   
   - ❌ **Problema:** Testes usando métodos inexistentes (`GetParameter()`, `Clear()`, `Contains()`, `Remove()`)
   - ✅ **Solução:** Reescrito testes para usar a API real: `Add()`, `Get()`, `Remove()` e `GetEnumerator()`
   
   - ❌ **Problema:** Falta de validação de parâmetros nulos
   - ✅ **Solução:** Adicionado teste `Add_ThrowsArgumentNullException_WhenParameterIsNull()`

2. **ParameterCollection.cs:**
   - ❌ **Problema:** Método `Add()` não validava parâmetros nulos
   - ✅ **Solução:** Adicionado `ArgumentNullException.ThrowIfNull(parameter)`
   
   - ❌ **Problema:** Faltava diretiva `using System;`
   - ✅ **Solução:** Adicionado `using System;` no cabeçalho

#### Resultados:
- ✅ **Build:** Sucesso com 0 erros de compilação
- ✅ **Testes:** 8 testes passando (100% de sucesso)
- ⚠️ **Avisos:** 13 avisos de nullability (não críticos)

#### Arquivos Modificados:
- `/tests/GeneticProgramming.Standalone.Tests/ParameterCollectionTests.cs`
- `/src/Abstractions/Parameters/ParameterCollection.cs`

**Data de Início da Etapa:** 29/05/2025
