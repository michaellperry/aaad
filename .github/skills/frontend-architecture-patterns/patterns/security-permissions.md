# Security and Permissions

Use when designing permission checks and guards at route and component levels.

## Permission-Based Rendering

```typescript
function usePermissions() {
  const { user } = useAuth()
  return {
    canCreate: user?.permissions.includes('venue-manager'),
    canEdit: user?.permissions.includes('venue-manager'),
    canDelete: user?.permissions.includes('admin'),
    isAdmin: user?.permissions.includes('admin')
  }
}

function VenueActions({ venue }: { venue: Venue }) {
  const { canEdit, canDelete } = usePermissions()
  return (
    <div className="venue-actions">
      {canEdit && (
        <Button onClick={() => navigate(`/venues/${venue.id}/edit`)}>Edit</Button>
      )}
      {canDelete && (
        <Button variant="danger" onClick={() => deleteVenue(venue.id)}>Delete</Button>
      )}
    </div>
  )
}
```

## Route-Level Security

```typescript
const routePermissions = {
  '/venues': { read: ['tenant-member'] },
  '/venues/new': { create: ['venue-manager', 'admin'] },
  '/venues/:id/edit': { update: ['venue-manager', 'admin'] },
  '/venues/:id/delete': { delete: ['admin'] }
}

function useRoutePermission(path: string, action: string) {
  const { permissions } = useAuth()
  const routeConfig = routePermissions[path]
  return routeConfig?.[action]?.some(permission => permissions.includes(permission)) ?? false
}
```
