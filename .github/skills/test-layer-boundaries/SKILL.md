---
name: test-layer-boundaries
description: 'Enforce unit test boundaries between value objects, aggregates, and features. Use for deciding what each test layer should validate and what it must not duplicate from other layers.'
argument-hint: 'Describe the layer under test and the risk of duplicated validation across layers.'
user-invocable: true
---

# Test Layer Boundaries

## When to Use
- Creating new tests and deciding the right scope.
- Refactoring tests that duplicate responsibilities across layers.
- Reviewing whether feature, aggregate, and value object tests are validating only their own layer.

## What This Skill Produces
- A clear rule set for layer-specific assertions.
- Reduced duplication between feature, aggregate, and value object tests.
- Better test suites with fewer brittle overlaps.

## Layer Rules
- Value object tests own null or empty checks, limits, format validation, enum validation, conversions, normalization, equality (record semantics), and shared base class inheritance assertions (`ValueObject<TValue>` or `ValueObject`).
- Aggregate tests own aggregate creation rules, child composition, rehydration, state transitions, and aggregate-level invariants.
- Feature tests own orchestration, dependency interaction, result mapping, and translation of lower-layer failures through `Result` / `Error`.

## What Each Layer Must Not Do
- Value object tests must not validate feature orchestration.
- Aggregate tests must not duplicate every dedicated value object validation case.
- Feature tests must not reproduce every invalid domain permutation just to trigger a domain failure result.

## Procedure
1. Identify the layer under test.
2. List the behavior that truly belongs to that layer.
3. Remove or avoid assertions that belong to a lower layer already covered elsewhere.
4. Keep only the interaction with lower-layer failures that is visible at the current layer boundary.
5. Validate that the test still proves the intended behavior without redundant rule matrices.

## Quality Checks
- Each test class has a single layer responsibility.
- Domain invalid-input matrices live in value object or aggregate tests, not feature tests.
- Feature tests assert result mapping, error codes, and visible orchestration behavior only.

## Boundary Matrix
```text
Layer: ValueObject
- Deve validar: nulidade, vazio, limites, formato, enum, conversoes
- Nao deve validar: orquestracao de feature, regras de aggregate

Layer: Aggregate
- Deve validar: criacao do aggregate, composicao, reidratacao, invariantes do aggregate
- Nao deve validar: toda a matriz detalhada de validacoes de value objects ja coberta em testes dedicados

Layer: Feature
- Deve validar: orquestracao, interacao com dependencias, resultado, codigos de erro e mapeamento de falha visivel no boundary da feature
- Nao deve validar: todas as combinacoes invalidas que fazem o domain falhar internamente
```
