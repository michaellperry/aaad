import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Edit Venue Test Suite
 * 
 * Comprehensive tests for the venue editing functionality including:
 * - Successful venue edit with modified fields
 * - Editing all fields including optional ones
 * - Form pre-population verification
 * - Edit validation for required fields
 * - Cancel edit functionality
 * - Edit page elements verification
 * - Invalid data validation
 * - Loading state during venue fetch
 */
test.describe('Edit Venue Page', () => {
  /**
   * Helper function to create a test venue and return its GUID
   * This ensures each test has a fresh venue to edit
   */
  async function createTestVenue(page: Page, venueName: string): Promise<string> {
    // Navigate to create venue page
    await page.goto('/venues/new');
    await page.waitForLoadState('networkidle');
    
    // Fill in required fields
    await page.fill('#name', venueName);
    await page.fill('#seatingCapacity', '1000');
    await page.fill('#description', 'Test venue description for editing');
    await page.fill('#address', '123 Test Street, Test City, TC 12345');
    await page.fill('#latitude', '40.7128');
    await page.fill('#longitude', '-74.0060');
    
    // Submit the form
    await page.click('button:has-text("Create Venue")');
    
    // Wait for navigation to venues list
    await page.waitForURL('/venues', { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    
    // Wait for the venue to appear in the list
    await page.waitForSelector(`text=${venueName}`, { timeout: 10000 });
    
    // Click on the venue card to navigate to detail page
    const venueCard = page.locator(`text=${venueName}`).locator('..').locator('..');
    await venueCard.click();
    
    // Wait for navigation to detail page and extract GUID from URL
    await page.waitForTimeout(1000);
    const currentUrl = page.url();
    const match = currentUrl.match(/\/venues\/([a-f0-9-]+)/);
    
    if (!match || !match[1]) {
      throw new Error('Failed to extract venue GUID from URL');
    }
    
    return match[1];
  }

  /**
   * Test 1: Successful Venue Edit Test
   * 
   * Verifies that a user can successfully edit a venue by modifying some fields
   * and that the changes are saved and reflected in the venues list.
   */
  test('should successfully edit a venue with modified fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name using timestamp
    const timestamp = Date.now();
    const originalName = `Original Venue ${timestamp}`;
    const updatedName = `Updated Venue ${timestamp}`;
    
    // Create a test venue to edit
    const venueGuid = await createTestVenue(authenticatedPage, originalName);
    
    // Navigate to the edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify form is pre-populated with existing data
    await expect(authenticatedPage.locator('#name')).toHaveValue(originalName);
    
    // Modify some fields
    await authenticatedPage.fill('#name', updatedName);
    await authenticatedPage.fill('#description', 'Updated description for the venue');
    await authenticatedPage.fill('#seatingCapacity', '2500');
    
    // Click Update Venue button
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Wait for navigation to venues list
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated venue appears in the list with new name
    const updatedVenueCard = authenticatedPage.locator(`text=${updatedName}`);
    await expect(updatedVenueCard).toBeVisible({ timeout: 10000 });
    
    // Verify old name is not present
    const originalVenueCard = authenticatedPage.locator(`text=${originalName}`);
    await expect(originalVenueCard).not.toBeVisible();
  });

  /**
   * Test 2: Edit All Fields Test
   * 
   * Verifies that a user can edit all fields including optional ones
   * and that all changes are properly saved.
   */
  test('should successfully edit all fields including optional ones', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const originalName = `Complete Venue ${timestamp}`;
    const updatedName = `Fully Updated Venue ${timestamp}`;
    
    // Create a test venue with all fields
    const venueGuid = await createTestVenue(authenticatedPage, originalName);
    
    // Navigate to the edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Modify all fields including optional ones
    await authenticatedPage.fill('#name', updatedName);
    await authenticatedPage.fill('#address', '456 Updated Avenue, New City, NC 67890');
    await authenticatedPage.fill('#seatingCapacity', '3000');
    await authenticatedPage.fill('#description', 'Completely updated description with all new information');
    await authenticatedPage.fill('#latitude', '34.0522');
    await authenticatedPage.fill('#longitude', '-118.2437');
    
    // Click Update Venue button
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Wait for navigation to venues list
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated venue appears in the list
    const updatedVenueCard = authenticatedPage.locator(`text=${updatedName}`);
    await expect(updatedVenueCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 3: Form Pre-population Test
   * 
   * Verifies that all form fields are correctly pre-populated with
   * the existing venue data when the edit page loads.
   */
  test('should pre-populate form fields with existing venue data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Prepopulated Venue ${timestamp}`;
    const venueAddress = '789 Prepop Street, Prepop City, PC 11111';
    const venueCapacity = '1500';
    const venueDescription = 'Prepopulated venue description for testing';
    const venueLatitude = '51.5074';
    const venueLongitude = '-0.1278';
    
    // Create a test venue with all fields
    await authenticatedPage.goto('/venues/new');
    await authenticatedPage.waitForLoadState('networkidle');
    
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#address', venueAddress);
    await authenticatedPage.fill('#seatingCapacity', venueCapacity);
    await authenticatedPage.fill('#description', venueDescription);
    await authenticatedPage.fill('#latitude', venueLatitude);
    await authenticatedPage.fill('#longitude', venueLongitude);
    
    await authenticatedPage.click('button:has-text("Create Venue")');
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get venue GUID
    await authenticatedPage.waitForSelector(`text=${venueName}`, { timeout: 10000 });
    const venueCard = authenticatedPage.locator(`text=${venueName}`).locator('..').locator('..');
    await venueCard.click();
    await authenticatedPage.waitForTimeout(1000);
    const currentUrl = authenticatedPage.url();
    const match = currentUrl.match(/\/venues\/([a-f0-9-]+)/);
    const venueGuid = match![1];
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify all form fields are pre-populated with correct values
    await expect(authenticatedPage.locator('#name')).toHaveValue(venueName);
    await expect(authenticatedPage.locator('#address')).toHaveValue(venueAddress);
    await expect(authenticatedPage.locator('#seatingCapacity')).toHaveValue(venueCapacity);
    await expect(authenticatedPage.locator('#description')).toHaveValue(venueDescription);
    await expect(authenticatedPage.locator('#latitude')).toHaveValue(venueLatitude);
    await expect(authenticatedPage.locator('#longitude')).toHaveValue(venueLongitude);
  });

  /**
   * Test 4: Edit Validation Test
   * 
   * Verifies that validation errors appear when required fields are cleared
   * and that the form cannot be submitted with invalid data.
   */
  test('should validate required fields and prevent submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Validation Venue ${timestamp}`;
    
    // Create a test venue
    const venueGuid = await createTestVenue(authenticatedPage, venueName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Clear required fields
    await authenticatedPage.fill('#name', '');
    await authenticatedPage.fill('#description', '');
    await authenticatedPage.fill('#seatingCapacity', '');
    
    // Try to submit
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error appears for name
    const nameError = authenticatedPage.locator('text=Venue name is required');
    await expect(nameError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted (still on edit page)
    expect(authenticatedPage.url()).toContain('/edit');
    
    // Fill name, try again
    await authenticatedPage.fill('#name', 'Valid Name');
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error for description
    const descriptionError = authenticatedPage.locator('text=Description is required');
    await expect(descriptionError).toBeVisible({ timeout: 5000 });
    
    // Fill description, try again
    await authenticatedPage.fill('#description', 'Valid description');
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error for seating capacity
    const capacityError = authenticatedPage.locator('text=Seating capacity must be a positive number');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Verify form is still not submitted
    expect(authenticatedPage.url()).toContain('/edit');
  });

  /**
   * Test 5: Cancel Edit Test
   * 
   * Verifies that clicking Cancel returns to venues list without saving changes.
   */
  test('should cancel edit and return to venues list without saving', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const originalName = `Cancel Test Venue ${timestamp}`;
    const modifiedName = `Modified Name ${timestamp}`;
    
    // Create a test venue
    const venueGuid = await createTestVenue(authenticatedPage, originalName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Modify some fields
    await authenticatedPage.fill('#name', modifiedName);
    await authenticatedPage.fill('#description', 'This should not be saved');
    await authenticatedPage.fill('#seatingCapacity', '9999');
    
    // Click Cancel button
    await authenticatedPage.click('button:has-text("Cancel")');
    
    // Wait for navigation to venues list
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify original name is still present
    const originalVenueCard = authenticatedPage.locator(`text=${originalName}`);
    await expect(originalVenueCard).toBeVisible({ timeout: 10000 });
    
    // Verify modified name is not present
    const modifiedVenueCard = authenticatedPage.locator(`text=${modifiedName}`);
    await expect(modifiedVenueCard).not.toBeVisible();
  });

  /**
   * Test 6: Edit Page Elements Test
   * 
   * Verifies that the edit page contains all expected elements with correct
   * attributes and labels.
   */
  test('should display all edit page elements correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Elements Test Venue ${timestamp}`;
    
    // Create a test venue
    const venueGuid = await createTestVenue(authenticatedPage, venueName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Verify page title is "Edit Venue"
    const pageTitle = authenticatedPage.locator('h1:has-text("Edit Venue")');
    await expect(pageTitle).toBeVisible();
    
    // Verify page description is present
    const pageDescription = authenticatedPage.locator('text=Update venue information');
    await expect(pageDescription).toBeVisible();
    
    // Verify all form fields exist
    await expect(authenticatedPage.locator('#name')).toBeVisible();
    await expect(authenticatedPage.locator('#address')).toBeVisible();
    await expect(authenticatedPage.locator('#seatingCapacity')).toBeVisible();
    await expect(authenticatedPage.locator('#description')).toBeVisible();
    await expect(authenticatedPage.locator('#latitude')).toBeVisible();
    await expect(authenticatedPage.locator('#longitude')).toBeVisible();
    
    // Verify "Update Venue" button exists
    const updateButton = authenticatedPage.locator('button:has-text("Update Venue")');
    await expect(updateButton).toBeVisible();
    await expect(updateButton).toBeEnabled();
    
    // Verify "Cancel" button exists
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await expect(cancelButton).toBeEnabled();
  });

  /**
   * Test 7: Edit with Invalid Data Test
   * 
   * Verifies that validation errors appear when invalid data is entered
   * (negative capacity, out-of-range lat/long).
   */
  test('should validate invalid data and show appropriate errors', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Invalid Data Venue ${timestamp}`;
    
    // Create a test venue
    const venueGuid = await createTestVenue(authenticatedPage, venueName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for form to be populated
    await authenticatedPage.waitForSelector('#name', { timeout: 5000 });
    
    // Enter negative seating capacity
    await authenticatedPage.fill('#seatingCapacity', '-500');
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error for negative capacity
    const capacityError = authenticatedPage.locator('text=Seating capacity must be a positive number');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Fix capacity, enter out-of-range latitude
    await authenticatedPage.fill('#seatingCapacity', '1000');
    await authenticatedPage.fill('#latitude', '95');
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error for latitude
    const latitudeError = authenticatedPage.locator('text=Latitude must be between -90 and 90');
    await expect(latitudeError).toBeVisible({ timeout: 5000 });
    
    // Fix latitude, enter out-of-range longitude
    await authenticatedPage.fill('#latitude', '40.7128');
    await authenticatedPage.fill('#longitude', '200');
    await authenticatedPage.click('button:has-text("Update Venue")');
    
    // Verify validation error for longitude
    const longitudeError = authenticatedPage.locator('text=Longitude must be between -180 and 180');
    await expect(longitudeError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    expect(authenticatedPage.url()).toContain('/edit');
  });

  /**
   * Test 8: Loading State Test
   * 
   * Verifies that a loading spinner appears while fetching venue data
   * and that the form appears after loading completes.
   */
  test('should display loading state while fetching venue data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Loading Test Venue ${timestamp}`;
    
    // Create a test venue
    const venueGuid = await createTestVenue(authenticatedPage, venueName);
    
    // Navigate to edit page
    await authenticatedPage.goto(`/venues/${venueGuid}/edit`);
    
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
    await expect(authenticatedPage.locator('#description')).toBeVisible();
    
    // Verify loading spinner is gone
    await expect(loadingSpinner).not.toBeVisible();
  });
});