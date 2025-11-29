# Acts Pages Implementation Specification

## Overview

This specification documents the patterns and structure for implementing Acts pages in the GloboTicket.Web application, based on the existing Venue implementation patterns. The Acts pages will follow the same atomic design principles, component hierarchy, and architectural patterns established in the Venue implementation.

---

## 1. Backend Entity Analysis

### Act Entity Structure

**Domain Entity** ([`Act.cs`](../src/GloboTicket.Domain/Entities/Act.cs:1)):
```csharp
public class Act : MultiTenantEntity
{
    public Guid ActGuid { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

**DTOs**:
- [`ActDto`](../src/GloboTicket.Application/DTOs/ActDto.cs:1): `{ Id, ActGuid, Name, CreatedAt, UpdatedAt }`
- [`CreateActDto`](../src/GloboTicket.Application/DTOs/CreateActDto.cs:1): `{ ActGuid, Name }` (Name max 100 chars)
- [`UpdateActDto`](../src/GloboTicket.Application/DTOs/UpdateActDto.cs:1): `{ Name }` (Name max 100 chars)

**API Endpoints** ([`ActEndpoints.cs`](../src/GloboTicket.API/Endpoints/ActEndpoints.cs:1)):
- `GET /api/acts` - Get all acts
- `GET /api/acts/{guid}` - Get act by GUID
- `POST /api/acts` - Create new act
- `PUT /api/acts/{guid}` - Update act
- `DELETE /api/acts/{guid}` - Delete act

### Key Differences from Venue

| Aspect | Venue | Act |
|--------|-------|-----|
| **Complexity** | High (7 properties) | Low (2 properties) |
| **Properties** | Name, Address, Location, Latitude, Longitude, SeatingCapacity, Description | Name only |
| **Validation** | Multiple fields with constraints | Single field (Name, max 100 chars) |
| **Geographic Data** | Yes (Point, Lat/Long) | No |
| **Capacity Info** | Yes (SeatingCapacity) | No |
| **Form Complexity** | Complex multi-field form | Simple single-field form |

---

## 2. Atomic Design Patterns (from Venue Implementation)

### Design Principles

Based on [`atomic-design.md`](../docs/atomic-design.md:1):

1. **Visual styling lives as low as possible** in the component tree
2. **Application layers mostly compose, not restyle**
3. **Theme (colors, typography) is separated from layout (flex, grid, spacing)**
4. **Light mode is the default** - dark mode must be explicit

### Component Hierarchy

```
Pages (Templates)
  └─ Organisms (VenueList, VenueForm)
      └─ Molecules (VenueCard, PageHeader, Card, EmptyState)
          └─ Atoms (Button, Heading, Text, Icon, Badge, Spinner)
              └─ Layout Primitives (Stack, Grid, Row, Container)
```

---

## 3. Type Definitions

### File: `src/GloboTicket.Web/src/types/act.ts`

Based on [`venue.ts`](../src/GloboTicket.Web/src/types/venue.ts:1) pattern:

```typescript
/**
 * Act interface matching the backend ActDto
 */
export interface Act {
  /** Unique identifier for the act */
  id: number;
  
  /** Unique GUID identifier for the act */
  actGuid: string;
  
  /** Name of the act */
  name: string;
  
  /** UTC timestamp when the act was created */
  createdAt: string;
  
  /** UTC timestamp when the act was last updated */
  updatedAt?: string;
}

/**
 * DTO for creating a new act
 */
export interface CreateActDto {
  actGuid: string;
  name: string;
}

/**
 * DTO for updating an existing act
 */
export interface UpdateActDto {
  name: string;
}
```

---

## 4. API Client Integration

### File: `src/GloboTicket.Web/src/api/client.ts`

Add to existing [`client.ts`](../src/GloboTicket.Web/src/api/client.ts:1):

```typescript
import type { Act, CreateActDto, UpdateActDto } from '../types/act';

export async function getActs(): Promise<Act[]> {
  const response = await fetch(`${API_BASE_URL}/api/acts`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Act[]>(response);
}

export async function getAct(id: string): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Act>(response);
}

