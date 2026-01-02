---
name: spec-writing-ui
description: Use this skill when specifying user interface designs, components, and interaction flows.
---

# User Interface Specification

```markdown
## User Interface Design

### Page Structure & Navigation

**New Pages**:
1. `/resources` - Resource listing page
2. `/resources/create` - Resource creation page
3. `/resources/:id` - Resource detail page
4. `/resources/:id/edit` - Resource edit page

**Navigation Updates**:
- Add "Resources" link to main navigation menu
- Update breadcrumb component to include resource paths

### Component Breakdown

#### ResourceListPage

**Purpose**: Display paginated, filterable list of resources

**Props**: None (uses URL query params)

**State Management**: Use Tanstack Query to fetch resources based on filters using endpoint `GET /api/resources` with query key ['resources', filters]

**Child Components**:
- `ResourceCard`: Display individual resource summary
- `ResourceFilters`: Filter controls
- `Pagination`: Page navigation

**User Interactions**:
- Click card → Navigate to detail page
- Apply filters → Update URL params, refetch
- Change page → Update URL, refetch

#### ResourceForm

**Purpose**: Reusable form for create/edit operations

**Props**:
- initialValues: ResourceFormData | undefined
- onSubmit: (data: ResourceFormData) => Promise<void>
- isLoading: boolean

**Validation Rules**:
Define validation once here, then reference in API section:
- Name: Required, max 100 characters
- Description: Optional, max 500 characters
- [List all validation rules with exact constraints matching OpenAPI spec]

**Form Fields**:
1. Name (text input, required, maxLength: 100)
2. Description (textarea, optional, maxLength: 500)
3. [List all fields with types - validation rules already defined above]

**API Integration**:
- Create mode: `POST /api/resources` (see [OpenAPI spec](#openapi-specification))
- Edit mode: `PUT /api/resources/:id`
- Uses validation rules defined above

### Interaction Flows

#### Create Resource Flow
1. User clicks "Create Resource" button
2. Navigate to /resources/create
3. User fills form (fields and validation defined in ResourceForm component)
4. On submit:
   a. Validate form data (using validation rules from component spec)
   b. POST /api/resources
   c. On success: Navigate to detail page
   d. On error: Display inline validation errors

### Accessibility Requirements
- All form inputs have associated labels
- Error messages announced to screen readers
- Keyboard navigation supported throughout
- Focus management on modal open/close
- ARIA labels on icon-only buttons

### Responsive Behavior
- Desktop (>1024px): 3-column grid
- Tablet (768-1024px): 2-column grid
- Mobile (<768px): Single column, stacked layout
```

## UI Design Principles

- Component-based architecture (atomic design)
- Use Tanstack Query for server state
- Implement optimistic updates where appropriate
- Show loading states during async operations
- Display meaningful error messages
- Support keyboard navigation
- Follow existing UI patterns in codebase
- Mobile-first responsive design
- Ensure accessibility compliance (WCAG 2.1 AA)

