---
name: unit-testing
description: Provides unit testing patterns for .NET applications using EF Core In-Memory Provider, AAA structure, test data helpers, and TDD workflows. Use when writing unit tests for services, repositories, or domain logic.
---

# Unit Testing Patterns for .NET

Unit testing patterns for .NET applications covering test structure, EF Core in-memory database setup, test data helpers, and TDD workflows.

## TDD Philosophy

**TDD is about unit tests only.** Unit tests drive implementation using EF Core In-Memory Provider for fast, isolated testing.

**Integration tests follow implementation.** They use real databases to verify migrations, complex queries, and system integration.

## Test Structure

- **AAA Pattern**: Arrange-Act-Assert with Given-When-Then comments
- **Test Naming**: `MethodName_Scenario_ExpectedBehavior`
- **Given-When-Then Comments**: Describe test flow and intent

## EF Core In-Memory Database Setup
Load the resource when configuring DbContext for unit tests, setting up `ITenantContext`, ensuring per-test database isolation, and understanding provider limitations. See [patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md).

## Multi-Tenancy Testing
Use the in-memory setup resource for guidance on tenant filtering, admin/null-tenant behavior, and navigation property filter limitations. Load [patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md) when multi-tenancy affects your tests.

## Test Data Helper Patterns
Load the resource when authoring `Given` helpers: require parent navigation properties for child entities, use sensible defaults for scalars, and avoid foreign keys. See [patterns/test-data-helpers.md](patterns/test-data-helpers.md).

## Mocking Strategy

- Use EF Core In-Memory for data operations; avoid mocking data access.
- Mock external dependencies only (email, payment gateways, APIs).


## Best Practices

1. Use AAA Pattern with Given-When-Then comments
2. Follow `MethodName_Scenario_ExpectedBehavior` naming
3. Initialize navigation properties, never foreign keys
4. Make parent entities required parameters in Given methods
5. Use in-memory database for repository/service tests
6. Mock external dependencies only
7. Focus unit tests on business logic
8. Use integration tests for navigation property tenant isolation

## Resources

- [patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md): Load when setting up the in-memory DbContext, tenant context, or needing isolation/limitations guidance.
- [patterns/test-data-helpers.md](patterns/test-data-helpers.md): Load when creating `Given` helpers, distinguishing required parents vs optional scalars, and avoiding foreign keys.
- [examples/service-tests.md](examples/service-tests.md): Load when you want concrete service test examples using DbContext + `ITenantContext` and `Given` helpers.
- [examples/domain-tests.md](examples/domain-tests.md): Load when testing domain entities/value objects with invariants and equality.
