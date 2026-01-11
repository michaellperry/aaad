# Authentication and Fixtures

Use when sharing auth state or multi-tenant setup across tests.

## Auth Setup
- Create a dedicated `.setup.ts` to log in once and save `storageState` under `playwright/.auth/user.json`.
- In specs, apply `test.use({ storageState: 'playwright/.auth/user.json' })`.

```typescript
import { test as setup, expect } from '@playwright/test'
const authFile = 'playwright/.auth/user.json'
setup('authenticate', async ({ page }) => {
  await page.goto('/login')
  await page.getByLabel('Email').fill('test@example.com')
  await page.getByLabel('Password').fill('password123')
  await page.getByRole('button', { name: 'Sign In' }).click()
  await expect(page).toHaveURL('/dashboard')
  await page.context().storageState({ path: authFile })
})
```

## Multi-Tenant Fixture
- Extend `test` with a tenant fixture that creates tenant + user, logs in, yields context, and cleans up.

```typescript
import { test as base } from '@playwright/test'
type TenantFixtures = { tenantContext: { tenantId: string; tenantName: string; userEmail: string } }
export const test = base.extend<TenantFixtures>({
  tenantContext: async ({ page }, use) => {
    const tenantId = await createTestTenant()
    const userEmail = `test-${tenantId}@example.com`
    await createTenantUser(tenantId, userEmail)
    await page.goto('/login')
    await page.getByLabel('Email').fill(userEmail)
    await page.getByLabel('Password').fill('password123')
    await page.getByRole('button', { name: 'Sign In' }).click()
    await use({ tenantId, tenantName: `Test Tenant ${tenantId}`, userEmail })
    await cleanupTestTenant(tenantId)
  }
})
```
