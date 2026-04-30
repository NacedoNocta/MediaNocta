using TechLibrary;
using TechLibrary.Interfaces;
using System.Text.Json;

namespace Website.Services
{
    public class TechUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TechUpdateService> _logger;
        private static readonly TimeSpan ListTtl = TimeSpan.FromDays(1);
        private static readonly TimeSpan ActivityTtl = TimeSpan.FromHours(1);
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        public TechUpdateService(HttpClient httpClient, ILogger<TechUpdateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tech-updates?page={page}&pageSize={pageSize}");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TechUpdate>>(content, JsonOpts) ?? new List<TechUpdate>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates for page {Page}", page);
                return new List<TechUpdate>();
            }
        }

        public async Task<List<TechUpdate>> GetTechUpdatesByProjectIdAsync(string projectId, uint page = 1, uint pageSize = 10)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tech-updates?projectId={projectId}&page={page}&pageSize={pageSize}");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TechUpdate>>(content, JsonOpts) ?? new List<TechUpdate>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates for project {ProjectId}, page {Page}", projectId, page);
                return new List<TechUpdate>();
            }
        }

        public async Task<uint> GetTechUpdatesCountAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/api/tech-updates/count");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return uint.Parse(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates count");
                return 0;
            }
        }

        public async Task<uint> GetTechUpdatesCountByProjectIdAsync(string projectId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tech-updates/count?projectId={projectId}");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return uint.Parse(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates count for project {ProjectId}", projectId);
                return 0;
            }
        }

        public async Task<TechUpdate?> GetTechUpdateByIdAsync(Guid id)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tech-updates/{id}");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TechUpdate>(content, JsonOpts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech update with ID {Id}", id);
                return null;
            }
        }

        public async Task<Dictionary<string, DateTime?>> GetLastActivityDatesAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/api/tech-projects/last-activity");
                request.Options.Set(WebsiteCachingHandler.CacheableTtl, ActivityTtl);
                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Dictionary<string, DateTime?>>(content, JsonOpts) ?? new Dictionary<string, DateTime?>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last activity dates");
                return new Dictionary<string, DateTime?>();
            }
        }
    }
}
