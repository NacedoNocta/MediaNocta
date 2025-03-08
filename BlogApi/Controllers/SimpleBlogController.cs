using BlogLibrairy;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("")]

public class SimpleBlogController : ControllerBase
{

    private readonly ILogger<SimpleBlogController> _logger;

    public SimpleBlogController(ILogger<SimpleBlogController> logger)
    {
        _logger = logger;
    }

    [Route("simpleBlogs")]
    [HttpGet(Name = "simpleBlogs")]
    public IEnumerable<Blog> Get()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");

        return Enumerable.Range(1, 5).Select(index => new Blog(
            ("Post " + index.ToString()),
            "Summary " + index.ToString(),
            "Content " + index.ToString(),
            author,
            null)
        )
        .ToArray();
    }
}
