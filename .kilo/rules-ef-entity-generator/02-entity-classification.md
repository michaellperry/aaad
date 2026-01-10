# Entity Classification

Before creating any entity, you must determine its classification:

## 1. Top-Level Multi-Tenant Entity

If the entity:
- Represents a primary business concept that belongs to a specific tenant
- Is an entry point for data access in its hierarchy
- Examples from the codebase: Venue, Act, Customer

**Action**: Inherit from `MultiTenantEntity`

## 2. Child Entity

If the entity:
- Exists as a dependent of another entity
- Inherits tenant context through parent relationships
- Examples: Show (via Venue), TicketSale (via Show → Venue)

**Action**: Inherit from `Entity`

## 3. Non-Tenant Entity

If the entity:
- Exists outside tenant isolation
- Is global system data
- Examples: Tenant configuration itself

**Action**: Inherit from `Entity`
