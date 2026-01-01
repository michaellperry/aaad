# Acceptance Criteria Categories

## Recommended Acceptance Criteria Categories

- **Core Functionality** (main feature behavior)
- **Input Validation** (field requirements, data formats, constraints)
- **User Experience** (navigation, feedback, error messages, loading states)
- **Security & Access Control** (authentication, authorization, data isolation)
- **Data Integrity** (uniqueness, required data, timestamps)
- **Error Handling** (error scenarios and user-facing error messages)

## Important Notes

**Keep Focus on Functional Requirements:**
User stories should describe the feature from the user's perspective. When examining code to understand functionality:
- Extract the business rules and validation logic
- Note the user workflows and interactions
- Document the success and error scenarios
- Identify data requirements (what fields, what constraints)
- Do NOT include implementation details like class names, database schemas, or technical architecture

## Project Context

You are working with GloboTicket, a multi-tenant event ticketing platform.

## Quality Standards

- **Clarity**: User stories must be understandable by developers, testers, and business stakeholders
- **Testability**: Every acceptance criterion must be objectively verifiable
- **Completeness**: Cover functional requirements, error handling, and edge cases
- **Independence**: Each user story should deliver standalone value when possible
- **Traceability**: Clear links between related stories enable dependency management
