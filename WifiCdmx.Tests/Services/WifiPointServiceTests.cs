using Moq;
using WifiCdmx.Application.Interfaces;
using WifiCdmx.Application.Services;
using WifiCdmx.Domain.Entities;

namespace WifiCdmx.Tests.Services;

/// <summary>
/// Unit tests for WifiPointService.
/// Uses Moq to mock the repository layer — no database required.
/// </summary>
public class WifiPointServiceTests
{
    private readonly Mock<IWifiPointRepository> _repositoryMock;
    private readonly WifiPointService _service;

    public WifiPointServiceTests()
    {
        _repositoryMock = new Mock<IWifiPointRepository>();
        _service = new WifiPointService(_repositoryMock.Object);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult_WithCorrectTotal()
    {
        // Arrange
        var fakePoints = GenerateFakePoints(3);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 20))
            .ReturnsAsync((fakePoints, 3));

        // Act
        var result = await _service.GetAllAsync(1, 20);

        // Assert
        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.Data.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoPointsExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 20))
            .ReturnsAsync((new List<WifiPoint>(), 0));

        // Act
        var result = await _service.GetAllAsync(1, 20);

        // Assert
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Data);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenPointExists()
    {
        // Arrange
        var point = GenerateFakePoints(1).First();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(point.Id))
            .ReturnsAsync(point);

        // Act
        var result = await _service.GetByIdAsync(point.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(point.Id, result.Id);
        Assert.Equal(point.Borough, result.Borough);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPointDoesNotExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync("non-existent-id"))
            .ReturnsAsync((WifiPoint?)null);

        // Act
        var result = await _service.GetByIdAsync("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    // --- GetByBoroughAsync ---

    [Fact]
    public async Task GetByBoroughAsync_ReturnsOnlyMatchingBorough()
    {
        // Arrange
        var fakePoints = GenerateFakePoints(2, borough: "IZTAPALAPA");
        _repositoryMock
            .Setup(r => r.GetByBoroughAsync("IZTAPALAPA", 1, 20))
            .ReturnsAsync((fakePoints, 2));

        // Act
        var result = await _service.GetByBoroughAsync("IZTAPALAPA", 1, 20);

        // Assert
        Assert.Equal(2, result.Total);
        Assert.All(result.Data, p => Assert.Equal("IZTAPALAPA", p.Borough));
    }

    // --- GetNearbyAsync ---

    [Fact]
    public async Task GetNearbyAsync_ReturnsPagedResult()
    {
        // Arrange
        var fakePoints = GenerateFakePoints(3);
        _repositoryMock
            .Setup(r => r.GetNearbyAsync(19.4326, -99.1332, 1, 20))
            .ReturnsAsync((fakePoints, 3));

        // Act
        var result = await _service.GetNearbyAsync(19.4326, -99.1332, 1, 20);

        // Assert
        Assert.Equal(3, result.Total);
        Assert.Equal(3, result.Data.Count());
    }

    // --- GetStatsAsync ---

    [Fact]
    public async Task GetStatsAsync_ReturnsCorrectTotals()
    {
        // Arrange
        var byBorough = new Dictionary<string, int>
        {
            { "IZTAPALAPA", 10 },
            { "BENITO JUAREZ", 5 }
        };
        var byProgram = new Dictionary<string, int>
        {
            { "PILARES", 8 },
            { "Centros_de_Salud", 7 }
        };

        _repositoryMock
            .Setup(r => r.GetStatsByBoroughAsync())
            .ReturnsAsync(byBorough);
        _repositoryMock
            .Setup(r => r.GetStatsByProgramAsync())
            .ReturnsAsync(byProgram);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, int.MaxValue))
            .ReturnsAsync((GenerateFakePoints(3), 3));

        // Act
        var result = await _service.GetStatsAsync();

        // Assert
        Assert.Equal(3, result.TotalPoints);
        Assert.Equal(15, result.TotalAccessPoints);
        Assert.Equal(2, result.ByBorough.Count);
        Assert.Equal(2, result.ByProgram.Count);
    }

    // --- Helpers ---

    private static List<WifiPoint> GenerateFakePoints(int count, string borough = "ÁLVARO OBREGÓN") =>
        Enumerable.Range(1, count).Select(i => new WifiPoint
        {
            Id = $"point-{i}",
            Neighborhood = $"Colonia {i}",
            Borough = borough,
            Latitude = 19.4326 + i * 0.001,
            Longitude = -99.1332 + i * 0.001,
            AccessPointCount = i,
            Program = "PILARES"
        }).ToList();
}
