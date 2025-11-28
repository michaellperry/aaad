import { type ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { Spinner } from '../atoms/Spinner';
import { ROUTES } from '../../router/routes';

export interface ProtectedRouteProps {
  /**
   * Child components to render if authenticated
   */
  children: ReactNode;
}

/**
 * Protected route wrapper component that checks authentication status.
 * Redirects to login page if user is not authenticated.
 * Shows loading spinner while checking authentication.
 * Preserves the attempted URL to redirect back after login.
 * 
 * @example
 * ```tsx
 * <ProtectedRoute>
 *   <DashboardPage />
 * </ProtectedRoute>
 * ```
 */
export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  // Show loading spinner while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Spinner size="lg" label="Checking authentication..." />
      </div>
    );
  }

  // Redirect to login if not authenticated, preserving the attempted URL
  if (!isAuthenticated) {
    return (
      <Navigate
        to={ROUTES.LOGIN}
        state={{ from: location.pathname }}
        replace
      />
    );
  }

  // Render children if authenticated
  return <>{children}</>;
};