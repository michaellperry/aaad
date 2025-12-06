using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping geocoding-related endpoints.
/// </summary>
public static class GeocodingEndpoints
{
    /// <summary>
    /// Maps geocoding endpoints for address search.
    /// All endpoints require authentication and are rate-limited.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapGeocodingEndpoints(this WebApplication app)
    {
        var geocoding = app.MapGroup("/api/geocoding")
            .RequireAuthorization();

        // GET /api/geocoding/search?query={query}
        geocoding.MapGet("/search", async (string query, IConfiguration configuration, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
        {
            var logger = loggerFactory.CreateLogger("GloboTicket.API.Endpoints.GeocodingEndpoints");
            
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest(new { error = "Query parameter is required" });
            }

            var accessToken = configuration["Mapbox:AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                logger.LogError("Mapbox access token is not configured");
                return Results.Problem("Geocoding service is not configured", statusCode: 500);
            }

            try
            {
                using var httpClient = new HttpClient();
                var encodedQuery = Uri.EscapeDataString(query);
                var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{encodedQuery}.json?access_token={accessToken}&limit=5&types=address,poi,place";

                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Mapbox API returned status {StatusCode} for query {Query}", response.StatusCode, query);
                    return Results.Problem("Failed to search addresses", statusCode: (int)response.StatusCode);
                }

                var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var mapboxResponse = JsonSerializer.Deserialize<MapboxGeocodingResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (mapboxResponse?.Features == null || mapboxResponse.Features.Count == 0)
                {
                    return Results.Ok(Array.Empty<GeocodeResult>());
                }

                var results = mapboxResponse.Features.Select(feature => new GeocodeResult
                {
                    Id = feature.Id,
                    DisplayName = feature.PlaceName ?? feature.Text ?? "Unknown location",
                    Address = feature.PlaceName ?? feature.Text ?? "",
                    Latitude = feature.Center?[1] ?? 0,
                    Longitude = feature.Center?[0] ?? 0,
                    Context = string.Join(", ", feature.Context?.Select(c => c.Text).Where(t => !string.IsNullOrEmpty(t)) ?? Array.Empty<string>())
                }).ToArray();

                return Results.Ok(results);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Network error while calling Mapbox API for query {Query}", query);
                return Results.Problem("Network error while searching addresses", statusCode: 503);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while processing geocoding request for query {Query}", query);
                return Results.Problem("An error occurred while searching addresses", statusCode: 500);
            }
        })
        .WithName("SearchAddresses");

        return app;
    }

    // Response models for Mapbox API
    private class MapboxGeocodingResponse
    {
        public List<MapboxFeature> Features { get; set; } = new();
    }

    private class MapboxFeature
    {
        public string Id { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? PlaceName { get; set; }
        public double[]? Center { get; set; }
        public List<MapboxContext>? Context { get; set; }
    }

    private class MapboxContext
    {
        public string? Text { get; set; }
    }

    // Response model for our API
    public class GeocodeResult
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Context { get; set; } = string.Empty;
    }
}

