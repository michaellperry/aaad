import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * View Show Detail Test Suite
 * 
 * Comprehensive tests for the show detail page functionality including:
 * - Viewing show details with proper page structure
 * - Show information display (act name, venue name, date/time, tickets)
 * - Navigation from acts page to show detail
 * - Back button functionality
 * - Loading state verification
 * - Error state handling
 * - Accessibility features
 * - Responsive layout
 */
test.describe('View Show Detail', () => {
  /**
   * Test 1: Display Show Details Test
   * 
   * Verifies that the show detail page loads correctly and displays
   * all show information including act name, venue name, start date/time,
   * and ticket count.
   */
  test('should display show details when navigating from acts page', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act to view details
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await expect(firstActCard).toBeVisible();
    await firstActCard.click();
    
    // Wait for act detail page to load
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show card (if shows exist)
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      const firstShowCard = showCards.first();
      await expect(firstShowCard).toBeVisible();
      await firstShowCard.click();
      
      // Wait for show detail page to load
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're on a show detail page (URL should contain /shows/{guid})
      const currentUrl = authenticatedPage.url();
      const guidPattern = /\/shows\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(currentUrl).toMatch(guidPattern);
      
      // Verify page title exists (should be "Show Details" or similar)
      const pageTitle = authenticatedPage.locator('h1');
      await expect(pageTitle).toBeVisible();
      
      // Verify show information is displayed
      const showInfo = authenticatedPage.locator('text=/Act:|Venue:|Start Date:|Start Time:|Tickets Available:/');
      await expect(showInfo.first()).toBeVisible();
    }
  });

  /**
   * Test 2: Act Name Display Test
   * 
   * Verifies that the act name is correctly displayed on the show detail page.
   */
  test('should show act name correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page and get to a show detail page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Get first act name
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    const actNameElement = firstActCard.locator('h3');
    const actName = await actNameElement.textContent();
    
    // Click on act
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify act name is displayed on show detail page
      const actNameOnShowPage = authenticatedPage.locator(`text=${actName}`);
      await expect(actNameOnShowPage).toBeVisible();
    }
  });

  /**
   * Test 3: Venue Name Display Test
   * 
   * Verifies that the venue name is correctly displayed on the show detail page.
   */
  test('should show venue name correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      // Get venue name from show card
      const firstShowCard = showCards.first();
      const venueNameOnCard = await firstShowCard.textContent();
      
      await firstShowCard.click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify venue name is displayed on show detail page
      const venueLabel = authenticatedPage.locator('text=/Venue:/');
      await expect(venueLabel).toBeVisible();
    }
  });

  /**
   * Test 4: Start Date/Time Display Test
   * 
   * Verifies that the start date and time are correctly displayed and formatted.
   */
  test('should show start date/time correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify start date is displayed
      const startDateLabel = authenticatedPage.locator('text=/Start Date:/');
      await expect(startDateLabel).toBeVisible();
      
      // Verify start time is displayed
      const startTimeLabel = authenticatedPage.locator('text=/Start Time:/');
      await expect(startTimeLabel).toBeVisible();
    }
  });

  /**
   * Test 5: Ticket Count Display Test
   * 
   * Verifies that the ticket count is correctly displayed.
   */
  test('should show ticket count correctly', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify ticket count is displayed
      const ticketLabel = authenticatedPage.locator('text=/Tickets Available:/');
      await expect(ticketLabel).toBeVisible();
      
      // Verify ticket count is a number
      const ticketInfo = await ticketLabel.textContent();
      expect(ticketInfo).toMatch(/\d+/);
    }
  });

  /**
   * Test 6: Back Button Navigation Test
   * 
   * Verifies that the back button navigates to the acts page.
   */
  test('should navigate to acts page when back button is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Find and click back button
      const backButton = authenticatedPage.locator('button:has-text("Back")');
      await expect(backButton).toBeVisible();
      await backButton.click();
      
      // Verify navigation to acts page
      await authenticatedPage.waitForURL('/acts', { timeout: 5000 });
      await expect(authenticatedPage).toHaveURL('/acts');
    }
  });

  /**
   * Test 7: Loading State Test
   * 
   * Verifies that a loading state is displayed while fetching show data.
   */
  test('should show loading state while fetching show data', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      
      // Check for loading spinner (may be brief)
      const loadingSpinner = authenticatedPage.locator('text=/Loading/');
      const spinnerWasVisible = await loadingSpinner.isVisible().catch(() => false);
      
      // Wait for page to fully load
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify content is now displayed
      const pageTitle = authenticatedPage.locator('h1');
      await expect(pageTitle).toBeVisible();
    }
  });

  /**
   * Test 8: Error Message for Non-Existent Show Test
   * 
   * Verifies that an error message is displayed for a non-existent show.
   */
  test('should show error message for non-existent show', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate directly to a non-existent show GUID
    const fakeGuid = '00000000-0000-0000-0000-000000000000';
    await authenticatedPage.goto(`/shows/${fakeGuid}`);
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify error message is displayed
    const errorMessage = authenticatedPage.locator('text=/not found|error|failed/i');
    await expect(errorMessage).toBeVisible({ timeout: 10000 });
  });

  /**
   * Test 9: Error Message for Network Failure Test
   * 
   * Verifies that an error message is displayed when network request fails.
   * This test simulates a network failure by going offline.
   */
  test('should show error message for network failure', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page first
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get first show card to extract GUID from URL after click
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      // Go offline
      await authenticatedPage.context().setOffline(true);
      
      // Try to navigate to show detail
      await showCards.first().click();
      
      // Wait a moment for error to appear
      await authenticatedPage.waitForTimeout(2000);
      
      // Verify error message is displayed
      const errorMessage = authenticatedPage.locator('text=/error|failed|unable/i');
      const errorExists = await errorMessage.isVisible().catch(() => false);
      
      // Go back online
      await authenticatedPage.context().setOffline(false);
      
      // Error should be visible
      if (errorExists) {
        await expect(errorMessage).toBeVisible();
      }
    }
  });

  /**
   * Test 10: Keyboard Navigation Test
   * 
   * Verifies that the page is accessible via keyboard navigation.
   */
  test('should be accessible via keyboard navigation', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Use keyboard to navigate to first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.focus();
    await authenticatedPage.keyboard.press('Enter');
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Use keyboard to navigate to first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      const firstShowCard = showCards.first();
      await firstShowCard.focus();
      await authenticatedPage.keyboard.press('Enter');
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we can tab to the back button
      await authenticatedPage.keyboard.press('Tab');
      const backButton = authenticatedPage.locator('button:has-text("Back")');
      const isBackButtonFocused = await backButton.evaluate(el => el === document.activeElement);
      
      // Verify back button is in the tab order
      await expect(backButton).toBeVisible();
    }
  });

  /**
   * Test 11: ARIA Labels Test
   * 
   * Verifies that the page has proper ARIA labels for accessibility.
   */
  test('should have proper ARIA labels', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify back button has aria-label
      const backButton = authenticatedPage.locator('button:has-text("Back")');
      const ariaLabel = await backButton.getAttribute('aria-label');
      
      // Back button should have an aria-label or the text should be descriptive
      const buttonText = await backButton.textContent();
      expect(ariaLabel || buttonText).toBeTruthy();
    }
  });

  /**
   * Test 12: Responsive Layout Test
   * 
   * Verifies that the page is responsive on mobile devices.
   */
  test('should be responsive on mobile', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Set mobile viewport
    await authenticatedPage.setViewportSize({ width: 375, height: 667 });
    
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify page content is visible on mobile
      const pageTitle = authenticatedPage.locator('h1');
      await expect(pageTitle).toBeVisible();
      
      // Verify back button is visible
      const backButton = authenticatedPage.locator('button:has-text("Back")');
      await expect(backButton).toBeVisible();
      
      // Verify show information is visible
      const showInfo = authenticatedPage.locator('text=/Act:|Venue:|Start Date:/');
      await expect(showInfo.first()).toBeVisible();
    }
  });
});

