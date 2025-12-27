# User Story: Edit Venue

**As a** GloboTicket event organizer
**I want to** update existing venue information
**So that** I can keep venue details accurate and current as circumstances change

## Description

This user story describes the capability for authorized users to modify existing venue records in the GloboTicket multi-tenant ticketing platform. Users can update venue properties such as name, address, location coordinates, seating capacity, and descriptive information while maintaining data integrity and tenant isolation.

The edit functionality provides a pre-populated form with the venue's current values, allowing users to make changes and save updates. The system automatically tracks when venues are modified by recording update timestamps.

**Business Value:**
- Maintains accuracy of venue information as details change over time
- Supports operational flexibility for evolving venue configurations
- Enables correction of data entry errors without recreating venue records
- Preserves venue history and relationships (e.g., existing shows remain linked)
- Maintains audit trail through automatic update timestamp tracking

## Scenario

### Main Success Scenario

**Given** I am an authenticated user with access to the GloboTicket system
**And** a venue exists within my tenant's data scope
**When** I navigate to the venue detail page and click the "Edit" button
**Then** I am directed to the edit venue page
**And** the venue form is pre-populated with the current venue data
**When** I modify one or more fields with valid values and submit the form
**Then** the venue record is updated
**And** the system records when the update occurred
**And** I am redirected to the venues list page
**And** the updated venue information is visible in the list

### Alternative Scenario: Update Location Information

**Given** I am editing an existing venue that has no location data
**When** I provide latitude and longitude coordinates within valid ranges
**And** I optionally update the address field
**Then** the system stores the location information
**And** the venue becomes available for location-based features like mapping

### Alternative Scenario: Remove Optional Data

**Given** I am editing a venue that has address or location coordinates
**When** I clear the address, latitude, or longitude fields
**And** I submit the form
**Then** the system removes those optional values
**And** the venue remains valid with only required fields populated

### Alternative Scenario: Venue Not Found

**Given** I attempt to access the edit page for a venue
**When** the venue does not exist in my tenant's data scope
**Then** the system displays a "Venue not found" error message
**And** no form is displayed
**And** I can navigate back to the venues list

### Alternative Scenario: Validation Errors

**Given** I am editing a venue
**When** I enter invalid data (e.g., empty name, negative capacity, invalid coordinates)
**And** I attempt to submit the form
**Then** the system displays specific error messages for each validation failure
**And** the form is not submitted
**And** I can correct the errors and resubmit

## Acceptance Criteria

### Form Display and Navigation

- [ ] Venue detail page displays an "Edit" button that navigates to the edit page
- [ ] Edit page displays "Edit Venue" header
- [ ] Form is pre-populated with all current venue values:
  - Name
  - Address (if previously set)
  - Seating Capacity
  - Description
  - Latitude (if previously set)
  - Longitude (if previously set)
- [ ] While loading venue data, page displays a loading indicator
- [ ] If venue cannot be found or loaded, page displays clear error message
- [ ] Form includes "Update Venue" button to save changes
- [ ] Form includes "Cancel" button to abandon changes and return to venues list

### Editable Fields

- [ ] User can modify the venue name (required, max 200 characters)
- [ ] User can modify the address (optional, max 500 characters)
- [ ] User can modify the seating capacity (required, non-negative integer)
- [ ] User can modify the description (required, max 2000 characters)
- [ ] User can modify the latitude (optional, range -90 to 90)
- [ ] User can modify the longitude (optional, range -180 to 180)
- [ ] Form includes location picker to assist with finding addresses and coordinates

### Validation

- [ ] Name is required and cannot be empty whitespace
- [ ] Name must not exceed 200 characters
- [ ] Description is required and cannot be empty whitespace
- [ ] Description must not exceed 2000 characters
- [ ] Seating capacity is required and must be zero or positive
- [ ] Address must not exceed 500 characters when provided
- [ ] Latitude must be between -90 and 90 when provided
- [ ] Longitude must be between -180 and 180 when provided
- [ ] Form displays clear, specific error messages for validation failures
- [ ] Form prevents submission when validation fails
- [ ] Error messages clear when user corrects the invalid input

### Save Behavior

- [ ] Submitting the form updates the venue record
- [ ] System records timestamp of when venue was updated
- [ ] During save operation, submit button is disabled to prevent double-submission
- [ ] During save operation, form displays loading indicator
- [ ] Successful save redirects user to the venues list page
- [ ] Updated venue appears in the list with new values immediately visible
- [ ] If save fails, error message is displayed and user remains on edit page

### Data Integrity

- [ ] Venue's unique identifier remains unchanged
- [ ] Venue's original creation timestamp remains unchanged
- [ ] Venue remains associated with the same tenant
- [ ] Only venues within my tenant can be edited
- [ ] Attempting to edit a venue from another tenant results in "not found" error

### Geographic Data

- [ ] Providing latitude and longitude stores location data for the venue
- [ ] Clearing latitude and longitude removes location data
- [ ] Location data uses standard geographic coordinate system
- [ ] Updated location information is available for mapping features

## Prerequisites

This user story depends on the following features being implemented:

- [Add Venue](./add-venue.md) - Venues must exist in the system before they can be edited
- User Authentication - Users must be logged in to edit venues
- Tenant Resolution - System must identify which tenant the user belongs to in order to restrict access to that tenant's venues

