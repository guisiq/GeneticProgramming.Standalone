# Plano de Execução para Sistema de Programação Genética Multi-Output

Este documento apresenta um plano de implementação em três etapas incrementais, cada uma resultando em um sistema completamente funcional. O objetivo é estender o sistema de Programação Genética atual para suportar tipos genéricos, múltiplas saídas e tipos heterogêneos.

> **Observação importante:** Todos os tipos manipulados pelo sistema GP devem ser `struct` para otimizar a interpretação e evitar problemas de referência nula.

## Etapa 1: Suporte a Tipos Genéricos

**Objetivo:** Refatorar todo o sistema para trabalhar com qualquer tipo `struct`, não apenas `double`.

### Tarefas:

1. **Interfaces Genéricas:**
   - Refatorar `ISymbolicExpressionTree` para `ISymbolicExpressionTree<T> where T : struct`
   - Refatorar `ISymbolicExpressionTreeNode` para `ISymbolicExpressionTreeNode<T> where T : struct`

2. **Nós de Expressão:**
   - Adaptar a classe base `SymbolicExpressionTreeNode` para ser genérica
   - Converter nós funcionais (Add, Multiply, etc.) para implementações genéricas
   - Adicionar verificação de compatibilidade de tipos nos símbolos e operadores:
     - **Símbolos:**
       - Estender `FunctionalSymbol<T>` para armazenar `InputTypes[]` e `OutputType`.
       - Na gramática (`SymbolicExpressionTreeGrammar<T>`), manter índices dos símbolos compatíveis com `T`.
     - **Nós:**
       - Ao criar um nó (`new SymbolicExpressionTreeNode<T>(symbol)`), verificar se `symbol.OutputType == typeof(T)`.
       - Antes de adicionar subárvores (`node.AddSubtree(child)`), checar se `child.OutputType` está entre `symbol.InputTypes`.
       - Lançar `ArgumentException` em caso de incompatibilidade.
     - **Operadores de Mutação:**
       - Adaptar operadores como `SubtreeMutator` para validar tipos antes de substituir subárvores.
       - Usar gramática filtrada para selecionar apenas símbolos compatíveis com os tipos esperados.
     - **Lista de Símbolos:**
       - Métodos como `GetFunctionalSymbols()` devem aceitar tipos de entrada/saída e filtrar símbolos compatíveis.
   - Implementar nós de constante e variável genéricos

3. **Gramática e Interpretação:**
   - Criar `ISymbolicExpressionTreeGrammar<T>` genérica
   - Implementar gramáticas específicas para tipos comuns (int, float, bool)
   - Adaptar o interpretador para trabalhar com o tipo genérico

4. **Operadores Genéticos:**
   - Refatorar `ISymbolicExpressionTreeCreator` para `ISymbolicExpressionTreeCreator<T>`
   - Converter `ISymbolicExpressionTreeCrossover` e `ISymbolicExpressionTreeMutator` para suportar tipos genéricos
   - Adaptar operadores concretos (SubtreeCrossover, SubtreeMutator, etc.)

5. **Algoritmo GP:**
   - Adaptar `GeneticProgrammingAlgorithm` para trabalhar com tipo genérico
   - Refatorar `IFitnessEvaluator` para ser genérico

6. **Validação:**
   - Adicionar testes para diferentes tipos: `double`, `int`, `bool`
   - Verificar compatibilidade com código existente

### Resultados:
- Sistema capaz de evoluir expressões simbólicas para qualquer tipo `struct`
- Mesma API familiar, mas com tipo parametrizado
- Base sólida para etapas futuras

---

## Etapa 2: Suporte a Múltiplas Saídas (Homogêneas)

**Objetivo:** Permitir que uma única árvore de expressão gere múltiplas saídas do mesmo tipo.

### Tarefas:

