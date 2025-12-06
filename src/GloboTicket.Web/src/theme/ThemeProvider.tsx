/**
 * Theme Provider
 * 
 * Provides theme context for the application with light/dark mode support.
 */

import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';

type Theme = 'light' | 'dark';

interface ThemeContextValue {
  /** Current theme mode */
  theme: Theme;
  /** Set the theme mode */
  setTheme: (theme: Theme) => void;
  /** Toggle between light and dark mode */
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

interface ThemeProviderProps {
  /** Child components */
  children: ReactNode;
  /** Default theme (defaults to 'light') */
  defaultTheme?: Theme;
  /** Storage key for persisting theme preference */
  storageKey?: string;
}

/**
 * ThemeProvider component
 * 
 * Wraps the application to provide theme context and manage light/dark mode.
 * Persists theme preference to localStorage.
 * 
 * @example
 * ```tsx
 * <ThemeProvider defaultTheme="light">
 *   <App />
 * </ThemeProvider>
 * ```
 */
export function ThemeProvider({ 
  children, 
  defaultTheme = 'light',
  storageKey = 'globoticket-theme'
}: ThemeProviderProps) {
  const [theme, setTheme] = useState<Theme>(() => {
    // Try to load theme from localStorage
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem(storageKey);
      if (stored === 'light' || stored === 'dark') {
        return stored;
      }
    }
    return defaultTheme;
  });
  
  useEffect(() => {
    const root = window.document.documentElement;
    
    // Remove both classes first
    root.classList.remove('light', 'dark');
    
    // Add the current theme class
    root.classList.add(theme);
    
    // Persist to localStorage
    localStorage.setItem(storageKey, theme);
  }, [theme, storageKey]);
  
  const toggleTheme = () => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  };
  
  const value: ThemeContextValue = {
    theme,
    setTheme,
    toggleTheme,
  };
  
  return (
    <ThemeContext.Provider value={value}>
      {children}
    </ThemeContext.Provider>
  );
}

/**
 * useTheme hook
 * 
 * Access the current theme and theme controls.
 * Must be used within a ThemeProvider.
 * 
 * @example
 * ```tsx
 * function ThemeToggle() {
 *   const { theme, toggleTheme } = useTheme();
 *   return (
 *     <button onClick={toggleTheme}>
 *       Current theme: {theme}
 *     </button>
 *   );
 * }
 * ```
 * Note: Hook export is needed alongside component - fast refresh warning is acceptable
 */
// eslint-disable-next-line react-refresh/only-export-components
export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
}