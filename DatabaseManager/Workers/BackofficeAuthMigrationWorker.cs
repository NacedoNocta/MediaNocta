using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DatabaseManager.Workers;

/// <summary>
/// Background service that applies migrations for the AuthDbContext (backofficeDatabase).
/// Runs separately from the website auth database migrations.
/// </summary>
public class BackofficeAuthMigrationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackofficeAuthMigrationWorker> _logger;
    private readonly MigrationCoordinator _coordinator;

    public BackofficeAuthMigrationWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<BackofficeAuthMigrationWorker> logger,
        MigrationCoordinator coordinator)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
        _coordinator = coordinator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool success = false;
        try
        {
            _logger.LogInformation("Starting Backoffice Auth Database Migration Worker...");

            // Get the connection string for backofficeDatabase
            var connectionString = _configuration.GetConnectionString("backofficeDatabase");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string for backofficeDatabase not found");
                success = false;
            }
            else
            {
                // Create a separate DbContext instance with the backoffice connection string
                var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
                optionsBuilder.UseNpgsql(connectionString);

                using (var authDbContext = new AuthDbContext(optionsBuilder.Options))
                {
                    _logger.LogInformation("Applying backoffice auth database migrations...");
                    await authDbContext.Database.MigrateAsync(stoppingToken);

                    _logger.LogInformation("Backoffice auth database migrations completed successfully.");
                }

                success = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during backoffice auth database migration");
            success = false;
        }
        finally
        {
            // Notify coordinator that this worker completed
            _coordinator.NotifyCompletion("BackofficeAuthMigrationWorker", success);
        }
    }
}
