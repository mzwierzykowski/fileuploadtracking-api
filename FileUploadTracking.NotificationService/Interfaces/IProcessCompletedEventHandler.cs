using FileUploadTracking.NotificationService.Models;

namespace FileUploadTracking.NotificationService.Interfaces;
public interface IProcessCompletedEventHandler
{
    Task HandleAsync(ProcessCompletedEvent processEvent);
}
