using WifiCdmx.Application.DTOs;

namespace WifiCdmx.Application.Interfaces;

/// <summary>
/// Service contract for WiFi point business logic.
/// </summary>
public interface IWifiPointService
{
    Task<PagedResultDto<WifiPointDto>> GetAllAsync(int page, int pageSize);
    Task<WifiPointDto?> GetByIdAsync(string id);
    Task<PagedResultDto<WifiPointDto>> GetByBoroughAsync(string borough, int page, int pageSize);
    Task<PagedResultDto<WifiPointDto>> GetNearbyAsync(double latitude, double longitude, int page, int pageSize);
    Task<StatsDto> GetStatsAsync();
}