export async function createAct(dto: CreateActDto): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<Act>(response);
}

export async function updateAct(id: string, dto: UpdateActDto): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<Act>(response);
}

export async function deleteAct(id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }
}
```

---

## 5. Component Specifications

### 5.1 ActCard (Molecule)

**File**: `src/GloboTicket.Web/src/components/molecules/ActCard.tsx`

**Pattern**: Based on [`VenueCard.tsx`](../src/GloboTicket.Web/src/components/molecules/VenueCard.tsx:1)

**Purpose**: Display act information in a card format

**Props**:
```typescript
export interface ActCardProps {
  act: Act;
  onClick?: (act: Act) => void;
}
```

**Key Features**:
- Composes [`Card`](../src/GloboTicket.Web/src/components/molecules/Card.tsx:1), [`Heading`](../src/GloboTicket.Web/src/components/atoms/Heading.tsx:1), [`Text`](../src/GloboTicket.Web/src/components/atoms/Text.tsx:1), [`Badge`](../src/GloboTicket.Web/src/components/atoms/Badge.tsx:1), [`Icon`](../src/GloboTicket.Web/src/components/atoms/Icon.tsx:1)
- Interactive card with hover effect when `onClick` provided
- Keyboard accessible (Enter/Space)
- Uses `Music` icon from lucide-react (instead of `MapPin` for venues)
- Displays act name prominently
- Shows creation date as metadata
- Simpler than VenueCard (no address, capacity, or description)

**Visual Structure**:
```
Card (interactive)
  └─ Stack (gap="md")
      ├─ Row (justify-between)
      │   ├─ Heading (h3) - Act Name
      │   └─ Badge (info) - Music Icon
      └─ Text (muted, sm) - Created date
```

---

### 5.2 ActList (Organism)

**File**: `src/GloboTicket.Web/src/components/organisms/ActList.tsx`

**Pattern**: Based on [`VenueList.tsx`](../src/GloboTicket.Web/src/components/organisms/VenueList.tsx:1)

**Purpose**: Fetch and display acts in a responsive grid

**Props**:
```typescript
export interface ActListProps {
  onActClick?: (act: Act) => void;
}
```

**State Management**:
```typescript
const [acts, setActs] = useState<Act[]>([]);
const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);
```

**States**:
1. **Loading**: [`Spinner`](../src/GloboTicket.Web/src/components/atoms/Spinner.tsx:1) with "Loading acts..." message
2. **Error**: [`EmptyState`](../src/GloboTicket.Web/src/components/molecules/EmptyState.tsx:1) with `AlertCircle` icon and retry action
3. **Empty**: [`EmptyState`](../src/GloboTicket.Web/src/components/molecules/EmptyState.tsx:1) with `Music` icon (instead of `Building2`)
4. **Success**: [`Grid`](../src/GloboTicket.Web/src/components/layout/Grid.tsx:1) of ActCards (responsive: 1/2/3 columns)

**Data Fetching**:
- Uses `getActs()` from API client
- Fetches on component mount
- Handles errors gracefully

---

### 5.3 ActForm (Organism)

**File**: `src/GloboTicket.Web/src/components/organisms/ActForm.tsx`

**Pattern**: Based on [`VenueForm.tsx`](../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx:1)

**Purpose**: Create or edit act with validation

**Props**:
```typescript
interface ActFormProps {
  act?: Act; // Optional for edit mode
  onSuccess?: (act: Act) => void;
  onCancel?: () => void;
}
```

**State**:
```typescript
const [name, setName] = useState(act?.name || '');
const [error, setError] = useState<string | null>(null);
const [isLoading, setIsLoading] = useState(false);
```

**Validation Rules**:
- Name is required
- Name max length: 100 characters
- Name must be trimmed

**Form Structure** (Much simpler than VenueForm):
```
<form>
  {error && <ErrorAlert />}
  
  <FormField>
    <label>Act Name *</label>
    <input type="text" maxLength={100} required />
  </FormField>
  
  <ButtonGroup>
    <Button variant="primary" isLoading={isLoading}>
      {isEditMode ? 'Update Act' : 'Create Act'}
    </Button>
    <Button variant="secondary" onClick={onCancel}>
      Cancel
    </Button>
  </ButtonGroup>
