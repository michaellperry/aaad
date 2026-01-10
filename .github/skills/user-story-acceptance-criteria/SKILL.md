---
name: user-story-acceptance-criteria
description: User story writing patterns with comprehensive acceptance criteria categories and quality standards. Use when creating user stories, defining acceptance criteria, or reviewing story completeness for multi-tenant applications.
---

# User Story Acceptance Criteria Patterns

Comprehensive patterns for writing user stories with well-defined acceptance criteria that ensure clarity, testability, and completeness.

## Acceptance Criteria Categories

### Core Functionality
**Main feature behavior and primary user workflows**

```markdown
**Example: Create Venue Story**

**Core Functionality:**
- User can create a new venue with name, address, capacity, and venue type
- System assigns unique ID and creation timestamp to new venue
- User is redirected to venue details page after successful creation
- New venue appears in venue list for the tenant
```

### Input Validation
**Field requirements, data formats, constraints, and business rules**

```markdown
**Input Validation:**
- Venue name is required (1-200 characters)
- Address is required (1-500 characters)
- Capacity must be a positive integer (1-100,000)
- Venue type must be selected from predefined list
- Postal code must match valid format for selected country
- Duplicate venue names within tenant are not allowed
```

### User Experience
**Navigation, feedback, error messages, and loading states**

```markdown
**User Experience:**
- Form fields show validation errors immediately on blur
- Submit button is disabled while form is invalid
- Loading spinner appears during venue creation
- Success toast shows "Venue created successfully" message
- Cancel button returns to venue list without saving
- Form remembers partially entered data if user navigates away
```

### Security & Access Control
**Authentication, authorization, and data isolation requirements**

```markdown
**Security & Access Control:**
- Only authenticated users can create venues
- User must have 'venue-manager' or 'admin' role
- Created venue is automatically assigned to user's tenant
- User cannot see or access venues from other tenants
- API endpoints validate tenant context in JWT token
```

### Data Integrity
**Uniqueness constraints, required data, timestamps, and relationships**

```markdown
**Data Integrity:**
- Venue ID is unique across all tenants
- Created venue must have valid tenant association
- CreatedAt and UpdatedAt timestamps are automatically set
- Venue type must exist and be active
- Address validation ensures complete location data
```

### Error Handling
**Error scenarios and user-facing error messages**

```markdown
**Error Handling:**
- Network errors show "Unable to save venue. Please try again."
- Duplicate name error shows "A venue with this name already exists"
- Invalid capacity shows "Capacity must be between 1 and 100,000"
- Server errors show generic "An error occurred. Please contact support."
- Validation errors are shown inline with specific field guidance
```

## User Story Template

### Standard Format
```markdown
**As a** [persona]
**I want** [functionality]
**So that** [business value]

**Acceptance Criteria:**

**Core Functionality:**
- [Primary behavior 1]
- [Primary behavior 2]
- [Expected workflow outcome]

**Input Validation:**
- [Field requirement 1]
- [Data format constraint 2]
- [Business rule validation 3]

**User Experience:**
- [Navigation behavior]
- [Feedback mechanism]
- [Error message display]

**Security & Access Control:**
- [Authentication requirement]
- [Authorization rule]
- [Data isolation rule]

**Data Integrity:**
- [Uniqueness constraint]
- [Required relationship]
- [Data consistency rule]

**Error Handling:**
- [Error scenario 1 → Expected message]
- [Error scenario 2 → Expected behavior]
- [Fallback behavior]
```

