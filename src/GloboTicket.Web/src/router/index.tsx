import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from '../components/templates';
import { DashboardPage } from '../pages';
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
import {
  ShowsPage,
  ShowDetailPage,
  CreateShowPage,
  EditShowPage,
} from '../pages/shows';
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
  {
    path: '/',
    element: <AppLayout />,
    children: [
      // Dashboard
      {
        index: true,
        element: <DashboardPage />,
      },
      
      // Venues
      {
        path: ROUTES.VENUES,
        element: <VenuesPage />,
      },
      {
        path: ROUTES.VENUE_CREATE,
        element: <CreateVenuePage />,
      },
      {
        path: ROUTES.VENUE_DETAIL,
        element: <VenueDetailPage />,
      },
      {
        path: ROUTES.VENUE_EDIT,
        element: <EditVenuePage />,
      },
      
      // Acts
      {
        path: ROUTES.ACTS,
        element: <ActsPage />,
      },
      {
        path: ROUTES.ACT_CREATE,
        element: <CreateActPage />,
      },
      {
        path: ROUTES.ACT_DETAIL,
        element: <ActDetailPage />,
      },
      {
        path: ROUTES.ACT_EDIT,
        element: <EditActPage />,
      },
      
      // Shows
      {
        path: ROUTES.SHOWS,
        element: <ShowsPage />,
      },
      {
        path: ROUTES.SHOW_CREATE,
        element: <CreateShowPage />,
      },
      {
        path: ROUTES.SHOW_DETAIL,
        element: <ShowDetailPage />,
      },
      {
        path: ROUTES.SHOW_EDIT,
        element: <EditShowPage />,
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