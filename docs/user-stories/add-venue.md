# User Story: Add Venue

**As a** GloboTicket event organizer
**I want to** add new venues to the system
**So that** I can host events at various locations within my organization's tenant

## Description

This user story describes the capability for authorized users to create new venue records in the GloboTicket multi-tenant ticketing platform. Venues represent physical locations where events can be held. Each venue is scoped to the user's tenant, ensuring data isolation in the multi-tenant environment.

The feature provides a comprehensive form for capturing essential venue information including name, location data, seating capacity, and descriptive details. The system supports optional geolocation data to enable mapping and location-based features.

**Business Value:**
- Enables organizations to build their venue inventory for event planning
- Provides foundational data for event creation and ticketing
- Supports location-based features through optional geographic coordinates
- Maintains data isolation through tenant-scoped venue creation

## Scenario

### Main Success Scenario

**Given** I am an authenticated user with access to the GloboTicket system
**And** I am viewing the venue management interface
**When** I navigate to the create venue page and submit a form with valid venue details
**Then** a new venue record is created within my tenant's data scope
**And** I am redirected to the venues list where the new venue is visible
**And** the venue is assigned a unique GUID identifier
**And** the system records creation timestamp metadata

### Alternative Scenario: Create Venue with Optional Location Data

**Given** I am creating a new venue
**And** I have geographic coordinates for the venue location
**When** I provide latitude and longitude values within valid ranges
**Then** the system stores the location as a WGS84 geography point
**And** the venue can be used for location-based features like mapping

### Alternative Scenario: Create Venue with Minimal Required Fields

**Given** I am creating a new venue
**When** I provide only the required fields (name, seating capacity, description)
**And** I leave optional fields (address, latitude, longitude) empty
**Then** the venue is successfully created with null values for optional fields
**And** I can add location details later through the edit functionality

## Acceptance Criteria

### Functional Requirements

- [ ] User can access the create venue page at `/venues/new` when authenticated
- [ ] User can submit a form with the following fields:
  - Name (required, max 100 characters)
  - Address (optional, max 300 characters)
  - Seating Capacity (required, positive integer)
  - Description (required, max 2000 characters)
  - Latitude (optional, range -90 to 90)
  - Longitude (optional, range -180 to 180)
- [ ] System generates a unique GUID (VenueGuid) for the new venue
- [ ] System automatically associates the venue with the current user's tenant (TenantId)
- [ ] System records CreatedAt timestamp in UTC when venue is created
- [ ] Successful creation redirects user to the venues list page (`/venues`)
- [ ] New venue appears immediately in the venues list after creation

### Validation Requirements

- [ ] Name field is required and cannot be empty whitespace
- [ ] Name must not exceed 100 characters
- [ ] Description field is required and cannot be empty whitespace
- [ ] Description must not exceed 2000 characters
- [ ] Seating capacity is required and must be a positive number (> 0)
- [ ] Address must not exceed 300 characters if provided
- [ ] Latitude must be between -90 and 90 if provided
- [ ] Longitude must be between -180 and 180 if provided
- [ ] Validation errors display clear, actionable messages
- [ ] Form prevents submission when validation fails
- [ ] Validation errors clear when user corrects invalid fields

### Geographic Data Requirements

- [ ] System converts latitude/longitude coordinates to NetTopologySuite Point geometry
- [ ] Geographic points use WGS84 coordinate system (SRID 4326)
- [ ] Location data is stored in SQL Server geography column type
- [ ] System handles null location data gracefully (when coordinates not provided)
- [ ] Geographic coordinates are stored as Point(longitude, latitude) internally

### Multi-Tenancy Requirements

- [ ] TenantId is automatically set from the authenticated user's tenant context
- [ ] Users cannot create venues in other tenants' data scopes
- [ ] Venue GUID is unique within the tenant (enforced by composite key: TenantId + VenueGuid)
- [ ] Query filters automatically restrict venue visibility to current tenant

### User Experience Requirements

- [ ] Form fields have clear labels with required field indicators (*)
- [ ] Form displays page header "Create Venue" with description
- [ ] Submit button is labeled "Create Venue"
- [ ] Cancel button navigates back to venues list without creating venue
- [ ] Submit button is disabled during form submission to prevent double-submission
- [ ] Form shows loading state during venue creation
- [ ] Form displays error messages in a prominent, styled error container
- [ ] Optional location picker component assists with address and coordinate entry

### API Requirements

- [ ] POST endpoint at `/api/venues` accepts CreateVenueDto payload
- [ ] Endpoint requires authentication (returns 401 if not authenticated)
- [ ] Successful creation returns HTTP 201 Created with Location header
- [ ] Response includes full VenueDto with generated ID and metadata
- [ ] Location header points to `/api/venues/{venueGuid}`

### Data Integrity Requirements

- [ ] Database enforces primary key constraint on Id
- [ ] Database enforces composite alternate key on (TenantId, VenueGuid)
- [ ] Database maintains foreign key relationship to Tenant with cascade delete
- [ ] Name and Description columns are marked as required in database
- [ ] All string fields respect configured maximum lengths
- [ ] CreatedAt timestamp is set automatically by DbContext.SaveChanges()

## Prerequisites

### Entity and Domain Prerequisites

- [User Authentication](./user-authentication.md) - Users must be authenticated to create venues
- [Tenant Resolution](./tenant-resolution.md) - System must resolve user's tenant context
- Tenant entity must exist in database (venues require valid TenantId foreign key)

### Infrastructure Prerequisites

- Database with applied Venue migration (AddVenueEntity)
- SQL Server with NetTopologySuite geography type support enabled
- VenueService registered in dependency injection container
- TenantContext middleware configured and running after authentication

### Frontend Prerequisites

- React Router configured with `/venues/new` route
- VenueForm component implemented with validation logic
- CreateVenuePage component mounted and accessible
- API client configured for venue endpoints
- Authentication state management providing user session
