# View Show Feature - Frontend Architecture

**Status**: Draft  
**Created**: 2026-01-02  
**Author**: Frontend Architect  
**Related Documents**:
- Technical Specification: [`docs/specs/view-show.md`](view-show.md)
- User Story: [`docs/user-stories/view-show.md`](../user-stories/view-show.md)
- Design System: [`docs/design-system-architecture.md`](../design-system-architecture.md)
- Atomic Design: [`docs/atomic-design.md`](../atomic-design.md)

---

## Executive Summary

This document defines the frontend architecture for the View Show feature, which enables event organizers to view detailed information about a specific show including the act name, venue name, and start date/time. The implementation follows the established GloboTicket design system principles with Atomic Design, React Router for navigation, and TypeScript for type safety.

**Key Components:**
1. **ShowDetailPage** - New page component to display show details
2. **ActDetailPage Enhancement** - Add shows list with navigation to show details
3. **Show Type Definition** - TypeScript interface for show data
4. **API Client Functions** - `getShow()` and `getShowsByAct()` functions
5. **Routing Configuration** - New route for `/shows/:id`

**Architecture Principles:**
- Follow existing patterns from [`ActDetailPage`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx) and [`VenueDetailPage`](../../src/GloboTicket.Web/src/pages/venues/VenueDetailPage.tsx)
- Use Atomic Design with existing atoms and molecules
- Implement client-side date/time formatting for user locale
- Handle loading, error, and not-found states consistently
- Maintain accessibility and responsive design standards

---

## 1. Component Architecture

### 1.1 Component Hierarchy

Following Atomic Design principles, the View Show feature uses existing atoms and molecules, with new page-level components:

```
ShowDetailPage (Page - New)
├── Stack (Layout Primitive)
│   ├── Button (Atom - Back button)
│   ├── Page Header Section
│   │   ├── Icon Container (Calendar icon)
│   │   ├── Heading (Atom - Show title)
│   │   └── Text (Atom - Subtitle)
│   └── Card (Molecule - Show information)
│       └── Stack (Layout Primitive)
│           ├── Text (Atom - Field labels and values)
│           └── ... (Additional fields)

ActDetailPage (Page - Enhanced)
├── Stack (Layout Primitive)
│   ├── ... (Existing sections)
│   └── Card (Molecule - Shows section)
│       └── Stack (Layout Primitive)
│           └── ShowCard (Molecule - New, clickable)
│               ├── Text (Atom - Venue name)
│               ├── Text (Atom - Start date/time)
│               └── Text (Atom - Ticket count)
```

### 1.2 New Components

#### ShowDetailPage

