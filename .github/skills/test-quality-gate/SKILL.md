---
name: test-quality-gate
description: 'Run the final validation gate for unit test work. Use as the last step after any test scaffold, regression alignment, or test refactor to check structure, AAA consistency, naming, helper reuse, build success, and test execution.'
argument-hint: 'Describe the module or test files that should pass final validation and which earlier test skill produced the changes.'
user-invocable: true
---

# Test Quality Gate

## When to Use
- As the mandatory final step after any `test-*` skill changes files.
- After creating new tests.
- After refactoring legacy tests.
- After updating tests for feature changes.

## What This Skill Produces
- A final validation checklist outcome.
- Confirmation of remaining risks or unresolved issues.

## Procedure
1. Verify that test files are in the correct module test project.
2. Verify naming, `DisplayName`, `Trait`, and AAA structure.
3. Check whether reusable builders, fixtures, and assertion helpers were used where appropriate.
4. Run build validation.
5. Run test validation.
6. Report only the material issues that remain.

## Constraints
- Do not add unrelated refactors during validation.
- Do not claim success without build and test evidence when execution is possible.

If this validation fails, reopen the most relevant scaffold or alignment skill instead of silently accepting the gap.

## Validation Checklist
- Projeto de testes correto.
- Classe `XxxTests` no local correto.
- AAA presente.
- `DisplayName` e `Trait` presentes.
- Builders e fixtures reutilizados quando possível.
- Build executada com sucesso.
- Testes executados com sucesso.
