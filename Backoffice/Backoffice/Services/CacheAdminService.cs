using System.Net.Http.Json;
using CacheLibrary;

namespace Backoffice.Services;

public interface ICacheAdminService
{
    Task<CacheGroupsResponse?> GetGroupsAsync(CancellationToken ct = default);
    Task<CacheEntriesResponse?> GetEntriesAsync(string? @namespace, string? producer, string? routePrefix, int page, int pageSize, CancellationToken ct = default);
    Task<bool> EvictEntryAsync(string keyBase64, CancellationToken ct = default);
    Task<int> EvictGroupAsync(string @namespace, string producer, CancellationToken ct = default);
    Task<int> FlushAllAsync(CancellationToken ct = default);
}

public sealed class CacheAdminService : ICacheAdminService
{
    private readonly HttpClient _http;
    private readonly ILogger<CacheAdminService> _logger;

    public CacheAdminService(HttpClient http, ILogger<CacheAdminService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<CacheGroupsResponse?> GetGroupsAsync(CancellationToken ct = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<CacheGroupsResponse>("groups", ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch cache groups");
            return null;
        }
    }

    public async Task<CacheEntriesResponse?> GetEntriesAsync(string? @namespace, string? producer, string? routePrefix, int page, int pageSize, CancellationToken ct = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrEmpty(@namespace)) query.Add($"namespace={Uri.EscapeDataString(@namespace)}");
        if (!string.IsNullOrEmpty(producer)) query.Add($"producer={Uri.EscapeDataString(producer)}");
        if (!string.IsNullOrEmpty(routePrefix)) query.Add($"routePrefix={Uri.EscapeDataString(routePrefix)}");
        query.Add($"page={page}");
        query.Add($"pageSize={pageSize}");
        var url = "entries?" + string.Join("&", query);

        try
        {
            return await _http.GetFromJsonAsync<CacheEntriesResponse>(url, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch cache entries for {Url}", url);
            return null;
        }
    }

    public async Task<bool> EvictEntryAsync(string keyBase64, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.DeleteAsync($"entries/{keyBase64}", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to evict cache entry {Key}", keyBase64);
            return false;
        }
    }

    public async Task<int> EvictGroupAsync(string @namespace, string producer, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.DeleteAsync($"groups?namespace={Uri.EscapeDataString(@namespace)}&producer={Uri.EscapeDataString(producer)}", ct);
            if (!response.IsSuccessStatusCode) return 0;
            var body = await response.Content.ReadFromJsonAsync<CacheEvictionResponse>(ct);
            return body?.EvictedCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to evict cache group {Namespace}/{Producer}", @namespace, producer);
            return 0;
        }
    }

    public async Task<int> FlushAllAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _http.DeleteAsync("all?confirm=YES_FLUSH", ct);
            if (!response.IsSuccessStatusCode) return 0;
            var body = await response.Content.ReadFromJsonAsync<CacheEvictionResponse>(ct);
            return body?.EvictedCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to flush cache");
            return 0;
        }
    }
}
