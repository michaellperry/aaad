import { useState, useEffect } from 'react';
import { Button } from '../atoms/Button';
import { Text } from '../atoms/Text';
import { Card } from '../molecules/Card';
import { createShow, getVenues, getNearbyShows } from '../../api/client';
import { constructDateTime, formatDate, formatTime } from '../../utils/format';
import type { CreateShowDto, Show, NearbyShow } from '../../types/show';
import type { Venue } from '../../types/venue';

interface ShowFormProps {
  actGuid: string;
  actName: string;
  onSuccess?: (show: Show) => void;
  onCancel?: () => void;
}

export function ShowForm({ actGuid, actName, onSuccess, onCancel }: ShowFormProps) {
  // Form state
  const [venueGuid, setVenueGuid] = useState('');
  const [ticketCount, setTicketCount] = useState('');
  const [startDate, setStartDate] = useState('');
  const [startTime, setStartTime] = useState('');
  
  // UI state
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [isLoading, setIsLoading] = useState(false);
  
  // Data state
  const [venues, setVenues] = useState<Venue[]>([]);
  const [isLoadingVenues, setIsLoadingVenues] = useState(true);
  const [nearbyShows, setNearbyShows] = useState<NearbyShow[]>([]);
  const [isLoadingNearbyShows, setIsLoadingNearbyShows] = useState(false);
  
  // Computed values
  const selectedVenue = venues.find(v => v.venueGuid === venueGuid);
  const venueCapacity = selectedVenue?.seatingCapacity || 0;
  
  // Fetch venues on mount
  useEffect(() => {
    const fetchVenues = async () => {
      try {
        const data = await getVenues();
        setVenues(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load venues');
      } finally {
        setIsLoadingVenues(false);
      }
    };

    fetchVenues();
  }, []);
  
  // Fetch nearby shows when venue, date, and time all change
  useEffect(() => {
    const fetchNearbyShows = async () => {
      // Only fetch if all required fields are present
      if (!venueGuid || !startDate || !startTime) {
        setNearbyShows([]);
        return;
      }

      setIsLoadingNearbyShows(true);
      try {
        // Construct ISO 8601 datetime with timezone offset
        const dateTime = constructDateTime(startDate, startTime);
        const data = await getNearbyShows(venueGuid, dateTime);
        setNearbyShows(data.shows);
      } catch (err) {
        // Don't show error for nearby shows - it's informational only
        console.error('Failed to load nearby shows:', err);
        setNearbyShows([]);
      } finally {
        setIsLoadingNearbyShows(false);
      }
    };

    fetchNearbyShows();
  }, [venueGuid, startDate, startTime]);
  
  // Validation
  const validateForm = (): Record<string, string> => {
    const errors: Record<string, string> = {};
    
    // Venue validation
    if (!venueGuid) {
      errors.venue = 'Please select a venue';
    }
    
    // Ticket count validation
    if (!ticketCount) {
      errors.ticketCount = 'Ticket count is required';
    } else {
      const count = parseInt(ticketCount);
      if (isNaN(count) || count <= 0) {
        errors.ticketCount = 'Ticket count must be a positive number';
      } else if (venueCapacity > 0 && count > venueCapacity) {
        errors.ticketCount = `Ticket count cannot exceed venue capacity of ${venueCapacity.toLocaleString()}`;
      }
    }
    
    // Start date validation
    if (!startDate) {
      errors.startDate = 'Start date is required';
    } else {
      const selectedDate = new Date(startDate);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (selectedDate <= today) {
        errors.startDate = 'Start date must be in the future';
      }
    }
    
    // Start time validation
    if (!startTime) {
      errors.startTime = 'Start time is required';
    } else if (!/^\d{2}:\d{2}$/.test(startTime)) {
      errors.startTime = 'Please enter a valid time';
    }
    
    return errors;
  };
  
  // Handle submit
  const handleSubmit = async () => {
    setError(null);
    setFieldErrors({});

    // Validate all fields
    const errors = validateForm();
    
    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors);
      return;
    }

    setIsLoading(true);

    try {
      // Construct ISO 8601 datetime with timezone offset
      const startTimeISO = constructDateTime(startDate, startTime);
      
      const dto: CreateShowDto = {
        showGuid: crypto.randomUUID(),
        venueGuid,
        ticketCount: parseInt(ticketCount),
        startTime: startTimeISO,
      };

      const newShow = await createShow(actGuid, dto);

      if (onSuccess) {
        onSuccess(newShow);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create show');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    }
  };

  // Calculate tomorrow's date for min attribute
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  const minDate = tomorrow.toISOString().split('T')[0];

  return (
    <form className="space-y-6" onSubmit={(e) => e.preventDefault()}>
      {error && (
        <div className="p-4 rounded-lg bg-error/10 border border-error/20" role="alert" aria-live="polite">
          <Text size="sm" className="text-error">
            {error}
          </Text>
        </div>
      )}

      {/* Venue Selection */}
      <div>
        <label htmlFor="venue" className="block text-sm font-medium text-text-primary mb-2">
          Venue *
        </label>
        {isLoadingVenues ? (
          <div className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-muted">
            Loading venues...
          </div>
        ) : venues.length === 0 ? (
          <div className="p-4 rounded-lg bg-surface-elevated border border-border-default">
            <Text size="sm" className="text-text-muted">
              No venues available. Please create a venue first.
            </Text>
          </div>
        ) : (
          <>
            <select
              id="venue"
              value={venueGuid}
              onChange={(e) => setVenueGuid(e.target.value)}
              disabled={isLoading}
              className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
              required
            >
              <option value="">Select venue...</option>
              {venues.map((venue) => (
                <option key={venue.venueGuid} value={venue.venueGuid}>
                  {venue.name}
                </option>
              ))}
            </select>
            {fieldErrors.venue && (
              <Text size="sm" className="text-error mt-1">
                {fieldErrors.venue}
              </Text>
            )}
          </>
        )}
      </div>

      {/* Venue Capacity Display */}
      {selectedVenue && (
        <div className="p-4 rounded-lg bg-primary-base/10 border border-primary-base/20" aria-live="polite">
          <Text size="sm" className="font-medium text-primary-base">
            Venue Capacity: {venueCapacity.toLocaleString()} seats
          </Text>
        </div>
      )}

      {/* Ticket Count */}
      <div>
        <label htmlFor="ticketCount" className="block text-sm font-medium text-text-primary mb-2">
          Number of Tickets *
        </label>
        <input
          type="number"
          id="ticketCount"
          value={ticketCount}
          onChange={(e) => setTicketCount(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="Enter ticket count"
          min="1"
          max={venueCapacity || undefined}
          required
        />
        {fieldErrors.ticketCount && (
          <Text size="sm" className="text-error mt-1">
            {fieldErrors.ticketCount}
          </Text>
        )}
      </div>

      {/* Date and Time Fields */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label htmlFor="startDate" className="block text-sm font-medium text-text-primary mb-2">
            Start Date *
          </label>
          <input
            type="date"
            id="startDate"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
            min={minDate}
            required
          />
          {fieldErrors.startDate && (
            <Text size="sm" className="text-error mt-1">
              {fieldErrors.startDate}
            </Text>
          )}
        </div>

        <div>
          <label htmlFor="startTime" className="block text-sm font-medium text-text-primary mb-2">
            Start Time *
          </label>
          <input
            type="time"
            id="startTime"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
            required
          />
          {fieldErrors.startTime && (
            <Text size="sm" className="text-error mt-1">
              {fieldErrors.startTime}
            </Text>
          )}
        </div>
      </div>

      {/* Nearby Shows Display */}
      {venueGuid && startDate && startTime && (
        <Card>
          <div className="space-y-3">
            <Text size="sm" className="font-medium text-text-primary">
              Other Shows at This Venue
            </Text>
            {isLoadingNearbyShows ? (
              <Text size="sm" className="text-text-muted">
                Loading nearby shows...
              </Text>
            ) : nearbyShows.length === 0 ? (
              <Text size="sm" className="text-text-muted">
                No other shows scheduled at this venue within 48 hours
              </Text>
            ) : (
              <div className="space-y-2" aria-live="polite">
                {nearbyShows.map((show) => (
                  <div
                    key={show.showGuid}
                    className="p-3 rounded-lg bg-surface-elevated border border-border-default"
                  >
                    <Text size="sm" className="font-medium text-text-primary">
                      {show.actName}
                    </Text>
                    <Text size="sm" className="text-text-muted">
                      {formatDate(show.startTime)} at {formatTime(show.startTime)}
                    </Text>
                  </div>
                ))}
              </div>
            )}
          </div>
        </Card>
      )}

      {/* Buttons */}
      <div className="flex gap-4 pt-4">
        <Button
          type="button"
          variant="primary"
          size="lg"
          isLoading={isLoading}
          disabled={isLoading || venues.length === 0}
          className="flex-1"
          onClick={handleSubmit}
        >
          Create Show
        </Button>
        <Button
          type="button"
          variant="secondary"
          size="lg"
          onClick={handleCancel}
          disabled={isLoading}
        >
          Cancel
        </Button>
      </div>
    </form>
  );
}
