using System.Collections.Concurrent;

namespace GloboTicket.API.Services;

/// <summary>
/// In-memory implementation of rate limiting service using sliding window algorithm.
/// Thread-safe implementation using ConcurrentDictionary.
/// </summary>
public class RateLimitService : IRateLimitService
{
    private readonly ConcurrentDictionary<string, RateLimitEntry> _cache = new();
    private readonly ILogger<RateLimitService> _logger;
    private readonly Timer _cleanupTimer;

    public RateLimitService(ILogger<RateLimitService> logger)
    {
        _logger = logger;
        // Clean up expired entries every 5 minutes
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public bool IsAllowed(string key, int requestsPerWindow, int windowSeconds)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-windowSeconds);

        var entry = _cache.AddOrUpdate(
            key,
            _ => new RateLimitEntry { RequestTimestamps = new List<DateTime> { now } },
            (_, existing) =>
            {
                // Remove timestamps outside the current window
                existing.RequestTimestamps.RemoveAll(t => t < windowStart);
                
                // Add current request timestamp
                existing.RequestTimestamps.Add(now);
                
                return existing;
            });

        var requestCount = entry.RequestTimestamps.Count;
        var isAllowed = requestCount <= requestsPerWindow;

        if (!isAllowed)
        {
            _logger.LogWarning(
                "Rate limit exceeded for key {Key}: {Count}/{Limit} requests in {WindowSeconds}s window",
                key, requestCount, requestsPerWindow, windowSeconds);
        }

        return isAllowed;
    }

    public RateLimitStatus GetStatus(string key, int requestsPerWindow, int windowSeconds)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-windowSeconds);
        var resetTime = now.AddSeconds(windowSeconds);

        if (!_cache.TryGetValue(key, out var entry))
        {
            return new RateLimitStatus
            {
                Limit = requestsPerWindow,
                Remaining = requestsPerWindow,
                Reset = ((DateTimeOffset)resetTime).ToUnixTimeSeconds()
            };
        }

        // Remove expired timestamps
        entry.RequestTimestamps.RemoveAll(t => t < windowStart);
        var requestCount = entry.RequestTimestamps.Count;
        var remaining = Math.Max(0, requestsPerWindow - requestCount);

        // Calculate reset time based on oldest request in window
        if (entry.RequestTimestamps.Any())
        {
            var oldestRequest = entry.RequestTimestamps.Min();
            resetTime = oldestRequest.AddSeconds(windowSeconds);
        }

        return new RateLimitStatus
        {
            Limit = requestsPerWindow,
            Remaining = remaining,
            Reset = ((DateTimeOffset)resetTime).ToUnixTimeSeconds()
        };
    }

    private void CleanupExpiredEntries(object? state)
    {
        var now = DateTime.UtcNow;
        var keysToRemove = new List<string>();

        foreach (var kvp in _cache)
        {
            // Remove entries that haven't been accessed in the last hour
            if (kvp.Value.RequestTimestamps.All(t => t < now.AddHours(-1)))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }

        if (keysToRemove.Count > 0)
        {
            _logger.LogDebug("Cleaned up {Count} expired rate limit entries", keysToRemove.Count);
        }
    }

    private class RateLimitEntry
    {
        public List<DateTime> RequestTimestamps { get; set; } = new();
    }
}

