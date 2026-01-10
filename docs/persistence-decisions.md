# Persistence Engineering Decision Log

This document tracks architectural decisions made by the Persistence Engineer mode to maintain consistency and provide context for future work.

## Purpose

The Persistence Engineer is responsible for:
- Entity Framework Core configurations
- Database migrations
- Index optimization
- Multi-tenant isolation at the database level
- Query performance

This log ensures decisions remain consistent across features and provides rationale for future engineers.

## How to Use This Log

**Before starting work:**
1. Read this document to understand existing patterns
2. Search for similar entities or features
3. Follow established conventions unless there's a compelling reason to deviate

**After completing work:**
1. Append a new decision entry using the template below
2. Document key technical choices and rationale
3. Link to related decisions for cross-reference

## Decision Entry Template

```markdown
### [Entity/Feature Name] - [YYYY-MM-DD]
**Migration**: `YYYYMMDDHHMMSS_MigrationName`
**Related Files**: 
- `path/to/Configuration.cs`
- `path/to/Service.cs`

**Decisions Made**:
- Decision 1: Description
- Decision 2: Description

**Rationale**:
- Why these decisions were made
- Performance considerations
- Multi-tenancy requirements
- Domain constraints

**Trade-offs**:
- What alternatives were considered
- Why they were rejected
- Known limitations

**Related Decisions**: [Link to related entry](#anchor)
```

---

## Decisions

### Tenant Entity - 2025-11-22
**Migration**: `20251122194837_InitialCreate`
**Related Files**: 
- [`src/GloboTicket.Infrastructure/Data/Configurations/TenantConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/TenantConfiguration.cs)

**Decisions Made**:
- `TenantIdentifier` as unique string identifier for tenant resolution
- No query filter on Tenant entity (root of multi-tenancy hierarchy)
- Required `Name` field with max length 200

**Rationale**:
- TenantIdentifier enables subdomain-based tenant resolution
- Tenant is the root entity and should not be filtered by tenant context
- Name length balances business needs with database efficiency

**Trade-offs**:
- String identifier vs. GUID: String chosen for human-readable subdomains
- No soft delete: Tenant deletion is permanent (business decision)

---

### Venue Entity - 2025-11-28
**Migration**: `20251128154725_AddVenueEntity`
**Related Files**: 
- [`src/GloboTicket.Infrastructure/Data/Configurations/VenueConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/VenueConfiguration.cs)

**Decisions Made**:
- Applied `HasQueryFilter` for tenant isolation via direct `TenantId` property
- Owned entity for `Address` using `OwnsOne` configuration
- Index on `TenantId` for query performance
- Required fields: Name, Address components

**Rationale**:
- Direct TenantId property enables query filter in both unit and integration tests
- Address as owned entity enforces value object pattern (no separate table)
- TenantId index critical for filtered queries on large datasets
- Required fields enforce domain invariants at database level

**Trade-offs**:
- Owned entity vs. separate table: Owned chosen for simplicity (Address has no independent lifecycle)
- Index overhead on writes acceptable for read-heavy venue queries

