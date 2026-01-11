---
name: ef-core-patterns
description: Entity Framework Core best practices for configuration, queries, concurrency, and multi-tenancy.
---

# Entity Framework Core Best Practices

Use when configuring entities, optimizing queries, or implementing multi-tenant data access with EF Core.

## When to use
- Configuring entity mappings, indexes, relationships, and value objects
- Optimizing read queries (AsNoTracking, projections) or preventing N+1 problems
- Adding row version concurrency tokens or handling conflicts
- Applying global tenant filters or implementing repositories

## Core principles
- Prefer Fluent API over data annotations; keep entities clean with private setters
- Use AsNoTracking for reads; Include/split queries to avoid N+1; project to DTOs when possible
- Index tenant columns and frequent filter combinations; create unique composites for business rules
- Add RowVersion for optimistic concurrency; catch DbUpdateConcurrencyException
- Apply global query filters for multi-tenancy; bypass with IgnoreQueryFilters when needed
- Encapsulate in repositories or inject DbContext directly into services

## Resources
- Entity configuration: [patterns/entity-configuration.md](patterns/entity-configuration.md)
- Value objects: [patterns/value-objects.md](patterns/value-objects.md)
- Query performance: [patterns/query-performance.md](patterns/query-performance.md)
- Indexing strategy: [patterns/indexing.md](patterns/indexing.md)
- Optimistic concurrency: [patterns/concurrency.md](patterns/concurrency.md)
- Global query filters: [patterns/query-filters.md](patterns/query-filters.md)
- Repository pattern: [patterns/repository-pattern.md](patterns/repository-pattern.md)

## Default locations
- Entity configurations: src/GloboTicket.Infrastructure/Persistence/Configurations
- DbContext: src/GloboTicket.Infrastructure/Persistence/GloboTicketDbContext.cs
- Repositories (if used): src/GloboTicket.Infrastructure/Persistence/Repositories
- Migrations: src/GloboTicket.Infrastructure/Migrations

## Validation checklist
- Entities have private setters and parameterless constructors for EF
- Configurations use Fluent API with separate IEntityTypeConfiguration classes
- Tenant columns indexed; composite indexes match query patterns
- Read-only queries use AsNoTracking and project to DTOs
- Include/split queries prevent N+1; large collections use AsSplitQuery
- Concurrency tokens configured where updates are common
- Global filters applied to ITenantEntity types; IgnoreQueryFilters used sparingly
- SaveChanges called in repositories or services consistently