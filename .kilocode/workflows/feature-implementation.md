# Feature Implementation Workflow

This workflow defines the standard order of operations for implementing a new feature, with validation checkpoints. The Orchestrator delegates to each mode in sequence.

## Delegation Sequence

1. **Delegate to user-story-writer** → Create structured user stories
2. **Delegate to spec-writer** → Create technical specifications
3. **Delegate to frontend-architect** → Define frontend architecture (parallel)
4. **Begin TDD Red-Green-Refactor Cycle** (repeat for each test scenario):
   - **Delegate to tdd-test-first** → Write ONE failing unit test
   - **Delegate to domain-modeler** → Implement logic to make that test pass
   - **Delegate to code-refactorer** (optional) → Improve code/test quality while keeping tests green
5. **After all domain tests pass, delegate to implementation-validator** ← VALIDATE DOMAIN
6. **Delegate to ef-migrations** → Create persistence layer
7. **Delegate to implementation-validator** ← VALIDATE PERSISTENCE
8. **Delegate to backend-api-developer** → Implement API endpoints
9. **Delegate to implementation-validator** ← VALIDATE API
10. **Delegate to integration-test-writer** → Write integration tests
11. **Delegate to design-system-engineer** → Build UI components (if needed)
12. **Delegate to implementation-validator** ← VALIDATE COMPONENTS
13. **Delegate to product-developer** → Assemble feature
14. **Delegate to implementation-validator** ← VALIDATE FEATURE
15. **Delegate to test-automation-engineer** → Write E2E tests
16. **Delegate to implementation-validator** ← FINAL VALIDATION

## Validation Principle

**The Orchestrator MUST delegate to implementation-validator after EVERY implementation step** to verify spec compliance before delegating to the next implementation mode. This ensures early detection of gaps and reduces rework.

## Key Principles

1. **Documentation First**: User stories → Technical specs before any code
2. **Incremental TDD**: Write ONE test → Implement → Refactor → Repeat (tight Red-Green-Refactor cycles)
3. **Continuous Validation**: Validate after each implementation step, not just at the end
4. **Layer Separation**: Domain → Persistence → API (respecting Clean Architecture)
5. **Frontend Parallel Track**: Architecture → Components → Assembly
6. **Testing Pyramid**: Unit tests → Integration tests → E2E tests
7. **Early Detection**: Catch specification gaps as soon as minimal implementation exists

## TDD Red-Green-Refactor Cycle

The orchestrator coordinates multiple Red-Green-Refactor cycles during domain implementation:

1. **Red**: tdd-test-first writes ONE failing test for a specific behavior
2. **Green**: domain-modeler implements minimal code to make that test pass
3. **Refactor** (optional): code-refactorer improves code and test quality while keeping all tests green
   - ONLY refactors when ALL tests are passing
   - Runs tests after refactoring to ensure no regressions
   - Refactors BOTH production code AND test code
4. **Repeat**: Return to step 1 for the next test scenario

This continues until all domain behaviors have tests and implementations. After all domain tests pass, the orchestrator delegates to implementation-validator to verify the complete domain implementation matches the specification before proceeding to the persistence layer.

### Refactoring Guidelines

The code-refactorer mode should be invoked when:
- Code duplication is detected
- Methods are too long or complex
- Naming can be improved
- Test code is unclear or repetitive
- Design patterns could simplify the code

Refactoring is MANDATORY when:
- All tests are passing (green state) **AND** code quality issues are identified

Refactoring is FORBIDDEN when:
- Any tests are failing (red state)
- Tests would need to change behavior (not just structure)

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
