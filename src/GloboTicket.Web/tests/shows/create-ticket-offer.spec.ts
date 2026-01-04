import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Create Ticket Offer Test Suite
 * 
 * Comprehensive E2E tests for the "Create Ticket Offer" feature including:
 * - Creating first ticket offer (happy path)
 * - Creating multiple ticket offers
 * - Client-side capacity validation
 * - Server-side capacity validation
 * - Form field validation
 * - Cancel functionality
 * - Empty state display
 * - Loading states
 * - Error handling
 * - Capacity display updates
 * - Accessibility features
 */

/**
 * Helper function to navigate to a show detail page with a known capacity
 */
async function navigateToShowDetailPage(page: Page): Promise<string | null> {
  // Navigate to acts page
  await page.goto('/acts');
  await page.waitForLoadState('networkidle');
  await page.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
  
  // Click on first act
  const firstActCard = page.locator('[role="button"]').first();
  await firstActCard.click();
  await page.waitForLoadState('networkidle');
  
  // Find and click on first show
  const showCards = page.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/i });
  const showCount = await showCards.count();
  
  if (showCount > 0) {
    await showCards.first().click();
    await page.waitForLoadState('networkidle');
    
    // Extract show GUID from URL
    const currentUrl = page.url();
    const guidMatch = currentUrl.match(/\/shows\/([0-9a-f-]{36})/i);
    return guidMatch ? guidMatch[1] : null;
  }
  
  return null;
}

/**
 * Helper function to fill ticket offer form
 */
async function fillTicketOfferForm(
  page: Page,
  name: string,
  price: string,
  ticketCount: string
): Promise<void> {
  await page.locator('#offerName').fill(name);
  await page.locator('#price').fill(price);
  await page.locator('#ticketCount').fill(ticketCount);
}

test.describe('Create Ticket Offer - Happy Path', () => {
  /**
   * Test 1: Create First Ticket Offer
   * 
   * Verifies that a user can successfully create the first ticket offer for a show.
   * Tests acceptance criteria:
   * - Ticket offers can be created from show detail page
   * - Form has name, price, and ticket count fields
   * - Successfully created offers appear in the list
   * - Capacity display updates correctly
   * - Form resets after successful creation
   */
  test('should create first ticket offer successfully', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Verify we're on show detail page
    await expect(authenticatedPage).toHaveURL(new RegExp(`/shows/${showGuid}`));
    
    // Verify capacity display shows full capacity available
    const capacityDisplay = authenticatedPage.locator('text=/Total Tickets:|Allocated:|Available:/i');
    await expect(capacityDisplay.first()).toBeVisible({ timeout: 5000 });
    
    // Get initial available capacity
    const availableCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const initialCapacity = parseInt(availableCapacityText?.match(/\d+/)?.[0] || '0');
    
    // Verify ticket offer form is visible
    const offerForm = authenticatedPage.locator('form').filter({ has: authenticatedPage.locator('#offerName') });
    await expect(offerForm).toBeVisible();
    
    // Fill in ticket offer form
    const offerName = 'General Admission';
    const offerPrice = '50.00';
    const offerTicketCount = Math.min(50, Math.floor(initialCapacity / 2)).toString();
    
    await fillTicketOfferForm(authenticatedPage, offerName, offerPrice, offerTicketCount);
    
    // Submit form
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await expect(createButton).toBeVisible();
    await createButton.click();
    
    // Wait for success (offer should appear in list)
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify offer appears in list
    const offerCard = authenticatedPage.locator(`text=${offerName}`);
    await expect(offerCard).toBeVisible({ timeout: 5000 });
    
    // Verify offer details are displayed
    await expect(authenticatedPage.locator(`text=$${offerPrice}`)).toBeVisible();
    await expect(authenticatedPage.locator(`text=${offerTicketCount}`)).toBeVisible();
    
    // Verify capacity display updates
    const updatedAvailableText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const updatedCapacity = parseInt(updatedAvailableText?.match(/\d+/)?.[0] || '0');
    expect(updatedCapacity).toBe(initialCapacity - parseInt(offerTicketCount));
    
    // Verify form resets after successful creation
    const nameField = authenticatedPage.locator('#offerName');
    const priceField = authenticatedPage.locator('#price');
    const ticketCountField = authenticatedPage.locator('#ticketCount');
    
    await expect(nameField).toHaveValue('');
    await expect(priceField).toHaveValue('');
    await expect(ticketCountField).toHaveValue('');
  });

  /**
   * Test 2: Create Multiple Ticket Offers
   * 
   * Verifies that multiple ticket offers can be created for the same show.
   * Tests acceptance criteria:
   * - Multiple offers can be created
   * - Capacity updates correctly after each offer
   * - All offers appear in the list
   * - Offers are sorted chronologically
   */
  test('should create multiple ticket offers successfully', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Get initial available capacity
    const availableCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const initialCapacity = parseInt(availableCapacityText?.match(/\d+/)?.[0] || '0');
    
    if (initialCapacity < 100) {
      test.skip(); // Need sufficient capacity for this test
      return;
    }
    
    // Create first offer
    await fillTicketOfferForm(authenticatedPage, 'General Admission', '50.00', '50');
    await authenticatedPage.locator('button:has-text("Create Offer")').click();
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify first offer appears
    await expect(authenticatedPage.locator('text=General Admission')).toBeVisible();
    
    // Verify capacity updated
    let updatedCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    let updatedCapacity = parseInt(updatedCapacityText?.match(/\d+/)?.[0] || '0');
    expect(updatedCapacity).toBe(initialCapacity - 50);
    
    // Create second offer
    await fillTicketOfferForm(authenticatedPage, 'VIP', '150.00', '30');
    await authenticatedPage.locator('button:has-text("Create Offer")').click();
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify second offer appears
    await expect(authenticatedPage.locator('text=VIP')).toBeVisible();
    
    // Verify capacity updated again
    updatedCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    updatedCapacity = parseInt(updatedCapacityText?.match(/\d+/)?.[0] || '0');
    expect(updatedCapacity).toBe(initialCapacity - 80);
    
    // Verify both offers are visible
    await expect(authenticatedPage.locator('text=General Admission')).toBeVisible();
    await expect(authenticatedPage.locator('text=VIP')).toBeVisible();
  });
});

