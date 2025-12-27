# User Story: Edit Act

**As a** GloboTicket event organizer
**I want to** update existing act information
**So that** I can keep performer details accurate and current as circumstances change

## Description

This user story describes the capability for authorized users to modify existing act records in the GloboTicket multi-tenant ticketing platform. Users can update the act name while maintaining data integrity and tenant isolation.

The edit functionality provides a pre-populated form with the act's current name, allowing users to make changes and save updates. The system automatically tracks when acts are modified by recording update timestamps.

**Business Value:**
- Maintains accuracy of performer information as details change over time
- Enables correction of typos or naming errors without recreating act records
- Preserves act history and relationships (e.g., existing shows remain linked to the act)
- Maintains audit trail through automatic update timestamp tracking
- Supports professional name changes or rebranding of performers

## Scenario

### Main Success Scenario

**Given** I am an authenticated user with access to the GloboTicket system
**And** an act exists within my tenant's data scope
**When** I navigate to the act detail page and click the "Edit" button
**Then** I am directed to the edit act page
**And** the act form is pre-populated with the current act name
**When** I modify the name field with a valid value and submit the form
**Then** the act record is updated
**And** the system records when the update occurred
**And** I am redirected to the acts list page
**And** the updated act information is visible in the list

### Alternative Scenario: Correcting a Typo

**Given** I notice a misspelled performer name in my acts list
**When** I navigate to edit that act
**And** I correct the spelling in the name field
**And** I submit the form
**Then** the act name is updated throughout the system
**And** all associated shows display the corrected name
**And** I am returned to the acts list showing the corrected name

### Alternative Scenario: Act Not Found

**Given** I attempt to access the edit page for an act
**When** the act does not exist in my tenant's data scope
**Then** the system displays a "Act not found" error message
**And** no form is displayed
**And** I can navigate back to the acts list

### Error Scenario: Validation Errors

**Given** I am editing an act
**When** I enter invalid data (e.g., empty name, name exceeding maximum length)
**And** I attempt to submit the form
**Then** the system displays specific error messages for each validation failure
**And** the form is not submitted
**And** I can correct the errors and resubmit

### Error Scenario: Missing Required Information

**Given** I am editing an act
**When** I clear the act name field
**And** I attempt to submit the form
**Then** the system displays an error message "Act name is required"
**And** the form is not submitted
**And** I remain on the edit page to correct the error

## Acceptance Criteria

### Form Display and Navigation

- [ ] Act detail page displays an "Edit" button that navigates to the edit page
- [ ] Edit page displays "Edit Act" header
- [ ] Form is pre-populated with the current act name
- [ ] While loading act data, page displays a loading indicator
- [ ] If act cannot be found or loaded, page displays clear error message
- [ ] Form includes "Update Act" button to save changes
- [ ] Form includes "Cancel" button to abandon changes and return to acts list

### Editable Fields

- [ ] User can modify the act name (required, max 100 characters)
- [ ] Input field displays the current name when the page loads
- [ ] Input field allows typing and editing the full 100 characters
- [ ] Input field shows placeholder text when empty (for context)

### Input Validation

- [ ] Act name is a required field
- [ ] Act name cannot be empty or contain only whitespace
- [ ] Act name has a maximum length of 100 characters
- [ ] Leading and trailing spaces are automatically removed before saving
- [ ] Validation occurs when user attempts to submit the form
- [ ] Form displays clear, specific error messages for validation failures
- [ ] Form prevents submission when validation fails
- [ ] Error messages clear when user corrects the invalid input

### User Experience

- [ ] Page header reads "Edit Act"
- [ ] Act name field label reads "Act Name *" (asterisk indicates required)
- [ ] Update button is labeled "Update Act"
- [ ] Cancel button is labeled "Cancel"
- [ ] Submit button is disabled while the act is being updated
- [ ] Submit button shows a loading indicator during update
- [ ] Input field is disabled while the act is being updated
- [ ] Form maintains visual consistency with other forms in the application

### Save Behavior

- [ ] Submitting the form updates the act record
- [ ] System records timestamp of when act was updated
- [ ] During save operation, submit button is disabled to prevent double-submission
- [ ] During save operation, form displays loading indicator
- [ ] Successful save redirects user to the acts list page
- [ ] Updated act appears in the list with new values immediately visible
- [ ] If save fails, error message is displayed and user remains on edit page

### Navigation and Workflow

- [ ] User accesses edit act page from act detail page via edit button
- [ ] Clicking "Cancel" returns user to acts list without saving changes
- [ ] Successful update redirects to acts list (URL: /acts)
- [ ] Failed update keeps user on edit page to retry
- [ ] User can navigate away from edit page using browser back button
- [ ] Navigating away without saving does not modify the act

### Error Handling

- [ ] Missing act name displays error: "Act name is required"
- [ ] Whitespace-only name displays error: "Act name is required"
- [ ] Name exceeding 100 characters displays error: "Act name must be 100 characters or less"
- [ ] Errors display in a prominent error message container
- [ ] Error message container has red/error styling to draw attention
- [ ] Error messages appear above the form fields
- [ ] Network/system errors display message: "Failed to update act"
- [ ] Act not found displays message: "Act with GUID [guid] not found"
- [ ] After error, user can correct input and resubmit without refreshing page

### Data Integrity

- [ ] Act's unique identifier remains unchanged
- [ ] Act's original creation timestamp remains unchanged
- [ ] Act remains associated with the same tenant
- [ ] Only the name field can be modified through this feature
- [ ] Update timestamp is automatically set when changes are saved

### Data Isolation

- [ ] Only acts within my tenant can be edited
- [ ] Attempting to edit an act from another tenant results in "not found" error
- [ ] Users only see acts that belong to their organization
- [ ] Updated act remains associated with the user's organization
- [ ] Acts list shows only acts within the user's organization after update

### Access Control

- [ ] Only authenticated users can access the edit act page
- [ ] Unauthenticated users are redirected to login page
- [ ] All users within an organization can edit acts belonging to their organization
- [ ] Acts can only be edited within the user's organizational context

## Prerequisites

This user story depends on the following features being implemented:

- [Add Act](./add-act.md) - Acts must exist in the system before they can be edited
- User Authentication - Users must be logged in to edit acts
- Tenant Resolution - System must identify which tenant the user belongs to in order to restrict access to that tenant's acts
