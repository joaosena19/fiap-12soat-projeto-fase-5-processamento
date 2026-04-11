---
name: test-shouldly-conversion
description: 'Convert or align tests to the Shouldly assertion style. Use for migrating legacy FluentAssertions tests, standardizing result assertions, and adopting module-local Shouldly helpers.'
argument-hint: 'Describe the test file or module that should be aligned to Shouldly.'
user-invocable: true
---

# Test Shouldly Conversion

## When to Use
- Legacy tests still use `FluentAssertions`.
- The target module already standardized on `Shouldly`.
- Result assertions, async failure checks, or collection assertions need alignment.
- Creating new tests in this repository workflow.

## What This Skill Produces
- Tests aligned to `Shouldly`.
- Use of local helper assertions where appropriate.

## Procedure
1. Detect the current assertion style in the target module.
2. Replace `FluentAssertions` usages with equivalent `Shouldly` expressions.
3. Align success/failure assertions to `IsSuccess`, `IsFailure`, `Errors`, and semantic Shouldly checks.
4. Reuse module-local helpers such as count, matching, uniqueness, or single-item helpers when they exist.
5. Verify that the new assertions preserve the same semantics.

## Constraints
- Do not create new tests with `FluentAssertions` in this workflow.
- Do not weaken assertions during migration.

## Standard
- `Shouldly` is the default and expected assertion library for new tests and test refactors covered by these test skills.
- `FluentAssertions` is treated as legacy in this workflow.

## Quality Checks
- Converted assertions preserve original intent.
- Result assertions are consistent and explicit.
- Imports and global usings are aligned after conversion.

## Code Examples
- [Async exception assertion](./references/AsyncExceptionAssertion.md)
