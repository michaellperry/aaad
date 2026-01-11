# Permission Guards

Use when rendering privileged UI or protecting routes.

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
