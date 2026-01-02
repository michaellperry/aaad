# Feature Implementation Workflow

This workflow defines the standard order of operations for implementing a new feature, with validation checkpoints. The Orchestrator delegates to each mode in sequence.

## Delegation Sequence

1. **Delegate to user-story-writer** → Create structured user stories
2. **Delegate to spec-writer** → Create technical specifications
3. **Delegate to frontend-architect** → Define frontend architecture (parallel)
4. **Delegate to tdd-test-first** → Write failing unit tests
5. **Delegate to domain-modeler** → Implement domain logic
6. **Delegate to implementation-validator** ← VALIDATE DOMAIN
7. **Delegate to ef-migrations** → Create persistence layer
8. **Delegate to implementation-validator** ← VALIDATE PERSISTENCE
9. **Delegate to backend-api-developer** → Implement API endpoints
10. **Delegate to implementation-validator** ← VALIDATE API
11. **Delegate to integration-test-writer** → Write integration tests
12. **Delegate to design-system-engineer** → Build UI components (if needed)
13. **Delegate to implementation-validator** ← VALIDATE COMPONENTS
14. **Delegate to product-developer** → Assemble feature
15. **Delegate to implementation-validator** ← VALIDATE FEATURE
16. **Delegate to test-automation-engineer** → Write E2E tests
17. **Delegate to implementation-validator** ← FINAL VALIDATION

## Validation Principle

**The Orchestrator MUST delegate to implementation-validator after EVERY implementation step** to verify spec compliance before delegating to the next implementation mode. This ensures early detection of gaps and reduces rework.

## Key Principles

1. **Documentation First**: User stories → Technical specs before any code
2. **TDD Workflow**: Tests before implementation (Red → Green)
3. **Continuous Validation**: Validate after each major implementation phase
4. **Layer Separation**: Domain → Persistence → API (respecting Clean Architecture)
5. **Frontend Parallel Track**: Architecture → Components → Assembly
6. **Testing Pyramid**: Unit tests → Integration tests → E2E tests
7. **Early Detection**: Catch specification gaps as soon as minimal implementation exists

## Validation Checkpoints

### After Domain Implementation
- All entities match ER diagram
- Business rules are enforced
- Unit tests pass
- No dependencies on infrastructure concerns

### After Persistence Implementation
- Migrations match schema specification
- Multi-tenant isolation is enforced
- Indexes are created
- DbContext configurations are correct

### After API Implementation
- Endpoints match OpenAPI spec
- DTOs map correctly to domain entities
- Request/response validation works
- Integration tests pass

### After UI Component Implementation
- Components match design specification
- Accessibility requirements met
- Props and state match spec
- Component tests pass

### After Feature Assembly
- All acceptance criteria met
- User flows work as specified
- API integration works correctly
- Ready for E2E testing

### Final Validation
- Complete feature matches all specifications
- All tests pass (unit, integration, E2E)
- No gaps between requirements and implementation
- Documentation is complete

## Optional: Platform Engineer

**Delegate to platform-engineer** as needed throughout the workflow when:
- Infrastructure changes are required
- Docker configurations need updates
- CI/CD pipelines need modification
- Security middleware needs configuration
- Build scripts need changes

The Platform Engineer can be invoked at any point when infrastructure needs arise.
