---
name: playwright-testing-patterns
description: End-to-end testing patterns with Playwright. Use when writing or debugging E2E tests, focusing on reliability, isolation, and flakiness prevention.
---

# Playwright Testing Patterns

Concise guidance for building reliable, maintainable Playwright tests.

## When To Use
- Writing new Playwright E2E tests
- Hardening flaky tests (timing, retries, animations)
- Setting up auth and multi-tenant fixtures
- Managing test data via API or setup projects

## Core Patterns
- **AAA + Naming**: Explicit Arrange/Act/Assert and behavior-based names.
- **Selectors**: Prefer roles/labels/text; test IDs last.
- **Waiting**: Web-first assertions and targeted `waitForResponse`; avoid fixed sleeps.
- **Fixtures**: Shared auth state and tenant fixtures; cleanup included.
- **Data**: Use APIs for setup/teardown; reset DB in global setup.
- **Flakiness**: Reduced motion, retries in CI, `expect().toPass` for fragile flows.
- **Parallel**: Only when data isolation is guaranteed.

## Resources
- [Test Structure](patterns/test-structure.md) — load when organizing tests and isolation.
- [Selectors and Forms](patterns/selectors-forms.md) — load when choosing locators and form patterns.
- [Waiting and Assertions](patterns/waiting-assertions.md) — load when coordinating waits and network assertions.
- [Authentication and Fixtures](patterns/authentication-fixtures.md) — load when sharing auth state or multi-tenant context.
- [Test Data Management](patterns/test-data-management.md) — load when creating data via API or resetting DB.
- [Flakiness Prevention](patterns/flakiness-prevention.md) — load when reducing retries/timeouts and handling intermittent failures.
- [Parallel Execution](patterns/parallel-execution.md) — load when enabling parallel mode safely.