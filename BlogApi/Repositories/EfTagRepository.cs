using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repositories;

public class EfTagRepository : ITagRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfTagRepository> _logger;

    public EfTagRepository(AppDbContext context, ILogger<EfTagRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ITag>> GetAllAsync()
    {
        try
        {
            var tags = await _context.Tags.ToListAsync();
            return tags.Cast<ITag>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tags from database");
            throw;
        }
    }

    public async Task<ITag?> GetByIdAsync(Guid id)
    {
        try
        {
            var tag = await _context.Tags.FindAsync(id);
            return tag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tag with ID {Id} from database", id);
            throw;
        }
    }

    public async Task<ITag> CreateAsync(ITag tag)
    {
        try
        {
            var tagEntity = tag as Tag ?? throw new ArgumentException("Tag must be of type Tag", nameof(tag));
            
            _context.Tags.Add(tagEntity);
            await _context.SaveChangesAsync();

            return tagEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag in database");
            throw;
        }
    }

    public async Task<ITag?> UpdateAsync(ITag tag)
    {
        try
        {
            var tagEntity = tag as Tag ?? throw new ArgumentException("Tag must be of type Tag", nameof(tag));
            
            var existingTag = await _context.Tags.FindAsync(tagEntity.Id);
            if (existingTag == null)
                return null;

            existingTag.Name = tagEntity.Name;

            await _context.SaveChangesAsync();
            return existingTag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag with ID {Id} in database", tag.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return false;

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag with ID {Id} from database", id);
            throw;
        }
    }

    public Task<ITag?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ITag>> GetTagsByBlogIdAsync(Guid blogId)
    {
        throw new NotImplementedException();
    }
}