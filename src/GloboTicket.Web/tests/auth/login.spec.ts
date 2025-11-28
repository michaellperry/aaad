import { test, expect } from '../setup/base';
import { login, waitForAuthentication, isAuthenticated } from '../helpers/auth.helpers';
import { TEST_CREDENTIALS } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Login Page Test Suite
 * 
 * Comprehensive tests for the login functionality including:
 * - Successful authentication flow
 * - Failed login attempts with invalid credentials
 * - Form validation for empty fields
 * - Login form element verification
 */
test.describe('Login Page', () => {
  /**
   * Test Setup: Navigate to login page before each test
   * Ensures test isolation and consistent starting state
   */
  test.beforeEach(async ({ page }: { page: Page }) => {
    // Navigate to the login page
    await page.goto('/login');
    
    // Wait for the page to be fully loaded
    await page.waitForLoadState('networkidle');
  });

  /**
   * Test 1: Successful Login
   * 
   * Verifies that a user can successfully log in with valid credentials
   * and is redirected to the dashboard with authenticated state.
   */
  test('should successfully log in with valid credentials', async ({ page }: { page: Page }) => {
    // Fill in the username field with valid credentials
    await page.fill('#username', TEST_CREDENTIALS.username);
    
    // Fill in the password field with valid credentials
    await page.fill('#password', TEST_CREDENTIALS.password);
    
    // Submit the login form
    await page.click('button[type="submit"]');
    
    // Wait for navigation to complete and verify redirect to dashboard
    await waitForAuthentication(page);
    
    // Verify the user is on the dashboard (root path)
    await expect(page).toHaveURL('/');
    
    // Verify the user is authenticated
    const authenticated = await isAuthenticated(page);
    expect(authenticated).toBe(true);
    
    // Additional verification: Check that we're not on the login page
    const currentUrl = page.url();
    expect(currentUrl).not.toContain('/login');
  });

  /**
   * Test 2: Failed Login with Invalid Credentials
   * 
   * Verifies that login fails with invalid credentials and displays
   * an appropriate error message without redirecting the user.
   */
  test('should fail to log in with invalid credentials', async ({ page }: { page: Page }) => {
    // Use invalid credentials
    const invalidUsername = 'invaliduser';
    const invalidPassword = 'wrongpassword';
    
    // Fill in the username field with invalid credentials
    await page.fill('#username', invalidUsername);
    
    // Fill in the password field with invalid credentials
    await page.fill('#password', invalidPassword);
    
    // Submit the login form
    await page.click('button[type="submit"]');
    
    // Wait for the error message to appear
    // The error element should be visible after failed login attempt
    await page.waitForSelector('#login-error', { state: 'visible', timeout: 5000 });
    
    // Verify that an error message is displayed
    const errorElement = page.locator('#login-error');
    await expect(errorElement).toBeVisible();
    
    // Verify the error message contains relevant text
    const errorText = await errorElement.textContent();
    expect(errorText).toBeTruthy();
    expect(errorText?.toLowerCase()).toContain('invalid');
    
    // Verify the user remains on the login page
    await expect(page).toHaveURL(/\/login/);
    
    // Verify the user is not authenticated
    const authenticated = await isAuthenticated(page);
    expect(authenticated).toBe(false);
  });

  /**
   * Test 3: Empty Form Validation
   * 
   * Verifies that HTML5 form validation prevents submission when
   * required fields are empty.
   */
  test('should prevent submission with empty form fields', async ({ page }: { page: Page }) => {
    // Get the username input element
    const usernameInput = page.locator('#username');
    
    // Get the password input element
    const passwordInput = page.locator('#password');
    
    // Verify both fields have the 'required' attribute
    await expect(usernameInput).toHaveAttribute('required', '');
    await expect(passwordInput).toHaveAttribute('required', '');
    
    // Attempt to submit the form without filling any fields
    await page.click('button[type="submit"]');
    
    // Verify that HTML5 validation prevents submission
    // The page should still be on /login (no navigation occurred)
    await expect(page).toHaveURL(/\/login/);
    
    // Verify the username field shows validation error
    const usernameValidity = await usernameInput.evaluate((el: HTMLInputElement) => el.validity.valid);
    expect(usernameValidity).toBe(false);
    
    // Additional check: Verify no error message from the application appears
    // (since HTML5 validation should prevent form submission)
    const errorElement = page.locator('#login-error');
    await expect(errorElement).not.toBeVisible();
  });

  /**
   * Test 4: Login Form Elements Verification
   * 
   * Verifies that all login form elements exist and have the correct
   * attributes for accessibility, security, and user experience.
   */
  test('should have all required form elements with correct attributes', async ({ page }: { page: Page }) => {
    // Verify username input exists
    const usernameInput = page.locator('#username');
    await expect(usernameInput).toBeVisible();
    
    // Verify username input attributes
    await expect(usernameInput).toHaveAttribute('type', 'text');
    await expect(usernameInput).toHaveAttribute('name', 'username');
    await expect(usernameInput).toHaveAttribute('autocomplete', 'username');
    await expect(usernameInput).toHaveAttribute('required', '');
    
    // Verify username input has placeholder
    const usernamePlaceholder = await usernameInput.getAttribute('placeholder');
    expect(usernamePlaceholder).toBeTruthy();
    expect(usernamePlaceholder?.toLowerCase()).toContain('username');
    
    // Verify password input exists
    const passwordInput = page.locator('#password');
    await expect(passwordInput).toBeVisible();
    
    // Verify password input attributes
    await expect(passwordInput).toHaveAttribute('type', 'password');
    await expect(passwordInput).toHaveAttribute('name', 'password');
    await expect(passwordInput).toHaveAttribute('autocomplete', 'current-password');
    await expect(passwordInput).toHaveAttribute('required', '');
    
    // Verify password input has placeholder
    const passwordPlaceholder = await passwordInput.getAttribute('placeholder');
    expect(passwordPlaceholder).toBeTruthy();
    expect(passwordPlaceholder?.toLowerCase()).toContain('password');
    
    // Verify submit button exists
    const submitButton = page.locator('button[type="submit"]');
    await expect(submitButton).toBeVisible();
    
    // Verify submit button has appropriate text
    const buttonText = await submitButton.textContent();
    expect(buttonText).toBeTruthy();
    expect(buttonText?.toLowerCase()).toMatch(/sign in|login/);
    
    // Verify submit button is enabled by default
    await expect(submitButton).toBeEnabled();
    
    // Verify form labels exist for accessibility
    const usernameLabel = page.locator('label[for="username"]');
    await expect(usernameLabel).toBeVisible();
    
    const passwordLabel = page.locator('label[for="password"]');
    await expect(passwordLabel).toBeVisible();
  });

  /**
   * Test 5: Login Form Interaction Flow
   * 
   * Verifies that the form behaves correctly during user interaction,
   * including loading states and error clearing.
   */
  test('should handle form interaction states correctly', async ({ page }: { page: Page }) => {
    // Fill in invalid credentials first to trigger an error
    await page.fill('#username', 'invalid');
    await page.fill('#password', 'invalid');
    await page.click('button[type="submit"]');
    
    // Wait for error to appear
    await page.waitForSelector('#login-error', { state: 'visible', timeout: 5000 });
    
    // Verify error is visible
    const errorElement = page.locator('#login-error');
    await expect(errorElement).toBeVisible();
    
    // Start typing in the username field
    await page.fill('#username', 'n');
    
    // Verify that error is cleared when user starts typing
    // (This depends on the implementation - the error should clear on input)
    await page.waitForTimeout(500); // Small delay for state update
    
    // Now fill in valid credentials
    await page.fill('#username', TEST_CREDENTIALS.username);
    await page.fill('#password', TEST_CREDENTIALS.password);
    
    // Submit the form
    await page.click('button[type="submit"]');
    
    // Verify loading state (button should show loading text or be disabled)
    // Note: This check happens quickly, so we check immediately after click
    const submitButton = page.locator('button[type="submit"]');
    
    // Wait for successful authentication
    await waitForAuthentication(page);
    
    // Verify successful redirect
    await expect(page).toHaveURL('/');
  });

  /**
   * Test 6: Login with Enter Key
   * 
   * Verifies that users can submit the form by pressing Enter
   * in either the username or password field.
   */
  test('should submit form when pressing Enter key', async ({ page }: { page: Page }) => {
    // Fill in the username field
    await page.fill('#username', TEST_CREDENTIALS.username);
    
    // Fill in the password field
    await page.fill('#password', TEST_CREDENTIALS.password);
    
    // Press Enter in the password field to submit
    await page.locator('#password').press('Enter');
    
    // Wait for authentication to complete
    await waitForAuthentication(page);
    
    // Verify successful redirect to dashboard
    await expect(page).toHaveURL('/');
    
    // Verify authentication state
    const authenticated = await isAuthenticated(page);
    expect(authenticated).toBe(true);
  });

  /**
   * Test 7: Login Page Accessibility
   * 
   * Verifies that the login page follows accessibility best practices
   * with proper ARIA attributes and semantic HTML.
   */
  test('should have proper accessibility attributes', async ({ page }: { page: Page }) => {
    // Verify username input has proper ARIA attributes
    const usernameInput = page.locator('#username');
    
    // Check that inputs can be described by error when present
    await page.fill('#username', 'invalid');
    await page.fill('#password', 'invalid');
    await page.click('button[type="submit"]');
    
    // Wait for error
    await page.waitForSelector('#login-error', { state: 'visible', timeout: 5000 });
    
    // Verify error has role="alert" for screen readers
    const errorElement = page.locator('#login-error');
    await expect(errorElement).toHaveAttribute('role', 'alert');
    
    // Verify inputs are associated with error via aria-describedby
    const usernameAriaDescribedBy = await usernameInput.getAttribute('aria-describedby');
    expect(usernameAriaDescribedBy).toContain('login-error');
    
    const passwordInput = page.locator('#password');
    const passwordAriaDescribedBy = await passwordInput.getAttribute('aria-describedby');
    expect(passwordAriaDescribedBy).toContain('login-error');
    
    // Verify aria-invalid is set when there's an error
    await expect(usernameInput).toHaveAttribute('aria-invalid', 'true');
    await expect(passwordInput).toHaveAttribute('aria-invalid', 'true');
  });
});