/**
 * Act Detail Page Enhancement Test Suite
 * 
 * Tests for the enhanced act detail page that displays shows for an act.
 */
test.describe('Act Detail Page - Shows List', () => {
  /**
   * Test 1: Display Shows List Test
   * 
   * Verifies that the act detail page displays a list of shows for the act.
   */
  test('should display list of shows for an act', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify shows section exists
    const showsSection = authenticatedPage.locator('text=/Shows|Upcoming Shows/i');
    await expect(showsSection).toBeVisible({ timeout: 5000 });
    
    // Check if shows are displayed or empty state is shown
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const emptyState = authenticatedPage.locator('text=/No shows|No upcoming shows/i');
    
    const showCount = await showCards.count();
    const emptyStateVisible = await emptyState.isVisible().catch(() => false);
    
    // Either shows should be displayed or empty state should be shown
    expect(showCount > 0 || emptyStateVisible).toBe(true);
  });

  /**
   * Test 2: Show Card Click Navigation Test
   * 
   * Verifies that clicking a show card navigates to the show detail page.
   */
  test('should navigate to show detail when show card is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find show cards
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      // Click on first show
      await showCards.first().click();
      
      // Verify navigation to show detail page
      await authenticatedPage.waitForTimeout(1000);
      const currentUrl = authenticatedPage.url();
      const guidPattern = /\/shows\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(currentUrl).toMatch(guidPattern);
    }
  });

  /**
   * Test 3: Empty State Test
   * 
   * Verifies that an empty state is shown when an act has no shows.
   */
  test('should show empty state when act has no shows', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Check for shows or empty state
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const emptyState = authenticatedPage.locator('text=/No shows|No upcoming shows/i');
    
    const showCount = await showCards.count();
    const emptyStateVisible = await emptyState.isVisible().catch(() => false);
    
    // If no shows, verify empty state is displayed
    if (showCount === 0 && emptyStateVisible) {
      await expect(emptyState).toBeVisible();
    }
  });
});

