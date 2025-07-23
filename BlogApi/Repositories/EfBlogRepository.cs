using BlogApi.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repositories;

public class EfBlogRepository : IBlogRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfBlogRepository> _logger;

    public EfBlogRepository(AppDbContext context, ILogger<EfBlogRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<IBlog>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var skip = (page - 1) * pageSize;
            var blogs = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return blogs.Cast<IBlog>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blogs from database (page: {Page}, pageSize: {PageSize})", page, pageSize);
            throw;
        }
    }

    public async Task<IBlog?> GetByIdAsync(Guid id)
    {
        try
        {
            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == id);

            return blog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blog with ID {Id} from database", id);
            throw;
        }
    }

    public async Task<uint> GetCountAsync()
    {
        try
        {
            var count = await _context.Blogs.CountAsync();
            return (uint)count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blog count from database");
            throw;
        }
    }

    public async Task<IBlog> CreateAsync(IBlog blog)
    {
        try
        {
            var blogEntity = blog as Blog ?? throw new ArgumentException("Blog must be of type Blog", nameof(blog));
            
            // Ensure the author exists or create it
            var existingAuthor = await _context.Authors.FindAsync(blogEntity.Author.Id);
            if (existingAuthor == null)
            {
                _context.Authors.Add(blogEntity.Author);
            }
            else
            {
                blogEntity.Author = existingAuthor;
            }

            // Handle tags - attach existing ones, create new ones
            var existingTags = new List<Tag>();
            foreach (var tag in blogEntity.Tags)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                if (existingTag != null)
                {
                    existingTags.Add(existingTag);
                }
                else
                {
                    _context.Tags.Add(tag);
                    existingTags.Add(tag);
                }
            }
            blogEntity.Tags = existingTags;

            _context.Blogs.Add(blogEntity);
            await _context.SaveChangesAsync();

            return blogEntity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog in database");
            throw;
        }
    }

    public async Task<IBlog?> UpdateAsync(IBlog blog)
    {
        try
        {
            var blogEntity = blog as Blog ?? throw new ArgumentException("Blog must be of type Blog", nameof(blog));
            
            var existingBlog = await _context.Blogs
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == blogEntity.Id);

            if (existingBlog == null)
                return null;

            // Update blog properties
            existingBlog.Title = blogEntity.Title;
            existingBlog.Summary = blogEntity.Summary;
            existingBlog.Content = blogEntity.Content;
            existingBlog.ImageUrl = blogEntity.ImageUrl;
            existingBlog.Links = blogEntity.Links;

            // Update tags
            existingBlog.Tags.Clear();
            foreach (var tag in blogEntity.Tags)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                if (existingTag != null)
                {
                    existingBlog.Tags.Add(existingTag);
                }
                else
                {
                    _context.Tags.Add(tag);
                    existingBlog.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
            return existingBlog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog with ID {Id} in database", blog.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return false;

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog with ID {Id} from database", id);
            throw;
        }
    }

    public async Task<IEnumerable<IBlog>> GetByAuthorIdAsync(Guid authorId)
    {
        try
        {
            var blogs = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .Where(b => b.Author.Id == authorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blogs.Cast<IBlog>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blogs for author ID {AuthorId} from database", authorId);
            throw;
        }
    }

    public async Task<IEnumerable<IBlog>> GetByTagIdAsync(Guid tagId)
    {
        try
        {
            var blogs = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .Where(b => b.Tags.Any(t => t.Id == tagId))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blogs.Cast<IBlog>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blogs for tag ID {TagId} from database", tagId);
            throw;
        }
    }
}