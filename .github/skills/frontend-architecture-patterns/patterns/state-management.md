# State Management (TanStack Query)

Use when planning query keys, mutations, and error handling for a feature.

## Query Key Factory
Centralize keys to avoid collisions and enable typed keys.

```typescript
export const queryKeys = {
  venues: {
    all: ['venues'] as const,
    lists: () => [...queryKeys.venues.all, 'list'] as const,
    list: (filters: VenueFilters) => [...queryKeys.venues.lists(), { filters }] as const,
    details: () => [...queryKeys.venues.all, 'detail'] as const,
    detail: (id: string) => [...queryKeys.venues.details(), id] as const,
  },
  acts: {
    all: ['acts'] as const,
    byVenue: (venueId: string) => [...queryKeys.acts.all, 'venue', venueId] as const,
  }
}
```

## Query Patterns

```typescript
function useVenues(filters: VenueFilters = {}) {
  return useQuery({
    queryKey: queryKeys.venues.list(filters),
    queryFn: () => venueApi.getList(filters),
    staleTime: 5 * 60 * 1000,
  })
}

function useVenue(id: string) {
  return useQuery({
    queryKey: queryKeys.venues.detail(id),
    queryFn: () => venueApi.getById(id),
    enabled: !!id,
  })
}
```

## Mutation Patterns

```typescript
function useCreateVenue() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: venueApi.create,
    onMutate: async (newVenue) => {
      await queryClient.cancelQueries({ queryKey: queryKeys.venues.lists() })
      const previousVenues = queryClient.getQueryData(queryKeys.venues.lists())
      queryClient.setQueryData(queryKeys.venues.lists(), (old: any) => old ? [...old, { ...newVenue, id: 'temp' }] : [newVenue])
      return { previousVenues }
    },
    onError: (err, newVenue, context) => {
      if (context?.previousVenues) {
        queryClient.setQueryData(queryKeys.venues.lists(), context.previousVenues)
      }
    },
    onSuccess: (venue) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.venues.lists() })
      queryClient.setQueryData(queryKeys.venues.detail(venue.id), venue)
    }
  })
}
```

## Error Handling

```typescript
function useVenuesWithErrorHandling() {
  return useQuery({
    queryKey: queryKeys.venues.lists(),
    queryFn: venueApi.getList,
    onError: (error) => {
      if (error.status === 401) {
        navigate('/login')
      } else {
        toast.error('Failed to load venues')
      }
    },
    retry: (failureCount, error) => {
      if (error.status === 401 || error.status === 403) return false
      return failureCount < 3
    }
  })
}
```
