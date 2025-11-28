import { Page, expect } from '@playwright/test';

/**
 * Performs login via the UI using the login form.
 * 
 * @param page - Playwright page object
 * @param username - Username to log in with
 * @param password - Password to log in with
 */
export async function login(page: Page, username: string, password: string): Promise<void> {
  // Navigate to login page if not already there
  const currentUrl = page.url();
  if (!currentUrl.includes('/login')) {
    await page.goto('/login');
  }
  
  // Fill in the login form using the identified selectors
  await page.fill('#username', username);
  await page.fill('#password', password);
  
  // Submit the form
  await page.click('button[type="submit"]');
}

/**
 * Waits for successful authentication by checking for redirect to dashboard.
 * 
 * @param page - Playwright page object
 */
export async function waitForAuthentication(page: Page): Promise<void> {
  // Wait for navigation to the dashboard (root path)
  await page.waitForURL('/', { timeout: 10000 });
  
  // Additional verification that we're on the authenticated page
  await expect(page).toHaveURL('/');
}

/**
 * Checks if the user is currently authenticated.
 * 
 * @param page - Playwright page object
 * @returns True if user is authenticated, false otherwise
 */
export async function isAuthenticated(page: Page): Promise<boolean> {
  try {
    // Check if we're on the dashboard (authenticated route)
    const url = page.url();
    
    // If we're on the login page, we're not authenticated
    if (url.includes('/login')) {
      return false;
    }
    
    // Try to access a protected route indicator
    // This could be checking for user menu, logout button, or other auth-only elements
    const isOnDashboard = url.endsWith('/') || url.includes('/venues') || url.includes('/acts');
    
    return isOnDashboard;
  } catch (error) {
    return false;
  }
}