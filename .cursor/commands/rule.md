# How to Generate a Cursor Rule

Cursor rules are markdown files (`.mdc` format) that provide context-specific guidance to the AI assistant. They help maintain consistency, enforce patterns, and document best practices for your codebase.

## File Format

Cursor rules use the standard `.mdc` (Markdown with Cursor) format, which includes:

1. **Frontmatter** (YAML) - Describes when and how the rule should be applied
2. **Markdown content** - The actual rule documentation

### Frontmatter Structure

Every rule file must start with YAML frontmatter that includes:

```yaml
---
description: Brief description of what this rule covers
globs:
  - "**/*.cs"
  - "**/Entities/*.cs"
alwaysApply: false
---
```

**Frontmatter Fields:**

- `description` (required): A concise description of the rule's purpose and scope
- `globs` (optional): Array of file patterns where this rule should be applied. Use glob patterns like `"**/*.cs"` for all C# files, or `"**/Entities/*.cs"` for entity files in any Entities directory
- `alwaysApply` (optional, default: `false`): If `true`, the rule is always available regardless of file context. Use sparingly for general coding standards

**Example:**
```yaml
---
description: Patterns for entity properties including dual keys, initialization, and data types
globs:
  - "**/Entities/*.cs"
  - "**/Domain/Entities/*.cs"
alwaysApply: false
---
```

## Best Practices

### 1. Keep Rules Under 500 Lines

- Large rules are harder to maintain and understand
- The AI assistant works better with focused, scoped guidance
- If a rule exceeds 500 lines, split it into multiple composable rules

### 2. Split Large Rules into Multiple, Composable Rules

Instead of one monolithic rule, create focused rules that work together:

**Bad:** One 1000-line rule covering "Entity Framework Everything"

**Good:** Separate rules for:
- `entity-properties.mdc` - Property patterns
- `entity-configuration.mdc` - Fluent API configuration
- `relationships.mdc` - Navigation properties and foreign keys
- `multi-tenancy.mdc` - Tenant isolation patterns

### 3. Provide Concrete Examples or Referenced Files

Rules should include:
- **Code examples** showing the correct pattern
- **References to existing files** in the codebase that demonstrate the pattern
- **Anti-patterns** showing what NOT to do (when helpful)

### 4. Avoid Vague Guidance - Write Rules Like Clear Internal Docs

Rules should be:
- **Specific**: "Use `Guid` for public identifiers" not "Use appropriate types"
- **Actionable**: "Always call `IsRequired()` for GUID properties" not "Consider required properties"
- **Contextual**: Explain WHY, not just WHAT

### 5. Keep Rules Focused, Actionable, and Scoped

Each rule should:
- **Focus on one domain** (e.g., entity properties, API patterns, testing)
- **Provide actionable guidance** (specific patterns to follow)
- **Have clear scope** (what files/contexts it applies to)

## Rule File Organization

Store rules in `.cursor/rules/` directory:

```
.cursor/
  rules/
    entity-properties.mdc
    entity-configuration.mdc
    relationships.mdc
    multi-tenancy.mdc
    ...
```
