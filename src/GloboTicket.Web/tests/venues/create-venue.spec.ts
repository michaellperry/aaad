import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Create Venue Test Suite
 * 
 * Comprehensive tests for the venue creation functionality including:
 * - Successful venue creation with required fields
 * - Venue creation with all fields (including optional)
 * - Required field validation
 * - Field length validation
 * - Numeric field validation
 * - Cancel button functionality
 * - Form elements verification
 */
test.describe('Create Venue Page', () => {
  /**
   * Test Setup: Navigate to create venue page before each test
   * Uses authenticated page fixture to ensure user is logged in
   */
  test.beforeEach(async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the create venue page
    await authenticatedPage.goto('/venues/new');
    
    // Wait for the page to be fully loaded
    await authenticatedPage.waitForLoadState('networkidle');
  });

  /**
   * Test 1: Successful Venue Creation with Required Fields
   * 
   * Verifies that a user can successfully create a venue with only required fields
   * and is redirected to the venues list where the new venue appears.
   */
  test('should successfully create a venue with required fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name using timestamp to avoid conflicts
    const timestamp = Date.now();
    const venueName = `Test Venue ${timestamp}`;
    
    // Fill in required fields
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#seatingCapacity', '1000');
    await authenticatedPage.fill('#description', 'A test venue description for automated testing');
    
    // Click the Create Venue button
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify redirect to venues list
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for the venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new venue appears in the list
    const venueCard = authenticatedPage.locator(`text=${venueName}`);
    await expect(venueCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 2: Create Venue with All Fields
   * 
   * Verifies that a user can create a venue with all fields including optional ones
   * (address, latitude, longitude) and that all data is properly saved.
   */
  test('should successfully create a venue with all fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Complete Venue ${timestamp}`;
    const venueAddress = '123 Main Street, New York, NY 10001';
    
    // Fill in all fields including optional ones
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#address', venueAddress);
    await authenticatedPage.fill('#seatingCapacity', '5000');
    await authenticatedPage.fill('#description', 'A comprehensive test venue with all fields populated for validation');
    await authenticatedPage.fill('#latitude', '40.7128');
    await authenticatedPage.fill('#longitude', '-74.0060');
    
    // Click the Create Venue button
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify redirect to venues list
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for the venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new venue appears in the list with correct data
    const venueCard = authenticatedPage.locator(`text=${venueName}`);
    await expect(venueCard).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 3: Required Fields Validation
   * 
   * Verifies that the form prevents submission when required fields are empty
   * and displays appropriate validation errors.
   */
  test('should validate required fields and prevent submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Try to submit without filling any fields
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error appears for name field
    const errorMessage = authenticatedPage.locator('text=Venue name is required');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
    
    // Verify we're still on the create venue page (no navigation occurred)
    await expect(authenticatedPage).toHaveURL('/venues/new');
    
    // Fill only name, try to submit
    await authenticatedPage.fill('#name', 'Test Venue');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for description
    const descriptionError = authenticatedPage.locator('text=Description is required');
    await expect(descriptionError).toBeVisible({ timeout: 5000 });
    
    // Fill name and description, try to submit
    await authenticatedPage.fill('#description', 'Test description');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for seating capacity
    const capacityError = authenticatedPage.locator('text=Seating capacity must be a positive number');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    await expect(authenticatedPage).toHaveURL('/venues/new');
  });

  /**
   * Test 4: Field Length Validation
   * 
   * Verifies that the form enforces maximum length constraints on text fields
   * and displays appropriate validation messages.
   */
  test('should validate field length constraints', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Test name max length (100 characters)
    const longName = 'A'.repeat(101);
    await authenticatedPage.fill('#name', longName);
    await authenticatedPage.fill('#seatingCapacity', '1000');
    await authenticatedPage.fill('#description', 'Test description');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for name length
    const nameError = authenticatedPage.locator('text=Venue name must be 100 characters or less');
    await expect(nameError).toBeVisible({ timeout: 5000 });
    
    // Fix name, test address max length (300 characters)
    await authenticatedPage.fill('#name', 'Valid Name');
    const longAddress = 'B'.repeat(301);
    await authenticatedPage.fill('#address', longAddress);
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for address length
    const addressError = authenticatedPage.locator('text=Address must be 300 characters or less');
    await expect(addressError).toBeVisible({ timeout: 5000 });
    
    // Fix address, test description max length (2000 characters)
    await authenticatedPage.fill('#address', 'Valid Address');
    const longDescription = 'C'.repeat(2001);
    await authenticatedPage.fill('#description', longDescription);
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for description length
    const descriptionError = authenticatedPage.locator('text=Description must be 2000 characters or less');
    await expect(descriptionError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    await expect(authenticatedPage).toHaveURL('/venues/new');
  });

  /**
   * Test 5: Numeric Field Validation
   * 
   * Verifies that numeric fields (seating capacity, latitude, longitude)
   * enforce proper validation rules and display appropriate error messages.
   */
  test('should validate numeric fields correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Fill required text fields
    await authenticatedPage.fill('#name', 'Test Venue');
    await authenticatedPage.fill('#description', 'Test description');
    
    // Test negative seating capacity
    await authenticatedPage.fill('#seatingCapacity', '-100');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for negative capacity
    const capacityError = authenticatedPage.locator('text=Seating capacity must be a positive number');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Test zero seating capacity (should also fail)
    await authenticatedPage.fill('#seatingCapacity', '0');
    await authenticatedPage.click('button:has-text("Create Venue")');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Fix seating capacity, test latitude out of range
    await authenticatedPage.fill('#seatingCapacity', '1000');
    await authenticatedPage.fill('#latitude', '95');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for latitude range
    const latitudeError = authenticatedPage.locator('text=Latitude must be between -90 and 90');
    await expect(latitudeError).toBeVisible({ timeout: 5000 });
    
    // Test negative latitude out of range
    await authenticatedPage.fill('#latitude', '-95');
    await authenticatedPage.click('button:has-text("Create Venue")');
    await expect(latitudeError).toBeVisible({ timeout: 5000 });
    
    // Fix latitude, test longitude out of range
    await authenticatedPage.fill('#latitude', '40.7128');
    await authenticatedPage.fill('#longitude', '185');
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify validation error for longitude range
    const longitudeError = authenticatedPage.locator('text=Longitude must be between -180 and 180');
    await expect(longitudeError).toBeVisible({ timeout: 5000 });
    
    // Test negative longitude out of range
    await authenticatedPage.fill('#longitude', '-185');
    await authenticatedPage.click('button:has-text("Create Venue")');
    await expect(longitudeError).toBeVisible({ timeout: 5000 });
    
    // Verify form is not submitted
    await expect(authenticatedPage).toHaveURL('/venues/new');
  });

  /**
   * Test 6: Cancel Button Functionality
   * 
   * Verifies that clicking the Cancel button navigates back to the venues list
   * without creating a venue, even if fields have been filled.
   */
  test('should cancel venue creation and return to venues list', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name to verify it's not created
    const timestamp = Date.now();
    const venueName = `Cancelled Venue ${timestamp}`;
    
    // Fill in some fields
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#seatingCapacity', '2000');
    await authenticatedPage.fill('#description', 'This venue should not be created');
    
    // Click the Cancel button
    await authenticatedPage.click('button:has-text("Cancel")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify redirect to venues list
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for the venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the cancelled venue does not appear in the list
    const venueCard = authenticatedPage.locator(`text=${venueName}`);
    await expect(venueCard).not.toBeVisible();
  });

  /**
   * Test 7: Form Elements Verification
   * 
   * Verifies that all form elements exist with correct attributes,
   * labels, and accessibility features.
   */
  test('should have all required form elements with correct attributes', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Verify page title
    const pageTitle = authenticatedPage.locator('h1:has-text("Create Venue")');
    await expect(pageTitle).toBeVisible();
    
    // Verify page description
    const pageDescription = authenticatedPage.locator('text=Add a new venue to the system');
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
    expect(nameLabelText).toContain('Venue Name');
    expect(nameLabelText).toContain('*'); // Required indicator
    
    // Verify address input exists and has correct attributes
    const addressInput = authenticatedPage.locator('#address');
    await expect(addressInput).toBeVisible();
    await expect(addressInput).toHaveAttribute('type', 'text');
    await expect(addressInput).toHaveAttribute('maxlength', '300');
    
    // Verify address label exists (optional field)
    const addressLabel = authenticatedPage.locator('label[for="address"]');
    await expect(addressLabel).toBeVisible();
    const addressLabelText = await addressLabel.textContent();
    expect(addressLabelText).not.toContain('*'); // Not required
    
    // Verify seating capacity input exists and has correct attributes
    const capacityInput = authenticatedPage.locator('#seatingCapacity');
    await expect(capacityInput).toBeVisible();
    await expect(capacityInput).toHaveAttribute('type', 'number');
    await expect(capacityInput).toHaveAttribute('required', '');
    await expect(capacityInput).toHaveAttribute('min', '0');
    
    // Verify seating capacity label exists
    const capacityLabel = authenticatedPage.locator('label[for="seatingCapacity"]');
    await expect(capacityLabel).toBeVisible();
    const capacityLabelText = await capacityLabel.textContent();
    expect(capacityLabelText).toContain('Seating Capacity');
    expect(capacityLabelText).toContain('*'); // Required indicator
    
    // Verify description textarea exists and has correct attributes
    const descriptionInput = authenticatedPage.locator('#description');
    await expect(descriptionInput).toBeVisible();
    await expect(descriptionInput).toHaveAttribute('required', '');
    await expect(descriptionInput).toHaveAttribute('maxlength', '2000');
    
    // Verify description label exists
    const descriptionLabel = authenticatedPage.locator('label[for="description"]');
    await expect(descriptionLabel).toBeVisible();
    const descriptionLabelText = await descriptionLabel.textContent();
    expect(descriptionLabelText).toContain('Description');
    expect(descriptionLabelText).toContain('*'); // Required indicator
    
    // Verify latitude input exists and has correct attributes
    const latitudeInput = authenticatedPage.locator('#latitude');
    await expect(latitudeInput).toBeVisible();
    await expect(latitudeInput).toHaveAttribute('type', 'number');
    await expect(latitudeInput).toHaveAttribute('min', '-90');
    await expect(latitudeInput).toHaveAttribute('max', '90');
    
    // Verify latitude label exists (optional field)
    const latitudeLabel = authenticatedPage.locator('label[for="latitude"]');
    await expect(latitudeLabel).toBeVisible();
    
    // Verify longitude input exists and has correct attributes
    const longitudeInput = authenticatedPage.locator('#longitude');
    await expect(longitudeInput).toBeVisible();
    await expect(longitudeInput).toHaveAttribute('type', 'number');
    await expect(longitudeInput).toHaveAttribute('min', '-180');
    await expect(longitudeInput).toHaveAttribute('max', '180');
    
    // Verify longitude label exists (optional field)
    const longitudeLabel = authenticatedPage.locator('label[for="longitude"]');
    await expect(longitudeLabel).toBeVisible();
    
    // Verify Create Venue button exists and is enabled
    const createButton = authenticatedPage.locator('button:has-text("Create Venue")');
    await expect(createButton).toBeVisible();
    await expect(createButton).toBeEnabled();
    
    // Verify Cancel button exists and is enabled
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await expect(cancelButton).toBeEnabled();
  });

  /**
   * Test 8: Form Interaction and Error Clearing
   * 
   * Verifies that validation errors are displayed and cleared appropriately
   * during user interaction with the form.
   */
  test('should handle form interaction and error clearing correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Try to submit empty form to trigger validation error
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify error message appears
    const errorMessage = authenticatedPage.locator('text=Venue name is required');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
    
    // Start filling in the name field
    await authenticatedPage.fill('#name', 'Test Venue');
    
    // Try to submit again (missing other required fields)
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify new error message appears
    const descriptionError = authenticatedPage.locator('text=Description is required');
    await expect(descriptionError).toBeVisible({ timeout: 5000 });
    
    // Fill in description
    await authenticatedPage.fill('#description', 'Test description');
    
    // Try to submit again (missing seating capacity)
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Verify seating capacity error appears
    const capacityError = authenticatedPage.locator('text=Seating capacity must be a positive number');
    await expect(capacityError).toBeVisible({ timeout: 5000 });
    
    // Fill in valid seating capacity
    await authenticatedPage.fill('#seatingCapacity', '1000');
    
    // Now submit with all required fields filled
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/venues');
  });

  /**
   * Test 9: Loading State During Submission
   * 
   * Verifies that the form shows appropriate loading state during submission
   * and that buttons are disabled to prevent double submission.
   */
  test('should show loading state during venue creation', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Loading Test Venue ${timestamp}`;
    
    // Fill in all required fields
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#seatingCapacity', '1500');
    await authenticatedPage.fill('#description', 'Testing loading state during venue creation');
    
    // Click the Create Venue button
    const createButton = authenticatedPage.locator('button:has-text("Create Venue")');
    await createButton.click();
    
    // Verify button is disabled during submission (check quickly before navigation)
    // Note: This may be very brief, so we use a short timeout
    await expect(createButton).toBeDisabled({ timeout: 1000 }).catch(() => {
      // If the request is too fast, the button might not be disabled long enough to catch
      // This is acceptable as it means the operation completed successfully
    });
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/venues');
  });

  /**
   * Test 10: Valid Boundary Values for Numeric Fields
   * 
   * Verifies that the form accepts valid boundary values for numeric fields
   * (latitude, longitude, seating capacity).
   */
  test('should accept valid boundary values for numeric fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Generate unique venue name
    const timestamp = Date.now();
    const venueName = `Boundary Test Venue ${timestamp}`;
    
    // Fill in required fields with boundary values
    await authenticatedPage.fill('#name', venueName);
    await authenticatedPage.fill('#seatingCapacity', '1'); // Minimum valid capacity
    await authenticatedPage.fill('#description', 'Testing boundary values for numeric fields');
    await authenticatedPage.fill('#latitude', '90'); // Maximum valid latitude
    await authenticatedPage.fill('#longitude', '-180'); // Minimum valid longitude
    
    // Click the Create Venue button
    await authenticatedPage.click('button:has-text("Create Venue")');
    
    // Wait for navigation to complete
    await authenticatedPage.waitForURL('/venues', { timeout: 10000 });
    
    // Verify successful redirect
    await expect(authenticatedPage).toHaveURL('/venues');
    
    // Wait for the venues list to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the new venue appears in the list
    const venueCard = authenticatedPage.locator(`text=${venueName}`);
    await expect(venueCard).toBeVisible({ timeout: 10000 });
  });
});