using FragmentLibrary;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Website.Services
{
    public interface IFragmentService
    {
        Task<List<Fragment>> GetFragmentsAsync(int page = 1, int pageSize = 10);
        Task<Fragment?> GetFragmentAsync(Guid id);
        Task<bool> AddVibeAsync(Guid fragmentId);
        Task<List<FragmentType>> GetFragmentTypesAsync();
        Task<List<Fragment>> SearchFragmentsAsync(string query, string? typeTag = null);
        Task<bool> RemoveVibeAsync(Guid fragmentId);
        Task<List<Fragment>> GetFragmentsByTypeAsync(string typeTag);
    }

    public class FragmentService : IFragmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FragmentService> _logger;
        private const string ApiEndpoint = "api/Fragment";

        public FragmentService(HttpClient httpClient, ILogger<FragmentService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Fragment>> GetFragmentsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching fragments for page {Page} with page size {PageSize}", page, pageSize);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                var fragments = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.IEnumerableFragment);
                return fragments?.ToList() ?? new List<Fragment>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve fragments for page {Page}", page);
                return new List<Fragment>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize fragments response");
                return new List<Fragment>();
            }
        }

        public async Task<Fragment?> GetFragmentAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching fragment with ID {Id}", id);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var fragment = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.Fragment);
                return fragment;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve fragment with ID {Id}", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize fragment response");
                return null;
            }
        }

        public async Task<bool> AddVibeAsync(Guid fragmentId)
        {
            try
            {
                _logger.LogInformation("Adding vibe to fragment {FragmentId}", fragmentId);

                var response = await _httpClient.PostAsync($"{ApiEndpoint}/{fragmentId}/vibe", null);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to add vibe to fragment {FragmentId}", fragmentId);
                return false;
            }
        }

        public async Task<List<FragmentType>> GetFragmentTypesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching fragment types");

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/types");
                response.EnsureSuccessStatusCode();

                var types = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.IEnumerableFragmentType);
                return types?.ToList() ?? new List<FragmentType>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve fragment types");
                return new List<FragmentType>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize fragment types response");
                return new List<FragmentType>();
            }
        }

        public async Task<List<Fragment>> SearchFragmentsAsync(string query, string? typeTag = null)
        {
            try
            {
                _logger.LogInformation("Searching fragments with query '{Query}' and type '{TypeTag}'", query, typeTag);

                var url = $"{ApiEndpoint}/search?query={Uri.EscapeDataString(query)}";
                if (!string.IsNullOrEmpty(typeTag))
                {
                    url += $"&typeTag={Uri.EscapeDataString(typeTag)}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var fragments = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.IEnumerableFragment);
                return fragments?.ToList() ?? new List<Fragment>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to search fragments");
                return new List<Fragment>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize fragments search response");
                return new List<Fragment>();
            }
        }

        public async Task<bool> RemoveVibeAsync(Guid fragmentId)
        {
            try
            {
                _logger.LogInformation("Removing vibe from fragment {FragmentId}", fragmentId);

                var response = await _httpClient.PostAsync($"{ApiEndpoint}/{fragmentId}/unvibe", null);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to remove vibe from fragment {FragmentId}", fragmentId);
                return false;
            }
        }

        public async Task<List<Fragment>> GetFragmentsByTypeAsync(string typeTag)
        {
            try
            {
                _logger.LogInformation("Fetching fragments by type '{TypeTag}'", typeTag);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/by-type/{Uri.EscapeDataString(typeTag)}");
                response.EnsureSuccessStatusCode();

                var fragments = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.IEnumerableFragment);
                return fragments?.ToList() ?? new List<Fragment>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve fragments by type {TypeTag}", typeTag);
                return new List<Fragment>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize fragments by type response");
                return new List<Fragment>();
            }
        }
    }
}