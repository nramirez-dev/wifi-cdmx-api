using Microsoft.AspNetCore.Mvc;
using WifiCdmx.Application.DTOs;
using WifiCdmx.Application.Interfaces;

namespace WifiCdmx.API.Controllers;

/// <summary>
/// REST API controller for querying Mexico City WiFi access points.
/// </summary>
[ApiController]
[Route("api/wifi-points")]
[Produces("application/json")]
public class WifiPointsController(IWifiPointService service) : ControllerBase
{
    /// <summary>
    /// Returns a paginated list of all WiFi access points.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<WifiPointDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than 0.");

        var result = await service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single WiFi point by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WifiPointDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Returns a paginated list of WiFi points filtered by borough (alcaldía).
    /// </summary>
    [HttpGet("borough/{borough}")]
    [ProducesResponseType(typeof(PagedResultDto<WifiPointDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByBorough(
        string borough,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than 0.");

        var result = await service.GetByBoroughAsync(borough, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Returns a paginated list of WiFi points ordered by proximity to given coordinates.
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(PagedResultDto<WifiPointDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be greater than 0.");

        var result = await service.GetNearbyAsync(lat, lon, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Returns aggregated statistics about WiFi points by borough and program.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var result = await service.GetStatsAsync();
        return Ok(result);
    }
}
