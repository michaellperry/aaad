# Create Ticket Offer Feature - Frontend Architecture

**Status**: Draft  
**Created**: 2026-01-03  
**Author**: Frontend Architect  
**Related Documents**:
- Technical Specification: [`docs/specs/create-ticket-offer.md`](create-ticket-offer.md)
- User Story: [`docs/user-stories/create-ticket-offer.md`](../user-stories/create-ticket-offer.md)
- Design System: [`docs/design-system-architecture.md`](../design-system-architecture.md)
- Atomic Design: [`docs/atomic-design.md`](../atomic-design.md)
- View Show Architecture: [`docs/specs/view-show-frontend-architecture.md`](view-show-frontend-architecture.md)

---

## Executive Summary

This document defines the frontend architecture for the Create Ticket Offer feature, which enables event organizers to create ticket offers from the show detail page. The implementation integrates new components into the existing [`ShowDetailPage`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx) following established GloboTicket design system principles with Atomic Design, TanStack Query for data management, and TypeScript for type safety.

**Key Components:**
1. **TicketOfferForm** (Organism) - Form to create new ticket offers with capacity validation
2. **TicketOffersList** (Organism) - Display list of existing ticket offers
3. **CapacityDisplay** (Molecule) - Visual capacity information with progress bar
4. **ShowDetailPage Enhancement** - Integration of ticket offer components

**Architecture Principles:**
- Follow existing patterns from [`ShowForm`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx) and [`VenueForm`](../../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx)
- Use TanStack Query for server state management
- Implement real-time capacity validation
- Handle loading, error, and empty states consistently
- Maintain accessibility and responsive design standards

---

## 1. Component Architecture

### 1.1 Component Hierarchy

Following Atomic Design principles, the Create Ticket Offer feature uses existing atoms and molecules, with new organisms integrated into the ShowDetailPage:

```
ShowDetailPage (Page - Enhanced)
├── Stack (Layout Primitive)
│   ├── Button (Atom - Back button)
│   ├── Page Header Section
│   │   ├── Icon Container (Calendar icon)
│   │   ├── Heading (Atom - Show title)
│   │   └── Text (Atom - Subtitle)
│   ├── Card (Molecule - Show information)
│   │   └── Stack (Layout Primitive)
│   │       └── ... (Existing show fields)
│   │
│   ├── Card (Molecule - Capacity Information) [NEW]
│   │   └── CapacityDisplay (Molecule - New)
│   │       ├── Text (Atom - Total tickets)
│   │       ├── Text (Atom - Allocated tickets)
│   │       ├── Text (Atom - Available capacity)
│   │       └── Progress bar visualization
│   │
│   ├── Card (Molecule - Create Ticket Offer) [NEW]
│   │   └── TicketOfferForm (Organism - New)
│   │       ├── Input fields (Atoms)
│   │       ├── Validation messages (Text atoms)
│   │       └── Buttons (Atoms)
│   │
│   └── Card (Molecule - Ticket Offers) [NEW]
│       └── TicketOffersList (Organism - New)
│           ├── EmptyState (Molecule - if no offers)
│           └── TicketOfferCard (Molecule - New)
│               ├── Text (Atom - Offer name)
│               ├── Text (Atom - Price)
│               └── Text (Atom - Ticket count)
```

### 1.2 New Components

#### CapacityDisplay (Molecule)

**Location**: [`src/GloboTicket.Web/src/components/molecules/CapacityDisplay.tsx`](../../src/GloboTicket.Web/src/components/molecules/CapacityDisplay.tsx)

**Purpose**: Display show capacity allocation with visual progress bar.

**Component Structure**:
```tsx
interface CapacityDisplayProps {
  totalTickets: number;
  allocatedTickets: number;
  availableCapacity: number;
}

export const CapacityDisplay = ({ 
  totalTickets, 
  allocatedTickets, 
  availableCapacity 
}: CapacityDisplayProps) => {
  const allocationPercentage = (allocatedTickets / totalTickets) * 100;
  const isLowCapacity = availableCapacity < totalTickets * 0.1; // Less than 10%
  const isFullyAllocated = availableCapacity === 0;
  
  return (
    <Stack gap="sm">
      {/* Capacity metrics */}
      {/* Progress bar */}
      {/* Warning/success indicators */}
    </Stack>
  );
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ Capacity Information                                │
│ ┌─────────────────────────────────────────────────┐│
│ │ Total Tickets: 1,000                            ││
│ │ Allocated: 800                                  ││
│ │ Available: 200 ⚠️                               ││
│ │                                                 ││
│ │ ████████████████████░░░░░░░░ 80%               ││
│ └─────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘
```

**Props**:
- `totalTickets: number` - Total ticket count for the show
- `allocatedTickets: number` - Sum of tickets across all offers
- `availableCapacity: number` - Remaining capacity (totalTickets - allocatedTickets)