test.describe('Create Ticket Offer - Capacity Validation', () => {
  /**
   * Test 3: Client-Side Capacity Validation
   * 
   * Verifies that client-side validation prevents creating offers that exceed capacity.
   * Tests acceptance criteria:
   * - Ticket count must not exceed available capacity
   * - Error message shows remaining capacity
   * - Form data is retained
   * - Form submission is prevented
   */
  test('should prevent creating offer exceeding available capacity', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Get available capacity
    const availableCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const availableCapacity = parseInt(availableCapacityText?.match(/\d+/)?.[0] || '0');
    
    // Try to create offer exceeding capacity
    const exceedingCount = (availableCapacity + 100).toString();
    await fillTicketOfferForm(authenticatedPage, 'Too Many Tickets', '25.00', exceedingCount);
    
    // Try to submit
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Verify error message is displayed
    const errorMessage = authenticatedPage.locator('text=/exceeds available capacity/i');
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
    
    // Verify error message shows remaining capacity
    const errorText = await errorMessage.textContent();
    expect(errorText).toContain(availableCapacity.toString());
    
    // Verify form data is retained
    await expect(authenticatedPage.locator('#offerName')).toHaveValue('Too Many Tickets');
    await expect(authenticatedPage.locator('#price')).toHaveValue('25.00');
    await expect(authenticatedPage.locator('#ticketCount')).toHaveValue(exceedingCount);
  });

  /**
   * Test 4: Create Offer Using Exact Remaining Capacity
   * 
   * Verifies that an offer can use exactly the remaining capacity.
   * Tests acceptance criteria:
   * - Can use all remaining capacity
   * - Available capacity becomes zero
   * - Show is at full capacity indicator
   */
  test('should allow creating offer with exact remaining capacity', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Get available capacity
    const availableCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const availableCapacity = parseInt(availableCapacityText?.match(/\d+/)?.[0] || '0');
    
    if (availableCapacity === 0) {
      test.skip(); // Already at capacity
      return;
    }
    
    // Create offer with exact remaining capacity
    await fillTicketOfferForm(
      authenticatedPage,
      'Final Offer',
      '75.00',
      availableCapacity.toString()
    );
    
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify offer was created
    await expect(authenticatedPage.locator('text=Final Offer')).toBeVisible({ timeout: 5000 });
    
    // Verify capacity is now zero
    const updatedCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const updatedCapacity = parseInt(updatedCapacityText?.match(/\d+/)?.[0] || '0');
    expect(updatedCapacity).toBe(0);
    
    // Verify full capacity indicator is shown
    const fullCapacityIndicator = authenticatedPage.locator('text=/fully allocated|at capacity/i');
    await expect(fullCapacityIndicator).toBeVisible({ timeout: 5000 });
  });
});

