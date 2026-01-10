---
name: playwright-testing-patterns
description: End-to-end testing patterns using Playwright for web application testing, including test structure, selectors, fixtures, and flakiness prevention. Use when writing or debugging Playwright tests for multi-tenant applications.
---

# Playwright Testing Patterns

Best practices for E2E testing with Playwright focusing on reliable, maintainable test suites with proper isolation and minimal flakiness.

## Role & Responsibilities

The Test Automation Engineer ensures features work as intended and prevents regression:
- **Input**: Working feature implementation and User Stories with acceptance criteria
- **Output**: Robust test suite in `tests/` directory with proper coverage
- **Goal**: High confidence testing with minimal flakiness and fast execution

## Test Structure Patterns

### AAA Test Pattern
**Structure tests with clear Arrange-Act-Assert sections**

```typescript
import { test, expect } from '@playwright/test'

test('should create venue successfully when valid data provided', async ({ page }) => {
  // Arrange - Setup test conditions
  await page.goto('/venues')
  await page.getByRole('button', { name: 'Add Venue' }).click()
  
  // Act - Perform the action being tested
  await page.getByLabel('Venue Name').fill('Madison Square Garden')
  await page.getByLabel('Address').fill('4 Pennsylvania Plaza, New York, NY 10001')
  await page.getByLabel('Capacity').fill('20000')
  await page.selectOption('[data-testid="venue-type"]', 'Arena')
  await page.getByRole('button', { name: 'Create Venue' }).click()
  
  // Assert - Verify the expected outcome
  await expect(page.getByText('Venue created successfully')).toBeVisible()
  await expect(page).toHaveURL(/\/venues\/[a-f0-9-]+/)
  await expect(page.getByText('Madison Square Garden')).toBeVisible()
})
```

### Test Isolation
**Each test should run independently with proper cleanup**

```typescript
import { test, expect } from '@playwright/test'

test.describe('Venue Management', () => {
  test.beforeEach(async ({ page }) => {
    // Setup for each test - fresh state
    await page.goto('/venues')
    
    // Clear any existing test data (if needed)
    await cleanupTestData()
  })
  
  test.afterEach(async ({ page }) => {
    // Cleanup after each test
    await cleanupTestData()
  })
  
  test('should display empty state when no venues exist', async ({ page }) => {
    // Test starts with clean slate
    await expect(page.getByText('No venues found')).toBeVisible()
    await expect(page.getByRole('button', { name: 'Add Venue' })).toBeVisible()
  })
  
  test('should show venue list when venues exist', async ({ page }) => {
    // Create test data for this specific test
    await createTestVenue('Test Arena')
    await page.reload()
    
    await expect(page.getByText('Test Arena')).toBeVisible()
    await expect(page.getByText('No venues found')).not.toBeVisible()
  })
})
```

### Descriptive Test Naming
```typescript
// ✅ Good - Describes behavior and condition
test('should display validation error when venue name is empty')
test('should redirect to venue details after successful creation')
test('should prevent duplicate venue names within same tenant')
test('should allow editing venue capacity when user has manager role')

// ❌ Bad - Unclear or implementation-focused
test('venue creation')
test('test venue form')
test('POST /api/venues endpoint')
test('VenueService.CreateAsync method')
```

## Selector Patterns

### Selector Priority
**Use selectors in order of user-facing priority**

```typescript
// 1. ✅ BEST - User-facing roles and accessible names
await page.getByRole('button', { name: 'Create Venue' })
await page.getByRole('textbox', { name: 'Venue Name' })
await page.getByRole('combobox', { name: 'Venue Type' })
await page.getByRole('link', { name: 'Edit Venue' })

// 2. ✅ GOOD - Text content users can see
await page.getByText('Venue created successfully')
await page.getByText('Madison Square Garden')
await page.getByLabel('Address')

// 3. ✅ ACCEPTABLE - Placeholder text
await page.getByPlaceholder('Enter venue name')
await page.getByPlaceholder('Search venues...')

// 4. ⚠️ LAST RESORT - Test IDs (when semantic selectors aren't possible)
await page.getByTestId('venue-capacity-input')
await page.getByTestId('venue-form-submit')

// ❌ AVOID - Fragile selectors
await page.locator('.btn-primary')          // CSS classes change
await page.locator('button:nth-child(2)')   // Position-dependent
await page.locator('#submit-123')           // Dynamic IDs
```