**Related Decisions**: [Tenant Entity](#tenant-entity---2025-11-22)

---

### Act Entity - 2025-11-28
**Migration**: `20251128155742_AddActEntity`
**Related Files**: 
- [`src/GloboTicket.Infrastructure/Data/Configurations/ActConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/ActConfiguration.cs)

**Decisions Made**:
- Applied `HasQueryFilter` for tenant isolation via direct `TenantId` property
- Index on `TenantId` for query performance
- Required `Name` field with max length 200

**Rationale**:
- Consistent with Venue entity pattern for multi-tenancy
- TenantId index enables efficient filtering in act listings
- Name length matches Tenant entity for consistency

**Trade-offs**:
- Same pattern as Venue for consistency across multi-tenant entities

**Related Decisions**: [Venue Entity](#venue-entity---2025-11-28), [Tenant Entity](#tenant-entity---2025-11-22)

### Customer Entity - 2025-12-19
**Migration**: `20251219030537_AddCustomerEntity`
**Related Files**:
- [`src/GloboTicket.Infrastructure/Data/Configurations/CustomerConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/CustomerConfiguration.cs)

**Decisions Made**:
- Applied `HasQueryFilter` for tenant isolation via direct `TenantId` property
- Composite alternate key on `(TenantId, CustomerGuid)` for multi-tenant uniqueness
- Index on `CustomerGuid` for GUID-based queries
- Index on `TenantId` for query performance
- Required fields: Name (max 250 chars)
- Cascade delete from Tenant

**Rationale**:
- Direct TenantId property maintains consistency with Venue/Act pattern
- Composite alternate key prevents duplicate CustomerGuids within a tenant
- CustomerGuid index enables efficient lookups by public identifier
- Name field consolidates customer identification (simplified from FirstName/LastName)
- Cascade delete ensures customer data removed when tenant deleted

**Trade-offs**:
- Single Name field vs. FirstName/LastName: Simplified for initial implementation
- Composite key adds constraint overhead but critical for data integrity

**Related Decisions**: [Venue Entity](#venue-entity---2025-11-28), [BillingAndShippingAddress Migration](#customer-addresses---2026-01-02)

---

### Customer Addresses - 2026-01-02
**Migration**: `20260102031130_BillingAndShippingAddress`
**Related Files**:
- [`src/GloboTicket.Infrastructure/Data/Configurations/CustomerConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/CustomerConfiguration.cs)

**Decisions Made**:
- Migrated from `OwnsOne` to `ComplexProperty` for address configuration (EF Core 8+ pattern)
- BillingAddress as required complex type with all fields required except StreetLine2
- ShippingAddress as optional complex type (nullable at entity level)
- Column naming: Prefixed with `Billing` or `Shipping` (e.g., `BillingStreetLine1`)
- Max lengths: StreetLine1/2 (200), City/State/Country (100), PostalCode (20)

**Rationale**:
- ComplexProperty is the modern EF Core 8+ approach replacing OwnsOne for value objects
- Required BillingAddress enforces complete billing information for transactions
- Optional ShippingAddress supports "ship to billing address" scenarios
- Explicit column naming prevents ambiguity and improves database readability
- Max lengths balance international address formats with database efficiency

**Trade-offs**:
- ComplexProperty vs. OwnsOne: ComplexProperty chosen for EF Core 8+ best practices
- Optional ShippingAddress increases nullable columns but provides flexibility
- All ShippingAddress fields nullable at DB level (EF limitation with optional complex types)
- Larger row size (12 additional columns) vs. separate table: Inline chosen for query performance

**Related Decisions**: [Customer Entity](#customer-entity---2025-12-19), [Venue Entity](#venue-entity---2025-11-28) (owned entity pattern)

### Show Entity - 2026-01-03
**Migration**: `20260103020953_AddShowEntity`
**Related Files**:
- [`src/GloboTicket.Infrastructure/Data/Configurations/ShowConfiguration.cs`](../src/GloboTicket.Infrastructure/Data/Configurations/ShowConfiguration.cs)
- [`src/GloboTicket.Infrastructure/Services/ShowService.cs`](../src/GloboTicket.Infrastructure/Services/ShowService.cs)

**Decisions Made**:
- No direct TenantId property (tenant isolation via Act relationship)
- Unique index on `ShowGuid` for public identifier lookups
- Composite index on `(VenueId, StartTime)` for nearby shows query optimization
- Index on `ActId` for listing shows by act
- `StartTime` as `DateTimeOffset` to preserve timezone information
- Required fields: ShowGuid, VenueId, ActId, TicketCount, StartTime
- Venue relationship: Cascade delete
- Act relationship: Restrict delete (SQL Server multiple cascade path limitation)

**Rationale**:
- Show inherits tenant context through Act relationship (no redundant TenantId)
- ShowGuid unique index enables efficient public API lookups
- Composite (VenueId, StartTime) index optimizes "shows at this venue in date range" queries
- DateTimeOffset preserves timezone for international venues
- Restrict on Act prevents SQL Server error (both Act and Venue cascade from Tenant)
- Application layer must handle Act deletion cascade to Shows

**Trade-offs**:
- No direct TenantId: Cleaner model but requires navigation property for query filter
- Restrict on Act vs. Cascade: SQL Server limitation forces application-level cascade logic
- Composite index adds write overhead but critical for nearby shows feature
- DateTimeOffset vs. DateTime: Larger storage (10 bytes vs 8) but timezone-aware

**Related Decisions**: [Act Entity](#act-entity---2025-11-28), [Venue Entity](#venue-entity---2025-11-28)

## Patterns and Conventions

### Multi-Tenancy Isolation

**Direct TenantId Property** (Venue, Act, Customer):
```csharp
builder.HasQueryFilter(e => e.TenantId == _tenantContext.CurrentTenantId);
builder.HasIndex(e => e.TenantId);
```
- Use for entities that directly belong to a tenant
- Enables query filtering in both unit tests (in-memory) and integration tests
- Always add index on TenantId for query performance

**Navigation Property Filtering** (Show):
```csharp
builder.HasQueryFilter(s => s.Act.TenantId == _tenantContext.CurrentTenantId);
```
- Use when entity inherits tenant context through parent relationship
- Avoids redundant TenantId storage
- Requires integration tests (not supported by EF Core in-memory provider)

### Index Strategy

- **TenantId**: Always index on multi-tenant entities with direct TenantId property
- **Foreign Keys**: Index all foreign keys for frequently joined relationships
- **Public Identifiers**: Unique index on GUID columns (e.g., ShowGuid, CustomerGuid)
- **Composite Indexes**: Create for common query patterns (e.g., `(VenueId, StartTime)` for date-range queries)
- **Trade-off**: Indexes improve read performance but add write overhead and storage

### Complex Types (EF Core 8+)

**Required Complex Type** (Customer.BillingAddress):
```csharp
builder.ComplexProperty(c => c.BillingAddress, address =>
{
    address.Property(a => a.StreetLine1)
        .IsRequired()
        .HasMaxLength(200)
        .HasColumnName("BillingStreetLine1");
    // ... other properties
});
```

**Optional Complex Type** (Customer.ShippingAddress):
```csharp
builder.ComplexProperty(c => c.ShippingAddress, address =>
{
    // Properties configured same as required, but entity-level is nullable
});
```

- Use `ComplexProperty` for value objects (replaces `OwnsOne` in EF Core 8+)
- Prefix column names explicitly (e.g., `BillingStreetLine1`, `ShippingCity`)
- Keep complex types simple (no navigation properties or complex relationships)
- Optional complex types result in all columns being nullable at DB level

### Decimal Precision

- **Financial amounts**: `HasPrecision(18, 2)` - Standard for currency
- **Percentages**: `HasPrecision(5, 2)` - Supports 0.00% to 999.99%
- **Quantities**: Use integer types (int, long) when fractional values not needed

### Cascade Behavior

- **Parent-child relationships**: `OnDelete(DeleteBehavior.Cascade)`
  - Example: Tenant → Customer, Venue → Show
- **Optional relationships**: `OnDelete(DeleteBehavior.SetNull)`
  - Use when relationship can be broken without deleting child
- **Independent entities**: `OnDelete(DeleteBehavior.Restrict)`
  - Use to prevent accidental deletion
- **SQL Server Limitation**: Multiple cascade paths not allowed
  - Example: Show → Act (Restrict) and Show → Venue (Cascade) because both Act and Venue cascade from Tenant
  - Application layer must handle cascade logic when Restrict is forced

### String Length Conventions

- **Names** (Tenant, Act, Venue): 200 characters
- **Customer Name**: 250 characters (full name in single field)
- **Street Addresses**: 200 characters per line
- **City/State/Country**: 100 characters
- **Postal Code**: 20 characters (international formats)
- **Email**: 256 characters (RFC 5321 standard)

### Timestamp Fields

- **DateTimeOffset**: Use for user-facing dates/times (preserves timezone)
  - Example: Show.StartTime
- **DateTime**: Use for audit fields (CreatedAt, UpdatedAt)
  - Stored in UTC, timezone not relevant for audit trail
