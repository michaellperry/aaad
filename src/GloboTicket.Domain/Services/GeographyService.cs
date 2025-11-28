using NetTopologySuite.Geometries;

namespace GloboTicket.Domain.Services;

/// <summary>
/// Provides helper methods for working with geographic data.
/// </summary>
public static class GeographyService
{
    /// <summary>
    /// The SRID (Spatial Reference System Identifier) for WGS84 coordinate system.
    /// This is the standard coordinate system used for GPS and geographic data.
    /// </summary>
    private const int WGS84_SRID = 4326;

    /// <summary>
    /// Creates a Point geometry from latitude and longitude coordinates.
    /// </summary>
    /// <param name="latitude">The latitude coordinate (Y-axis, -90 to 90).</param>
    /// <param name="longitude">The longitude coordinate (X-axis, -180 to 180).</param>
    /// <returns>A Point with SRID 4326 (WGS84), or null if either coordinate is null.</returns>
    public static Point? CreatePoint(double? latitude, double? longitude)
    {
        if (latitude == null || longitude == null)
        {
            return null;
        }

        // Note: NetTopologySuite Point constructor takes (X, Y) which is (longitude, latitude)
        var point = new Point(longitude.Value, latitude.Value)
        {
            SRID = WGS84_SRID
        };

        return point;
    }
}