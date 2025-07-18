# Genetic Programming Standalone - Projeto Independente

## 📋 Visão Geral
Este projeto é uma extração independente dos componentes de Programação Genética do HeuristicLab, criado para ser uma biblioteca standalone funcional.


## 🎯 STATUS ATUAL: ✅ Validação inicial concluída
**MARCO ALCANÇADO!** O projeto compila sem erros e os testes automatizados estão passando, demonstrando que os principais componentes funcionam conforme esperado.

### ✅ CONCLUÍDO RECENTEMENTE
- [x] **SymbolicExpressionTreeNode implementado** - Componente crítico finalizado
- [x] **Operações de árvore funcionais** - Adicionar, remover, substituir subárvores
- [x] **Sistema de iteração completo** - Breadth-first, prefix, postfix
- [x] **Gerenciamento de cache** - Otimização de profundidade e comprimento
- [x] **Relacionamentos pai-filho** - Manutenção automática de referências
- [x] **Compilação bem-sucedida** - Projeto compila sem erros
- [x] **Padronização da clonagem profunda com `CreateCloneInstance`** - Implementado o método `CreateCloneInstance` em classes chave (`SymbolicExpressionTreeGrammar`, `GeneticProgrammingAlgorithm`, `MersenneTwister`, `IntValue`, `DoubleValue`, `BoolValue`, `StringValue`, `ConcreteSymbolicExpressionTreeOperator`, `Variable`, `Constant`) para um mecanismo de clonagem mais robusto e consistente. O método `CreateCloneInstance` foi tornado abstrato em `SymbolicExpressionTreeOperator`.

### 🔄 PRÓXIMO OBJETIVO: Expandir cobertura de testes e exemplos
Com os símbolos e operadores básicos implementados, o foco passa a ser aumentar a cobertura de testes e disponibilizar exemplos de uso mais completos.

---

## 🏗️ Estrutura do Projeto

```
GeneticProgramming.Standalone/
├── src/
│   ├── Core/              # ✅ Classes base e interfaces fundamentais
│   ├── Expressions/       # 🔄 Árvores de expressão simbólica 
│   ├── Operators/         # 📋 Operadores genéticos (crossover, mutation)
│   ├── Algorithms/        # 📋 Algoritmos GP principais
│   ├── Problems/          # 📋 Problemas exemplo
│   └── Abstractions/      # 📋 Abstrações para dependências externas
├── tests/                 # 📋 Testes unitários
├── examples/              # 📋 Exemplos de uso
└── docs/                  # 📋 Documentação
```

---

## 🧩 Componentes Principais

### ✅ Core (Concluído)
- **IItem**: Interface base para todos os objetos do framework
- **Item**: Implementação base com suporte a clonagem profunda
- **Cloner**: Responsável por operações de clonagem profunda
- **DataTypes**: Wrappers para tipos básicos (IntValue, DoubleValue, BoolValue, StringValue)
- **Random**: Gerador de números aleatórios (MersenneTwister)

### 🔄 Expressions (Funcional)
- **ISymbolicExpressionTree**: ✅ Interface para árvores de expressão
- **SymbolicExpressionTree**: ✅ Implementação de árvore de expressão simbólica
- **ISymbolicExpressionTreeNode**: ✅ Interface para nós da árvore
- **SymbolicExpressionTreeNode**: ✅ **IMPLEMENTADO** - Nós funcionais com operações completas
- **ISymbol**: ✅ **IMPLEMENTADO**
- **ISymbolicExpressionTreeGrammar**: ✅ **IMPLEMENTADO**

### 🔄 Operators (Parcialmente Implementado)
- **Creators**: ✅ `GrowTreeCreator`, `FullTreeCreator`
- **Crossovers**: ✅ `SubtreeCrossover`, `OnePointCrossover`, `UniformCrossover`
- **Manipulators**: ✅ `SubtreeMutator`, `ChangeNodeTypeMutator`
- **Architecture Manipulators**: ✅ `NodeInsertionManipulator`, `NodeRemovalManipulator`

