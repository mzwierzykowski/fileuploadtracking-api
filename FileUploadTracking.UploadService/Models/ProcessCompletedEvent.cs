namespace FileUploadTracking.Events;

/// <summary>
/// Event data for a process that has completed.
/// </summary>
public class ProcessCompletedEvent
{
    /// <summary>
    /// Gets or sets the tracking identifier for the process.
    /// </summary>
    public Guid TrackingId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the process.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the callback URL that should be notified when the process is complete.
    /// </summary>
    public string CallbackUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the process was completed.
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// List of files for the customer
    /// </summary>
    public List<string> Files { get; set; } = [];
}
