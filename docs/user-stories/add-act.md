# User Story: Add Act

**As a** GloboTicket event organizer
**I want to** add new performing acts to the system
**So that** I can build my roster of performers and associate them with events

## Description

This user story describes the capability for authorized users to create new act records in the GloboTicket ticketing platform. An act represents a performing artist, band, comedian, speaker, or any other performer who will participate in events.

The feature provides a simple form for capturing the performer's name. Once created, acts can be associated with shows and events, enabling event organizers to schedule performances and manage their roster of talent.

**Business Value:**
- Enables organizations to build and maintain their roster of performers
- Provides foundational data for event and show scheduling
- Simplifies performer management with minimal required information
- Supports quick addition of acts as new talent is booked

## Scenario

### Main Success Scenario

**Given** I am logged into the GloboTicket system
**And** I am viewing the acts management page
**When** I click the "Create Act" button and enter a valid performer name
**And** I submit the form
**Then** the new act is created and saved
**And** I am redirected to the acts list
**And** the newly created act appears in the list

### Alternative Scenario: Quick Act Creation During Event Planning

**Given** I am planning an event and need to add a new performer
**When** I navigate to create act page from the acts list
**And** I enter the performer's name
**And** I click "Create Act"
**Then** the act is immediately available for selection when creating shows
**And** I can return to my event planning workflow

### Error Scenario: Missing Required Information

**Given** I am on the create act page
**When** I attempt to submit the form without entering an act name
**Then** the system displays an error message "Act name is required"
**And** the form is not submitted
**And** I remain on the create act page to correct the error

### Error Scenario: Name Too Long

**Given** I am entering an act name
**When** I type more than 100 characters
**Then** the input field prevents additional characters
**And** I understand the maximum length constraint

## Acceptance Criteria

### Core Functionality

- [ ] User can navigate to the create act page from the acts list
- [ ] Page displays a clear header "Create Act" with description
- [ ] Form contains a single text input field for "Act Name"
- [ ] User can enter a name up to 100 characters
- [ ] User can submit the form by clicking "Create Act" button
- [ ] User can cancel and return to acts list without saving
- [ ] After successful creation, user is redirected to acts list
- [ ] Newly created act appears immediately in the acts list
- [ ] Each act is automatically assigned a unique identifier

### Input Validation

- [ ] Act name is a required field
- [ ] Act name cannot be empty or contain only whitespace
- [ ] Act name has a maximum length of 100 characters
- [ ] Input field enforces the 100-character limit (user cannot type beyond it)
- [ ] Leading and trailing spaces are automatically removed before saving
- [ ] Validation occurs when user attempts to submit the form

### User Experience Requirements

- [ ] Page header reads "Create Act"
- [ ] Page description reads "Add a new performing act to the system"
- [ ] Act name field label reads "Act Name *" (asterisk indicates required)
- [ ] Input field shows placeholder text "Enter act name"
- [ ] Submit button is labeled "Create Act"
- [ ] Cancel button is labeled "Cancel"
- [ ] Submit button is disabled while the act is being created
- [ ] Submit button shows a loading indicator during creation
- [ ] Input field is disabled while the act is being created
- [ ] Form maintains visual consistency with other forms in the application

### Navigation & Workflow

- [ ] User accesses create act page from acts list via create/add button
- [ ] Clicking "Cancel" returns user to acts list without saving
- [ ] Successful creation redirects to acts list (URL: /acts)
- [ ] Failed creation keeps user on create page to retry
- [ ] User can navigate away from create page using browser back button

### Error Handling

- [ ] Missing act name displays error: "Act name is required"
- [ ] Whitespace-only name displays error: "Act name is required"
- [ ] Name exceeding 100 characters displays error: "Act name must be 100 characters or less"
- [ ] Errors display in a prominent error message container
- [ ] Error message container has red/error styling to draw attention
- [ ] Error messages appear above the form fields
- [ ] Error messages clear when user corrects the invalid input
- [ ] Network/system errors display message: "Failed to create act"
- [ ] After error, user can correct input and resubmit without refreshing page

### Data Isolation

- [ ] Users only see acts that belong to their organization
- [ ] Users cannot view or access acts created by other organizations
- [ ] Newly created acts are automatically associated with the user's organization
- [ ] Acts list shows only acts within the user's organization

### Access Control

- [ ] Only authenticated users can access the create act page
- [ ] Unauthenticated users are redirected to login page
- [ ] All users within an organization can create acts
- [ ] Acts are automatically created within the user's organizational context

## Prerequisites

### User and Access Prerequisites

- [User Authentication](./user-authentication.md) - Users must be logged in to create acts
- User must have a valid session with organizational context
- User must have permission to manage acts (standard user permission)

### Data Prerequisites

- User's organization must exist in the system
- Acts list page must be accessible to display results after creation

### Navigation Prerequisites

- Acts list page must be available at /acts
- Create act page must be accessible at /acts/new
- Navigation between pages must work correctly
