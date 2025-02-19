using FileUploadTracking.NotificationService;
using FileUploadTracking.NotificationService.EventHandlers;
using FileUploadTracking.NotificationService.Interfaces;
using FileUploadTracking.NotificationService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICallbackService, CallbackService>();
builder.Services.AddSingleton<IQueueService, RabbitMqQueueService>();
builder.Services.AddSingleton<IProcessCompletedEventHandler, ProcessCompletedEventHandler>();

var host = builder.Build();
host.Run();
