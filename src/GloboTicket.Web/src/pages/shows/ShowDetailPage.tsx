import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Calendar } from 'lucide-react';
import { Heading, Text, Button, Spinner } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { useShow } from '../../features/shows/hooks';

/**
 * Format date in long format (e.g., "March 15, 2026")
 */
function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

/**
 * Format time in 12-hour format (e.g., "7:30 PM")
 */
function formatTime(dateString: string): string {
  return new Date(dateString).toLocaleTimeString('en-US', {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  });
}

/**
 * Format date and time together (e.g., "March 15, 2026 at 7:30 PM")
 */
function formatDateTime(dateString: string): string {
  return `${formatDate(dateString)} at ${formatTime(dateString)}`;
}

/**
 * Format timestamp with date and time (e.g., "1/2/2026, 10:30:00 AM")
 */
function formatTimestamp(dateString: string): string {
  return new Date(dateString).toLocaleString();
}

/**
 * Show detail page - displays show information
 */
export const ShowDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: show, isLoading, error } = useShow(id);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <Spinner size="lg" />
      </div>
    );
  }

  if (error || !show) {
    return (
      <Stack gap="xl">
        <Card>
          <div className="p-8 text-center">
            <Text className="text-error">
              {error ? error.message : 'Show not found'}
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
      <div className="flex items-start gap-4">
        <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
          <Calendar className="w-8 h-8 text-brand-primary" />
        </div>
        <div>
          <Heading level="h1" variant="default" className="mb-2">
            {show.actName} at {show.venueName}
          </Heading>
          <Text variant="muted">
            {formatDateTime(show.startTime)}
          </Text>
        </div>
      </div>

      {/* Show Information */}
      <Card header={<Heading level="h2">Show Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Act Name
            </Text>
            <Text>{show.actName}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Venue Name
            </Text>
            <Text>{show.venueName}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Start Date
            </Text>
            <time dateTime={show.startTime}>
              <Text>{formatDate(show.startTime)}</Text>
            </time>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Start Time
            </Text>
            <time dateTime={show.startTime}>
              <Text>{formatTime(show.startTime)}</Text>
            </time>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Tickets Available
            </Text>
            <Text>{show.ticketCount.toLocaleString()}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Created
            </Text>
            <Text>{formatTimestamp(show.createdAt)}</Text>
          </div>
          {show.updatedAt && (
            <div>
              <Text variant="muted" size="sm" className="mb-1">
                Last Updated
              </Text>
              <Text>{formatTimestamp(show.updatedAt)}</Text>
            </div>
          )}
        </Stack>
      </Card>
    </Stack>
  );
};
