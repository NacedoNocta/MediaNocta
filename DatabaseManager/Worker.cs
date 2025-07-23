using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager;

public class DatabaseMigrationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationWorker> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public DatabaseMigrationWorker(IServiceProvider serviceProvider, ILogger<DatabaseMigrationWorker> logger, IHostApplicationLifetime applicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Database Migration Worker...");
            _logger.LogInformation("THIS IS THE DATABASE MANAGER WORKER");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                _logger.LogInformation("Applying database migrations...");
                await dbContext.Database.MigrateAsync(stoppingToken);

                _logger.LogInformation("Database migrations completed successfully.");
            }

            _logger.LogInformation("Migration work completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database migration");
            Environment.ExitCode = 1;
        }
        finally
        {
            // Stop the application gracefully
            _applicationLifetime.StopApplication();
        }
    }
}