### Form Interaction Patterns
```typescript
// ✅ Accessible form filling
test('should create venue with all required fields', async ({ page }) => {
  await page.goto('/venues/new')
  
  // Use labels for form fields
  await page.getByLabel('Venue Name').fill('Apollo Theater')
  await page.getByLabel('Address').fill('253 W 125th St, New York, NY 10027')
  await page.getByLabel('Capacity').fill('1500')
  
  // Select from dropdown
  await page.getByRole('combobox', { name: 'Venue Type' }).click()
  await page.getByRole('option', { name: 'Theater' }).click()
  
  // Submit form
  await page.getByRole('button', { name: 'Create Venue' }).click()
  
  // Verify success
  await expect(page.getByText('Venue created successfully')).toBeVisible()
})

// ✅ Error state testing
test('should show validation errors for invalid form data', async ({ page }) => {
  await page.goto('/venues/new')
  
  // Submit empty form
  await page.getByRole('button', { name: 'Create Venue' }).click()
  
  // Check for validation errors
  await expect(page.getByText('Venue name is required')).toBeVisible()
  await expect(page.getByText('Address is required')).toBeVisible()
  await expect(page.getByText('Capacity must be greater than 0')).toBeVisible()
})
```

## Waiting and Assertions

### Auto-Waiting Patterns
**Rely on Playwright's built-in waiting mechanisms**

```typescript
// ✅ Good - Web-first assertions with auto-waiting
await expect(page.getByText('Loading...')).toBeVisible()
await expect(page.getByText('Loading...')).not.toBeVisible()  // Waits for loading to finish
await expect(page.getByText('Venues loaded')).toBeVisible()

// ✅ Good - Wait for specific network responses
const responsePromise = page.waitForResponse(resp => 
  resp.url().includes('/api/venues') && resp.status() === 201
)
await page.getByRole('button', { name: 'Create Venue' }).click()
await responsePromise

// ✅ Good - Wait for navigation
await expect(page).toHaveURL(/\/venues\/[a-f0-9-]+/)

// ❌ Bad - Fixed timeouts
await page.waitForTimeout(2000)  // Flaky and slow

// ❌ Bad - Polling for elements
while (!(await page.getByText('Success').isVisible())) {
  await page.waitForTimeout(100)
}
```

### Network State Management
```typescript
// ✅ Wait for specific API calls to complete
test('should refresh venue list after creation', async ({ page }) => {
  await page.goto('/venues')
  
  // Wait for initial load
  await page.waitForLoadState('networkidle')
  
  // Create venue and wait for API response
  await page.getByRole('button', { name: 'Add Venue' }).click()
  
  const createResponse = page.waitForResponse(
    resp => resp.url().includes('/api/venues') && resp.request().method() === 'POST'
  )
  
  await page.getByLabel('Venue Name').fill('Test Venue')
  await page.getByRole('button', { name: 'Create' }).click()
  
  await createResponse
  
  // Verify venue appears in list
  await expect(page.getByText('Test Venue')).toBeVisible()
})
```

## Authentication and Fixtures

### Authentication Setup
```typescript
// tests/auth.setup.ts
import { test as setup, expect } from '@playwright/test'

const authFile = 'playwright/.auth/user.json'

setup('authenticate', async ({ page }) => {
  await page.goto('/login')
  
  await page.getByLabel('Email').fill('test@example.com')
  await page.getByLabel('Password').fill('password123')
  await page.getByRole('button', { name: 'Sign In' }).click()
  
  // Wait for successful login
  await expect(page).toHaveURL('/dashboard')
  
  // Save authentication state
  await page.context().storageState({ path: authFile })
})

// Individual test file
import { test, expect } from '@playwright/test'

test.use({ storageState: 'playwright/.auth/user.json' })

test('should access protected venue page', async ({ page }) => {
  await page.goto('/venues')
  await expect(page.getByText('Welcome to Venues')).toBeVisible()
})
```

### Multi-Tenant Test Fixtures
```typescript
// tests/fixtures/tenant.ts
import { test as base } from '@playwright/test'

type TenantFixtures = {
  tenantContext: {
    tenantId: string
    tenantName: string
    userEmail: string
  }
}

export const test = base.extend<TenantFixtures>({
  tenantContext: async ({ page }, use) => {
    // Create isolated tenant for test
    const tenantId = await createTestTenant()
    const userEmail = `test-${tenantId}@example.com`
    
    // Setup tenant user
    await createTenantUser(tenantId, userEmail)
    
    // Login as tenant user
    await page.goto('/login')
    await page.getByLabel('Email').fill(userEmail)
    await page.getByLabel('Password').fill('password123')
    await page.getByRole('button', { name: 'Sign In' }).click()
    
    await use({
      tenantId,
      tenantName: `Test Tenant ${tenantId}`,
      userEmail
    })
    
    // Cleanup
    await cleanupTestTenant(tenantId)
  }
})

// Usage in tests
test('should create venue within tenant context', async ({ page, tenantContext }) => {
  await page.goto('/venues')
  
  // Verify tenant isolation
  await expect(page.getByText(tenantContext.tenantName)).toBeVisible()
  
  // Create venue - will be isolated to this tenant
  await page.getByRole('button', { name: 'Add Venue' }).click()
  await page.getByLabel('Venue Name').fill('Tenant Specific Venue')
  await page.getByRole('button', { name: 'Create' }).click()
  
  await expect(page.getByText('Venue created successfully')).toBeVisible()
})
```

## Test Data Management

