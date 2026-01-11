# Indexes

Specify non-clustered indexes for query performance, uniqueness, and foreign key lookups.

## Performance Indexes
```sql
-- Multi-tenant query optimization: TenantId first column
CREATE INDEX IX_Venues_TenantId_IsActive 
    ON Venues (TenantId, IsActive) 
    INCLUDE (Name, City, Capacity);

CREATE INDEX IX_Shows_TenantId_EventDate 
    ON Shows (TenantId, EventDate DESC) 
    INCLUDE (VenueId, ActId, TicketPrice, IsPublished);

-- Filtered index for active records only
CREATE INDEX IX_Venues_TenantId_Name 
    ON Venues (TenantId) 
    INCLUDE (Name, City)
    WHERE IsActive = 1;

-- Foreign key indexes
CREATE INDEX IX_Shows_VenueId ON Shows (VenueId);
CREATE INDEX IX_Shows_ActId ON Shows (ActId);
CREATE INDEX IX_TicketOffers_ShowId ON TicketOffers (ShowId);
```

## Unique Constraint Indexes
```sql
-- Unique composite indexes
CREATE UNIQUE INDEX UQ_Venues_TenantGuid 
    ON Venues (TenantId, VenueGuid);

-- Filtered unique index for conditional uniqueness
CREATE UNIQUE INDEX UQ_Venues_TenantName 
    ON Venues (TenantId, Name)
    WHERE IsActive = 1;
```

## Query-Specific Indexes
```sql
-- Dashboard queries
CREATE INDEX IX_Shows_TenantId_IsPublished_EventDate 
    ON Shows (TenantId, IsPublished, EventDate DESC)
    INCLUDE (VenueId, ActId, AvailableTickets);

-- Reporting queries
CREATE INDEX IX_TicketOffers_TenantId_CreatedAt 
    ON TicketOffers (TenantId, CreatedAt DESC)
    INCLUDE (ShowId, Price, Quantity, IsActive);
```

Always lead with TenantId; use INCLUDE for covering indexes; filter WHERE for active records.
