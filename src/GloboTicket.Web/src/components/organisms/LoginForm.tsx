import { useState, type FormEvent } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Button } from '../atoms/Button';
import { Heading } from '../atoms/Heading';
import { Text } from '../atoms/Text';
import { useAuth } from '../../hooks/useAuth';
import { ROUTES } from '../../router/routes';

export interface LoginFormProps {
  /**
   * Callback fired on successful login
   */
  onSuccess?: () => void;
  
  /**
   * Callback fired on login error
   */
  onError?: (error: string) => void;
}

/**
 * Login form organism component.
 * Handles user authentication with username and password.
 * 
 * @example
 * ```tsx
 * <LoginForm onSuccess={() => navigate('/dashboard')} />
 * ```
 */
export const LoginForm = ({ onSuccess, onError }: LoginFormProps) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    // Get form data directly from the form element as fallback
    const formData = new FormData(e.currentTarget);
    const formUsername = formData.get('username') as string || '';
    const formPassword = formData.get('password') as string || '';
    
    // Use form values if state values are empty (for browser automation compatibility)
    const actualUsername = username.trim() || formUsername.trim();
    const actualPassword = password || formPassword;
    
    // Clear previous errors
    setError(null);
    
    // Validate fields
    if (!actualUsername) {
      const errorMsg = 'Username is required';
      setError(errorMsg);
      onError?.(errorMsg);
      return;
    }
    
    if (!actualPassword) {
      const errorMsg = 'Password is required';
      setError(errorMsg);
      onError?.(errorMsg);
      return;
    }
    
    setIsLoading(true);
    
    try {
      const response = await login(actualUsername, actualPassword);
      
      if (response.success) {
        onSuccess?.();
        // Redirect to the original attempted URL or dashboard
        const from = (location.state as { from?: string })?.from || ROUTES.DASHBOARD;
        navigate(from, { replace: true });
      } else {
        const errorMsg = response.message || 'Login failed';
        setError(errorMsg);
        onError?.(errorMsg);
      }
    } catch (err) {
      const errorMsg = err instanceof Error ? err.message : 'An unexpected error occurred';
      setError(errorMsg);
      onError?.(errorMsg);
    } finally {
      setIsLoading(false);
    }
  };

  const handleUsernameChange = (value: string) => {
    setUsername(value);
    // Clear error when user starts typing
    if (error) {
      setError(null);
    }
  };

  const handlePasswordChange = (value: string) => {
    setPassword(value);
    // Clear error when user starts typing
    if (error) {
      setError(null);
    }
  };

  return (
    <div className="w-full max-w-md">
      <div className="mb-8 text-center">
        <Heading level="h1" className="mb-2">
          Welcome Back
        </Heading>
        <Text variant="muted">
          Sign in to your GloboTicket account
        </Text>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Username Field */}
        <div>
          <label
            htmlFor="username"
            className="block text-sm font-medium text-text-primary mb-2"
          >
            Username
          </label>
          <input
            id="username"
            name="username"
            type="text"
            autoComplete="username"
            required
            value={username}
            onChange={(e) => handleUsernameChange(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-tertiary focus:outline-none focus:ring-2 focus:ring-brand-primary focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            placeholder="Enter your username"
            aria-invalid={error ? 'true' : 'false'}
            aria-describedby={error ? 'login-error' : undefined}
          />
        </div>

        {/* Password Field */}
        <div>
          <label
            htmlFor="password"
            className="block text-sm font-medium text-text-primary mb-2"
          >
            Password
          </label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            required
            value={password}
            onChange={(e) => handlePasswordChange(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-tertiary focus:outline-none focus:ring-2 focus:ring-brand-primary focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            placeholder="Enter your password"
            aria-invalid={error ? 'true' : 'false'}
            aria-describedby={error ? 'login-error' : undefined}
          />
        </div>

        {/* Error Message */}
        {error && (
          <div
            id="login-error"
            role="alert"
            className="p-3 rounded-lg bg-error/10 border border-error/20"
          >
            <Text size="sm" className="text-error">
              {error}
            </Text>
          </div>
        )}

        {/* Submit Button */}
        <Button
          type="submit"
          variant="primary"
          size="lg"
          fullWidth
          isLoading={isLoading}
          disabled={isLoading}
        >
          {isLoading ? 'Signing in...' : 'Sign In'}
        </Button>

        {/* Test Credentials Hint */}
        <div className="mt-4 p-3 rounded-lg bg-surface-elevated border border-border-default">
          <Text size="xs" variant="muted" className="text-center">
            Test credentials: <strong>prod</strong> / <strong>prod123</strong>, <strong>smoke</strong> / <strong>smoke123</strong>, or <strong>playwright</strong> / <strong>playwright123</strong>
          </Text>
        </div>
      </form>
    </div>
  );
};