using Microsoft.EntityFrameworkCore;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Domain.Entities;
using WifiCdmx.Infrastructure.Data;

namespace WifiCdmx.Infrastructure.Repositories;

/// <summary>
/// PostgreSQL implementation of IWifiPointRepository using EF Core.
/// Proximity search uses the Haversine formula calculated in SQL.
/// </summary>
public class WifiPointRepository(AppDbContext context) : IWifiPointRepository
{
    public async Task<(IEnumerable<WifiPoint> Items, int Total)> GetAllAsync(int page, int pageSize)
    {
        var query = context.WifiPoints.AsNoTracking();
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task<WifiPoint?> GetByIdAsync(string id) =>
        await context.WifiPoints.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<(IEnumerable<WifiPoint> Items, int Total)> GetByBoroughAsync(
        string borough, int page, int pageSize)
    {
        var query = context.WifiPoints.AsNoTracking()
            .Where(w => w.Borough.ToLower() == borough.ToLower());
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task<(IEnumerable<WifiPoint> Items, int Total)> GetNearbyAsync(
        double latitude, double longitude, int page, int pageSize)
    {
        // Haversine formula implemented in PostgreSQL via raw SQL for efficiency
        const double earthRadiusKm = 6371.0;
        var items = await context.WifiPoints
            .AsNoTracking()
            .OrderBy(w =>
                earthRadiusKm * 2 * Math.Asin(Math.Sqrt(
                    Math.Pow(Math.Sin((w.Latitude - latitude) * Math.PI / 180 / 2), 2) +
                    Math.Cos(latitude * Math.PI / 180) *
                    Math.Cos(w.Latitude * Math.PI / 180) *
                    Math.Pow(Math.Sin((w.Longitude - longitude) * Math.PI / 180 / 2), 2)
                )))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await context.WifiPoints.CountAsync();
        return (items, total);
    }

    public async Task<Dictionary<string, int>> GetStatsByBoroughAsync() =>
        await context.WifiPoints
            .AsNoTracking()
            .GroupBy(w => w.Borough)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(w => w.AccessPointCount));

    public async Task<Dictionary<string, int>> GetStatsByProgramAsync() =>
        await context.WifiPoints
            .AsNoTracking()
            .GroupBy(w => w.Program)
            .ToDictionaryAsync(g => g.Key, g => g.Sum(w => w.AccessPointCount));

    public async Task BulkInsertAsync(IEnumerable<WifiPoint> points)
    {
        await context.WifiPoints.AddRangeAsync(points);
        await context.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync() =>
        await context.WifiPoints.AnyAsync();
}