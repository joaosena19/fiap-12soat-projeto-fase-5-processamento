---
name: test-regression-alignment
description: 'Align existing tests after feature changes. Use when handlers, DTOs, rules, or dependencies changed and existing tests, builders, fixtures, or assertion helpers must be updated consistently.'
argument-hint: 'Describe the feature change and the tests or helpers likely affected.'
user-invocable: true
---

# Test Regression Alignment

## When to Use
- A feature changed and current tests no longer reflect the new behavior.
- Shared test helpers such as builders, fixtures, or semantic assertions must be updated first.
- Existing tests need targeted maintenance rather than full refactor.

## What This Skill Produces
- A focused impact map for changed tests.
- Ordered updates from shared helpers to specific tests.

## Procedure
1. Identify the changed feature contract: input, output, business rule, dependency, or naming.
2. Locate impacted tests and shared helpers.
3. Update builders, fixtures, and semantic assertions before editing individual tests when shared contracts changed.
4. Adjust specific tests to the smallest coherent extent.
5. Add only the missing scenarios required by the new feature behavior.
6. Validate with build and tests.

## Constraints
- Do not refactor the full module when only one feature changed.
- Do not add speculative scenarios unrelated to the change.
- Do not break fluent abstractions by patching only leaf tests when the shared contract changed.
- Do not use regression work as an excuse to blur layer boundaries.

Expected impact report shape:
```text
Feature alterada: CreatePriceInquiry
Impactos detectados:
- CreatePriceInquiryCommandBuilder
- PriceInquiryAssertionExtensions
- CreatePriceInquiryCommandHandlerTests

Acao sugerida:
- ajustar builder
- ajustar semantic assertions
- adicionar cenarios novos no handler
```
