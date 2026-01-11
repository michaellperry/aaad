# Test Data Management

Use when setting up data faster via APIs and keeping suites isolated.

## API-Based Creation
- Prefer API calls over UI for setup; assert response `.ok()`.

```typescript
import { request, APIRequestContext } from '@playwright/test'
export class TestDataHelper {
  constructor(private apiContext: APIRequestContext) {}
  async createTestVenue(data: { name: string; address: string; capacity: number; venueTypeId: string }) {
    const response = await this.apiContext.post('/api/venues', { data })
    expect(response.ok()).toBeTruthy()
    return await response.json()
  }
  async deleteTestVenue(venueId: string) {
    await this.apiContext.delete(`/api/venues/${venueId}`)
  }
}
```

## Reset and Seeding
- Use global setup/teardown to reset DB and seed reference data before suites.
- Configure Playwright projects to run setup before specs and share auth state.

```typescript
export default defineConfig({
  projects: [
    { name: 'setup', testMatch: /.*\.setup\.ts/ },
    { name: 'e2e-tests', use: { storageState: 'playwright/.auth/user.json' }, dependencies: ['setup'], testMatch: /.*\.spec\.ts/ }
  ],
  globalSetup: require.resolve('./tests/global.setup.ts'),
  globalTeardown: require.resolve('./tests/global.teardown.ts'),
})
```
