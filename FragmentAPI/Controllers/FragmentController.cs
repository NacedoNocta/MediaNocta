using FragmentLibrary;
using Microsoft.AspNetCore.Mvc;
using DatabaseManager;
using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FragmentAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FragmentController : ControllerBase
{
    private readonly ILogger<FragmentController> _logger;
    private readonly AppDbContext _context;

    public FragmentController(ILogger<FragmentController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Fragment>> GetFragments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var skip = (page - 1) * pageSize;
            
            var fragments = await _context.Fragments
                .Where(f => f.IsPublic && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} fragments for page {Page}", fragments.Count, page);
            return fragments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fragments for page {Page}", page);
            return new List<Fragment>();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Fragment?>> GetFragment(Guid id)
    {
        try
        {
            var fragment = await _context.Fragments
                .Where(f => f.Id == id && f.IsPublic && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved fragment {FragmentId}: {Title}", id, fragment.Title);
            return fragment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while retrieving the fragment");
        }
    }

    [HttpPost("{id}/vibe")]
    public async Task<IActionResult> AddVibe(Guid id)
    {
        try
        {
            var fragment = await _context.Fragments
                .Where(f => f.Id == id && f.IsPublic && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for vibe", id);
                return NotFound();
            }

            fragment.VibeCount++;
            fragment.LastVibeDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vibed with fragment {FragmentId}: {Title}! New count: {VibeCount}", 
                id, fragment.Title, fragment.VibeCount);
                
            return Ok(new { message = $"Vibed with fragment {fragment.Title}!", vibeCount = fragment.VibeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vibe to fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while adding the vibe");
        }
    }

    [HttpGet("types")]
    public async Task<IEnumerable<FragmentType>> GetFragmentTypes()
    {
        try
        {
            var types = await _context.FragmentTypes
                .OrderBy(t => t.Name)
                .ToListAsync();

            // If no types in database, return default types
            if (!types.Any())
            {
                _logger.LogInformation("No fragment types found in database, returning default types");
                return FragmentTypes.DefaultTypes;
            }

            _logger.LogInformation("Retrieved {Count} fragment types from database", types.Count);
            return types;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fragment types from database, falling back to defaults");
            return FragmentTypes.DefaultTypes;
        }
    }

    [HttpGet("search")]
    public async Task<IEnumerable<Fragment>> SearchFragments([FromQuery] string query, [FromQuery] string? typeTag = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Fragment>();
            }

            var fragmentsQuery = _context.Fragments
                .Where(f => f.IsPublic && !f.IsDeleted);

            // Apply text search
            fragmentsQuery = fragmentsQuery.Where(f =>
                EF.Functions.ILike(f.Title, $"%{query}%") ||
                EF.Functions.ILike(f.Summary, $"%{query}%") ||
                EF.Functions.ILike(f.Content, $"%{query}%"));

            // Apply type filter if specified
            if (!string.IsNullOrEmpty(typeTag))
            {
                fragmentsQuery = fragmentsQuery.Where(f => f.TypeTag == typeTag);
            }

            var results = await fragmentsQuery
                .OrderByDescending(f => f.CreatedAt)
                .Take(50) // Limit search results
                .ToListAsync();

            _logger.LogInformation("Search for '{Query}' with type '{TypeTag}' returned {Count} results", 
                query, typeTag ?? "all", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching fragments with query '{Query}' and type '{TypeTag}'", query, typeTag);
            return new List<Fragment>();
        }
    }

    [HttpPost("{id}/unvibe")]
    public async Task<IActionResult> RemoveVibe(Guid id)
    {
        try
        {
            var fragment = await _context.Fragments
                .Where(f => f.Id == id && f.IsPublic && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for unvibe", id);
                return NotFound();
            }

            if (fragment.VibeCount > 0)
            {
                fragment.VibeCount--;
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Removed vibe from fragment {FragmentId}: {Title}! New count: {VibeCount}", 
                id, fragment.Title, fragment.VibeCount);
                
            return Ok(new { message = $"Removed vibe from fragment {fragment.Title}!", vibeCount = fragment.VibeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing vibe from fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while removing the vibe");
        }
    }

    [HttpGet("by-type/{typeTag}")]
    public async Task<IEnumerable<Fragment>> GetFragmentsByType(string typeTag)
    {
        try
        {
            var fragments = await _context.Fragments
                .Where(f => f.TypeTag == typeTag && f.IsPublic && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} fragments for type '{TypeTag}'", fragments.Count, typeTag);
            return fragments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fragments for type '{TypeTag}'", typeTag);
            return new List<Fragment>();
        }
    }
}