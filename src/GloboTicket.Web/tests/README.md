# GloboTicket Test Utilities

This directory contains reusable test utilities, fixtures, and comprehensive end-to-end tests for the GloboTicket web application using Playwright.

## 📚 Documentation

- **[TEST_SUITE.md](TEST_SUITE.md)** - Comprehensive test suite overview, coverage summary, and best practices
- **[EXECUTION_GUIDE.md](EXECUTION_GUIDE.md)** - Detailed guide for running tests in various configurations
- **[MAINTENANCE_GUIDE.md](MAINTENANCE_GUIDE.md)** - Guide for maintaining and updating tests

## 🗂️ Directory Structure

```
tests/
├── auth/                    # Authentication tests
│   └── login.spec.ts       # Login functionality (7 tests)
├── acts/                    # Act management tests
│   ├── view-acts.spec.ts   # View acts list (10 tests)
│   ├── create-act.spec.ts  # Create act (tests)
│   ├── edit-act.spec.ts    # Edit act (tests)
│   └── delete-act.spec.ts  # Delete act (tests)
├── venues/                  # Venue management tests
│   ├── view-venues.spec.ts # View venues list (10 tests)
│   ├── create-venue.spec.ts # Create venue (10 tests)
│   ├── edit-venue.spec.ts  # Edit venue (8 tests)
│   └── delete-venue.spec.ts # Delete venue (SKIPPED - not implemented)
├── shows/                   # Show management tests
│   └── view-show.spec.ts   # View show detail (18 tests)
├── fixtures/
│   └── auth.ts             # Authentication fixtures and constants
├── helpers/
│   └── auth.helpers.ts     # Authentication helper functions
├── setup/
│   └── base.ts             # Base test configuration
├── TEST_SUITE.md           # Comprehensive test suite documentation
├── EXECUTION_GUIDE.md      # Test execution guide
├── MAINTENANCE_GUIDE.md    # Test maintenance guide
└── README.md               # This file
```

## 🚀 Quick Start

### Run All Tests
```bash
npm run test:e2e
```

### Run Specific Test Suite
```bash
# Authentication tests
npx playwright test tests/auth/login.spec.ts

# Act tests
npx playwright test tests/acts/view-acts.spec.ts

# Venue tests
npx playwright test tests/venues/view-venues.spec.ts
npx playwright test tests/venues/create-venue.spec.ts
npx playwright test tests/venues/edit-venue.spec.ts

# Show tests
npx playwright test tests/shows/view-show.spec.ts
```

### Run Tests in UI Mode
```bash
npx playwright test --ui
```

### Debug Tests
```bash
npx playwright test --debug
```

## 📊 Test Coverage

| Feature | Tests | Status |
|---------|-------|--------|
| Login | 7 | ✅ Active |
| View Acts | 10 | ✅ Active |
| View Venues | 10 | ✅ Active |
| Create Venue | 10 | ✅ Active |
| Edit Venue | 8 | ✅ Active |
| Delete Venue | 8 | ⚠️ Skipped (not implemented) |
| View Show Detail | 12 | ✅ Active |
| Act Detail - Shows List | 3 | ✅ Active |
| End-to-End Show Flow | 3 | ✅ Active |
| **Total** | **71** | **63 active, 8 skipped** |

## 🧪 Test Files

### Authentication Tests

#### [`auth/login.spec.ts`](auth/login.spec.ts)
Tests the login functionality including:
- Successful login with valid credentials
- Failed login with invalid credentials
- Form validation for empty fields
- Form element verification
- Keyboard navigation (Enter key)
- Accessibility attributes

### Act Management Tests

#### [`acts/view-acts.spec.ts`](acts/view-acts.spec.ts)
Tests the acts list page including:
- Page structure and act cards display
- Add Act button functionality
- Card interaction and navigation
- Empty state handling
- Loading states
- Responsive grid layout
- Keyboard accessibility

### Venue Management Tests

#### [`venues/view-venues.spec.ts`](venues/view-venues.spec.ts)
Tests the venues list page including:
- Page structure and venue cards display
- Add Venue button functionality
- Card interaction and navigation
- Empty state handling
- Loading states
- Responsive grid layout
- Keyboard accessibility

#### [`venues/create-venue.spec.ts`](venues/create-venue.spec.ts)
Tests venue creation including:
- Successful creation with required/optional fields
- Form validation (required fields, length, numeric ranges)
- Cancel functionality
- Loading states
- Boundary value testing

#### [`venues/edit-venue.spec.ts`](venues/edit-venue.spec.ts)
Tests venue editing including:
- Form pre-population with existing data
- Successful updates
- Validation on edit
- Cancel without saving
- Loading states

#### [`venues/delete-venue.spec.ts`](venues/delete-venue.spec.ts)
⚠️ **Currently skipped** - Delete functionality not yet implemented.

See the file for planned test coverage and implementation requirements.

### Show Management Tests

#### [`shows/view-show.spec.ts`](shows/view-show.spec.ts)
Tests the show detail page and related flows including:

**Show Detail Page Tests (12 scenarios):**
- Display show details when navigating from acts page
- Show act name correctly
- Show venue name correctly
- Show start date/time correctly
- Show ticket count correctly
- Back button navigation to acts page
- Loading state while fetching show data
- Error message for non-existent show
- Error message for network failure
- Keyboard navigation accessibility
- Proper ARIA labels
- Responsive layout on mobile

**Act Detail Page Enhancement Tests (3 scenarios):**
- Display list of shows for an act
- Navigate to show detail when show card is clicked
- Show empty state when act has no shows

