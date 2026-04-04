---
name: test-feature-handler-scaffold
description: 'Scaffold unit tests for command handlers and query handlers. Use after `test-module-discovery` for creating or extending XxxCommandHandlerTests or XxxQueryHandlerTests with xUnit, Traits, DisplayName, AAA comments, and vertical-slice organization.'
argument-hint: 'Describe the handler class or feature that needs test scaffolding, the scenarios to cover, and whether `test-module-discovery` already mapped existing fixtures, builders, and tests.'
user-invocable: true
---

# Test Feature Handler Scaffold

## When to Use
- After `test-module-discovery` confirms the target module test structure.
- Creating tests for a new command handler or query handler.
- Adding structured new scenarios to an existing handler test class.
- Following `create-feature-handler` after handler or endpoint behavior changes.

## What This Skill Produces
- A handler test class with correct naming.
- Regions grouped by scenario type.
- Test methods structured with AAA.
- Feature tests that stay focused on orchestration rather than revalidating domain internals.

## Procedure
1. Locate the target handler and its dependencies.
2. Check whether a test class already exists.
3. Reuse an existing fixture if the module already has one.
4. Create or update the test class with the name `XxxTests` matching the target class.
5. Organize scenarios by region, such as success cases, validation failures, domain failure results, and unexpected technical errors when behavior is defined.
6. Apply `Fact`, `Theory`, `DisplayName`, and `Trait` consistently.
7. Leave detailed object construction to builders and detailed mock orchestration to fixtures.
8. Assert feature behavior, dependency interactions, and `Result` / `Error` mapping from lower layers without duplicating lower-layer rule matrices.
9. If repeated setup or repeated assertions appear, route support work to `test-builder-create`, `test-fixture-scenario-dsl`, or `test-semantic-assertions-create` instead of duplicating structure inside the test class.

## Constraints
- Do not bypass `test-module-discovery` when the current test layout is unknown.
- Do not inline repeated mock setup if a fixture is already appropriate.
- Do not introduce builders inside the test class.
- Do not test unrelated infrastructure concerns.
- Do not revalidate each domain or value object invalid-input permutation inside a feature test.
- Do not directly duplicate mock setup when a fixture or mock extension exists.
- Do not directly duplicate repeated structural assertion chains when a semantic assertion helper exists.

Close this workflow with `test-quality-gate` after handler tests are updated.

## Quality Checks
- Class name matches the target handler with `Tests` suffix.
- Async tests end with `Async`.
- Methods are in Brazilian Portuguese.
- AAA comments are present.
- `DisplayName` is present.
- `Trait("Category", "Unit")` is present.
- The test verifies feature orchestration or visible result mapping only, not lower-layer validation matrices.

## Code Examples
- [Create handler tests](./references/CreateHandlerTests.md)
- [Query handler tests](./references/QueryHandlerTests.md)
