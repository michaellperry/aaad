# Tenant Isolation Strategy

## Overview

GloboTicket uses tenant-based data isolation to enable multiple data contexts within the same environment's database. This allows scenarios such as running smoke tests in production without affecting production data.

## Concepts

### Environments
An **environment** is a complete deployment of the GloboTicket solution, including:
- Application servers
- Database instance
- Configuration and infrastructure

Examples: Development, Staging, Production

### Tenants
A **tenant** provides data isolation within an environment's database:
- Multiple tenants can coexist in the same database
- Complete data isolation between tenants
- Each tenant has its own data context
- No cross-tenant data access possible

## Use Case: Smoke Testing in Production

### Scenario
After deploying to a Production environment, you need to validate the deployment without affecting production data.

### Solution
Use tenant isolation within the Production environment's database:

1. **Production Environment** contains:
   - **Production Tenant** (ID: 1): Live production data
   - **Smoke Test Tenant** (ID: 2): Validation test data

2. **Deployment Process**:
   ```
   1. Deploy application to Production environment
   2. Run smoke tests using "smoke" user credentials
   3. All smoke test operations isolated to Tenant ID 2
   4. Production tenant data (ID: 1) completely unaffected
   ```

3. **Benefits**:
   - Validate against production infrastructure
   - Test with production-like performance
   - Zero risk to production data
   - No additional infrastructure costs

## Tenant Setup

### Default Tenants

Each environment typically includes:

- **Production Tenant** (ID: 1)
  - Purpose: Live production data
  - User: `prod` / `prod123`
  
- **Smoke Test Tenant** (ID: 2)
  - Purpose: Post-deployment validation
  - User: `smoke` / `smoke123`

### Adding New Tenants

To add a new tenant to an environment:

1. Create tenant record in database:
   ```sql
   INSERT INTO Tenants (Name, Slug, IsActive, CreatedAt)
   VALUES ('Integration Test', 'integration-test', 1, GETUTCDATE());
   ```

2. Add user configuration in `appsettings.json`:
   ```json
   {
     "Username": "integration",
     "Password": "integration123",
     "TenantId": 3
   }
   ```

3. Use new credentials to access the tenant's isolated data context

## Technical Implementation

### How It Works

1. **User Authentication**: User logs in with credentials
2. **Tenant Mapping**: System maps user to tenant ID via configuration
3. **Context Setting**: Middleware sets tenant context for the request
4. **Query Filtering**: EF Core automatically filters all queries by tenant ID
5. **Data Isolation**: User only sees data for their assigned tenant

### Database Structure

- All tenant-isolated entities implement `ITenantEntity`
- `TenantId` column on all tenant-isolated tables
- EF Core global query filters enforce isolation
- No application code changes needed for new tenants

## Best Practices

1. **Naming Conventions**: Use clear tenant names (Production, Smoke Test, etc.)
2. **User Mapping**: One user per tenant for clarity
3. **Documentation**: Document tenant purposes and usage
4. **Monitoring**: Track tenant usage and data volumes
5. **Security**: Ensure proper access controls per tenant

## Examples

### Running Smoke Tests

```bash
# Login as smoke test user
curl -X POST http://production-api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"smoke","password":"smoke123"}'

# Execute smoke test scenarios
# All operations isolated to Smoke Test tenant (ID: 2)
```

### Production Operations

```bash
# Login as production user
curl -X POST http://production-api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"prod","password":"prod123"}'

# All operations isolated to Production tenant (ID: 1)
```

## Summary

Tenant isolation enables:
- Multiple data contexts within the same database
- Safe testing scenarios in production environments
- Complete data isolation between tenants
- Cost-effective infrastructure utilization
- Flexible data segregation strategies

