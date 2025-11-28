# Atomic Design with React

This React + Atomic Design approach is built around three ideas:

* Visual styling lives as low as possible in the component tree.
* Application layers mostly compose, not restyle.
* Theme (colors, typography, radius, shadows) is cleanly separated from layout (flex, grid, spacing).

On top of that:

* **Light mode is the default.** Every visual style must define a light-mode baseline and then explicitly declare what changes in dark mode.

The result is a system where pages are assembled from pre-styled building blocks, global changes happen either in the theme or in layout primitives, and color schemes behave predictably.

---

## Atomic Design in a React App

In this model:

* Atoms are the smallest reusable UI elements (buttons, inputs, headings, icons).
* Molecules combine atoms (input + label + error message, icon + label, etc.).
* Organisms are larger sections (navbars, cards, forms, sidebars).
* Templates and pages arrange organisms into real screens.

As you move up this hierarchy, you write less CSS and rely more on composition, props, and variants.

---

## Theme vs Layout: Hard Separation

Define theme and layout as two separate layers in your design system.

**Theme** is about visual identity:

* Color palette
* Typography
* Border radii and shadows
* State styles (hover, focus, disabled)
* Light vs dark mode tokens and mappings

Express this with design tokens, a theme object, or CSS variables. Atoms read from this layer but don’t reinvent it.

**Layout** is about structure:

* Flexbox and grid
* Gaps, padding, and alignment
* Breakpoints and responsive rules

Keep layout concerns in layout primitives (`Stack`, `Row`, `Column`, `Grid`, `Container`, etc.) and composition in higher-level components, not inside every atom.

Theme should never directly describe layout, and layout should never hardcode theme values. A `Stack` should care about gap sizes (“sm”, “md”, “lg”), not exact colors or fonts.

---

## Dark and Light Mode: Explicit, Not Magical

Take a strong stance:

1. **Light mode is the canonical definition.**
   Every visual style is specified first for light mode. This is the base truth of the design system.

2. **Dark mode must be explicit.**
   Dark mode is not “auto-invert” or a filter. For every token or variant that affects appearance, you should be able to answer:

   * “What does this look like in light mode?”
   * “What does this look like in dark mode?”

3. **Theme owns the modes, atoms consume them.**

   * Theme exposes `mode: 'light' | 'dark'` and mode-specific token values (e.g., `colors.surface`, `colors.surfaceInverted`).
   * Atoms never guess or invert colors on their own; they just read the active tokens.
   * Dark mode overrides live in the theme and/or in component styles that are explicitly keyed off `mode`.

4. **No style without a mode plan.**
   When you introduce a new visual style (button variant, card type, badge color), you must define:

   * Light-mode appearance
   * Its dark-mode override (even if it’s “same color but brighter/darker”)

5. **Application layer doesn’t define modes.**
   Pages and templates toggle `mode` (through context, provider, or CSS class) but don’t assign raw colors. They switch modes; atoms and theme decide what that looks like.

This keeps dark mode from becoming a patchwork of ad-hoc overrides and ensures both modes evolve together.

---

## Atoms: Styling Lives Here

Atoms are where most component-level CSS belongs.

Atoms:

* Bind theme tokens to actual CSS: `color`, `background`, `border`, `shadow`, typography, spacing internal to the component.
* React to theme mode via tokens, not via hard-coded conditionals like `mode === 'dark' ? '#111' : '#fff'` sprinkled everywhere.
* Stay layout-agnostic: no “fixing layout” with random margins.

For dark/light:

* Each atom has a single, clear definition in light mode.
* Dark mode behavior arises from tokens (`colors.textPrimary`, `colors.buttonPrimaryBg`, etc.) that have distinct light/dark values.
* If an atom needs a special dark-mode tweak (e.g., subtle border to separate dark surfaces), that tweak is still expressed in terms of tokens or a clearly labeled dark-mode style block.

---

## Molecules: Compose Atoms, Don’t Redefine Theme

Molecules combine atoms into patterns like `TextField`, `UserChip`, `TagWithClose`.

They should:

* Prefer atom variants instead of redefining colors or typography.
* Use minimal layout (local flex/grid with gaps) to arrange children.
* Avoid introducing new colors or font sizes; they should come from atoms or theme.

For modes:

* Molecules normally don’t care whether it’s light or dark; their children already react to tokens.
* If a molecule introduces its own surface (e.g. a composite card), that surface must define both light and dark behavior explicitly, again via tokens.

---

## Organisms: Section Layout, Not New Themes

Organisms define larger structural chunks: headers, sidebars, dashboards, forms.

They:

* Arrange molecules and atoms using layout primitives and responsive rules.
* Choose variants (e.g., “primary” vs “subtle” nav items) but don’t invent new colors or font stacks.
* Avoid raw CSS for theme values; they depend on theme and lower-level components.

For modes:

* Organisms receive the active mode via context or class and just render; their appearance is the emergent result of atom/molecule styling.
* If they must differentiate surfaces (e.g., header is darker than page background), they do so through well-defined tokens that provide both light and dark values.

---

## Templates and Pages: Composition, Not CSS

Templates and pages should be almost CSS-free.

Their responsibilities:

* Arrange organisms and layout primitives into a screen.
* Wire them to data, routing, loading/error states.
* Toggle light/dark mode (e.g., by setting a theme context value or a `data-theme` attribute).

What they **don’t** do:

* Introduce new colors, font sizes, or ad-hoc dark-mode overrides.
* Use inline styles to “fix things visually” that belong inside atoms or layout primitives.

If a page needs a unique look, that’s usually a sign to define a reusable component or layout and give it proper light/dark design, not to sprinkle page-local styles.

---

## Technical Implementation Tips

A few practical patterns:

* Theme provider exposes both tokens and `mode`. Components read from tokens only.
* Layout primitives (`Stack`, `Grid`, `Cluster`, `Container`) encapsulate flex/grid rules and responsive behavior.
* Use your styling tool (CSS Modules, styled-components, Emotion, Tailwind, etc.) with the same philosophy:

  * Low-level components own styles.
  * Higher-level components primarily compose.

For dark mode:

* Use a single source of truth for mode (`ThemeProvider`, `data-theme`, or root class).
* Define paired tokens like `surface / surfaceInverted`, `text / textMuted`, `border / borderSubtle` with different mappings in light and dark.
* Avoid runtime color math in components; keep “what color” decisions inside theme/token configuration.

Code review rule of thumb:

* If a page or template contains a raw hex color or font size, question it.
* If a new component variant ships without thinking through dark mode, send it back.

---

## Accessibility and Responsiveness

Because styling is concentrated at the bottom:

* Contrast, focus states, and readable typography are enforced in atoms and theme for both light and dark.
* Layout primitives handle how the UI rearranges on different screen sizes.

Dark mode often needs extra care for contrast and focus indicators; fixing those at the atom/theme level ensures consistency across the app.

---

## Summary

Think of the system like this:

* **Theme** defines the design language once, in both light and dark. Light is the baseline; dark is an explicit override.
* **Atoms** bind that language to concrete UI elements and include well-defined light and dark visuals.
* **Molecules and organisms** arrange those elements and pick variants, but don’t reinvent styling.
* **Templates and pages** wire everything together, toggle modes, and avoid custom CSS.

Keep visual identity and color schemes close to atoms and theme, keep layout in dedicated layout components, and let the application layer focus on behavior and composition.
