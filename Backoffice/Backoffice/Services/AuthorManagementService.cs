using BlogLibrary;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backoffice.Services
{
    public interface IAuthorManagementService
    {
        Task<List<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(Guid id);
        Task<Author?> CreateAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Guid id, Author author);
        Task<bool> DeleteAuthorAsync(Guid id);
    }

    public class AuthorManagementService : IAuthorManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthorManagementService> _logger;
        private const string ApiEndpoint = "Authors";

        public AuthorManagementService(HttpClient httpClient, ILogger<AuthorManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all authors");

                var response = await _httpClient.GetAsync(ApiEndpoint);
                response.EnsureSuccessStatusCode();

                var authors = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.ListAuthor);
                return authors ?? new List<Author>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve authors");
                return new List<Author>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize authors response");
                return new List<Author>();
            }
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching author with ID {Id}", id);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var author = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.Author);
                return author;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve author with ID {Id}", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize author response");
                return null;
            }
        }

        public async Task<Author?> CreateAuthorAsync(Author author)
        {
            try
            {
                _logger.LogInformation("Creating new author");

                var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, author, BlogLibraryJsonContext.Default.Author);
                response.EnsureSuccessStatusCode();

                var createdAuthor = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.Author);
                return createdAuthor;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create author");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize created author response");
                return null;
            }
        }

        public async Task<bool> UpdateAuthorAsync(Guid id, Author author)
        {
            try
            {
                _logger.LogInformation("Updating author with ID {Id}", id);

                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", author, BlogLibraryJsonContext.Default.Author);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to update author with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAuthorAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting author with ID {Id}", id);

                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to delete author with ID {Id}", id);
                return false;
            }
        }
    }
}
