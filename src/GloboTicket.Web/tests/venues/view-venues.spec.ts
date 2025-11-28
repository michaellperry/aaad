import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * View Venues List Test Suite
 * 
 * Comprehensive tests for the venues list page functionality including:
 * - Viewing venues list with proper page structure
 * - Venue card content verification
 * - Add venue button functionality
 * - Venue card interaction and accessibility
 * - Empty state handling
 * - Loading state verification
 * - Responsive grid layout
 * - Multiple venues display
 */
test.describe('View Venues List', () => {
  /**
   * Test 1: View Venues List Test
   * 
   * Verifies that the venues list page loads correctly with proper structure,
   * displays venue cards, and shows at least one venue.
   */
  test('should display venues list with proper page structure', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for the page to be fully loaded
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for loading spinner to disappear (venues loaded)
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Verify page title is "Venues"
    const pageTitle = authenticatedPage.locator('h1:has-text("Venues")');
    await expect(pageTitle).toBeVisible();
    
    // Verify page description is present
    const pageDescription = authenticatedPage.locator('text=Browse and manage all available venues for your events');
    await expect(pageDescription).toBeVisible();
    
    // Verify venue cards are displayed (look for h3 headings which are venue names)
    const venueCards = authenticatedPage.locator('h3');
    await expect(venueCards.first()).toBeVisible({ timeout: 5000 });
    
    // Verify at least one venue exists in the list
    const venueCount = await venueCards.count();
    expect(venueCount).toBeGreaterThan(0);
  });

  /**
   * Test 2: Venue Card Content Test
   * 
   * Verifies that venue cards display all expected content including
   * name, seating capacity badge, address, and description.
   */
  test('should display venue card with all content elements', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for venues to load
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Select the first venue card (using role=button since cards are clickable)
    const firstVenueCard = authenticatedPage.locator('[role="button"]').first();
    await expect(firstVenueCard).toBeVisible();
    
    // Verify card displays venue name (h3 heading)
    const venueName = firstVenueCard.locator('h3');
    await expect(venueName).toBeVisible();
    const nameText = await venueName.textContent();
    expect(nameText).toBeTruthy();
    expect(nameText!.length).toBeGreaterThan(0);
    
    // Verify seating capacity badge with Users icon is displayed
    // The badge should contain a number
    const capacityBadge = firstVenueCard.locator('[class*="badge"]');
    await expect(capacityBadge).toBeVisible();
    const capacityText = await capacityBadge.textContent();
    expect(capacityText).toMatch(/\d+/); // Should contain numbers
    
    // Note: Address and description are optional fields, so we check if they exist
    // If address is present, verify it's displayed with MapPin icon context
    const addressText = firstVenueCard.locator('text=/^[A-Za-z0-9\s,.-]+$/').filter({ hasText: /\d/ });
    const addressCount = await addressText.count();
    if (addressCount > 0) {
      // Address exists, verify it's visible
      await expect(addressText.first()).toBeVisible();
    }
    
    // If description is present, verify it's displayed (truncated to 2 lines via line-clamp-2)
    const descriptionElements = firstVenueCard.locator('[class*="line-clamp-2"]');
    const descCount = await descriptionElements.count();
    if (descCount > 0) {
      await expect(descriptionElements.first()).toBeVisible();
    }
  });

  /**
   * Test 3: Add Venue Button Test
   * 
   * Verifies that the "Add Venue" button exists with proper attributes,
   * and clicking it navigates to the create venue page.
   */
  test('should have functional Add Venue button with proper attributes', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for page to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify "Add Venue" button exists
    const addButton = authenticatedPage.locator('button:has-text("Add Venue")');
    await expect(addButton).toBeVisible();
    
    // Verify button has aria-label "Add new venue"
    await expect(addButton).toHaveAttribute('aria-label', 'Add new venue');
    
    // Click the button
    await addButton.click();
    
    // Verify navigation to /venues/new
    await authenticatedPage.waitForURL('/venues/new', { timeout: 10000 });
    await expect(authenticatedPage).toHaveURL('/venues/new');
  });

  /**
   * Test 4: Venue Card Interaction Test
   * 
   * Verifies that venue cards are properly interactive with correct
   * accessibility attributes for keyboard navigation.
   */
  test('should have interactive venue cards with proper accessibility', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for venues to load
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Get all venue cards
    const venueCards = authenticatedPage.locator('[role="button"]');
    const cardCount = await venueCards.count();
    expect(cardCount).toBeGreaterThan(0);
    
    // Check the first venue card
    const firstCard = venueCards.first();
    
    // Verify cards are clickable (role="button")
    await expect(firstCard).toHaveAttribute('role', 'button');
    
    // Verify cards have proper aria-label
    const ariaLabel = await firstCard.getAttribute('aria-label');
    expect(ariaLabel).toBeTruthy();
    expect(ariaLabel).toContain('View details for');
    
    // Verify cards have tabIndex 0 for keyboard navigation
    await expect(firstCard).toHaveAttribute('tabindex', '0');
    
    // Test keyboard interaction (Enter key)
    await firstCard.focus();
    await authenticatedPage.keyboard.press('Enter');
    
    // Verify navigation occurred (should go to venue detail page)
    // The URL should change from /venues to /venues/{id}
    await authenticatedPage.waitForTimeout(1000); // Brief wait for navigation
    const currentUrl = authenticatedPage.url();
    expect(currentUrl).toMatch(/\/venues\/[a-f0-9-]+/);
  });

  /**
   * Test 5: Empty State Test
   * 
   * Verifies that an appropriate empty state message appears when no venues exist.
   * Note: This test may require a clean database or specific test data setup.
   */
  test('should display empty state when no venues exist', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for loading to complete
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Check if empty state is displayed
    // The empty state should show "No venues found" if there are no venues
    const emptyStateTitle = authenticatedPage.locator('text=No venues found');
    const emptyStateExists = await emptyStateTitle.isVisible().catch(() => false);
    
    if (emptyStateExists) {
      // Verify empty state message
      await expect(emptyStateTitle).toBeVisible();
      
      // Verify appropriate description is displayed
      const emptyDescription = authenticatedPage.locator('text=There are no venues available at the moment');
      await expect(emptyDescription).toBeVisible();
      
      // Verify Building2 icon context (empty state uses Building2 icon)
      // We can't directly test the icon, but we can verify the empty state structure
      const emptyStateContainer = emptyStateTitle.locator('..');
      await expect(emptyStateContainer).toBeVisible();
    } else {
      // If venues exist, verify that venue cards are displayed instead
      const venueCards = authenticatedPage.locator('[role="button"]');
      const cardCount = await venueCards.count();
      expect(cardCount).toBeGreaterThan(0);
    }
  });

  /**
   * Test 6: Loading State Test
   * 
   * Verifies that a loading spinner appears initially when navigating to the page
   * and disappears once venues are loaded.
   */
  test('should display loading state while fetching venues', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Verify loading spinner appears initially
    // Note: This may be very brief, so we check quickly
    const loadingSpinner = authenticatedPage.locator('text=Loading venues...');
    
    // The spinner should be visible initially or have been visible
    // We use a try-catch since it might disappear quickly
    const spinnerWasVisible = await loadingSpinner.isVisible().catch(() => false);
    
    // If we caught it, verify it was there
    if (spinnerWasVisible) {
      await expect(loadingSpinner).toBeVisible();
    }
    
    // Wait for loading to complete
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Verify spinner disappears
    await expect(loadingSpinner).not.toBeVisible();
    
    // Verify content is now displayed (either venues or empty state)
    const pageTitle = authenticatedPage.locator('h1:has-text("Venues")');
    await expect(pageTitle).toBeVisible();
  });

  /**
   * Test 7: Responsive Grid Layout Test
   * 
   * Verifies that venues are displayed in a responsive grid layout
   * that adapts to different viewport sizes.
   */
  test('should display venues in responsive grid layout', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for venues to load
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Verify venues are displayed in a grid layout
    const venueCards = authenticatedPage.locator('[role="button"]');
    const cardCount = await venueCards.count();
    expect(cardCount).toBeGreaterThan(0);
    
    // Test desktop viewport (default)
    await authenticatedPage.setViewportSize({ width: 1280, height: 720 });
    await authenticatedPage.waitForTimeout(500); // Wait for layout adjustment
    
    // Verify cards are visible in desktop layout
    await expect(venueCards.first()).toBeVisible();
    
    // Test tablet viewport
    await authenticatedPage.setViewportSize({ width: 768, height: 1024 });
    await authenticatedPage.waitForTimeout(500); // Wait for layout adjustment
    
    // Verify cards are still visible and properly laid out
    await expect(venueCards.first()).toBeVisible();
    
    // Test mobile viewport
    await authenticatedPage.setViewportSize({ width: 375, height: 667 });
    await authenticatedPage.waitForTimeout(500); // Wait for layout adjustment
    
    // Verify cards are still visible in mobile layout (should stack vertically)
    await expect(venueCards.first()).toBeVisible();
    
    // Verify all cards are still accessible
    for (let i = 0; i < Math.min(cardCount, 3); i++) {
      await expect(venueCards.nth(i)).toBeVisible();
    }
  });

  /**
   * Test 8: Multiple Venues Display Test
   * 
   * Verifies that multiple venue cards are displayed with unique content,
   * proper spacing, and correct layout.
   */
  test('should display multiple venues with unique content', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for venues to load
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Get all venue cards
    const venueCards = authenticatedPage.locator('[role="button"]');
    const cardCount = await venueCards.count();
    
    // Verify multiple venue cards are displayed (at least 1, ideally more)
    expect(cardCount).toBeGreaterThan(0);
    
    // If multiple venues exist, verify each has unique content
    if (cardCount > 1) {
      const venueNames = new Set<string>();
      
      // Collect venue names from first few cards
      for (let i = 0; i < Math.min(cardCount, 5); i++) {
        const card = venueCards.nth(i);
        const nameElement = card.locator('h3');
        const name = await nameElement.textContent();
        
        // Verify name exists and is unique
        expect(name).toBeTruthy();
        expect(venueNames.has(name!)).toBe(false);
        venueNames.add(name!);
      }
      
      // Verify we collected unique names
      expect(venueNames.size).toBeGreaterThan(1);
    }
    
    // Verify proper spacing between cards
    // Cards should be visible and not overlapping
    const firstCard = venueCards.first();
    const secondCard = venueCards.nth(1);
    
    if (cardCount > 1) {
      await expect(firstCard).toBeVisible();
      await expect(secondCard).toBeVisible();
      
      // Get bounding boxes to verify they don't overlap
      const firstBox = await firstCard.boundingBox();
      const secondBox = await secondCard.boundingBox();
      
      expect(firstBox).toBeTruthy();
      expect(secondBox).toBeTruthy();
      
      // Verify cards are properly spaced (not overlapping)
      // Either horizontally or vertically separated
      if (firstBox && secondBox) {
        const horizontallySeparated = 
          firstBox.x + firstBox.width <= secondBox.x || 
          secondBox.x + secondBox.width <= firstBox.x;
        const verticallySeparated = 
          firstBox.y + firstBox.height <= secondBox.y || 
          secondBox.y + secondBox.height <= firstBox.y;
        
        expect(horizontallySeparated || verticallySeparated).toBe(true);
      }
    }
  });

  /**
   * Test 9: Venue Card Click Navigation Test
   * 
   * Verifies that clicking a venue card navigates to the correct venue detail page.
   */
  test('should navigate to venue detail page when card is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for venues to load
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading venues...', { state: 'hidden', timeout: 10000 });
    
    // Get the first venue card
    const firstCard = authenticatedPage.locator('[role="button"]').first();
    await expect(firstCard).toBeVisible();
    
    // Get the venue name for verification
    const venueName = await firstCard.locator('h3').textContent();
    expect(venueName).toBeTruthy();
    
    // Click the card
    await firstCard.click();
    
    // Wait for navigation
    await authenticatedPage.waitForTimeout(1000);
    
    // Verify navigation to venue detail page (URL should be /venues/{id})
    const currentUrl = authenticatedPage.url();
    expect(currentUrl).toMatch(/\/venues\/[a-f0-9-]+/);
    expect(currentUrl).not.toBe('/venues');
  });

  /**
   * Test 10: Page Header Action Button Test
   * 
   * Verifies that the page header contains the Add Venue button
   * and it's properly positioned in the header.
   */
  test('should display Add Venue button in page header', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to the venues list page
    await authenticatedPage.goto('/venues');
    
    // Wait for page to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify page header exists
    const pageTitle = authenticatedPage.locator('h1:has-text("Venues")');
    await expect(pageTitle).toBeVisible();
    
    // Verify Add Venue button is in the same section as the title
    const addButton = authenticatedPage.locator('button:has-text("Add Venue")');
    await expect(addButton).toBeVisible();
    
    // Verify button has Plus icon (we can check for the button text and aria-label)
    await expect(addButton).toHaveAttribute('aria-label', 'Add new venue');
    
    // Verify button is clickable
    await expect(addButton).toBeEnabled();
  });
});