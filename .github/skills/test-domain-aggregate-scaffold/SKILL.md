---
name: test-domain-aggregate-scaffold
description: 'Scaffold aggregate tests for domain rules, creation paths, optional fields, child entity linkage, and rehydration. Use after `test-module-discovery` for aggregate roots or aggregate-owned entity behavior in the domain layer.'
argument-hint: 'Describe the aggregate root, the new or changed domain scenarios, and confirm whether `test-module-discovery` already identified the module test project and existing helpers.'
user-invocable: true
---

# Test Domain Aggregate Scaffold

## When to Use
- After `test-module-discovery` confirms the target module test structure.
- Creating first-time tests for an aggregate root.
- Expanding aggregate coverage after a new rule or lifecycle path is added.
- Following `create-aggregate` after aggregate implementation changes.
- Following `create-entity` as a temporary fallback when entity behavior changes and no dedicated entity scaffold exists.

## What This Skill Produces
- Aggregate-focused tests that validate domain invariants.
- Coverage for creation, invalid inputs, optional data, computed behavior, and rehydration.
- Aggregate tests that exercise aggregate-level behavior without duplicating every dedicated value object test case.

## Procedure
1. Locate the aggregate root and related child entities or value objects.
2. Identify the aggregate creation path and any rehydration method.
3. Create scenarios for valid creation.
4. Create scenarios for invalid creation and domain failure results.
5. Add scenarios for optional fields and computed fields when present.
6. Add rehydration scenarios preserving IDs, timestamps, and relationships.
7. Keep tests pure; avoid mocks unless the domain shape truly requires them.
8. Reuse dedicated value object tests as the source of truth for value object rule matrices.

## Constraints
- Do not bypass `test-module-discovery` when the test project structure is not already known.
- Do not turn aggregate tests into handler tests.
- Do not use repository or service mocks for domain-only behavior.
- Do not rewrite every value object validation case inside aggregate tests when value object tests already own that responsibility.
- Do not assert feature-orchestration concerns in aggregate tests.

Close this workflow with `test-quality-gate` after the aggregate tests are updated.

## Quality Checks
- Assertions express business rules, not just structure.
- Failure assertions verify `IsFailure`, `Errors`, and domain error codes instead of expected exceptions.
- Rehydration tests preserve original persisted values.
- Child entities remain linked to the aggregate correctly.
- Aggregate tests validate aggregate behavior at the aggregate boundary.

## Code Examples
- [Aggregate tests](./references/AggregateTests.md)
