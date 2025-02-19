using FileUploadTracking.NotificationService.Interfaces;

namespace FileUploadTracking.NotificationService;

public class Worker(ILogger<Worker> logger, IQueueService queueService) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IQueueService _queueService = queueService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueService.StartConsumingAsync(stoppingToken);
        _logger.LogInformation("Notification Worker started at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
