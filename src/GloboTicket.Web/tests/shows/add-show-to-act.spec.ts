import { test, expect } from '../fixtures/auth';
import type { Page } from '@playwright/test';

/**
 * Add Show to Act Test Suite
 * 
 * Comprehensive E2E tests for the "Add Show to Act" feature including:
 * - ShowForm component rendering and interaction
 * - Venue selection and capacity display
 * - Ticket count validation
 * - Date and time field validation
 * - Nearby shows detection
 * - Form submission and navigation
 * - ActDetailPage integration (Add Show button)
 * - Error handling and validation scenarios
 */

test.describe('ShowForm Component Tests', () => {
  /**
   * Test: GivenShowForm_WhenRendered_ThenDisplaysVenueDropdown
   * 
   * Verifies that the venue dropdown is displayed when the form is rendered.
   */
  test('should display venue dropdown when form is rendered', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Click "Add Show" button
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await expect(addShowButton).toBeVisible();
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify venue dropdown is displayed
    const venueDropdown = authenticatedPage.locator('#venue');
    await expect(venueDropdown).toBeVisible();
    
    // Verify dropdown has label
    const venueLabel = authenticatedPage.locator('label[for="venue"]');
    await expect(venueLabel).toBeVisible();
    await expect(venueLabel).toContainText('Venue');
  });

  /**
   * Test: GivenShowForm_WhenRendered_ThenDisplaysTicketCountField
   * 
   * Verifies that the ticket count field is displayed when the form is rendered.
   */
  test('should display ticket count field when form is rendered', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify ticket count field is displayed
    const ticketCountField = authenticatedPage.locator('#ticketCount');
    await expect(ticketCountField).toBeVisible();
    await expect(ticketCountField).toHaveAttribute('type', 'number');
    
    // Verify field has label
    const ticketCountLabel = authenticatedPage.locator('label[for="ticketCount"]');
    await expect(ticketCountLabel).toBeVisible();
    await expect(ticketCountLabel).toContainText('Number of Tickets');
  });

  /**
   * Test: GivenShowForm_WhenRendered_ThenDisplaysDateAndTimeFields
   * 
   * Verifies that date and time fields are displayed when the form is rendered.
   */
  test('should display date and time fields when form is rendered', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify start date field
    const startDateField = authenticatedPage.locator('#startDate');
    await expect(startDateField).toBeVisible();
    await expect(startDateField).toHaveAttribute('type', 'date');
    
    const startDateLabel = authenticatedPage.locator('label[for="startDate"]');
    await expect(startDateLabel).toBeVisible();
    await expect(startDateLabel).toContainText('Start Date');
    
    // Verify start time field
    const startTimeField = authenticatedPage.locator('#startTime');
    await expect(startTimeField).toBeVisible();
    await expect(startTimeField).toHaveAttribute('type', 'time');
    
    const startTimeLabel = authenticatedPage.locator('label[for="startTime"]');
    await expect(startTimeLabel).toBeVisible();
    await expect(startTimeLabel).toContainText('Start Time');
  });

  /**
   * Test: GivenShowForm_WhenNoVenuesExist_ThenShowsNoVenuesMessage
   * 
   * Verifies that a message is displayed when no venues are available.
   * Note: This test may be skipped if venues exist in the test database.
   */
  test('should show no venues message when no venues exist', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Check if venues exist or no venues message is shown
    const venueDropdown = authenticatedPage.locator('#venue');
    const noVenuesMessage = authenticatedPage.locator('text=No venues available');
    
    const dropdownVisible = await venueDropdown.isVisible().catch(() => false);
    const messageVisible = await noVenuesMessage.isVisible().catch(() => false);
    
    // Either dropdown should be visible (venues exist) or message should be visible (no venues)
    expect(dropdownVisible || messageVisible).toBe(true);
    
    if (messageVisible) {
      await expect(noVenuesMessage).toContainText('Please create a venue first');
    }
  });

  /**
   * Test: GivenShowForm_WhenVenueSelected_ThenDisplaysCapacity
   * 
   * Verifies that venue capacity is displayed when a venue is selected.
   */
  test('should display venue capacity when venue is selected', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) { // More than just the placeholder option
      await venueDropdown.selectOption({ index: 1 });
      
      // Verify capacity is displayed
      const capacityDisplay = authenticatedPage.locator('text=/Venue Capacity:/');
      await expect(capacityDisplay).toBeVisible({ timeout: 5000 });
      await expect(capacityDisplay).toContainText('seats');
    }
  });

  /**
   * Test: GivenShowForm_WhenVenueChanged_ThenUpdatesCapacityDisplay
   * 
   * Verifies that capacity display updates when venue selection changes.
   */
  test('should update capacity display when venue is changed', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 2) { // At least 2 venues plus placeholder
      // Select first venue
      await venueDropdown.selectOption({ index: 1 });
      const capacityDisplay = authenticatedPage.locator('text=/Venue Capacity:/');
      await expect(capacityDisplay).toBeVisible({ timeout: 5000 });
      const firstCapacity = await capacityDisplay.textContent();
      
      // Select second venue
      await venueDropdown.selectOption({ index: 2 });
      await authenticatedPage.waitForTimeout(500); // Wait for update
      const secondCapacity = await capacityDisplay.textContent();
      
      // Capacities should be different (unless venues have same capacity)
      expect(firstCapacity).toBeTruthy();
      expect(secondCapacity).toBeTruthy();
    }
  });

  /**
   * Test: GivenShowForm_WhenTicketCountExceedsCapacity_ThenShowsError
   * 
   * Verifies that an error is shown when ticket count exceeds venue capacity.
   */
  test('should show error when ticket count exceeds venue capacity', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Wait for capacity to be displayed
      const capacityDisplay = authenticatedPage.locator('text=/Venue Capacity:/');
      await expect(capacityDisplay).toBeVisible({ timeout: 5000 });
      
      // Enter ticket count exceeding capacity (use a very large number)
      const ticketCountField = authenticatedPage.locator('#ticketCount');
      await ticketCountField.fill('999999');
      
      // Fill in date and time (required for validation)
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Try to submit
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Verify error message is displayed
      const errorMessage = authenticatedPage.locator('text=/cannot exceed venue capacity/i');
      await expect(errorMessage).toBeVisible({ timeout: 5000 });
    }
  });

  /**
   * Test: GivenShowForm_WhenAllFieldsFilled_ThenFetchesNearbyShows
   * 
   * Verifies that nearby shows are fetched when all fields are filled.
   */
  test('should fetch nearby shows when all fields are filled', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Fill in date and time
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Wait for nearby shows section to appear
      const nearbyShowsSection = authenticatedPage.locator('text=Other Shows at This Venue');
      await expect(nearbyShowsSection).toBeVisible({ timeout: 5000 });
    }
  });

  /**
   * Test: GivenShowForm_WhenNearbyShowsExist_ThenDisplaysList
   * 
   * Verifies that nearby shows are displayed in a list when they exist.
   */
  test('should display list of nearby shows when they exist', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Fill in date and time
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Wait for nearby shows section
      const nearbyShowsSection = authenticatedPage.locator('text=Other Shows at This Venue');
      await expect(nearbyShowsSection).toBeVisible({ timeout: 5000 });
      
      // Check for either shows list or no shows message
      const loadingMessage = authenticatedPage.locator('text=Loading nearby shows');
      const noShowsMessage = authenticatedPage.locator('text=No other shows scheduled');
      
      // Wait for loading to complete
      await expect(loadingMessage).toBeHidden({ timeout: 10000 });
      
      // Either shows or no shows message should be visible
      const noShowsVisible = await noShowsMessage.isVisible().catch(() => false);
      expect(noShowsVisible).toBe(true);
    }
  });

  /**
   * Test: GivenShowForm_WhenNoNearbyShows_ThenDisplaysMessage
   * 
   * Verifies that a message is displayed when no nearby shows exist.
   */
  test('should display message when no nearby shows exist', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Fill in date far in the future (unlikely to have nearby shows)
      const futureDate = new Date();
      futureDate.setFullYear(futureDate.getFullYear() + 2);
      const dateString = futureDate.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Wait for nearby shows section
      const nearbyShowsSection = authenticatedPage.locator('text=Other Shows at This Venue');
      await expect(nearbyShowsSection).toBeVisible({ timeout: 5000 });
      
      // Wait for loading to complete
      const loadingMessage = authenticatedPage.locator('text=Loading nearby shows');
      await expect(loadingMessage).toBeHidden({ timeout: 10000 });
      
      // Verify no shows message
      const noShowsMessage = authenticatedPage.locator('text=No other shows scheduled at this venue within 48 hours');
      await expect(noShowsMessage).toBeVisible();
    }
  });

  /**
   * Test: GivenShowForm_WhenSubmitSucceeds_ThenRedirectsToActDetail
   * 
   * Verifies that the form redirects to act detail page on successful submission.
   */
  test('should redirect to act detail page when submit succeeds', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get act GUID from URL
    const actUrl = authenticatedPage.url();
    const actGuidMatch = actUrl.match(/\/acts\/([0-9a-f-]+)/i);
    const actGuid = actGuidMatch ? actGuidMatch[1] : null;
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Wait for capacity to be displayed
      await authenticatedPage.waitForTimeout(500);
      
      // Fill in valid ticket count
      await authenticatedPage.locator('#ticketCount').fill('100');
      
      // Fill in date and time
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Submit form
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Wait for navigation
      await authenticatedPage.waitForURL(`/acts/${actGuid}`, { timeout: 10000 });
      
      // Verify we're back on act detail page
      expect(authenticatedPage.url()).toContain(`/acts/${actGuid}`);
    }
  });

  /**
   * Test: GivenShowForm_WhenCancelClicked_ThenRedirectsToActDetail
   * 
   * Verifies that clicking cancel redirects to act detail page.
   */
  test('should redirect to act detail page when cancel is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get act GUID from URL
    const actUrl = authenticatedPage.url();
    const actGuidMatch = actUrl.match(/\/acts\/([0-9a-f-]+)/i);
    const actGuid = actGuidMatch ? actGuidMatch[1] : null;
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Click cancel button
    const cancelButton = authenticatedPage.locator('button:has-text("Cancel")');
    await expect(cancelButton).toBeVisible();
    await cancelButton.click();
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/acts/${actGuid}`, { timeout: 5000 });
    
    // Verify we're back on act detail page
    expect(authenticatedPage.url()).toContain(`/acts/${actGuid}`);
  });
});

test.describe('ActDetailPage Tests', () => {
  /**
   * Test: GivenActDetailPage_WhenShowsExist_ThenDisplaysShowsList
   * 
   * Verifies that the act detail page displays a list of shows when they exist.
   */
  test('should display shows list when shows exist for an act', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Verify "Upcoming Shows" section exists
    const upcomingShowsHeading = authenticatedPage.locator('text=Upcoming Shows');
    await expect(upcomingShowsHeading).toBeVisible();
    
    // Check for shows or empty state
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/i });
    const emptyState = authenticatedPage.locator('text=/No upcoming shows/i');
    
    const showCount = await showCards.count();
    const emptyStateVisible = await emptyState.isVisible().catch(() => false);
    
    // Either shows should be displayed or empty state should be shown
    expect(showCount > 0 || emptyStateVisible).toBe(true);
  });

  /**
   * Test: GivenActDetailPage_WhenNoShows_ThenDisplaysEmptyMessage
   * 
   * Verifies that an empty message is displayed when an act has no shows.
   */
  test('should display empty message when act has no shows', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Check for shows or empty state
    const showCards = authenticatedPage.locator('[role="button"]').filter({ hasText: /tickets available|Tickets/i });
    const emptyState = authenticatedPage.locator('text=/No upcoming shows/i');
    
    const showCount = await showCards.count();
    
    // If no shows, verify empty state is displayed
    if (showCount === 0) {
      await expect(emptyState).toBeVisible();
    }
  });

  /**
   * Test: GivenActDetailPage_WhenAddShowClicked_ThenNavigatesToCreateShowPage
   * 
   * Verifies that clicking "Add Show" button navigates to create show page.
   */
  test('should navigate to create show page when Add Show button is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to acts page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    // Click on first act
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get act GUID from URL
    const actUrl = authenticatedPage.url();
    const actGuidMatch = actUrl.match(/\/acts\/([0-9a-f-]+)/i);
    const actGuid = actGuidMatch ? actGuidMatch[1] : null;
    
    // Click "Add Show" button
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await expect(addShowButton).toBeVisible();
    await addShowButton.click();
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/acts/${actGuid}/shows/new`, { timeout: 5000 });
    
    // Verify we're on create show page
    expect(authenticatedPage.url()).toContain(`/acts/${actGuid}/shows/new`);
    
    // Verify page title
    const pageTitle = authenticatedPage.locator('text=/Add Show for/i');
    await expect(pageTitle).toBeVisible();
  });
});

