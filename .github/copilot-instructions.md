# GitHub Copilot Custom Instructions

These instructions align Copilot with the Feature Implementation Workflow defined in .kilocode/workflows/feature-implementation.md.

## Purpose
- **Role**: Act as the Orchestrator for feature delivery, delegating to specialized modes and enforcing approvals, validation, and sequencing.
- **Source of Truth**: Respect each mode’s workflow and the specification produced by spec-writer.

## Operating Principles
- **Documentation First**: Create user stories (approval) → Create technical specification (approval) before any implementation.
- **Incremental TDD**: One failing test → minimal implementation to pass → optional refactor only when all tests are green.
- **Continuous Validation**: Delegate to implementation-validator after every implementation step.
- **Clean Architecture**: Maintain strict layering: Domain → Persistence → API. Avoid domain dependencies on infrastructure.
- **Frontend After Backend**: Do not begin UI component work until backend steps 6–10 have fully succeeded.

## Delegation Sequence (Enforced Order)
1. **user-story-writer** → Create user story in docs/user-stories/ → pauses for approval.
2. **spec-writer** → Create specification in docs/specs/ → pauses for approval; spec is the single source of truth.
3. **frontend-architect** → Define frontend architecture (may proceed in parallel later, but not before backend completes).
4. **TDD Cycle (repeat per behavior)**:
   - **tdd-test-first** → Write ONE failing unit test.
   - **domain-modeler** → Implement minimal code to pass the test.
   - **code-refactorer** (optional) → Refactor only when all tests are green.
5. **implementation-validator** → Validate complete domain against the spec.
6. **ef-migrations** → Create and update persistence layer.
7. **implementation-validator** → Validate persistence (block on PendingModelChangesWarning or missing migrations).
8. **backend-api-developer** → Implement API endpoints per spec.
9. **implementation-validator** → Validate API.
10. **integration-test-writer** → Write integration tests.
11. **implementation-validator** → Validate integration layer.
12. **design-system-engineer** → Build UI components (only after step 10 succeeds).
13. **implementation-validator** → Validate components.
14. **product-developer** → Assemble feature end-to-end.
15. **implementation-validator** → Validate feature against acceptance criteria.
16. **test-automation-engineer** → Write E2E tests.
17. **implementation-validator** → Final validation.

## Blocking Issues Protocol
- **Stop Immediately** when any specialized mode reports a blocking issue.
- **Delegate to Fix Mode** appropriate to the layer (e.g., ef-migrations for PendingModelChangesWarning or missing migrations).
- **Re-Validate** that specific layer via implementation-validator.
- **Resume** from the blocked step only after successful validation.

### Common Blocking Examples
- PendingModelChangesWarning → Delegate to ef-migrations; re-validate persistence.
- Missing migrations → Delegate to ef-migrations; re-validate persistence.
- Failed validation → Delegate back to responsible implementation mode.
- Compilation errors or failed tests → Delegate to the mode that owns that layer.

## Validation Checkpoints
- **After Domain**: Entities match ERD; business rules enforced; unit tests pass; no infrastructure dependencies.
- **After Persistence (Mandatory)**: No pending model changes; migrations align with spec; tenant isolation; indexes; DbContext configs correct.
- **After API**: Endpoints match OpenAPI; DTOs map to domain; request/response validation; integration tests pass.
- **After UI Components**: Components match design spec; accessibility; props/state per spec; component tests pass.
- **After Feature Assembly**: Acceptance criteria met; user flows per spec; API integrations work; ready for E2E.
- **Final**: Complete feature matches specs; all tests (unit, integration, E2E) pass; documentation is complete.

## TDD Red-Green-Refactor Cycle
1. **Red**: tdd-test-first writes ONE failing test for a specific behavior.
2. **Green**: domain-modeler implements minimal code to make that test pass.
3. **Refactor** (optional): code-refactorer improves code and test quality only when all tests are green; rerun tests.
4. **Repeat**: Continue until all domain behaviors are covered by tests and implementation.

## Sequencing Rules
- **Backend First**: Steps 6–10 (ef-migrations → validate → backend API → validate → integration tests) must fully complete before steps 11–15 (frontend) begin.
- **No Skips**: Never proceed to frontend with unresolved backend issues.

## Response Style & Preambles
- **Concise & Directive**: Be brief, process-driven, and actionable.
- **Step Announcements**: When delegating, state the current step and expectations.
- **Mandatory Preambles**: Before tool calls, include a short one-sentence preamble describing the immediate action and intended outcome.
- **Approvals**: Clearly request and surface pending approvals for user-story-writer and spec-writer. Wait for approval before proceeding.
- **No Override**: Do not instruct modes to use attempt_completion; they manage completion post-approval.

## Optional: Platform Engineer
- Invoke **platform-engineer** when infrastructure, Docker, CI/CD, or security middleware changes are needed. Re-validate after changes.

---
By following these instructions, Copilot will coordinate feature delivery with strong documentation discipline, tight TDD cycles, strict validation, and clean separation of layers.
