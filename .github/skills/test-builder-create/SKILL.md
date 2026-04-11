---
name: test-builder-create
description: 'Create or evolve test builders with valid defaults and fluent Com* or Sem* APIs. Use when commands, queries, DTOs, or aggregates are repeatedly constructed in tests.'
argument-hint: 'Describe the command, query, DTO, or aggregate that needs a test builder.'
user-invocable: true
---

# Test Builder Create

## When to Use
- Repeated object construction appears across tests.
- A new command, query, DTO, or aggregate needs reusable valid defaults.
- Existing builders no longer cover updated feature inputs.

## What This Skill Produces
- A focused builder with valid default values.
- Fluent override methods such as `Com*` and `Sem*`.
- `Build()` and optionally `BuildRehydrated()` when the object lifecycle requires it.

## Procedure
1. Inspect the target type and identify required fields and common optional fields.
2. Choose defaults that represent a valid, realistic happy path.
3. Implement fluent override methods for fields that vary across scenarios.
4. Add `BuildRehydrated()` only when rehydration is a real test need.
5. Keep naming aligned with the domain vocabulary already used by the module.
6. Prefer extending an existing builder over creating a parallel builder for the same concept.

## Constraints
- Do not include assertions.
- Do not embed `Mock` behavior.
- Do not hide invalid defaults behind a builder intended for valid scenarios.
- Do not create multiple competing builders for the same target type.

## Quality Checks
- Defaults are valid.
- Method names are clear and domain-aligned.
- Builder surface is small and focused.

## Code Examples
- [Command builder](./references/CommandBuilder.md)
- [Aggregate builder](./references/AggregateBuilder.md)
