using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Workers;

/// <summary>
/// Background service that applies migrations for the AuthDbContext (websiteDatabase).
/// Runs separately from the main content database migrations.
/// </summary>
public class AuthMigrationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthMigrationWorker> _logger;
    private readonly MigrationCoordinator _coordinator;

    public AuthMigrationWorker(
        IServiceProvider serviceProvider,
        ILogger<AuthMigrationWorker> logger,
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
            _logger.LogInformation("Starting Auth Database Migration Worker...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                _logger.LogInformation("Applying auth database migrations...");
                await authDbContext.Database.MigrateAsync(stoppingToken);

                _logger.LogInformation("Auth database migrations completed successfully.");
            }

            success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during auth database migration");
            success = false;
        }
        finally
        {
            // Notify coordinator that this worker completed
            _coordinator.NotifyCompletion("AuthMigrationWorker", success);
        }
    }
}
