# Routing Setup (React Router 7)

Use when defining page structure and loader/protection patterns.

## File-Based Routing Structure
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

## Protected Routes
```typescript
function ProtectedRoute({ children, requiredPermissions }: { children: React.ReactNode; requiredPermissions: string[] }) {
  const { user, permissions } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  const hasPermission = requiredPermissions.some(p => permissions.includes(p))
  if (!hasPermission) return <Navigate to="/unauthorized" replace />
  return <>{children}</>
}

// Usage
<Route path="/venues/new" element={
  <ProtectedRoute requiredPermissions={['venue-manager', 'admin']}>
    <CreateVenue />
  </ProtectedRoute>
} />
```

## Data Loaders
```typescript
export async function venueLoader({ params }: LoaderFunctionArgs) {
  const venueId = params.id!
  const venue = await queryClient.ensureQueryData({
    queryKey: venueQueries.detail(venueId),
    queryFn: () => venueApi.getById(venueId),
  })
  return { venue }
}

<Route path="/venues/:id" loader={venueLoader} element={<VenueDetail />} />
```
