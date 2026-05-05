namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Aggregated statistics about WiFi points.
/// </summary>
public record StatsDto(
    int TotalPoints,
    int TotalAccessPoints,
    IEnumerable<StatEntryDto> ByBorough,
    IEnumerable<StatEntryDto> ByProgram
);
