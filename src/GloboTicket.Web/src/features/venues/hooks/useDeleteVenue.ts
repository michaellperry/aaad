import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteVenue } from '../../../api/client';
import { queryKeys } from '../../queryKeys';

/**
 * Hook to delete a venue
 * Invalidates the venues list query on success to refresh the data
 * @returns Mutation result with mutate function, loading state, and error
 */
export function useDeleteVenue() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteVenue(id),
    onSuccess: () => {
      // Invalidate venues list to trigger a refetch
      queryClient.invalidateQueries({ queryKey: queryKeys.venues.all });
    },
  });
}