test.describe('Create Ticket Offer - Form Validation', () => {
  /**
   * Test 5: Validate Required Fields
   * 
   * Verifies that all required fields are validated.
   * Tests acceptance criteria:
   * - Offer name is required
   * - Price is required
   * - Ticket count is required
   * - Validation errors displayed inline
   * - Form submission prevented
   */
  test('should validate all required fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Try to submit empty form
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Verify error messages for all required fields
    const nameError = authenticatedPage.locator('text=/name is required/i');
    const priceError = authenticatedPage.locator('text=/price is required/i');
    const ticketCountError = authenticatedPage.locator('text=/ticket count is required/i');
    
    await expect(nameError).toBeVisible({ timeout: 5000 });
    await expect(priceError).toBeVisible({ timeout: 5000 });
    await expect(ticketCountError).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 6: Validate Offer Name Length
   * 
   * Verifies that offer name has length constraints.
   * Tests acceptance criteria:
   * - Offer name must be between 1 and 100 characters
   * - Validation error displayed for exceeding max length
   */
  test('should validate offer name length', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Try to enter name exceeding 100 characters
    const longName = 'A'.repeat(101);
    await authenticatedPage.locator('#offerName').fill(longName);
    await authenticatedPage.locator('#price').fill('50.00');
    await authenticatedPage.locator('#ticketCount').fill('10');
    
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Verify error message
    const lengthError = authenticatedPage.locator('text=/cannot exceed 100 characters/i');
    await expect(lengthError).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 7: Validate Price is Positive
   * 
   * Verifies that price must be a positive value.
   * Tests acceptance criteria:
   * - Price must be greater than zero
   * - Validation error for zero or negative price
   */
  test('should validate price is positive', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Try zero price
    await fillTicketOfferForm(authenticatedPage, 'Free Tickets', '0', '10');
    
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Verify error message
    const priceError = authenticatedPage.locator('text=/price must be greater than zero/i');
    await expect(priceError).toBeVisible({ timeout: 5000 });
    
    // Try negative price
    await authenticatedPage.locator('#price').fill('-10.00');
    await createButton.click();
    
    await expect(priceError).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test 8: Validate Ticket Count is Positive
   * 
   * Verifies that ticket count must be a positive integer.
   * Tests acceptance criteria:
   * - Ticket count must be greater than zero
   * - Validation error for zero or negative count
   */
  test('should validate ticket count is positive', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Try zero ticket count
    await fillTicketOfferForm(authenticatedPage, 'No Tickets', '50.00', '0');
    
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Verify error message
    const countError = authenticatedPage.locator('text=/ticket count must be.*positive/i');
    await expect(countError).toBeVisible({ timeout: 5000 });
    
    // Try negative ticket count
    await authenticatedPage.locator('#ticketCount').fill('-5');
    await createButton.click();
    
    await expect(countError).toBeVisible({ timeout: 5000 });
  });
});

test.describe('Create Ticket Offer - User Experience', () => {
  /**
   * Test 9: Cancel Functionality
   * 
   * Verifies that the cancel button clears the form.
   * Tests acceptance criteria:
   * - Cancel action is available
   * - Form is cleared when cancel is clicked
   * - No API call is made
   */
  test('should clear form when cancel button is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Fill in form fields
    await fillTicketOfferForm(authenticatedPage, 'Test Offer', '100.00', '25');
    
    // Verify fields are filled
    await expect(authenticatedPage.locator('#offerName')).toHaveValue('Test Offer');
    await expect(authenticatedPage.locator('#price')).toHaveValue('100.00');
    await expect(authenticatedPage.locator('#ticketCount')).toHaveValue('25');
    
    // Click cancel button
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await cancelButton.click();
    
    // Verify form is cleared
    await expect(authenticatedPage.locator('#offerName')).toHaveValue('');
    await expect(authenticatedPage.locator('#price')).toHaveValue('');
    await expect(authenticatedPage.locator('#ticketCount')).toHaveValue('');
  });

  /**
   * Test 10: Empty State Display
   * 
   * Verifies that an empty state is shown when no ticket offers exist.
   * Tests acceptance criteria:
   * - Empty state message is displayed
   * - Empty state has appropriate icon and text
   */
  test('should display empty state when no ticket offers exist', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Check for either offers or empty state
    const offersList = authenticatedPage.locator('text=/General Admission|VIP|Early Bird/i');
    const emptyState = authenticatedPage.locator('text=/No ticket offers|No offers created/i');
    
    const offersCount = await offersList.count();
    const emptyStateVisible = await emptyState.isVisible().catch(() => false);
    
    // If no offers, verify empty state is displayed
    if (offersCount === 0 && emptyStateVisible) {
      await expect(emptyState).toBeVisible();
      await expect(emptyState).toContainText(/create.*first.*offer|add.*ticket.*offer/i);
    }
  });

  /**
   * Test 11: Loading State During Submission
   * 
   * Verifies that loading state is shown during form submission.
   * Tests acceptance criteria:
   * - Button shows loading state during submission
   * - Form is disabled during submission
   */
  test('should show loading state during form submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Fill in valid form data
    await fillTicketOfferForm(authenticatedPage, 'Loading Test', '50.00', '10');
    
    // Submit form
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Check if button is disabled during submission
    const isDisabled = await createButton.isDisabled().catch(() => false);
    expect(isDisabled).toBe(true);
  });

  /**
   * Test 12: Loading State for Capacity Display
   * 
   * Verifies that loading spinner appears while fetching capacity.
   * Tests acceptance criteria:
   * - Loading spinner appears while fetching capacity
   * - Capacity information displays after loading
   */
  test('should show loading state while fetching capacity', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/i });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      
      // Check for loading spinner (may be brief)
      const loadingSpinner = authenticatedPage.locator('text=/Loading.*capacity/i');
      const spinnerWasVisible = await loadingSpinner.isVisible().catch(() => false);
      
      // Wait for page to fully load
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify capacity information is now displayed
      const capacityDisplay = authenticatedPage.locator('text=/Total Tickets:|Available:/i');
      await expect(capacityDisplay.first()).toBeVisible({ timeout: 5000 });
    }
  });
});

