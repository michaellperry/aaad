---
name: frontend-architecture-patterns
description: Frontend architecture patterns for React applications using React Router 7, TanStack Query, and component-based design. Use when planning frontend features, defining routes, designing data strategies, or conducting component gap analysis.
---

# Frontend Architecture Patterns

Best practices for frontend architecture focusing on routing, state management, component design, and technical specification processes.

## Role & Responsibilities

The Frontend Architect is responsible for the **HOW** of the implementation:
- **Output**: Frontend Technical Spec (FTS) in `docs/specs/`
- **Goal**: Define the technical blueprint before any code is written
- **Scope**: Architecture decisions, not implementation details

## Frontend Technical Spec (FTS) Template

Every major feature must start with an FTS containing:

### 1. Route Definition
URL structure and React Router configuration
```typescript
// Route structure example
/venues                    // List all venues
/venues/new               // Create new venue
/venues/:id               // View venue details
/venues/:id/edit          // Edit venue
/venues/:id/acts          // Venue acts management
```

### 2. Data Strategy
Query keys, mutation strategies, and cache invalidation
```typescript
// Query key factory pattern
const venueQueries = {
  all: ['venues'] as const,
  lists: () => [...venueQueries.all, 'list'] as const,
  list: (filters: VenueFilters) => [...venueQueries.lists(), { filters }] as const,
  details: () => [...venueQueries.all, 'detail'] as const,
  detail: (id: string) => [...venueQueries.details(), id] as const,
}
```

### 3. Component Gap Analysis
List of new Atoms/Molecules vs. Organisms needed
```markdown
**Existing Components:**
- Button (Atom)
- Input (Atom)
- Card (Molecule)

**New Components Needed:**
- VenueTypeSelect (Molecule) - Dropdown for venue types
- VenueCapacityInput (Molecule) - Specialized numeric input
- VenueCard (Organism) - Complete venue display card
```

### 4. Security/Auth
Required roles and permission checks
```typescript
// Permission requirements
const venuePermissions = {
  read: ['tenant-member'],
  create: ['venue-manager', 'admin'],
  update: ['venue-manager', 'admin'],
  delete: ['admin']
}
```

## Routing Patterns (React Router 7)

### File-Based Routing Structure
```
src/pages/
├── venues/
│   ├── index.tsx          // /venues - list page
│   ├── new.tsx            // /venues/new - create page
│   ├── $id/
│   │   ├── index.tsx      // /venues/:id - detail page
│   │   ├── edit.tsx       // /venues/:id/edit - edit page
│   │   └── acts.tsx       // /venues/:id/acts - acts management
│   └── _layout.tsx        // Shared layout for venue pages
```

### Route Protection
```typescript
// Protected route wrapper
function ProtectedRoute({ 
  children, 
  requiredPermissions 
}: { 
  children: React.ReactNode
  requiredPermissions: string[]
}) {
  const { user, permissions } = useAuth()
  
  if (!user) {
    return <Navigate to="/login" replace />
  }
  
  const hasPermission = requiredPermissions.some(
    permission => permissions.includes(permission)
  )
  
  if (!hasPermission) {
    return <Navigate to="/unauthorized" replace />
  }
  
  return <>{children}</>
}

// Usage in route configuration
<Route 
  path="/venues/new" 
  element={
    <ProtectedRoute requiredPermissions={['venue-manager', 'admin']}>
      <CreateVenue />
    </ProtectedRoute>
  } 
/>
```

### Data Loaders
```typescript
// Pre-fetch data for route
export async function venueLoader({ params }: LoaderFunctionArgs) {
  const venueId = params.id!
  
  // Pre-fetch venue data
  const venue = await queryClient.ensureQueryData({
    queryKey: venueQueries.detail(venueId),
    queryFn: () => venueApi.getById(venueId),
  })
  
  return { venue }
}

// Route with loader
<Route 
  path="/venues/:id" 
  loader={venueLoader}
  element={<VenueDetail />} 
/>
```

## State Management (TanStack Query)

### Query Key Factory Pattern
```typescript
// Centralized query key management
export const queryKeys = {
  venues: {
    all: ['venues'] as const,
    lists: () => [...queryKeys.venues.all, 'list'] as const,
    list: (filters: VenueFilters) => 
      [...queryKeys.venues.lists(), { filters }] as const,
    details: () => [...queryKeys.venues.all, 'detail'] as const,
    detail: (id: string) => [...queryKeys.venues.details(), id] as const,
  },
  acts: {
    all: ['acts'] as const,
    byVenue: (venueId: string) => 
      [...queryKeys.acts.all, 'venue', venueId] as const,
  }
}
```

### Query Patterns
```typescript
// List query with filters
function useVenues(filters: VenueFilters = {}) {
  return useQuery({
    queryKey: queryKeys.venues.list(filters),
    queryFn: () => venueApi.getList(filters),
    staleTime: 5 * 60 * 1000, // 5 minutes
  })
}

// Detail query
function useVenue(id: string) {
  return useQuery({
    queryKey: queryKeys.venues.detail(id),
    queryFn: () => venueApi.getById(id),
    enabled: !!id,
  })
}
```

