using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Infrastructure.Seeders;

/// <summary>
/// Reads the CDMX WiFi CSV file and seeds the database on startup.
/// Only runs if the database is empty to avoid duplicate data.
/// </summary>
public class CsvSeeder(IWifiPointRepository repository, ILogger<CsvSeeder> logger)
{
    public async Task SeedAsync(string csvFilePath)
    {
        if (await repository.AnyAsync())
        {
            logger.LogInformation("Database already contains WiFi points. Skipping seed.");
            return;
        }

        if (!File.Exists(csvFilePath))
        {
            logger.LogWarning("CSV file not found at path: {Path}. Skipping seed.", csvFilePath);
            return;
        }

        logger.LogInformation("Starting CSV seed from {Path}...", csvFilePath);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
        };

        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, config);

        var points = new List<WifiPoint>();

        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var point = new WifiPoint
            {
                Id = Guid.NewGuid(),
                Name = csv.GetField("ID") ?? string.Empty,
                Neighborhood = csv.GetField("Colonia") ?? string.Empty,
                Borough = csv.GetField("Alcaldía") ?? string.Empty,
                Latitude = csv.GetField<double>("Latitud"),
                Longitude = csv.GetField<double>("Longitud"),
                AccessPointCount = csv.GetField<int>("Puntos_de_acceso"),
                Program = csv.GetField("Programa") ?? string.Empty,
            };

            points.Add(point);
        }

        await repository.BulkInsertAsync(points);
        logger.LogInformation("Seeded {Count} WiFi points successfully.", points.Count);
    }
}