**End-to-End Flow Tests (3 scenarios):**
- Complete flow: Acts page → Act detail → Show detail → Back
- Direct URL access to show detail page
- Browser back button functionality

## Authentication Utilities

### Test Credentials

Default test credentials are defined in `fixtures/auth.ts`:

```typescript
import { TEST_CREDENTIALS } from './fixtures/auth';

// username: 'playwright'
// password: 'playwright123'
// tenant: 'playwright-test'
```

The Playwright tests use a dedicated test user with its own isolated tenant (`playwright-test`) to ensure test data doesn't interfere with production or smoke test data.

### Helper Functions

The `helpers/auth.helpers.ts` file provides three main functions:

#### `login(page, username, password)`
Performs login via the UI using the login form.

```typescript
import { login } from './helpers/auth.helpers';

await login(page, 'playwright', 'playwright123');
```

#### `waitForAuthentication(page)`
Waits for successful authentication by checking for redirect to dashboard.

```typescript
import { waitForAuthentication } from './helpers/auth.helpers';

await waitForAuthentication(page);
```

#### `isAuthenticated(page)`
Checks if the user is currently authenticated.

```typescript
import { isAuthenticated } from './helpers/auth.helpers';

const authenticated = await isAuthenticated(page);
```

### Authentication Fixtures

The `fixtures/auth.ts` file provides a custom test fixture with pre-authenticated context:

```typescript
import { test, expect } from './fixtures/auth';

test('should access protected route', async ({ authenticatedPage }) => {
  // authenticatedPage is already logged in
  await authenticatedPage.goto('/venues');
  await expect(authenticatedPage).toHaveURL('/venues');
});
```

### Base Test Configuration

The `setup/base.ts` file provides base configuration with authenticated context:

```typescript
import { test, expect, baseConfig } from './setup/base';

test('should use base config', async ({ authenticatedContext }) => {
  // authenticatedContext is already logged in
  // baseURL is automatically set from environment or defaults to http://localhost:5173
  console.log(baseConfig.baseURL);
});
```

## Storage State Management

Authentication state is persisted to `playwright/.auth/user.json` to avoid repeated logins across test runs. This file is automatically created when using the authentication fixtures.

## Environment Variables

- `BASE_URL`: Base URL for the application (default: `http://localhost:5173`)

## Usage Examples

### Using Authentication Fixtures

```typescript
import { test, expect } from './fixtures/auth';

test.describe('Venues Page', () => {
  test('should display venues list', async ({ authenticatedPage }) => {
    await authenticatedPage.goto('/venues');
    await expect(authenticatedPage.locator('h1')).toContainText('Venues');
  });
});
```

### Using Helper Functions Directly

```typescript
import { test, expect } from '@playwright/test';
import { login, waitForAuthentication } from './helpers/auth.helpers';

test('manual login test', async ({ page }) => {
  await page.goto('/login');
  await login(page, 'playwright', 'playwright123');
  await waitForAuthentication(page);
  await expect(page).toHaveURL('/');
});
```

### Using Base Configuration

```typescript
import { test, expect } from './setup/base';

test('test with base config', async ({ authenticatedContext, baseURL }) => {
  console.log(`Testing against: ${baseURL}`);
  await authenticatedContext.goto('/venues');
  await expect(authenticatedContext).toHaveURL(`${baseURL}/venues`);
});
```

## Best Practices

1. **Use fixtures for most tests**: The authentication fixtures handle login automatically and persist state.
2. **Use helpers for custom scenarios**: When you need more control over the login process.
3. **Reuse authentication state**: The storage state is saved to avoid repeated logins.
4. **Set BASE_URL environment variable**: For testing against different environments.

## Login Form Selectors

The authentication helpers use the following selectors:
- Username field: `#username`
- Password field: `#password`
- Submit button: `button[type="submit"]`

After successful login, the application redirects to `/` (dashboard).

## 🔧 Troubleshooting

### Common Issues

**Tests fail with timeout errors**
- Ensure the backend API is running
- Verify BASE_URL is correct (default: `http://localhost:5173`)
- Check network connectivity

**Authentication tests fail**
- Verify test credentials: username `playwright`, password `playwright123`
- Clear authentication state: `rm -rf playwright/.auth`
- Check login page selectors haven't changed

**Venue tests can't find elements**
- Wait for loading spinner to disappear
- Ensure test data exists in database
- Check element selectors match current implementation

**Tests are flaky**
- Use explicit waits: `waitForLoadState('networkidle')`
- Avoid hard-coded timeouts
- Ensure test data is unique (use timestamps)

## 🔗 Related Documentation

- [TEST_SUITE.md](TEST_SUITE.md) - Comprehensive test suite documentation
- [EXECUTION_GUIDE.md](EXECUTION_GUIDE.md) - Detailed test execution guide
- [MAINTENANCE_GUIDE.md](MAINTENANCE_GUIDE.md) - Test maintenance guide
- [Playwright Documentation](https://playwright.dev)
- [Venues Page Architecture](../../../docs/venues-page-architecture.md)
- [Project README](../README.md)

## 🤝 Contributing

When adding new tests:
1. Follow existing naming conventions
2. Add comprehensive comments
3. Include positive and negative test cases
4. Test accessibility features
5. Update documentation
6. Ensure tests are isolated and independent

For detailed guidelines, see [MAINTENANCE_GUIDE.md](MAINTENANCE_GUIDE.md).