import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Edit Ticket Offer Test Suite
 * 
 * Comprehensive E2E tests for the "Edit Ticket Offer" feature including:
 * - Successful ticket offer edit with modified fields
 * - Editing individual fields (name, price, ticket count)
 * - Form pre-population verification
 * - Capacity validation during edit
 * - Cancel edit functionality
 * - Edit page elements verification
 * - Invalid data validation
 */

/**
 * Helper function to navigate to a show detail page and create a ticket offer
 * Returns the show GUID and ticket offer GUID
 */
async function createTestTicketOffer(page: Page): Promise<{ showGuid: string; ticketOfferGuid: string }> {
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
  
  if (showCount === 0) {
    throw new Error('No shows found');
  }
  
  await showCards.first().click();
  await page.waitForLoadState('networkidle');
  
  // Extract show GUID from URL
  const showUrl = page.url();
  const showGuidMatch = showUrl.match(/\/shows\/([0-9a-f-]{36})/i);
  if (!showGuidMatch) {
    throw new Error('Failed to extract show GUID from URL');
  }
  const showGuid = showGuidMatch[1];
  
  // Create a ticket offer
  const timestamp = Date.now();
  await page.locator('#offerName').fill(`Test Offer ${timestamp}`);
  await page.locator('#price').fill('50.00');
  await page.locator('#ticketCount').fill('100');
  
  // Submit the form
  await page.click('button:has-text("Create Offer")');
  
  // Wait for the offer to appear in the list
  await page.waitForSelector(`text=Test Offer ${timestamp}`, { timeout: 10000 });
  
  // Find the edit button for the newly created offer
  const offerCard = page.locator(`text=Test Offer ${timestamp}`).locator('..').locator('..');
  const editButton = offerCard.locator('button[aria-label*="Edit"]');
  
  // Store the current state, then click edit to navigate to edit page
  await editButton.click();
  await page.waitForLoadState('networkidle');
  
  // Extract ticket offer GUID from URL
  const editUrl = page.url();
  const ticketOfferGuidMatch = editUrl.match(/\/ticket-offers\/([0-9a-f-]{36})\/edit/i);
  if (!ticketOfferGuidMatch) {
    throw new Error('Failed to extract ticket offer GUID from URL');
  }
  const ticketOfferGuid = ticketOfferGuidMatch[1];
  
  return { showGuid, ticketOfferGuid };
}

test.describe('Edit Ticket Offer - Happy Path', () => {
  /**
   * Test 1: Edit Ticket Offer with All Fields Modified
   * 
   * Verifies that a user can successfully edit all fields of a ticket offer
   * and that the changes are saved and reflected in the show detail page.
   */
  test('should successfully edit a ticket offer with all fields modified', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Verify we're on the edit page
    await expect(authenticatedPage.locator('text=Edit Ticket Offer')).toBeVisible();
    await expect(authenticatedPage.locator('text=Update the ticket offer details below')).toBeVisible();
    
    // Verify form is pre-populated
    const timestamp = Date.now();
    await expect(authenticatedPage.locator('#offerName')).toHaveValue(/Test Offer/);
    await expect(authenticatedPage.locator('#price')).toHaveValue('50');
    await expect(authenticatedPage.locator('#ticketCount')).toHaveValue('100');
    
    // Modify all fields
    await authenticatedPage.locator('#offerName').fill(`Updated Offer ${timestamp}`);
    await authenticatedPage.locator('#price').fill('75.50');
    await authenticatedPage.locator('#ticketCount').fill('150');
    
    // Submit the form
    await authenticatedPage.click('button:has-text("Update Offer")');
    
    // Wait for navigation back to show detail page
    await authenticatedPage.waitForURL(`/shows/${showGuid}`, { timeout: 10000 });
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify the updated offer appears with new values
    await expect(authenticatedPage.locator(`text=Updated Offer ${timestamp}`)).toBeVisible();
    await expect(authenticatedPage.locator('text=$75.50 per ticket')).toBeVisible();
    await expect(authenticatedPage.locator('text=150 tickets')).toBeVisible();
  });

  /**
   * Test 2: Edit Only Ticket Offer Name
   * 
   * Verifies that editing only the name field works correctly
   * and other fields remain unchanged.
   */
  test('should successfully edit only the offer name', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Modify only the name
    const timestamp = Date.now();
    await authenticatedPage.locator('#offerName').fill(`Name Only ${timestamp}`);
    
    // Keep other fields unchanged (verify they still have original values)
    await expect(authenticatedPage.locator('#price')).toHaveValue('50');
    await expect(authenticatedPage.locator('#ticketCount')).toHaveValue('100');
    
    // Submit the form
    await authenticatedPage.click('button:has-text("Update Offer")');
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/shows/${showGuid}`, { timeout: 10000 });
    
    // Verify only the name changed
    await expect(authenticatedPage.locator(`text=Name Only ${timestamp}`)).toBeVisible();
    await expect(authenticatedPage.locator('text=$50.00 per ticket')).toBeVisible();
    await expect(authenticatedPage.locator('text=100 tickets')).toBeVisible();
  });

  /**
   * Test 3: Edit Only Price
   * 
   * Verifies that editing only the price field works correctly.
   */
  test('should successfully edit only the price', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Modify only the price
    await authenticatedPage.locator('#price').fill('99.99');
    
    // Submit the form
    await authenticatedPage.click('button:has-text("Update Offer")');
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/shows/${showGuid}`, { timeout: 10000 });
    
    // Verify the price changed
    await expect(authenticatedPage.locator('text=$99.99 per ticket')).toBeVisible();
  });
});

