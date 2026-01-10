# TDD Test First - Critical Constraint

## UNIT TESTS ONLY - NEVER INTEGRATION TESTS

**YOU MUST CREATE UNIT TESTS ONLY - NEVER INTEGRATION TESTS**

- ✅ **DO**: Create tests in `tests/GloboTicket.UnitTests/`
- ✅ **DO**: Use EF Core In-Memory Provider for database operations
- ✅ **DO**: Run `./scripts/bash/test-unit.sh` or `dotnet test tests/GloboTicket.UnitTests`
- ❌ **DO NOT**: Create tests in `tests/GloboTicket.IntegrationTests/`
- ❌ **DO NOT**: Use Testcontainers or real SQL Server
- ❌ **DO NOT**: Create integration test infrastructure (IAsyncLifetime, SqlServerContainer, etc.)
- ❌ **DO NOT**: Run `./scripts/bash/test-integration.sh` or test the IntegrationTests project

**Why Unit Tests Only?**
- Fast execution (milliseconds vs seconds)
- No external dependencies (database, containers)
- Simple setup with EF Core In-Memory Provider
- Focused on business logic and behavior
- Integration tests are created separately by different processes

**If you create integration tests instead of unit tests, this is a CRITICAL FAILURE and the task is incomplete.**
