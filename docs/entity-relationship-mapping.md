# Entity Relationship Mapping & Multi-Tenant Strategy

## Entity Relationship Diagram

```
Tenant (1) ──────────────────── (N) Venue
                                   │
                                   │ (1:N - Venue to Show)
                                   ▼
                                  Show (N) ──────── (N:1) Act
                                   │
                                   │ (1:N - Show to TicketSale)
                                   ▼
                              TicketSale
```

## Detailed Relationship Analysis

### 1. Venue → Show (One-to-Many)
- **Foreign Key**: Show.VenueId → Venue.VenueId
- **Navigation**: Show.Venue, Venue.Shows (implicit)
- **Cascade Delete**: Yes (from migrations)
- **Business Rule**: Each show must be associated with a venue

### 2. Act → Show (One-to-Many)
- **Foreign Key**: Show.ActId → Act.ActId
- **Navigation**: Show.Act, Act.Shows (implicit)
- **Cascade Delete**: Yes (from migrations)
- **Business Rule**: Each show must be associated with an act

### 3. Show → TicketSale (One-to-Many)
- **Foreign Key**: TicketSale.ShowId → Show.ShowId
- **Navigation**: TicketSale.Show, Show.TicketSales (explicit)
- **Cascade Delete**: Yes (from migrations)
- **Business Rule**: Ticket sales are tied to specific shows

## Multi-Tenant Isolation Strategy

### Entity Isolation Requirements

#### High Isolation (Must be Tenant-Specific)
1. **Venue** - Each tenant manages their own venues
2. **Show** - Shows are specific to tenant's venues and acts
3. **TicketSale** - Sales data must be completely isolated

#### Shared Reference Data (Consider Tenant Boundary)
1. **Act** - Could be shared across tenants (like a artist database)
   - **Recommendation**: Make tenant-specific for data integrity
   - **Rationale**: Avoids cross-tenant data contamination

### TenantId Implementation Strategy

#### All Entities Should Implement ITenantEntity
```csharp
public class Venue : Entity, ITenantEntity
{
    public int TenantId { get; set; }
    // ... existing properties
}
```

#### Relationship Constraints with Multi-Tenancy

**Approach 1: Hard Foreign Key Constraints (Recommended)**
- Add TenantId to all foreign key tables
- Create compound foreign keys: (TenantId, ForeignKeyId)
- Ensures referential integrity across tenants
- More complex but safer

**Approach 2: Soft Foreign Key Validation**
- Keep existing foreign key structure
- Use query filters to prevent cross-tenant access
- Simpler implementation
- Risk of data corruption if query filters fail

**Recommended**: Approach 1 with compound foreign keys

### Global Query Filter Strategy

#### Current Target Project Implementation
```csharp
// Applied to all ITenantEntity types
entity => _tenantContext.CurrentTenantId == null || entity.TenantId == _tenantContext.CurrentTenantId
```

#### Implications for New Entities
- All Venue, Act, Show, TicketSale entities will be automatically filtered
- Cross-tenant data access prevented at database level
- Requires proper TenantContext setup in all operations

## Migration Strategy

### Phase 1: Entity Foundation (TDD Red Phase)

