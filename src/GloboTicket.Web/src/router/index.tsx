import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from '../components/templates';
import { DashboardPage, LoginPage } from '../pages';
import {
  VenuesPage,
  VenueDetailPage,
  CreateVenuePage,
  EditVenuePage,
} from '../pages/venues';
import {
  ActsPage,
  ActDetailPage,
  CreateActPage,
  EditActPage,
} from '../pages/acts';
import { ProtectedRoute } from '../components/routing/ProtectedRoute';
import { ROUTES } from './routes';

/**
 * 404 Not Found page component
 */
const NotFoundPage = () => (
  <div className="flex items-center justify-center min-h-[60vh]">
    <div className="text-center">
      <h1 className="text-6xl font-bold text-text-secondary mb-4">404</h1>
      <p className="text-xl text-text-muted mb-8">Page not found</p>
      <a
        href={ROUTES.DASHBOARD}
        className="text-brand-primary hover:text-brand-primary-hover underline"
      >
        Return to Dashboard
      </a>
    </div>
  </div>
);

/**
 * Application router configuration
 */
export const router = createBrowserRouter([
  // Public login route (outside AppLayout)
  {
    path: ROUTES.LOGIN,
    element: <LoginPage />,
  },
  
  // Protected routes (inside AppLayout)
  {
    path: '/',
    element: <AppLayout />,
    children: [
      // Dashboard
      {
        index: true,
        element: (
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        ),
      },
      
      // Venues
      {
        path: ROUTES.VENUES,
        element: (
          <ProtectedRoute>
            <VenuesPage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.VENUE_CREATE,
        element: (
          <ProtectedRoute>
            <CreateVenuePage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.VENUE_DETAIL,
        element: (
          <ProtectedRoute>
            <VenueDetailPage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.VENUE_EDIT,
        element: (
          <ProtectedRoute>
            <EditVenuePage />
          </ProtectedRoute>
        ),
      },
      
      // Acts
      {
        path: ROUTES.ACTS,
        element: (
          <ProtectedRoute>
            <ActsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.ACT_CREATE,
        element: (
          <ProtectedRoute>
            <CreateActPage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.ACT_DETAIL,
        element: (
          <ProtectedRoute>
            <ActDetailPage />
          </ProtectedRoute>
        ),
      },
      {
        path: ROUTES.ACT_EDIT,
        element: (
          <ProtectedRoute>
            <EditActPage />
          </ProtectedRoute>
        ),
      },
      
      // 404 Not Found
      {
        path: '404',
        element: <NotFoundPage />,
      },
      {
        path: '*',
        element: <Navigate to="/404" replace />,
      },
    ],
  },
]);