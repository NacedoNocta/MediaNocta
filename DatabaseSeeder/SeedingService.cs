using DatabaseManager;
using FragmentLibrary;
using BlogLibrary;
using BlogLibrary.Interfaces;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using DatabaseSeeder.SeedData;

namespace DatabaseSeeder;

public class SeedingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SeedingService> _logger;

    public SeedingService(AppDbContext context, ILogger<SeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAllAsync()
    {
        _logger.LogInformation("Starting database seeding process...");
        
        await SeedFragmentTypesAsync();
        await SeedFragmentsAsync();
        await SeedBlogDataAsync();
        
        _logger.LogInformation("Database seeding process completed.");
    }

    public async Task SeedFragmentTypesAsync()
    {
        try
        {
            var existingTypes = await _context.FragmentTypes.CountAsync();
            if (existingTypes > 0)
            {
                _logger.LogInformation("Fragment types already exist in database, skipping seed");
                return;
            }

            var defaultTypes = FragmentTypes.DefaultTypes.ToList();
            await _context.FragmentTypes.AddRangeAsync(defaultTypes);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} fragment types", defaultTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding fragment types");
        }
    }

    public async Task SeedFragmentsAsync()
    {
        try
        {
            var existingFragments = await _context.Fragments.CountAsync();
            if (existingFragments > 0)
            {
                _logger.LogInformation("Fragments already exist in database, skipping seed");
                return;
            }

            var sampleFragments = FragmentSeedData.GetSampleFragments();
            await _context.Fragments.AddRangeAsync(sampleFragments);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} sample fragments", sampleFragments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding fragments");
        }
    }

    public async Task SeedBlogDataAsync()
    {
        try
        {
            var existingBlogs = await _context.Blogs.CountAsync();
            if (existingBlogs > 0)
            {
                _logger.LogInformation("Blogs already exist in database, skipping seed");
                return;
            }

            var sampleTags = BlogSeedData.GetSampleTags();
            await _context.Tags.AddRangeAsync(sampleTags);
            await _context.SaveChangesAsync();

            var sampleAuthors = BlogSeedData.GetSampleAuthors();
            await _context.Authors.AddRangeAsync(sampleAuthors);
            await _context.SaveChangesAsync();

            var sampleBlogs = BlogSeedData.GetSampleBlogs(sampleAuthors, sampleTags);
            await _context.Blogs.AddRangeAsync(sampleBlogs);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} sample blogs with {TagCount} tags and {AuthorCount} authors", 
                sampleBlogs.Count, sampleTags.Count, sampleAuthors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding blog data");
        }
    }
}