import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Edit, Music, Trash2, Plus } from 'lucide-react';
import { Heading, Text, Button, Spinner } from '../../components/atoms';
import { Card, ShowCard } from '../../components/molecules';
import { Stack, Row } from '../../components/layout';
import { useAct, useDeleteAct } from '../../features/acts/hooks';
import { useShowsByAct } from '../../features/shows/hooks';

/**
 * Act detail page - displays act information and management options
 */
export const ActDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  // Fetch act details
  const { data: act, isLoading, error } = useAct(id);
  
  // Fetch shows for this act (dependent query - only runs when act.guid is available)
  const {
    data: shows = [],
    isLoading: isLoadingShows,
    error: showsError
  } = useShowsByAct(act?.actGuid);
  
  // Delete mutation
  const deleteActMutation = useDeleteAct();

  const handleDelete = () => {
    if (!act) return;
    
    if (window.confirm(`Are you sure you want to delete "${act.name}"? This action cannot be undone.`)) {
      deleteActMutation.mutate(act.actGuid, {
        onSuccess: () => {
          navigate('/acts');
        },
        onError: (err) => {
          alert(err instanceof Error ? err.message : 'Failed to delete act');
        },
      });
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <Spinner size="lg" />
      </div>
    );
  }

  if (error || !act) {
    return (
      <Stack gap="xl">
        <Card>
          <div className="p-8 text-center">
            <Text className="text-error">
              {error ? (error instanceof Error ? error.message : String(error)) : 'Act not found'}
            </Text>
          </div>
        </Card>
      </Stack>
    );
  }

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => navigate('/acts')}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Acts
      </Button>

      {/* Page Header */}
      <div className="flex items-start justify-between">
        <div className="flex items-start gap-4">
          <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
            <Music className="w-8 h-8 text-brand-primary" />
          </div>
          <div>
            <Heading level="h1" variant="default" className="mb-2">
              {act.name}
            </Heading>
            <Text variant="muted">
              Created {new Date(act.createdAt).toLocaleDateString()}
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => navigate(`/acts/${id}/edit`)}
          >
            <Edit className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button
            variant="danger"
            onClick={handleDelete}
          >
            <Trash2 className="w-4 h-4 mr-2" />
            Delete
          </Button>
        </Row>
      </div>

      {/* Act Information */}
      <Card header={<Heading level="h2">Act Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Name
            </Text>
            <Text>{act.name}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Created
            </Text>
            <Text>{new Date(act.createdAt).toLocaleString()}</Text>
          </div>
          {act.updatedAt && (
            <div>
              <Text variant="muted" size="sm" className="mb-1">
                Last Updated
              </Text>
              <Text>{new Date(act.updatedAt).toLocaleString()}</Text>
            </div>
          )}
        </Stack>
      </Card>

      {/* Upcoming Shows */}
      <Card
        header={
          <div className="flex justify-between items-center">
            <Heading level="h2">Upcoming Shows</Heading>
            <Button
              variant="primary"
              size="sm"
              onClick={() => navigate(`/acts/${act.actGuid}/shows/new`)}
            >
              <Plus className="w-4 h-4 mr-2" />
              Add Show
            </Button>
          </div>
        }
      >
        {isLoadingShows ? (
          <div className="flex justify-center p-8">
            <Spinner size="md" />
          </div>
        ) : showsError ? (
          <Text className="text-error">
            {showsError instanceof Error ? showsError.message : String(showsError)}
          </Text>
        ) : shows.length === 0 ? (
          <Text variant="muted">
            No upcoming shows scheduled for this act.
          </Text>
        ) : (
          <Stack gap="md">
            {shows.map((show) => (
              <ShowCard
                key={show.showGuid}
                show={show}
                onClick={(showGuid) => navigate(`/shows/${showGuid}`)}
              />
            ))}
          </Stack>
        )}
      </Card>

    </Stack>
  );
};