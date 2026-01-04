# User Story: Create Ticket Offer

**As an** event organizer  
**I want to** create ticket offers for a show with specific pricing and inventory  
**So that** I can make different ticket types available for sale while managing show capacity

## Description

Event organizers need to create ticket offers for their scheduled shows to make tickets available for purchase. Each ticket offer represents a distinct ticket type (e.g., "General Admission", "VIP", "Early Bird") with its own name, price, and allocated ticket count. The system enforces inventory constraints to ensure the total tickets allocated across all offers does not exceed the show's total ticket count.

When viewing a show's details page, organizers can create new ticket offers and see all existing offers for that show. The system validates that the requested ticket count for a new offer, combined with tickets already allocated to existing offers, does not exceed the show's available capacity.

**Business Value:**
- Enables monetization of shows through ticket sales
- Supports flexible pricing strategies with multiple ticket types
- Prevents overselling by enforcing inventory constraints
- Provides clear visibility of ticket allocation across offers
- Maintains data integrity within multi-tenant architecture

## Scenarios

### Scenario 1: Successfully Creating First Ticket Offer

**Given** an event organizer is viewing a show details page  
**And** the show belongs to their tenant  
**And** the show has a ticket count of 1000  
**And** no ticket offers exist for this show yet  
**When** they navigate to create a new ticket offer  
**And** they enter "General Admission" as the offer name  
**And** they enter "$50.00" as the price  
**And** they enter "600" as the ticket count  
**And** they submit the form  
**Then** the ticket offer is created successfully  
**And** the offer is associated with the show  
**And** the offer appears in the list of ticket offers on the show page  
**And** a success confirmation message is displayed

### Scenario 2: Creating Additional Ticket Offer with Remaining Capacity

**Given** an event organizer is viewing a show with 1000 total tickets  
**And** an existing ticket offer "General Admission" has allocated 600 tickets  
**And** 400 tickets remain available  
**When** they create a new ticket offer "VIP"  
**And** they enter "$150.00" as the price  
**And** they enter "200" as the ticket count  
**And** they submit the form  
**Then** the ticket offer is created successfully  
**And** both "General Admission" and "VIP" offers are displayed on the show page  
**And** 200 tickets remain available for future offers

### Scenario 3: Attempting to Exceed Available Ticket Capacity

**Given** an event organizer is viewing a show with 1000 total tickets  
**And** existing ticket offers have allocated 850 tickets  
**And** 150 tickets remain available  
**When** they attempt to create a new ticket offer  
**And** they enter "Late Entry" as the offer name  
**And** they enter "$30.00" as the price  
**And** they enter "200" as the ticket count (exceeding available capacity)  
**And** they submit the form  
**Then** the ticket offer is not created  
**And** a validation error message is displayed: "Ticket count exceeds available capacity. Only 150 tickets remain available."  
**And** the form retains the entered data for correction

### Scenario 4: Creating Ticket Offer Using All Remaining Capacity

**Given** an event organizer is viewing a show with 1000 total tickets  
**And** existing ticket offers have allocated 700 tickets  
**And** 300 tickets remain available  
**When** they create a new ticket offer "Student Discount"  
**And** they enter "$25.00" as the price  
**And** they enter "300" as the ticket count (exactly the remaining capacity)  
**And** they submit the form  
**Then** the ticket offer is created successfully  
**And** 0 tickets remain available  
**And** the show is at full capacity

### Scenario 5: Viewing All Ticket Offers on Show Page

**Given** an event organizer is viewing a show details page  
**And** the show has multiple ticket offers  
**When** the page loads  
**Then** all ticket offers are displayed in a list  
**And** each offer shows its name, price, and ticket count  
**And** the total allocated tickets across all offers is displayed  
**And** the remaining available capacity is displayed

## Acceptance Criteria

### Core Functionality

- [ ] Ticket offers can be created from a show's details page
- [ ] Each ticket offer is associated with exactly one show
- [ ] Each ticket offer has a name (text field)
- [ ] Each ticket offer has a price (decimal/currency field)
- [ ] Each ticket offer has a ticket count (positive integer)
- [ ] Ticket offers are automatically associated with the current tenant through the show relationship
- [ ] Successfully created ticket offers appear in the list on the show page
- [ ] The show page displays all ticket offers for the current show
- [ ] Each displayed offer shows: name, price, and ticket count

