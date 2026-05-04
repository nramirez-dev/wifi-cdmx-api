using WifiCdmx.Application.DTOs;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Application.Interfaces;

/// <summary>
/// Repository contract for WiF
/// i point data access.
/// </summary>
public interface IWifiPointRepository
{
    Task<(IEnumerable<WifiPoint> Items, int Total)> GetAllAsync(int page, int pageSize);
    Task<WifiPoint?> GetByIdAsync(Guid id);
    Task<(IEnumerable<WifiPoint> Items, int Total)> GetByBoroughAsync(string borough, int page, int pageSize);
    Task<(IEnumerable<WifiPoint> Items, int Total)> GetNearbyAsync(double latitude, double longitude, int page, int pageSize);
    Task<Dictionary<string, int>> GetStatsByBoroughAsync();
    Task<Dictionary<string, int>> GetStatsByProgramAsync();
    Task BulkInsertAsync(IEnumerable<WifiPoint> points);
    Task<bool> AnyAsync();
    Task<IEnumerable<HeatmapCellDto>> GetHeatmapAsync(double gridSize);
}
