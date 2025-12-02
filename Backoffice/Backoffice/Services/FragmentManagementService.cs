using FragmentLibrary;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backoffice.Services
{
    public interface IFragmentManagementService
    {
        Task<List<Fragment>> GetAllFragmentsAsync(int page = 1, int pageSize = 10);
        Task<Fragment?> GetFragmentByIdAsync(Guid id);
        Task<Fragment?> CreateFragmentAsync(Fragment fragment);
        Task<bool> UpdateFragmentAsync(Guid id, Fragment fragment);
        Task<bool> DeleteFragmentAsync(Guid id);
        Task<List<FragmentType>> GetFragmentTypesAsync();
    }

    public class FragmentManagementService : IFragmentManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FragmentManagementService> _logger;
        private const string ApiEndpoint = "api/Fragment";

        public FragmentManagementService(HttpClient httpClient, ILogger<FragmentManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Fragment>> GetAllFragmentsAsync(int page = 1, int pageSize = 10)
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

        public async Task<Fragment?> GetFragmentByIdAsync(Guid id)
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

        public async Task<Fragment?> CreateFragmentAsync(Fragment fragment)
        {
            try
            {
                _logger.LogInformation("Creating new fragment");

                var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, fragment, FragmentLibraryJsonContext.Default.Fragment);
                response.EnsureSuccessStatusCode();

                var createdFragment = await response.Content.ReadFromJsonAsync(FragmentLibraryJsonContext.Default.Fragment);
                return createdFragment;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create fragment");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize created fragment response");
                return null;
            }
        }

        public async Task<bool> UpdateFragmentAsync(Guid id, Fragment fragment)
        {
            try
            {
                _logger.LogInformation("Updating fragment with ID {Id}", id);

                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", fragment, FragmentLibraryJsonContext.Default.Fragment);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to update fragment with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteFragmentAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting fragment with ID {Id}", id);

                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to delete fragment with ID {Id}", id);
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
    }
}
