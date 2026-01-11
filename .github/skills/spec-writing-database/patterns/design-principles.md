# Database Design Principles

Multi-tenancy, data integrity, performance, and audit requirements.

## Multi-Tenancy Requirements
- All domain entities include TenantId FK; child entities inherit tenant context from parent relationships
- Unique constraints scoped within tenant boundaries: `(TenantId, BusinessKey)`
- Indexes optimized for tenant-filtered queries: TenantId as first column
- Audit timestamps: CreatedAt, UpdatedAt (UTC); use GETUTCDATE() defaults
- Soft delete patterns where business requires history; cascade delete rules respect relationships

## Performance Considerations
- Integer primary keys for performance; GUID alternate keys for external APIs
- Indexes include TenantId as first column; INCLUDE columns for common SELECT projections
- Filtered indexes for active records only; covering indexes for frequent query patterns
- Partitioning strategy for large tables; archive strategy for historical data

## Data Quality Standards
- Check constraints for business rules; NOT NULL for required business data
- Default values for system fields; appropriate data types and lengths
- Creation and modification timestamps; user tracking for data changes
- Soft delete with deletion timestamps; change log tables for critical entities
