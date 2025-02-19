using FileUploadTracking.UploadService.Interfaces;
using FileUploadTracking.UploadService.Models;
using FileUploadTracking.UploadService.Repositories;
using FileUploadTracking.UploadService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Application Logic
builder.Services.AddSingleton<IUploadProcessRepository, InMemoryUploadProcessRepository>();
builder.Services.AddScoped<IUploadProcessService, UploadProcessService>();
builder.Services.AddScoped<IQueueService, RabbitMqQueueService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Create a new upload process
app.MapPost("/api/processes", async (CreateProcessRequest request,
                                        IValidator<CreateProcessRequest> validator,
                                        IUploadProcessService uploadProcessService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var process = uploadProcessService.CreateUploadProcess(request);
    return Results.Created($"/api/processes/{process.TrackingId}", process);
});

// Retrieve process details by tracking ID and UserId
app.MapGet("/api/processes/{trackingId}", (Guid trackingId, 
                                             IUploadProcessService uploadProcessService) =>
{
    var process = uploadProcessService.GetUploadProcess(trackingId);
    if (process is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(process);
});

// Simulate file upload for a process with UserId verification
app.MapPost("/api/processes/{trackingId}/users/{userId}/files", async (Guid trackingId, Guid userId,
                                                                       UploadFileRequest fileRequest,
                                                                       IValidator<UploadFileRequest> validator,
                                                                       IUploadProcessService uploadProcessService) =>
{
    // Validate the file upload request
    var validationResult = await validator.ValidateAsync(fileRequest);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var process = uploadProcessService.GetUploadProcess(trackingId);
    if (process is null)
    {
        return Results.NotFound();
    }

    // Verify that the process belongs to the provided user.
    if (process.UserId != userId)
    {
        return Results.Unauthorized();
    }

    if (process.State == ProcessState.Completed)
    {
        return Results.BadRequest("Process already completed.");
    }

    await uploadProcessService.UploadFile(trackingId, fileRequest);

    return Results.Ok(process);
});

app.Run();