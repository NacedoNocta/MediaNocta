using TechLibrary;
using TechLibrary.Interfaces;
using System.Text.Json;

namespace Website.Services
{
    public class TechUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TechUpdateService> _logger;

        public TechUpdateService(HttpClient httpClient, ILogger<TechUpdateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/tech-updates?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TechUpdate>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TechUpdate>();
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
                var response = await _httpClient.GetAsync($"/api/tech-updates?projectId={projectId}&page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TechUpdate>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TechUpdate>();
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
                var response = await _httpClient.GetAsync("/api/tech-updates/count");
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
                var response = await _httpClient.GetAsync($"/api/tech-updates/count?projectId={projectId}");
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
                var response = await _httpClient.GetAsync($"/api/tech-updates/{id}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TechUpdate>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
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
                var response = await _httpClient.GetAsync("/api/tech-projects/last-activity");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Dictionary<string, DateTime?>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new Dictionary<string, DateTime?>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last activity dates");
                return new Dictionary<string, DateTime?>();
            }
        }
    }
}