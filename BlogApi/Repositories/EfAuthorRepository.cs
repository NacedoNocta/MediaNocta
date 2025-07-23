using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repositories;

public class EfAuthorRepository : IAuthorRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfAuthorRepository> _logger;

    public EfAuthorRepository(AppDbContext context, ILogger<EfAuthorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<IAuthor>> GetAllAsync()
    {
        try
        {
            var authors = await _context.Authors.ToListAsync();
            return authors.Cast<IAuthor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors from database");
            throw;
        }
    }

    public async Task<IAuthor?> GetByIdAsync(Guid id)
    {
        try
        {
            var author = await _context.Authors.FindAsync(id);
            return author;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving author with ID {Id} from database", id);
            throw;
        }
    }

    public async Task<IAuthor> CreateAsync(IAuthor author)
    {
        try
        {
            var authorEntity = author as Author ?? throw new ArgumentException("Author must be of type Author", nameof(author));
            
            _context.Authors.Add(authorEntity);
            await _context.SaveChangesAsync();

            return authorEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author in database");
            throw;
        }
    }

    public async Task<IAuthor?> UpdateAsync(IAuthor author)
    {
        try
        {
            var authorEntity = author as Author ?? throw new ArgumentException("Author must be of type Author", nameof(author));
            
            var existingAuthor = await _context.Authors.FindAsync(authorEntity.Id);
            if (existingAuthor == null)
                return null;

            existingAuthor.Name = authorEntity.Name;
            existingAuthor.Biography = authorEntity.Biography;
            existingAuthor.ImageUrl = authorEntity.ImageUrl;

            await _context.SaveChangesAsync();
            return existingAuthor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author with ID {Id} in database", author.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author with ID {Id} from database", id);
            throw;
        }
    }

    public Task<IAuthor?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}