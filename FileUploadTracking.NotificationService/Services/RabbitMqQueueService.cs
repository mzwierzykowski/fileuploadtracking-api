using FileUploadTracking.NotificationService.Interfaces;
using FileUploadTracking.NotificationService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace FileUploadTracking.NotificationService.Services;
internal class RabbitMqQueueService(ILogger<RabbitMqQueueService> logger, IConfiguration configuration, IProcessCompletedEventHandler eventHandler) : IQueueService, IDisposable
{
    private readonly ILogger<RabbitMqQueueService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IProcessCompletedEventHandler _eventHandler = eventHandler;
    private const string _exchangeName = "process.completed.exchange";
    private const string _queueName = "process.completed.notification.queue";
    private IChannel? _channel;
    private IConnection? _connection;

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "reabbitmq",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };
        int retryCount = 5;
        while (retryCount > 0)
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ...");
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                _logger.LogInformation("Connected to RabbitMQ successfully.");
                break;
            }
            catch
            {
                _logger.LogError("RabbitMQ connection failed. Retrying in 5 seconds...");
                retryCount--;
                await Task.Delay(5000, cancellationToken);
            }
        }
        if (_connection == null || _channel == null || !_connection.IsOpen)
        {
            _logger.LogError("Failed to connect to RabbitMQ after multiple attempts. Exiting...");
            throw new Exception("RabbitMQ connection failed.");
        }
        await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);
        await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
        await _channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {Message}", message);
            try
            {
                var processEvent = JsonSerializer.Deserialize<ProcessCompletedEvent>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (processEvent != null)
                {
                    await _eventHandler.HandleAsync(processEvent);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Listening for messages on queue: {QueueName}", _queueName);
    }
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