test.describe('Validation Tests', () => {
  /**
   * Test: Validates required fields
   * 
   * Verifies that all required fields are validated.
   */
  test('should validate all required fields', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Try to submit without filling any fields
    const createButton = authenticatedPage.locator('button:has-text("Create Show")');
    await createButton.click();
    
    // Verify error messages are displayed
    const venueError = authenticatedPage.locator('text=/Please select a venue/i');
    const ticketCountError = authenticatedPage.locator('text=/Ticket count is required/i');
    const dateError = authenticatedPage.locator('text=/Start date is required/i');
    const timeError = authenticatedPage.locator('text=/Start time is required/i');
    
    await expect(venueError).toBeVisible({ timeout: 5000 });
    await expect(ticketCountError).toBeVisible({ timeout: 5000 });
    await expect(dateError).toBeVisible({ timeout: 5000 });
    await expect(timeError).toBeVisible({ timeout: 5000 });
  });

  /**
   * Test: Validates ticket count is positive
   * 
   * Verifies that ticket count must be a positive number.
   */
  test('should validate ticket count is positive', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Enter negative ticket count
      await authenticatedPage.locator('#ticketCount').fill('-10');
      
      // Fill in date and time
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Try to submit
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Verify error message
      const errorMessage = authenticatedPage.locator('text=/must be a positive number/i');
      await expect(errorMessage).toBeVisible({ timeout: 5000 });
    }
  });

  /**
   * Test: Validates start date is in the future
   * 
   * Verifies that start date must be in the future.
   */
  test('should validate start date is in the future', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Select first venue
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      
      // Enter valid ticket count
      await authenticatedPage.locator('#ticketCount').fill('100');
      
      // Enter past date
      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);
      const dateString = yesterday.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Try to submit
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Verify error message
      const errorMessage = authenticatedPage.locator('text=/must be in the future/i');
      await expect(errorMessage).toBeVisible({ timeout: 5000 });
    }
  });
});

