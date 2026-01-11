# Flakiness Prevention

Use when hardening tests against timing and intermittent failures.

## Animation and Timing
- Set `use.reducedMotion: 'reduce'` in config to minimize animation timing issues.
- Use per-test timeouts only when necessary; prefer defaults and targeted waits.

```typescript
export default defineConfig({
  use: { reducedMotion: 'reduce' },
  timeout: 30000,
  retries: process.env.CI ? 2 : 0,
})
```

## Retry and Recovery
- Wrap flaky flows with `expect().toPass` and retry intervals.
- Simulate and recover from network errors with `page.route` + `unroute`.

```typescript
await expect(async () => {
  await page.getByRole('button', { name: 'Refresh' }).click()
  await expect(page.getByText('Data refreshed')).toBeVisible({ timeout: 5000 })
}).toPass({ intervals: [1000, 2000, 3000], timeout: 10000 })
```
