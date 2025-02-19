using FileUploadTracking.NotificationService.Interfaces;
using FileUploadTracking.NotificationService.Models;

namespace FileUploadTracking.NotificationService.EventHandlers;
internal class ProcessCompletedEventHandler(ILogger<ProcessCompletedEventHandler> logger, ICallbackService callbackService) : IProcessCompletedEventHandler
{
    private readonly ILogger<ProcessCompletedEventHandler> _logger = logger;
    private readonly ICallbackService _callbackService = callbackService;
    public async Task HandleAsync(ProcessCompletedEvent processEvent)
    {
        _logger.LogInformation("Handling ProcessCompletedEvent for TrackingId: {TrackingId}", processEvent.TrackingId);
        var customerCallbackRequest = new CustomerCallbackRequest()
        {
            Message = $"File upload process with id {processEvent.TrackingId} completed successfully"
        };

        await _callbackService.SendCallbackAsync(processEvent.CallbackUrl, customerCallbackRequest);
    }
}
