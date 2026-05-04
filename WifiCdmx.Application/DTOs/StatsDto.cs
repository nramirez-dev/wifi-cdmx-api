namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Aggregated statistics about WiFi points.
/// </summary>
public record StatsDto(
    int TotalPoints,
    int TotalAccessPoints,
    Dictionary<string, int> ByBorough,
    Dictionary<string, int> ByProgram
);
