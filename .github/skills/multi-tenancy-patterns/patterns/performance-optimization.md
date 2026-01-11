# Performance Optimization

Indexing and query strategies for tenant-filtered data.

## Indexing Strategy
```sql
-- Composite indexes: tenant_id as first column
CREATE INDEX IX_Venues_TenantId_Name ON Venues (tenant_id, name);
CREATE INDEX IX_Acts_TenantId_VenueId ON Acts (tenant_id, venue_id);
```

## Query Optimization
- Always include tenant_id in WHERE clauses
- Use covering indexes for tenant-specific queries
- Consider partitioning by tenant_id for large datasets
- Implement query result caching per tenant

Composite indexes with tenant_id as the leading column enable efficient tenant-scoped queries.