### Example: Complete User Story
```markdown
**As a** venue manager
**I want** to edit existing venue details
**So that** I can keep venue information accurate and up-to-date

**Acceptance Criteria:**

**Core Functionality:**
- User can modify venue name, address, capacity, and venue type
- Changes are saved when user clicks "Update Venue" button
- User sees updated information immediately after save
- Venue's UpdatedAt timestamp is automatically updated
- Other users see the updated venue information

**Input Validation:**
- Venue name is required (1-200 characters)
- Address is required (1-500 characters)
- Capacity must be positive integer (1-100,000)
- Cannot change venue name to match existing venue in tenant
- All validation rules from creation apply to updates

**User Experience:**
- Form pre-populates with current venue data
- "Save" button is disabled if no changes made
- "Cancel" button discards changes and returns to venue details
- Success message shows "Venue updated successfully"
- Loading state shown while saving changes
- Unsaved changes warning if user navigates away

**Security & Access Control:**
- Only authenticated users with 'venue-manager' or 'admin' role can edit
- User can only edit venues within their tenant
- Edit permission checked before displaying edit form
- API validates user permissions and tenant context

**Data Integrity:**
- UpdatedAt timestamp reflects actual save time
- Venue ID and CreatedAt fields cannot be modified
- Tenant association cannot be changed
- Venue type must exist and be active

**Error Handling:**
- "Venue not found" if venue deleted by another user
- "Permission denied" if user loses edit permissions
- "Another user modified this venue" for concurrent edits
- Network errors show retry option
- Field validation errors shown inline with guidance
```

## Quality Standards

### Clarity Requirements
**User stories must be understandable by all stakeholders**

```markdown
✅ **Clear Language:**
- Use business terminology, not technical jargon
- Avoid implementation details (no class names, database schemas)
- Focus on user value and business outcomes
- Write from user's perspective

❌ **Unclear Examples:**
- "Update the VenueEntity in the database"
- "Call the UpdateVenueAsync method"
- "Implement PUT /api/venues/{id} endpoint"
```

### Testability Standards
**Every acceptance criterion must be objectively verifiable**

```markdown
✅ **Testable Criteria:**
- "User sees success message after venue creation"
- "Form validation prevents submission with empty name field"
- "API returns 403 error for unauthorized venue access"

❌ **Non-testable Criteria:**
- "User has good experience creating venues"
- "Form is user-friendly"
- "System performs well"
```

### Completeness Checklist
```markdown
**Story Review Checklist:**
□ Core functionality clearly defined
□ All input validation rules specified
□ User experience patterns documented
□ Security requirements explicit
□ Data integrity constraints listed
□ Error scenarios covered
□ Success criteria measurable
□ Edge cases considered
□ Dependencies identified
□ Acceptance criteria atomic (one concept per bullet)
```

### Independence Guidelines
```markdown
**Story Independence:**
- Each story delivers standalone business value
- Story can be developed and deployed independently
- Minimal dependencies on other stories
- Clear definition of "done"

**Dependency Management:**
- Explicit prerequisites listed
- Shared components identified
- Integration points documented
- Testing coordination planned
```

## Multi-Tenant Considerations

### Tenant Isolation Requirements
```markdown
**Standard Tenant Security Criteria:**
- User can only access data within their tenant
- API validates tenant context from JWT claims
- Database queries automatically filter by tenant ID
- Cross-tenant data access is prevented
- Error messages don't leak information about other tenants
```

### Tenant-Specific Validation
```markdown
**Tenant Context Validation:**
- Resource uniqueness validated within tenant scope
- Business rules applied per tenant configuration
- User permissions evaluated within tenant context
- Audit logs include tenant information
- Data export/import respects tenant boundaries
```

## Story Estimation Patterns

### Complexity Indicators
```markdown
**Simple Story (1-2 points):**
- Single entity CRUD operation
- Minimal validation rules
- Standard UI patterns
- No external integrations

**Medium Story (3-5 points):**
- Multiple entity relationships
- Complex validation logic
- Custom UI components needed
- Basic third-party integration

**Complex Story (8+ points):**
- Cross-service coordination
- Advanced security requirements
- Performance optimization needed
- Major architectural changes
```

### Split Indicators
```markdown
**Story Should Be Split When:**
- Multiple acceptance criteria categories are extensive
- Development estimate exceeds sprint capacity
- Multiple personas involved
- Cross-team dependencies required
- Testing spans multiple environments
```

These patterns ensure user stories provide clear guidance for development, testing, and product validation while maintaining focus on user value and business outcomes.