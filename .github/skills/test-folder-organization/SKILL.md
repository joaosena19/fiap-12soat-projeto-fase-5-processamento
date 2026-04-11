---
name: test-folder-organization
description: 'Organize module test folders following the PriceInquiry.Tests pattern. Use for creating or refactoring test project structure with Helpers grouped by responsibility and Tests split into Domain and Features with business-specific subfolders.'
argument-hint: 'Describe the module test project that needs folder organization or reorganization.'
user-invocable: true
---

# Test Folder Organization

## When to Use
- Creating a new module test project and deciding its folder structure.
- Refactoring a disorganized test project to a clearer layout.
- Aligning another module test project with the successful `PriceInquiry.Tests` organization.
- Supporting both first-time test creation and legacy test refactors where structural organization is part of the work.

## What This Skill Produces
- A consistent test project folder layout.
- Separation between reusable test helpers and concrete tests.
- Predictable locations for feature tests, domain tests, builders, fixtures, extensions, and mock extensions.

## Agent Usage Rule
- In new test workflows, use this skill immediately after discovery and bootstrap, before creating concrete tests.
- In legacy refactor workflows, use this skill before detailed cleanup when the current folder structure hides reuse, duplicates helpers, or mixes layers.
- In maintenance workflows, use this skill only when the feature change requires structural moves, not for incidental edits.

## Reference Pattern
This skill is based on the structure used in `Modules/PriceInquiry.Tests`, where:
- `Helpers/Builder` stores builders for commands, queries, DTOs, and aggregates.
- `Helpers/Extensions` stores semantic assertion helpers and domain assertion helpers.
- `Helpers/Fixtures` stores shared feature fixtures.
- `Helpers/MockExtensions` stores mock setup and verification extensions.
- `Tests/Domain` stores aggregate and value object tests grouped by aggregate root.
- `Tests/Features` stores handler tests grouped first by bounded feature area, then by use case.

## Procedure
1. Inspect the target test project and identify whether it already separates helpers from concrete tests.
2. Create or normalize a top-level `Helpers` folder for reusable test infrastructure.
3. Under `Helpers`, group files by responsibility instead of by module feature.
4. Create or normalize a top-level `Tests` folder for executable test classes.
5. Under `Tests/Domain`, group by aggregate root and then by deeper domain concepts such as `ValueObjects` when needed.
6. Under `Tests/Features`, group first by feature area and then by individual use case folders.
7. Keep test file names matched to the class under test with the `Tests` suffix.
8. Avoid scattering helper files inside feature folders unless they are truly local and non-reusable.

## Decision Rules
- If a helper is reused across multiple test files, it belongs under `Helpers`.
- If a test validates domain invariants, it belongs under `Tests/Domain`.
- If a test validates command or query handlers, it belongs under `Tests/Features`.
- If a helper is specific to mock setup or verification, prefer `Helpers/MockExtensions`.
- If a helper is specific to assertions, prefer `Helpers/Extensions`.

## Constraints
- Do not mix builders, fixtures, and assertion extensions in the same folder.
- Do not place handler tests directly under the root of `Tests` when a feature hierarchy exists.
- Do not flatten domain tests when aggregate-specific grouping improves navigation.
- Do not mirror production folders mechanically if the test intent is better expressed by `Domain` and `Features`.

## Quality Checks
- The structure makes it obvious where to add a new handler test.
- The structure makes it obvious where to add a new aggregate or value object test.
- Reusable helpers are centralized and easy to find.
- Folder names reflect test intent, not arbitrary technical groupings.

## Recommended Structure
```text
Modules/[ModuleName].Tests/
├── [ModuleName].Tests.csproj
├── GlobalUsings.cs
├── Helpers/
│   ├── Builder/
│   ├── Extensions/
│   ├── Fixtures/
│   └── MockExtensions/
└── Tests/
	├── Domain/
	│   ├── [AggregateRoot]/
	│   │   ├── [AggregateRoot]Tests.cs
	│   │   └── ValueObjects/
	└── Features/
		├── [FeatureArea]/
		│   └── [UseCase]/
		│       └── [HandlerClass]Tests.cs
```
