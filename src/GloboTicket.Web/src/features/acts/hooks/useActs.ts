import { useQuery } from '@tanstack/react-query';
import { getActs } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Act } from '../../../types/act';

/**
 * Hook to fetch all acts
 * @returns Query result with acts data, loading state, and error
 */
export function useActs() {
  return useQuery<Act[], Error>({
    queryKey: queryKeys.acts.all,
    queryFn: getActs,
  });
}
