---
name: test-value-object-scaffold
description: 'Scaffold tests for value objects covering valid inputs, invalid boundaries, null or empty values, and implicit conversions. Use after `test-module-discovery` for domain value objects that should own the source-of-truth invariant matrix in module tests.'
argument-hint: 'Describe the value object to scaffold tests for, the factories or conversions to cover, and whether `test-module-discovery` already identified the module test project and helper structure.'
user-invocable: true
---

# Test Value Object Scaffold

## When to Use
- After `test-module-discovery` confirms the target module test structure.
- Creating tests for a new value object.
- Expanding value object coverage after validation rules or conversions change.
- Following `create-value-object` after value object creation or refactoring.

## What This Skill Produces
- Tests for valid values.
- Tests for null, empty, or out-of-range values.
- Tests for implicit conversions when they exist.
- The source-of-truth tests for value object invariants, so upper layers do not need to duplicate them.

## Procedure
1. Inspect the public factory methods (`Create`, `From`, `Criar`, `Parse`, `TryParse`) and outward conversions.
2. Check whether the VO inherits from `ValueObject<TValue>` (scalar/ID) or `ValueObject` (composite).
3. Identify valid inputs, invalid boundaries, conversions, and normalization rules.
4. Create tests for acceptance of valid values.
5. Create tests for null, empty, out-of-range, or invalid enum values.
6. Add conversion tests for implicit or explicit operators when present.
7. Add equality tests verifying record semantics (same values = same instance equivalence).
8. Add a type assertion test verifying the VO inherits from the correct base (`ValueObject<TValue>` or `ValueObject`).
9. Use shared domain assertion helpers if they already exist.

## Constraints
- Do not bypass `test-module-discovery` when the current test layout is unknown.
- Do not use mocks in value object tests.
- Do not create aggregate-oriented assertions in value object tests.
- Do not move feature or aggregate concerns into value object tests.

Value object tests are the source of truth for invariant matrices. Upper layers should not duplicate those same invalid-boundary permutations.

Close this workflow with `test-quality-gate` after value object tests are updated.

## Quality Checks
- Tests cover both positive and negative paths.
- Error expectations align with the actual `Result` failures and error codes.
- Conversion tests assert the semantic value, not just type existence.
- Equality tests verify that two instances with the same values are considered equal.
- A type assertion test confirms the VO inherits from the correct base class (`ValueObject<TValue>` or `ValueObject`).
- Upper-layer tests can rely on these tests instead of revalidating the same invariant matrix.

## Code Examples
- [String value object tests](./references/StringValueObjectTests.md)
