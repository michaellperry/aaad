namespace GloboTicket.API.Services;

/// <summary>
/// Service for managing rate limiting of API requests.
/// Tracks request counts per user/endpoint using sliding window algorithm.
/// </summary>
public interface IRateLimitService
{
    /// <summary>
    /// Checks if a request is allowed based on rate limit configuration.
    /// </summary>
    /// <param name="key">Unique key identifying the rate limit bucket (e.g., "geocoding:username")</param>
    /// <param name="requestsPerWindow">Maximum number of requests allowed in the time window</param>
    /// <param name="windowSeconds">Time window in seconds</param>
    /// <returns>True if request is allowed, false if rate limit exceeded</returns>
    bool IsAllowed(string key, int requestsPerWindow, int windowSeconds);

    /// <summary>
    /// Gets the current rate limit status for a key.
    /// </summary>
    /// <param name="key">Unique key identifying the rate limit bucket</param>
    /// <param name="requestsPerWindow">Maximum number of requests allowed in the time window</param>
    /// <param name="windowSeconds">Time window in seconds</param>
    /// <returns>Rate limit status with remaining requests and reset time</returns>
    RateLimitStatus GetStatus(string key, int requestsPerWindow, int windowSeconds);
}

/// <summary>
/// Represents the current rate limit status for a key.
/// </summary>
public class RateLimitStatus
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed in the window.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the number of requests remaining in the current window.
    /// </summary>
    public int Remaining { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp when the rate limit window resets.
    /// </summary>
    public long Reset { get; set; }
}

