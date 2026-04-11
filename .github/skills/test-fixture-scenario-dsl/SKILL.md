---
name: test-fixture-scenario-dsl
description: 'Create or extend test fixtures with centralized mocks and fluent scenario methods. Use when multiple feature tests share the same mock setup, handlers, and orchestration patterns.'
argument-hint: 'Describe the feature area and repeated mock scenarios that should be centralized.'
user-invocable: true
---

# Test Fixture Scenario DSL

## When to Use
- Several feature tests repeat the same mock setup.
- A handler depends on multiple repositories or external services.
- Scenario setup is noisy enough to obscure test intent.

## What This Skill Produces
- A fixture class that centralizes shared mocks.
- Lazy access to handlers under test.
- Fluent scenario methods that describe business context.
- A policy boundary where repeated direct mock setup is replaced by fluent reusable setup.

## Procedure
1. Inspect repeated setup across tests in the same feature area.
2. Extract only the shared mocks and handlers into a fixture.
3. Add fluent `Com*` scenario methods that express business meaning.
4. Keep scenario methods chainable by returning the fixture itself.
5. Leave result assertions in the tests or assertion extensions.
6. When mock interaction verification is reused, move it to dedicated mock extensions rather than repeating raw `Verify` calls.

## Constraints
- Do not hide final verification inside the fixture.
- Do not mix DTO builder logic into fixture scenario methods.
- Do not centralize mocks that are used by only one isolated test unless that is clearly beneficial.
- Do not keep duplicating raw `Setup` blocks after a fixture scenario method exists.

## Quality Checks
- Fixture methods improve readability.
- Scenario methods are domain-oriented, not implementation-oriented.
- Handlers are easy to access without verbose construction in each test.

## Code Examples
- [Feature fixture](./references/FeatureFixture.md)
