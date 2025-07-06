using Microsoft.Extensions.DependencyInjection;

namespace DatabaseSeeder;

public class DatabaseSeederWorker : BackgroundService
{
    private readonly ILogger<DatabaseSeederWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public DatabaseSeederWorker(ILogger<DatabaseSeederWorker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("DatabaseSeeder worker starting...");
            _logger.LogInformation("THIS IS THE DATABASE SEEDER WORKER");
            
            using var scope = _serviceProvider.CreateScope();
            var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();

            await seedingService.SeedAllAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            Environment.ExitCode = 1;
        }
        finally
        {
            _logger.LogInformation("DatabaseSeeder worker completed. Stopping application.");
            _applicationLifetime.StopApplication();
        }
    }
}