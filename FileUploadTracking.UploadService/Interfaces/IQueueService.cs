using FileUploadTracking.Events;

namespace FileUploadTracking.UploadService.Interfaces;

public interface IQueueService
{
    Task SendProcessCompletedNotificationAsync(ProcessCompletedEvent processCompletedEvent);
}
