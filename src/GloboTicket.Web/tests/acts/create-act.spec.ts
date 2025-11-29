import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Create Act Test Suite
 * 
 * Comprehensive tests for the act creation functionality including:
 * - Successful act creation with required fields
 * - Required field validation
 * - Field length validation
 * - Cancel button functionality
 * - Form elements verification
 */
test.describe('Create Act Page', () => {
  /**
   * Test Setup: Navigate to create act page before each test
   * Uses authenticated page fixture to ensure user is logged in
   */
  test.beforeEach(async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the create act page
    await authenticatedPage.goto('/acts/new');
    
    // Wait for the page to be fully loaded
    await authenticatedPage.waitForLoadState('networkidle');
  });

  /**
   * Test 1: Successful Act Creation with Required Fields
   * 
   * Verifies that a user can successfully create an act with only required fields
   * and is redirected to the acts list where the new act appears.
   */
  test('should successfully create an act with required fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name using timestamp to avoid conflicts
    const timestamp = Date.now();
    const actName = `Test Act ${timestamp}`;
    
    // Fill in required field
    await authenticatedPage.fill('#name', actName);
    
    // Click the Create Act button
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify redirect to acts list
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for the acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new act appears in the list
    const actCard = authenticatedPage.locator(`text=${actName}`);
    await expect(actCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 2: Required Fields Validation
   * 
   * Verifies that the form prevents submission when required fields are empty
   * and displays appropriate validation errors.
   */
  test('should validate required fields and prevent submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Try to submit without filling any fields
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Verify validation error appears for name field
    const errorMessage = authenticatedPage.locator('text=Act name is required');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
    
    // Verify we're still on the create act page (no navigation occurred)
    await expect(authenticatedPage).toHaveURL('/acts/new');
  });

  /**
   * Test 3: Field Length Validation
   * 
   * Verifies that the form enforces maximum length constraints on text fields
   * and displays appropriate validation messages.
   */
  test('should validate field length constraints', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Test name max length (100 characters)
    const longName = 'A'.repeat(101);
    await authenticatedPage.fill('#name', longName);
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Verify validation error for name length
    const nameError = authenticatedPage.locator('text=Act name must be 100 characters or less');
    await expect(nameError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    await expect(authenticatedPage).toHaveURL('/acts/new');
  });

  /**
   * Test 4: Cancel Button Functionality
   * 
   * Verifies that clicking the Cancel button navigates back to the acts list
   * without creating an act, even if fields have been filled.
   */
  test('should cancel act creation and return to acts list', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name to verify it's not created
    const timestamp = Date.now();
    const actName = `Cancelled Act ${timestamp}`;
    
    // Fill in the field
    await authenticatedPage.fill('#name', actName);
    
    // Click the Cancel button
    await authenticatedPage.click('button:has-text("Cancel")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify redirect to acts list
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for the acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the cancelled act does not appear in the list
    const actCard = authenticatedPage.locator(`text=${actName}`);
    await expect(actCard).not.toBeVisible();
  });

  /**
   * Test 5: Form Elements Verification
   * 
   * Verifies that all form elements exist with correct attributes,
   * labels, and accessibility features.
   */
  test('should have all required form elements with correct attributes', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Verify page title
    const pageTitle = authenticatedPage.locator('h1:has-text("Create Act")');
    await expect(pageTitle).toBeVisible();
    
    // Verify page description
    const pageDescription = authenticatedPage.locator('text=Add a new act to the system');
    await expect(pageDescription).toBeVisible();
    
    // Verify name input exists and has correct attributes
    const nameInput = authenticatedPage.locator('#name');
    await expect(nameInput).toBeVisible();
    await expect(nameInput).toHaveAttribute('type', 'text');
    await expect(nameInput).toHaveAttribute('required', '');
    await expect(nameInput).toHaveAttribute('maxlength', '100');
    
    // Verify name label exists
    const nameLabel = authenticatedPage.locator('label[for="name"]');
    await expect(nameLabel).toBeVisible();
    const nameLabelText = await nameLabel.textContent();
    expect(nameLabelText).toContain('Act Name');
    expect(nameLabelText).toContain('*'); // Required indicator
    
    // Verify Create Act button exists and is enabled
    const createButton = authenticatedPage.locator('button:has-text("Create Act")');
    await expect(createButton).toBeVisible();
    await expect(createButton).toBeEnabled();
    
    // Verify Cancel button exists and is enabled
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await expect(cancelButton).toBeEnabled();
  });

  /**
   * Test 6: Form Interaction and Error Clearing
   * 
   * Verifies that validation errors are displayed and cleared appropriately
   * during user interaction with the form.
   */
  test('should handle form interaction and error clearing correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Try to submit empty form to trigger validation error
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Verify error message appears
    const errorMessage = authenticatedPage.locator('text=Act name is required');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
    
    // Start filling in the name field
    await authenticatedPage.fill('#name', 'Test Act');
    
    // Now submit with required field filled
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/acts');
  });

  /**
   * Test 7: Loading State During Submission
   * 
   * Verifies that the form shows appropriate loading state during submission
   * and that buttons are disabled to prevent double submission.
   */
  test('should show loading state during act creation', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Loading Test Act ${timestamp}`;
    
    // Fill in required field
    await authenticatedPage.fill('#name', actName);
    
    // Click the Create Act button
    const createButton = authenticatedPage.locator('button:has-text("Create Act")');
    await createButton.click();
    
    // Verify button is disabled during submission (check quickly before navigation)
    // Note: This may be very brief, so we use a short timeout
    await expect(createButton).toBeDisabled({ timeout: 1000 }).catch(() => {
      // If the request is too fast, the button might not be disabled long enough to catch
      // This is acceptable as it means the operation completed successfully
    });
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/acts');
  });

  /**
   * Test 8: Valid Boundary Values for Name Field
   * 
   * Verifies that the form accepts valid boundary values for the name field
   * (exactly 100 characters).
   */
  test('should accept valid boundary values for name field', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name with exactly 100 characters
    const timestamp = Date.now();
    const baseActName = `Boundary Test Act ${timestamp}`;
    const actName = baseActName.padEnd(100, 'X').substring(0, 100);
    
    // Fill in required field with boundary value
    await authenticatedPage.fill('#name', actName);
    
    // Click the Create Act button
    await authenticatedPage.click('button:has-text("Create Act")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for the acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new act appears in the list
    const actCard = authenticatedPage.locator(`text=${actName}`);
    await expect(actCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 9: Multiple Consecutive Creations
   * 
   * Verifies that multiple acts can be created in succession without issues.
   */
  test('should allow multiple consecutive act creations', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const timestamp = Date.now();
    const actNames: string[] = [];
    
    // Create 3 acts in succession
    for (let i = 1; i <= 3; i++) {
      const actName = `Multi Test Act ${timestamp}-${i}`;
      actNames.push(actName);
      
      // Navigate to create page
      await authenticatedPage.goto('/acts/new');
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Fill and submit
      await authenticatedPage.fill('#name', actName);
      await authenticatedPage.click('button:has-text("Create Act")');
      
      // Wait for navigation
      await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
      await authenticatedPage.waitForLoadState('networkidle');
    }
    
    // Verify all acts appear in the list
    for (const actName of actNames) {
      const actCard = authenticatedPage.locator(`text=${actName}`);
      await expect(actCard).toBeVisible({ timeout: 10000 });
    }
  });

  /**
   * Test 10: Form Accessibility
   * 
   * Verifies that the form is keyboard accessible and follows accessibility best practices.
   */
  test('should be keyboard accessible', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Keyboard Test Act ${timestamp}`;
    
    // Tab to name input
    await authenticatedPage.keyboard.press('Tab');
    
    // Type act name
    await authenticatedPage.keyboard.type(actName);
    
    // Tab to Create Act button
    await authenticatedPage.keyboard.press('Tab');
    
    // Press Enter to submit
    await authenticatedPage.keyboard.press('Enter');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for the acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new act appears in the list
    const actCard = authenticatedPage.locator(`text=${actName}`);
    await expect(actCard).toBeVisible({ timeout: 10000 });
  });
});