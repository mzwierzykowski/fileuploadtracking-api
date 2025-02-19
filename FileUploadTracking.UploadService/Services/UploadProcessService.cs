using FileUploadTracking.Events;
using FileUploadTracking.UploadService.Interfaces;
using FileUploadTracking.UploadService.Models;

namespace FileUploadTracking.UploadService.Services;

public class UploadProcessService(ILogger<UploadProcessService> logger, IQueueService queueService, IUploadProcessRepository uploadProcessRepository) : IUploadProcessService
{
    private readonly ILogger<UploadProcessService> _logger = logger;
    private readonly IUploadProcessRepository _uploadProcessRepository = uploadProcessRepository;
    private readonly IQueueService _queueService = queueService;

    public UploadProcess CreateUploadProcess(CreateProcessRequest request)
    {
        var trackingId = Guid.NewGuid();
        _logger.LogInformation("[TrackingId: {TrackingId}] Received CreateUploadProcess request: {Request}", trackingId, request);
        var uploadProcess = new UploadProcess
        {
            TrackingId = trackingId,
            UserId = request.UserId,
            CustomerId = request.CustomerId,
            CallbackUrl = request.CallbackUrl,
            Files = [],
            CreatedDateTime = DateTime.UtcNow,
            State = ProcessState.New,
        };
        _uploadProcessRepository.AddProcess(uploadProcess);
        return uploadProcess;
    }

    public UploadProcess? GetUploadProcess(Guid trackingId)
    {
        return _uploadProcessRepository.GetProcess(trackingId);
    }

    public async Task UploadFile(Guid trackingId, UploadFileRequest request)
    {
        _logger.LogInformation("[TrackingId: {TrackingId}] Received UploadFile request: {Request}", trackingId, request);
        var process = _uploadProcessRepository.GetProcess(trackingId) ?? throw new ArgumentNullException(nameof(trackingId));
        process.Files.Add(request.FileName);
        CalculateProcessState(process);
        _logger.LogInformation("[TrackingId: {TrackingId}] Calculated process state: {State}", trackingId, process.State);
        _uploadProcessRepository.UpdateProcess(process);
        if (process.State == ProcessState.Completed)
        {
            await SendProcessCompletedEvent(process);
        }

    }

    private async Task SendProcessCompletedEvent(UploadProcess uploadProcess)
    {
        var processCompletedEvent = new ProcessCompletedEvent()
        {
            TrackingId = uploadProcess.TrackingId,
            UserId = uploadProcess.UserId,
            CustomerId = uploadProcess.CustomerId,
            CallbackUrl = uploadProcess.CallbackUrl,
            Files = uploadProcess.Files,
            CompletedAt = DateTime.UtcNow,
        };
        await _queueService.SendProcessCompletedNotificationAsync(processCompletedEvent);
    }

    private static void CalculateProcessState(UploadProcess uploadProcess)
    {
        if (uploadProcess.Files.Count > 0)
            uploadProcess.State = ProcessState.InProgress;
        if (uploadProcess.Files.Count == 3)
            uploadProcess.State = ProcessState.Completed;
    }
}
