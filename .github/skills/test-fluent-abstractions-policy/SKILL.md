---
name: test-fluent-abstractions-policy
description: 'Enforce fluent test abstractions for builders, fixtures, mock extensions, and semantic assertions. Use for deciding when direct setup or direct assertions are forbidden and how abstractions should be split technically.'
argument-hint: 'Describe the repeated setup, verification, or assertion pattern that should become a fluent abstraction.'
user-invocable: true
---

# Test Fluent Abstractions Policy

## When to Use
- Defining whether repeated setup should become a builder, fixture method, mock extension, or semantic assertion extension.
- Refactoring tests that still use raw repeated `Setup`, `Verify`, or assertion chains.
- Reviewing whether a new test should use existing fluent abstractions instead of direct low-level code.

## What This Skill Produces
- A technical rule set for fluent abstractions.
- Clear separation between builder, fixture, mock extension, and assertion extension responsibilities.
- Reduced direct duplication of setup and assertions.

## Technical Split
- Builder: creates valid or intentionally customized objects.
- Fixture: centralizes shared mocks, handlers, and fluent scenario composition.
- Mock extension: encapsulates repeated `Setup` and `Verify` patterns for a dependency.
- Semantic assertion extension: encapsulates repeated assertion clusters for results or domain objects.

## Rules
- If an abstraction already exists for the scenario, direct duplication is forbidden.
- When the same setup or assertion pattern appears for the second time, prefer creating or extending an abstraction.
- Keep builders free from mocks and assertions.
- Keep fixtures free from final result assertions.
- Keep mock extensions focused on dependency interaction.
- Keep semantic assertion extensions focused on value or result verification.

## Procedure
1. Inspect the repeated pattern.
2. Classify it as object creation, scenario setup, dependency interaction, or result assertion.
3. Route it respectively to builder, fixture, mock extension, or semantic assertion extension.
4. Name the abstraction with domain language and fluent method names.
5. Replace duplicated raw code with the new abstraction.

## Quality Checks
- The abstraction has one responsibility.
- The abstraction reduces repetition without hiding intent.
- Test bodies become shorter and easier to scan.

## Decision Guide
```text
Se o problema for criacao repetida de objetos:
- usar Builder

Se o problema for setup repetido de cenario de feature:
- usar Fixture com metodos Com*

Se o problema for Setup ou Verify repetido de uma dependencia:
- usar MockExtensions

Se o problema for bloco repetido de asserts estruturais:
- usar Semantic Assertion Extensions

Regra de ouro:
- se a abstracao ja existe, nao duplicar setup ou assert direto
- se o mesmo padrao apareceu pela segunda vez, extrair abstracao
```
