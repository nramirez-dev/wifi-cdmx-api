using WifiCdmx.Application.DTOs;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Application.Services;

/// <summary>
/// Business logic layer for WiFi point operations.
/// Maps domain entities to DTOs before returning to the API layer.
/// </summary>
public class WifiPointService(IWifiPointRepository repository) : IWifiPointService
{
    public async Task<PagedResultDto<WifiPointDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await repository.GetAllAsync(page, pageSize);
        return ToPagedResult(items, total, page, pageSize);
    }

    public async Task<WifiPointDto?> GetByIdAsync(Guid id)
    {
        var point = await repository.GetByIdAsync(id);
        return point is null ? null : ToDto(point);
    }

    public async Task<PagedResultDto<WifiPointDto>> GetByBoroughAsync(
        string borough, int page, int pageSize)
    {
        var (items, total) = await repository.GetByBoroughAsync(borough, page, pageSize);
        return ToPagedResult(items, total, page, pageSize);
    }

    public async Task<PagedResultDto<WifiPointDto>> GetNearbyAsync(
        double latitude, double longitude, int page, int pageSize)
    {
        var (items, total) = await repository.GetNearbyAsync(latitude, longitude, page, pageSize);
        return ToPagedResult(items, total, page, pageSize);
    }

    public async Task<StatsDto> GetStatsAsync()
    {
        var byBorough = await repository.GetStatsByBoroughAsync();
        var byProgram = await repository.GetStatsByProgramAsync();
        var totalPoints = await repository.GetAllAsync(1, int.MaxValue);

        return new StatsDto(
            TotalPoints: totalPoints.Total,
            TotalAccessPoints: byBorough.Values.Sum(),
            ByBorough: byBorough,
            ByProgram: byProgram
        );
    }

    public async Task<IEnumerable<HeatmapCellDto>> GetHeatmapAsync(double gridSize = 0.01) =>
        await repository.GetHeatmapAsync(gridSize);

    // --- Private helpers ---

    private static WifiPointDto ToDto(WifiPoint w) => new(
        w.Id, w.Name, w.Neighborhood, w.Borough,
        w.Latitude, w.Longitude, w.AccessPointCount, w.Program
    );

    private static PagedResultDto<WifiPointDto> ToPagedResult(
        IEnumerable<WifiPoint> items, int total, int page, int pageSize) =>
        new(items.Select(ToDto), total, page, pageSize);
}
