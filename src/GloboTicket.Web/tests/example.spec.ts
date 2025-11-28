import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('/');
  
  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/GloboTicket/);
});

test('get started link', async ({ page }) => {
  await page.goto('/');

  // Example test - update based on your actual application
  await expect(page.locator('body')).toBeVisible();
});