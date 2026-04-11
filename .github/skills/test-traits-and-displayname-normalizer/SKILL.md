---
name: test-traits-and-displayname-normalizer
description: 'Normalize test metadata such as DisplayName, Trait, and test method naming. Use for aligning xUnit tests to repository reporting conventions and Brazilian Portuguese naming.'
argument-hint: 'Describe the test files or feature area that need metadata normalization.'
user-invocable: true
---

# Test Traits And DisplayName Normalizer

## When to Use
- Tests lack `DisplayName` or `Trait` metadata.
- Method names do not follow the repository pattern.
- Reporting is inconsistent across a test class or feature area.

## What This Skill Produces
- Consistent xUnit metadata.
- Test names aligned with repository conventions.
- Explicit mandatory naming and metadata conventions for every new test.

## Procedure
1. Inspect the test class metadata patterns already used in the module.
2. Add or normalize `DisplayName` values in Brazilian Portuguese.
3. Add `Trait` values that match the feature area and severity level used by the module.
4. Align method names to the repository style.
5. Preserve existing good metadata; change only what is inconsistent.

## Mandatory Conventions
- Class name must be exactly the target class name plus `Tests`.
- Method name must follow `Metodo_Cenario_ComportamentoEsperado` and end with `Async` for asynchronous cases.
- `DisplayName` in Brazilian Portuguese is mandatory.
- `Trait("Category", "Unit")` is mandatory.
- Add at least one semantic trait representing the test focus, such as `Handle`, `Aggregate`, or `ValueObject`.
- Use `#region` blocks to separate major scenario groups when the class has more than one group.

## Constraints
- Do not rename correct tests only for stylistic preference.
- Do not invent new `Trait` taxonomies when the module already has one.

## Quality Checks
- Metadata improves test reporting.
- Naming is consistent and readable.
- Conventions match the target module, not a generic template.

## Code Examples
- [Test metadata](./references/TestMetadata.md)
