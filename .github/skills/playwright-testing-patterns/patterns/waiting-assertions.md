# Waiting and Assertions

Use when coordinating navigation, network, and UI waits.

## Auto-Waiting
- Prefer web-first assertions (`expect(locator).toBeVisible()` / `not.toBeVisible()`), URL assertions, and `waitForResponse` with predicates.
- Avoid fixed `waitForTimeout` unless diagnosing locally.

```typescript
const responsePromise = page.waitForResponse(resp => resp.url().includes('/api/venues') && resp.status() === 201)
await page.getByRole('button', { name: 'Create Venue' }).click()
await responsePromise
await expect(page).toHaveURL(/\/venues\/[a-f0-9-]+/)
```

## Network State
- Use `waitForLoadState('networkidle')` before interacting after navigation.
- For mutations, wait on the specific request before asserting UI.
