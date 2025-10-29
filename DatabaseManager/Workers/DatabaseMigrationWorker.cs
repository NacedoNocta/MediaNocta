using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Workers;

/// <summary>
/// Background service that applies migrations for the AppDbContext (mainDatabase).
/// Content entities only (blogs, fragments, etc.). Auth entities are in AuthMigrationWorker.
/// </summary>
public class DatabaseMigrationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationWorker> _logger;
    private readonly MigrationCoordinator _coordinator;

    public DatabaseMigrationWorker(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationWorker> logger,
        MigrationCoordinator coordinator)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _coordinator = coordinator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool success = false;
        try
        {
            _logger.LogInformation("Starting Content Database Migration Worker...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                _logger.LogInformation("Applying content database migrations...");
                await dbContext.Database.MigrateAsync(stoppingToken);

                _logger.LogInformation("Content database migrations completed successfully.");
            }

            success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during content database migration");
            success = false;
        }
        finally
        {
            // Notify coordinator that this worker completed
            _coordinator.NotifyCompletion("ContentMigrationWorker", success);
        }
    }
}
