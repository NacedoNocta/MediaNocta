using ArtLibrairy;
using BlogLibrairy;
using Microsoft.AspNetCore.Mvc;
using SharedLibrairy;

namespace ActivityAPI.Controllers;

[ApiController]
[Route("")]
public class ActivityController : ControllerBase
{
    private readonly ILogger<ActivityController> _logger;

    [HttpGet(Name = "recent")]
    [Route("recent")]
    public IEnumerable<IActivity> GetRecent()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");

        IActivity[] recent = Enumerable.Range(1, 5).Select(index => new Blog(
            ("Post " + index.ToString()),
            "Summary " + index.ToString(),
            "Content " + index.ToString(),
            author,
            null)
        )
        .ToArray();
        recent = [.. recent, new Art("Post 6", "Summary 6", "Content 6")];
        return recent;
    }

    [HttpGet(Name = "pinned")]
    [Route("pinned")]
    public IEnumerable<IActivity> GetPinned()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");

        return Enumerable.Range(1, 2).Select(index => new Blog(
            ("Post " + index.ToString()),
            "Summary " + index.ToString(),
            "Content " + index.ToString(),
            author,
            null)
        )
        .ToArray();
    }

    [HttpGet(Name = "random")]
    [Route("simpleBlogs")]
    public IEnumerable<IActivity> GetRandom()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");
        return [new Blog("Post ", "Summary", "Content ", author, null)];
    }
}

