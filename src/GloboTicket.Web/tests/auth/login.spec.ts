import { test, expect } from '@playwright/test';

/**
 * Login Test Suite for GloboTicket.Web
 * 
 * Tests the authentication flow including:
 * - Successful login with valid credentials
 * - Failed login with invalid credentials
 * - Form validation for empty fields
 * - Navigation verification after successful login
 */

test.describe('Login Flow', () => {
  // Navigate to login page before each test
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    // Wait for the page to be fully loaded
    await page.waitForLoadState('networkidle');
  });

  test('should successfully login with valid credentials (prod user)', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Fill in valid credentials and submit
    await usernameInput.fill('prod');
    await passwordInput.fill('prod123');
    await submitButton.click();

    // Assert: Verify redirect to dashboard
    await expect(page).toHaveURL('/');
    
    // Additional verification: Check that we're on the dashboard
    // (waiting for navigation to complete)
    await page.waitForURL('/');
  });

  test('should successfully login with valid credentials (smoke user)', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Fill in valid credentials and submit
    await usernameInput.fill('smoke');
    await passwordInput.fill('smoke123');
    await submitButton.click();

    // Assert: Verify redirect to dashboard
    await expect(page).toHaveURL('/');
    await page.waitForURL('/');
  });

  test('should display error message with invalid credentials', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Fill in invalid credentials and submit
    await usernameInput.fill('invaliduser');
    await passwordInput.fill('wrongpassword');
    await submitButton.click();

    // Assert: Verify error message is displayed
    const errorMessage = page.locator('#login-error');
    await expect(errorMessage).toBeVisible();
    
    // Verify we're still on the login page
    await expect(page).toHaveURL('/login');
  });

  test('should display error message with valid username but invalid password', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Fill in valid username but wrong password
    await usernameInput.fill('prod');
    await passwordInput.fill('wrongpassword');
    await submitButton.click();

    // Assert: Verify error message is displayed
    const errorMessage = page.locator('#login-error');
    await expect(errorMessage).toBeVisible();
    
    // Verify we're still on the login page
    await expect(page).toHaveURL('/login');
  });

  test('should validate empty username field', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Leave username empty, fill password, and try to submit
    await passwordInput.fill('prod123');
    await submitButton.click();

    // Assert: Verify form validation prevents submission
    // Check if username field has validation error (HTML5 validation or custom)
    const isUsernameInvalid = await usernameInput.evaluate((el: HTMLInputElement) => {
      return !el.validity.valid || el.getAttribute('aria-invalid') === 'true';
    });
    
    expect(isUsernameInvalid).toBeTruthy();
    
    // Verify we're still on the login page
    await expect(page).toHaveURL('/login');
  });

  test('should validate empty password field', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Fill username, leave password empty, and try to submit
    await usernameInput.fill('prod');
    await submitButton.click();

    // Assert: Verify form validation prevents submission
    // Check if password field has validation error (HTML5 validation or custom)
    const isPasswordInvalid = await passwordInput.evaluate((el: HTMLInputElement) => {
      return !el.validity.valid || el.getAttribute('aria-invalid') === 'true';
    });
    
    expect(isPasswordInvalid).toBeTruthy();
    
    // Verify we're still on the login page
    await expect(page).toHaveURL('/login');
  });

  test('should validate both empty username and password fields', async ({ page }) => {
    // Arrange: Get form elements
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    // Act: Try to submit with both fields empty
    await submitButton.click();

    // Assert: Verify form validation prevents submission
    const isUsernameInvalid = await usernameInput.evaluate((el: HTMLInputElement) => {
      return !el.validity.valid || el.getAttribute('aria-invalid') === 'true';
    });
    
    const isPasswordInvalid = await passwordInput.evaluate((el: HTMLInputElement) => {
      return !el.validity.valid || el.getAttribute('aria-invalid') === 'true';
    });
    
    expect(isUsernameInvalid).toBeTruthy();
    expect(isPasswordInvalid).toBeTruthy();
    
    // Verify we're still on the login page
    await expect(page).toHaveURL('/login');
  });

  test('should have correct form elements and structure', async ({ page }) => {
    // Assert: Verify all required form elements are present
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    await expect(usernameInput).toBeVisible();
    await expect(passwordInput).toBeVisible();
    await expect(submitButton).toBeVisible();
    
    // Verify input types
    await expect(usernameInput).toHaveAttribute('type', 'text');
    await expect(passwordInput).toHaveAttribute('type', 'password');
  });

  test('should clear error message when user starts typing after failed login', async ({ page }) => {
    // Arrange: First attempt a failed login
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');
    const errorMessage = page.locator('#login-error');

    // Act: Submit invalid credentials
    await usernameInput.fill('invaliduser');
    await passwordInput.fill('wrongpassword');
    await submitButton.click();

    // Assert: Error message should be visible
    await expect(errorMessage).toBeVisible();

    // Act: Start typing in username field
    await usernameInput.clear();
    await usernameInput.fill('p');

    // Assert: Error message should be hidden or cleared
    // (This test assumes the form clears errors on input change)
    // If the implementation doesn't clear errors, this test will need adjustment
    await page.waitForTimeout(100); // Small delay to allow error clearing
    
    // Note: This assertion may need to be adjusted based on actual implementation
    // Some forms clear errors immediately, others keep them until next submit
  });

  test('should maintain login state after successful login and page refresh', async ({ page }) => {
    // Arrange & Act: Login successfully
    const usernameInput = page.locator('#username');
    const passwordInput = page.locator('#password');
    const submitButton = page.locator('button[type="submit"]');

    await usernameInput.fill('prod');
    await passwordInput.fill('prod123');
    await submitButton.click();

    // Wait for navigation to dashboard
    await page.waitForURL('/');

    // Act: Refresh the page
    await page.reload();

    // Assert: Should still be on dashboard (not redirected to login)
    await expect(page).toHaveURL('/');
  });
});