**Location**: [`src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx)

**Purpose**: Display detailed information about a specific show.

**Component Structure**:
```tsx
export const ShowDetailPage = () => {
  // State management
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [show, setShow] = useState<Show | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Data fetching
  useEffect(() => {
    // Fetch show data using getShow(id)
  }, [id]);

  // Render states: loading, error, success
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ ← Back to Acts                                      │
├─────────────────────────────────────────────────────┤
│                                                     │
│  📅  The Rolling Stones at Madison Square Garden   │
│      March 15, 2026 at 7:30 PM                     │
│                                                     │
├─────────────────────────────────────────────────────┤
│ Show Information                                    │
│ ┌─────────────────────────────────────────────────┐│
│ │ Act Name                                        ││
│ │ The Rolling Stones                              ││
│ │                                                 ││
│ │ Venue Name                                      ││
│ │ Madison Square Garden                           ││
│ │                                                 ││
│ │ Start Date                                      ││
│ │ March 15, 2026                                  ││
│ │                                                 ││
│ │ Start Time                                      ││
│ │ 7:30 PM                                         ││
│ │                                                 ││
│ │ Tickets Available                               ││
│ │ 15,000                                          ││
│ │                                                 ││
│ │ Created                                         ││
│ │ January 2, 2026, 10:30:00 AM                   ││
│ │                                                 ││
│ │ Last Updated                                    ││
│ │ January 2, 2026, 2:45:00 PM                    ││
│ └─────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘
```

**Props**: None (uses URL params)

**State**:
- `show: Show | null` - Loaded show data
- `isLoading: boolean` - Loading state during fetch
- `error: string | null` - Error message if fetch fails

**Behavior**:
- Fetches show data on mount using `getShow(id)`
- Displays loading spinner while fetching
- Displays error message if show not found or fetch fails
- Formats dates/times using browser locale
- Navigates back to acts page on back button click

**Accessibility**:
- Page title uses semantic `<h1>` heading
- Section headers use semantic `<h2>` headings
- Loading state announced via `aria-live` region
- Error messages announced via `aria-live` region
- Back button accessible via keyboard
- Date/time values use semantic `<time>` elements with `datetime` attribute

**Responsive Behavior**:
- Desktop (>1024px): Full-width layout with comfortable spacing
- Tablet (768-1024px): Same as desktop
- Mobile (<768px): Single column, stacked layout, reduced spacing

#### ShowCard (Molecule - New)

**Location**: [`src/GloboTicket.Web/src/components/molecules/ShowCard.tsx`](../../src/GloboTicket.Web/src/components/molecules/ShowCard.tsx)

**Purpose**: Display a clickable show card in the acts detail page shows list.

**Component Structure**:
```tsx
interface ShowCardProps {
  show: Show;
  onClick: () => void;
}

export const ShowCard = ({ show, onClick }: ShowCardProps) => {
  // Render clickable card with show information
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ Madison Square Garden                               │
│ March 15, 2026 at 7:30 PM                          │
│ 15,000 tickets available                           │
└─────────────────────────────────────────────────────┘
```

**Props**:
- `show: Show` - Show data to display
- `onClick: () => void` - Click handler for navigation

**Behavior**:
- Displays venue name, start date/time, and ticket count
- Clickable card navigates to show detail page
- Hover state indicates interactivity
- Keyboard accessible (Enter/Space to activate)

**Accessibility**:
- Uses semantic button or link element
- Has clear accessible label
- Keyboard navigable
- Focus indicator visible

### 1.3 Enhanced Components

#### ActDetailPage Enhancement

**Location**: [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx)

**Changes Required**:
1. Add state for shows list: `const [shows, setShows] = useState<Show[]>([]);`
2. Fetch shows on mount using `getShowsByAct(actGuid)`
3. Replace "No upcoming shows scheduled" placeholder with shows list
4. Render `ShowCard` components for each show
5. Handle click to navigate to `/shows/{showGuid}`

**Enhanced Shows Section**:
```tsx
{/* Upcoming Shows */}
<Card header={<Heading level="h2">Upcoming Shows</Heading>}>
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

### 2.1 Component-Level State

Following the existing pattern from [`ActDetailPage`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx) and [`VenueDetailPage`](../../src/GloboTicket.Web/src/pages/venues/VenueDetailPage.tsx), the View Show feature uses React's built-in `useState` and `useEffect` hooks for state management.

**ShowDetailPage State**:
```typescript
const [show, setShow] = useState<Show | null>(null);
const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);
```

**ActDetailPage Additional State**:
```typescript
const [shows, setShows] = useState<Show[]>([]);
const [isLoadingShows, setIsLoadingShows] = useState(false);
const [showsError, setShowsError] = useState<string | null>(null);
```

### 2.2 Data Fetching Pattern

**Pattern**: Fetch data in `useEffect` on component mount, following existing conventions.

**ShowDetailPage Data Fetching**:
```typescript
useEffect(() => {
  const fetchShow = async () => {
    if (!id) {
      setError('Show ID is required');
      setIsLoading(false);
      return;
    }

    try {
      const data = await getShow(id);
      setShow(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load show');
    } finally {
      setIsLoading(false);
    }
  };

  fetchShow();
}, [id]);
```

**ActDetailPage Shows Fetching**:
```typescript
useEffect(() => {
  const fetchShows = async () => {
    if (!act?.actGuid) return;

    setIsLoadingShows(true);
    try {
      const data = await getShowsByAct(act.actGuid);
      setShows(data);
    } catch (err) {
      setShowsError(err instanceof Error ? err.message : 'Failed to load shows');
    } finally {
      setIsLoadingShows(false);
    }
  };

  fetchShows();
}, [act?.actGuid]);
```

### 2.3 State Management Rationale

**Why Not TanStack Query?**

While the design system architecture document mentions TanStack Query for server state management, the existing implementation uses simple `useState` and `useEffect` patterns. For consistency with the existing codebase and to avoid introducing new dependencies mid-project, the View Show feature follows the established pattern.

**Future Consideration**: If the application grows and requires more sophisticated caching, optimistic updates, or background refetching, migrating to TanStack Query would be beneficial. However, this should be a project-wide refactoring, not introduced piecemeal.

---

## 3. API Integration

### 3.1 API Client Functions

**Location**: [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)

Following the existing pattern from [`getAct()`](../../src/GloboTicket.Web/src/api/client.ts:145) and [`getVenue()`](../../src/GloboTicket.Web/src/api/client.ts:91), add two new functions:

#### getShow()

**Purpose**: Fetch a single show by GUID.

**Implementation**:
```typescript
export async function getShow(id: string): Promise<Show> {
  const response = await fetch(`${API_BASE_URL}/api/shows/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Show>(response);
}
```

**Parameters**:
- `id: string` - Show GUID

**Returns**: `Promise<Show>` - Show data

**Error Handling**:
- 401 Unauthorized: Throws error, handled by component
- 404 Not Found: Throws error with message "Show not found"
- 429 Rate Limit: Throws error with rate limit message
- 500 Server Error: Throws error with generic message

#### getShowsByAct()

**Purpose**: Fetch all shows for a specific act.

**Implementation**:
```typescript
export async function getShowsByAct(actGuid: string): Promise<Show[]> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${actGuid}/shows`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Show[]>(response);
}
```

**Parameters**:
- `actGuid: string` - Act GUID

**Returns**: `Promise<Show[]>` - Array of shows

**Error Handling**:
- Same as `getShow()`, but returns empty array on 404

### 3.2 API Response Handling

**Existing Pattern**: The [`handleResponse()`](../../src/GloboTicket.Web/src/api/client.ts:12) utility function handles all API responses consistently:

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

### 3.3 Authentication

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

Add new route constants for shows:

```typescript
export const ROUTES = {
  // ... existing routes
  
  // Shows
  SHOWS: '/shows',
  SHOW_DETAIL: '/shows/:id',
} as const;
```

Add route helper function:

```typescript
export const routeHelpers = {
  // ... existing helpers
  
  showDetail: (id: string) => `/shows/${id}`,
};
```

### 4.2 Router Configuration

**Location**: [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx)

Add new route for show detail page:

```typescript
import { ShowDetailPage } from '../pages/shows';

export const router = createBrowserRouter([
  // ... existing routes
  
  {
    path: '/',
    element: <AppLayout />,
    children: [
      // ... existing children
      
      // Shows
      {
        path: ROUTES.SHOW_DETAIL,
        element: (
          <ProtectedRoute>
            <ShowDetailPage />
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
3. Act detail page displays list of shows
4. User clicks on a show card
5. Navigate to show detail page (`/shows/:showId`)

**Back Navigation**:
- Show detail page has "Back to Acts" button
- Clicking back button navigates to `/acts`
- Browser back button also works (React Router history)

**URL Structure**:
- Acts list: `/acts`
- Act detail: `/acts/{actGuid}`
- Show detail: `/shows/{showGuid}`

**Navigation Implementation**:
```typescript
// In ShowCard component
const navigate = useNavigate();
const handleClick = () => navigate(`/shows/${show.showGuid}`);

// In ShowDetailPage component
const handleBack = () => navigate('/acts');
```

### 4.4 Protected Routes

All show routes are wrapped in [`ProtectedRoute`](../../src/GloboTicket.Web/src/components/routing/ProtectedRoute.tsx) component, which:
- Checks authentication status via [`AuthContext`](../../src/GloboTicket.Web/src/contexts/AuthContext.tsx)
- Redirects to login page if not authenticated
- Preserves intended destination for post-login redirect

---

## 5. TypeScript Interfaces

### 5.1 Show Interface

**Location**: [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts) (new file)

Following the pattern from [`Act`](../../src/GloboTicket.Web/src/types/act.ts:4) and [`Venue`](../../src/GloboTicket.Web/src/types/venue.ts:4) interfaces:

```typescript
/**
 * Show interface matching the backend ShowDto
 */
export interface Show {
  /** Database-generated unique identifier */
  id: number;
  
  /** Unique GUID identifier for the show */
  showGuid: string;
  
  /** GUID of the associated act */
  actGuid: string;
  
  /** Name of the act performing at this show */
  actName: string;
  
  /** GUID of the venue where the show takes place */
  venueGuid: string;
  
  /** Name of the venue where the show takes place */
  venueName: string;
  
  /** Maximum seating capacity of the venue */
  venueCapacity: number;
  
  /** Number of tickets available for this show */
  ticketCount: number;
  
  /** Show start time with timezone offset (ISO 8601 format) */
  startTime: string;
  
  /** UTC timestamp when the show was created */
  createdAt: string;
  
  /** UTC timestamp when the show was last updated */
  updatedAt?: string;
}
```

**Design Decisions**:
- Matches backend [`ShowDto`](../../src/GloboTicket.Application/DTOs/ShowDto.cs) exactly
- Uses `string` for dates (ISO 8601 format from API)
- Includes denormalized data (actName, venueName) for display
- Optional `updatedAt` field (may be null)
- All fields documented with JSDoc comments

### 5.2 Type Exports

**Location**: [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)

```typescript
export type { Show };
```

**Usage in Components**:
```typescript
import type { Show } from '../../types/show';

const [show, setShow] = useState<Show | null>(null);
```

### 5.3 Type Safety Benefits

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

## 6. Error Handling Strategy

### 6.1 Error States

Following the pattern from [`ActDetailPage`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx:62) and [`VenueDetailPage`](../../src/GloboTicket.Web/src/pages/venues/VenueDetailPage.tsx:62):

**Error State Types**:
1. **Loading State** - Show spinner while fetching data
2. **Not Found** - Show not found or invalid ID
3. **Network Error** - API request failed
4. **Unauthorized** - User not authenticated (handled by ProtectedRoute)
5. **Rate Limit** - Too many requests

### 6.2 Error Display Pattern

**ShowDetailPage Error Handling**:
```typescript
if (isLoading) {
  return (
    <div className="flex justify-center items-center min-h-[400px]">
      <Spinner size="lg" />
    </div>
  );
}

if (error || !show) {
  return (
    <Stack gap="xl">
      <Card>
        <div className="p-8 text-center">
          <Text className="text-error">{error || 'Show not found'}</Text>
        </div>
      </Card>
    </Stack>
  );
}
```

**ActDetailPage Shows Error Handling**:
```typescript
{isLoadingShows ? (
  <div className="flex justify-center p-8">
    <Spinner size="md" />
  </div>
) : showsError ? (
  <Text className="text-error">{showsError}</Text>
) : shows.length === 0 ? (
  <Text variant="muted">No upcoming shows scheduled for this act.</Text>
) : (
  // Render shows list
)}
```

### 6.3 Error Messages

**User-Friendly Messages**:
- "Show not found" - Show doesn't exist or cross-tenant access
- "Failed to load show details" - Generic fetch error
- "Failed to load shows" - Error fetching shows list
- "Please try again later" - Rate limit exceeded

**Technical Messages** (logged to console):
- Full error stack trace
- API response status and body
- Request details for debugging

### 6.4 Error Recovery

**Retry Strategy**:
- No automatic retry (keeps implementation simple)
- User can manually retry by refreshing page
- Navigation back to acts page always available

**Graceful Degradation**:
- If shows list fails to load, act detail page still displays
- Error message shown in shows section only
- Rest of page remains functional

### 6.5 Accessibility for Errors

**ARIA Live Regions**:
```typescript
<div role="alert" aria-live="polite">
  <Text className="text-error">{error}</Text>
</div>
```

**Screen Reader Announcements**:
- Error messages announced when they appear
- Loading state announced when data fetching starts
- Success state announced when data loads

---

## 7. Date/Time Formatting

### 7.1 Formatting Strategy

**Client-Side Formatting**: Use JavaScript's built-in `Date` object and locale methods to format dates/times according to the user's browser locale.

**Why Client-Side?**
- Respects user's locale and timezone preferences
- No server-side configuration needed
- Consistent with browser's date/time display
- Automatic timezone conversion

### 7.2 Formatting Functions

**Basic Formatting** (using default locale):
```typescript
// Start Date: "3/15/2026"
const startDate = new Date(show.startTime).toLocaleDateString();

// Start Time: "7:30:00 PM"
const startTime = new Date(show.startTime).toLocaleTimeString();

// Created: "1/2/2026, 10:30:00 AM"
const created = new Date(show.createdAt).toLocaleString();
```

**Enhanced Formatting** (with options):
```typescript
// Start Date: "March 15, 2026"
const startDate = new Date(show.startTime).toLocaleDateString('en-US', {
  year: 'numeric',
  month: 'long',
  day: 'numeric'
});

// Start Time: "7:30 PM"
const startTime = new Date(show.startTime).toLocaleTimeString('en-US', {
  hour: 'numeric',
  minute: '2-digit',
  hour12: true
});
```

### 7.3 Formatting Utility

**Location**: [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts) (if it exists, otherwise create)

**Utility Functions**:
```typescript
/**
 * Format date in long format (e.g., "March 15, 2026")
 */
export function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

/**
 * Format time in 12-hour format (e.g., "7:30 PM")
 */
export function formatTime(dateString: string): string {
  return new Date(dateString).toLocaleTimeString('en-US', {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  });
}

/**
 * Format date and time together (e.g., "March 15, 2026 at 7:30 PM")
 */
export function formatDateTime(dateString: string): string {
  return `${formatDate(dateString)} at ${formatTime(dateString)}`;
}

/**
 * Format timestamp with date and time (e.g., "1/2/2026, 10:30:00 AM")
 */
export function formatTimestamp(dateString: string): string {
  return new Date(dateString).toLocaleString();
}
```

**Usage in Components**:
```typescript
import { formatDate, formatTime, formatDateTime, formatTimestamp } from '../../utils/format';

// In ShowDetailPage
<Text>{formatDate(show.startTime)}</Text>
<Text>{formatTime(show.startTime)}</Text>
<Text>{formatTimestamp(show.createdAt)}</Text>
```

### 7.4 Semantic HTML for Dates

**Accessibility**: Use `<time>` element with `datetime` attribute for machine-readable dates:

```tsx
<time dateTime={show.startTime}>
  {formatDateTime(show.startTime)}
</time>
```

**Benefits**:
- Screen readers can announce dates properly
- Search engines understand date context
- Browser extensions can parse dates
- Consistent with web standards

---

## 8. Implementation Checklist

### 8.1 Phase 1: Type Definitions and API Client

- [ ] Create [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts)
  - [ ] Define `Show` interface
  - [ ] Add JSDoc comments
  - [ ] Export type

- [ ] Update [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)
  - [ ] Import `Show` type
  - [ ] Add `getShow(id: string)` function
  - [ ] Add `getShowsByAct(actGuid: string)` function

- [ ] Create/Update [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts)
  - [ ] Add `formatDate()` function
  - [ ] Add `formatTime()` function
  - [ ] Add `formatDateTime()` function
  - [ ] Add `formatTimestamp()` function

### 8.2 Phase 2: ShowDetailPage Component

- [ ] Create [`src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx)
  - [ ] Import dependencies (React, Router, atoms, molecules, API)
  - [ ] Define component with state management
  - [ ] Implement data fetching in `useEffect`
  - [ ] Implement loading state (spinner)
  - [ ] Implement error state (error card)
  - [ ] Implement success state (show details)
  - [ ] Add back button with navigation
  - [ ] Add page header with icon and title
  - [ ] Add show information card
  - [ ] Format dates/times using utility functions
  - [ ] Add semantic `<time>` elements
  - [ ] Add ARIA attributes for accessibility

- [ ] Create [`src/GloboTicket.Web/src/pages/shows/index.ts`](../../src/GloboTicket.Web/src/pages/shows/index.ts)
  - [ ] Export `ShowDetailPage`

### 8.3 Phase 3: ShowCard Molecule

- [ ] Create [`src/GloboTicket.Web/src/components/molecules/ShowCard.tsx`](../../src/GloboTicket.Web/src/components/molecules/ShowCard.tsx)
  - [ ] Define `ShowCardProps` interface
  - [ ] Implement component with Card molecule
  - [ ] Display venue name
  - [ ] Display start date/time (formatted)
  - [ ] Display ticket count
  - [ ] Add click handler
  - [ ] Add hover state styling
  - [ ] Add keyboard accessibility
  - [ ] Add ARIA attributes

- [ ] Update [`src/GloboTicket.Web/src/components/molecules/index.ts`](../../src/GloboTicket.Web/src/components/molecules/index.ts)
  - [ ] Export `ShowCard`

### 8.4 Phase 4: ActDetailPage Enhancement

- [ ] Update [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx)
  - [ ] Import `Show` type and `ShowCard` component
  - [ ] Import `getShowsByAct` API function
  - [ ] Add state for shows list
  - [ ] Add state for shows loading
  - [ ] Add state for shows error
  - [ ] Implement shows fetching in `useEffect`
  - [ ] Replace placeholder shows section
  - [ ] Render loading state (spinner)
  - [ ] Render error state (error message)
  - [ ] Render empty state (no shows message)
  - [ ] Render shows list (ShowCard components)
  - [ ] Add navigation handler for show cards

### 8.5 Phase 5: Routing Configuration

- [ ] Update [`src/GloboTicket.Web/src/router/routes.ts`](../../src/GloboTicket.Web/src/router/routes.ts)
  - [ ] Add `SHOWS` constant
  - [ ] Add `SHOW_DETAIL` constant
  - [ ] Add `showDetail()` helper function

- [ ] Update [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx)
  - [ ] Import `ShowDetailPage`
  - [ ] Add show detail route
  - [ ] Wrap in `ProtectedRoute`

### 8.6 Phase 6: Testing and Validation

- [ ] Manual Testing
  - [ ] Navigate from acts page to act detail page
  - [ ] Verify shows list displays on act detail page
  - [ ] Click on a show card
  - [ ] Verify navigation to show detail page
  - [ ] Verify show details display correctly
  - [ ] Verify dates/times formatted correctly
  - [ ] Test back button navigation
  - [ ] Test browser back button
  - [ ] Test loading states
  - [ ] Test error states (invalid ID, network error)
  - [ ] Test not found state (non-existent show)

- [ ] Accessibility Testing
  - [ ] Keyboard navigation works
  - [ ] Screen reader announces content
  - [ ] Focus indicators visible
  - [ ] ARIA attributes correct
  - [ ] Semantic HTML used

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

## 9. File Structure Summary

### 9.1 New Files

**Pages**:
- [`src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx) - Show detail page component
- [`src/GloboTicket.Web/src/pages/shows/index.ts`](../../src/GloboTicket.Web/src/pages/shows/index.ts) - Barrel export

**Components**:
- [`src/GloboTicket.Web/src/components/molecules/ShowCard.tsx`](../../src/GloboTicket.Web/src/components/molecules/ShowCard.tsx) - Show card molecule

**Types**:
- [`src/GloboTicket.Web/src/types/show.ts`](../../src/GloboTicket.Web/src/types/show.ts) - Show interface

**Utilities** (if not exists):
- [`src/GloboTicket.Web/src/utils/format.ts`](../../src/GloboTicket.Web/src/utils/format.ts) - Date/time formatting utilities

### 9.2 Modified Files

**API Client**:
- [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts) - Add `getShow()` and `getShowsByAct()` functions

**Routing**:
- [`src/GloboTicket.Web/src/router/routes.ts`](../../src/GloboTicket.Web/src/router/routes.ts) - Add show routes
- [`src/GloboTicket.Web/src/router/index.tsx`](../../src/GloboTicket.Web/src/router/index.tsx) - Add show detail route

**Pages**:
- [`src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/acts/ActDetailPage.tsx) - Add shows list section

**Molecules Index**:
- [`src/GloboTicket.Web/src/components/molecules/index.ts`](../../src/GloboTicket.Web/src/components/molecules/index.ts) - Export ShowCard

---

## 10. Architecture Validation

### 10.1 Design System Compliance

✅ **Atomic Design**: Uses existing atoms and molecules, follows component hierarchy  
✅ **Layout Primitives**: Uses Stack, Row, Card for layout (no custom margins)  
✅ **Theme Tokens**: Uses design tokens for colors, spacing, typography  
✅ **Styling Strategy**: Uses Tailwind CSS with consistent conventions  
✅ **Accessibility**: Semantic HTML, ARIA attributes, keyboard navigation  
✅ **Responsiveness**: Mobile-first approach with defined breakpoints  

### 10.2 Consistency with Existing Code

✅ **State Management**: Follows existing pattern from ActDetailPage and VenueDetailPage  
✅ **API Client**: Follows existing pattern from getAct() and getVenue()  
✅ **Routing**: Follows existing pattern from acts and venues routes  
✅ **Error Handling**: Follows existing pattern from other detail pages  
✅ **TypeScript**: Follows existing pattern from Act and Venue interfaces  

### 10.3 Architecture Principles

✅ **Separation of Concerns**: Clear separation between data, presentation, and routing  
✅ **Reusability**: Components are reusable and composable  
✅ **Type Safety**: TypeScript interfaces for all data structures  
✅ **Maintainability**: Clear file structure and naming conventions  
✅ **Testability**: Components designed for easy testing  

---

## 11. Next Steps

### 11.1 Implementation Order

1. **Phase 1**: Create type definitions and API client functions
2. **Phase 2**: Create ShowDetailPage component
3. **Phase 3**: Create ShowCard molecule component
4. **Phase 4**: Enhance ActDetailPage with shows list
5. **Phase 5**: Configure routing
6. **Phase 6**: Test and validate

### 11.2 Delegation

**Recommended Mode**: [`product-developer`](../../.kilocode/modes/product-developer.md) mode

**Rationale**: The View Show feature is a straightforward implementation that follows existing patterns. The product-developer mode is designed for assembling features from existing components, which is exactly what this feature requires.

**Alternative**: [`code`](../../.kilocode/modes/code.md) mode if product-developer is not available.

### 11.3 Success Criteria

**Feature Complete When**:
- [ ] User can navigate from acts page to show detail page
- [ ] Show detail page displays all required information
- [ ] Dates/times are formatted correctly
- [ ] Loading and error states work correctly
- [ ] Back navigation works
- [ ] Accessibility requirements met
- [ ] Responsive design works on all screen sizes
- [ ] Code follows existing patterns and conventions

---

**Document Status**: Ready for implementation  
**Last Updated**: 2026-01-02  
**Next Review**: After implementation complete
