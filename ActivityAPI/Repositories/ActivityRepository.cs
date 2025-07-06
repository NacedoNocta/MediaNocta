using BlogLibrary;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace ActivityAPI.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _context;

    public ActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activity>> GetRecentActivitiesAsync(int count = 5)
    {
        var recentBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .ToListAsync();

        return recentBlogs.Cast<Activity>();
    }

    public async Task<IEnumerable<Activity>> GetPinnedActivitiesAsync(int count = 2)
    {
        // For now, return the most recent blogs as "pinned"
        // This can be enhanced later with a proper pinned flag in the database
        var pinnedBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .ToListAsync();

        return pinnedBlogs.Cast<Activity>();
    }

    public async Task<IEnumerable<Activity>> GetRandomActivitiesAsync(int count = 1)
    {
        var totalBlogs = await _context.Blogs.CountAsync();
        if (totalBlogs == 0)
        {
            return [];
        }

        var randomSkip = new Random().Next(0, Math.Max(1, totalBlogs - count));
        var randomBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .Skip(randomSkip)
            .Take(count)
            .ToListAsync();

        return randomBlogs.Cast<Activity>();
    }
}