</form>
```

**Behavior**:
- Edit mode: Uses `updateAct(act.actGuid, dto)`
- Create mode: Uses `createAct(dto)` with `crypto.randomUUID()`
- Navigates to `/acts` on success (unless `onSuccess` provided)
- Shows validation errors inline

---

## 6. Page Specifications

### 6.1 ActsPage (Main Listing)

**File**: `src/GloboTicket.Web/src/pages/acts/ActsPage.tsx`

**Pattern**: Based on [`VenuesPage.tsx`](../src/GloboTicket.Web/src/pages/venues/VenuesPage.tsx:1)

**Purpose**: Main acts listing page

**Structure**:
```typescript
export function ActsPage() {
  const navigate = useNavigate();

  const handleActClick = (act: Act) => {
    navigate(`/acts/${act.actGuid}`);
  };

  return (
    <Stack gap="xl">
      <PageHeader
        title="Acts"
        description="Browse and manage all performing acts"
        action={
          <Button
            variant="primary"
            onClick={() => navigate(ROUTES.ACT_CREATE)}
          >
            <Icon icon={Plus} size="sm" />
            Add Act
          </Button>
        }
      />
      
      <ActList onActClick={handleActClick} />
    </Stack>
  );
}
```

**Composition**:
- [`PageHeader`](../src/GloboTicket.Web/src/components/molecules/PageHeader.tsx:1) with title, description, and action button
- [`ActList`](../src/GloboTicket.Web/src/components/organisms/ActList.tsx:1) organism
- [`Stack`](../src/GloboTicket.Web/src/components/layout/Stack.tsx:1) layout with `xl` gap

---

### 6.2 CreateActPage

**File**: `src/GloboTicket.Web/src/pages/acts/CreateActPage.tsx`

**Pattern**: Based on [`CreateVenuePage.tsx`](../src/GloboTicket.Web/src/pages/venues/CreateVenuePage.tsx:1)

**Purpose**: Create new act

**Structure**:
```typescript
export function CreateActPage() {
  const navigate = useNavigate();

  const handleSuccess = (act: Act) => {
    navigate('/acts');
  };

  const handleCancel = () => {
    navigate('/acts');
  };

  return (
    <Container>
      <Stack gap="xl">
        <PageHeader
          title="Create Act"
          description="Add a new performing act to the system"
        />
        <Card>
          <ActForm onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}
```

---

### 6.3 EditActPage

**File**: `src/GloboTicket.Web/src/pages/acts/EditActPage.tsx`

**Pattern**: Based on [`EditVenuePage.tsx`](../src/GloboTicket.Web/src/pages/venues/EditVenuePage.tsx:1)

**Purpose**: Edit existing act

**Structure**:
```typescript
export function EditActPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [act, setAct] = useState<Act | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAct = async () => {
      if (!id) {
        setError('Act ID is required');
        setIsLoading(false);
        return;
      }

      try {
        const data = await getAct(id);
        setAct(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load act');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAct();
  }, [id]);

  const handleSuccess = (updatedAct: Act) => {
    navigate('/acts');
  };

  const handleCancel = () => {
    navigate('/acts');
  };

  if (isLoading) {
    return (
      <Container>
        <div className="flex justify-center items-center min-h-[400px]">
          <Spinner size="lg" />
        </div>
      </Container>
    );
  }

  if (error || !act) {
    return (
      <Container>
        <Stack gap="xl">
          <PageHeader title="Edit Act" description="Update act information" />
          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">{error || 'Act not found'}</Text>
            </div>
          </Card>
        </Stack>
      </Container>
    );
  }

  return (
    <Container>
      <Stack gap="xl">
        <PageHeader
          title="Edit Act"
          description="Update act information"
        />
        <Card>
          <ActForm act={act} onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}
```

**States**:
1. Loading: Centered spinner
2. Error/Not Found: Error message in card
3. Success: ActForm with pre-filled data

---

### 6.4 ActDetailPage

**File**: `src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`

**Pattern**: Based on [`VenueDetailPage.tsx`](../src/GloboTicket.Web/src/pages/venues/VenueDetailPage.tsx:1)

**Purpose**: Display act details with management options

**Structure**:
```typescript
export const ActDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [act, setAct] = useState<Act | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAct = async () => {
      if (!id) {
        setError('Act ID is required');
        setIsLoading(false);
        return;
      }

      try {
        const data = await getAct(id);
        setAct(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load act');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAct();
  }, [id]);

  const handleDelete = async () => {
    if (!act) return;
    
    if (window.confirm(`Are you sure you want to delete "${act.name}"? This action cannot be undone.`)) {
      try {
        await deleteAct(act.actGuid);
        navigate('/acts');
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to delete act');
      }
    }
  };

  // Loading, error, and success states...

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button variant="ghost" onClick={() => navigate('/acts')}>
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Acts
      </Button>

      {/* Page Header with Icon */}
      <div className="flex items-start justify-between">
        <div className="flex items-start gap-4">
          <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
            <Music className="w-8 h-8 text-brand-primary" />
          </div>
          <div>
            <Heading level="h1" variant="default" className="mb-2">
              {act.name}
            </Heading>
            <Text variant="muted">
              Created {new Date(act.createdAt).toLocaleDateString()}
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => navigate(`/acts/${id}/edit`)}
          >
            <Edit className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button variant="danger" onClick={handleDelete}>
            <Trash2 className="w-4 h-4 mr-2" />
            Delete
          </Button>
        </Row>
      </div>

      {/* Act Information Card */}
      <Card header={<Heading level="h2">Act Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Name
            </Text>
            <Text>{act.name}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Created
            </Text>
            <Text>{new Date(act.createdAt).toLocaleString()}</Text>
          </div>
          {act.updatedAt && (
            <div>
              <Text variant="muted" size="sm" className="mb-1">
                Last Updated
              </Text>
              <Text>{new Date(act.updatedAt).toLocaleString()}</Text>
            </div>
          )}
        </Stack>
      </Card>

      {/* Upcoming Shows Card */}
      <Card header={<Heading level="h2">Upcoming Shows</Heading>}>
        <Text variant="muted">
          No upcoming shows scheduled for this act.
        </Text>
      </Card>
    </Stack>
  );
};
```

**Key Differences from VenueDetailPage**:
- Uses `Music` icon instead of `MapPin`
- Simpler information card (only name and timestamps)
- No address, capacity, or location coordinates
- Same delete confirmation pattern
- Same navigation and action buttons

---

## 7. Routing Configuration

### Update: `src/GloboTicket.Web/src/router/routes.ts`

Routes already defined in [`routes.ts`](../src/GloboTicket.Web/src/router/routes.ts:1):
```typescript
// Acts
ACTS: '/acts',
ACT_DETAIL: '/acts/:id',
ACT_CREATE: '/acts/new',
ACT_EDIT: '/acts/:id/edit',
```

Helper functions already defined:
```typescript
actDetail: (id: string) => `/acts/${id}`,
actEdit: (id: string) => `/acts/${id}/edit`,
```

### Update: `src/GloboTicket.Web/src/router/index.tsx`

Routes already configured in [`index.tsx`](../src/GloboTicket.Web/src/router/index.tsx:1) (lines 102-134):
```typescript
// Acts
{
  path: ROUTES.ACTS,
  element: (
    <ProtectedRoute>
      <ActsPage />
    </ProtectedRoute>
  ),
},
{
  path: ROUTES.ACT_CREATE,
  element: (
    <ProtectedRoute>
      <CreateActPage />
    </ProtectedRoute>
  ),
},
{
  path: ROUTES.ACT_DETAIL,
  element: (
    <ProtectedRoute>
      <ActDetailPage />
    </ProtectedRoute>
  ),
},
{
  path: ROUTES.ACT_EDIT,
  element: (
    <ProtectedRoute>
      <EditActPage />
    </ProtectedRoute>
  ),
},
```

**Note**: Routes are already configured but pages need to be created.

---

## 8. File Structure

### Required Files

```
src/GloboTicket.Web/src/
├── types/
│   └── act.ts                          # NEW - Act type definitions
├── api/
│   └── client.ts                       # UPDATE - Add act API functions
├── components/
│   ├── molecules/
│   │   ├── ActCard.tsx                 # NEW - Act card component
│   │   └── index.ts                    # UPDATE - Export ActCard
│   └── organisms/
│       ├── ActList.tsx                 # NEW - Act list component
│       ├── ActForm.tsx                 # NEW - Act form component
│       └── index.ts                    # UPDATE - Export ActList, ActForm
└── pages/
    └── acts/
        ├── ActsPage.tsx                # NEW - Main listing page
        ├── CreateActPage.tsx           # NEW - Create page
        ├── EditActPage.tsx             # NEW - Edit page
        ├── ActDetailPage.tsx           # NEW - Detail page
        └── index.ts                    # NEW - Export all act pages
```

### Export Barrel Files

**`src/GloboTicket.Web/src/components/molecules/index.ts`**:
```typescript
export * from './ActCard';
// ... existing exports
```

**`src/GloboTicket.Web/src/components/organisms/index.ts`**:
```typescript
export * from './ActList';
export * from './ActForm';
// ... existing exports
```

**`src/GloboTicket.Web/src/pages/acts/index.ts`** (NEW):
```typescript
export * from './ActsPage';
export * from './CreateActPage';
export * from './EditActPage';
export * from './ActDetailPage';
```

---

## 9. State Management Patterns

### Pattern from Venue Implementation

**No global state management** - Uses local component state with React hooks:

1. **Data Fetching**: `useState` + `useEffect` pattern
2. **Loading States**: Boolean `isLoading` flag
3. **Error Handling**: String `error` state
4. **Form State**: Individual `useState` for each field

**Example Pattern**:
```typescript
const [data, setData] = useState<Act[]>([]);
const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);

