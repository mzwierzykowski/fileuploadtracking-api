using FileUploadTracking.Events;
using FileUploadTracking.UploadService.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace FileUploadTracking.UploadService.Services;

public class RabbitMqQueueService(ILogger<RabbitMqQueueService> logger, IConfiguration configuration) : IQueueService
{
    private readonly ILogger<RabbitMqQueueService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly string _exchange = "process.completed.exchange";

    public async Task SendProcessCompletedNotificationAsync(ProcessCompletedEvent processCompletedEvent)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchange: _exchange, type: ExchangeType.Fanout, durable: true);
        var serializedEvent = JsonSerializer.Serialize(processCompletedEvent);
        var body = Encoding.UTF8.GetBytes(serializedEvent);
        _logger.LogInformation("Publishing ProcessCompletedEvent: {Event}", serializedEvent);
        await channel.BasicPublishAsync(exchange: _exchange, routingKey: "", body: body);
    }
}
