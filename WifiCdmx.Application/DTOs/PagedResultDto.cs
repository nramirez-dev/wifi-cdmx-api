namespace WifiCdmx.Application.DTOs;

/// <summary>
/// Generic paginated response wrapper.
/// </summary>
public record PagedResultDto<T>(
    IEnumerable<T> Data,
    int Total,
    int Page,
    int PageSize
);