**State**: None (presentational component)

**Behavior**:
- Displays three key metrics: total, allocated, available
- Shows progress bar with allocation percentage
- Highlights available capacity (primary color when healthy, warning when low)
- Shows warning indicator when capacity < 10%
- Shows success indicator when fully allocated

**Accessibility**:
- Progress bar uses `role="progressbar"` with `aria-valuenow`, `aria-valuemin`, `aria-valuemax`
- Capacity changes announced via `aria-live="polite"`
- Color is not the only indicator (uses icons and text)

**Responsive Behavior**:
- Desktop: Horizontal layout with inline metrics
- Mobile: Stacked layout with full-width progress bar

#### TicketOfferCard (Molecule)

**Location**: [`src/GloboTicket.Web/src/components/molecules/TicketOfferCard.tsx`](../../src/GloboTicket.Web/src/components/molecules/TicketOfferCard.tsx)

**Purpose**: Display a single ticket offer in the offers list.

**Component Structure**:
```tsx
interface TicketOfferCardProps {
  offer: TicketOffer;
}

export const TicketOfferCard = ({ offer }: TicketOfferCardProps) => {
  return (
    <div className="p-4 rounded-lg border border-border-default bg-surface-base">
      <Stack gap="xs">
        <Text className="font-medium">{offer.name}</Text>
        <Row gap="md" align="center">
          <Text variant="muted" size="sm">
            ${offer.price.toFixed(2)} per ticket
          </Text>
          <Text variant="muted" size="sm">
            {offer.ticketCount.toLocaleString()} tickets
          </Text>
        </Row>
      </Stack>
    </div>
  );
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ General Admission                                   │
│ $50.00 per ticket • 600 tickets                    │
└─────────────────────────────────────────────────────┘
```

**Props**:
- `offer: TicketOffer` - Ticket offer data to display

**State**: None (presentational component)

**Behavior**:
- Displays offer name prominently
- Shows price formatted as currency
- Shows ticket count with locale formatting
- Uses consistent card styling

#### TicketOffersList (Organism)

**Location**: [`src/GloboTicket.Web/src/components/organisms/TicketOffersList.tsx`](../../src/GloboTicket.Web/src/components/organisms/TicketOffersList.tsx)

**Purpose**: Display list of ticket offers for a show with loading and empty states.

**Component Structure**:
```tsx
interface TicketOffersListProps {
  showGuid: string;
}

export const TicketOffersList = ({ showGuid }: TicketOffersListProps) => {
  const { data: offers = [], isLoading, error } = useTicketOffers(showGuid);
  
  if (isLoading) {
    return <Spinner size="md" />;
  }
  
  if (error) {
    return <Text className="text-error">{error.message}</Text>;
  }
  
  if (offers.length === 0) {
    return (
      <EmptyState
        icon={<Ticket className="w-12 h-12" />}
        title="No ticket offers yet"
        description="Create your first ticket offer to start selling tickets for this show."
      />
    );
  }
  
  return (
    <Stack gap="md">
      {offers.map((offer) => (
        <TicketOfferCard key={offer.ticketOfferGuid} offer={offer} />
      ))}
    </Stack>
  );
};
```

**Props**:
- `showGuid: string` - GUID of the show to fetch offers for

**State**: Managed by TanStack Query hook

**Behavior**:
- Fetches ticket offers using `useTicketOffers(showGuid)` hook
- Displays loading spinner while fetching
- Displays error message if fetch fails
- Displays empty state if no offers exist
- Renders list of `TicketOfferCard` components
- Sorted by creation date (oldest first, as returned by API)

**API Integration**:
- Query key: `queryKeys.ticketOffers.byShow(showGuid)`
- Endpoint: `GET /api/shows/{showGuid}/ticket-offers`
- Auto-refetches when query is invalidated

#### TicketOfferForm (Organism)

**Location**: [`src/GloboTicket.Web/src/components/organisms/TicketOfferForm.tsx`](../../src/GloboTicket.Web/src/components/organisms/TicketOfferForm.tsx)

**Purpose**: Form component for creating ticket offers with real-time capacity validation.

**Component Structure**:
```tsx
interface TicketOfferFormProps {
  showGuid: string;
  availableCapacity: number;
  onSuccess?: (offer: TicketOffer) => void;
}

export const TicketOfferForm = ({ 
  showGuid, 
  availableCapacity, 
  onSuccess 
}: TicketOfferFormProps) => {
  // Form state
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [ticketCount, setTicketCount] = useState('');
  
  // UI state
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  
  // Mutation
  const createMutation = useCreateTicketOffer();
  
  // Validation and submission logic
  // ...
};
```

