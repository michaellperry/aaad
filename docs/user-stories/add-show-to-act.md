# User Story: Add Show to Act

## User Story

As an **event organizer**, I want to **add a show for an act at a specific venue** so that **I can schedule performances and make tickets available for sale**.

## Description

Event organizers need to schedule shows (performances) for their acts at various venues. When creating a show, they must specify the venue, start date/time, and total ticket count. The system provides contextual information about venue capacity and nearby shows to help organizers make informed scheduling decisions while respecting multi-tenant data isolation.

**Business Value:**
- Enables core event scheduling functionality
- Provides venue capacity guidance to prevent overselling
- Displays scheduling context to help avoid conflicts
- Maintains data isolation per tenant

## Scenarios

### Scenario 1: Successfully Adding a Show with No Nearby Shows

**Given** an event organizer is viewing an act's details page
**And** the act belongs to their tenant
**And** at least one venue exists for their tenant
**When** they navigate to add a new show
**And** they select a venue from the dropdown
**And** they enter a ticket count less than or equal to the venue's capacity
**And** they enter a future start date
**And** they enter a start time
**And** no other shows exist at that venue within 48 hours of the selected date/time
**And** they submit the form
**Then** the show is created successfully
**And** the show is associated with the selected act
**And** the show displays on the act's details page
**And** a success confirmation message is displayed

### Scenario 2: Adding a Show with Nearby Shows Displayed

**Given** an event organizer is adding a show
**And** they have selected a venue and entered a date/time
**When** other shows exist at the same venue within 48 hours before or after the entered date/time
**Then** the system displays a list of those nearby shows
**And** each nearby show displays its act name, start date, and start time
**And** the organizer can still proceed to create the show
**And** the show is created successfully when submitted

### Scenario 3: Attempting to Add a Show with Invalid Data

**Given** an event organizer is adding a show
**When** they enter a ticket count greater than the venue's capacity
**Or** they enter a start date in the past
**Or** they leave required fields empty
**And** they attempt to submit the form
**Then** the show is not created
**And** validation error messages are displayed inline next to the relevant fields
**And** the form retains the entered data for correction

### Scenario 4: Viewing Venue Capacity During Show Creation

**Given** an event organizer is adding a show
**When** they select a venue from the dropdown
**Then** the venue's capacity is displayed prominently
**And** this capacity information remains visible while entering ticket count
**And** the organizer can use this information to determine appropriate ticket allocation

## Acceptance Criteria

### Core Functionality

- [ ] Shows can be added from an act's details page
- [ ] Each show is associated with exactly one act (the act from whose page it was created)
- [ ] Each show is associated with exactly one venue (selected during creation)
- [ ] Shows are automatically associated with the current tenant
- [ ] Successfully created shows appear on the act's details page
- [ ] The show creation form includes fields for: venue selection, ticket count, start date, and start time

### Venue Selection and Capacity Display

- [ ] The venue dropdown displays only venues belonging to the current tenant
- [ ] When a venue is selected, its capacity is displayed on the form
- [ ] The venue capacity display updates immediately when a different venue is selected
- [ ] The capacity display is clearly labeled and positioned near the ticket count input
- [ ] If no venues exist for the tenant, an appropriate message is shown and the form cannot be submitted

### Input Validation

- [ ] Venue selection is required
- [ ] Ticket count is required and must be a positive integer
- [ ] Ticket count must be less than or equal to the selected venue's capacity
- [ ] Start date is required and must be in the future (not today or past dates)
- [ ] Start time is required and must be in valid time format (HH:MM)
- [ ] All validation errors are displayed inline next to the relevant input fields
- [ ] Validation errors are specific and actionable (e.g., "Ticket count cannot exceed venue capacity of 5000")
- [ ] Form submission is prevented when validation errors exist

### Time Zone Handling

- [ ] Start time is interpreted as local to the event location
- [ ] Start time uses the time zone that would be current on the specified start date at the venue location
- [ ] The system correctly handles daylight saving time transitions based on the start date

### Nearby Shows Display

- [ ] When a venue and start date/time are entered, the system queries for other shows at that venue
- [ ] Shows starting within 48 hours before the entered start date/time are displayed
- [ ] Shows starting within 48 hours after the entered start date/time are displayed
- [ ] Each nearby show displays: act name, start date, and start time
- [ ] The nearby shows display updates when the venue, date, or time is changed
- [ ] If no nearby shows exist, a message indicates "No other shows scheduled at this venue within 48 hours"
- [ ] The presence of nearby shows does NOT prevent show creation (informational only)
- [ ] Nearby shows are sorted chronologically by start date/time

### User Experience

- [ ] The form provides clear labels for all input fields
- [ ] Required fields are visually indicated (e.g., with asterisk or "required" label)
- [ ] A "Cancel" action is available to return to the act details page without saving
- [ ] A "Save" or "Create Show" action submits the form
- [ ] Upon successful creation, a confirmation message is displayed
- [ ] Upon successful creation, the user is returned to the act details page showing the new show
- [ ] If submission fails due to server error, an error message is displayed and the form data is retained

### Security & Access Control

- [ ] Only authenticated users can access the add show functionality
- [ ] Users can only add shows for acts that belong to their tenant
- [ ] Users can only select venues that belong to their tenant
- [ ] Attempting to add a show for an act from a different tenant returns an authorization error
- [ ] The nearby shows display only includes shows from the current tenant

### Data Integrity

- [ ] Each show must reference a valid act ID
- [ ] Each show must reference a valid venue ID
- [ ] Each show must have a valid tenant ID matching the current user's tenant
- [ ] The ticket count is stored as an integer
- [ ] The start date and time are stored in a format that preserves the local time zone context
- [ ] CreatedAt timestamp is automatically set when the show is created
- [ ] Show creation is transactional (all-or-nothing)

### Error Handling

- [ ] If the selected venue no longer exists when submitting, a clear error message is displayed
- [ ] If the selected act no longer exists when submitting, a clear error message is displayed
- [ ] If a database error occurs, a user-friendly error message is displayed (not technical details)
- [ ] Network errors during submission provide appropriate user feedback with retry guidance
- [ ] If the venue capacity changes between loading the form and submission, validation uses the current capacity

## Prerequisites

This user story depends on the following functionality being completed:

- [Venue Management](./manage-venues.md) - Venues must exist before shows can be created
- [Act Management](./manage-acts.md) - Acts must exist before shows can be associated with them
- User Authentication - Users must be authenticated to create shows
- Tenant Resolution - The system must correctly identify the current tenant

## Technical Notes

**Multi-Tenant Considerations:**
- Shows are child entities that inherit tenant context through their relationship with Venue
- Shows do NOT store TenantId directly but are filtered via the Venue navigation property
- Query filters ensure shows are automatically scoped to the current tenant

**Entity Relationships:**
- Show → Act (many-to-one, required)
- Show → Venue (many-to-one, required)
- Show inherits tenant context from Venue

**Time Zone Implementation:**
- Start date/time should be stored in a way that preserves local context
- The 48-hour window for nearby shows must account for the local time zone
- Consider using DateTimeOffset or storing timezone identifier with the show
