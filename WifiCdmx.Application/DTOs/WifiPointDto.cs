namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Data transfer object for a WiFi access point.
/// </summary>
public record WifiPointDto(
    string Id,
    string Neighborhood,
    string Borough,
    double Latitude,
    double Longitude,
    int AccessPointCount,
    string Program
);