### API-Based Test Data Creation
```typescript
// tests/helpers/test-data.ts
import { request, APIRequestContext } from '@playwright/test'

export class TestDataHelper {
  constructor(private apiContext: APIRequestContext) {}
  
  async createTestVenue(data: {
    name: string
    address: string
    capacity: number
    venueTypeId: string
  }) {
    const response = await this.apiContext.post('/api/venues', {
      data: {
        name: data.name,
        address: data.address,
        capacity: data.capacity,
        venueTypeId: data.venueTypeId
      }
    })
    
    expect(response.ok()).toBeTruthy()
    return await response.json()
  }
  
  async deleteTestVenue(venueId: string) {
    await this.apiContext.delete(`/api/venues/${venueId}`)
  }
}

// Usage in test
test('should display venue in list after creation', async ({ page, request }) => {
  const testData = new TestDataHelper(request)
  
  // Create venue via API (faster than UI)
  const venue = await testData.createTestVenue({
    name: 'API Created Venue',
    address: '123 Test St',
    capacity: 1000,
    venueTypeId: 'theater-type-id'
  })
  
  try {
    // Test UI display
    await page.goto('/venues')
    await expect(page.getByText('API Created Venue')).toBeVisible()
    
  } finally {
    // Cleanup
    await testData.deleteTestVenue(venue.id)
  }
})
```

### Database Reset Patterns
```typescript
// playwright.config.ts
export default defineConfig({
  projects: [
    {
      name: 'setup',
      testMatch: /.*\.setup\.ts/,
    },
    {
      name: 'e2e-tests',
      use: { storageState: 'playwright/.auth/user.json' },
      dependencies: ['setup'],
      testMatch: /.*\.spec\.ts/,
    }
  ],
  
  // Global setup/teardown
  globalSetup: require.resolve('./tests/global.setup.ts'),
  globalTeardown: require.resolve('./tests/global.teardown.ts'),
})

// tests/global.setup.ts
import { chromium, FullConfig } from '@playwright/test'

async function globalSetup(config: FullConfig) {
  // Reset test database before test run
  await resetTestDatabase()
  
  // Seed reference data
  await seedReferenceData()
}

export default globalSetup
```

## Flakiness Prevention

### Animation and Timing
```typescript
// playwright.config.ts
export default defineConfig({
  use: {
    // Disable animations to prevent timing issues
    reducedMotion: 'reduce',
  },
  
  // Per-test timeout
  timeout: 30000,
  
  // Retry failed tests
  retries: process.env.CI ? 2 : 0,
})

// Individual test timing control
test('should handle slow loading content', async ({ page }) => {
  // Extend timeout for specific test
  test.setTimeout(60000)
  
  await page.goto('/venues')
  
  // Wait for specific loading states
  await expect(page.getByTestId('loading-spinner')).toBeVisible()
  await expect(page.getByTestId('loading-spinner')).not.toBeVisible()
  
  // Verify content loaded
  await expect(page.getByTestId('venues-grid')).toBeVisible()
})
```

### Retry and Error Handling
```typescript
// Retry patterns for flaky operations
test('should handle intermittent network issues', async ({ page }) => {
  await page.goto('/venues')
  
  // Retry mechanism for flaky operations
  await expect(async () => {
    await page.getByRole('button', { name: 'Refresh' }).click()
    await expect(page.getByText('Data refreshed')).toBeVisible({ timeout: 5000 })
  }).toPass({
    intervals: [1000, 2000, 3000],
    timeout: 10000
  })
})

// Error state recovery
test('should recover from network errors gracefully', async ({ page }) => {
  await page.goto('/venues')
  
  // Simulate network failure
  await page.route('**/api/venues', route => 
    route.fulfill({ status: 500, body: 'Server Error' })
  )
  
  await page.getByRole('button', { name: 'Load Venues' }).click()
  await expect(page.getByText('Failed to load venues')).toBeVisible()
  
  // Restore network and retry
  await page.unroute('**/api/venues')
  await page.getByRole('button', { name: 'Retry' }).click()
  
  await expect(page.getByText('Venues loaded successfully')).toBeVisible()
})
```

## Parallel Execution

### Test Isolation for Parallelization
```typescript
// Ensure tests can run in parallel
test.describe.configure({ mode: 'parallel' })

test.describe('Venue CRUD Operations', () => {
  test('create venue - tenant A', async ({ page }) => {
    const tenantId = 'tenant-a-' + Date.now()
    await setupTenantContext(page, tenantId)
    
    // Test implementation with unique data
    await createVenueTest(page, `Venue-${tenantId}`)
  })
  
  test('create venue - tenant B', async ({ page }) => {
    const tenantId = 'tenant-b-' + Date.now()
    await setupTenantContext(page, tenantId)
    
    // Test implementation with unique data
    await createVenueTest(page, `Venue-${tenantId}`)
  })
})
```

These Playwright testing patterns ensure robust, maintainable, and reliable E2E tests that provide high confidence in feature functionality while minimizing flakiness and execution time.