---
name: tdd-testing-patterns
description: Provides TDD patterns, AAA structure, and testing strategies for .NET applications. Use when writing unit tests or implementing test-driven development workflows in GloboTicket. Note: TDD is about unit tests only; integration tests follow implementation and are handled separately.
---

# Test-Driven Development Patterns

This skill provides TDD patterns and testing strategies for .NET applications, supporting the Red-Green-Refactor cycle.

## TDD Philosophy

**TDD is about unit tests only.** Unit tests drive implementation through the red-green-refactor cycle. They use real repositories with EF Core In-Memory Provider for fast, isolated testing of business logic.

**Integration tests follow implementation.** They use real repositories with SQL Server (via Testcontainers) and migrated databases to verify migrations, complex queries, and system integration. Integration tests are valuable but separate from the TDD workflow.

## Test Structure and Organization

### Unit Test Project
**Place all unit tests in the `GloboTicket.UnitTests` project.**

### AAA Pattern (Arrange-Act-Assert)
**Structure every test using the AAA pattern for clarity and maintainability.**

### Test Naming Conventions
**Use descriptive test names that follow the pattern: MethodName_Scenario_ExpectedBehavior**

### Given-When-Then Format
**Use Given-When-Then comments to make test intent crystal clear. This is different from the `Given` prefix used in test data helper methods (e.g., `GivenVenue()`). See [Test Data Helpers](patterns/test-data-helpers.md) for details on helper methods.**

**Note**: "Given-When-Then" comments describe test flow and intent. The `Given` prefix in helper methods (e.g., `GivenVenue()`) is a separate pattern for creating test data with default parameters. Both patterns can be used together in the same test.

## Detailed Patterns

For comprehensive examples and detailed guidance, see:

- **[Unit Testing](patterns/unit-testing.md)**: Testing entities, value objects, and domain logic
- **[Mocking](patterns/mocking.md)**: Prefer in-memory database for repositories; mock only external dependencies
- **[Test Data Helpers](patterns/test-data-helpers.md)**: Given helper methods with default parameters for creating test data

**Note**: Integration testing patterns are separate from TDD. Integration tests follow implementation and use Testcontainers with real SQL Server. See the `integration-test-writer` mode and `integration-test-patterns` skill for integration testing guidance.
