# Genetic Programming Standalone - Sistema de Símbolos e Gramática

## 🎯 Objetivo da Etapa
Implementar o sistema de símbolos básicos (Add, Sub, Mul, Div, Variable, Constant) e a gramática de árvores de expressão simbólica.

## ✅ Concluído
- [x] Interface `ISymbol` e classe base `Symbol`
- [x] Implementação de símbolos matemáticos: `Addition`, `Subtraction`, `Multiplication`, `Division`
- [x] Implementação de símbolos terminais: `Variable`, `Constant`
- [x] Interfaces e classes de gramática: `ISymbolicExpressionTreeGrammar`, `SymbolicExpressionTreeGrammar`
- [x] Gramática padrão: `DefaultSymbolicExpressionTreeGrammar` e `SymbolicRegressionGrammar`
- [x] Configuração de regras de gramática e validação básica

## 🔄 Próximo Objetivo: Operadores Genéticos Básicos
Implementar operadores de criação, crossover e mutação de árvores.

### Subetapas Planejadas
1. Criadores de Árvores (Tree Creators):
   - [x] `GrowTreeCreator`
   - [x] `FullTreeCreator`
2. Operadores de Crossover:
   - [x] `SubtreeCrossover`
   - [x] `OnePointCrossover`
   - [x] Outros crossovers básicos (ex.: `UniformCrossover`)
3. Operadores de Mutação:
   - [x] `SubtreeMutator`
   - [x] `ChangeNodeTypeMutator`
4. Integração com `GeneticProgrammingAlgorithm`:
   - [x] Configurar pipeline de operadores

**Data:** 28/05/2025
