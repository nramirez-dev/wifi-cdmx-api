using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Infrastructure.Seeders;

/// <summary>
/// Reads the official CDMX WiFi dataset and seeds the database on startup.
/// Only runs if the database is empty to avoid duplicate data.
/// Supports Excel (.xlsx) format as provided by the official CDMX open data portal.
/// </summary>
public class DataSeeder(IWifiPointRepository repository, ILogger<DataSeeder> logger)
{
    public async Task SeedAsync(string filePath)
    {
        if (await repository.AnyAsync())
        {
            logger.LogInformation("Database already contains WiFi points. Skipping seed.");
            return;
        }

        if (!File.Exists(filePath))
        {
            logger.LogWarning("Data file not found at path: {Path}. Skipping seed.", filePath);
            return;
        }

        logger.LogInformation("Starting seed from {Path}...", filePath);

        var points = new List<WifiPoint>();

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed()!.RowsUsed().Skip(1);

        int skipped = 0;
        foreach (var row in rows)
        {
            var latStr = row.Cell(3).GetString();
            var lonStr = row.Cell(4).GetString();

            if (!double.TryParse(latStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(lonStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var lon))
            {
                skipped++;
                continue;
            }

            var point = new WifiPoint
            {
                Id = Guid.NewGuid(),
                OriginalId = row.Cell(1).GetString(),
                Program = row.Cell(2).GetString(),
                Latitude = lat,
                Longitude = lon,
                Borough = row.Cell(5).GetString(),
            };
            points.Add(point);
        }

        if (skipped > 0)
            logger.LogWarning("Skipped {Count} rows with unparseable coordinates.", skipped);

        await repository.BulkInsertAsync(points);
        logger.LogInformation("Seeded {Count} WiFi points successfully.", points.Count);
    }
}
