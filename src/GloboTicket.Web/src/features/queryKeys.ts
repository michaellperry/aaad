export const queryKeys = {
  venues: {
    all: ['venues'] as const,
    detail: (id: string) => ['venues', id] as const,
  },
  acts: {
    all: ['acts'] as const,
    detail: (id: string) => ['acts', id] as const,
    shows: (actGuid: string) => ['acts', actGuid, 'shows'] as const,
  },
  shows: {
    detail: (id: string) => ['shows', id] as const,
    byAct: (actGuid: string) => ['shows', 'by-act', actGuid] as const,
  },
};
