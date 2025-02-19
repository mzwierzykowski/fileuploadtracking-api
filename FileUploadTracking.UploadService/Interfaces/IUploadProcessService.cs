using FileUploadTracking.UploadService.Models;

namespace FileUploadTracking.UploadService.Interfaces;

public interface IUploadProcessService
{
    UploadProcess CreateUploadProcess(CreateProcessRequest request);
    Task UploadFile(Guid trackingId, UploadFileRequest request);
    UploadProcess? GetUploadProcess(Guid trackingId);
}
