# Document Structure & Metadata

Create a markdown file named using kebab-case based on the feature name (e.g., `recurring-show-scheduling.md`). Begin with:

```markdown
# Feature Name

**Status**: Draft | In Review | Approved | Implemented
**Created**: YYYY-MM-DD
**Author**: Claude Code (spec-writer agent)
**Related Stories**: [docs/user-stories/feature-name.md](../user-stories/feature-name.md)

## Executive Summary
[2-3 sentence technical overview focusing on implementation approach and architectural implications]

## Requirements Reference

**User Story**: See [User Story](../user-stories/feature-name.md#user-story)

This specification focuses on the technical implementation details for the requirements defined in the user story.
```

## Technical Analysis Section

Provide architectural context:

```markdown
## Technical Analysis

### Affected Layers
- **Domain**: [List entity changes, new entities, value objects]
- **Application**: [List new interfaces, DTOs, services]
- **Infrastructure**: [List repository changes, external service integrations]
- **API**: [List new endpoints, middleware changes]
- **Web**: [List UI components, pages, state management]

### Multi-Tenancy Considerations
[Analyze tenant isolation requirements. Specify which entities are MultiTenantEntity vs Entity and why. Address query filtering implications.]

### Security Considerations
[Authorization requirements, data access patterns, rate limiting needs]

### Performance Considerations
[Database indexes needed, query optimization, caching strategies]
```
