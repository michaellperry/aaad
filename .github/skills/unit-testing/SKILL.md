---
name: unit-testing
description: Unit testing patterns for .NET applications using EF Core In-Memory Provider, AAA structure, test data helpers, and TDD workflows. Use when writing unit tests for Application Services, domain logic, or data access.
---

# Unit Testing Patterns for .NET

Unit testing patterns for .NET applications covering test structure, EF Core in-memory database setup, test data helpers, and TDD workflows.

## GloboTicket Architecture Context

**Application Services Location:** `GloboTicket.Application/Services/`
- Application Services inject base `DbContext` class (not concrete `GloboTicketDbContext`)
- Use `_dbContext.Set<TEntity>()` for data access instead of specific DbSets
- Inject `ITenantContext` from `GloboTicket.Application.MultiTenancy`
- Unit tests use EF Core In-Memory Provider to test services without Infrastructure dependencies

## TDD Philosophy

**TDD is about unit tests only.** Unit tests drive implementation using EF Core In-Memory Provider for fast, isolated testing.

**Integration tests follow implementation.** They use real databases to verify migrations, complex queries, and system integration.

## Test Structure

- **AAA Pattern**: Arrange-Act-Assert with Given-When-Then comments
- **Test Naming**: `MethodName_Scenario_ExpectedBehavior`
- **Given-When-Then Comments**: Describe test flow and intent

## EF Core In-Memory Database Setup
Use EF Core In-Memory for all unit-test data access; avoid mocking data stores. Load [patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md) when:
- Configuring `DbContext` and `ITenantContext` for tests
- Ensuring per-test database isolation and async disposal
- Understanding provider limitations (e.g., navigation-property filters)

## Multi-Tenancy Testing

**Direct Tenant Filtering** (works in unit tests): Entities with direct `TenantId` properties

**Navigation Property Filtering** (integration tests only): EF Core In-Memory does NOT support query filters using navigation properties.

**For child entities filtered through navigation properties:**
- **Unit Tests**: Focus on business logic (validation, transformations, error handling)
- **Integration Tests**: Test tenant isolation with real database

For setup, patterns, and limitations, load [patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md).

## Test Data Helper Patterns
Always initialize parent navigation properties (never foreign keys). Require parents for child entities; allow sensible defaults for scalars. Load [patterns/test-data-helpers.md](patterns/test-data-helpers.md) when creating `Given` helpers and structuring required vs optional parameters.

## Example Unit Tests

For complete examples that use `DbContext` + `ITenantContext` and `Given` helpers:
- Load [examples/service-tests.md](examples/service-tests.md) for application service tests
- Load [examples/domain-tests.md](examples/domain-tests.md) for entity/value object tests

Repository pattern is not used; services access the database via `DbContext`. Use `Given` helpers to create root entities and pass required parents to child entities.

## Mocking Strategy

- Use in-memory database for repository/service tests
- Mock external dependencies only (email, payment gateways, APIs)
- For mocking examples, load [examples/service-tests.md](examples/service-tests.md)

## Best Practices

1. **Use AAA Pattern** with Given-When-Then comments
2. **Follow naming convention**: `MethodName_Scenario_ExpectedBehavior`
3. **Initialize navigation properties**, never foreign keys
4. **Make parent entities required parameters** in Given methods
5. **Use in-memory database** for repository/service tests
6. **Mock external dependencies only**
7. **Focus unit tests on business logic**
8. **Use integration tests** for navigation property tenant isolation
9. **Create unique database per test** to ensure isolation
10. **Always test multi-tenancy** when relevant

This approach ensures fast, reliable, and maintainable unit tests that provide confidence in the business logic implementation.

## Resources
- [.github/skills/unit-testing/patterns/ef-core-in-memory-setup.md](patterns/ef-core-in-memory-setup.md)
- [.github/skills/unit-testing/patterns/test-data-helpers.md](patterns/test-data-helpers.md)
- [.github/skills/unit-testing/examples/service-tests.md](examples/service-tests.md)
- [.github/skills/unit-testing/examples/domain-tests.md](examples/domain-tests.md)