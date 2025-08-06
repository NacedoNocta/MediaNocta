using BlogLibrary;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;


namespace Website.Services
{
    public interface IBlogService
    {
        Task<List<Blog>> GetPostListAsync(uint page, uint pageSize = 10);
        Task<uint> GetPostCountAsync();
        Task<Blog?> GetPostByIdAsync(Guid id);
    }

    public class BlogService : IBlogService
    {
        private readonly IAuthenticatedHttpClient _authenticatedHttpClient;
        private readonly ILogger<BlogService> _logger;
        private const string ApiEndpoint = "/api/blog/SimpleBlogs";

        public BlogService(IAuthenticatedHttpClient authenticatedHttpClient, ILogger<BlogService> logger)
        {
            _authenticatedHttpClient = authenticatedHttpClient ?? throw new ArgumentNullException(nameof(authenticatedHttpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Blog>> GetPostListAsync(uint page, uint pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching blog posts for page {Page} with page size {PageSize}", page, pageSize);

                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                var posts = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.ListBlog);
                return posts ?? new List<Blog>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog posts for page {Page}", page);
                return new List<Blog>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blog posts response");
                return new List<Blog>();
            }
        }

        public async Task<uint> GetPostCountAsync()
        {
            try
            {
                _logger.LogInformation("Fetching total blog post count");

                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/count");
                
                response.EnsureSuccessStatusCode();

                var count = await response.Content.ReadFromJsonAsync<uint>();
                return count;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog post count");
                return 0;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blog post count response");
                return 0;
            }
        }

        public async Task<Blog?> GetPostByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching blog post with ID {Id}", id);

                var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/{id}");
                response.EnsureSuccessStatusCode();

                var post = await response.Content.ReadFromJsonAsync(BlogLibraryJsonContext.Default.Blog);
                return post;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve blog post with ID {Id}", id);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize blog post response");
                return null;
            }
        }
    }
}
