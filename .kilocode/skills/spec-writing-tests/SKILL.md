---
name: spec-writing-tests
description: Defines test scenarios using Given-When-Then format and maps tests to acceptance criteria. Use when writing technical specifications, defining unit and integration test requirements, or ensuring test coverage for acceptance criteria.
---

# Test Scenarios

Define test scenarios that validate the technical implementation. Use Given-When-Then format and map tests to acceptance criteria:

```markdown
## Testing Requirements

### Unit Test Scenarios

#### Domain Layer
- [ ] GivenNewEntity_WhenCreated_ThenPropertiesInitialized
- [ ] GivenEntity_WhenChecked_ThenInheritsFromCorrectBaseClass
- [List domain entity tests following project patterns]

#### Application Layer (DTO Validation)
- [ ] GivenDto_WhenRequiredFieldMissing_ThenValidationFails
- [ ] GivenDto_WhenFieldExceedsMaxLength_ThenValidationFails
- [List DTO validation tests]

### Integration Test Scenarios

#### Service Layer
- [ ] CreateEntity_WithValidData_CreatesInDatabase (validates AC: X)
- [ ] CreateEntity_SetsCorrectTenantId (validates AC: Y)
- [Map each test to relevant acceptance criteria]

#### API Endpoints
- [ ] PostEntity_WithValidData_Returns201Created (validates AC: Z)
- [ ] PostEntity_WithoutAuthentication_Returns401Unauthorized
- [Include tests for all HTTP status codes in OpenAPI spec]

#### Multi-Tenancy
- [ ] CreateEntity_InTenantA_NotVisibleToTenantB
- [ ] GetAllEntities_ReturnsOnlyCurrentTenantEntities
```

**Test Naming Convention**: Use `Given{State}_When{Action}_Then{Result}` format consistently throughout.

**Coverage Requirement**: Each acceptance criterion should be validated by at least one test scenario.

## Quality Standards

**Your specifications must:**

1. **Be Complete**: Address every acceptance criterion through technical implementation details
2. **Avoid Redundancy**: Never duplicate the user story or acceptance criteria; reference them instead
3. **Consolidate Information**: Define concepts once, reference them elsewhere (e.g., validation rules in one place)
4. **Be Precise**: Include exact property names, data types, constraints
5. **Follow Conventions**: Match existing codebase patterns (CLAUDE.md, .cursor/rules/)
6. **Be Implementation-Ready**: Developer can build directly from spec
7. **Consider Edge Cases**: Handle errors, validation, null values
8. **Respect Architecture**: Maintain Clean Architecture layer separation
9. **Ensure Multi-Tenancy**: Properly implement tenant isolation
10. **Document Decisions**: Explain why choices were made
11. **Be Testable**: Include clear testing requirements that map to acceptance criteria
12. **Be Maintainable**: Consider long-term code health

## Internal Redundancy Prevention

**Avoid repeating the same information in multiple sections:**

- **Validation Rules**: Define once in OpenAPI schema or UI component section, reference elsewhere
- **Error Messages**: List in one centralized location
- **Navigation Flows**: Choose either acceptance-criteria format OR detailed flow diagrams, not both
- **Multi-Tenancy Details**: Technical implementation in one section, testing implications reference it
- **API Contracts**: Full OpenAPI spec is sufficient; don't restate in prose

**Cross-Reference Pattern**: "See [Validation Rules](#validation-rules) for field constraints" instead of repeating the rules.

## Output Format

Your specification should be:
- Written in clear, technical markdown
- Saved to `docs/specs/{feature-name}.md`
- Structured exactly as outlined above
- Complete enough for immediate implementation
- Reviewable by both technical and non-technical stakeholders

