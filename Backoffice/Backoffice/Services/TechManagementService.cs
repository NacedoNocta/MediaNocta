using TechLibrary;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backoffice.Services
{
    public interface ITechManagementService
    {
        Task<List<TechUpdate>> GetAllTechUpdatesAsync(uint page = 1, uint pageSize = 10);
        Task<uint> GetTechUpdateCountAsync();
        Task<TechUpdate?> GetTechUpdateByIdAsync(Guid id);
        Task<TechUpdate?> CreateTechUpdateAsync(TechUpdate techUpdate);
        Task<bool> UpdateTechUpdateAsync(Guid id, TechUpdate techUpdate);
        Task<bool> DeleteTechUpdateAsync(Guid id);
    }

    public class TechManagementService : ITechManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TechManagementService> _logger;
        private const string ApiEndpoint = "api/tech-updates";

        public TechManagementService(HttpClient httpClient, ILogger<TechManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TechUpdate>> GetAllTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching tech updates for page {Page} with page size {PageSize}", page, pageSize);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var techUpdates = JsonSerializer.Deserialize<List<TechUpdate>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return techUpdates ?? new List<TechUpdate>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve tech updates for page {Page}", page);
                return new List<TechUpdate>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize tech updates response");
                return new List<TechUpdate>();
            }
        }

        public async Task<uint> GetTechUpdateCountAsync()
        {
            try
            {
                _logger.LogInformation("Fetching total tech update count");

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/count");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return uint.Parse(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve tech update count");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse tech update count response");
                return 0;
            }
        }

        public async Task<TechUpdate?> GetTechUpdateByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching tech update with ID {Id}", id);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var techUpdate = JsonSerializer.Deserialize<TechUpdate>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return techUpdate;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve tech update with ID {Id}", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize tech update response");
                return null;
            }
        }

        public async Task<TechUpdate?> CreateTechUpdateAsync(TechUpdate techUpdate)
        {
            try
            {
                _logger.LogInformation("Creating new tech update");

                var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, techUpdate);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var createdTechUpdate = JsonSerializer.Deserialize<TechUpdate>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return createdTechUpdate;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create tech update");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize created tech update response");
                return null;
            }
        }

        public async Task<bool> UpdateTechUpdateAsync(Guid id, TechUpdate techUpdate)
        {
            try
            {
                _logger.LogInformation("Updating tech update with ID {Id}", id);

                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", techUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to update tech update with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteTechUpdateAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting tech update with ID {Id}", id);

                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to delete tech update with ID {Id}", id);
                return false;
            }
        }
    }
}
