# Validation Workflow

## Phase 1: Specification Parsing

1. Read the specification document thoroughly
2. Extract all testable requirements into structured checklist
3. Categorize by architectural layer (Domain → Application → Infrastructure → API)
4. Note expected file locations based on project conventions

## Phase 2: Implementation Search

1. Use Glob to find entity classes: `src/GloboTicket.Domain/Entities/**/*.cs`
2. Use Glob to find configurations: `src/GloboTicket.Infrastructure/Data/Configurations/**/*.cs`
3. Use Grep to find registrations: Search Program.cs for `AddScoped`, `MapEndpoints`
4. Use Grep to find query filters: Search DbContext for `HasQueryFilter`
5. Use LSP to verify method signatures match interfaces
6. Use Read to examine implementation logic for NotImplementedException

## Phase 3: Gap Analysis

1. For each requirement, compare spec vs. implementation
2. Mark ✅ if fully implemented with correct location and logic
3. Mark ❌ if missing, incomplete, or throws NotImplementedException
4. Record exact file path and line number for context
5. Note specific remediation action needed

## Phase 4: Report Generation

Structure your report as:

```markdown
## Specification Validation Report

**Specification**: [path/to/spec.md]
**Status**: [✅ Complete | ❌ Incomplete (X% complete)]
**Validated**: [timestamp]

### Executive Summary
[Brief overview of implementation status, major accomplishments, critical gaps]

### Domain Layer
- [✅/❌] [Component name] ([file:path:line])
  [Additional notes if ❌: what's missing/wrong]

### Application Layer
- [✅/❌] [Component name] ([file:path:line])

### Infrastructure Layer
- [✅/❌] [Component name] ([file:path:line])

### API Layer
- [✅/❌] [Component name] ([file:path:line])

### Database Layer
- [✅/❌] [Migration/Configuration] ([file:path:line])

### Critical Missing Items
1. [Exact action needed with code example]
2. [Next action with file location]
3. [Priority ordered by dependency]

### Implementation Coverage
- Total Requirements: X
- Implemented: Y (Z%)
- Missing/Incomplete: N

### Recommendations
[Prioritized next steps to achieve 100% spec compliance]
```
