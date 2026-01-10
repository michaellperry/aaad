import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteAct } from '../../../api/client';
import { queryKeys } from '../../queryKeys';

/**
 * Hook to delete an act
 * Invalidates the acts list query on success to refresh the data
 * @returns Mutation result with mutate function, loading state, and error
 */
export function useDeleteAct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => deleteAct(id),
    onSuccess: () => {
      // Invalidate acts list to trigger a refetch
      queryClient.invalidateQueries({ queryKey: queryKeys.acts.all });
    },
  });
}
