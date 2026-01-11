# Test Structure

Use when organizing Playwright specs and fixtures.

## AAA and Naming
- Keep Arrange/Act/Assert visually separated with comments or whitespace.
- Name tests by behavior + condition (e.g., "redirects to detail after create").

## Isolation
- Use `test.beforeEach`/`test.afterEach` to reset state and data per test.
- Prefer API-driven setup/cleanup over UI where possible.

## Example
```typescript
import { test, expect } from '@playwright/test'

test('creates venue with valid data', async ({ page }) => {
  await page.goto('/venues')
  await page.getByRole('button', { name: 'Add Venue' }).click()
  await page.getByLabel('Venue Name').fill('Madison Square Garden')
  await page.getByLabel('Address').fill('4 Pennsylvania Plaza, New York, NY 10001')
  await page.getByLabel('Capacity').fill('20000')
  await page.getByRole('button', { name: 'Create Venue' }).click()
  await expect(page.getByText('Venue created successfully')).toBeVisible()
})
```
