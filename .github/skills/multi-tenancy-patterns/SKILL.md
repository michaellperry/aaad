---
name: multi-tenancy-patterns
description: Multi-tenancy implementation patterns for row level security, tenant isolation, and data segregation in SaaS applications.
---

# Multi-Tenancy Implementation Patterns

Use when implementing multi-tenant data isolation, tenant context resolution, or designing tenant-aware entities for SaaS applications.

## When to use
- Implementing row level security with shared database architecture
- Creating tenant-aware entities with ITenantEntity interface
- Resolving tenant context from subdomain, custom domain, header, or JWT claim
- Configuring automatic tenant filtering in DbContext with global query filters
- Testing cross-tenant access denial and tenant isolation

## Core principles
- Use row level security (shared database) for cost-effectiveness and scalability; all entities implement ITenantEntity
- Resolve tenant in middleware from subdomain → custom domain → header → JWT claim; inject ITenantContext
- Apply global query filters in OnModelCreating for automatic tenant filtering; validate ownership in SaveChanges
- Always include TenantId in WHERE clauses; composite indexes with tenant_id as first column
- Never expose tenant_id in API responses unless needed; log tenant_id for audit trails; monitor cross-tenant access
- Test cross-tenant access denial (unit) and require tenant context for all endpoints (integration)

## Resources
- Isolation strategies: [patterns/isolation-strategies.md](patterns/isolation-strategies.md)
- Entity structure: [patterns/entity-structure.md](patterns/entity-structure.md)
- Tenant resolution: [patterns/tenant-resolution.md](patterns/tenant-resolution.md)
- Database configuration: [patterns/database-configuration.md](patterns/database-configuration.md)
- Security considerations: [patterns/security-considerations.md](patterns/security-considerations.md)
- Performance optimization: [patterns/performance-optimization.md](patterns/performance-optimization.md)
- Testing multi-tenancy: [patterns/testing-multi-tenancy.md](patterns/testing-multi-tenancy.md)

## Default locations
- Tenant entities: src/GloboTicket.Domain/Entities (inherit from MultiTenantEntity)
- Tenant context: src/GloboTicket.Infrastructure/MultiTenancy/TenantContext.cs
- Resolution middleware: src/GloboTicket.API/Middleware/TenantResolutionMiddleware.cs
- DbContext filters: src/GloboTicket.Infrastructure/Persistence/GloboTicketDbContext.cs (OnModelCreating)

## Validation checklist
- All domain entities inherit from MultiTenantEntity and implement ITenantEntity
- TenantResolutionMiddleware resolves tenant from subdomain/domain/header/JWT and injects ITenantContext
- DbContext applies global query filters to all ITenantEntity types in OnModelCreating
- SaveChanges sets TenantId for new entities and validates ownership for updates/deletes
- All queries explicitly include TenantId filter (even with global filters for clarity)
- Composite indexes include tenant_id as first column for tenant-scoped queries
- API responses do not expose tenant_id unless explicitly needed; logged for audit
- Unit tests verify cross-tenant access denial; integration tests require tenant context for endpoints