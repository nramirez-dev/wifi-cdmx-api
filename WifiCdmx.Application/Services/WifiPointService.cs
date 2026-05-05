using WifiCdmx.Application.DTOs;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Domain.Entities;
using WifiCdmx.Application.Extensions;

namespace WifiCdmx.Application.Services;

/// <summary>
/// Business logic layer for WiFi point operations.
/// Applies functional programming principles throughout:
/// - Pure static functions for data transformation
/// - Curried functions for partial application
/// - Pattern matching for Option-like null handling
/// - Immutable records as DTOs
/// - Concurrent async composition
/// </summary>
public class WifiPointService(IWifiPointRepository repository) : IWifiPointService
{
    public async Task<PagedResultDto<WifiPointDto>> GetAllAsync(int page, int pageSize) =>
        await repository.GetAllAsync(page, pageSize)
            .MapAsync(ToPagedResult(page, pageSize));

    public async Task<WifiPointDto?> GetByIdAsync(Guid id) =>
        await repository.GetByIdAsync(id)
            .MapAsync(ToNullableDto);

    public async Task<PagedResultDto<WifiPointDto>> GetByBoroughAsync(
        string borough, int page, int pageSize) =>
        await repository.GetByBoroughAsync(borough, page, pageSize)
            .MapAsync(ToPagedResult(page, pageSize));

    public async Task<PagedResultDto<WifiPointDto>> GetNearbyAsync(
        double latitude, double longitude, int page, int pageSize) =>
        await repository.GetNearbyAsync(latitude, longitude, page, pageSize)
            .MapAsync(ToPagedResult(page, pageSize));

    public async Task<StatsDto> GetStatsAsync()
    {
        // Sequential execution — EF Core scoped DbContext does not support concurrent operations
        var byBorough = await repository.GetStatsByBoroughAsync();
        var byProgram = await repository.GetStatsByProgramAsync();
        var all = await repository.GetAllAsync(1, int.MaxValue);

        return new StatsDto(
            TotalPoints: all.Total,
            TotalAccessPoints: byBorough.Values.Sum(),
            ByBorough: byBorough.Select(x => new StatEntryDto(x.Key, x.Value)),
            ByProgram: byProgram.Select(x => new StatEntryDto(x.Key, x.Value))
        );
    }

    public async Task<IEnumerable<HeatmapCellDto>> GetHeatmapAsync(double gridSize) =>
        await repository.GetHeatmapAsync(gridSize);

    // --- Pure transformation functions ---

    /// <summary>
    /// Pure function: maps a domain entity to a DTO.
    /// No side effects — same input always produces same output.
    /// </summary>
    private static WifiPointDto ToDto(WifiPoint point) => new(
        point.Id,
        point.OriginalId,
        point.Program,
        point.Latitude,
        point.Longitude,
        point.Borough
    );

    /// <summary>
    /// Option-like null handling using pattern matching.
    /// Transforms the value if present, returns null otherwise.
    /// </summary>
    private static WifiPointDto? ToNullableDto(WifiPoint? point) => point switch
    {
        null => null,
        var p => ToDto(p)
    };

    /// <summary>
    /// Curried function — returns a transformation function pre-loaded with
    /// pagination parameters. Enables clean pipeline composition.
    /// </summary>
    private static Func<(IEnumerable<WifiPoint> Items, int Total), PagedResultDto<WifiPointDto>>
        ToPagedResult(int page, int pageSize) =>
            result => new PagedResultDto<WifiPointDto>(
                result.Items.Select(ToDto),
                result.Total,
                page,
                pageSize
            );
}
