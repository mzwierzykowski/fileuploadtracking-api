using FileUploadTracking.UploadService.Models;

namespace FileUploadTracking.UploadService.Interfaces;

public interface IUploadProcessRepository
{
    void AddProcess(UploadProcess uploadProcess);
    UploadProcess? GetProcess(Guid trackingId);
    void UpdateProcess(UploadProcess uploadProcess);
}
