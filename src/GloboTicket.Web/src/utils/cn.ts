/**
 * Class Name Utility
 * 
 * Merges Tailwind CSS classes intelligently, handling conflicts.
 * Uses clsx for conditional classes and tailwind-merge for deduplication.
 */

import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

/**
 * Merge class names with Tailwind-aware deduplication
 * 
 * @param inputs - Class names to merge
 * @returns Merged class string
 * 
 * @example
 * ```tsx
 * cn('px-4 py-2', 'px-6') // => 'py-2 px-6'
 * cn('text-red-500', condition && 'text-blue-500') // => 'text-blue-500' if condition is true
 * ```
 */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}