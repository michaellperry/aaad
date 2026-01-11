import { useState, useEffect } from 'react';
import { Button, Text } from '../atoms';
import { useCreateTicketOffer, useUpdateTicketOffer } from '../../features/ticketOffers/hooks';
import type { CreateTicketOfferDto, TicketOffer } from '../../types/ticketOffer';

export interface TicketOfferFormProps {
  showGuid: string;
  availableCapacity: number;
  ticketOffer?: TicketOffer; // Optional: if provided, form is in edit mode
  onSuccess?: () => void; // Optional: callback on successful submit
  onCancel?: () => void; // Optional: callback on cancel
}

/**
 * TicketOfferForm organism - form for creating or editing ticket offers
 * 
 * Includes validation for name, price, and ticket count.
 * Validates ticket count against available capacity.
 * Resets form after successful creation.
 * Pre-populates form when editing an existing offer.
 * 
 * @param showGuid - GUID of the show to create offer for
 * @param availableCapacity - Remaining capacity for validation
 * @param ticketOffer - Optional: existing ticket offer for edit mode
 * @param onSuccess - Optional: callback on successful submit
 * @param onCancel - Optional: callback on cancel
 */
export const TicketOfferForm = ({ 
  showGuid, 
  availableCapacity, 
  ticketOffer,
  onSuccess,
  onCancel
}: TicketOfferFormProps) => {
  const isEditMode = !!ticketOffer;

  // Form state
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [ticketCount, setTicketCount] = useState('');

  // Initialize form with ticket offer data in edit mode
  useEffect(() => {
    if (ticketOffer) {
      setName(ticketOffer.name);
      setPrice(ticketOffer.price.toString());
      setTicketCount(ticketOffer.ticketCount.toString());
    }
  }, [ticketOffer]);

  // UI state
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  // Mutations
  const createMutation = useCreateTicketOffer();
  const updateMutation = useUpdateTicketOffer();
  
  const isLoading = createMutation.isPending || updateMutation.isPending;

  // Validation
  const validateForm = (): Record<string, string> => {
    const errors: Record<string, string> = {};

    // Name validation
    if (!name.trim()) {
      errors.name = 'Offer name is required';
    } else if (name.length > 100) {
      errors.name = 'Offer name cannot exceed 100 characters';
    }

    // Price validation
    if (!price) {
      errors.price = 'Price is required';
    } else {
      const priceNum = parseFloat(price);
      if (isNaN(priceNum) || priceNum <= 0) {
        errors.price = 'Price must be greater than zero';
      }
    }

    // Ticket count validation
    if (!ticketCount) {
      errors.ticketCount = 'Ticket count is required';
    } else {
      const count = parseInt(ticketCount);
      if (isNaN(count) || count <= 0) {
        errors.ticketCount = 'Ticket count must be a positive number';
      } else if (count > availableCapacity) {
        errors.ticketCount = `Ticket count exceeds available capacity. Only ${availableCapacity.toLocaleString()} tickets remain available.`;
      }
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

    try {
      if (isEditMode && ticketOffer) {
        // Update existing ticket offer
        await updateMutation.mutateAsync({
          ticketOfferGuid: ticketOffer.ticketOfferGuid,
          dto: {
            name: name.trim(),
            price: parseFloat(price),
            ticketCount: parseInt(ticketCount),
          }
        });
      } else {
        // Create new ticket offer
        const dto: CreateTicketOfferDto = {
          ticketOfferGuid: crypto.randomUUID(),
          name: name.trim(),
          price: parseFloat(price),
          ticketCount: parseInt(ticketCount),
        };

        await createMutation.mutateAsync({ showGuid, dto });

        // Clear form on success (only for create mode)
        setName('');
        setPrice('');
        setTicketCount('');
      }

      setError(null);
      setFieldErrors({});
      
      // Call success callback if provided
      if (onSuccess) {
        onSuccess();
      }
    } catch (err) {
      if (err instanceof Error) {
        // Parse error message for specific field errors
        if (err.message.toLowerCase().includes('capacity')) {
          setFieldErrors({ ticketCount: err.message });
        } else {
          setError(err.message);
        }
      }
    }
  };

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    } else {
      // Default cancel behavior: clear form
      setName('');
      setPrice('');
      setTicketCount('');
      setError(null);
      setFieldErrors({});
    }
  };

  return (
    <form className="space-y-6" onSubmit={(e) => e.preventDefault()}>
      {error && (
        <div
          className="p-4 rounded-lg bg-error/10 border border-error/20"
          role="alert"
          aria-live="polite"
        >
          <Text size="sm" className="text-error">
            {error}
          </Text>
        </div>
      )}

      {/* Offer Name */}
      <div>
        <label htmlFor="offerName" className="block text-sm font-medium text-text-primary mb-2">
          Offer Name *
        </label>
        <input
          type="text"
          id="offerName"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="e.g., General Admission, VIP, Early Bird"
          maxLength={100}
          required
          aria-describedby={fieldErrors.name ? 'offerName-error' : undefined}
        />
        {fieldErrors.name && (
          <Text id="offerName-error" size="sm" className="text-error mt-1" role="alert">
            {fieldErrors.name}
          </Text>
        )}
      </div>

      {/* Price */}
      <div>
        <label htmlFor="price" className="block text-sm font-medium text-text-primary mb-2">
          Price per Ticket *
        </label>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-text-muted">$</span>
          <input
            type="number"
            id="price"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            disabled={isLoading}
            className="w-full pl-8 pr-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
            placeholder="0.00"
            step="0.01"
            min="0.01"
            required
            aria-describedby={fieldErrors.price ? 'price-error' : undefined}
          />
        </div>
        {fieldErrors.price && (
          <Text id="price-error" size="sm" className="text-error mt-1" role="alert">
            {fieldErrors.price}
          </Text>
        )}
      </div>

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
          max={availableCapacity}
          required
          aria-describedby={fieldErrors.ticketCount ? 'ticketCount-error' : 'ticketCount-helper'}
        />
        {!fieldErrors.ticketCount && (
          <Text id="ticketCount-helper" size="sm" className="text-text-muted mt-1">
            Available capacity: {availableCapacity.toLocaleString()} tickets
          </Text>
        )}
        {fieldErrors.ticketCount && (
          <Text id="ticketCount-error" size="sm" className="text-error mt-1" role="alert">
            {fieldErrors.ticketCount}
          </Text>
        )}
      </div>

      {/* Buttons */}
      <div className="flex gap-4 pt-4">
        <Button
          type="button"
          variant="primary"
          size="lg"
          isLoading={isLoading}
          disabled={isLoading || availableCapacity === 0}
          className="flex-1"
          onClick={handleSubmit}
        >
          {isEditMode ? 'Update Offer' : 'Create Offer'}
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
};
