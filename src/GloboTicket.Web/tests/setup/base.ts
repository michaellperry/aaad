import { test as base, expect } from '@playwright/test';
import type { Page } from '@playwright/test';
import { login, waitForAuthentication } from '../helpers/auth.helpers';
import { TEST_CREDENTIALS, AUTH_STATE_PATH } from '../fixtures/auth';

// Get base URL from environment or use default
const BASE_URL = process.env.BASE_URL || 'http://localhost:5173';

// Extended test fixtures with authenticated context
type BaseFixtures = {
  authenticatedContext: Page;
  baseURL: string;
};

/**
 * Base test configuration that extends Playwright's test with custom fixtures.
 * Provides an authenticated context that can be reused across all tests.
 */
export const test = base.extend<BaseFixtures>({
  // Base URL fixture
  baseURL: async ({}, use) => {
    await use(BASE_URL);
  },

  // Authenticated context fixture
  authenticatedContext: async ({ page, baseURL }, use) => {
    // Set the base URL for the page
    page.context().setDefaultNavigationTimeout(30000);
    
    // Navigate to the base URL
    await page.goto(baseURL);
    
    // Perform login with test credentials
    await login(page, TEST_CREDENTIALS.username, TEST_CREDENTIALS.password);
    
    // Wait for successful authentication
    await waitForAuthentication(page);
    
    // Save authentication state for reuse
    await page.context().storageState({ path: AUTH_STATE_PATH });
    
    // Provide the authenticated page to tests
    await use(page);
  },
});

// Re-export expect for convenience
export { expect };

/**
 * Configuration options for base test setup
 */
export const baseConfig = {
  baseURL: BASE_URL,
  storageStatePath: AUTH_STATE_PATH,
  testCredentials: TEST_CREDENTIALS,
};