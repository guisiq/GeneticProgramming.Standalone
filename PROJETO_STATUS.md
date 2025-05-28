# Genetic Programming Standalone - Projeto Independente

## 📋 Visão Geral
Este projeto é uma extração independente dos componentes de Programação Genética do HeuristicLab, criado para ser uma biblioteca standalone funcional.

**Origem:** Extraído do HeuristicLab (https://github.com/heal-research/HeuristicLab)  
**Licença:** GNU General Public License v3.0  
**Status:** 🔄 EM DESENVOLVIMENTO ATIVO

## 🎯 STATUS ATUAL: ✅ SymbolicExpressionTreeNode Implementado
**MARCO ALCANÇADO!** O projeto agora possui um sistema funcional de árvores de expressão simbólica com nós completos capazes de realizar operações de árvore reais.

### ✅ CONCLUÍDO RECENTEMENTE
- [x] **SymbolicExpressionTreeNode implementado** - Componente crítico finalizado
- [x] **Operações de árvore funcionais** - Adicionar, remover, substituir subárvores
- [x] **Sistema de iteração completo** - Breadth-first, prefix, postfix
- [x] **Gerenciamento de cache** - Otimização de profundidade e comprimento
- [x] **Relacionamentos pai-filho** - Manutenção automática de referências
- [x] **Compilação bem-sucedida** - Projeto compila sem erros

### 🔄 PRÓXIMO OBJETIVO: Sistema de Símbolos
Com a base das árvores funcionando, agora podemos implementar símbolos específicos (Add, Mul, Variable, Constant) que darão significado matemático às árvores.

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
- **ISymbol**: ❌ Interface para símbolos da gramática
- **ISymbolicExpressionTreeGrammar**: ❌ Interface para gramáticas

### 📋 Operators (Não Iniciado)
- **Creators**: Criadores de árvores (Grow, Full, Ramped Half-and-Half)
- **Crossovers**: Operadores de crossover (Subtree, etc.)
- **Manipulators**: Operadores de mutação
- **Architecture Manipulators**: Alteração da estrutura

---

## ✅ Progresso Atual - Validação

### ✅ CONCLUÍDO
- [x] **Estrutura base do projeto** (Pastas, .csproj)
- [x] **Interfaces fundamentais** (IItem, IDeepCloneable)
- [x] **Classe base Item** (Com PropertyChanged, clonagem)
- [x] **Sistema de clonagem** (Cloner com mapeamento)
- [x] **Tipos de dados básicos** (IntValue, DoubleValue, BoolValue, StringValue)
- [x] **Interface para árvores de expressão** (ISymbolicExpressionTree)
- [x] **Implementação SymbolicExpressionTree** (Com iteradores)
- [x] **Interface para nós da árvore** (ISymbolicExpressionTreeNode)
- [x] **Interface gerador de números aleatórios** (IRandom)
- [x] **Implementação MersenneTwister** (Gerador de números aleatórios)
- [x] **SymbolicExpressionTreeNode completo** ✨ **NOVO!**
- [x] **Compilação sem erros** (dotnet build ✅)

### 🔄 EM PROGRESSO
- [ ] **Sistema de símbolos básicos** (Add, Mul, Variable, Constant) ← **PRÓXIMO**
- [ ] **Gramáticas de expressão**
- [ ] **Operadores genéticos básicos**

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
- [ ] **SymbolicExpressionTreeNode** ← **PRÓXIMO**
- [ ] Sistema de símbolos
- [ ] Gramáticas

### Fase 3: Operadores Genéticos 📋
- [ ] Criadores de população
- [ ] Operadores de crossover
- [ ] Operadores de mutação
- [ ] Seletores

### Fase 4: Algoritmos e Problemas 📋
- [ ] Algoritmo GP básico
- [ ] Problemas exemplo
- [ ] Avaliadores de fitness

### Fase 5: Testes e Exemplos 📋
- [ ] Testes unitários
- [ ] Exemplos funcionais
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

**PRÓXIMO PASSO:** Implementar `SymbolicExpressionTreeNode.cs`

Este é o componente crítico que permitirá:
1. Criar nós de árvore funcionales
2. Implementar operações de árvore (adicionar/remover subárvores)
3. Suportar iteração e manipulação de árvores
4. Base para todos os operadores genéticos

**Comando para continuar:**
```bash
# Implementar SymbolicExpressionTreeNode
# Extrair do HeuristicLab.Encodings.SymbolicExpressionTreeEncoding
```
