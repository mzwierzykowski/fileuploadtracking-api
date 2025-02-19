namespace FileUploadTracking.UploadService.Models;

public class UploadProcess
{
    public Guid TrackingId { get; set; }
    public Guid UserId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public List<string> Files { get; set; } = [];
    public DateTime CreatedDateTime { get; set; }
    public ProcessState State { get; set; }
}