test.describe('Edit Ticket Offer - Validation', () => {
  /**
   * Test 4: Edit with Ticket Count Exceeding Capacity
   * 
   * Verifies that the system prevents editing a ticket offer
   * with a ticket count that would exceed available capacity.
   */
  test('should prevent editing with ticket count exceeding capacity', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Try to set ticket count to a very large number (likely exceeds capacity)
    await authenticatedPage.locator('#ticketCount').fill('999999');
    
    // Submit the form
    await authenticatedPage.click('button:has-text("Update Offer")');
    
    // Wait for error message
    await authenticatedPage.waitForSelector('text=/capacity/i', { timeout: 5000 });
    
    // Verify error message appears
    const errorMessage = authenticatedPage.locator('[role="alert"]').filter({ hasText: /capacity/i });
    await expect(errorMessage).toBeVisible();
    
    // Verify we're still on the edit page (no navigation occurred)
    await expect(authenticatedPage.locator('text=Edit Ticket Offer')).toBeVisible();
  });

  /**
   * Test 5: Edit with Empty Name
   * 
   * Verifies that the system requires a name for the ticket offer.
   */
  test('should prevent editing with empty name', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    await createTestTicketOffer(authenticatedPage);
    
    // Clear the name field
    await authenticatedPage.locator('#offerName').fill('');
    
    // Submit the form
    await authenticatedPage.click('button:has-text("Update Offer")');
    
    // Wait for error message
    await authenticatedPage.waitForSelector('text=/required/i', { timeout: 5000 });
    
    // Verify error message appears
    const errorMessage = authenticatedPage.locator('[role="alert"]').filter({ hasText: /required/i });
    await expect(errorMessage).toBeVisible();
  });
});

test.describe('Edit Ticket Offer - Navigation', () => {
  /**
   * Test 6: Cancel Edit Navigation
   * 
   * Verifies that clicking Cancel navigates back to the show detail page
   * without saving changes.
   */
  test('should navigate back to show page when Cancel is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Modify a field but don't save
    await authenticatedPage.locator('#offerName').fill('Should Not Be Saved');
    
    // Click Cancel
    await authenticatedPage.click('button:has-text("Cancel")');
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/shows/${showGuid}`, { timeout: 10000 });
    
    // Verify the original offer name is still present (changes were not saved)
    await expect(authenticatedPage.locator('text=/Test Offer/i')).toBeVisible();
    await expect(authenticatedPage.locator('text=Should Not Be Saved')).not.toBeVisible();
  });

  /**
   * Test 7: Back Button Navigation
   * 
   * Verifies that clicking the Back button navigates to the show detail page.
   */
  test('should navigate back to show page when Back button is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    const { showGuid } = await createTestTicketOffer(authenticatedPage);
    
    // Click Back button
    await authenticatedPage.click('button:has-text("Back to Show")');
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/shows/${showGuid}`, { timeout: 10000 });
    
    // Verify we're on the show detail page
    await expect(authenticatedPage.locator('text=/Ticket Offers/i')).toBeVisible();
  });
});

test.describe('Edit Ticket Offer - UI Elements', () => {
  /**
   * Test 8: Form Pre-population
   * 
   * Verifies that the edit form is correctly pre-populated with
   * the existing ticket offer data.
   */
  test('should pre-populate form with existing ticket offer data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    await createTestTicketOffer(authenticatedPage);
    
    // Verify all fields are pre-populated
    await expect(authenticatedPage.locator('#offerName')).not.toHaveValue('');
    await expect(authenticatedPage.locator('#price')).not.toHaveValue('');
    await expect(authenticatedPage.locator('#ticketCount')).not.toHaveValue('');
    
    // Verify button text says "Update Offer" not "Create Offer"
    await expect(authenticatedPage.locator('button:has-text("Update Offer")')).toBeVisible();
    await expect(authenticatedPage.locator('button:has-text("Create Offer")')).not.toBeVisible();
  });

  /**
   * Test 9: Edit Page Elements
   * 
   * Verifies that all expected UI elements are present on the edit page.
   */
  test('should display all expected edit page elements', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    await createTestTicketOffer(authenticatedPage);
    
    // Verify page heading and description
    await expect(authenticatedPage.locator('text=Edit Ticket Offer')).toBeVisible();
    await expect(authenticatedPage.locator('text=Update the ticket offer details below')).toBeVisible();
    
    // Verify form fields
    await expect(authenticatedPage.locator('#offerName')).toBeVisible();
    await expect(authenticatedPage.locator('#price')).toBeVisible();
    await expect(authenticatedPage.locator('#ticketCount')).toBeVisible();
    
    // Verify buttons
    await expect(authenticatedPage.locator('button:has-text("Update Offer")')).toBeVisible();
    await expect(authenticatedPage.locator('button:has-text("Cancel")')).toBeVisible();
    await expect(authenticatedPage.locator('button:has-text("Back to Show")')).toBeVisible();
    
    // Verify labels
    await expect(authenticatedPage.locator('text=Offer Name *')).toBeVisible();
    await expect(authenticatedPage.locator('text=Price per Ticket *')).toBeVisible();
    await expect(authenticatedPage.locator('text=Number of Tickets *')).toBeVisible();
  });
});
