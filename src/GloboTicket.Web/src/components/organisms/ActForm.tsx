import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../atoms/Button';
import { Text } from '../atoms/Text';
import { createAct, updateAct } from '../../api/client';
import type { CreateActDto, UpdateActDto, Act } from '../../types/act';

interface ActFormProps {
  act?: Act; // Optional act for edit mode
  onSuccess?: (act: Act) => void;
  onCancel?: () => void;
}

export function ActForm({ act, onSuccess, onCancel }: ActFormProps) {
  const navigate = useNavigate();
  const isEditMode = !!act;
  
  // Initialize state with act data if in edit mode
  const [name, setName] = useState(act?.name || '');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async () => {
    setError(null);

    // Validation
    if (!name.trim()) {
      setError('Act name is required');
      return;
    }
    if (name.length > 100) {
      setError('Act name must be 100 characters or less');
      return;
    }

    setIsLoading(true);

    try {
      if (isEditMode && act) {
        // Update existing act
        const dto: UpdateActDto = {
          name: name.trim(),
        };

        const updatedAct = await updateAct(act.actGuid, dto);

        if (onSuccess) {
          onSuccess(updatedAct);
        } else {
          navigate('/acts');
        }
      } else {
        // Create new act
        const dto: CreateActDto = {
          actGuid: crypto.randomUUID(),
          name: name.trim(),
        };

        const newAct = await createAct(dto);

        if (onSuccess) {
          onSuccess(newAct);
        } else {
          navigate('/acts');
        }
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : `Failed to ${isEditMode ? 'update' : 'create'} act`);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    if (onCancel) {
      onCancel();
    } else {
      navigate('/acts');
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
          Act Name *
        </label>
        <input
          type="text"
          id="name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={isLoading}
          className="w-full px-4 py-2 rounded-lg border border-border-default bg-surface-base text-text-primary placeholder-text-muted focus:outline-none focus:ring-2 focus:ring-primary-base focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder="Enter act name"
          maxLength={100}
          required
        />
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
          {isEditMode ? 'Update Act' : 'Create Act'}
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