#### 1. Create Base Multi-Tenant Entities
```csharp
// Target structure for all entities
public abstract class MultiTenantEntity : Entity, ITenantEntity
{
    public int TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### 2. Implement Core Entities
- Venue with TenantId and geospatial support
- Act with TenantId
- Show with TenantId and navigation properties
- TicketSale with TenantId

#### 3. EF Core Configurations
- Update all configurations to include TenantId
- Set up compound foreign keys
- Configure global query filters
- Maintain GUID patterns for public APIs

### Phase 2: Service Layer Migration

#### 1. Update Domain Services
- Modify PromotionService to work with tenant context
- Update SalesService for tenant isolation
- Ensure all queries respect tenant boundaries
- Add tenant validation in service methods

#### 2. Constructor Updates
```csharp
// Updated constructor pattern with tenant context
public Show(Venue venue, Act act, int tenantId)
{
    Venue = venue;
    Act = act;
    TenantId = tenantId;
}
```

### Phase 3: API Layer Updates

#### 1. Controller Modifications
- Update all controllers to work with tenant context
- Add tenant validation in controller actions
- Update routing to include tenant awareness
- Ensure GUID patterns are preserved

#### 2. Model Updates
- Add TenantId to API models where needed
- Update validation to include tenant context
- Ensure proper serialization/deserialization

## Technical Implementation Details

### Database Schema Changes

#### Venue Table
```sql
ALTER TABLE Venue ADD TenantId INT NOT NULL DEFAULT 1;
ALTER TABLE Venue ADD CONSTRAINT FK_Venue_Tenant FOREIGN KEY (TenantId) REFERENCES Tenant(Id);
```

#### Act Table
```sql
ALTER TABLE Act ADD TenantId INT NOT NULL DEFAULT 1;
ALTER TABLE Act ADD CONSTRAINT FK_Act_Tenant FOREIGN KEY (TenantId) REFERENCES Tenant(Id);
```

#### Show Table
```sql
ALTER TABLE Show ADD TenantId INT NOT NULL DEFAULT 1;
-- Compound foreign keys
ALTER TABLE Show DROP CONSTRAINT FK_Show_Venue_VenueId;
ALTER TABLE Show DROP CONSTRAINT FK_Show_Act_ActId;
ALTER TABLE Show ADD CONSTRAINT FK_Show_Venue_Tenant 
    FOREIGN KEY (TenantId, VenueId) REFERENCES Venue(TenantId, VenueId);
ALTER TABLE Show ADD CONSTRAINT FK_Show_Act_Tenant 
    FOREIGN KEY (TenantId, ActId) REFERENCES Act(TenantId, ActId);
ALTER TABLE Show ADD CONSTRAINT FK_Show_Tenant FOREIGN KEY (TenantId) REFERENCES Tenant(Id);
```

#### TicketSale Table
```sql
ALTER TABLE TicketSale ADD TenantId INT NOT NULL DEFAULT 1;
-- Compound foreign key to Show
ALTER TABLE TicketSale DROP CONSTRAINT FK_TicketSale_Show_ShowId;
ALTER TABLE TicketSale ADD CONSTRAINT FK_TicketSale_Show_Tenant 
    FOREIGN KEY (TenantId, ShowId) REFERENCES Show(TenantId, ShowId);
ALTER TABLE TicketSale ADD CONSTRAINT FK_TicketSale_Tenant FOREIGN KEY (TenantId) REFERENCES Tenant(Id);
```

### Query Filter Testing Strategy

#### Required Tests
1. **Tenant Isolation Tests**
   - Verify entities from different tenants are not visible
   - Test cross-tenant query prevention
   - Validate automatic TenantId assignment

2. **Relationship Integrity Tests**
   - Test compound foreign key constraints
   - Verify navigation properties work correctly
   - Validate cascade delete behavior per tenant

3. **Performance Tests**
   - Verify query filters don't significantly impact performance
   - Test complex geospatial queries with tenant filters
   - Validate indexing strategy for compound keys

## Risk Assessment & Mitigation

### High Risk Areas
1. **Geospatial Queries with Tenant Filters**
   - Risk: Performance degradation with compound location + tenant queries
   - Mitigation: Strategic indexing on (TenantId, Location)

2. **Complex Join Operations**
   - Risk: Incorrect tenant isolation in multi-table joins
   - Mitigation: Comprehensive testing of all service methods

3. **Migration Data Integrity**
   - Risk: Data corruption during migration to tenant-aware schema
   - Mitigation: Staged migration with validation steps

### Medium Risk Areas
1. **API Compatibility**
   - Risk: Breaking changes in public APIs
   - Mitigation: Preserve GUID patterns and method signatures

2. **Existing Service Logic**
   - Risk: Business logic breaking with tenant context changes
   - Mitigation: Incremental refactoring with comprehensive tests

## Success Criteria

### Technical Success
- [ ] All entities implement ITenantEntity interface
- [ ] Global query filters prevent cross-tenant data access
- [ ] Compound foreign key constraints enforce referential integrity
- [ ] All existing functionality preserved with tenant isolation

### Business Success
- [ ] Complete data isolation between tenants
- [ ] No performance degradation in common operations
- [ ] Seamless integration with existing Tenant management
- [ ] Maintained support for complex geospatial queries