---

## ✅ Progresso Atual - Validação

### ✅ CONCLUÍDO
- [x] Estrutura base do projeto (Pastas, .csproj)
- [x] Interfaces fundamentais (IItem, IDeepCloneable)
- [x] Classe base Item (Com PropertyChanged, clonagem)
- [x] Sistema de clonagem (Cloner com mapeamento)
- [x] Tipos de dados básicos (IntValue, DoubleValue, BoolValue, StringValue)
- [x] Interface para árvores de expressão (ISymbolicExpressionTree)
- [x] Implementação SymbolicExpressionTree (Com iteradores)
- [x] Interface para nós da árvore (ISymbolicExpressionTreeNode)
- [x] Interface gerador de números aleatórios (IRandom)
- [x] Implementação MersenneTwister (Gerador de números aleatórios)
- [x] SymbolicExpressionTreeNode completo
- [x] Implementação de símbolos e gramáticas
- [x] Operadores genéticos básicos

### 🔄 EM PROGRESSO
- [x] Criar abstrações necessárias

### 📋 PRÓXIMO PASSO
- [ ] Implementar exemplos
- [ ] Testes básicos

---

## 🎯 PRÓXIMO PASSO - Implementação SymbolicExpressionTreeNode

### O que precisa ser feito:
1. **Implementar SymbolicExpressionTreeNode.cs** baseado no HeuristicLab
2. **Criar sistema de símbolos básicos** (Add, Mul, Variable, Constant)
3. **Implementar gramática básica**
4. **Fazer commit do progresso**
5. **Testar compilação**

### Componentes identificados no HeuristicLab para extração:
- `HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionTreeNode`
- `HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols.*`
- `HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Grammars.*`

---

## 📋 Roadmap Completo

### Fase 1: Core Fundamentals ✅
- [x] Estrutura do projeto
- [x] Interfaces base
- [x] Sistema de clonagem
- [x] Tipos de dados

### Fase 2: Expressões Simbólicas 🔄
- [x] Interfaces de árvores
- [x] SymbolicExpressionTree
- [x] SymbolicExpressionTreeNode
- [x] Sistema de símbolos
- [x] Gramáticas

### Fase 3: Operadores Genéticos 📋
- [x] Criadores de população
- [x] Operadores de crossover
- [x] Operadores de mutação
- [ ] Seletores

### Fase 4: Algoritmos e Problemas 📋
- [x] Algoritmo GP básico
- [ ] Problemas exemplo
- [ ] Avaliadores de fitness

### Fase 5: Testes e Exemplos 📋
- [x] Testes unitários e de integração
- [ ] Exemplos funcionais adicionais
- [ ] Documentação

---

## 🔧 Dependências Mapeadas

### Core Dependencies (Implementadas como abstrações):
- ✅ **HeuristicLab.Core** → `GeneticProgramming.Core`
- ✅ **HeuristicLab.Common** → Utilitários integrados
- ✅ **HeuristicLab.Data** → `DataTypes.cs`
- ✅ **HeuristicLab.Random** → `Random.cs`
- 📋 **HeuristicLab.Parameters** → Será abstraído
- 📋 **HeuristicLab.Operators** → Será abstraído
- 📋 **HeuristicLab.Optimization** → Será abstraído

---

## 🎯 AÇÃO IMEDIATA

**PRÓXIMO PASSO:** Aprimorar cobertura de testes e exemplos

Agora que a infraestrutura principal está estável, o foco é criar mais testes automatizados e exemplos demonstrando o uso do framework.

**Comando para continuar:**
```bash
# Executar `dotnet test` para garantir que todos os testes continuem passando
```

## 🔍 Detalhes do Sistema de Símbolos

Os principais símbolos (Add, Sub, Mul, Div, Variable, Constant) já foram implementados e validados nos testes.

**Última atualização:** 30/05/2025 - Build e 153 testes executados com sucesso.
