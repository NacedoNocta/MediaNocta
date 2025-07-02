using DatabaseManager;
using Microsoft.EntityFrameworkCore;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Database Migration Worker...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            _logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync(stoppingToken);

            _logger.LogInformation("Database migrations completed successfully.");
        }
    }
}
