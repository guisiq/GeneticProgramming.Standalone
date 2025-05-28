# Genetic Programming Standalone

Este projeto é uma extração independente dos componentes de Programação Genética do HeuristicLab, criado para ser uma biblioteca standalone funcional.

## Estrutura do Projeto

```
GeneticProgramming.Standalone/
├── src/
│   ├── Core/              # Classes base e interfaces fundamentais
│   ├── Expressions/       # Árvores de expressão simbólica
│   ├── Operators/         # Operadores genéticos (crossover, mutation)
│   ├── Algorithms/        # Algoritmos GP principais
│   ├── Problems/          # Problemas exemplo
│   └── Abstractions/      # Abstrações para dependências externas
├── tests/                 # Testes unitários
├── examples/              # Exemplos de uso
└── docs/                  # Documentação
```

## Componentes Principais

### Core
- **IItem**: Interface base para todos os objetos do framework
- **Item**: Implementação base com suporte a clonagem profunda
- **Cloner**: Responsável por operações de clonagem profunda
- **DataTypes**: Wrappers para tipos básicos (int, double, bool, string)
- **Random**: Gerador de números aleatórios

### Expressions
- **ISymbolicExpressionTree**: Interface para árvores de expressão
- **SymbolicExpressionTree**: Implementação de árvore de expressão simbólica
- **ISymbolicExpressionTreeNode**: Interface para nós da árvore
- **ISymbol**: Interface para símbolos da gramática
- **ISymbolicExpressionTreeGrammar**: Interface para gramáticas

## Status de Implementação

### ✅ Concluído
- [x] Estrutura base do projeto
- [x] Interfaces fundamentais (IItem, IDeepCloneable)
- [x] Classe base Item
- [x] Sistema de clonagem (Cloner)
- [x] Tipos de dados básicos (IntValue, DoubleValue, BoolValue, StringValue)
- [x] Interface para árvores de expressão
- [x] Implementação básica de SymbolicExpressionTree
- [x] Interface para gerador de números aleatórios

### 🔄 Em Progresso
- [ ] Implementação de SymbolicExpressionTreeNode
- [ ] Sistema de símbolos e gramáticas
- [ ] Operadores genéticos básicos

### 📋 Pendente
- [ ] Operadores de crossover
- [ ] Operadores de mutação
- [ ] Criadores de população
- [ ] Avaliadores de fitness
- [ ] Algoritmos GP completos
- [ ] Problemas exemplo
- [ ] Testes unitários
- [ ] Documentação completa

## Origem

Este projeto foi extraído do HeuristicLab (https://github.com/heal-research/HeuristicLab), mantendo compatibilidade com a licença GPL v3.

## Licença

GNU General Public License v3.0 - veja LICENSE.txt para detalhes.
