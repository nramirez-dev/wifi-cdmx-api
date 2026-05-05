namespace WifiCdmx.Domain.Entities;

/// <summary>
/// Represents a WiFi access point in Mexico City.
/// Based on the official CDMX open data dataset.
/// </summary>
public class WifiPoint
{
    public Guid Id { get; set; }
    public string OriginalId { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Borough { get; set; } = string.Empty;
}
