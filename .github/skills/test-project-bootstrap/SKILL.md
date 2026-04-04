---
name: test-project-bootstrap
description: 'Create a new module test project when Modules/[ModuleName].Tests does not exist. Use for bootstrapping xUnit test structure, GlobalUsings, base folders, and initial project organization.'
argument-hint: 'Describe the module that needs a new test project.'
user-invocable: true
---

# Test Project Bootstrap

## When to Use
- The target module has no `Modules/[ModuleName].Tests` project.
- A new module needs its first unit test structure.

## What This Skill Produces
- A base test project aligned with repository conventions.
- Initial folder structure for helpers and tests.
- A starting `GlobalUsings.cs` aligned with the module needs.

## Procedure
1. Confirm the target module has no existing test project.
2. Create `Modules/[ModuleName].Tests` with the appropriate solution structure.
3. Add a base `GlobalUsings.cs` using the actual assertion and mocking stack adopted by the module.
4. Create initial folders such as `Helpers`, `Tests`, `Extensions`, and `Builders` only when justified.
5. Keep the project minimal; do not scaffold all tests yet.
6. Validate that the new project can be referenced and built.

## Constraints
- Do not create test files for every class automatically.
- Do not create speculative builders or fixtures before a concrete test need exists.
- Prefer the real module conventions over generic defaults.

## Completion Criteria
- Test project exists in the correct location.
- Initial organization is coherent with other module test projects.
- The project is ready for scaffold skills to add concrete tests.

## Code Examples
- [GlobalUsings example](./references/GlobalUsings.md)