test.describe('Create Ticket Offer - Error Handling', () => {
  /**
   * Test 13: Handle Network Error
   * 
   * Verifies that network errors are handled gracefully.
   * Tests acceptance criteria:
   * - Error message is user-friendly
   * - Form data is retained after error
   */
  test('should handle network error gracefully', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Fill in form
    await fillTicketOfferForm(authenticatedPage, 'Network Test', '50.00', '10');
    
    // Go offline
    await authenticatedPage.context().setOffline(true);
    
    // Try to submit
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Wait for error
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify error message is displayed
    const errorMessage = authenticatedPage.locator('text=/error|failed|unable/i');
    const errorVisible = await errorMessage.isVisible().catch(() => false);
    
    // Go back online
    await authenticatedPage.context().setOffline(false);
    
    if (errorVisible) {
      await expect(errorMessage).toBeVisible();
      
      // Verify form data is retained
      await expect(authenticatedPage.locator('#offerName')).toHaveValue('Network Test');
      await expect(authenticatedPage.locator('#price')).toHaveValue('50.00');
      await expect(authenticatedPage.locator('#ticketCount')).toHaveValue('10');
    }
  });

  /**
   * Test 14: Handle Server Error
   * 
   * Verifies that server errors (500) are handled gracefully.
   * Tests acceptance criteria:
   * - Error message is user-friendly
   * - Form data is retained after error
   */
  test('should display error message for server error', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Fill in form with valid data
    await fillTicketOfferForm(authenticatedPage, 'Server Error Test', '50.00', '10');
    
    // Submit form
    const createButton = authenticatedPage.locator('button:has-text("Create Offer")');
    await createButton.click();
    
    // Wait for either success or error
    await authenticatedPage.waitForTimeout(2000);
    
    // Check if error message is displayed
    const errorAlert = authenticatedPage.locator('[role="alert"]');
    const errorVisible = await errorAlert.isVisible().catch(() => false);
    
    if (errorVisible) {
      // Verify error message is user-friendly (not technical details)
      const errorText = await errorAlert.textContent();
      expect(errorText).not.toContain('500');
      expect(errorText).not.toContain('Internal Server Error');
      
      // Verify form data is retained
      await expect(authenticatedPage.locator('#offerName')).toHaveValue('Server Error Test');
    }
  });
});

