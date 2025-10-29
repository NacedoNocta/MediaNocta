using BlogLibrary;
using TechLibrary;
using DatabaseManager;
using DatabaseManager.DbContexts;
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
        // Get recent blogs
        var recentBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        // Get recent tech updates
        var recentTechUpdates = await _context.TechUpdates
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        // Combine and sort by creation date, then take the requested count
        var allActivities = new List<Activity>();
        allActivities.AddRange(recentBlogs.Cast<Activity>());
        allActivities.AddRange(recentTechUpdates.Cast<Activity>());

        return allActivities
            .OrderByDescending(a => a.CreatedAt)
            .Take(count);
    }

    public async Task<IEnumerable<Activity>> GetPinnedActivitiesAsync(int count = 2)
    {
        // Get featured/pinned blogs
        var pinnedBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .Where(b => b.Featured)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        // Get featured/pinned tech updates
        var pinnedTechUpdates = await _context.TechUpdates
            .Where(t => t.Featured)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        // Combine and sort by creation date, then take the requested count
        var allPinnedActivities = new List<Activity>();
        allPinnedActivities.AddRange(pinnedBlogs.Cast<Activity>());
        allPinnedActivities.AddRange(pinnedTechUpdates.Cast<Activity>());

        return allPinnedActivities
            .OrderByDescending(a => a.CreatedAt)
            .Take(count);
    }

    public async Task<IEnumerable<Activity>> GetRandomActivitiesAsync(int count = 1)
    {
        var totalBlogs = await _context.Blogs.CountAsync();
        var totalTechUpdates = await _context.TechUpdates.CountAsync();
        var totalActivities = totalBlogs + totalTechUpdates;
        
        if (totalActivities == 0)
        {
            return [];
        }

        var allActivities = new List<Activity>();
        
        // Get all blogs
        var allBlogs = await _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .ToListAsync();
        allActivities.AddRange(allBlogs.Cast<Activity>());

        // Get all tech updates
        var allTechUpdates = await _context.TechUpdates.ToListAsync();
        allActivities.AddRange(allTechUpdates.Cast<Activity>());

        // Return random selection
        return allActivities
            .OrderBy(a => Guid.NewGuid())
            .Take(count);
    }
}