using FileUploadTracking.NotificationService.Interfaces;
using FileUploadTracking.NotificationService.Models;
using System.Net.Http.Json;

namespace FileUploadTracking.NotificationService.Services;
public class CallbackService(IHttpClientFactory httpClientFactory, ILogger<CallbackService> logger) : ICallbackService
{
    private readonly ILogger<CallbackService> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    public async Task SendCallbackAsync(string callbackUrl, CustomerCallbackRequest customerCallbackRequest)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(callbackUrl, customerCallbackRequest);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Callback notification sent successfully to {CallbackUrl}", callbackUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending callback notification to {CallbackUrl}", callbackUrl);
            throw;
        }
    }
}
