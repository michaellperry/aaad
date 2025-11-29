# Playwright Test User Setup

## Overview

The Playwright end-to-end tests now use a dedicated test user with its own isolated tenant to ensure complete data isolation from production and smoke test environments.

## Configuration

### Test User Credentials

- **Username**: `playwright`
- **Password**: `playwright123`
- **Tenant Identifier**: `playwright-test` (auto-created on first login)

### User Configuration

The Playwright test user is configured in `src/GloboTicket.API/appsettings.json`:

```json
{
  "Username": "playwright",
  "Password": "playwright123",
  "TenantIdentifier": "playwright-test"
}
```

### Test Fixtures

The test credentials are defined in `src/GloboTicket.Web/tests/fixtures/auth.ts`:

```typescript
export const TEST_CREDENTIALS = {
  username: 'playwright',
  password: 'playwright123',
} as const;
```

## Benefits

1. **Data Isolation**: Playwright tests operate in their own tenant (`playwright-test`), completely isolated from:
   - Production data (tenant: `production`)
   - Smoke test data (tenant: `smoke-test`)

2. **Test Reliability**: Tests can create, modify, and delete data without affecting other environments or test runs.

3. **Parallel Execution**: Multiple test runs can execute safely without interfering with each other, as they all use the same isolated tenant.

4. **Clean State**: The `playwright-test` tenant can be easily cleaned/reset without affecting other tenants.

5. **Deterministic**: Tests use the tenant identifier (`playwright-test`) rather than a numeric ID, ensuring consistent behavior across environments.

## Tenant Isolation

The authentication system automatically creates the `playwright-test` tenant on first login. The tenant isolation is enforced at the database query level through EF Core query filters.

### How It Works

1. User logs in with `playwright` credentials
2. Authentication endpoint looks up the user and tenant identifier
3. If the tenant doesn't exist, it's auto-created with:
   - TenantIdentifier: "playwright-test"
   - Name: "Playwright Test"
   - Slug: "playwright-test"
   - IsActive: true
4. Session includes the tenant ID (auto-generated) in claims
5. All database queries are automatically filtered to this tenant

## Running Tests

All existing test commands continue to work as before:

```bash
# Run all Playwright tests
npm run test:e2e

# Run with UI
npm run test:e2e:ui

# Run in headed mode
npm run test:e2e:headed

# Debug tests
npm run test:e2e:debug

# View test report
npm run test:e2e:report
```

## Files Updated

1. **Configuration**:
   - `src/GloboTicket.API/appsettings.json` - Added playwright user

2. **Test Files**:
   - `src/GloboTicket.Web/tests/fixtures/auth.ts` - Updated test credentials

3. **Frontend**:
   - `src/GloboTicket.Web/src/components/organisms/LoginForm.tsx` - Added playwright to test credentials hint

4. **Documentation**:
   - `README.md` - Added playwright user to credentials table
   - `GETTING_STARTED.md` - Added playwright user to setup guide
   - `docs/tenant-isolation.md` - Added playwright tenant documentation
   - `src/GloboTicket.Web/tests/README.md` - Updated test credentials documentation

## Security Notes

⚠️ **Development Only**: The `playwright` user credentials are for development and testing only. Do not use these credentials in production environments.

## Cleanup

To reset the Playwright test data:

```sql
-- Clear all data for the Playwright tenant
DELETE FROM Venue WHERE TenantId = (SELECT Id FROM Tenant WHERE TenantIdentifier = 'playwright-test');
DELETE FROM Act WHERE TenantId = (SELECT Id FROM Tenant WHERE TenantIdentifier = 'playwright-test');
-- Add other entity deletes as needed

-- Or delete the entire tenant (will be recreated on next login)
DELETE FROM Tenant WHERE TenantIdentifier = 'playwright-test';
```

## Verification

To verify the setup is working:

1. Start the API: `cd src/GloboTicket.API && dotnet run`
2. Start the Web app: `cd src/GloboTicket.Web && npm run dev`
3. Run a single test: `npx playwright test tests/auth/login.spec.ts --headed`
4. Observe that the test logs in with the `playwright` user
5. Check the database to see the `playwright-test` tenant was created

## Additional Resources

- [Test Suite Documentation](src/GloboTicket.Web/tests/TEST_SUITE.md)
- [Test Execution Guide](src/GloboTicket.Web/tests/EXECUTION_GUIDE.md)
- [Tenant Isolation Documentation](docs/tenant-isolation.md)

