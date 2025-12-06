import { useState, useEffect, Fragment } from 'react';
import { Combobox, Transition } from '@headlessui/react';
import { Search } from 'lucide-react';
import { Icon } from '../atoms/Icon';
import { Spinner } from '../atoms/Spinner';
import { searchAddresses } from '../../api/geocoding';
import { RateLimitError } from '../../types/geocoding';
import { cn } from '../../utils';
import type { GeocodeResult } from '../../types/geocoding';

export interface AddressSearchInputProps {
  /**
   * Callback when a location is selected
   */
  onSelect: (result: GeocodeResult) => void;
  
  /**
   * Optional placeholder text
   */
  placeholder?: string;
  
  /**
   * Optional className for styling
   */
  className?: string;
}

/**
 * Address search input component with autocomplete.
 * Uses Headless UI Combobox for accessible autocomplete functionality.
 * Debounces search queries and displays results in a dropdown.
 */
export function AddressSearchInput({
  onSelect,
  placeholder = 'Search for a location...',
  className,
}: AddressSearchInputProps) {
  const [query, setQuery] = useState('');
  const [debouncedQuery, setDebouncedQuery] = useState('');
  const [results, setResults] = useState<GeocodeResult[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Debounce search query
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedQuery(query);
    }, 300);

    return () => clearTimeout(timer);
  }, [query]);

  // Perform search when debounced query changes
  useEffect(() => {
    if (!debouncedQuery.trim()) {
      // Clear state when query is empty - necessary to reset UI state
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setResults([]);
      setError(null);
      setIsLoading(false);
      return;
    }

    let cancelled = false;

    setIsLoading(true);
    setError(null);

    searchAddresses(debouncedQuery)
      .then((data) => {
        if (!cancelled) {
          setResults(data);
          setError(null);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          if (err instanceof RateLimitError) {
            setError(`Rate limit exceeded. Please try again in ${err.retryAfter} second(s).`);
          } else {
            setError(err instanceof Error ? err.message : 'Failed to search addresses');
          }
          setResults([]);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setIsLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [debouncedQuery]);

  const handleSelect = (result: GeocodeResult | null) => {
    if (result) {
      onSelect(result);
      setQuery('');
      setResults([]);
    }
  };

  return (
    <div className={cn('relative', className)}>
      <Combobox value={null} onChange={handleSelect}>
        <div className="relative">
          <div className="relative">
            <Combobox.Input
              className={cn(
                'w-full px-4 py-2 pl-10 pr-10',
                'rounded-lg border border-border-default',
                'bg-surface-base text-text-primary',
                'placeholder-text-muted',
                'focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent',
                'disabled:opacity-50 disabled:cursor-not-allowed'
              )}
              placeholder={placeholder}
              displayValue={() => query}
              onChange={(event) => setQuery(event.target.value)}
              aria-label="Search for a location"
            />
            <div className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
              <Icon icon={Search} size="sm" className="text-text-muted" />
            </div>
            {isLoading && (
              <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                <Spinner size="sm" />
              </div>
            )}
          </div>

          <Transition
            as={Fragment}
            leave="transition ease-in duration-100"
            leaveFrom="opacity-100"
            leaveTo="opacity-0"
            afterLeave={() => setQuery('')}
          >
            <Combobox.Options
              className={cn(
                'absolute z-10 mt-1 w-full',
                'max-h-60 overflow-auto',
                'rounded-lg border border-border-default',
                'bg-surface-base shadow-lg',
                'focus:outline-none',
                'py-1'
              )}
            >
              {error && (
                <div className="px-4 py-2 text-sm text-error">
                  {error}
                </div>
              )}
              {!error && results.length === 0 && debouncedQuery && !isLoading && (
                <div className="px-4 py-2 text-sm text-text-secondary">
                  No results found
                </div>
              )}
              {results.map((result) => (
                <Combobox.Option
                  key={result.id}
                  value={result}
                  className={({ active }) =>
                    cn(
                      'relative cursor-pointer select-none px-4 py-2',
                      active ? 'bg-surface-elevated text-text-primary' : 'text-text-primary'
                    )
                  }
                >
                  {({ selected }) => (
                    <div>
                      <div className={cn('text-sm font-medium', selected && 'text-brand-primary')}>
                        {result.displayName}
                      </div>
                      {result.context && (
                        <div className="text-xs text-text-secondary mt-0.5">
                          {result.context}
                        </div>
                      )}
                    </div>
                  )}
                </Combobox.Option>
              ))}
            </Combobox.Options>
          </Transition>
        </div>
      </Combobox>
    </div>
  );
}

