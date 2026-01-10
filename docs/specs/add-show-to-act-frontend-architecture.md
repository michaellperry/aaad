# Add Show to Act - Frontend Architecture

**Status**: Draft  
**Created**: 2026-01-03  
**Author**: Frontend Architect  
**Related Documents**:
- Technical Specification: [`docs/specs/add-show-to-act.md`](add-show-to-act.md)
- User Story: [`docs/user-stories/add-show-to-act.md`](../user-stories/add-show-to-act.md)
- Design System: [`docs/design-system-architecture.md`](../design-system-architecture.md)
- Atomic Design: [`docs/atomic-design.md`](../atomic-design.md)

---

## Executive Summary

This document defines the frontend architecture for the "Add Show to Act" feature, which enables event organizers to create shows (performances) for acts at specific venues. The implementation follows the established GloboTicket design system principles with Atomic Design, React Router for navigation, and TypeScript for type safety.

**Key Components:**
1. **CreateShowPage** - New page component for show creation
2. **ShowForm** - New organism component with venue selection, capacity display, and nearby shows detection
3. **ActDetailPage Enhancement** - Add "Add Show" button and display shows list
4. **API Client Functions** - `createShow()`, `getVenues()`, and `getNearbyShows()` functions
5. **Routing Configuration** - New route for `/acts/:id/shows/new`

**Architecture Principles:**
- Follow existing patterns from [`ActForm`](../../src/GloboTicket.Web/src/components/organisms/ActForm.tsx) and [`VenueForm`](../../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx)
- Use Atomic Design with existing atoms and molecules
- Implement client-side validation matching server-side rules
- Handle timezone conversion for DateTimeOffset
- Display venue capacity and nearby shows dynamically
- Maintain accessibility and responsive design standards

---

## 1. Component Architecture

### 1.1 Component Hierarchy

Following Atomic Design principles, the Add Show to Act feature uses existing atoms and molecules, with new page and organism components:

```
CreateShowPage (Page - New)
├── Stack (Layout Primitive)
│   ├── Button (Atom - Back button)
│   ├── PageHeader (Molecule - "Add Show" title)
│   └── Card (Molecule - Form container)
│       └── ShowForm (Organism - New)
│           ├── Dropdown (Molecule - Venue selection)
│           ├── Text (Atom - Venue capacity display)
│           ├── Input (Native - Ticket count)
│           ├── Input (Native - Start date)
│           ├── Input (Native - Start time)
│           ├── Card (Molecule - Nearby shows section)
│           │   └── Stack (Layout Primitive)
│           │       └── Text (Atom - Nearby show items)
│           └── Button Group
│               ├── Button (Atom - Create Show)
│               └── Button (Atom - Cancel)

ActDetailPage (Page - Enhanced)
├── Stack (Layout Primitive)
│   ├── ... (Existing sections)
│   └── Card (Molecule - Shows section)
│       ├── Heading (Atom - "Upcoming Shows")
│       ├── Button (Atom - "Add Show")
│       └── Stack (Layout Primitive)
│           └── ShowCard (Molecule - Existing, clickable)
│               ├── Text (Atom - Venue name)
│               ├── Text (Atom - Start date/time)
│               └── Text (Atom - Ticket count)
```

### 1.2 New Components

#### CreateShowPage

