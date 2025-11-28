# Test Maintenance Guide

This guide provides comprehensive instructions for maintaining, updating, and extending the GloboTicket Playwright test suite.

## Table of Contents

- [Adding New Tests](#adding-new-tests)
- [Updating Existing Tests](#updating-existing-tests)
- [Handling UI Changes](#handling-ui-changes)
- [Managing Flaky Tests](#managing-flaky-tests)
- [Test Data Management](#test-data-management)
- [Authentication Management](#authentication-management)
- [Common Patterns](#common-patterns)
- [Anti-Patterns to Avoid](#anti-patterns-to-avoid)
- [Code Review Checklist](#code-review-checklist)
- [Refactoring Tests](#refactoring-tests)

## Adding New Tests

### 1. Choose the Right Location

Organize tests by feature area:

```
tests/
├── auth/           # Authentication-related tests
├── venues/         # Venue management tests
├── acts/           # Act management tests (future)
├── shows/          # Show management tests (future)
└── tickets/        # Ticket sales tests (future)
```

### 2. Create a New Test File

Use the naming convention: `feature-name.spec.ts`

```typescript
import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Feature Name Test Suite
 * 
 * Brief description of what this test suite covers.
 * Include any special setup requirements or dependencies.
 */
test.describe('Feature Name', () => {
  /**
   * Setup that runs before each test
   */
  test.beforeEach(async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the feature page
    await authenticatedPage.goto('/feature-path');
    await authenticatedPage.waitForLoadState('networkidle');
  });

  /**
   * Test 1: Description
   * 
   * Detailed explanation of what this test verifies.
   */
  test('should do something specific', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Arrange: Set up test data
    const testData = 'test value';
    
    // Act: Perform the action
    await authenticatedPage.fill('#input', testData);
    await authenticatedPage.click('button[type="submit"]');
    
    // Assert: Verify the result
    await expect(authenticatedPage.locator('#result')).toContainText(testData);
  });
});
```

### 3. Follow the AAA Pattern

Structure tests using Arrange-Act-Assert:

```typescript
test('should create a new item', async ({ authenticatedPage }) => {
  // Arrange: Prepare test data
  const timestamp = Date.now();
  const itemName = `Test Item ${timestamp}`;
  
  // Act: Perform the action
  await authenticatedPage.fill('#name', itemName);
  await authenticatedPage.click('button:has-text("Create")');
  
  // Assert: Verify the outcome
  await authenticatedPage.waitForURL('/items');
  await expect(authenticatedPage.locator(`text=${itemName}`)).toBeVisible();
});
```

### 4. Add Comprehensive Comments

```typescript
/**
 * Test: Successful Item Creation
 * 
 * Verifies that a user can successfully create a new item with valid data
 * and is redirected to the items list where the new item appears.
 * 
 * Prerequisites:
 * - User must be authenticated
 * - Database must be accessible
 * 
 * Steps:
 * 1. Navigate to create item page
 * 2. Fill in required fields
 * 3. Submit the form
 * 4. Verify redirect to items list
 * 5. Verify new item appears in the list
 */
test('should successfully create a new item', async ({ authenticatedPage }) => {
  // Test implementation
});
```

### 5. Use Unique Test Data

Always use timestamps or UUIDs to ensure test data uniqueness:

```typescript
const timestamp = Date.now();
const uniqueName = `Test Item ${timestamp}`;

// Or use UUID
import { v4 as uuidv4 } from 'uuid';
const uniqueId = uuidv4();
```

### 6. Add Both Positive and Negative Tests

```typescript
// Positive test
test('should create item with valid data', async ({ authenticatedPage }) => {
  // Test successful creation
});

// Negative tests
test('should show error with empty required field', async ({ authenticatedPage }) => {
  // Test validation error
});

test('should show error with invalid data format', async ({ authenticatedPage }) => {
  // Test format validation
});
```

## Updating Existing Tests

### When UI Changes

1. **Identify affected tests**:
```bash
npx playwright test --grep "feature-name"
```

2. **Update selectors**:
```typescript
// Old selector
await page.click('#old-button-id');

// New selector
await page.click('[data-testid="new-button"]');
```

3. **Run tests to verify**:
```bash
npx playwright test tests/feature/test-file.spec.ts
```

### When API Changes

1. **Update test data structures**:
```typescript
// Old structure
const venue = {
  name: 'Test Venue',
  capacity: 1000
};

// New structure (added required field)
const venue = {
  name: 'Test Venue',
  capacity: 1000,
  category: 'Concert Hall' // New required field
};
```

2. **Update assertions**:
```typescript
// Update expected responses
await expect(response).toHaveProperty('newField');
```

### When Business Logic Changes

1. **Review test scenarios**
2. **Update test expectations**
3. **Add new test cases for new behavior**
4. **Remove obsolete tests**

## Handling UI Changes

### Strategy 1: Use Data Test IDs

**Recommended approach** - Add `data-testid` attributes to components:

```tsx
// Component
<button data-testid="create-venue-button">Create Venue</button>

// Test
await page.click('[data-testid="create-venue-button"]');
```

### Strategy 2: Use Semantic Selectors

Prefer semantic HTML and ARIA attributes:

```typescript
// Good - semantic and stable
await page.click('button[type="submit"]');
await page.click('[aria-label="Add new venue"]');
await page.click('role=button[name="Create"]');

// Avoid - fragile
await page.click('.btn-primary.mt-4.px-6');
```

### Strategy 3: Create Selector Constants

Centralize selectors for easy updates:

```typescript
// tests/selectors/venue.selectors.ts
export const VenueSelectors = {
  nameInput: '#name',
  capacityInput: '#seatingCapacity',
  submitButton: 'button:has-text("Create Venue")',
  cancelButton: 'button:has-text("Cancel")',
} as const;

// In test
import { VenueSelectors } from '../selectors/venue.selectors';
await page.fill(VenueSelectors.nameInput, 'Test Venue');
```

### Strategy 4: Use Page Object Model

For complex pages, create page objects:

```typescript
// tests/pages/CreateVenuePage.ts
export class CreateVenuePage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/venues/new');
    await this.page.waitForLoadState('networkidle');
  }

  async fillName(name: string) {
    await this.page.fill('#name', name);
  }

  async fillCapacity(capacity: number) {
    await this.page.fill('#seatingCapacity', capacity.toString());
  }

  async submit() {
    await this.page.click('button:has-text("Create Venue")');
  }

  async createVenue(data: VenueData) {
    await this.fillName(data.name);
    await this.fillCapacity(data.capacity);
    await this.submit();
  }
}

// In test
const createPage = new CreateVenuePage(authenticatedPage);
await createPage.goto();
await createPage.createVenue({ name: 'Test', capacity: 1000 });
```

## Managing Flaky Tests

### Identifying Flaky Tests

1. **Run tests multiple times**:
```bash
npx playwright test --repeat-each=10
```

2. **Check test reports** for intermittent failures

3. **Look for timing issues** in test logs

### Common Causes and Fixes

#### 1. Race Conditions

**Problem**: Test proceeds before page is ready

**Solution**: Use explicit waits
```typescript
// Bad
await page.click('button');
await page.waitForTimeout(1000); // Arbitrary wait

// Good
await page.click('button');
await page.waitForLoadState('networkidle');
await page.waitForSelector('#result', { state: 'visible' });
```

#### 2. Animation Delays

**Problem**: Elements are animating when test tries to interact

**Solution**: Wait for animations to complete
```typescript
// Wait for element to be stable
await page.waitForSelector('#element', { state: 'visible' });
await page.waitForTimeout(300); // Wait for animation

// Or disable animations in test config
use: {
  launchOptions: {
    args: ['--disable-blink-features=AutomationControlled']
  }
}
```

#### 3. Network Timing

**Problem**: API responses vary in timing

**Solution**: Wait for specific network states
```typescript
// Wait for specific API call
await page.waitForResponse(response => 
  response.url().includes('/api/venues') && response.status() === 200
);

// Or wait for network idle
await page.waitForLoadState('networkidle');
```

#### 4. Stale Elements

**Problem**: Element reference becomes stale

**Solution**: Re-query elements
```typescript
// Bad
const button = page.locator('button');
await page.reload();
await button.click(); // May fail - stale reference

// Good
await page.reload();
await page.locator('button').click(); // Fresh query
```

### Retry Strategy

Configure retries for flaky tests:

```typescript
// In playwright.config.ts
retries: process.env.CI ? 2 : 0,

// Or per-test
test('flaky test', async ({ page }) => {
  test.setTimeout(60000);
  test.slow(); // Triples timeout
  // Test code
});
```

## Test Data Management

### Creating Test Data

#### Option 1: Create via UI (Current Approach)

```typescript
async function createTestVenue(page: Page, name: string): Promise<string> {
  await page.goto('/venues/new');
  await page.fill('#name', name);
  await page.fill('#seatingCapacity', '1000');
  await page.fill('#description', 'Test venue');
  await page.click('button:has-text("Create Venue")');
  await page.waitForURL('/venues');
  
  // Extract and return venue ID from URL
  const url = page.url();
  return extractVenueId(url);
}
```

#### Option 2: Create via API (Faster)

```typescript
async function createTestVenueViaAPI(name: string): Promise<string> {
  const response = await fetch('http://localhost:5000/api/venues', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + authToken
    },
    body: JSON.stringify({
      name,
      seatingCapacity: 1000,
      description: 'Test venue'
    })
  });
  
  const venue = await response.json();
  return venue.id;
}
```

### Cleaning Up Test Data

#### Option 1: Manual Cleanup (Current)

Periodically clean test data from database:
```sql
DELETE FROM Venues WHERE Name LIKE 'Test Venue %';
```

#### Option 2: Automated Cleanup (Recommended)

```typescript
test.afterEach(async ({ authenticatedPage }) => {
  // Clean up test data created in this test
  if (createdVenueId) {
    await deleteVenue(authenticatedPage, createdVenueId);
  }
});
```

#### Option 3: Database Reset

For integration tests, reset database before test run:
```bash
npm run db:reset:test
```

### Test Data Isolation

Ensure tests don't interfere with each other:

```typescript
// Use unique identifiers
const testId = `test-${Date.now()}-${Math.random()}`;
const venueName = `Test Venue ${testId}`;

// Or use test.describe.serial for dependent tests
test.describe.serial('Venue workflow', () => {
  let venueId: string;
  
  test('create venue', async ({ authenticatedPage }) => {
    venueId = await createVenue(authenticatedPage);
  });
  
  test('edit venue', async ({ authenticatedPage }) => {
    await editVenue(authenticatedPage, venueId);
  });
});
```

## Authentication Management

### Current Implementation

Authentication is handled via fixtures in [`fixtures/auth.ts`](fixtures/auth.ts):

```typescript
import { test, expect } from './fixtures/auth';

test('my test', async ({ authenticatedPage }) => {
  // Page is already authenticated
});
```

### Updating Authentication

If login flow changes:

1. **Update helper functions** in [`helpers/auth.helpers.ts`](helpers/auth.helpers.ts):
```typescript
export async function login(page: Page, username: string, password: string) {
  // Update selectors if login form changes
  await page.fill('#username', username);
  await page.fill('#password', password);
  await page.click('button[type="submit"]');
}
```

2. **Update test credentials** in [`fixtures/auth.ts`](fixtures/auth.ts):
```typescript
export const TEST_CREDENTIALS = {
  username: 'new-username',
  password: 'new-password',
};
```

3. **Clear authentication state**:
```bash
rm -rf playwright/.auth
```

### Multiple User Roles

To test with different user roles:

```typescript
// Create role-specific fixtures
export const adminTest = base.extend<{ adminPage: Page }>({
  adminPage: async ({ page }, use) => {
    await login(page, 'admin', 'admin123');
    await use(page);
  },
});

export const userTest = base.extend<{ userPage: Page }>({
  userPage: async ({ page }, use) => {
    await login(page, 'user', 'user123');
    await use(page);
  },
});
```

## Common Patterns

### Pattern 1: Wait for Navigation

```typescript
// Wait for URL change
await page.click('button');
await page.waitForURL('/new-page');

// Wait for specific URL pattern
await page.waitForURL(/\/venues\/[a-f0-9-]+/);
```

### Pattern 2: Handle Dialogs

```typescript
// Accept confirmation dialog
page.on('dialog', dialog => dialog.accept());
await page.click('button:has-text("Delete")');

// Dismiss dialog
page.on('dialog', dialog => dialog.dismiss());
```

### Pattern 3: File Upload

```typescript
await page.setInputFiles('input[type="file"]', 'path/to/file.pdf');
```

### Pattern 4: Multiple Elements

```typescript
// Get all matching elements
const items = page.locator('.item');
const count = await items.count();

// Iterate over elements
for (let i = 0; i < count; i++) {
  const text = await items.nth(i).textContent();
  console.log(text);
}
```

### Pattern 5: Conditional Logic

```typescript
// Check if element exists
const errorExists = await page.locator('#error').isVisible();
if (errorExists) {
  // Handle error case
}

// Wait for one of multiple conditions
await Promise.race([
  page.waitForSelector('#success'),
  page.waitForSelector('#error')
]);
```

## Anti-Patterns to Avoid

### ❌ Hard-Coded Waits

```typescript
// Bad
await page.waitForTimeout(5000);

// Good
await page.waitForLoadState('networkidle');
await page.waitForSelector('#element');
```

### ❌ Brittle Selectors

```typescript
// Bad - depends on styling
await page.click('.btn.btn-primary.mt-4');

// Good - semantic
await page.click('button[type="submit"]');
await page.click('[data-testid="submit-button"]');
```

### ❌ Testing Implementation Details

```typescript
// Bad - testing internal state
expect(component.state.isLoading).toBe(false);

// Good - testing user-visible behavior
await expect(page.locator('#loading')).not.toBeVisible();
```

### ❌ Shared Mutable State

```typescript
// Bad - tests affect each other
let sharedVenueId: string;

test('test 1', async () => {
  sharedVenueId = await createVenue();
});

test('test 2', async () => {
  await editVenue(sharedVenueId); // Depends on test 1
});

// Good - isolated tests
test('test 1', async () => {
  const venueId = await createVenue();
  // Use venueId only in this test
});
```

### ❌ Overly Complex Tests

```typescript
// Bad - testing too much
test('complete venue workflow', async () => {
  await createVenue();
  await editVenue();
  await deleteVenue();
  await verifyVenueGone();
  // 50 more lines...
});

// Good - focused tests
test('should create venue', async () => {
  await createVenue();
  await verifyVenueCreated();
});

test('should edit venue', async () => {
  const id = await createVenue();
  await editVenue(id);
  await verifyVenueEdited();
});
```

## Code Review Checklist

When reviewing test code, check for:

- [ ] Tests are independent and can run in any order
- [ ] Test names clearly describe what is being tested
- [ ] Tests use explicit waits, not arbitrary timeouts
- [ ] Test data is unique (uses timestamps or UUIDs)
- [ ] Selectors are semantic and stable
- [ ] Tests include both positive and negative cases
- [ ] Comments explain complex logic
- [ ] Tests follow AAA pattern (Arrange-Act-Assert)
- [ ] No hard-coded credentials or sensitive data
- [ ] Tests clean up after themselves (if applicable)
- [ ] Error messages are descriptive
- [ ] Tests are focused and not overly complex
- [ ] Accessibility is tested where relevant

## Refactoring Tests

### When to Refactor

- Tests are duplicating code
- Tests are hard to understand
- Tests are brittle and break often
- New features require similar test patterns

### Refactoring Techniques

#### 1. Extract Helper Functions

```typescript
// Before
test('test 1', async ({ page }) => {
  await page.fill('#name', 'Test');
  await page.fill('#capacity', '1000');
  await page.click('button:has-text("Create")');
});

test('test 2', async ({ page }) => {
  await page.fill('#name', 'Test 2');
  await page.fill('#capacity', '2000');
  await page.click('button:has-text("Create")');
});

// After
async function fillVenueForm(page: Page, name: string, capacity: number) {
  await page.fill('#name', name);
  await page.fill('#capacity', capacity.toString());
  await page.click('button:has-text("Create")');
}

test('test 1', async ({ page }) => {
  await fillVenueForm(page, 'Test', 1000);
});

test('test 2', async ({ page }) => {
  await fillVenueForm(page, 'Test 2', 2000);
});
```

#### 2. Use Test Fixtures

```typescript
// Create reusable fixtures
type VenueFixtures = {
  venueData: VenueData;
  createdVenue: string;
};

const venueTest = test.extend<VenueFixtures>({
  venueData: async ({}, use) => {
    const data = {
      name: `Test Venue ${Date.now()}`,
      capacity: 1000
    };
    await use(data);
  },
  
  createdVenue: async ({ authenticatedPage, venueData }, use) => {
    const id = await createVenue(authenticatedPage, venueData);
    await use(id);
    // Cleanup
    await deleteVenue(authenticatedPage, id);
  }
});
```

#### 3. Parameterized Tests

```typescript
const testCases = [
  { name: 'Small Venue', capacity: 100 },
  { name: 'Medium Venue', capacity: 1000 },
  { name: 'Large Venue', capacity: 10000 },
];

for (const testCase of testCases) {
  test(`should create ${testCase.name}`, async ({ page }) => {
    await createVenue(page, testCase.name, testCase.capacity);
    await verifyVenueCreated(testCase.name);
  });
}
```

## Documentation Updates

When making changes, update:

1. **This file** (MAINTENANCE_GUIDE.md) - for new patterns or practices
2. **[TEST_SUITE.md](TEST_SUITE.md)** - for test coverage changes
3. **[EXECUTION_GUIDE.md](EXECUTION_GUIDE.md)** - for new execution options
4. **[README.md](README.md)** - for utility changes
5. **Test file comments** - keep inline documentation current

## Getting Help

- Review existing tests for patterns
- Check [Playwright Best Practices](https://playwright.dev/docs/best-practices)
- Ask in team chat or code review
- Consult [TEST_SUITE.md](TEST_SUITE.md) for architecture
- See [EXECUTION_GUIDE.md](EXECUTION_GUIDE.md) for debugging

## Next Steps

- Review [TEST_SUITE.md](TEST_SUITE.md) for current test coverage
- Check [EXECUTION_GUIDE.md](EXECUTION_GUIDE.md) for running tests
- Read [README.md](README.md) for authentication utilities
- Start adding tests following these guidelines!