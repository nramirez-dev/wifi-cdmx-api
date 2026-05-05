namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Data transfer object for a WiFi access point.
/// </summary>
public record WifiPointDto(
    Guid Id,
    string OriginalId,
    string Program,
    double Latitude,
    double Longitude,
    string Borough
);
