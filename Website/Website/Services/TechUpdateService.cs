using TechLibrary;
using TechLibrary.Interfaces;
using System.Text.Json;

namespace Website.Services
{
    public interface ITechUpdateService
    {
        Task<List<TechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10);
        Task<List<TechUpdate>> GetTechUpdatesByProjectIdAsync(string projectId, uint page = 1, uint pageSize = 10);
        Task<uint> GetTechUpdatesCountAsync();
        Task<uint> GetTechUpdatesCountByProjectIdAsync(string projectId);
        Task<TechUpdate?> GetTechUpdateByIdAsync(Guid id);
        Task<Dictionary<string, DateTime?>> GetLastActivityDatesAsync();
    }

    public class TechUpdateService : ITechUpdateService
    {
        private readonly IAuthenticatedHttpClient _authenticatedHttpClient;
        private readonly ILogger<TechUpdateService> _logger;
        private const string ApiEndpoint = "/api/tech-projects";

        public TechUpdateService(IAuthenticatedHttpClient authenticatedHttpClient, ILogger<TechUpdateService> logger)
        {
            _authenticatedHttpClient = authenticatedHttpClient ?? throw new ArgumentNullException(nameof(authenticatedHttpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}?page={page}&pageSize={pageSize}");
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
                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}?projectId={projectId}&page={page}&pageSize={pageSize}");
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
                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/count");
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
                var response = await _authenticatedHttpClient.GetAsync($"/{ApiEndpoint}/count?projectId={projectId}");
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
                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/{id}");
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
                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/last-activity");
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