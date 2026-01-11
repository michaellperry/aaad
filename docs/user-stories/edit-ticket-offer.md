# User Story: Edit Ticket Offer

**As an** event organizer  
**I want to** update an existing ticket offer's name, price, or ticket count for a show  
**So that** I can adjust ticket pricing and inventory while ensuring total allocated tickets never exceed the show's capacity

## Description

Event organizers need the ability to modify ticket offers after they have been created to accommodate changing business needs. This includes adjusting prices, correcting errors, changing offer names, or reallocating ticket inventory across different offer types. When updating the ticket count, the system must validate that the total tickets allocated across all offers (including the updated offer) does not exceed the show's capacity.

The edit functionality provides a pre-populated form with the ticket offer's current values, allowing organizers to make changes and save updates. The system automatically tracks when offers are modified by recording update timestamps.

**Business Value:**
- Enables flexible pricing strategies that can adapt to market conditions
- Supports correction of data entry errors without recreating offers
- Allows reallocation of ticket inventory across different offer types
- Maintains data integrity by enforcing capacity constraints during updates
- Preserves offer history and relationships (e.g., existing sales remain linked)
- Maintains audit trail through automatic update timestamp tracking

## Scenarios

### Scenario 1: Successfully Updating Ticket Offer Price

**Given** an event organizer is viewing a show details page  
**And** the show has an existing ticket offer "General Admission" with price $50.00  
**And** the ticket offer belongs to their tenant  
**When** they navigate to edit the ticket offer  
**And** they change the price to "$45.00"  
**And** they submit the form  
**Then** the ticket offer is updated successfully  
**And** the updated price is displayed on the show page  
**And** a success confirmation message is displayed

### Scenario 2: Updating Ticket Count Within Available Capacity

**Given** a show with 1000 total tickets  
**And** an existing ticket offer "General Admission" has 600 tickets allocated  
**And** an existing ticket offer "VIP" has 200 tickets allocated  
**And** 200 tickets remain available  
**When** the organizer edits the "General Admission" offer  
**And** they reduce the ticket count to "400"  
**And** they submit the form  
**Then** the ticket offer is updated successfully  
**And** the available capacity increases to 400 tickets  
**And** the "VIP" offer remains unchanged at 200 tickets

### Scenario 3: Attempting to Exceed Capacity When Updating Ticket Count

