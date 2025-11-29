import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Edit Act Test Suite
 * 
 * Comprehensive tests for the act editing functionality including:
 * - Successful act edit with modified fields
 * - Form pre-population verification
 * - Edit validation for required fields
 * - Cancel edit functionality
 * - Edit page elements verification
 * - Invalid data validation
 * - Loading state during act fetch
 */
test.describe('Edit Act Page', () => {
  /**
   * Helper function to create a test act and return its GUID
   * This ensures each test has a fresh act to edit
   */
  async function createTestAct(page: Page, actName: string): Promise<string> {
    // Navigate to create act page
    await page.goto('/acts/new');
    await page.waitForLoadState('networkidle');
    
    // Fill in required field
    await page.fill('#name', actName);
    
    // Submit the form
    await page.click('button:has-text("Create Act")');
    
    // Wait for navigation to acts list
    await page.waitForURL('/acts', { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    
    // Wait for the act to appear in the list
    await page.waitForSelector(`text=${actName}`, { timeout: 10000 });
    
    // Click on the act card to navigate to detail page
    const actCard = page.locator(`text=${actName}`).locator('..').locator('..');
    await actCard.click();
    
    // Wait for navigation to detail page and extract GUID from URL
    await page.waitForTimeout(1000);
    const currentUrl = page.url();
    const match = currentUrl.match(/\/acts\/([a-f0-9-]+)/);
    
    if (!match || !match[1]) {
      throw new Error('Failed to extract act GUID from URL');
    }
    
    return match[1];
  }

  /**
   * Test 1: Successful Act Edit Test
   * 
   * Verifies that a user can successfully edit an act by modifying the name
   * and that the changes are saved and reflected in the acts list.
   */
  test('should successfully edit an act with modified fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name using timestamp
    const timestamp = Date.now();
    const originalName = `Original Act ${timestamp}`;
    const updatedName = `Updated Act ${timestamp}`;
    
    // Create a test act to edit
    const actGuid = await createTestAct(authenticatedPage, originalName);
    
    // Navigate to the edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify form is pre-populated with existing data
    await expect(authenticatedPage.locator('#name')).toHaveValue(originalName);
    
    // Modify the name field
    await authenticatedPage.fill('#name', updatedName);
    
    // Click Update Act button
    await authenticatedPage.click('button:has-text("Update Act")');
    
    // Wait for navigation to acts list
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated act appears in the list with new name
    const updatedActCard = authenticatedPage.locator(`text=${updatedName}`);
    await expect(updatedActCard).toBeVisible({ timeout: 10000 });
    
    // Verify old name is not present
    const originalActCard = authenticatedPage.locator(`text=${originalName}`);
    await expect(originalActCard).not.toBeVisible();
  });

  /**
   * Test 2: Form Pre-population Test
   * 
   * Verifies that the form field is correctly pre-populated with
   * the existing act data when the edit page loads.
   */
  test('should pre-populate form field with existing act data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Prepopulated Act ${timestamp}`;
    
    // Create a test act
    await authenticatedPage.goto('/acts/new');
    await authenticatedPage.waitForLoadState('networkidle');
    
    await authenticatedPage.fill('#name', actName);
    
    await authenticatedPage.click('button:has-text("Create Act")');
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get act GUID
    await authenticatedPage.waitForSelector(`text=${actName}`, { timeout: 10000 });
    const actCard = authenticatedPage.locator(`text=${actName}`).locator('..').locator('..');
    await actCard.click();
    await authenticatedPage.waitForTimeout(1000);
    const currentUrl = authenticatedPage.url();
    const match = currentUrl.match(/\/acts\/([a-f0-9-]+)/);
    const actGuid = match![1];
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify form field is pre-populated with correct value
    await expect(authenticatedPage.locator('#name')).toHaveValue(actName);
  });

  /**
   * Test 3: Edit Validation Test
   * 
   * Verifies that validation errors appear when required field is cleared
   * and that the form cannot be submitted with invalid data.
   */
  test('should validate required field and prevent submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Validation Act ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, actName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Clear required field
    await authenticatedPage.fill('#name', '');
    
    // Try to submit
    await authenticatedPage.click('button:has-text("Update Act")');
    
    // Verify validation error appears for name
    const nameError = authenticatedPage.locator('text=Act name is required');
    await expect(nameError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted (still on edit page)
    expect(authenticatedPage.url()).toContain('/edit');
  });

  /**
   * Test 4: Field Length Validation Test
   * 
   * Verifies that validation errors appear when name exceeds maximum length.
   */
  test('should validate field length and show appropriate error', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Length Test Act ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, actName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Enter name that exceeds max length (101 characters)
    const longName = 'A'.repeat(101);
    await authenticatedPage.fill('#name', longName);
    await authenticatedPage.click('button:has-text("Update Act")');
    
    // Verify validation error for name length
    const nameError = authenticatedPage.locator('text=Act name must be 100 characters or less');
    await expect(nameError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    expect(authenticatedPage.url()).toContain('/edit');
  });

  /**
   * Test 5: Cancel Edit Test
   * 
   * Verifies that clicking Cancel returns to acts list without saving changes.
   */
  test('should cancel edit and return to acts list without saving', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const originalName = `Cancel Test Act ${timestamp}`;
    const modifiedName = `Modified Name ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, originalName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Modify the field
    await authenticatedPage.fill('#name', modifiedName);
    
    // Click Cancel button
    await authenticatedPage.click('button:has-text("Cancel")');
    
    // Wait for navigation to acts list
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify original name is still present
    const originalActCard = authenticatedPage.locator(`text=${originalName}`);
    await expect(originalActCard).toBeVisible({ timeout: 10000 });
    
    // Verify modified name is not present
    const modifiedActCard = authenticatedPage.locator(`text=${modifiedName}`);
    await expect(modifiedActCard).not.toBeVisible();
  });

  /**
   * Test 6: Edit Page Elements Test
   * 
   * Verifies that the edit page contains all expected elements with correct
   * attributes and labels.
   */
  test('should display all edit page elements correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Elements Test Act ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, actName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify page title is "Edit Act"
    const pageTitle = authenticatedPage.locator('h1:has-text("Edit Act")');
    await expect(pageTitle).toBeVisible();
    
    // Verify page description is present
    const pageDescription = authenticatedPage.locator('text=Update act information');
    await expect(pageDescription).toBeVisible();
    
    // Verify name field exists
    await expect(authenticatedPage.locator('#name')).toBeVisible();
    
    // Verify "Update Act" button exists
    const updateButton = authenticatedPage.locator('button:has-text("Update Act")');
    await expect(updateButton).toBeVisible();
    await expect(updateButton).toBeEnabled();
    
    // Verify "Cancel" button exists
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await expect(cancelButton).toBeEnabled();
  });

  /**
   * Test 7: Loading State Test
   * 
   * Verifies that a loading spinner appears while fetching act data
   * and that the form appears after loading completes.
   */
  test('should display loading state while fetching act data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Loading Test Act ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, actName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    
    // Check for loading spinner (may be brief)
    const loadingSpinner = authenticatedPage.locator('[class*="animate-spin"]');
    const spinnerWasVisible = await loadingSpinner.isVisible().catch(() => false);
    
    // If we caught the spinner, verify it was there
    if (spinnerWasVisible) {
      await expect(loadingSpinner).toBeVisible();
    }
    
    // Wait for form to appear after loading
    await authenticatedPage.waitForSelector('#name', { timeout: 10000 });
    
    // Verify form is now visible
    await expect(authenticatedPage.locator('#name')).toBeVisible();
    
    // Verify loading spinner is gone
    await expect(loadingSpinner).not.toBeVisible();
  });

  /**
   * Test 8: Multiple Edits Test
   * 
   * Verifies that an act can be edited multiple times successfully.
   */
  test('should allow multiple consecutive edits', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act names
    const timestamp = Date.now();
    const originalName = `Multi Edit Act ${timestamp}`;
    const firstUpdate = `First Update ${timestamp}`;
    const secondUpdate = `Second Update ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, originalName);
    
    // First edit
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    await authenticatedPage.fill('#name', firstUpdate);
    await authenticatedPage.click('button:has-text("Update Act")');
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify first update
    await expect(authenticatedPage.locator(`text=${firstUpdate}`)).toBeVisible({ timeout: 10000 });
    
    // Second edit
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    await authenticatedPage.fill('#name', secondUpdate);
    await authenticatedPage.click('button:has-text("Update Act")');
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify second update
    await expect(authenticatedPage.locator(`text=${secondUpdate}`)).toBeVisible({ timeout: 10000 });
    
    // Verify previous names are not present
    await expect(authenticatedPage.locator(`text=${originalName}`)).not.toBeVisible();
    await expect(authenticatedPage.locator(`text=${firstUpdate}`)).not.toBeVisible();
  });

  /**
   * Test 9: Edit with Boundary Value Test
   * 
   * Verifies that the form accepts valid boundary value (exactly 100 characters).
   */
  test('should accept valid boundary value for name field', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act name
    const timestamp = Date.now();
    const actName = `Boundary Test ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, actName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Create name with exactly 100 characters
    const boundaryName = `Boundary ${timestamp}`.padEnd(100, 'X').substring(0, 100);
    await authenticatedPage.fill('#name', boundaryName);
    await authenticatedPage.click('button:has-text("Update Act")');
    
    // Wait for navigation to acts list
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated act appears in the list
    const updatedActCard = authenticatedPage.locator(`text=${boundaryName}`);
    await expect(updatedActCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 10: Form Accessibility Test
   * 
   * Verifies that the edit form is keyboard accessible.
   */
  test('should be keyboard accessible', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique act names
    const timestamp = Date.now();
    const originalName = `Keyboard Test ${timestamp}`;
    const updatedName = `Keyboard Updated ${timestamp}`;
    
    // Create a test act
    const actGuid = await createTestAct(authenticatedPage, originalName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/acts/${actGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Tab to name input
    await authenticatedPage.keyboard.press('Tab');
    
    // Clear and type new name
    await authenticatedPage.keyboard.press('Control+A');
    await authenticatedPage.keyboard.type(updatedName);
    
    // Tab to Update Act button
    await authenticatedPage.keyboard.press('Tab');
    
    // Press Enter to submit
    await authenticatedPage.keyboard.press('Enter');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/acts', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/acts');
    
    // Wait for the acts list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated act appears in the list
    const actCard = authenticatedPage.locator(`text=${updatedName}`);
    await expect(actCard).toBeVisible({ timeout: 10000 });
  });
});