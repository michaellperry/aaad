# Parallel Execution

Use when enabling parallel mode and keeping tests isolated.

- Configure `test.describe.configure({ mode: 'parallel' })` only when data isolation is guaranteed.
- Generate unique data per test (e.g., suffix with timestamp/uuid) and isolate tenants.

```typescript
test.describe.configure({ mode: 'parallel' })

test('create venue - tenant A', async ({ page }) => {
  const tenantId = 'tenant-a-' + Date.now()
  await setupTenantContext(page, tenantId)
  await createVenueTest(page, `Venue-${tenantId}`)
})
```
