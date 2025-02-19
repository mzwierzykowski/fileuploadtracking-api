namespace FileUploadTracking.NotificationService.Interfaces;
public interface IQueueService
{
    Task StartConsumingAsync(CancellationToken cancellationToken);
}