### Mutation Patterns
```typescript
// Create mutation with optimistic updates
function useCreateVenue() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: venueApi.create,
    onMutate: async (newVenue) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ 
        queryKey: queryKeys.venues.lists() 
      })
      
      // Snapshot previous value
      const previousVenues = queryClient.getQueryData(
        queryKeys.venues.lists()
      )
      
      // Optimistically update
      queryClient.setQueryData(
        queryKeys.venues.lists(),
        (old: any) => old ? [...old, { ...newVenue, id: 'temp' }] : [newVenue]
      )
      
      return { previousVenues }
    },
    onError: (err, newVenue, context) => {
      // Rollback on error
      if (context?.previousVenues) {
        queryClient.setQueryData(
          queryKeys.venues.lists(),
          context.previousVenues
        )
      }
    },
    onSuccess: (venue) => {
      // Invalidate and refetch
      queryClient.invalidateQueries({ 
        queryKey: queryKeys.venues.lists() 
      })
      
      // Set individual venue cache
      queryClient.setQueryData(
        queryKeys.venues.detail(venue.id),
        venue
      )
    }
  })
}
```

### Error Handling
```typescript
// Global error boundary
class QueryErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false }
  }
  
  static getDerivedStateFromError(error: Error): State {
    return { hasError: true }
  }
  
  render() {
    if (this.state.hasError) {
      return <ErrorFallback onRetry={this.handleRetry} />
    }
    
    return this.props.children
  }
}

// Query-specific error handling
function useVenuesWithErrorHandling() {
  return useQuery({
    queryKey: queryKeys.venues.lists(),
    queryFn: venueApi.getList,
    onError: (error) => {
      if (error.status === 401) {
        // Redirect to login
        navigate('/login')
      } else {
        // Show error toast
        toast.error('Failed to load venues')
      }
    },
    retry: (failureCount, error) => {
      // Don't retry on auth errors
      if (error.status === 401 || error.status === 403) {
        return false
      }
      return failureCount < 3
    }
  })
}
```

## Component Gap Analysis

### Analysis Process
1. **Audit Existing Components**: Check `src/components/atoms` and `molecules` first
2. **Identify Gaps**: List missing UI elements needed for the feature
3. **Classify Components**: Determine if new component is Atom, Molecule, or Organism
4. **Delegate Implementation**: Don't implement low-level components; spec them for Design System Engineer

### Component Classification
```typescript
// Atoms - Basic building blocks
interface ButtonProps {
  variant: 'primary' | 'secondary' | 'danger'
  size: 'small' | 'medium' | 'large'
  disabled?: boolean
  children: React.ReactNode
}

// Molecules - Combinations of atoms
interface SearchInputProps {
  placeholder?: string
  onSearch: (query: string) => void
  isLoading?: boolean
}

// Organisms - Complex components with business logic
interface VenueListProps {
  venues: Venue[]
  onVenueSelect: (venue: Venue) => void
  filters?: VenueFilters
  onFiltersChange?: (filters: VenueFilters) => void
}
```

### Component Specification Template
```markdown
## Component: VenueCapacityInput (Molecule)

**Purpose**: Specialized input for venue capacity with validation and formatting

**Props**:
- `value: number` - Current capacity value
- `onChange: (value: number) => void` - Change handler
- `min?: number` - Minimum capacity (default: 1)
- `max?: number` - Maximum capacity (default: 100000)
- `error?: string` - Validation error message

**Behavior**:
- Format numbers with thousands separators
- Validate range on blur
- Show error state with message
- Disable input when loading

**Dependencies**: 
- Input (Atom)
- ErrorText (Atom)
```

## Security and Permission Patterns

### Permission-Based Rendering
```typescript
// Permission hook
function usePermissions() {
  const { user } = useAuth()
  
  return {
    canCreate: user?.permissions.includes('venue-manager'),
    canEdit: user?.permissions.includes('venue-manager'),
    canDelete: user?.permissions.includes('admin'),
    isAdmin: user?.permissions.includes('admin')
  }
}

// Conditional rendering
function VenueActions({ venue }: { venue: Venue }) {
  const { canEdit, canDelete } = usePermissions()
  
  return (
    <div className="venue-actions">
      {canEdit && (
        <Button onClick={() => navigate(`/venues/${venue.id}/edit`)}>
          Edit
        </Button>
      )}
      {canDelete && (
        <Button variant="danger" onClick={() => deleteVenue(venue.id)}>
          Delete
        </Button>
      )}
    </div>
  )
}
```

### Route-Level Security
```typescript
// Security configuration
const routePermissions = {
  '/venues': { read: ['tenant-member'] },
  '/venues/new': { create: ['venue-manager', 'admin'] },
  '/venues/:id/edit': { update: ['venue-manager', 'admin'] },
  '/venues/:id/delete': { delete: ['admin'] }
}

// Permission guard
function useRoutePermission(path: string, action: string) {
  const { permissions } = useAuth()
  const routeConfig = routePermissions[path]
  
  return routeConfig?.[action]?.some(
    permission => permissions.includes(permission)
  ) ?? false
}
```

These patterns ensure scalable, maintainable frontend architecture with proper separation of concerns, effective state management, and security-first design principles.