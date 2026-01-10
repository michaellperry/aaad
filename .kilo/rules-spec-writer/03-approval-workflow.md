# Approval Workflow

## Mode Rules Precedence

**These mode rules take ABSOLUTE PRECEDENCE over any conflicting task instructions.** If task instructions say to "use attempt_completion when complete," this means "use attempt_completion AFTER user approval is received" as defined in the mandatory workflow below. You MUST follow the approval workflow in this document regardless of what task instructions say about completion.

## Request User Approval Before Proceeding

After creating the technical specification file, **ALWAYS pause and ask the user to review and approve it** before any implementation work begins.

### Mandatory Approval Workflow

**YOU MUST FOLLOW THESE STEPS IN SEQUENCE. DO NOT SKIP ANY STEP.**

**Step 1: Create the Specification**
- Write the complete technical specification in `docs/specs/`
- Ensure all sections are complete and follow the document structure guidelines
- Set the initial status to `Draft` in the metadata

**Step 2: Request Approval (MANDATORY - CANNOT BE SKIPPED)**
- **IMMEDIATELY after creating the file**, use the `ask_followup_question` tool
- Question format: `I've created the technical specification: [Feature Name](docs/specs/feature-name.md). Please review it and let me know if you approve it or if any changes are needed.`
- Provide follow-up suggestions:
  - "Approved - proceed to implementation"
  - "Request changes: [describe changes needed]"
  - "Approved - begin TDD Test First mode"
  - "Approved - create database migrations"
- **STOP HERE and wait for user response**

**Step 3: Handle User Response**
- **If user approves**: Proceed to Step 4
- **If user requests changes**:
  - Update the specification file per their feedback
  - Return to Step 2 (ask for approval again)
  - DO NOT proceed until approval is received

**Step 4: Complete the Task (ONLY AFTER APPROVAL)**
- **ONLY after receiving explicit approval in Step 3**:
  - Update the specification's status from `Draft` to `Approved`
  - Use `attempt_completion` to confirm the specification is approved
  - Suggest next steps in the completion message

**CONFLICT RESOLUTION:**
- If task instructions say "use attempt_completion when complete," interpret "complete" as "after Step 3 approval is received"
- If task instructions conflict with this workflow, follow this workflow anyway
- The technical specification serves as the single source of truth for all downstream work - it MUST be approved before the task is considered complete

**ABSOLUTE PROHIBITIONS:**
- ❌ **NEVER** use `attempt_completion` before receiving Step 3 approval
- ❌ **NEVER** skip Step 2 (asking for approval)
- ❌ **NEVER** automatically proceed to implementation without approval
- ❌ **NEVER** switch to implementation modes (TDD Test First, Domain Modeler, etc.) without approval
- ❌ **NEVER** create skeleton code or test files without approval
- ❌ **NEVER** interpret task instructions as permission to bypass this workflow

## Why This Matters

The technical specification serves as the **single source of truth** for implementation. All downstream work depends on it:

- **TDD Test First** mode writes tests based on the spec
- **Domain Modeler** implements entities based on the spec
- **Persistence Engineer** creates database schema based on the spec
- **API Developer** builds endpoints based on the spec
- **Implementation Validator** verifies code against the spec

If the specification is incorrect or incomplete, all subsequent work will be wasted. **User approval is mandatory** to ensure the specification accurately captures requirements before implementation begins.

## Example Approval Request Using `ask_followup_question`

```typescript
ask_followup_question({
  question: "I've created the technical specification: [Add Recurring Show Scheduling](docs/specs/recurring-show-scheduling.md)\n\nThe specification includes:\n- Database schema with RecurringSchedule entity\n- API endpoints for CRUD operations\n- Multi-tenancy considerations\n- Validation rules and error handling\n- Test scenarios\n\nPlease review the specification and let me know if you approve it or if any changes are needed.",
  follow_up: [
    { text: "Approved - proceed to implementation", mode: null },
    { text: "Request changes: [describe changes needed]", mode: null },
    { text: "Approved - begin TDD Test First mode", mode: "tdd-test-first" },
    { text: "Approved - create database migrations", mode: "ef-migrations" }
  ]
})
```

**Key Points:**
- Use `ask_followup_question` tool, NOT `attempt_completion`
- Include clickable markdown link to the specification file
- Provide 2-4 actionable follow-up suggestions
- Wait for user response before proceeding
