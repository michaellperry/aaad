/**
 * Design Tokens
 * 
 * Core design tokens for the GloboTicket design system.
 * All tokens have explicit light and dark mode values.
 */

export const tokens = {
  colors: {
    // Brand colors
    brand: {
      primary: {
        light: '#3B82F6',      // Blue-500
        dark: '#60A5FA',       // Blue-400
      },
      secondary: {
        light: '#8B5CF6',      // Violet-500
        dark: '#A78BFA',       // Violet-400
      },
    },
    
    // Semantic colors
    success: {
      light: '#10B981',        // Green-500
      dark: '#34D399',         // Green-400
    },
    warning: {
      light: '#F59E0B',        // Amber-500
      dark: '#FCD34D',         // Amber-300
    },
    error: {
      light: '#EF4444',        // Red-500
      dark: '#F87171',         // Red-400
    },
    info: {
      light: '#3B82F6',        // Blue-500
      dark: '#60A5FA',         // Blue-400
    },
    
    // Surface colors
    surface: {
      base: {
        light: '#FFFFFF',
        dark: '#1F2937',       // Gray-800
      },
      elevated: {
        light: '#F9FAFB',      // Gray-50
        dark: '#374151',       // Gray-700
      },
      overlay: {
        light: '#F3F4F6',      // Gray-100
        dark: '#4B5563',       // Gray-600
      },
    },
    
    // Text colors
    text: {
      primary: {
        light: '#111827',      // Gray-900
        dark: '#F9FAFB',       // Gray-50
      },
      secondary: {
        light: '#6B7280',      // Gray-500
        dark: '#D1D5DB',       // Gray-300
      },
      tertiary: {
        light: '#9CA3AF',      // Gray-400
        dark: '#9CA3AF',       // Gray-400
      },
      inverse: {
        light: '#FFFFFF',
        dark: '#111827',       // Gray-900
      },
      disabled: {
        light: '#D1D5DB',      // Gray-300
        dark: '#6B7280',       // Gray-500
      },
    },
    
    // Border colors
    border: {
      default: {
        light: '#E5E7EB',      // Gray-200
        dark: '#4B5563',       // Gray-600
      },
      subtle: {
        light: '#F3F4F6',      // Gray-100
        dark: '#374151',       // Gray-700
      },
      strong: {
        light: '#D1D5DB',      // Gray-300
        dark: '#6B7280',       // Gray-500
      },
    },
  },
  
  spacing: {
    xs: '0.25rem',    // 4px
    sm: '0.5rem',     // 8px
    md: '1rem',       // 16px
    lg: '1.5rem',     // 24px
    xl: '2rem',       // 32px
    '2xl': '3rem',    // 48px
    '3xl': '4rem',    // 64px
  },
  
  typography: {
    fontFamily: {
      sans: 'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
      mono: 'ui-monospace, SFMono-Regular, "SF Mono", Menlo, Consolas, monospace',
    },
    fontSize: {
      xs: '0.75rem',      // 12px
      sm: '0.875rem',     // 14px
      base: '1rem',       // 16px
      lg: '1.125rem',     // 18px
      xl: '1.25rem',      // 20px
      '2xl': '1.5rem',    // 24px
      '3xl': '1.875rem',  // 30px
      '4xl': '2.25rem',   // 36px
    },
    fontWeight: {
      normal: '400',
      medium: '500',
      semibold: '600',
      bold: '700',
    },
    lineHeight: {
      tight: '1.25',
      normal: '1.5',
      relaxed: '1.75',
    },
  },
  
  borderRadius: {
    none: '0',
    sm: '0.25rem',    // 4px
    md: '0.375rem',   // 6px
    lg: '0.5rem',     // 8px
    xl: '0.75rem',    // 12px
    full: '9999px',
  },
  
  shadows: {
    sm: {
      light: '0 1px 2px 0 rgb(0 0 0 / 0.05)',
      dark: '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    },
    md: {
      light: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
      dark: '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.3)',
    },
    lg: {
      light: '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
      dark: '0 10px 15px -3px rgb(0 0 0 / 0.4), 0 4px 6px -4px rgb(0 0 0 / 0.3)',
    },
    xl: {
      light: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
      dark: '0 20px 25px -5px rgb(0 0 0 / 0.4), 0 8px 10px -6px rgb(0 0 0 / 0.3)',
    },
  },
  
  transitions: {
    fast: '150ms cubic-bezier(0.4, 0, 0.2, 1)',
    base: '200ms cubic-bezier(0.4, 0, 0.2, 1)',
    slow: '300ms cubic-bezier(0.4, 0, 0.2, 1)',
  },
  
  breakpoints: {
    sm: '640px',
    md: '768px',
    lg: '1024px',
    xl: '1280px',
    '2xl': '1536px',
  },
} as const;

export type Tokens = typeof tokens;