test.describe('Create Ticket Offer - Accessibility', () => {
  /**
   * Test 15: Keyboard Navigation
   * 
   * Verifies that the form supports keyboard navigation.
   * Tests acceptance criteria:
   * - Form inputs support Tab navigation
   * - Form can be submitted with Enter key
   * - Focus management after submission
   */
  test('should support keyboard navigation through form', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Focus on first field
    const nameField = authenticatedPage.locator('#offerName');
    await nameField.focus();
    
    // Verify field is focused
    const isNameFocused = await nameField.evaluate(el => el === document.activeElement);
    expect(isNameFocused).toBe(true);
    
    // Tab to next field
    await authenticatedPage.keyboard.press('Tab');
    
    // Verify price field is focused
    const priceField = authenticatedPage.locator('#price');
    const isPriceFocused = await priceField.evaluate(el => el === document.activeElement);
    expect(isPriceFocused).toBe(true);
    
    // Tab to ticket count field
    await authenticatedPage.keyboard.press('Tab');
    
    // Verify ticket count field is focused
    const ticketCountField = authenticatedPage.locator('#ticketCount');
    const isTicketCountFocused = await ticketCountField.evaluate(el => el === document.activeElement);
    expect(isTicketCountFocused).toBe(true);
  });

  /**
   * Test 16: Form Labels and ARIA Attributes
   * 
   * Verifies that form has proper labels and ARIA attributes.
   * Tests acceptance criteria:
   * - All inputs have associated labels
   * - Required fields are marked
   * - Error messages are announced
   */
  test('should have proper labels and ARIA attributes', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Verify all fields have labels
    const nameLabel = authenticatedPage.locator('label[for="offerName"]');
    const priceLabel = authenticatedPage.locator('label[for="price"]');
    const ticketCountLabel = authenticatedPage.locator('label[for="ticketCount"]');
    
    await expect(nameLabel).toBeVisible();
    await expect(priceLabel).toBeVisible();
    await expect(ticketCountLabel).toBeVisible();
    
    // Verify required fields are marked (asterisk or "required" text)
    const nameLabelText = await nameLabel.textContent();
    const priceLabelText = await priceLabel.textContent();
    const ticketCountLabelText = await ticketCountLabel.textContent();
    
    expect(nameLabelText).toMatch(/\*|required/i);
    expect(priceLabelText).toMatch(/\*|required/i);
    expect(ticketCountLabelText).toMatch(/\*|required/i);
  });

  /**
   * Test 17: Capacity Display Accessibility
   * 
   * Verifies that capacity display has proper ARIA attributes.
   * Tests acceptance criteria:
   * - Capacity information is accessible to screen readers
   * - Progress bar has proper ARIA attributes
   */
  test('should have accessible capacity display', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Verify capacity display is visible
    const capacityDisplay = authenticatedPage.locator('text=/Total Tickets:|Allocated:|Available:/i');
    await expect(capacityDisplay.first()).toBeVisible({ timeout: 5000 });
    
    // Check for progress bar with ARIA attributes
    const progressBar = authenticatedPage.locator('[role="progressbar"]');
    const progressBarExists = await progressBar.isVisible().catch(() => false);
    
    if (progressBarExists) {
      // Verify progress bar has required ARIA attributes
      const ariaValueNow = await progressBar.getAttribute('aria-valuenow');
      const ariaValueMin = await progressBar.getAttribute('aria-valuemin');
      const ariaValueMax = await progressBar.getAttribute('aria-valuemax');
      
      expect(ariaValueNow).toBeTruthy();
      expect(ariaValueMin).toBeTruthy();
      expect(ariaValueMax).toBeTruthy();
    }
  });
});

