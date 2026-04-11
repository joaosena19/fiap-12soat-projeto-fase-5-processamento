---
name: test-aaa-normalizer
description: 'Normalize Arrange, Act, Assert comments and structure in unit tests. Use for refactoring legacy tests or aligning new tests to the repository AAA pattern.'
argument-hint: 'Describe the test class or files that need AAA normalization.'
user-invocable: true
---

# Test AAA Normalizer

## When to Use
- Tests are missing `Arrange`, `Act`, `Assert` comments.
- Tests use incorrect AAA comments for single-line combined phases.
- New tests were added without the repository AAA structure.
- Legacy tests mix setup, action, and verification in a hard-to-scan way.

## What This Skill Produces
- Consistent AAA comments and structure.
- Better readability without changing test behavior.

## Procedure
1. Inspect the test method body.
2. Identify setup, action, and verification boundaries.
3. Add comments only for phases that actually exist in the test method.
4. Use `Arrange`, `Act`, `Assert`, `Arrange & Act`, or `Act & Assert` according to the real structure.
5. Remove orphan comments when a phase does not exist.
6. Reorder the code only when necessary to make AAA explicit.
7. Keep comments minimal and structural.

## Constraints
- Do not rewrite working tests only for cosmetic preference.
- Do not add verbose comments that restate obvious code.
- Do not keep `Arrange` when there is no setup.
- Do not keep `Act` when the action is merged with assert or does not exist as a standalone step.
- Do not keep `Assert` when there is no assertion block.

## Quality Checks
- Every non-trivial test has visible AAA structure using only existing phases.
- Comments reflect the real execution order.
- Combined-phase comments are used when phases are truly merged (`Arrange & Act` or `Act & Assert`).
- Behavior remains unchanged.

## Code Examples
- [AAA pattern](./references/AaaPattern.md)
