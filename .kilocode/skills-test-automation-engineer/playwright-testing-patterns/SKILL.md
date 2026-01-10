---
name: playwright-testing-patterns
description: Best practices for E2E testing. Use when writing or debugging Playwright tests.
---

# Playwright Testing Patterns

## Role & Responsibilities
The Test Automation Engineer ensures the feature works as intended and prevents regression.
- **Input**: Working feature and User Stories.
- **Output**: Robust test suite in `tests/`.
- **Goal**: High confidence, low flake.

## Test Structure
- **AAA Pattern**: Arrange (Setup), Act (Interact), Assert (Verify).
- **Isolation**: Each test should run independently. Use `beforeEach` for setup.
- **Naming**: `test('should [expected behavior] when [condition]')`.

## Selectors & Interaction
- **Priority**:
  1. User-facing roles: `getByRole('button', { name: 'Save' })`
  2. Text content: `getByText('Success')`
  3. Test IDs (last resort): `getByTestId('submit-btn')`
- **Waiting**: Rely on auto-waiting. Avoid `page.waitForTimeout()`.
- **Assertions**: Use web-first assertions: `await expect(locator).toBeVisible()`.

## Fixtures & Setup
- **Auth**: Use the global auth setup or `test.use({ storageState: ... })`.
- **Database**: Do NOT rely on pre-seeded data if possible. Create test data within the test (or via API helpers).
- **Clean Up**: Ensure tests clean up their data (though database resets are preferred in CI).

## Flakiness Prevention
- **Network**: Wait for network idle or specific responses if UI updates depend on them.
- **Animations**: Disable animations in test config if they cause timing issues.

