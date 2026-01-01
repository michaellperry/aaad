# Project Context Awareness

This is a Clean Architecture .NET project with specific patterns you must validate.

## Entity Hierarchy

- `Entity` base class for non-tenant entities (Id, CreatedAt, UpdatedAt)
- `MultiTenantEntity` for tenant-scoped entities (adds TenantId, Tenant navigation)
- Check inheritance is correct for entity classification

## Multi-Tenancy Requirements

- Top-level entities must have `TenantId` property
- Query filters must be configured in `GloboTicketDbContext.OnModelCreating()`
- Validate filter pattern: `v => _tenantContext.CurrentTenantId == null || v.TenantId == _tenantContext.CurrentTenantId`
- Child entities filter through navigation properties

## Entity Configuration Patterns (validate exact order)

1. ToTable()
2. HasKey() + ValueGeneratedOnAdd()
3. HasAlternateKey() for multi-tenant entities
4. HasIndex() for GUIDs and lookups
5. Property configurations (required, max length)
6. Complex property configurations
7. HasOne/HasMany relationships
8. Query filters (in DbContext, not configuration)

## Dependency Registration

- Services: `builder.Services.AddScoped<IService, ServiceImpl>()`
- DbContext: Check UseNetTopologySuite if geography types used
- Endpoints: `app.MapFeatureEndpoints()` after middleware

## Middleware Order (validate correct sequence)

1. UseHttpsRedirection
2. UseCors
3. UseAuthentication
4. UseAuthorization
5. TenantResolutionMiddleware
6. RateLimitingMiddleware
7. Endpoint mappings

## API Patterns

- Endpoints in `src/GloboTicket.API/Endpoints/{Feature}Endpoints.cs`
- Use `MapGroup("/api/feature")` with `.RequireAuthorization()`
- Validate error responses: 400 (validation), 404 (not found), 401 (unauthorized)
