using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using CacheLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BlogApi.Controllers;

[ApiController]
[Route("")]
public class BlogController : ControllerBase
{
    private readonly ILogger<BlogController> _logger;
    private readonly IBlogService _blogService;
    private readonly IOutputCacheStore _cacheStore;

    public BlogController(ILogger<BlogController> logger, IBlogService blogService, IOutputCacheStore cacheStore)
    {
        _logger = logger;
        _blogService = blogService;
        _cacheStore = cacheStore;
    }

    [HttpGet("SimpleBlogs", Name = "GetBlogPosts")]
    [OutputCache(PolicyName = "BlogList")]
    public async Task<ActionResult<IEnumerable<IBlog>>> GetBlogPosts([FromQuery] uint page = 1, [FromQuery] uint pageSize = 10)
    {
        try
        {
            var posts = await _blogService.GetBlogPostsAsync(page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blog posts for page {Page}", page);
            return StatusCode(500, "An error occurred while retrieving blog posts");
        }
    }

    [HttpGet("SimpleBlogs/count", Name = "GetBlogPostCount")]
    [OutputCache(PolicyName = "BlogCount")]
    public async Task<ActionResult<uint>> GetBlogPostCount()
    {
        try
        {
            var count = await _blogService.GetBlogPostCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blog post count");
            return StatusCode(500, "An error occurred while retrieving blog post count");
        }
    }

    [HttpGet("SimpleBlogs/{id:guid}", Name = "GetBlogPostById")]
    [OutputCache(PolicyName = "BlogDetail")]
    public async Task<ActionResult<IBlog>> GetBlogPostById(Guid id)
    {
        try
        {
            var post = await _blogService.GetBlogPostByIdAsync(id);

            if (post == null)
            {
                _logger.LogWarning("Blog post with ID {Id} not found", id);
                return NotFound($"Blog post with ID {id} not found");
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blog post with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the blog post");
        }
    }

    [HttpPost("SimpleBlogs", Name = "CreateBlogPost")]
    public async Task<ActionResult<IBlog>> CreateBlogPost([FromBody] Blog blog, CancellationToken ct)
    {
        try
        {
            if (blog == null)
            {
                return BadRequest("Blog post cannot be null");
            }

            var createdPost = await _blogService.CreateBlogPostAsync(blog);
            await EvictAllAsync(ct);
            return CreatedAtRoute("GetBlogPostById", new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog post");
            return StatusCode(500, "An error occurred while creating the blog post");
        }
    }

    [HttpPut("SimpleBlogs/{id:guid}", Name = "UpdateBlogPost")]
    public async Task<ActionResult<IBlog>> UpdateBlogPost(Guid id, [FromBody] Blog blog, CancellationToken ct)
    {
        try
        {
            if (blog == null)
            {
                return BadRequest("Blog post cannot be null");
            }

            if (id != blog.Id)
            {
                return BadRequest("ID mismatch");
            }

            var updatedPost = await _blogService.UpdateBlogPostAsync(blog);

            if (updatedPost == null)
            {
                _logger.LogWarning("Blog post with ID {Id} not found for update", id);
                return NotFound($"Blog post with ID {id} not found");
            }

            await EvictAllAsync(ct);
            return Ok(updatedPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog post with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the blog post");
        }
    }

    [HttpDelete("SimpleBlogs/{id:guid}", Name = "DeleteBlogPost")]
    public async Task<ActionResult> DeleteBlogPost(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _blogService.DeleteBlogPostAsync(id);

            if (!result)
            {
                _logger.LogWarning("Blog post with ID {Id} not found for deletion", id);
                return NotFound($"Blog post with ID {id} not found");
            }

            await EvictAllAsync(ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the blog post");
        }
    }

    // OutputCache.Tag() takes literal strings only, so per-id detail tags aren't possible
    // without a custom IOutputCachePolicy. Sledgehammer eviction is acceptable here:
    // 1-day TTLs bound the over-eviction cost, and writes are infrequent.
    private async Task EvictAllAsync(CancellationToken ct)
    {
        await _cacheStore.EvictByTagAsync(CacheTags.BlogApi.List, ct);
        await _cacheStore.EvictByTagAsync(CacheTags.BlogApi.Count, ct);
        await _cacheStore.EvictByTagAsync(CacheTags.BlogApi.DetailAll, ct);
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-list");
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-count");
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-detail");
    }
}
