using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace ActivityAPI.Controllers;

[ApiController]
[Route("")]
public class ActivityController : ControllerBase
{
    private readonly ILogger<ActivityController> _logger;
    private readonly IActivityRepository _activityRepository;

    public ActivityController(ILogger<ActivityController> logger, IActivityRepository activityRepository)
    {
        _logger = logger;
        _activityRepository = activityRepository;
    }

    [HttpGet(Name = "recent")]
    [Route("recent")]
    public async Task<IEnumerable<Activity>> GetRecent()
    {
        return await _activityRepository.GetRecentActivitiesAsync(5);
    }

    [HttpGet(Name = "pinned")]
    [Route("pinned")]
    public async Task<IEnumerable<Activity>> GetPinned()
    {
        return await _activityRepository.GetPinnedActivitiesAsync(2);
    }

    [HttpGet(Name = "random")]
    [Route("simpleBlogs")]
    public async Task<IEnumerable<Activity>> GetRandom()
    {
        return await _activityRepository.GetRandomActivitiesAsync(1);
    }
}

