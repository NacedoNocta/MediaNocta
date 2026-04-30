using TechAPI.Interfaces;
using TechLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace TechAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class TechUpdateController : ControllerBase
    {
        private readonly ILogger<TechUpdateController> _logger;
        private readonly ITechUpdateService _techUpdateService;

        public TechUpdateController(ILogger<TechUpdateController> logger, ITechUpdateService techUpdateService)
        {
            _logger = logger;
            _techUpdateService = techUpdateService;
        }

        [Route("tech-updates")]
        [HttpGet(Name = "GetTechUpdates")]
        [OutputCache(PolicyName = "TechUpdates")]
        public async Task<ActionResult<IEnumerable<ITechUpdate>>> GetTechUpdates(
            [FromQuery] string? projectId = null, 
            [FromQuery] uint page = 1, 
            [FromQuery] uint pageSize = 10)
        {
            try
            {
                IEnumerable<ITechUpdate> updates;
                
                if (!string.IsNullOrEmpty(projectId))
                {
                    updates = await _techUpdateService.GetTechUpdatesByProjectIdAsync(projectId, page, pageSize);
                }
                else
                {
                    updates = await _techUpdateService.GetTechUpdatesAsync(page, pageSize);
                }

                return Ok(updates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates for project {ProjectId}, page {Page}", projectId, page);
                return StatusCode(500, "An error occurred while retrieving tech updates");
            }
        }

        [Route("tech-updates/count")]
        [HttpGet(Name = "GetTechUpdatesCount")]
        [OutputCache(PolicyName = "TechUpdatesCount")]
        public async Task<ActionResult<uint>> GetTechUpdatesCount([FromQuery] string? projectId = null)
        {
            try
            {
                uint count;
                
                if (!string.IsNullOrEmpty(projectId))
                {
                    count = await _techUpdateService.GetTechUpdatesCountByProjectIdAsync(projectId);
                }
                else
                {
                    count = await _techUpdateService.GetTechUpdatesCountAsync();
                }

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates count for project {ProjectId}", projectId);
                return StatusCode(500, "An error occurred while retrieving tech updates count");
            }
        }

        [Route("tech-updates/{id:guid}")]
        [HttpGet(Name = "GetTechUpdateById")]
        [OutputCache(PolicyName = "TechUpdateDetail")]
        public async Task<ActionResult<ITechUpdate>> GetTechUpdateById(Guid id)
        {
            try
            {
                var update = await _techUpdateService.GetTechUpdateByIdAsync(id);
                
                if (update == null)
                {
                    _logger.LogWarning("Tech update with ID {Id} not found", id);
                    return NotFound($"Tech update with ID {id} not found");
                }

                return Ok(update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech update with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the tech update");
            }
        }

        [Route("tech-projects/last-activity")]
        [HttpGet(Name = "GetLastActivityDates")]
        [OutputCache(PolicyName = "TechProjectActivity")]
        public async Task<ActionResult<Dictionary<string, DateTime?>>> GetLastActivityDates()
        {
            try
            {
                var lastActivityDates = await _techUpdateService.GetLastActivityDatesAsync();
                return Ok(lastActivityDates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last activity dates");
                return StatusCode(500, "An error occurred while retrieving last activity dates");
            }
        }
    }
}