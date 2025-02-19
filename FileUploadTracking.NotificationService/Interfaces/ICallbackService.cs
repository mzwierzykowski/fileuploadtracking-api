using FileUploadTracking.NotificationService.Models;

namespace FileUploadTracking.NotificationService.Interfaces;
public interface ICallbackService
{
    Task SendCallbackAsync(string callbackUrl, CustomerCallbackRequest customerCallbackRequest);
}