**Given** a show with 1000 total tickets  
**And** an existing ticket offer "General Admission" has 600 tickets allocated  
**And** an existing ticket offer "VIP" has 200 tickets allocated  
**And** 200 tickets remain available (not including current offer's allocation)  
**When** the organizer edits the "General Admission" offer  
**And** they increase the ticket count to "900"  
**And** they submit the form  
**Then** the ticket offer is not updated  
**And** a validation error message is displayed: "Ticket count exceeds available capacity. Only 600 tickets available for this offer (including its current allocation)."  
**And** the form retains the entered data for correction

### Scenario 4: Updating Multiple Fields Simultaneously

**Given** an event organizer is editing a ticket offer  
**And** the current values are: name "Early Bird", price "$40.00", ticket count 300  
**When** they change the name to "Super Early Bird"  
**And** they change the price to "$35.00"  
**And** they change the ticket count to "250"  
**And** they submit the form  
**Then** all three fields are updated successfully  
**And** the updated offer appears on the show page with all new values

### Scenario 5: Updating Only the Offer Name

**Given** an event organizer is editing a ticket offer  
**And** they want to correct a typo in the offer name  
**When** they change the name from "Genral Admission" to "General Admission"  
**And** they leave price and ticket count unchanged  
**And** they submit the form  
**Then** only the name is updated  
**And** the price and ticket count remain unchanged  
**And** the update timestamp is set

### Scenario 6: Cross-Tenant Isolation

**Given** an event organizer from Tenant A  
**And** a ticket offer exists for a show belonging to Tenant B  
**When** they attempt to access the edit page for Tenant B's ticket offer  
**Then** the system displays a "Ticket offer not found" error  
**And** no form is displayed  
**And** the ticket offer data is not exposed

## Acceptance Criteria

### Form Display and Navigation

- [ ] Show detail page displays an "Edit" button or link for each ticket offer
- [ ] Edit page displays "Edit Ticket Offer" header
- [ ] Form is pre-populated with the current ticket offer data:
  - Offer name
  - Price
  - Ticket count
- [ ] While loading ticket offer data, page displays a loading indicator
- [ ] If ticket offer cannot be found or loaded, page displays clear error message
- [ ] Form includes "Update Offer" button to save changes
- [ ] Form includes "Cancel" button to abandon changes and return to show page

### Editable Fields

- [ ] User can modify the offer name (required, max 100 characters)
- [ ] User can modify the price (required, positive decimal)
- [ ] User can modify the ticket count (required, positive integer)
- [ ] All fields display their current values when the page loads
- [ ] Fields support updating one field, multiple fields, or all fields

### Input Validation

- [ ] Offer name is required and cannot be empty
- [ ] Offer name must be between 1 and 100 characters
- [ ] Price is required and must be a positive decimal value
- [ ] Price must be greater than zero
- [ ] Ticket count is required and must be a positive integer
- [ ] Ticket count must be greater than zero
- [ ] All validation errors are displayed inline next to the relevant input fields
- [ ] Form submission is prevented when validation errors exist
- [ ] Error messages clear when user corrects the invalid input

### Capacity Validation

- [ ] System calculates available capacity for the offer being edited
- [ ] Available capacity = show ticket count - sum of all OTHER offers' ticket counts
- [ ] The offer's current ticket count is not included in the "already allocated" calculation
- [ ] New ticket count must not exceed available capacity (including current allocation)
- [ ] If ticket count exceeds capacity, validation error displays with specific remaining capacity
- [ ] Validation error format: "Ticket count exceeds available capacity. Only [X] tickets available for this offer (including its current allocation)."
- [ ] Validation occurs before saving to prevent race conditions
- [ ] Concurrent updates are handled safely with transaction isolation

### User Experience

- [ ] The ticket offer edit form provides clear labels for all input fields
- [ ] Required fields are visually indicated (e.g., with asterisk or "required" label)
- [ ] Price field includes currency symbol or indicator (e.g., "$" or "USD")
- [ ] A "Cancel" action returns to the show page without saving changes
- [ ] An "Update Offer" action submits the form
- [ ] Upon successful update, a confirmation message is displayed
- [ ] Upon successful update, the user is returned to the show page showing the updated offer
- [ ] If submission fails due to server error, an error message is displayed and form data is retained
- [ ] Submit button is disabled while the offer is being updated
- [ ] Submit button shows a loading indicator during update
- [ ] Input fields are disabled while the offer is being updated

### Security & Access Control

- [ ] Only authenticated users can edit ticket offers
- [ ] Users can only edit ticket offers for shows that belong to their tenant
- [ ] Attempting to edit an offer for a show from a different tenant returns an authorization error
- [ ] The ticket offer remains associated with the same show after update

### Data Integrity

- [ ] Ticket offer's unique identifier (Id, TicketOfferGuid) remains unchanged
- [ ] Ticket offer's association with the show remains unchanged
- [ ] Ticket offer's original creation timestamp remains unchanged
- [ ] UpdatedAt timestamp is automatically set when changes are saved
- [ ] Only the name, price, and ticket count fields can be modified through this feature
- [ ] Ticket offer update is transactional (all-or-nothing)
- [ ] Database constraints prevent invalid data (negative values, empty names)

### Error Handling

- [ ] If the ticket offer no longer exists when submitting, a clear error message is displayed
- [ ] If the show no longer exists when submitting, a clear error message is displayed
- [ ] If a database error occurs, a user-friendly error message is displayed (not technical details)
- [ ] Network errors during submission provide appropriate user feedback with retry guidance
- [ ] If the show's ticket count or other offers change between loading the form and submission, validation uses the current values
- [ ] If other offers are modified concurrently, validation recalculates available capacity before saving

## Prerequisites

This user story depends on the following functionality being completed:

- [Create Ticket Offer](./create-ticket-offer.md) - Ticket offers must exist before they can be edited
- [View Show](./view-show.md) - Shows must be viewable to access ticket offer management
- User Authentication - Users must be authenticated to edit ticket offers
- Tenant Resolution - The system must correctly identify the current tenant

## Technical Notes

**Multi-Tenant Considerations:**
- Ticket offers are child entities that inherit tenant context through their relationship with Show
- Ticket offers do NOT store TenantId directly but are filtered via the Show → Venue navigation property chain
- Query filters ensure ticket offers are automatically scoped to the current tenant

**Entity Relationships:**
- TicketOffer → Show (many-to-one, required, never modified)
- TicketOffer inherits tenant context from Show

**Capacity Calculation for Updates:**
- When editing an offer, available capacity = Show.TicketCount - SUM(TicketOffer.TicketCount for all OTHER offers on the show)
- The current offer's ticket count is excluded from the "already allocated" total
- Example: Show has 1000 tickets, Offer A has 600, Offer B has 200. When editing Offer A, available capacity = 1000 - 200 = 800 tickets
- Validation must be performed within a transaction to prevent race conditions
- Consider using optimistic concurrency for inventory protection
