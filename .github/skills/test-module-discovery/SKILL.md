---
name: test-module-discovery
description: 'Discover the target module and current test structure before changing tests. Use this first for any test creation, test update, regression alignment, or scaffold flow so the orchestrator can decide between bootstrap, scaffold, and maintenance skills.'
argument-hint: 'Describe the target module plus the feature, handler, aggregate, or value object that will drive the test work, and whether the request is new tests, test maintenance, or regression alignment.'
user-invocable: true
---

# Test Module Discovery

## When to Use
- Before any test-related work routed by the architecture orchestrator.
- Starting any test-related task.
- Determining whether the correct flow is new tests, legacy refactor, or maintenance after feature changes.
- Checking what reusable test infrastructure already exists.

## What This Skill Produces
- A concise map of the target module.
- Identification of existing test project artifacts.
- A recommendation for which test workflow to use next.

This skill is the default entry point for the test workflow. Do not jump directly to scaffold or alignment skills before discovery.

## Procedure
1. Locate the target module under `Modules/[ModuleName]`.
2. Check whether `Modules/[ModuleName].Tests` exists.
3. Locate the target handler, aggregate, or value object.
4. Search for reusable builders, fixtures, mock extensions, semantic assertions, and global usings.
5. Infer the likely workflow:
   new tests, legacy test refactor, or maintenance after feature changes.
6. Return findings before editing any file.

## Decision Rules
- If there is no test project, route to `test-project-bootstrap`.
- If tests exist but use older conventions, route to refactor-oriented skills.
- If tests exist and a feature changed, route to `test-regression-alignment`.
- If new aggregate scenarios are required after discovery, route to `test-domain-aggregate-scaffold`.
- If new handler scenarios are required after discovery, route to `test-feature-handler-scaffold`.
- If value object invariants need source-of-truth coverage, route to `test-value-object-scaffold`.
- Any workflow that changes tests should end at `test-quality-gate`.

## Output Format
- Module target.
- Test project status.
- Relevant existing helpers.
- Likely workflow.

Expected output shape:
```text
Modulo alvo: PriceInquiry
Projeto de testes: existe
Feature alvo: CreatePriceInquiry
Fixture existente: PriceInquiryTestFixture
Builder existente: CreatePriceInquiryCommandBuilder
Assertion extension existente: PriceInquiryAssertionExtensions
Fluxo provavel: manutencao de testes existentes
```
