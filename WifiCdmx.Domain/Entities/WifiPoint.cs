namespace WifiCdmx.Domain.Entities;

public class WifiPoint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Borough { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int AccessPointCount { get; set; }
    public string Program { get; set; } = string.Empty;
}
