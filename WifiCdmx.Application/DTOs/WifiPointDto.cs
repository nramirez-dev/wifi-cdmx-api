namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Data transfer object for a WiFi access point.
/// </summary>
public record WifiPointDto(
    Guid Id,
    string Name,
    string Neighborhood,
    string Borough,
    double Latitude,
    double Longitude,
    int AccessPointCount,
    string Program
);
