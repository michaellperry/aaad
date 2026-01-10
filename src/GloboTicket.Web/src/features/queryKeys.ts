export const queryKeys = {
  venues: {
    all: ['venues'] as const,
    count: ['venues', 'count'] as const,
    detail: (id: string) => ['venues', id] as const,
  },
  acts: {
    all: ['acts'] as const,
    count: ['acts', 'count'] as const,
    detail: (id: string) => ['acts', id] as const,
    shows: (actGuid: string) => ['acts', actGuid, 'shows'] as const,
  },
  shows: {
    detail: (id: string) => ['shows', id] as const,
    byAct: (actGuid: string) => ['shows', 'by-act', actGuid] as const,
  },
};
