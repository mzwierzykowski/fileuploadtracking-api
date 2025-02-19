using FileUploadTracking.UploadService.Interfaces;
using FileUploadTracking.UploadService.Models;
using System.Collections.Concurrent;

namespace FileUploadTracking.UploadService.Repositories;

public class InMemoryUploadProcessRepository : IUploadProcessRepository
{
    private readonly ConcurrentDictionary<Guid, UploadProcess> _processes = [];
    public void AddProcess(UploadProcess uploadProcess)
    {
        _processes[uploadProcess.TrackingId] = uploadProcess;
    }

    public UploadProcess? GetProcess(Guid trackingId)
    {
        _processes.TryGetValue(trackingId, out var process);
        return process;
    }

    public void UpdateProcess(UploadProcess uploadProcess)
    {
        _processes[uploadProcess.TrackingId] = uploadProcess;

    }
}
