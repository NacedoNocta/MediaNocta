using BlogApi.Interfaces;
using BlogLibrairy;
using BlogLibrairy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("")]
public class BlogController : ControllerBase
{
    private readonly ILogger<BlogController> _logger;
    private readonly IBlogService _blogService;

    public BlogController(ILogger<BlogController> logger, IBlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    [Route("SimpleBlogs")]
    [HttpGet(Name = "GetBlogPosts")]
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

    [Route("SimpleBlogs/count")]
    [HttpGet(Name = "GetBlogPostCount")]
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

    [Route("SimpleBlogs/{id:guid}")]
    [HttpGet(Name = "GetBlogPostById")]
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
}