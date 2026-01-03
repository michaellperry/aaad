/**
 * Route path constants for the application
 */
export const ROUTES = {
  // Auth
  LOGIN: '/login',
  
  // Dashboard
  DASHBOARD: '/',
  
  // Venues
  VENUES: '/venues',
  VENUE_DETAIL: '/venues/:id',
  VENUE_CREATE: '/venues/new',
  VENUE_EDIT: '/venues/:id/edit',
  
  // Acts
  ACTS: '/acts',
  ACT_DETAIL: '/acts/:id',
  ACT_CREATE: '/acts/new',
  ACT_EDIT: '/acts/:id/edit',
  
  // Shows
  SHOWS: '/shows',
  SHOW_DETAIL: '/shows/:id',
} as const;

/**
 * Helper functions to generate route paths with parameters
 */
export const routeHelpers = {
  venueDetail: (id: string) => `/venues/${id}`,
  venueEdit: (id: string) => `/venues/${id}/edit`,
  
  actDetail: (id: string) => `/acts/${id}`,
  actEdit: (id: string) => `/acts/${id}/edit`,
  
  showDetail: (id: string) => `/shows/${id}`,
};