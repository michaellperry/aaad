# Testing

Use when writing component tests and visual regression coverage.

## Component Tests
```tsx
import { render, screen, userEvent } from '@testing-library/react'
import { Button } from './Button'

describe('Button', () => {
  it('renders variant styles', () => {
    render(<Button variant="primary">Click me</Button>)
    expect(screen.getByRole('button', { name: /click me/i })).toHaveClass('bg-primary-600')
  })

  it('handles click and keyboard', async () => {
    const handleClick = vi.fn()
    render(<Button onClick={handleClick}>Click me</Button>)
    const button = screen.getByRole('button', { name: /click me/i })
    await userEvent.click(button)
    expect(handleClick).toHaveBeenCalledOnce()
    button.focus()
    await userEvent.keyboard('{Enter}')
    await userEvent.keyboard(' ')
    expect(handleClick).toHaveBeenCalledTimes(3)
  })
})
```

## Visual Regression (Storybook)
```tsx
import type { Meta, StoryObj } from '@storybook/react'
import { Button } from './Button'

const meta: Meta<typeof Button> = {
  title: 'Atoms/Button',
  component: Button,
  parameters: { layout: 'centered' },
  argTypes: { variant: { control: { type: 'select' }, options: ['primary', 'secondary', 'danger'] } }
}
export default meta

type Story = StoryObj<typeof meta>
export const AllVariants: Story = {
  render: () => (
    <div className="space-x-2">
      <Button variant="primary">Primary</Button>
      <Button variant="secondary">Secondary</Button>
      <Button variant="danger">Danger</Button>
    </div>
  )
}
```
