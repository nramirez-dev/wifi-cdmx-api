namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Represents a geographic grid cell with aggregated WiFi point data.
/// Ready to be consumed by mapping libraries like Leaflet, Google Maps or Mapbox.
/// </summary>
public record HeatmapCellDto(
    double Latitude,
    double Longitude,
    int PointCount,
    int TotalAccessPoints
);
