using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
    [OutputCache(PolicyName = "ActivityRecent")]
    public async Task<IEnumerable<Activity>> GetRecent()
    {
        return await _activityRepository.GetRecentActivitiesAsync(9);
    }

    [HttpGet(Name = "pinned")]
    [Route("pinned")]
    [OutputCache(PolicyName = "ActivityPinned")]
    public async Task<IEnumerable<Activity>> GetPinned()
    {
        return await _activityRepository.GetPinnedActivitiesAsync(2);
    }

    [HttpGet(Name = "random")]
    [Route("simpleBlogs")]
    [OutputCache(PolicyName = "ActivityRandom")]
    public async Task<IEnumerable<Activity>> GetRandom()
    {
        return await _activityRepository.GetRandomActivitiesAsync(1);
    }
}
