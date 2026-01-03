# Approval Workflow

## Request User Approval Before Proceeding

After creating the technical specification file, **ALWAYS pause and ask the user to review and approve it** before any implementation work begins.

### Approval Process

1. **Create the Specification**
   - Write the complete technical specification in `docs/specs/`
   - Ensure all sections are complete and follow the document structure guidelines
   - Set the initial status to `Draft` in the metadata

2. **Request Review and Approval Using `ask_followup_question` Tool**
   - **Use the `ask_followup_question` tool** to request approval (DO NOT use `attempt_completion`)
   - Question format: `I've created the technical specification: [Feature Name](docs/specs/feature-name.md). Please review it and let me know if you approve it or if any changes are needed.`
   - Provide follow-up suggestions:
     - "Approved - proceed to implementation"
     - "Request changes: [describe changes needed]"
     - "Approved - begin TDD Test First mode"
     - "Approved - create database migrations"

3. **Wait for User Response**
   - **DO NOT** automatically proceed to implementation
   - **DO NOT** switch to implementation modes (TDD Test First, Domain Modeler, etc.)
   - **DO NOT** create skeleton code or test files
   - **DO NOT** use `attempt_completion` until the user has explicitly approved the specification
   - Wait for explicit user approval or feedback

4. **Handle Feedback**
   - If the user requests changes, update the specification accordingly
   - After making changes, use `ask_followup_question` again to request approval with an updated link
   - Iterate until the user explicitly approves the specification

5. **Update Status After Approval**
   - Once approved, update the specification's status from `Draft` to `Approved`
   - Use `attempt_completion` to confirm the specification is approved and ready for implementation
   - Suggest next steps in the completion message (e.g., "The specification is now approved and ready for implementation. You can delegate to TDD Test First mode to begin development.")

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
