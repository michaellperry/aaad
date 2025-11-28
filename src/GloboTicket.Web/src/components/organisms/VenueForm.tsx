import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../atoms/Button';
import { Text } from '../atoms/Text';
import { createVenue, updateVenue } from '../../api/client';
import type { CreateVenueDto, UpdateVenueDto, Venue } from '../../types/venue';

interface VenueFormProps {
  venue?: Venue; // Optional venue for edit mode
  onSuccess?: (venue: Venue) => void;
  onCancel?: () => void;
}

export function VenueForm({ venue, onSuccess, onCancel }: VenueFormProps) {
  const navigate = useNavigate();
  const isEditMode = !!venue;
  
  // Initialize state with venue data if in edit mode
  const [name, setName] = useState(venue?.name || '');
  const [address, setAddress] = useState(venue?.address || '');
  const [seatingCapacity, setSeatingCapacity] = useState(venue?.seatingCapacity?.toString() || '');
  const [description, setDescription] = useState(venue?.description || '');
  const [latitude, setLatitude] = useState(venue?.latitude?.toString() || '');
  const [longitude, setLongitude] = useState(venue?.longitude?.toString() || '');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async () => {
    setError(null);

    // Validation
    if (!name.trim()) {
      setError('Venue name is required');
      return;
    }
    if (name.length > 100) {
      setError('Venue name must be 100 characters or less');
      return;
    }
    if (!description.trim()) {
      setError('Description is required');
      return;
    }
    if (description.length > 2000) {
      setError('Description must be 2000 characters or less');
      return;
    }
    if (!seatingCapacity || parseInt(seatingCapacity) < 0) {
      setError('Seating capacity must be a positive number');
      return;
    }
    if (address && address.length > 300) {
      setError('Address must be 300 characters or less');
      return;
    }
    if (latitude && (parseFloat(latitude) < -90 || parseFloat(latitude) > 90)) {
      setError('Latitude must be between -90 and 90');
      return;
    }
    if (longitude && (parseFloat(longitude) < -180 || parseFloat(longitude) > 180)) {
      setError('Longitude must be between -180 and 180');
      return;
    }

    setIsLoading(true);

    try {
      if (isEditMode && venue) {
        // Update existing venue
        const dto: UpdateVenueDto = {
          name: name.trim(),
          address: address.trim() || undefined,
          seatingCapacity: parseInt(seatingCapacity),
          description: description.trim(),
          latitude: latitude ? parseFloat(latitude) : undefined,
          longitude: longitude ? parseFloat(longitude) : undefined,
        };

        const updatedVenue = await updateVenue(venue.venueGuid, dto);

        if (onSuccess) {
          onSuccess(updatedVenue);
        } else {
          navigate('/venues');
        }
      } else {
        // Create new venue
        const dto: CreateVenueDto = {
          venueGuid: crypto.randomUUID(),
          name: name.trim(),
          address: address.trim() || undefined,
          seatingCapacity: parseInt(seatingCapacity),
          description: description.trim(),
          latitude: latitude ? parseFloat(latitude) : undefined,
          longitude: longitude ? parseFloat(longitude) : undefined,
        };

        const newVenue = await createVenue(dto);

        if (onSuccess) {
          onSuccess(newVenue);
        } else {
          navigate('/venues');
        }
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to ${isEditMode ? 'update' : 'create'} venue`);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    } else {
      navigate('/venues');
    }
  };

  return (
    <form className="space-y-6" onSubmit={(e) => e.preventDefault()}>
      {error && (
        <div className="p-4 rounded-lg bg-error/10 border border-error/20">
          <Text size="sm" className="text-error">
            {error}
          </Text>
        </div>
      )}

      <div>
        <label htmlFor="name" className="block text-sm font-medium text-text-primary mb-2">
          Venue Name *
        </label>
        <input
          type="text"
          id="name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="Enter venue name"
          maxLength={100}
          required
        />
      </div>

      <div>
        <label htmlFor="address" className="block text-sm font-medium text-text-primary mb-2">
          Address
        </label>
        <input
          type="text"
          id="address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="Enter venue address"
          maxLength={300}
        />
      </div>

      <div>
        <label htmlFor="seatingCapacity" className="block text-sm font-medium text-text-primary mb-2">
          Seating Capacity *
        </label>
        <input
          type="number"
          id="seatingCapacity"
          value={seatingCapacity}
          onChange={(e) => setSeatingCapacity(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="Enter seating capacity"
          min="0"
          required
        />
      </div>

      <div>
        <label htmlFor="description" className="block text-sm font-medium text-text-primary mb-2">
          Description *
        </label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed resize-vertical"
          placeholder="Enter venue description"
          rows={4}
          maxLength={2000}
          required
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label htmlFor="latitude" className="block text-sm font-medium text-text-primary mb-2">
            Latitude
          </label>
          <input
            type="number"
            id="latitude"
            value={latitude}
            onChange={(e) => setLatitude(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
            placeholder="e.g., 40.7128"
            step="any"
            min="-90"
            max="90"
          />
        </div>

        <div>
          <label htmlFor="longitude" className="block text-sm font-medium text-text-primary mb-2">
            Longitude
          </label>
          <input
            type="number"
            id="longitude"
            value={longitude}
            onChange={(e) => setLongitude(e.target.value)}
            disabled={isLoading}
            className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
            placeholder="e.g., -74.0060"
            step="any"
            min="-180"
            max="180"
          />
        </div>
      </div>

      <div className="flex gap-4 pt-4">
        <Button
          type="button"
          variant="primary"
          size="lg"
          isLoading={isLoading}
          disabled={isLoading}
          className="flex-1"
          onClick={handleSubmit}
        >
          {isEditMode ? 'Update Venue' : 'Create Venue'}
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