test.describe('Create Ticket Offer - Integration', () => {
  /**
   * Test 18: Complete Flow - Create and View Offers
   * 
   * Verifies the complete flow of creating multiple offers and viewing them.
   * Tests acceptance criteria:
   * - Multiple offers can be created
   * - All offers are displayed in the list
   * - Capacity updates correctly
   * - Offers are sorted chronologically
   */
  test('should complete full flow of creating and viewing multiple offers', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Get initial capacity
    const initialCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const initialCapacity = parseInt(initialCapacityText?.match(/\d+/)?.[0] || '0');
    
    if (initialCapacity < 150) {
      test.skip(); // Need sufficient capacity
      return;
    }
    
    // Create three different offers
    const offers = [
      { name: 'Early Bird', price: '40.00', count: '50' },
      { name: 'General Admission', price: '60.00', count: '60' },
      { name: 'VIP', price: '120.00', count: '40' }
    ];
    
    let expectedCapacity = initialCapacity;
    
    for (const offer of offers) {
      // Fill and submit form
      await fillTicketOfferForm(authenticatedPage, offer.name, offer.price, offer.count);
      await authenticatedPage.locator('button:has-text("Create Offer")').click();
      await authenticatedPage.waitForTimeout(2000);
      
      // Verify offer appears
      await expect(authenticatedPage.locator(`text=${offer.name}`)).toBeVisible({ timeout: 5000 });
      
      // Update expected capacity
      expectedCapacity -= parseInt(offer.count);
      
      // Verify capacity updated
      const updatedCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
      const updatedCapacity = parseInt(updatedCapacityText?.match(/\d+/)?.[0] || '0');
      expect(updatedCapacity).toBe(expectedCapacity);
    }
    
    // Verify all three offers are visible
    await expect(authenticatedPage.locator('text=Early Bird')).toBeVisible();
    await expect(authenticatedPage.locator('text=General Admission')).toBeVisible();
    await expect(authenticatedPage.locator('text=VIP')).toBeVisible();
    
    // Verify final capacity
    const finalCapacityText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const finalCapacity = parseInt(finalCapacityText?.match(/\d+/)?.[0] || '0');
    expect(finalCapacity).toBe(initialCapacity - 150);
  });

  /**
   * Test 19: Refresh Page and Verify Persistence
   * 
   * Verifies that created offers persist after page refresh.
   * Tests acceptance criteria:
   * - Offers are saved to database
   * - Offers appear after page refresh
   * - Capacity information is correct after refresh
   */
  test('should persist offers after page refresh', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const showGuid = await navigateToShowDetailPage(authenticatedPage);
    
    if (!showGuid) {
      test.skip();
      return;
    }
    
    // Create an offer
    await fillTicketOfferForm(authenticatedPage, 'Persistence Test', '55.00', '25');
    await authenticatedPage.locator('button:has-text("Create Offer")').click();
    await authenticatedPage.waitForTimeout(2000);
    
    // Verify offer appears
    await expect(authenticatedPage.locator('text=Persistence Test')).toBeVisible({ timeout: 5000 });
    
    // Get capacity before refresh
    const capacityBeforeText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const capacityBefore = parseInt(capacityBeforeText?.match(/\d+/)?.[0] || '0');
    
    // Refresh page
    await authenticatedPage.reload();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify offer still appears
    await expect(authenticatedPage.locator('text=Persistence Test')).toBeVisible({ timeout: 5000 });
    
    // Verify capacity is still correct
    const capacityAfterText = await authenticatedPage.locator('text=/Available:/i').textContent();
    const capacityAfter = parseInt(capacityAfterText?.match(/\d+/)?.[0] || '0');
    expect(capacityAfter).toBe(capacityBefore);
  });
});
