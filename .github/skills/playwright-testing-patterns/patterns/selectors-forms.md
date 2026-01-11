# Selectors and Forms

Use when choosing locators and interacting with forms.

## Selector Priority
1) Roles + accessible names; 2) Visible text; 3) Placeholders; 4) Test IDs as last resort; avoid CSS nth-child/dynamic IDs.

## Form Interaction
- Use labels/roles for inputs and options.
- Cover success and validation-error flows.

```typescript
await page.getByRole('button', { name: 'Create Venue' })
await page.getByRole('textbox', { name: 'Venue Name' })
await page.getByText('Venue created successfully')
```

```typescript
test('shows validation errors on empty submit', async ({ page }) => {
  await page.goto('/venues/new')
  await page.getByRole('button', { name: 'Create Venue' }).click()
  await expect(page.getByText('Venue name is required')).toBeVisible()
})
```
