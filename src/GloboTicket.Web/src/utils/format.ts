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
 * Construct ISO 8601 datetime string with timezone offset
 * @param date - Date string in YYYY-MM-DD format
 * @param time - Time string in HH:MM format
 * @returns ISO 8601 datetime string with timezone offset (e.g., "2025-07-15T20:00:00-04:00")
 */
export function constructDateTime(date: string, time: string): string {
  // Combine date and time
  const dateTimeString = `${date}T${time}:00`;
  
  // Create Date object (will use local timezone)
  const dateTime = new Date(dateTimeString);
  
  // Get timezone offset in minutes
  const offsetMinutes = dateTime.getTimezoneOffset();
  
  // Convert offset to hours and minutes
  const offsetHours = Math.floor(Math.abs(offsetMinutes) / 60);
  const offsetMins = Math.abs(offsetMinutes) % 60;
  
  // Format offset as +/-HH:MM
  const offsetSign = offsetMinutes <= 0 ? '+' : '-';
  const offsetString = `${offsetSign}${String(offsetHours).padStart(2, '0')}:${String(offsetMins).padStart(2, '0')}`;
  
  // Construct ISO 8601 string with offset
  return `${dateTimeString}${offsetString}`;
}
