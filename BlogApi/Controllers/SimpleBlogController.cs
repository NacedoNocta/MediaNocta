using Microsoft.AspNetCore.Mvc;
using BlogLibrairy;

namespace BlogApi.Controllers;

[ApiController]
[Route("[controller]")]

public class SimpleBlogController : ControllerBase
{

    private readonly ILogger<SimpleBlogController> _logger;

    public SimpleBlogController(ILogger<SimpleBlogController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "simpleBlogs")]
    public IEnumerable<SimpleBlog> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new SimpleBlog
        {
            Index = index, 
            Title = "Post " + index.ToString(),
            Author = "Author " + index.ToString(),
            Content = "Content " + index.ToString(),
            Tags = "Tag " + index.ToString(),
        })
        .ToArray();
    }
}
