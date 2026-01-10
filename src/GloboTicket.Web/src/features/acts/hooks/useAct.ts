import { useQuery } from '@tanstack/react-query';
import { getAct } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Act } from '../../../types/act';

/**
 * Hook to fetch a single act by ID
 * @param id - The act GUID
 * @returns Query result with act data, loading state, and error
 */
export function useAct(id: string | undefined) {
  return useQuery<Act, Error>({
    queryKey: queryKeys.acts.detail(id || ''),
    queryFn: () => getAct(id!),
    enabled: !!id,
  });
}
