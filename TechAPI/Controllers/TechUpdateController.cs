using TechAPI.Interfaces;
using TechLibrary.Interfaces;
using TechLibrary;
using SharedLibrary;
using Microsoft.AspNetCore.Mvc;

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

        [Route("tech-updates")]
        [HttpPost(Name = "CreateTechUpdate")]
        public async Task<ActionResult<ITechUpdate>> CreateTechUpdate([FromBody] CreateTechUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var techUpdate = new TechUpdate(
                    request.Title,
                    request.Summary,
                    request.Content,
                    request.ProjectId,
                    request.UpdateType ?? "",
                    request.ImageUrl,
                    request.Links
                );

                var created = await _techUpdateService.CreateTechUpdateAsync(techUpdate);
                return CreatedAtAction(nameof(GetTechUpdateById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tech update");
                return StatusCode(500, "An error occurred while creating the tech update");
            }
        }

        [Route("tech-updates/{id:guid}")]
        [HttpPut(Name = "UpdateTechUpdate")]
        public async Task<ActionResult<ITechUpdate>> UpdateTechUpdate(Guid id, [FromBody] UpdateTechUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUpdate = await _techUpdateService.GetTechUpdateByIdAsync(id);
                if (existingUpdate == null)
                {
                    return NotFound($"Tech update with ID {id} not found");
                }

                var techUpdate = new TechUpdate(
                    request.Title,
                    request.Summary,
                    request.Content,
                    request.ProjectId,
                    request.UpdateType ?? "",
                    request.ImageUrl,
                    request.Links
                ) { Id = id };

                var updated = await _techUpdateService.UpdateTechUpdateAsync(techUpdate);
                if (updated == null)
                {
                    return NotFound($"Tech update with ID {id} not found");
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tech update with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the tech update");
            }
        }

        [Route("tech-updates/{id:guid}")]
        [HttpDelete(Name = "DeleteTechUpdate")]
        public async Task<ActionResult> DeleteTechUpdate(Guid id)
        {
            try
            {
                var deleted = await _techUpdateService.DeleteTechUpdateAsync(id);
                if (!deleted)
                {
                    return NotFound($"Tech update with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tech update with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the tech update");
            }
        }
    }

    public class CreateTechUpdateRequest
    {
        public required string Title { get; set; }
        public required string Summary { get; set; }
        public required string Content { get; set; }
        public required string ProjectId { get; set; }
        public string? UpdateType { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? Links { get; set; }
    }

    public class UpdateTechUpdateRequest
    {
        public required string Title { get; set; }
        public required string Summary { get; set; }
        public required string Content { get; set; }
        public required string ProjectId { get; set; }
        public string? UpdateType { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? Links { get; set; }
    }
}