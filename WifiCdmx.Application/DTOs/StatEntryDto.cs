namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Represents a single aggregation entry — a name and its count.
/// Used instead of Dictionary to ensure GraphQL compatibility.
/// </summary>
public record StatEntryDto(string Name, int Count);
