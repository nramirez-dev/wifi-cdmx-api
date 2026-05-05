using WifiCdmx.Application.DTOs;
using WifiCdmx.Application.Interfaces;

namespace WifiCdmx.API.GraphQL.Queries;

/// <summary>
/// GraphQL query root — exposes the same operations as the REST API
/// but through a single flexible endpoint at /graphql.
/// </summary>
public class WifiPointQuery
{
    /// <summary>
    /// Returns a paginated list of all WiFi access points.
    /// </summary>
    public async Task<PagedResultDto<WifiPointDto>> GetWifiPoints(
        [Service] IWifiPointService service,
        int page = 1,
        int pageSize = 20) =>
        await service.GetAllAsync(page, pageSize);

    /// <summary>
    /// Returns a single WiFi point by its Guid ID.
    /// </summary>
    public async Task<WifiPointDto?> GetWifiPointById(
        [Service] IWifiPointService service,
        Guid id) =>
        await service.GetByIdAsync(id);

    /// <summary>
    /// Returns a paginated list of WiFi points filtered by borough.
    /// </summary>
    public async Task<PagedResultDto<WifiPointDto>> GetWifiPointsByBorough(
        [Service] IWifiPointService service,
        string borough,
        int page = 1,
        int pageSize = 20) =>
        await service.GetByBoroughAsync(borough, page, pageSize);

    /// <summary>
    /// Returns WiFi points ordered by proximity to the given coordinates.
    /// Uses the Haversine formula for accurate geographic distance calculation.
    /// </summary>
    public async Task<PagedResultDto<WifiPointDto>> GetNearbyWifiPoints(
        [Service] IWifiPointService service,
        double latitude,
        double longitude,
        int page = 1,
        int pageSize = 20) =>
        await service.GetNearbyAsync(latitude, longitude, page, pageSize);

    /// <summary>
    /// Returns aggregated statistics about WiFi points by borough and program.
    /// </summary>
    public async Task<StatsDto> GetStats(
        [Service] IWifiPointService service) =>
        await service.GetStatsAsync();

    /// <summary>
    /// Returns WiFi points grouped into geographic grid cells for heatmap visualization.
    /// </summary>
    public async Task<IEnumerable<HeatmapCellDto>> GetHeatmap(
        [Service] IWifiPointService service,
        double gridSize = 0.01) =>
        await service.GetHeatmapAsync(gridSize);
}