### Input Validation

- [ ] Offer name is required and cannot be empty
- [ ] Offer name must be between 1 and 100 characters
- [ ] Price is required and must be a positive decimal value
- [ ] Price must be greater than zero
- [ ] Price is stored with two decimal places (currency format)
- [ ] Ticket count is required and must be a positive integer
- [ ] Ticket count must be greater than zero
- [ ] All validation errors are displayed inline next to the relevant input fields
- [ ] Form submission is prevented when validation errors exist

### Inventory Validation

- [ ] System calculates total tickets allocated across all existing offers for the show
- [ ] System calculates remaining available capacity: (show ticket count - sum of existing offer ticket counts)
- [ ] New offer ticket count must not exceed remaining available capacity
- [ ] If ticket count exceeds capacity, validation error displays: "Ticket count exceeds available capacity. Only [X] tickets remain available."
- [ ] Validation occurs before saving to prevent race conditions
- [ ] If multiple offers are created simultaneously, only valid offers within capacity are saved

### Capacity Display

- [ ] Show page displays the show's total ticket count
- [ ] Show page displays the total tickets allocated across all offers
- [ ] Show page displays the remaining available capacity
- [ ] Capacity information updates immediately when new offers are created
- [ ] Capacity display is clearly labeled and easy to understand
- [ ] When capacity reaches zero, a clear indicator shows the show is fully allocated

### User Experience

- [ ] The ticket offer creation form provides clear labels for all input fields
- [ ] Required fields are visually indicated (e.g., with asterisk or "required" label)
- [ ] Price field includes currency symbol or indicator (e.g., "$" or "USD")
- [ ] A "Cancel" action is available to return to the show page without saving
- [ ] A "Save" or "Create Offer" action submits the form
- [ ] Upon successful creation, a confirmation message is displayed
- [ ] Upon successful creation, the user remains on or returns to the show page showing the new offer
- [ ] If submission fails due to server error, an error message is displayed and form data is retained
- [ ] The ticket offers list is sorted in a consistent order (e.g., by creation date or name)

### Security & Access Control

- [ ] Only authenticated users can create ticket offers
- [ ] Users can only create ticket offers for shows that belong to their tenant
- [ ] Attempting to create an offer for a show from a different tenant returns an authorization error
- [ ] The ticket offers list only displays offers for the current show within the user's tenant

### Data Integrity

- [ ] Each ticket offer must reference a valid show ID
- [ ] Each ticket offer inherits tenant context through its relationship with the show
- [ ] The offer name is stored as a string
- [ ] The price is stored as a decimal with appropriate precision for currency
- [ ] The ticket count is stored as an integer
- [ ] CreatedAt timestamp is automatically set when the offer is created
- [ ] Ticket offer creation is transactional (all-or-nothing)
- [ ] Database constraints prevent ticket count from being negative

### Error Handling

- [ ] If the show no longer exists when submitting, a clear error message is displayed
- [ ] If a database error occurs, a user-friendly error message is displayed (not technical details)
- [ ] Network errors during submission provide appropriate user feedback with retry guidance
- [ ] If the show's ticket count changes between loading the form and submission, validation uses the current count
- [ ] If other offers are created concurrently, validation recalculates available capacity before saving

## Prerequisites

This user story depends on the following functionality being completed:

- [View Show](./view-show.md) - Shows must exist and be viewable before ticket offers can be created
- [Add Show to Act](./add-show-to-act.md) - Shows must be created with ticket counts
- User Authentication - Users must be authenticated to create ticket offers
- Tenant Resolution - The system must correctly identify the current tenant

## Technical Notes

**Multi-Tenant Considerations:**
- Ticket offers are child entities that inherit tenant context through their relationship with Show
- Ticket offers do NOT store TenantId directly but are filtered via the Show navigation property
- Query filters ensure ticket offers are automatically scoped to the current tenant

**Entity Relationships:**
- TicketOffer → Show (many-to-one, required)
- TicketOffer inherits tenant context from Show
- Show → TicketOffer (one-to-many)

**Inventory Management:**
- Available capacity = Show.TicketCount - SUM(TicketOffer.TicketCount for all offers on the show)
- Validation must be performed within a transaction to prevent race conditions
- Consider using database-level constraints or optimistic concurrency for inventory protection