useEffect(() => {
  const fetchData = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = await getActs();
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load');
    } finally {
      setIsLoading(false);
    }
  };

  fetchData();
}, []);
```

---

## 10. Error Handling Patterns

### From Venue Implementation

1. **API Errors**: Caught in try-catch, displayed in UI
2. **Validation Errors**: Inline form validation with error messages
3. **Not Found**: Dedicated error state with helpful message
4. **Network Errors**: Generic error message with retry option

**Error Display Pattern**:
```typescript
{error && (
  <div className="p-4 rounded-lg bg-error/10 border border-error/20">
    <Text size="sm" className="text-error">
      {error}
    </Text>
  </div>
)}
```

---

## 11. Accessibility Patterns

### From Venue Implementation

1. **Keyboard Navigation**: Cards support Enter/Space keys
2. **ARIA Labels**: Buttons have descriptive `aria-label`
3. **Focus Management**: Proper focus indicators
4. **Semantic HTML**: Proper heading hierarchy (h1 → h2 → h3)
5. **Form Labels**: All inputs have associated labels
6. **Loading States**: Spinner has `label` prop for screen readers

---

## 12. Responsive Design Patterns

### Grid Breakpoints (from VenueList)

```typescript
<Grid
  cols={1}
  gap="lg"
  responsive={{ sm: 1, md: 2, lg: 3 }}
