/**
 * Date and time formatting utilities
 * 
 * Provides consistent date/time formatting across the application
 * using the browser's locale settings.
 */

/**
 * Format date in long format (e.g., "March 15, 2026")
 */
export function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

/**
 * Format time in 12-hour format (e.g., "7:30 PM")
 */
export function formatTime(dateString: string): string {
  return new Date(dateString).toLocaleTimeString('en-US', {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  });
}

/**