**Location**: [`src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx)

**Purpose**: Container page for show creation, providing layout and navigation context.

**Component Structure**:
```tsx
export const CreateShowPage = () => {
  const { id } = useParams<{ id: string }>(); // Act GUID
  const navigate = useNavigate();
  const [act, setAct] = useState<Act | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch act data to display act name
  useEffect(() => {
    // Fetch act using getAct(id)
  }, [id]);

  const handleSuccess = (show: Show) => {
    navigate(`/acts/${id}`);
  };

  const handleCancel = () => {
    navigate(`/acts/${id}`);
  };

  // Render states: loading, error, success
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ ← Back to Act                                       │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Add Show for The Rolling Stones                   │
│                                                     │
├─────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────┐│
│ │ Venue *                                         ││
│ │ [Select venue...                            ▼] ││
│ │                                                 ││
│ │ Venue Capacity: 20,000 seats                   ││
│ │                                                 ││
│ │ Number of Tickets *                            ││
│ │ [Enter ticket count                          ] ││
│ │                                                 ││
│ │ Start Date *                                   ││
│ │ [MM/DD/YYYY                                  ] ││
│ │                                                 ││
│ │ Start Time *                                   ││
│ │ [HH:MM                                       ] ││
│ │                                                 ││
│ │ Other Shows at This Venue                      ││
│ │ ┌───────────────────────────────────────────┐ ││
│ │ │ The Beatles                               │ ││
│ │ │ March 13, 2026 at 8:00 PM                │ ││
│ │ └───────────────────────────────────────────┘ ││
│ │                                                 ││
│ │ [Create Show]  [Cancel]                        ││
│ └─────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘
```

**Props**: None (uses URL params)

**State**:
- `act: Act | null` - Loaded act data for display
- `isLoading: boolean` - Loading state during act fetch
- `error: string | null` - Error message if act fetch fails

**Behavior**:
- Fetches act data on mount to display act name in header
- Delegates form logic to ShowForm component
- Handles navigation on success/cancel
- Displays loading spinner while fetching act
- Displays error message if act not found

**Accessibility**:
- Page title uses semantic `<h1>` heading
- Loading state announced via `aria-live` region
- Error messages announced via `aria-live` region
- Back button accessible via keyboard

**Responsive Behavior**:
- Desktop (>1024px): Full-width layout with comfortable spacing
- Tablet (768-1024px): Same as desktop
- Mobile (<768px): Single column, stacked layout, reduced spacing

#### ShowForm (Organism - New)

**Location**: [`src/GloboTicket.Web/src/components/organisms/ShowForm.tsx`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx)

**Purpose**: Form component for creating shows with venue capacity display and nearby shows detection.

**Component Structure**:
```tsx
interface ShowFormProps {
  actGuid: string;
  actName: string;
  onSuccess?: (show: Show) => void;
  onCancel?: () => void;
}

export const ShowForm = ({ actGuid, actName, onSuccess, onCancel }: ShowFormProps) => {
  // Form state
  const [venueGuid, setVenueGuid] = useState('');
  const [ticketCount, setTicketCount] = useState('');
  const [startDate, setStartDate] = useState('');
  const [startTime, setStartTime] = useState('');
  
  // UI state
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [isLoading, setIsLoading] = useState(false);
  
  // Data state
  const [venues, setVenues] = useState<Venue[]>([]);
  const [isLoadingVenues, setIsLoadingVenues] = useState(true);
  const [nearbyShows, setNearbyShows] = useState<NearbyShow[]>([]);
  const [isLoadingNearbyShows, setIsLoadingNearbyShows] = useState(false);
  
  // Computed values
  const selectedVenue = venues.find(v => v.venueGuid === venueGuid);
  const venueCapacity = selectedVenue?.seatingCapacity || 0;
  
  // Effects
  useEffect(() => {
    // Fetch venues on mount
  }, []);
  
  useEffect(() => {
    // Fetch nearby shows when venue, date, and time all change
  }, [venueGuid, startDate, startTime]);
  
  // Handlers
  const handleSubmit = async () => {
    // Validate and submit
  };
  
  // Render form
};
```

**Props**:
- `actGuid: string` - GUID of the act to create show for
- `actName: string` - Name of the act (for display)
- `onSuccess?: (show: Show) => void` - Callback on successful submission
- `onCancel?: () => void` - Callback on cancel

**State**:
- **Form Fields**:
  - `venueGuid: string` - Selected venue GUID
  - `ticketCount: string` - Ticket count input (string for validation)
  - `startDate: string` - Start date (YYYY-MM-DD format)
  - `startTime: string` - Start time (HH:MM format)
- **UI State**:
  - `error: string | null` - General error message
  - `fieldErrors: Record<string, string>` - Per-field validation errors
  - `isLoading: boolean` - Loading state during submission
- **Data State**:
  - `venues: Venue[]` - List of available venues
  - `isLoadingVenues: boolean` - Loading state for venues
  - `nearbyShows: NearbyShow[]` - Nearby shows data
  - `isLoadingNearbyShows: boolean` - Loading state for nearby shows

**Form Fields**:

1. **Venue** (Dropdown)
   - Label: "Venue *"
   - Component: [`Dropdown`](../../src/GloboTicket.Web/src/components/molecules/Dropdown.tsx) molecule
   - Options: List of venues from current tenant
   - On change: Update displayed capacity, fetch nearby shows
   - If no venues exist: Display message "No venues available. Please create a venue first."

2. **Venue Capacity Display** (Read-only)
   - Label: "Venue Capacity"
   - Displays: "{capacity} seats" when venue selected
   - Position: Below venue dropdown, prominently styled
   - Component: [`Text`](../../src/GloboTicket.Web/src/components/atoms/Text.tsx) atom with variant="muted"

3. **Ticket Count** (Number input)
   - Label: "Number of Tickets *"
   - Type: `<input type="number">`
   - Placeholder: "Enter ticket count"
   - min: 1
   - max: dynamically set to venue capacity
   - Validation tied to selected venue capacity

4. **Start Date** (Date input)
   - Label: "Start Date *"
   - Type: `<input type="date">`
   - min: tomorrow's date (calculated dynamically)
   - On change: Re-fetch nearby shows if venue selected

5. **Start Time** (Time input)
   - Label: "Start Time *"
   - Type: `<input type="time">`
   - Format: HH:MM (24-hour or 12-hour based on browser locale)
   - On change: Re-fetch nearby shows if venue and date selected

6. **Nearby Shows Display** (Informational section)
   - Label: "Other Shows at This Venue"
   - Component: [`Card`](../../src/GloboTicket.Web/src/components/molecules/Card.tsx) molecule
   - Appears when venue, date, and time are all selected
   - Shows loading state while fetching
   - Displays list of nearby shows with: Act name, date, time
   - If no nearby shows: "No other shows scheduled at this venue within 48 hours"
   - Sorted chronologically

**Buttons**:
- **Create Show** (primary): Submits the form
- **Cancel** (secondary): Returns to act detail page

**Validation Rules** (Client-Side):

| Field | Rule | Error Message |
|-------|------|---------------|
| Venue | Required | "Please select a venue" |
| Ticket Count | Required | "Ticket count is required" |
| Ticket Count | Must be positive integer | "Ticket count must be a positive number" |
| Ticket Count | Must be <= venue capacity | "Ticket count cannot exceed venue capacity of {capacity}" |
| Start Date | Required | "Start date is required" |
| Start Date | Must be in the future | "Start date must be in the future" |
| Start Time | Required | "Start time is required" |
| Start Time | Valid HH:MM format | "Please enter a valid time" |

**Behavior**:
- Fetches venues on mount using `getVenues()`
- Displays venue capacity when venue selected
- Fetches nearby shows when venue, date, and time all provided
- Validates all fields on submit
- Displays inline errors for each field
- Displays general error for API failures
- Calls `onSuccess` callback on successful creation
- Calls `onCancel` callback on cancel button click

**Accessibility**:
- All form inputs have associated `<label>` elements with `htmlFor` attribute
- Required fields marked with asterisk (*) in label text
- Venue capacity announced to screen readers when venue selection changes
- Nearby shows section uses appropriate ARIA live region for updates
- Error messages announced via ARIA live regions
- Form inputs support keyboard navigation (Tab/Shift+Tab)
- Date and time inputs use native browser controls for accessibility
- Disabled state on submit button communicated via `aria-disabled`
- Loading states have appropriate ARIA labels

**Responsive Behavior**:
- Desktop (>1024px): Two-column layout for date/time fields; venue capacity inline
- Tablet (768-1024px): Same as desktop
- Mobile (<768px): Single column, stacked layout; venue capacity below dropdown

### 1.3 Enhanced Components

#### ActDetailPage Enhancement

**Location**: [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx)

**Changes Required**:
1. Add "Add Show" button in the "Upcoming Shows" card header
2. Shows list already implemented (from View Show feature)
3. Add navigation handler for "Add Show" button

**Enhanced Shows Section**:
```tsx
{/* Upcoming Shows */}
<Card 
  header={
    <div className="flex justify-between items-center">
      <Heading level="h2">Upcoming Shows</Heading>
      <Button
        variant="primary"
        size="sm"
        onClick={() => navigate(`/acts/${act.actGuid}/shows/new`)}
      >
        Add Show
      </Button>
    </div>
  }
>
  {isLoadingShows ? (
    <div className="flex justify-center p-8">
      <Spinner size="md" />
    </div>
  ) : shows.length === 0 ? (
    <Text variant="muted">
      No upcoming shows scheduled for this act.
    </Text>
  ) : (
    <Stack gap="md">
      {shows.map((show) => (
        <ShowCard
          key={show.showGuid}
          show={show}
          onClick={() => navigate(`/shows/${show.showGuid}`)}
        />
      ))}
    </Stack>
  )}
</Card>
```

---

## 2. State Management

### 2.1 TanStack Query Integration

**Status**: ✅ **IMPLEMENTED** - The GloboTicket frontend now uses TanStack Query for all server state management across all features (shows, venues, acts).

The Add Show to Act feature uses TanStack Query hooks for data fetching, following the project-wide migration completed in January 2026.

**CreateShowPage State**:
```typescript
const [act, setAct] = useState<Act | null>(null);
const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);
```

**ShowForm State**:
```typescript
// Form fields
const [venueGuid, setVenueGuid] = useState('');
const [ticketCount, setTicketCount] = useState('');
const [startDate, setStartDate] = useState('');
const [startTime, setStartTime] = useState('');

// UI state
const [error, setError] = useState<string | null>(null);
const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
const [isLoading, setIsLoading] = useState(false);

// Data state
const [venues, setVenues] = useState<Venue[]>([]);
const [isLoadingVenues, setIsLoadingVenues] = useState(true);
const [nearbyShows, setNearbyShows] = useState<NearbyShow[]>([]);
const [isLoadingNearbyShows, setIsLoadingNearbyShows] = useState(false);
```

### 2.2 Data Fetching Pattern

**Pattern**: Fetch data in `useEffect` on component mount or when dependencies change, following existing conventions.

**CreateShowPage Data Fetching**:
```typescript
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
```

**ShowForm Venues Fetching**:
```typescript
useEffect(() => {
  const fetchVenues = async () => {
    try {
      const data = await getVenues();
      setVenues(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load venues');
    } finally {
      setIsLoadingVenues(false);
    }
  };

  fetchVenues();
}, []);
```

**ShowForm Nearby Shows Fetching**:
```typescript
useEffect(() => {
  const fetchNearbyShows = async () => {
    // Only fetch if all required fields are present
    if (!venueGuid || !startDate || !startTime) {
      setNearbyShows([]);
      return;
    }

    setIsLoadingNearbyShows(true);
    try {
      // Construct ISO 8601 datetime with timezone offset
      const dateTime = constructDateTime(startDate, startTime);
      const data = await getNearbyShows(venueGuid, dateTime);
      setNearbyShows(data.shows);
    } catch (err) {
      // Don't show error for nearby shows - it's informational only
      console.error('Failed to load nearby shows:', err);
      setNearbyShows([]);
    } finally {
      setIsLoadingNearbyShows(false);
    }
  };

  fetchNearbyShows();
}, [venueGuid, startDate, startTime]);
```

### 2.3 Form Submission Pattern

**Pattern**: Validate fields, call API, handle response, following existing form patterns.

**ShowForm Submission**:
```typescript
const handleSubmit = async () => {
  setError(null);
  setFieldErrors({});

  // Validate all fields
  const errors: Record<string, string> = {};
  
  if (!venueGuid) {
    errors.venue = 'Please select a venue';
  }
  
  if (!ticketCount) {
    errors.ticketCount = 'Ticket count is required';
  } else if (parseInt(ticketCount) <= 0) {
    errors.ticketCount = 'Ticket count must be a positive number';
  } else if (venueCapacity > 0 && parseInt(ticketCount) > venueCapacity) {
    errors.ticketCount = `Ticket count cannot exceed venue capacity of ${venueCapacity}`;
  }
  
  if (!startDate) {
    errors.startDate = 'Start date is required';
  } else {
    const selectedDate = new Date(startDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (selectedDate <= today) {
      errors.startDate = 'Start date must be in the future';
    }
  }
  
  if (!startTime) {
    errors.startTime = 'Start time is required';
  }
  
  if (Object.keys(errors).length > 0) {
    setFieldErrors(errors);
    return;
  }

  setIsLoading(true);

  try {
    // Construct ISO 8601 datetime with timezone offset
    const startTimeISO = constructDateTime(startDate, startTime);
    
    const dto: CreateShowDto = {
      showGuid: crypto.randomUUID(),
      venueGuid,
      ticketCount: parseInt(ticketCount),
      startTime: startTimeISO,
    };

    const newShow = await createShow(actGuid, dto);

    if (onSuccess) {
      onSuccess(newShow);
    }
  } catch (err) {
    setError(err instanceof Error ? err.message : 'Failed to create show');
  } finally {
    setIsLoading(false);
  }
};
```

### 2.4 Query Hooks for Data Fetching

**Available Hooks**:
- [`useAct(id)`](../../src/GloboTicket.Web/src/features/acts/hooks/useAct.ts) - Fetch act details
- [`useVenues()`](../../src/GloboTicket.Web/src/features/venues/hooks/useVenues.ts) - Fetch all venues

**Form State Management**:
- Form fields (venueGuid, ticketCount, startDate, startTime) still use `useState` for local form state
- Server data fetching uses TanStack Query hooks
- Form submission uses mutation hooks for optimistic updates and cache invalidation

**Benefits**:
- Automatic caching of venues list
- Background refetching keeps venue data fresh
- Optimistic updates for better UX
- Automatic retry on failure

---

## 3. API Integration

### 3.1 API Client Functions

**Location**: [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)

Following the existing pattern from [`createAct()`](../../src/GloboTicket.Web/src/api/client.ts:154) and [`createVenue()`](../../src/GloboTicket.Web/src/api/client.ts:100), add new functions:

#### createShow()

**Purpose**: Create a new show for an act.

**Implementation**:
```typescript
export async function createShow(actGuid: string, dto: CreateShowDto): Promise<Show> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${actGuid}/shows`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<Show>(response);
}
```

**Parameters**:
- `actGuid: string` - Act GUID
- `dto: CreateShowDto` - Show creation data

**Returns**: `Promise<Show>` - Created show data

**Error Handling**:
- 400 Bad Request: Validation errors (ticket count exceeds capacity, start time in past)
- 401 Unauthorized: Throws error, handled by component
- 404 Not Found: Act or Venue not found
- 429 Rate Limit: Throws error with rate limit message
- 500 Server Error: Throws error with generic message

#### getNearbyShows()

**Purpose**: Fetch nearby shows at a venue within 48 hours of a proposed start time.

**Implementation**:
```typescript
export async function getNearbyShows(venueGuid: string, startTime: string): Promise<NearbyShowsResponse> {
  const response = await fetch(
    `${API_BASE_URL}/api/venues/${venueGuid}/shows/nearby?startTime=${encodeURIComponent(startTime)}`,
    {
      method: 'GET',
      credentials: 'include',
    }
  );
  return handleResponse<NearbyShowsResponse>(response);
}
```

**Parameters**:
- `venueGuid: string` - Venue GUID
- `startTime: string` - Proposed start time in ISO 8601 format with timezone offset

**Returns**: `Promise<NearbyShowsResponse>` - Nearby shows data

**Error Handling**:
- 400 Bad Request: Invalid startTime parameter
- 401 Unauthorized: Throws error, handled by component
- 404 Not Found: Venue not found
- 500 Server Error: Throws error with generic message

**Note**: Errors in nearby shows fetch are logged but not displayed to user, as this is informational only.

#### getVenues() (Already Exists)

**Purpose**: Fetch all venues for the current tenant.

**Implementation**: Already exists in [`client.ts:84`](../../src/GloboTicket.Web/src/api/client.ts:84)

```typescript
export async function getVenues(): Promise<Venue[]> {
  const response = await fetch(`${API_BASE_URL}/api/venues`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Venue[]>(response);
}
```

### 3.2 DateTime Construction Utility

**Purpose**: Construct ISO 8601 datetime string with timezone offset from date and time inputs.

**Location**: [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts)

**Implementation**:
```typescript
/**
 * Construct ISO 8601 datetime string with timezone offset
 * @param date - Date string in YYYY-MM-DD format
 * @param time - Time string in HH:MM format
 * @returns ISO 8601 datetime string with timezone offset (e.g., "2025-07-15T20:00:00-04:00")
 */
export function constructDateTime(date: string, time: string): string {
  // Combine date and time
  const dateTimeString = `${date}T${time}:00`;
  
  // Create Date object (will use local timezone)
  const dateTime = new Date(dateTimeString);
  
  // Get timezone offset in minutes
  const offsetMinutes = dateTime.getTimezoneOffset();
  
  // Convert offset to hours and minutes
  const offsetHours = Math.floor(Math.abs(offsetMinutes) / 60);
  const offsetMins = Math.abs(offsetMinutes) % 60;
  
  // Format offset as +/-HH:MM
  const offsetSign = offsetMinutes <= 0 ? '+' : '-';
  const offsetString = `${offsetSign}${String(offsetHours).padStart(2, '0')}:${String(offsetMins).padStart(2, '0')}`;
  
  // Construct ISO 8601 string with offset
  return `${dateTimeString}${offsetString}`;
}
```

**Usage**:
```typescript
const startTimeISO = constructDateTime('2025-07-15', '20:00');
// Result: "2025-07-15T20:00:00-04:00" (if user is in EDT timezone)
```

**Rationale**:
- Uses browser's local timezone for offset calculation
- Matches user's expectation of "local time" for venue
- Server stores as `DateTimeOffset` preserving the offset
- Future enhancement: Use venue's timezone instead of browser timezone

### 3.3 API Response Handling

**Existing Pattern**: The [`handleResponse()`](../../src/GloboTicket.Web/src/api/client.ts:13) utility function handles all API responses consistently:

```typescript
async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }
  return response.json();
}
```

**Benefits**:
- Consistent error handling across all API calls
- Automatic JSON parsing
- Type-safe responses via generics
- Clear error messages for debugging

### 3.4 Authentication

**Pattern**: All API calls use `credentials: 'include'` to send authentication cookies.

**Cookie-Based Auth**:
- Cookie name: `.GloboTicket.Auth`
- Secure, HTTP-only, SameSite=Strict
- Automatically included in all API requests
- Managed by backend authentication middleware

**Unauthenticated Requests**:
- Backend returns 401 Unauthorized
- Frontend catches error and redirects to login (handled by [`ProtectedRoute`](../../src/GloboTicket.Web/src/components/routing/ProtectedRoute.tsx))

---

## 4. Routing Configuration

### 4.1 Route Definitions

**Location**: [`src/GloboTicket.Web/src/router/routes.ts`](../../src/GloboTicket.Web/src/router/routes.ts)

Add new route constants for show creation:

```typescript
export const ROUTES = {
  // ... existing routes
  
  // Shows
  SHOWS: '/shows',
  SHOW_DETAIL: '/shows/:id',
  SHOW_CREATE: '/acts/:id/shows/new', // New route
} as const;
```

Add route helper function:

```typescript
export const routeHelpers = {
  // ... existing helpers
  
  showCreate: (actId: string) => `/acts/${actId}/shows/new`,
};
```

### 4.2 Router Configuration

**Location**: [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx)

Add new route for show creation page:

```typescript
import { CreateShowPage } from '../pages/shows';

export const router = createBrowserRouter([
  // ... existing routes
  
  {
    path: '/',
    element: <AppLayout />,
    children: [
      // ... existing children
      
      // Shows
      {
        path: ROUTES.SHOW_CREATE,
        element: (
          <ProtectedRoute>
            <CreateShowPage />
          </ProtectedRoute>
        ),
      },
    ],
  },
]);
```

### 4.3 Navigation Flow

**Primary Navigation Path**:
1. User navigates to Acts page (`/acts`)
2. User clicks on an act to view details (`/acts/:actId`)
3. Act detail page displays "Add Show" button
4. User clicks "Add Show" button
5. Navigate to create show page (`/acts/:actId/shows/new`)
6. User fills out form and submits
7. Navigate back to act detail page (`/acts/:actId`)

**Cancel Navigation**:
- Create show page has "Cancel" button
- Clicking cancel button navigates to `/acts/:actId`
- Browser back button also works (React Router history)

**URL Structure**:
- Acts list: `/acts`
- Act detail: `/acts/{actGuid}`
- Create show: `/acts/{actGuid}/shows/new`
- Show detail: `/shows/{showGuid}`

**Navigation Implementation**:
```typescript
// In ActDetailPage component
const handleAddShow = () => navigate(`/acts/${act.actGuid}/shows/new`);

// In CreateShowPage component
const handleSuccess = () => navigate(`/acts/${actGuid}`);
const handleCancel = () => navigate(`/acts/${actGuid}`);
```

### 4.4 Protected Routes

All show routes are wrapped in [`ProtectedRoute`](../../src/GloboTicket.Web/src/components/routing/ProtectedRoute.tsx) component, which:
- Checks authentication status via [`AuthContext`](../../src/GloboTicket.Web/src/contexts/AuthContext.tsx)
- Redirects to login page if not authenticated
- Preserves intended destination for post-login redirect

---

## 5. TypeScript Interfaces

### 5.1 CreateShowDto Interface

**Location**: [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)

Following the pattern from [`CreateActDto`](../../src/GloboTicket.Web/src/types/act.ts:18) and [`CreateVenueDto`](../../src/GloboTicket.Web/src/types/venue.ts:18):

```typescript
/**
 * CreateShowDto interface matching the backend CreateShowDto
 */
export interface CreateShowDto {
  /** Client-generated unique identifier for the show */
  showGuid: string;
  
  /** GUID of the venue where the show will be held */
  venueGuid: string;
  
  /** Number of tickets available (must be <= venue capacity) */
  ticketCount: number;
  
  /** Show start time in ISO 8601 format with timezone offset */
  startTime: string;
}
```

### 5.2 NearbyShow and NearbyShowsResponse Interfaces

**Location**: [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)

```typescript
/**
 * NearbyShow interface for shows within 48 hours at the same venue
 */
export interface NearbyShow {
  /** Unique GUID identifier for the show */
  showGuid: string;
  
  /** Name of the act performing */
  actName: string;
  
  /** Show start time with timezone offset (ISO 8601 format) */
  startTime: string;
}

/**
 * NearbyShowsResponse interface for the nearby shows API response
 */
export interface NearbyShowsResponse {
  /** GUID of the venue */
  venueGuid: string;
  
  /** Name of the venue */
  venueName: string;
  
  /** The time around which nearby shows were searched */
  referenceTime: string;
  
  /** List of nearby shows */
  shows: NearbyShow[];
  
  /** Informational message (e.g., "No other shows scheduled at this venue within 48 hours") */
  message: string;
}
```

### 5.3 Type Exports

**Location**: [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)

```typescript
export type { Show, CreateShowDto, NearbyShow, NearbyShowsResponse };
```

**Usage in Components**:
```typescript
import type { Show, CreateShowDto, NearbyShow, NearbyShowsResponse } from '../../types/show';

const [nearbyShows, setNearbyShows] = useState<NearbyShow[]>([]);
```

### 5.4 Type Safety Benefits

**Compile-Time Checks**:
- Ensures correct property access
- Prevents typos in property names
- Validates data structure from API
- Provides IntelliSense in IDE

**Runtime Safety**:
- TypeScript interfaces are erased at runtime
- API response validation happens in `handleResponse()`
- Type assertions used when necessary: `as Show`

---

## 6. Validation Strategy

### 6.1 Client-Side Validation Rules

**Single Source of Truth**: Client-side validation rules match server-side validation rules defined in [`add-show-to-act.md:573`](add-show-to-act.md:573).

**Validation Rules Table**:

| Field | Rule | Error Message | Validation Timing |
|-------|------|---------------|-------------------|
| Venue | Required | "Please select a venue" | On submit |
| Ticket Count | Required | "Ticket count is required" | On submit |
| Ticket Count | Must be positive integer | "Ticket count must be a positive number" | On submit |
| Ticket Count | Must be <= venue capacity | "Ticket count cannot exceed venue capacity of {capacity}" | On submit |
| Start Date | Required | "Start date is required" | On submit |
| Start Date | Must be in the future | "Start date must be in the future" | On submit |
| Start Time | Required | "Start time is required" | On submit |
| Start Time | Valid HH:MM format | "Please enter a valid time" | On submit |

**Validation Implementation**:
```typescript
const validateForm = (): Record<string, string> => {
  const errors: Record<string, string> = {};
  
  // Venue validation
  if (!venueGuid) {
    errors.venue = 'Please select a venue';
  }
  
  // Ticket count validation
  if (!ticketCount) {
    errors.ticketCount = 'Ticket count is required';
  } else {
    const count = parseInt(ticketCount);
    if (isNaN(count) || count <= 0) {
      errors.ticketCount = 'Ticket count must be a positive number';
    } else if (venueCapacity > 0 && count > venueCapacity) {
      errors.ticketCount = `Ticket count cannot exceed venue capacity of ${venueCapacity.toLocaleString()}`;
    }
  }
  
  // Start date validation
  if (!startDate) {
    errors.startDate = 'Start date is required';
  } else {
    const selectedDate = new Date(startDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (selectedDate <= today) {
      errors.startDate = 'Start date must be in the future';
    }
  }
  
  // Start time validation
  if (!startTime) {
    errors.startTime = 'Start time is required';
  } else if (!/^\d{2}:\d{2}$/.test(startTime)) {
    errors.startTime = 'Please enter a valid time';
  }
  
  return errors;
};
```

### 6.2 Server-Side Validation

**Server-Side Re-Validation**: All validation rules are re-validated on the server to ensure data integrity.

**Additional Server-Side Checks**:
- Venue exists and belongs to current tenant
- Act exists and belongs to current tenant
- Venue capacity hasn't changed since form load
- Start time is still in the future (accounting for submission delay)

**Server Error Handling**:
```typescript
try {
  const newShow = await createShow(actGuid, dto);
  // Success
} catch (err) {
  // Parse server error response
  const errorMessage = err instanceof Error ? err.message : 'Failed to create show';
  
  // Check for specific validation errors
  if (errorMessage.includes('capacity')) {
    setFieldErrors({ ticketCount: errorMessage });
  } else if (errorMessage.includes('past')) {
    setFieldErrors({ startDate: errorMessage });
  } else {
    setError(errorMessage);
  }
}
```

### 6.3 Real-Time Validation

**Capacity Check**: Real-time validation when ticket count changes and venue is selected.

```typescript
const handleTicketCountChange = (value: string) => {
  setTicketCount(value);
  
  // Clear previous error
  const newErrors = { ...fieldErrors };
  delete newErrors.ticketCount;
  
  // Validate against capacity if venue selected
  if (venueCapacity > 0 && value) {
    const count = parseInt(value);
    if (!isNaN(count) && count > venueCapacity) {
      newErrors.ticketCount = `Ticket count cannot exceed venue capacity of ${venueCapacity.toLocaleString()}`;
    }
  }
  
  setFieldErrors(newErrors);
};
```

### 6.4 Error Display Pattern

**Field-Level Errors**:
```tsx
<div>
  <label htmlFor="ticketCount" className="block text-sm font-medium text-text-primary mb-2">
    Number of Tickets *
  </label>
  <input
    type="number"
    id="ticketCount"
    value={ticketCount}
    onChange={(e) => handleTicketCountChange(e.target.value)}
    className={`w-full px-4 py-2 rounded-lg border ${
      fieldErrors.ticketCount ? 'border-error' : 'border-border-default'
    } bg-surface-base text-text-primary`}
    min="1"
    max={venueCapacity || undefined}
  />
  {fieldErrors.ticketCount && (
    <Text size="sm" className="text-error mt-1">
      {fieldErrors.ticketCount}
    </Text>
  )}
</div>
```

**General Error Display**:
```tsx
{error && (
  <div className="p-4 rounded-lg bg-error/10 border border-error/20">
    <Text size="sm" className="text-error">
      {error}
    </Text>
  </div>
)}
```

---

## 7. Error Handling Strategy

### 7.1 Error States

Following the pattern from [`ActForm`](../../src/GloboTicket.Web/src/components/organisms/ActForm.tsx:67) and [`VenueForm`](../../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx:107):

**Error State Types**:
1. **Loading State** - Show spinner while fetching data
2. **Validation Errors** - Show inline errors for invalid fields
3. **Not Found** - Act or Venue not found
4. **Network Error** - API request failed
5. **Unauthorized** - User not authenticated (handled by ProtectedRoute)
6. **Rate Limit** - Too many requests
7. **Server Error** - Generic server error

### 7.2 Error Display Pattern

**CreateShowPage Error Handling**:
```typescript
if (isLoading) {
  return (
    <div className="flex justify-center items-center min-h-[400px]">
      <Spinner size="lg" />
    </div>
  );
}

if (error || !act) {
  return (
    <Stack gap="xl">
      <Card>
        <div className="p-8 text-center">
          <Text className="text-error">{error || 'Act not found'}</Text>
        </div>
      </Card>
    </Stack>
  );
}
```

**ShowForm Error Handling**:
```typescript
{error && (
  <div className="p-4 rounded-lg bg-error/10 border border-error/20">
    <Text size="sm" className="text-error">
      {error}
    </Text>
  </div>
)}

{/* Field-level errors */}
{fieldErrors.venue && (
  <Text size="sm" className="text-error mt-1">
    {fieldErrors.venue}
  </Text>
)}
```

### 7.3 Error Messages

**User-Friendly Messages**:
- "Please select a venue" - Venue not selected
- "Ticket count cannot exceed venue capacity of {capacity}" - Capacity exceeded
- "Start date must be in the future" - Past date selected
- "Failed to create show" - Generic creation error
- "Failed to load venues" - Venues fetch error
- "Act not found" - Invalid act ID

**Technical Messages** (logged to console):
- Full error stack trace
- API response status and body
- Request details for debugging

### 7.4 Error Recovery

**Retry Strategy**:
- No automatic retry (keeps implementation simple)
- User can manually retry by re-submitting form
- Navigation back to act detail page always available

**Graceful Degradation**:
- If venues fail to load, show error message with retry option
- If nearby shows fail to load, silently fail (informational only)
- Form remains functional even if nearby shows unavailable

### 7.5 Accessibility for Errors

**ARIA Live Regions**:
```typescript
<div role="alert" aria-live="polite">
  <Text className="text-error">{error}</Text>
</div>
```

**Screen Reader Announcements**:
- Error messages announced when they appear
- Loading state announced when data fetching starts
- Success state announced when show created

---

## 8. Component Gap Analysis

### 8.1 Existing Components (Can Reuse)

**Atoms**:
- ✅ [`Button`](../../src/GloboTicket.Web/src/components/atoms/Button.tsx) - For submit, cancel, and "Add Show" buttons
- ✅ [`Heading`](../../src/GloboTicket.Web/src/components/atoms/Heading.tsx) - For page and section titles
- ✅ [`Text`](../../src/GloboTicket.Web/src/components/atoms/Text.tsx) - For labels, errors, and informational text
- ✅ [`Spinner`](../../src/GloboTicket.Web/src/components/atoms/Spinner.tsx) - For loading states

**Molecules**:
- ✅ [`Card`](../../src/GloboTicket.Web/src/components/molecules/Card.tsx) - For form container and nearby shows section
- ✅ [`PageHeader`](../../src/GloboTicket.Web/src/components/molecules/PageHeader.tsx) - For page title
- ✅ [`Dropdown`](../../src/GloboTicket.Web/src/components/molecules/Dropdown.tsx) - For venue selection
- ✅ [`ShowCard`](../../src/GloboTicket.Web/src/components/molecules/ShowCard.tsx) - Already exists (from View Show feature)
- ✅ [`EmptyState`](../../src/GloboTicket.Web/src/components/molecules/EmptyState.tsx) - For "no venues" message

**Layout Primitives**:
- ✅ [`Stack`](../../src/GloboTicket.Web/src/components/layout/Stack.tsx) - For vertical spacing
- ✅ [`Row`](../../src/GloboTicket.Web/src/components/layout/Row.tsx) - For horizontal layout
- ✅ [`Grid`](../../src/GloboTicket.Web/src/components/layout/Grid.tsx) - For date/time fields

### 8.2 New Components (Need to Create)

**Pages**:
- ❌ [`CreateShowPage`](../../src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx) - Container page for show creation

**Organisms**:
- ❌ [`ShowForm`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx) - Form component for creating shows

**Note**: No new atoms or molecules needed. All form inputs use native HTML elements styled with Tailwind CSS, following the pattern from [`ActForm`](../../src/GloboTicket.Web/src/components/organisms/ActForm.tsx) and [`VenueForm`](../../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx).

### 8.3 Component Delegation

**Design System Engineer**: Not needed - all required atoms and molecules already exist.

**Product Developer**: Will implement the new page and organism components using existing atoms and molecules.

---

## 9. Implementation Checklist

### 9.1 Phase 1: Type Definitions and API Client

- [ ] Update [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)
  - [ ] Add `CreateShowDto` interface
  - [ ] Add `NearbyShow` interface
  - [ ] Add `NearbyShowsResponse` interface
  - [ ] Add JSDoc comments
  - [ ] Export types

- [ ] Update [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)
  - [ ] Import `CreateShowDto`, `NearbyShow`, `NearbyShowsResponse` types
  - [ ] Add `createShow(actGuid: string, dto: CreateShowDto)` function
  - [ ] Add `getNearbyShows(venueGuid: string, startTime: string)` function

- [ ] Update [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts)
  - [ ] Add `constructDateTime(date: string, time: string)` function
  - [ ] Add JSDoc comments
  - [ ] Add unit tests (optional)

### 9.2 Phase 2: ShowForm Component

- [ ] Create [`src/GloboTicket.Web/src/components/organisms/ShowForm.tsx`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx)
  - [ ] Import dependencies (React, atoms, molecules, API, types)
  - [ ] Define `ShowFormProps` interface
  - [ ] Define component with state management
  - [ ] Implement venues fetching in `useEffect`
  - [ ] Implement nearby shows fetching in `useEffect`
  - [ ] Implement form validation
  - [ ] Implement form submission
  - [ ] Implement venue dropdown with capacity display
  - [ ] Implement ticket count input with validation
  - [ ] Implement date and time inputs
  - [ ] Implement nearby shows display section
  - [ ] Add submit and cancel buttons
  - [ ] Add error display (general and field-level)
  - [ ] Add loading states
  - [ ] Add ARIA attributes for accessibility
  - [ ] Add responsive styling

- [ ] Update [`src/GloboTicket.Web/src/components/organisms/index.ts`](../../src/GloboTicket.Web/src/components/organisms/index.ts)
  - [ ] Export `ShowForm`

### 9.3 Phase 3: CreateShowPage Component

- [ ] Create [`src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx)
  - [ ] Import dependencies (React, Router, atoms, molecules, organisms, API)
  - [ ] Define component with state management
  - [ ] Implement act fetching in `useEffect`
  - [ ] Implement loading state (spinner)
  - [ ] Implement error state (error card)
  - [ ] Implement success state (ShowForm)
  - [ ] Add back button with navigation
  - [ ] Add page header with act name
  - [ ] Add success and cancel handlers
  - [ ] Add ARIA attributes for accessibility

- [ ] Update [`src/GloboTicket.Web/src/pages/shows/index.ts`](../../src/GloboTicket.Web/src/pages/shows/index.ts)
  - [ ] Export `CreateShowPage`

### 9.4 Phase 4: ActDetailPage Enhancement

- [ ] Update [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx)
  - [ ] Add "Add Show" button to "Upcoming Shows" card header
  - [ ] Add navigation handler for "Add Show" button
  - [ ] Verify shows list displays correctly (already implemented)

### 9.5 Phase 5: Routing Configuration

- [ ] Update [`src/GloboTicket.Web/src/router/routes.ts`](../../src/GloboTicket.Web/src/router/routes.ts)
  - [ ] Add `SHOW_CREATE` constant
  - [ ] Add `showCreate()` helper function

- [ ] Update [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx)
  - [ ] Import `CreateShowPage`
  - [ ] Add show create route
  - [ ] Wrap in `ProtectedRoute`

### 9.6 Phase 6: Testing and Validation

- [ ] Manual Testing
  - [ ] Navigate from acts page to act detail page
  - [ ] Click "Add Show" button
  - [ ] Verify navigation to create show page
  - [ ] Verify act name displays in header
  - [ ] Verify venues load in dropdown
  - [ ] Select venue and verify capacity displays
  - [ ] Enter ticket count and verify validation
  - [ ] Enter date and time
  - [ ] Verify nearby shows fetch and display
  - [ ] Submit form with valid data
  - [ ] Verify navigation back to act detail page
  - [ ] Verify new show appears in shows list
  - [ ] Test cancel button navigation
  - [ ] Test browser back button
  - [ ] Test loading states
  - [ ] Test error states (invalid data, network error)
  - [ ] Test validation errors (capacity exceeded, past date)

- [ ] Accessibility Testing
  - [ ] Keyboard navigation works
  - [ ] Screen reader announces content
  - [ ] Focus indicators visible
  - [ ] ARIA attributes correct
  - [ ] Semantic HTML used
  - [ ] Error messages announced

- [ ] Responsive Testing
  - [ ] Test on mobile viewport (<768px)
  - [ ] Test on tablet viewport (768-1024px)
  - [ ] Test on desktop viewport (>1024px)
  - [ ] Verify layout adapts correctly

- [ ] Cross-Browser Testing
  - [ ] Test in Chrome
  - [ ] Test in Firefox
  - [ ] Test in Safari
  - [ ] Test in Edge

---

## 10. File Structure Summary

### 10.1 New Files

**Pages**:
- [`src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/CreateShowPage.tsx) - Create show page component

**Organisms**:
- [`src/GloboTicket.Web/src/components/organisms/ShowForm.tsx`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx) - Show form organism

### 10.2 Modified Files

**Types**:
- [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts) - Add `CreateShowDto`, `NearbyShow`, `NearbyShowsResponse` interfaces

**API Client**:
- [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts) - Add `createShow()` and `getNearbyShows()` functions

**Utilities**:
- [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts) - Add `constructDateTime()` function

**Routing**:
- [`src/GloboTicket.Web/src/router/routes.ts`](../../src/GloboTicket.Web/src/router/routes.ts) - Add show create route
- [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx) - Add show create route configuration

**Pages**:
- [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx) - Add "Add Show" button

**Organisms Index**:
- [`src/GloboTicket.Web/src/components/organisms/index.ts`](../../src/GloboTicket.Web/src/components/organisms/index.ts) - Export ShowForm

**Pages Index**:
- [`src/GloboTicket.Web/src/pages/shows/index.ts`](../../src/GloboTicket.Web/src/pages/shows/index.ts) - Export CreateShowPage

---

## 11. Architecture Validation

### 11.1 Design System Compliance

✅ **Atomic Design**: Uses existing atoms and molecules, follows component hierarchy  
✅ **Layout Primitives**: Uses Stack, Row, Grid, Card for layout (no custom margins)  
✅ **Theme Tokens**: Uses design tokens for colors, spacing, typography  
✅ **Styling Strategy**: Uses Tailwind CSS with consistent conventions  
✅ **Accessibility**: Semantic HTML, ARIA attributes, keyboard navigation  
✅ **Responsiveness**: Mobile-first approach with defined breakpoints  

### 11.2 Consistency with Existing Code

✅ **State Management**: Follows existing pattern from ActForm and VenueForm  
✅ **API Client**: Follows existing pattern from createAct() and createVenue()  
✅ **Routing**: Follows existing pattern from acts and venues routes  
✅ **Error Handling**: Follows existing pattern from other forms  
✅ **TypeScript**: Follows existing pattern from Act and Venue interfaces  
✅ **Form Structure**: Follows existing pattern from ActForm and VenueForm  

### 11.3 Architecture Principles

✅ **Separation of Concerns**: Clear separation between data, presentation, and routing  
✅ **Reusability**: Components are reusable and composable  
✅ **Type Safety**: TypeScript interfaces for all data structures  
✅ **Maintainability**: Clear file structure and naming conventions  
✅ **Testability**: Components designed for easy testing  

---

## 12. Next Steps

### 12.1 Implementation Order

1. **Phase 1**: Create type definitions and API client functions
2. **Phase 2**: Create ShowForm organism component
3. **Phase 3**: Create CreateShowPage component
4. **Phase 4**: Enhance ActDetailPage with "Add Show" button
5. **Phase 5**: Configure routing
6. **Phase 6**: Test and validate

### 12.2 Delegation

**Recommended Mode**: [`product-developer`](../../.kilocode/modes/product-developer.md) mode

**Rationale**: The Add Show to Act feature follows existing patterns from ActForm and VenueForm. The product-developer mode is designed for assembling features from existing components, which is exactly what this feature requires.

**Alternative**: [`code`](../../.kilocode/modes/code.md) mode if product-developer is not available.

### 12.3 Success Criteria

**Feature Complete When**:
- [ ] User can navigate from act detail page to create show page
- [ ] Create show page displays act name in header
- [ ] Venue dropdown loads and displays venues
- [ ] Venue capacity displays when venue selected
- [ ] Ticket count validation works (capacity check)
- [ ] Date and time inputs work correctly
- [ ] Nearby shows fetch and display when all fields filled
- [ ] Form validation works (all rules enforced)
- [ ] Show creation succeeds with valid data
- [ ] Navigation back to act detail page works
- [ ] New show appears in act detail page shows list
- [ ] Cancel button navigates back to act detail page
- [ ] Loading and error states work correctly
- [ ] Accessibility requirements met
- [ ] Responsive design works on all screen sizes
- [ ] Code follows existing patterns and conventions

---

**Document Status**: Ready for implementation  
**Last Updated**: 2026-01-03  
**Next Review**: After implementation complete
