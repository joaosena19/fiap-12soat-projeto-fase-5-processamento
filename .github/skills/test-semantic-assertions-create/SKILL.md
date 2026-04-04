---
name: test-semantic-assertions-create
description: 'Create semantic assertion extensions for repeated test verifications. Use when the same group of asserts appears across multiple handler, aggregate, or DTO tests.'
argument-hint: 'Describe the repeated result or object assertions that should become semantic helpers.'
user-invocable: true
---

# Test Semantic Assertions Create

## When to Use
- Four or more related asserts repeat across multiple tests.
- Handler results or DTOs are verified with the same structure repeatedly.
- Aggregate or collection assertions need a reusable semantic name.

## What This Skill Produces
- Focused assertion helpers that express domain intent.
- Cleaner tests with less structural repetition.
- A policy boundary where repeated direct structural assertions are replaced by fluent semantic assertions.

## Procedure
1. Identify repeated assertion clusters.
2. Extract only cohesive groups of asserts into an extension method.
3. Name the extension according to business meaning.
4. Reuse the module's preferred assertion style.
5. Keep the helper narrow; create more than one helper if concerns differ.

## Constraints
- Do not mix mock verification with object or result assertions.
- Do not extract a helper when the reuse is weak or the semantic meaning is unclear.
- Do not keep duplicating the same assertion cluster after a semantic assertion helper exists.

## Quality Checks
- Tests become shorter and clearer.
- Helper names communicate intent immediately.
- Assertions remain easy to debug when they fail.

## Code Examples
- [Result assertions](./references/ResultAssertions.md)