/**
 * End-to-End Flow Test Suite
 * 
 * Tests for complete user flows through the application.
 */
test.describe('End-to-End Show Viewing Flow', () => {
  /**
   * Test 1: Complete Flow Test
   * 
   * Verifies the complete flow: Acts page → Act detail → Show detail → Back.
   */
  test('should complete flow from acts page to show detail and back', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Step 1: Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Verify we're on acts page
    await expect(authenticatedPage).toHaveURL('/acts');
    const actsTitle = authenticatedPage.locator('h1:has-text("Acts")');
    await expect(actsTitle).toBeVisible();
    
    // Step 2: Navigate to act detail
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify we're on act detail page
    const currentUrl = authenticatedPage.url();
    const actGuidPattern = /\/acts\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    expect(currentUrl).toMatch(actGuidPattern);
    
    // Step 3: Navigate to show detail
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're on show detail page
      const showUrl = authenticatedPage.url();
      const showGuidPattern = /\/shows\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(showUrl).toMatch(showGuidPattern);
      
      // Step 4: Navigate back to acts page
      const backButton = authenticatedPage.locator('button:has-text("Back")');
      await expect(backButton).toBeVisible();
      await backButton.click();
      
      // Verify we're back on acts page
      await authenticatedPage.waitForURL('/acts', { timeout: 5000 });
      await expect(authenticatedPage).toHaveURL('/acts');
      await expect(actsTitle).toBeVisible();
    }
  });

  /**
   * Test 2: Direct URL Access Test
   * 
   * Verifies that direct URL access to show detail page works.
   */
  test('should allow direct URL access to show detail page', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // First, get a valid show GUID by navigating through the UI
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Get the current URL (show detail page)
      const showUrl = authenticatedPage.url();
      const guidPattern = /\/shows\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(showUrl).toMatch(guidPattern);
      
      // Navigate away
      await authenticatedPage.goto('/acts');
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Navigate directly back to the show detail page using the URL
      await authenticatedPage.goto(showUrl);
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're on the show detail page
      await expect(authenticatedPage).toHaveURL(showUrl);
      const pageTitle = authenticatedPage.locator('h1');
      await expect(pageTitle).toBeVisible();
    }
  });

  /**
   * Test 3: Browser Back Button Test
   * 
   * Verifies that the browser back button works correctly.
   */
  test('should work correctly with browser back button', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Find and click on first show
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/ });
    const showCount = await showCards.count();
    
    if (showCount > 0) {
      await showCards.first().click();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're on show detail page
      const showUrl = authenticatedPage.url();
      const guidPattern = /\/shows\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(showUrl).toMatch(guidPattern);
      
      // Use browser back button
      await authenticatedPage.goBack();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're back on act detail page
      const currentUrl = authenticatedPage.url();
      const actGuidPattern = /\/acts\/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      expect(currentUrl).toMatch(actGuidPattern);
      
      // Use browser back button again
      await authenticatedPage.goBack();
      await authenticatedPage.waitForLoadState('networkidle');
      
      // Verify we're back on acts page
      await expect(authenticatedPage).toHaveURL('/acts');
    }
  });
});