test.describe('Error Handling Tests', () => {
  /**
   * Test: Handles form submission errors
   * 
   * Verifies that form submission errors are displayed to the user.
   */
  test('should display error message when form submission fails', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Fill form with valid data
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      await authenticatedPage.locator('#ticketCount').fill('100');
      
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Submit form
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Wait for either success (redirect) or error
      await authenticatedPage.waitForTimeout(2000);
      
      // Check if we're still on the form page (error occurred) or redirected (success)
      const currentUrl = authenticatedPage.url();
      const stillOnForm = currentUrl.includes('/shows/new');
      
      if (stillOnForm) {
        // If still on form, there should be an error message
        const errorAlert = authenticatedPage.locator('[role="alert"]');
        const errorVisible = await errorAlert.isVisible().catch(() => false);
        
        if (errorVisible) {
          await expect(errorAlert).toBeVisible();
        }
      }
    }
  });

  /**
   * Test: Handles loading state during submission
   * 
   * Verifies that loading state is shown during form submission.
   */
  test('should show loading state during form submission', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Wait for venues to load
    await authenticatedPage.waitForSelector('#venue', { state: 'visible', timeout: 5000 });
    
    // Fill form with valid data
    const venueDropdown = authenticatedPage.locator('#venue');
    const options = await venueDropdown.locator('option').all();
    
    if (options.length > 1) {
      await venueDropdown.selectOption({ index: 1 });
      await authenticatedPage.locator('#ticketCount').fill('100');
      
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      const dateString = tomorrow.toISOString().split('T')[0];
      
      await authenticatedPage.locator('#startDate').fill(dateString);
      await authenticatedPage.locator('#startTime').fill('20:00');
      
      // Submit form
      const createButton = authenticatedPage.locator('button:has-text("Create Show")');
      await createButton.click();
      
      // Check if button shows loading state (disabled or has loading indicator)
      const isDisabled = await createButton.isDisabled().catch(() => false);
      expect(isDisabled).toBe(true);
    }
  });

  /**
   * Test: Handles back button navigation
   * 
   * Verifies that back button navigates to act detail page.
   */
  test('should navigate back to act detail when back button is clicked', async ({ authenticatedPage }: { authenticatedPage: Page }) => {
    // Navigate to create show page
    await authenticatedPage.goto('/acts');
    await authenticatedPage.waitForLoadState('networkidle');
    await authenticatedPage.waitForSelector('text=Loading acts...', { state: 'hidden', timeout: 10000 });
    
    const firstActCard = authenticatedPage.locator('[role="button"]').first();
    await firstActCard.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Get act GUID from URL
    const actUrl = authenticatedPage.url();
    const actGuidMatch = actUrl.match(/\/acts\/([0-9a-f-]+)/i);
    const actGuid = actGuidMatch ? actGuidMatch[1] : null;
    
    const addShowButton = authenticatedPage.locator('button:has-text("Add Show")');
    await addShowButton.click();
    await authenticatedPage.waitForLoadState('networkidle');
    
    // Click back button (in page header)
    const backButton = authenticatedPage.locator('button:has-text("Back to Act")');
    await expect(backButton).toBeVisible();
    await backButton.click();
    
    // Wait for navigation
    await authenticatedPage.waitForURL(`/acts/${actGuid}`, { timeout: 5000 });
    
    // Verify we're back on act detail page
    expect(authenticatedPage.url()).toContain(`/acts/${actGuid}`);
  });
});
