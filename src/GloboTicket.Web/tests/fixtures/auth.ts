import { test as base, type Page } from '@playwright/test';
import { login, waitForAuthentication } from '../helpers/auth.helpers';

// Test credentials constants - using dedicated Playwright test user with isolated tenant
export const TEST_CREDENTIALS = {
  username: 'playwright',
  password: 'playwright123',
} as const;

// Storage state path for persisting authentication
export const AUTH_STATE_PATH = 'playwright/.auth/user.json';

// Extended test fixtures with authentication
type AuthFixtures = {
  authenticatedPage: Page;
};

/**
 * Custom test fixture that provides an authenticated page context.
 * This fixture automatically logs in before tests and persists the auth state.
 */
export const test = base.extend<AuthFixtures>({
  authenticatedPage: async ({ page }, use) => {
    // Perform login
    await login(page, TEST_CREDENTIALS.username, TEST_CREDENTIALS.password);
    
    // Wait for successful authentication
    await waitForAuthentication(page);
    
    // Use the authenticated page in tests
    await use(page);
  },
});

export { expect } from '@playwright/test';