**Visual Layout**:
```
┌─────────────────────────────────────────────────────┐
│ Create Ticket Offer                                 │
│ ┌─────────────────────────────────────────────────┐│
│ │ Offer Name *                                    ││
│ │ [e.g., General Admission, VIP, Early Bird]     ││
│ │                                                 ││
│ │ Price per Ticket *                              ││
│ │ $ [0.00]                                        ││
│ │                                                 ││
│ │ Number of Tickets *                             ││
│ │ [Enter ticket count]                            ││
│ │ Available capacity: 200 tickets                 ││
│ │                                                 ││
│ │ [Create Offer]  [Cancel]                        ││
│ └─────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘
```

**Props**:
- `showGuid: string` - GUID of the show to create offer for
- `availableCapacity: number` - Remaining capacity for validation
- `onSuccess?: (offer: TicketOffer) => void` - Callback on successful creation

**State**:
- `name: string` - Offer name input
- `price: string` - Price input (as string for controlled input)
- `ticketCount: string` - Ticket count input (as string for controlled input)
- `error: string | null` - General error message
- `fieldErrors: Record<string, string>` - Per-field validation errors

**Validation Rules**:

| Field | Rule | Error Message |
|-------|------|---------------|
| Name | Required | "Offer name is required" |
| Name | Max 100 characters | "Offer name cannot exceed 100 characters" |
| Price | Required | "Price is required" |
| Price | Must be positive number | "Price must be greater than zero" |
| Price | Valid decimal format | "Please enter a valid price" |
| Ticket Count | Required | "Ticket count is required" |
| Ticket Count | Must be positive integer | "Ticket count must be a positive number" |
| Ticket Count | Must be <= available capacity | "Ticket count exceeds available capacity. Only {availableCapacity} tickets remain available." |

**Form Fields**:

1. **Offer Name** (text input)
   - Label: "Offer Name *"
   - Placeholder: "e.g., General Admission, VIP, Early Bird"
   - maxLength: 100
   - Required

2. **Price** (number input)
   - Label: "Price per Ticket *"
   - Placeholder: "0.00"
   - Currency symbol: "$" (prefix)
   - step: 0.01
   - min: 0.01
   - Required

3. **Ticket Count** (number input)
   - Label: "Number of Tickets *"
   - Placeholder: "Enter ticket count"
   - min: 1
   - max: dynamically set to available capacity
   - Helper text: "Available capacity: {availableCapacity} tickets"
   - Required

**Buttons**:
- **Create Offer** (primary): Submits the form
- **Cancel** (secondary): Clears the form

**Behavior**:
- Client-side validation on blur and submit
- Prevents submission if validation fails
- Shows inline field errors
- Disables submit button while loading
- Clears form on successful submission
- Retains form data on error for correction
- Invalidates ticket offers and capacity queries on success

**API Integration**:
- Create offer: `POST /api/shows/{showGuid}/ticket-offers`
- On success: 
  - Invalidate `queryKeys.ticketOffers.byShow(showGuid)`
  - Invalidate `queryKeys.capacity.byShow(showGuid)`
  - Call `onSuccess` callback if provided
  - Clear form fields

**Accessibility**:
- All inputs have associated `<label>` elements with `htmlFor`
- Required fields marked with asterisk (*) in label
- Error messages use `aria-describedby` to link to inputs
- Form errors announced via `aria-live="polite"`
- Disabled state communicated via `aria-disabled`
- Loading state has appropriate ARIA label

**Responsive Behavior**:
- Desktop: Two-column layout for price and ticket count
- Mobile: Single column, stacked layout

### 1.3 Enhanced Components

#### ShowDetailPage Enhancement

