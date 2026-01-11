---
name: integration-testing-performance
description: Integration testing performance optimization, test parallelization, cleanup strategies, and CI/CD categorization patterns. Use when optimizing test execution speed, managing test data, or structuring tests for automated pipelines.
---
# Integration Testing Performance and Maintainability

Use when speeding up integration suites, keeping data isolated/clean, and tailoring what runs in CI/CD stages.

## When to use
- Parallelizing integration tests with isolated tenants/DbContexts
- Adding cleanup for shared containers or fixtures
- Tagging tests for selective CI/CD execution (PR vs main vs nightly vs deploy)
- Tracking slow/memory-heavy tests and optimizing data setup

## Core principles
- Parallel by default with unique tenant IDs and fresh scopes per test
- Clean up created data (disposable scopes first; manual cleanup as fallback)
- Trait tests to slice by category/speed/feature/environment; align pipeline filters
- Monitor execution time and memory; fail fast on regressions
- Bulk-create data when needed; share expensive fixtures sparingly

## Resources
- Parallelization: [patterns/parallelization.md](patterns/parallelization.md)
- Cleanup: [patterns/cleanup.md](patterns/cleanup.md)
- Categorization and CI: [patterns/categorization-ci.md](patterns/categorization-ci.md)
- Performance monitoring: [patterns/monitoring.md](patterns/monitoring.md)
- Test data management: [patterns/test-data.md](patterns/test-data.md)

## Default locations
- Integration tests: tests/GloboTicket.IntegrationTests
- Shared fixtures/test data helpers: tests/GloboTicket.IntegrationTests/Fixtures or Helpers
- CI filters: pipeline scripts or dotnet test arguments in scripts/bash|powershell

## Validation checklist
- Parallelization enabled in csproj and safe (isolated tenant IDs, no shared state)
- Cleanup runs (disposable scopes or explicit) to prevent cross-test pollution
- Traits applied consistently; pipeline filters match categories/speed
- Long-running tests have timeouts; memory checks guard bulk ops where relevant
- Bulk data creation uses single round trips; shared fixtures dispose correctly