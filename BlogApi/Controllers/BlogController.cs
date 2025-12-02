using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
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

    [HttpGet("SimpleBlogs", Name = "GetBlogPosts")]
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
    public async Task<ActionResult<IBlog>> CreateBlogPost([FromBody] Blog blog)
    {
        try
        {
            if (blog == null)
            {
                return BadRequest("Blog post cannot be null");
            }

            var createdPost = await _blogService.CreateBlogPostAsync(blog);
            return CreatedAtRoute("GetBlogPostById", new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog post");
            return StatusCode(500, "An error occurred while creating the blog post");
        }
    }

    [HttpPut("SimpleBlogs/{id:guid}", Name = "UpdateBlogPost")]
    public async Task<ActionResult<IBlog>> UpdateBlogPost(Guid id, [FromBody] Blog blog)
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

            return Ok(updatedPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog post with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the blog post");
        }
    }

    [HttpDelete("SimpleBlogs/{id:guid}", Name = "DeleteBlogPost")]
    public async Task<ActionResult> DeleteBlogPost(Guid id)
    {
        try
        {
            var result = await _blogService.DeleteBlogPostAsync(id);

            if (!result)
            {
                _logger.LogWarning("Blog post with ID {Id} not found for deletion", id);
                return NotFound($"Blog post with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the blog post");
        }
    }
}