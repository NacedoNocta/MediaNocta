using BlogLibrary;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;

namespace ActivityAPI.Controllers;

[ApiController]
[Route("")]
public class ActivityController : ControllerBase
{
    private readonly ILogger<ActivityController> _logger;

    [HttpGet(Name = "recent")]
    [Route("recent")]
    public IEnumerable<Activity> GetRecent()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");

        Activity[] recent = Enumerable.Range(1, 5).Select(index => new Blog(
            ("Post " + index.ToString()),
            "Summary " + index.ToString(),
            "Content " + index.ToString(),
            author,
            null,
            imageUrl: "https://cdn.discordapp.com/attachments/915961412537450556/1343137362154225737/image.png?ex=67fe18a5&is=67fcc725&hm=dc6d8f2f0f125b29d108fa1e7f80e64c297dbab4c3127212357480c3a1289853&"
            )
        )
        .ToArray();
        return recent;
    }

    [HttpGet(Name = "pinned")]
    [Route("pinned")]
    public IEnumerable<Activity> GetPinned()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");

        return Enumerable.Range(1, 2).Select(index => new Blog(
            ("Post " + index.ToString()),
            "Summary " + index.ToString(),
            "Content " + index.ToString(),
            author,
            null,
            imageUrl: "https://cdn.discordapp.com/attachments/915961412537450556/1343137362154225737/image.png?ex=67fe18a5&is=67fcc725&hm=dc6d8f2f0f125b29d108fa1e7f80e64c297dbab4c3127212357480c3a1289853&"
        )
        ).ToArray();
    }

    [HttpGet(Name = "random")]
    [Route("simpleBlogs")]
    public IEnumerable<Activity> GetRandom()
    {
        Author author = new Author("Author Name", "Author Biography", "Author Image Url");
        return [new Blog("Post ", "Summary", "Content ", author, null)];
    }
}