**Location**: [`src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx)

**Changes Required**:
1. Add capacity data fetching: `const { data: capacity } = useShowCapacity(id)`
2. Add ticket offers data fetching: `const { data: offers = [] } = useTicketOffers(id)`
3. Add three new card sections after show information:
   - Capacity Information card with `CapacityDisplay`
   - Create Ticket Offer card with `TicketOfferForm`
   - Ticket Offers card with `TicketOffersList`

**Enhanced Layout Structure**:
```tsx
<Stack gap="xl">
  {/* Existing: Back Button */}
  <Button variant="ghost" onClick={() => navigate('/acts')}>
    <ArrowLeft className="w-4 h-4 mr-2" />
    Back to Acts
  </Button>

  {/* Existing: Page Header */}
  <div className="flex items-start gap-4">
    {/* Icon and title */}
  </div>

  {/* Existing: Show Information Card */}
  <Card header={<Heading level="h2">Show Information</Heading>}>
    {/* Existing show fields */}
  </Card>

  {/* NEW: Capacity Information Card */}
  <Card header={<Heading level="h2">Capacity Information</Heading>}>
    {capacity && (
      <CapacityDisplay
        totalTickets={capacity.totalTickets}
        allocatedTickets={capacity.allocatedTickets}
        availableCapacity={capacity.availableCapacity}
      />
    )}
  </Card>

  {/* NEW: Create Ticket Offer Card */}
  <Card header={<Heading level="h2">Create Ticket Offer</Heading>}>
    {capacity && (
      <TicketOfferForm
        showGuid={id!}
        availableCapacity={capacity.availableCapacity}
      />
    )}
  </Card>

  {/* NEW: Ticket Offers Card */}
  <Card header={<Heading level="h2">Ticket Offers</Heading>}>
    <TicketOffersList showGuid={id!} />
  </Card>
</Stack>
```

---

## 2. State Management

### 2.1 TanStack Query Integration

The Create Ticket Offer feature uses TanStack Query for all server state management, following the project-wide pattern.

**Query Hooks**:

**Location**: [`src/GloboTicket.Web/src/features/ticketOffers/hooks/`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/)

```typescript
// useTicketOffers.ts
export function useTicketOffers(showGuid: string | undefined) {
  return useQuery<TicketOffer[], Error>({
    queryKey: queryKeys.ticketOffers.byShow(showGuid || ''),
    queryFn: () => getTicketOffers(showGuid!),
    enabled: !!showGuid,
  });
}

// useShowCapacity.ts
export function useShowCapacity(showGuid: string | undefined) {
  return useQuery<ShowCapacity, Error>({
    queryKey: queryKeys.capacity.byShow(showGuid || ''),
    queryFn: () => getShowCapacity(showGuid!),
    enabled: !!showGuid,
  });
}

// useCreateTicketOffer.ts
export function useCreateTicketOffer() {
  const queryClient = useQueryClient();
  
  return useMutation<TicketOffer, Error, CreateTicketOfferRequest>({
    mutationFn: ({ showGuid, dto }) => createTicketOffer(showGuid, dto),
    onSuccess: (_, { showGuid }) => {
      // Invalidate both ticket offers and capacity queries
      queryClient.invalidateQueries({ 
        queryKey: queryKeys.ticketOffers.byShow(showGuid) 
      });
      queryClient.invalidateQueries({ 
        queryKey: queryKeys.capacity.byShow(showGuid) 
      });
    },
  });
}
```

**Query Keys**: Centralized in [`queryKeys.ts`](../../src/GloboTicket.Web/src/features/queryKeys.ts)

```typescript
export const queryKeys = {
  // ... existing keys
  
  ticketOffers: {
    byShow: (showGuid: string) => ['ticket-offers', 'by-show', showGuid] as const,
  },
  
  capacity: {
    byShow: (showGuid: string) => ['capacity', 'by-show', showGuid] as const,
  },
};
```

### 2.2 Local Component State

**TicketOfferForm State**:
- Form inputs: `name`, `price`, `ticketCount` (all strings for controlled inputs)
- Validation: `fieldErrors` (per-field), `error` (general)
- UI state managed by mutation: `isLoading`, `isError`, `isSuccess`

**State Flow**:
```
User Input → Local State → Validation → Mutation → Server
                                           ↓
                                    Success/Error
                                           ↓
                              Query Invalidation
                                           ↓
                              Automatic Refetch
                                           ↓
                                    UI Update
```

### 2.3 Data Synchronization

**Automatic Updates**:
- When ticket offer is created, both `ticketOffers` and `capacity` queries are invalidated
- TanStack Query automatically refetches invalidated queries
- UI updates automatically with new data
- No manual state management required

**Optimistic Updates**: Not implemented initially (can be added later for better UX)

---

## 3. API Integration

### 3.1 API Client Functions

**Location**: [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)

Add three new functions following the existing pattern:

#### getTicketOffers()

```typescript
/**
 * Get all ticket offers for a show
 */
export async function getTicketOffers(showGuid: string): Promise<TicketOffer[]> {
  const response = await fetch(`${API_BASE_URL}/api/shows/${showGuid}/ticket-offers`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<TicketOffer[]>(response);
}
```

**Parameters**: `showGuid: string` - Show GUID  
**Returns**: `Promise<TicketOffer[]>` - Array of ticket offers  
**Error Handling**: Uses existing `handleResponse()` utility

#### getShowCapacity()

```typescript
/**
 * Get capacity information for a show
 */
export async function getShowCapacity(showGuid: string): Promise<ShowCapacity> {
  const response = await fetch(`${API_BASE_URL}/api/shows/${showGuid}/capacity`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<ShowCapacity>(response);
}
```

**Parameters**: `showGuid: string` - Show GUID  
**Returns**: `Promise<ShowCapacity>` - Capacity information  
**Error Handling**: Uses existing `handleResponse()` utility

#### createTicketOffer()

```typescript
/**
 * Create a new ticket offer for a show
 */
export async function createTicketOffer(
  showGuid: string, 
  dto: CreateTicketOfferDto
): Promise<TicketOffer> {
  const response = await fetch(`${API_BASE_URL}/api/shows/${showGuid}/ticket-offers`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<TicketOffer>(response);
}
```

**Parameters**:
- `showGuid: string` - Show GUID
- `dto: CreateTicketOfferDto` - Ticket offer data

**Returns**: `Promise<TicketOffer>` - Created ticket offer  
**Error Handling**: Uses existing `handleResponse()` utility

### 3.2 TypeScript Interfaces

**Location**: [`src/GloboTicket.Web/src/types/ticketOffer.ts`](../../src/GloboTicket.Web/src/types/ticketOffer.ts) (new file)

```typescript
/**
 * TicketOffer interface matching the backend TicketOfferDto
 */
export interface TicketOffer {
  /** Database-generated unique identifier */
  id: number;
  
  /** Client-generated unique identifier */
  ticketOfferGuid: string;
  
  /** GUID of the associated show */
  showGuid: string;
  
  /** Name of the ticket offer */
  name: string;
  
  /** Price per ticket */
  price: number;
  
  /** Number of tickets allocated to this offer */
  ticketCount: number;
  
  /** UTC timestamp when the offer was created */
  createdAt: string;
  
  /** UTC timestamp when the offer was last updated */
  updatedAt?: string;
}

/**
 * CreateTicketOfferDto interface matching the backend CreateTicketOfferDto
 */
export interface CreateTicketOfferDto {
  /** Client-generated unique identifier for the ticket offer */
  ticketOfferGuid: string;
  
  /** Name of the ticket offer */
  name: string;
  
  /** Price per ticket (must be > 0) */
  price: number;
  
  /** Number of tickets (must be > 0 and <= available capacity) */
  ticketCount: number;
}

/**
 * ShowCapacity interface matching the backend ShowCapacityDto
 */
export interface ShowCapacity {
  /** GUID of the show */
  showGuid: string;
  
  /** Total ticket count for the show */
  totalTickets: number;
  
  /** Sum of ticket counts across all offers */
  allocatedTickets: number;
  
  /** Remaining tickets available (totalTickets - allocatedTickets) */
  availableCapacity: number;
}
```

### 3.3 Authentication

All API calls use `credentials: 'include'` to send authentication cookies, following the existing pattern. Unauthenticated requests return 401 and are handled by [`ProtectedRoute`](../../src/GloboTicket.Web/src/components/routing/ProtectedRoute.tsx).

---

## 4. Validation and Error Handling

### 4.1 Client-Side Validation

**Validation Strategy**: Validate on blur and submit to provide immediate feedback without being intrusive.

**Validation Implementation**:
```typescript
const validateForm = (): Record<string, string> => {
  const errors: Record<string, string> = {};
  
  // Name validation
  if (!name.trim()) {
    errors.name = 'Offer name is required';
  } else if (name.length > 100) {
    errors.name = 'Offer name cannot exceed 100 characters';
  }
  
  // Price validation
  if (!price) {
    errors.price = 'Price is required';
  } else {
    const priceNum = parseFloat(price);
    if (isNaN(priceNum) || priceNum <= 0) {
      errors.price = 'Price must be greater than zero';
    }
  }
  
  // Ticket count validation
  if (!ticketCount) {
    errors.ticketCount = 'Ticket count is required';
  } else {
    const count = parseInt(ticketCount);
    if (isNaN(count) || count <= 0) {
      errors.ticketCount = 'Ticket count must be a positive number';
    } else if (count > availableCapacity) {
      errors.ticketCount = `Ticket count exceeds available capacity. Only ${availableCapacity} tickets remain available.`;
    }
  }
  
  return errors;
};
```

**Field-Level Validation**: Each field validates independently and shows errors inline below the input.

**Form-Level Validation**: All fields validated before submission. Form submission prevented if any errors exist.

### 4.2 Server-Side Validation

**Backend Validation**: The server performs authoritative validation including:
- Capacity validation within a transaction (prevents race conditions)
- Business rule enforcement
- Data type and format validation

**Error Response Handling**:
```typescript
try {
  const newOffer = await createTicketOffer(showGuid, dto);
  // Success handling
} catch (err) {
  if (err instanceof Error) {
    // Parse error message for specific field errors
    if (err.message.includes('capacity')) {
      setFieldErrors({ ticketCount: err.message });
    } else {
      setError(err.message);
    }
  }
}
```

### 4.3 Error Display Patterns

**Field Errors** (inline below input):
```tsx
{fieldErrors.name && (
  <Text size="sm" className="text-error mt-1" role="alert">
    {fieldErrors.name}
  </Text>
)}
```

**General Errors** (top of form):
```tsx
{error && (
  <div 
    className="p-4 rounded-lg bg-error/10 border border-error/20" 
    role="alert" 
    aria-live="polite"
  >
    <Text size="sm" className="text-error">
      {error}
    </Text>
  </div>
)}
```

**Loading States**:
```tsx
<Button
  variant="primary"
  isLoading={createMutation.isPending}
  disabled={createMutation.isPending}
>
  Create Offer
</Button>
```

**Empty States**:
```tsx
<EmptyState
  icon={<Ticket className="w-12 h-12" />}
  title="No ticket offers yet"
  description="Create your first ticket offer to start selling tickets for this show."
/>
```

### 4.4 Error Recovery

**User Actions**:
- Errors retain form data for correction
- Clear error messages on field change
- Allow immediate retry after fixing errors
- Provide clear guidance on what needs to be fixed

**Automatic Recovery**:
- Capacity information auto-refreshes after other users create offers
- Stale data automatically refetched by TanStack Query
- Network errors trigger automatic retry (TanStack Query default)

---

## 5. User Interaction Flows

### 5.1 Create Ticket Offer Flow

```
1. User navigates to ShowDetailPage
   |
2. Page loads three data sources in parallel:
   a. Show details (existing)
   b. Ticket offers list (new)
   c. Capacity information (new)
   |
3. User sees capacity information:
   - Total tickets: 1000
   - Allocated: 600
   - Available: 400
   |
4. User fills out TicketOfferForm:
   a. Enter name: "VIP"
   b. Enter price: "150.00"
   c. Enter ticket count: "200"
   |
5. Client-side validation (on blur):
   |-- Name valid? --> Continue
   |-- Price valid? --> Continue
   |-- Ticket count <= 400? --> Continue
   |
6. User clicks "Create Offer"
   |
7. Form submits:
   a. Generate ticketOfferGuid: crypto.randomUUID()
   b. POST /api/shows/{showGuid}/ticket-offers
   |
8. API Response:
   |-- 201 Created:
   |   |-- Invalidate ticket offers query
   |   |-- Invalidate capacity query
   |   |-- Automatic refetch
   |   |-- New offer appears in list
   |   |-- Capacity updates: Available now 200
   |   |-- Form clears
   |   |-- Success feedback (implicit via UI update)
   |
   |-- 400 Bad Request (capacity exceeded):
   |   |-- Display error: "Ticket count exceeds available capacity. Only 150 tickets remain available."
   |   |-- Keep form data for correction
   |   |-- Highlight ticketCount field
   |
   +-- 404 Not Found:
       |-- Display error: "Show not found"
       |-- Disable form
```

### 5.2 Capacity Exceeded Flow

```
1. User attempts to create offer with 300 tickets
   |
2. Available capacity is only 200 tickets
   |
3. Client-side validation catches:
   |-- Display error: "Ticket count exceeds available capacity. Only 200 tickets remain available."
   |-- Prevent form submission
   |-- Highlight ticketCount field
   |
4. User corrects ticket count to 200
   |-- Error clears automatically
   |
5. Form submits successfully
```

### 5.3 Concurrent Offer Creation Flow

```
1. User A loads form: Available capacity = 400
   |
2. User B loads form: Available capacity = 400
   |
3. User A creates offer with 300 tickets
   |-- Server validates: 400 available, 300 requested --> OK
   |-- Offer created, capacity now 100
   |-- User A's UI updates automatically
   |
4. User B submits offer with 200 tickets
   |-- Server validates: 100 available, 200 requested --> FAIL
   |-- Returns 400: "Ticket count exceeds available capacity. Only 100 tickets remain available."
   |
5. User B sees error message
   |-- Capacity display refreshes automatically (TanStack Query refetch)
   |-- Shows updated available capacity: 100
   |-- User corrects ticket count to 100
   |-- Resubmits successfully
```

---

## 6. Implementation Checklist

### 6.1 Phase 1: Type Definitions and API Client

- [ ] Create [`src/GloboTicket.Web/src/types/ticketOffer.ts`](../../src/GloboTicket.Web/src/types/ticketOffer.ts)
  - [ ] Define `TicketOffer` interface
  - [ ] Define `CreateTicketOfferDto` interface
  - [ ] Define `ShowCapacity` interface
  - [ ] Add JSDoc comments
  - [ ] Export types

- [ ] Update [`src/GloboTicket.Web/src/api/client.ts`](../../src/GloboTicket.Web/src/api/client.ts)
  - [ ] Import ticket offer types
  - [ ] Add `getTicketOffers(showGuid: string)` function
  - [ ] Add `getShowCapacity(showGuid: string)` function
  - [ ] Add `createTicketOffer(showGuid: string, dto: CreateTicketOfferDto)` function

- [ ] Update [`src/GloboTicket.Web/src/features/queryKeys.ts`](../../src/GloboTicket.Web/src/features/queryKeys.ts)
  - [ ] Add `ticketOffers.byShow(showGuid)` key
  - [ ] Add `capacity.byShow(showGuid)` key

### 6.2 Phase 2: TanStack Query Hooks

- [ ] Create [`src/GloboTicket.Web/src/features/ticketOffers/hooks/`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/) directory

- [ ] Create [`useTicketOffers.ts`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/useTicketOffers.ts)
  - [ ] Implement query hook
  - [ ] Add JSDoc comments
  - [ ] Export hook

- [ ] Create [`useShowCapacity.ts`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/useShowCapacity.ts)
  - [ ] Implement query hook
  - [ ] Add JSDoc comments
  - [ ] Export hook

- [ ] Create [`useCreateTicketOffer.ts`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/useCreateTicketOffer.ts)
  - [ ] Implement mutation hook
  - [ ] Add query invalidation on success
  - [ ] Add JSDoc comments
  - [ ] Export hook

- [ ] Create [`index.ts`](../../src/GloboTicket.Web/src/features/ticketOffers/hooks/index.ts)
  - [ ] Barrel export all hooks

### 6.3 Phase 3: Molecule Components

- [ ] Create [`src/GloboTicket.Web/src/components/molecules/CapacityDisplay.tsx`](../../src/GloboTicket.Web/src/components/molecules/CapacityDisplay.tsx)
  - [ ] Define `CapacityDisplayProps` interface
  - [ ] Implement component with Stack layout
  - [ ] Display total, allocated, available metrics
  - [ ] Add progress bar visualization
  - [ ] Add warning indicator for low capacity
  - [ ] Add success indicator for full allocation
  - [ ] Add ARIA attributes for accessibility
  - [ ] Add responsive styling

- [ ] Create [`src/GloboTicket.Web/src/components/molecules/TicketOfferCard.tsx`](../../src/GloboTicket.Web/src/components/molecules/TicketOfferCard.tsx)
  - [ ] Define `TicketOfferCardProps` interface
  - [ ] Implement component with card styling
  - [ ] Display offer name
  - [ ] Display price (formatted as currency)
  - [ ] Display ticket count (formatted with locale)
  - [ ] Add consistent spacing

- [ ] Update [`src/GloboTicket.Web/src/components/molecules/index.ts`](../../src/GloboTicket.Web/src/components/molecules/index.ts)
  - [ ] Export `CapacityDisplay`
  - [ ] Export `TicketOfferCard`

### 6.4 Phase 4: Organism Components

- [ ] Create [`src/GloboTicket.Web/src/components/organisms/TicketOffersList.tsx`](../../src/GloboTicket.Web/src/components/organisms/TicketOffersList.tsx)
  - [ ] Define `TicketOffersListProps` interface
  - [ ] Import `useTicketOffers` hook
  - [ ] Implement loading state (Spinner)
  - [ ] Implement error state (error message)
  - [ ] Implement empty state (EmptyState component)
  - [ ] Implement success state (list of TicketOfferCard)
  - [ ] Add ARIA attributes

- [ ] Create [`src/GloboTicket.Web/src/components/organisms/TicketOfferForm.tsx`](../../src/GloboTicket.Web/src/components/organisms/TicketOfferForm.tsx)
  - [ ] Define `TicketOfferFormProps` interface
  - [ ] Implement form state management
  - [ ] Implement validation logic
  - [ ] Add offer name input field
  - [ ] Add price input field (with $ prefix)
  - [ ] Add ticket count input field (with capacity helper text)
  - [ ] Add field-level error display
  - [ ] Add general error display
  - [ ] Add Create Offer button (with loading state)
  - [ ] Add Cancel button
  - [ ] Implement submit handler with mutation
  - [ ] Implement cancel handler (clear form)
  - [ ] Add ARIA attributes for accessibility
  - [ ] Add responsive layout

- [ ] Update [`src/GloboTicket.Web/src/components/organisms/index.ts`](../../src/GloboTicket.Web/src/components/organisms/index.ts)
  - [ ] Export `TicketOffersList`
  - [ ] Export `TicketOfferForm`

### 6.5 Phase 5: ShowDetailPage Enhancement

- [ ] Update [`src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx`](../../src/GloboTicket.Web/src/pages/shows/ShowDetailPage.tsx)
  - [ ] Import ticket offer components
  - [ ] Import ticket offer hooks
  - [ ] Add capacity data fetching: `useShowCapacity(id)`
  - [ ] Add ticket offers data fetching: `useTicketOffers(id)`
  - [ ] Add Capacity Information card section
  - [ ] Add Create Ticket Offer card section
  - [ ] Add Ticket Offers card section
  - [ ] Handle loading states for new sections
  - [ ] Handle error states for new sections

### 6.6 Phase 6: Testing and Validation

- [ ] Manual Testing
  - [ ] Navigate to show detail page
  - [ ] Verify capacity information displays correctly
  - [ ] Verify ticket offers list displays (empty state if no offers)
  - [ ] Create first ticket offer
  - [ ] Verify offer appears in list
  - [ ] Verify capacity updates
  - [ ] Create second ticket offer
  - [ ] Verify both offers display
  - [ ] Verify capacity updates again
  - [ ] Test validation errors (empty fields, invalid values)
  - [ ] Test capacity exceeded error
  - [ ] Test form cancel (clears form)
  - [ ] Test loading states
  - [ ] Test error states (network error, 404, etc.)

- [ ] Accessibility Testing
  - [ ] Keyboard navigation works
  - [ ] Screen reader announces content
  - [ ] Focus indicators visible
  - [ ] ARIA attributes correct
  - [ ] Error messages announced
  - [ ] Loading states announced
  - [ ] Progress bar accessible

- [ ] Responsive Testing
  - [ ] Test on mobile viewport (<768px)
  - [ ] Test on tablet viewport (768-1024px)
  - [ ] Test on desktop viewport (>1024px)
  - [ ] Verify layout adapts correctly
  - [ ] Verify form fields stack properly on mobile

- [ ] Cross-Browser Testing
  - [ ] Test in Chrome
  - [ ] Test in Firefox
  - [ ] Test in Safari
  - [ ] Test in Edge

---

## 7. Architecture Validation

### 7.1 Design System Compliance

✅ **Atomic Design**: Uses existing atoms and molecules, new molecules and organisms follow hierarchy  
✅ **Layout Primitives**: Uses Stack, Row for layout (no custom margins)  
✅ **Theme Tokens**: Uses design tokens for colors, spacing, typography  
✅ **Styling Strategy**: Uses Tailwind CSS with consistent conventions  
✅ **Accessibility**: Semantic HTML, ARIA attributes, keyboard navigation  
✅ **Responsiveness**: Mobile-first approach with defined breakpoints  

### 7.2 Consistency with Existing Code

✅ **State Management**: Follows TanStack Query pattern from shows and venues features  
✅ **API Client**: Follows existing pattern from `getShow()`, `getVenue()`, `getAct()`  
✅ **Form Pattern**: Follows existing pattern from [`ShowForm`](../../src/GloboTicket.Web/src/components/organisms/ShowForm.tsx) and [`VenueForm`](../../src/GloboTicket.Web/src/components/organisms/VenueForm.tsx)  
✅ **Error Handling**: Follows existing pattern from other forms  
✅ **TypeScript**: Follows existing pattern from Show, Venue, Act interfaces  
✅ **Component Structure**: Follows existing organism patterns  

### 7.3 Architecture Principles

✅ **Separation of Concerns**: Clear separation between data (hooks), presentation (components), and routing (pages)  
✅ **Reusability**: Components are reusable and composable  
✅ **Type Safety**: TypeScript interfaces for all data structures  
✅ **Maintainability**: Clear file structure and naming conventions  
✅ **Testability**: Components designed for easy testing  
✅ **Performance**: TanStack Query provides caching and automatic refetching  

---

## 8. Next Steps

### 8.1 Implementation Order

1. **Phase 1**: Create type definitions and API client functions
2. **Phase 2**: Create TanStack Query hooks
3. **Phase 3**: Create molecule components (CapacityDisplay, TicketOfferCard)
4. **Phase 4**: Create organism components (TicketOffersList, TicketOfferForm)
5. **Phase 5**: Enhance ShowDetailPage with new components
6. **Phase 6**: Test and validate

### 8.2 Delegation

**Recommended Mode**: [`design-system-engineer`](../../.kilocode/modes/design-system-engineer.md) mode for Phase 3 (molecules)

**Rationale**: The molecule components (CapacityDisplay, TicketOfferCard) are new UI components that need to be added to the design system. The design-system-engineer mode is specialized for creating reusable UI components following Atomic Design principles.

**Alternative**: [`product-developer`](../../.kilocode/modes/product-developer.md) mode for Phases 4-5 (organisms and page integration)

**Rationale**: Once the molecules exist, the product-developer mode can assemble the feature by creating the organism components and integrating them into the ShowDetailPage.

### 8.3 Success Criteria

**Feature Complete When**:
- [ ] User can view capacity information on show detail page
- [ ] User can create ticket offers with validation
- [ ] User can view list of existing ticket offers
- [ ] Capacity information updates after creating offers
- [ ] Client-side validation prevents invalid submissions
- [ ] Server-side validation errors display correctly
- [ ] Loading and error states work correctly
- [ ] Empty state displays when no offers exist
- [ ] Accessibility requirements met
- [ ] Responsive design works on all screen sizes
- [ ] Code follows existing patterns and conventions

---

**Document Status**: Ready for implementation  
**Last Updated**: 2026-01-03  
**Next Review**: After implementation complete
