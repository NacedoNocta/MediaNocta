using CacheLibrary;
using FragmentLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
    private readonly IOutputCacheStore _cacheStore;

    public FragmentController(ILogger<FragmentController> logger, AppDbContext context, IOutputCacheStore cacheStore)
    {
        _logger = logger;
        _context = context;
        _cacheStore = cacheStore;
    }

    [HttpGet]
    [OutputCache(PolicyName = "FragmentList")]
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
    [OutputCache(PolicyName = "FragmentDetail")]
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
    public async Task<IActionResult> AddVibe(Guid id, CancellationToken ct)
    {
        try
        {
            var fragment = await _context.Fragments
                .Where(f => f.Id == id && f.IsPublic && !f.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for vibe", id);
                return NotFound();
            }

            fragment.VibeCount++;
            fragment.LastVibeDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Vibed with fragment {FragmentId}: {Title}! New count: {VibeCount}",
                id, fragment.Title, fragment.VibeCount);

            await EvictAllAsync(ct);
            return Ok(new { message = $"Vibed with fragment {fragment.Title}!", vibeCount = fragment.VibeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vibe to fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while adding the vibe");
        }
    }

    [HttpGet("types")]
    [OutputCache(PolicyName = "FragmentTypes")]
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
    [OutputCache(PolicyName = "FragmentSearch")]
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
    public async Task<IActionResult> RemoveVibe(Guid id, CancellationToken ct)
    {
        try
        {
            var fragment = await _context.Fragments
                .Where(f => f.Id == id && f.IsPublic && !f.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for unvibe", id);
                return NotFound();
            }

            if (fragment.VibeCount > 0)
            {
                fragment.VibeCount--;
                await _context.SaveChangesAsync(ct);
            }

            _logger.LogInformation("Removed vibe from fragment {FragmentId}: {Title}! New count: {VibeCount}",
                id, fragment.Title, fragment.VibeCount);

            await EvictAllAsync(ct);
            return Ok(new { message = $"Removed vibe from fragment {fragment.Title}!", vibeCount = fragment.VibeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing vibe from fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while removing the vibe");
        }
    }

    [HttpGet("by-type/{typeTag}")]
    [OutputCache(PolicyName = "FragmentByType")]
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

    [HttpPost]
    public async Task<ActionResult<Fragment>> CreateFragment([FromBody] Fragment fragment, CancellationToken ct)
    {
        try
        {
            if (fragment == null)
            {
                _logger.LogWarning("Attempted to create a null fragment");
                return BadRequest("Fragment cannot be null");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(fragment.Title?.English))
            {
                _logger.LogWarning("Attempted to create fragment without a title");
                return BadRequest("Fragment title is required");
            }

            // Ensure the fragment has a new ID
            var newFragment = new Fragment
            {
                Title = fragment.Title,
                Summary = fragment.Summary,
                Content = fragment.Content,
                ImageUrl = fragment.ImageUrl,
                Links = fragment.Links ?? new List<string>(),
                Featured = fragment.Featured,
                TypeTag = fragment.TypeTag,
                AttachmentUrl = fragment.AttachmentUrl,
                AttachmentType = fragment.AttachmentType,
                VibeCount = fragment.VibeCount,
                IsPublic = fragment.IsPublic,
                IsDeleted = false,
                Meta = fragment.Meta,
                Tags = fragment.Tags,
                LastVibeDate = fragment.LastVibeDate
            };

            _context.Fragments.Add(newFragment);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Created new fragment with ID {FragmentId}: {Title}",
                newFragment.Id, newFragment.Title?.GetText());

            await EvictAllAsync(ct);
            return CreatedAtAction(nameof(GetFragment), new { id = newFragment.Id }, newFragment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fragment");
            return StatusCode(500, "An error occurred while creating the fragment");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Fragment>> UpdateFragment(Guid id, [FromBody] Fragment fragment, CancellationToken ct)
    {
        try
        {
            if (fragment == null)
            {
                _logger.LogWarning("Attempted to update fragment {FragmentId} with null data", id);
                return BadRequest("Fragment cannot be null");
            }

            var existingFragment = await _context.Fragments
                .FirstOrDefaultAsync(f => f.Id == id, ct);

            if (existingFragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for update", id);
                return NotFound($"Fragment with ID {id} not found");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(fragment.Title?.English))
            {
                _logger.LogWarning("Attempted to update fragment {FragmentId} without a title", id);
                return BadRequest("Fragment title is required");
            }

            // Update all fields
            existingFragment.Title = fragment.Title;
            existingFragment.Summary = fragment.Summary;
            existingFragment.Content = fragment.Content;
            existingFragment.ImageUrl = fragment.ImageUrl;
            existingFragment.Links = fragment.Links ?? new List<string>();
            existingFragment.Featured = fragment.Featured;
            existingFragment.TypeTag = fragment.TypeTag;
            existingFragment.AttachmentUrl = fragment.AttachmentUrl;
            existingFragment.AttachmentType = fragment.AttachmentType;
            existingFragment.VibeCount = fragment.VibeCount;
            existingFragment.IsPublic = fragment.IsPublic;
            existingFragment.IsDeleted = fragment.IsDeleted;
            existingFragment.Meta = fragment.Meta;
            existingFragment.Tags = fragment.Tags;
            existingFragment.LastVibeDate = fragment.LastVibeDate;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Updated fragment {FragmentId}: {Title}",
                id, existingFragment.Title?.GetText());

            await EvictAllAsync(ct);
            return Ok(existingFragment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while updating the fragment");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFragment(Guid id, CancellationToken ct)
    {
        try
        {
            var fragment = await _context.Fragments
                .FirstOrDefaultAsync(f => f.Id == id, ct);

            if (fragment == null)
            {
                _logger.LogWarning("Fragment with ID {FragmentId} not found for deletion", id);
                return NotFound($"Fragment with ID {id} not found");
            }

            _context.Fragments.Remove(fragment);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Deleted fragment {FragmentId}: {Title}",
                id, fragment.Title?.GetText());

            await EvictAllAsync(ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fragment {FragmentId}", id);
            return StatusCode(500, "An error occurred while deleting the fragment");
        }
    }

    private async Task EvictAllAsync(CancellationToken ct)
    {
        await _cacheStore.EvictByTagAsync(CacheTags.FragmentApi.List, ct);
        await _cacheStore.EvictByTagAsync(CacheTags.FragmentApi.DetailAll, ct);
        await _cacheStore.EvictByTagAsync(CacheTags.FragmentApi.Search, ct);
        await _cacheStore.EvictByTagAsync(CacheTags.FragmentApi.ByType, ct);
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-list");
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-detail");
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-search");
        CacheTelemetry.RecordEviction(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-by-type");
    }
}