1. **Interface Multi-Output:**
   - Criar `IMultiSymbolicExpressionTree<T> : ISymbolicExpressionTree<IReadOnlyList<T>>`
   - Adicionar propriedade `IReadOnlyList<ISymbolicExpressionTree<T>> OutputTrees`

2. **Implementação:**
   - Criar `MultiSymbolicExpressionTree<T>` contendo múltiplas árvores de saída
   - Implementar nó raiz multi-output que agrega resultados: `MultiOutputNode<T>`
   - Adaptar métodos `Evaluate`, `Depth`, `Length`, etc.

3. **Operadores Especializados:**
   - Implementar `MultiOutputTreeCreator<T>` para criar árvores com N saídas
   - Criar `MultiOutputCrossover<T>` com estratégias para cruzamento (por árvore ou global)
   - Implementar `MultiOutputMutator<T>` para mutação por árvore

4. **Avaliação de Fitness:**
   - Estender `IFitnessEvaluator<T>` para avaliar múltiplos resultados
   - Implementar estratégias de agregação de fitness (média, mínimo, máximo)

5. **Clone e Serialização:**
   - Garantir que o mecanismo de clonagem funcione corretamente com árvores multi-output
   - Adaptar `ToMathString()` para visualizar múltiplas expressões

6. **Validação:**
   - Testar criação/evolução de árvores multi-output
   - Verificar propagação correta de contexto para todas as árvores

### Resultados:
- Sistema capaz de resolver problemas de regressão multivariada
- Suporte a problemas com múltiplos objetivos do mesmo tipo
- Manutenção da correlação entre saídas através de operadores especializados

---

## Etapa 3: Suporte a Tipos Heterogêneos

**Objetivo:** Permitir múltiplas saídas com tipos diferentes e contexto com variáveis de tipos diversos.

### Tarefas:

1. **Contexto Tipado:**
   - Criar `ITypedContext` com métodos para acessar variáveis de diferentes tipos
   - Implementar `TypedContext` como repositório de variáveis heterogêneas
   - Definir mecanismos de conversão de tipo segura entre tipos compatíveis

2. **Árvores Heterogêneas:**
   - Criar `IHeterogeneousMultiSymbolicExpressionTree` sem parâmetro de tipo genérico
   - Implementar `IOutputTreeDescriptor<T>` para encapsular árvores tipadas
   - Criar `HeterogeneousMultiSymbolicExpressionTree` com método `EvaluateAll()`

3. **Adaptação de Nós:**
   - Implementar nós específicos para tipos diferentes (`BooleanNodes`, `NumericNodes`, etc.)
   - Criar nós de conversão entre tipos compatíveis (`ToIntNode<T>`, `ToDoubleNode<T>`, etc.)
   - Implementar operadores de comparação tipados (`GreaterThan`, `Equals`, etc.)

4. **Operadores Heterogêneos:**
   - Criar `HeterogeneousTreeCreator` que registra criadores para cada tipo
   - Implementar `HeterogeneousCrossover` que preserva os tipos de saída
   - Criar `HeterogeneousMutator` que respeita o tipo de cada árvore

5. **Fitness para Saídas Heterogêneas:**
   - Implementar `HeterogeneousFitnessEvaluator` com avaliadores específicos por tipo
   - Criar estratégias de ponderação e agregação para fitness multi-objetivo

6. **Validação:**
   - Testar com combinações de saídas (`double` + `bool`, `int` + `string`, etc.)
   - Verificar corretude em aplicações práticas (ex: previsão + sinalização)

### Resultados:
- Sistema completo para problemas de GP com múltiplas saídas heterogêneas
- Suporte a contextos com variáveis de diferentes tipos
- Capacidade de resolver problemas complexos como classificação + regressão simultaneamente

---

Este plano apresenta uma evolução incremental do sistema, mantendo compatibilidade com o código existente e adicionando novas capacidades de forma sistemática. Cada etapa constrói sobre a anterior e resulta em um sistema funcional, permitindo validação e uso imediato após a conclusão de cada fase.
