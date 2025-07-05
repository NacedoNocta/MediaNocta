using FragmentLibrary;
using Microsoft.AspNetCore.Mvc;

namespace FragmentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FragmentController : ControllerBase
{
    private readonly ILogger<FragmentController> _logger;

    public FragmentController(ILogger<FragmentController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Fragment>> GetFragments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Prototype implementation - return mock data
        var mockFragments = new List<Fragment>
        {
            new Fragment("First Fragment", "This is my first fragment", "# Hello World\n\nThis is a test fragment with some markdown content.", "https://picsum.photos/400/300?random=1")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 5,
                IsPublic = true
            },
            new Fragment("Code Snippet", "A useful code snippet", "```csharp\nConsole.WriteLine(\"Hello, World!\");\n```", "https://picsum.photos/400/300?random=2")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 12,
                IsPublic = true
            },
            new Fragment("Quote", "An inspiring quote", "> \"The only way to do great work is to love what you do.\" - Steve Jobs", "https://picsum.photos/400/300?random=3")
            {
                TypeTag = "quote",
                AttachmentType = "image",
                VibeCount = 8,
                IsPublic = true
            }
        };

        // Basic pagination
        var skip = (page - 1) * pageSize;
        var pagedFragments = mockFragments.Skip(skip).Take(pageSize);

        return await Task.FromResult(pagedFragments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Fragment?>> GetFragment(Guid id)
    {
        // Prototype implementation - return mock data for any ID
        var mockFragment = new Fragment("Fragment Details", "Detailed view of a fragment", "# Fragment Details\n\nThis is a detailed view of the fragment.")
        {
            Id = id,
            TypeTag = "fragment",
            AttachmentType = "image",
            VibeCount = 3,
            IsPublic = true
        };

        return await Task.FromResult(mockFragment);
    }

    [HttpPost("{id}/vibe")]
    public async Task<IActionResult> AddVibe(Guid id)
    {
        // Prototype implementation - console logging only
        _logger.LogInformation("Vibed with fragment {FragmentId}!", id);
        
        return await Task.FromResult(Ok(new { message = $"Vibed with fragment {id}!", vibeCount = 1 }));
    }

    [HttpGet("types")]
    public async Task<IEnumerable<FragmentType>> GetFragmentTypes()
    {
        return await Task.FromResult(FragmentTypes.DefaultTypes);
    }
}