>
```

- **Mobile** (default): 1 column
- **Tablet** (md): 2 columns
- **Desktop** (lg): 3 columns

---

## 13. Icon Usage

### Icons from lucide-react

**Acts-specific**:
- `Music` - Primary act icon (replaces `MapPin` from venues)
- `Plus` - Add new act button
- `Edit` - Edit action
- `Trash2` - Delete action
- `ArrowLeft` - Back navigation
- `AlertCircle` - Error states

---

## 14. Implementation Checklist

### Phase 1: Types and API
- [ ] Create `src/GloboTicket.Web/src/types/act.ts`
- [ ] Update `src/GloboTicket.Web/src/api/client.ts` with act functions

### Phase 2: Components (Molecules)
- [ ] Create `src/GloboTicket.Web/src/components/molecules/ActCard.tsx`
- [ ] Update `src/GloboTicket.Web/src/components/molecules/index.ts`

### Phase 3: Components (Organisms)
- [ ] Create `src/GloboTicket.Web/src/components/organisms/ActList.tsx`
- [ ] Create `src/GloboTicket.Web/src/components/organisms/ActForm.tsx`
- [ ] Update `src/GloboTicket.Web/src/components/organisms/index.ts`

### Phase 4: Pages
- [ ] Create `src/GloboTicket.Web/src/pages/acts/` directory
- [ ] Create `src/GloboTicket.Web/src/pages/acts/ActsPage.tsx`
- [ ] Create `src/GloboTicket.Web/src/pages/acts/CreateActPage.tsx`
- [ ] Create `src/GloboTicket.Web/src/pages/acts/EditActPage.tsx`
- [ ] Create `src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`
- [ ] Create `src/GloboTicket.Web/src/pages/acts/index.ts`

### Phase 5: Integration
- [ ] Update `src/GloboTicket.Web/src/router/index.tsx` imports
- [ ] Test all CRUD operations
- [ ] Verify routing works correctly
- [ ] Test responsive behavior
- [ ] Verify accessibility

---

## 15. Testing Considerations

### Manual Testing Checklist

1. **ActsPage**:
   - [ ] Displays loading state
   - [ ] Displays empty state when no acts
   - [ ] Displays acts in responsive grid
   - [ ] "Add Act" button navigates to create page
   - [ ] Clicking act card navigates to detail page

2. **CreateActPage**:
   - [ ] Form validates required name field
   - [ ] Form validates max length (100 chars)
   - [ ] Success creates act and navigates to list
   - [ ] Cancel button navigates back to list
   - [ ] Error messages display correctly

3. **EditActPage**:
   - [ ] Loads existing act data
   - [ ] Pre-fills form with act name
   - [ ] Updates act successfully
   - [ ] Handles not found errors
   - [ ] Cancel navigates back to list

4. **ActDetailPage**:
   - [ ] Displays act information
   - [ ] Edit button navigates to edit page
   - [ ] Delete button shows confirmation
   - [ ] Delete removes act and navigates to list
   - [ ] Back button navigates to list
   - [ ] Handles not found errors

---

## 16. Key Simplifications from Venue

The Act implementation is significantly simpler than Venue:

1. **Single Field Form**: Only name field vs. 7 fields for venue
2. **No Geographic Data**: No map, coordinates, or location handling
3. **No Capacity Info**: No seating capacity or venue-specific metadata
4. **Simpler Validation**: Only name length validation
5. **Simpler Card**: Just name and date vs. multiple data points
6. **Faster Implementation**: Estimated 50% less code than venue

---

## 17. Reusable Patterns

These patterns from Venue can be directly reused:

1. **Page Structure**: Container → Stack → PageHeader + Content
2. **Form Pattern**: State management, validation, submit/cancel
3. **List Pattern**: Loading/Error/Empty/Success states
4. **Card Pattern**: Interactive cards with onClick
5. **Detail Page**: Header with actions, info cards, related data
6. **API Integration**: Fetch pattern with error handling
7. **Navigation**: useNavigate for programmatic navigation
8. **Routing**: Protected routes with ProtectedRoute wrapper

---

## Summary

This specification provides a complete blueprint for implementing Acts pages by following the established Venue patterns. The implementation will be simpler due to the Act entity's minimal complexity (single name field vs. venue's 7 fields), but will maintain the same architectural principles, atomic design structure, and user experience patterns.

The key to success is **consistency**: every component, page, and pattern should mirror the Venue implementation while adapting to the simpler Act entity structure.