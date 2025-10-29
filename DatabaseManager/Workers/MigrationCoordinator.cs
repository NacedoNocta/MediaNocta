namespace DatabaseManager.Workers;

/// <summary>
/// Coordinates multiple migration workers to ensure all complete before stopping the application.
/// </summary>
public class MigrationCoordinator
{
    private int _completedWorkers = 0;
    private readonly int _totalWorkers;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<MigrationCoordinator> _logger;
    private readonly object _lock = new object();
    private int _exitCode = 0;

    public MigrationCoordinator(
        int totalWorkers,
        IHostApplicationLifetime applicationLifetime,
        ILogger<MigrationCoordinator> logger)
    {
        _totalWorkers = totalWorkers;
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    public void NotifyCompletion(string workerName, bool success)
    {
        lock (_lock)
        {
            _completedWorkers++;

            if (!success)
            {
                _exitCode = 1;
                _logger.LogError("{WorkerName} completed with errors", workerName);
            }
            else
            {
                _logger.LogInformation("{WorkerName} completed successfully", workerName);
            }

            _logger.LogInformation("Migration progress: {Completed}/{Total} workers completed",
                _completedWorkers, _totalWorkers);

            if (_completedWorkers >= _totalWorkers)
            {
                _logger.LogInformation("All migration workers completed. Stopping application...");
                Environment.ExitCode = _exitCode;
                _applicationLifetime.StopApplication();
            }
        }
    }
}
