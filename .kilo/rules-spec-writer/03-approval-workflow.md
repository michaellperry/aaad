# Approval Workflow

## Request User Approval Before Proceeding

After creating the technical specification file, **ALWAYS pause and ask the user to review and approve it** before any implementation work begins.

### Approval Process

1. **Create the Specification**
   - Write the complete technical specification in `docs/specs/`
   - Ensure all sections are complete and follow the document structure guidelines
   - Set the initial status to `Draft` in the metadata

2. **Request Review and Approval**
   - Provide a clickable markdown link to the specification file in your message
   - Format: `Please review the technical specification: [Feature Name](docs/specs/feature-name.md)`
   - Explicitly ask: "Please review the specification and let me know if you approve it or if any changes are needed."

3. **Wait for User Response**
   - **DO NOT** automatically proceed to implementation
   - **DO NOT** switch to implementation modes (TDD Test First, Domain Modeler, etc.)
   - **DO NOT** create skeleton code or test files
   - Wait for explicit user approval or feedback

4. **Handle Feedback**
   - If the user requests changes, update the specification accordingly
   - After making changes, request approval again with an updated link
   - Iterate until the user explicitly approves the specification

5. **Update Status After Approval**
   - Once approved, update the specification's status from `Draft` to `Approved`
   - Only then suggest next steps (e.g., "The specification is now approved. Would you like me to switch to TDD Test First mode to begin implementation?")

## Why This Matters

The technical specification serves as the **single source of truth** for implementation. All downstream work depends on it:

- **TDD Test First** mode writes tests based on the spec
- **Domain Modeler** implements entities based on the spec
- **Persistence Engineer** creates database schema based on the spec
- **API Developer** builds endpoints based on the spec
- **Implementation Validator** verifies code against the spec

If the specification is incorrect or incomplete, all subsequent work will be wasted. **User approval is mandatory** to ensure the specification accurately captures requirements before implementation begins.

## Example Approval Message

```markdown
I've created the technical specification for [Feature Name].

Please review the technical specification: [Add Recurring Show Scheduling](docs/specs/recurring-show-scheduling.md)

The specification includes:
- Database schema with RecurringSchedule entity
- API endpoints for CRUD operations
- Multi-tenancy considerations
- Validation rules and error handling
- Test scenarios

Please review the specification and let me know if you approve it or if any changes are needed.
```
