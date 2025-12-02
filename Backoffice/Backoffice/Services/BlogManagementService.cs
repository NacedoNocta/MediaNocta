using BlogLibrary;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backoffice.Services
{
    public interface IBlogManagementService
    {
        Task<List<Blog>> GetAllBlogsAsync(uint page = 1, uint pageSize = 10);
        Task<uint> GetBlogCountAsync();
        Task<Blog?> GetBlogByIdAsync(Guid id);
        Task<Blog?> CreateBlogAsync(Blog blog);
        Task<bool> UpdateBlogAsync(Guid id, Blog blog);
        Task<bool> DeleteBlogAsync(Guid id);
    }

    public class BlogManagementService : IBlogManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogManagementService> _logger;
        private const string ApiEndpoint = "SimpleBlogs";

        public BlogManagementService(HttpClient httpClient, ILogger<BlogManagementService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Blog>> GetAllBlogsAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching blogs for page {Page} with page size {PageSize}", page, pageSize);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                var blogs = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.ListBlog);
                return blogs ?? new List<Blog>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blogs for page {Page}", page);
                return new List<Blog>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blogs response");
                return new List<Blog>();
            }
        }

        public async Task<uint> GetBlogCountAsync()
        {
            try
            {
                _logger.LogInformation("Fetching total blog count");

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/count");
                response.EnsureSuccessStatusCode();

                var count = await response.Content.ReadFromJsonAsync<uint>();
                return count;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog count");
                return 0;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blog count response");
                return 0;
            }
        }

        public async Task<Blog?> GetBlogByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching blog with ID {Id}", id);

                var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var blog = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.Blog);
                return blog;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog with ID {Id}", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blog response");
                return null;
            }
        }

        public async Task<Blog?> CreateBlogAsync(Blog blog)
        {
            try
            {
                _logger.LogInformation("Creating new blog post");

                var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, blog, BlogLibraryJsonContext.Default.Blog);
                response.EnsureSuccessStatusCode();

                var createdBlog = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.Blog);
                return createdBlog;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create blog post");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize created blog response");
                return null;
            }
        }

        public async Task<bool> UpdateBlogAsync(Guid id, Blog blog)
        {
            try
            {
                _logger.LogInformation("Updating blog with ID {Id}", id);

                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{id}", blog, BlogLibraryJsonContext.Default.Blog);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to update blog with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteBlogAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting blog with ID {Id}", id);

                var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to delete blog with ID {Id}", id);
                return false;
            }
        }
    }
}
