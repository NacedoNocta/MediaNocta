using BlogApi.Interfaces;
using BlogLibrairy;
using BlogLibrairy.Interfaces;

namespace BlogApi.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly ILogger<BlogService> _logger;

    public BlogService(IBlogRepository blogRepository, ILogger<BlogService> logger)
    {
        _blogRepository = blogRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<IBlog>> GetBlogPostsAsync(uint page = 1, uint pageSize = 10)
    {
        _logger.LogInformation("Retrieving blog posts for page {Page} with page size {PageSize}", page, pageSize);
        
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        return await _blogRepository.GetAllAsync((int)page, (int)pageSize);
    }

    public async Task<IBlog?> GetBlogPostByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving blog post with ID {Id}", id);
        return await _blogRepository.GetByIdAsync(id);
    }

    public async Task<uint> GetBlogPostCountAsync()
    {
        _logger.LogInformation("Retrieving total blog post count");
        return await _blogRepository.GetCountAsync();
    }

    public async Task<IBlog> CreateBlogPostAsync(IBlog blog)
    {
        _logger.LogInformation("Creating new blog post with title {Title}", blog.Title);
        return await _blogRepository.CreateAsync(blog);
    }

    public async Task<IBlog?> UpdateBlogPostAsync(IBlog blog)
    {
        _logger.LogInformation("Updating blog post with ID {Id}", blog.Id);
        return await _blogRepository.UpdateAsync(blog);
    }

    public async Task<bool> DeleteBlogPostAsync(Guid id)
    {
        _logger.LogInformation("Deleting blog post with ID {Id}", id);
        return await _blogRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<IBlog>> GetBlogPostsByAuthorAsync(Guid authorId)
    {
        _logger.LogInformation("Retrieving blog posts for author ID {AuthorId}", authorId);
        return await _blogRepository.GetByAuthorIdAsync(authorId);
    }

    public async Task<IEnumerable<IBlog>> GetBlogPostsByTagAsync(Guid tagId)
    {
        _logger.LogInformation("Retrieving blog posts for tag ID {TagId}", tagId);
        return await _blogRepository.GetByTagIdAsync(tagId);
    }
}