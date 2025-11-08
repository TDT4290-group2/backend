using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Validation;
using Backend.Observers;
using Backend.Plugins.ThresholdChecker;
using Backend.Hubs;
using Backend.DTOs;
using Backend.Events;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);
var AllowDevFrontend = "_allowDevFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowDevFrontend,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173").AllowAnyHeader();
        });
});
builder.Services.AddControllers();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ValidateFieldForDataTypeFilter>();
builder.Services.AddScoped<INoteDataService, NoteDataService>();
builder.Services.AddScoped<IThresholdObserver, ThresholdNotificationObserver>();
builder.Services.AddScoped<IThresholdChecker>(sp => new ThresholdChecker(DataType.Noise));
builder.Services.AddScoped<IThresholdChecker>(sp => new ThresholdChecker(DataType.Dust));
builder.Services.AddScoped<IThresholdChecker>(sp => new ThresholdChecker(DataType.Vibration));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<ISensorDataService, SensorDataService>(sp =>
{
    var service = new SensorDataService(
        sp.GetRequiredService<AppDbContext>(),
        sp.GetRequiredService<IEnumerable<IThresholdChecker>>());

    var observer = sp.GetRequiredService<IThresholdObserver>();
    observer.Subscribe(service);

    return service;
});

builder.Services.AddSignalR();

var app = builder.Build();
app.UseRouting();
app.UseCors(AllowDevFrontend);
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hub");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
var sensorService = app.Services.GetRequiredService<ISensorDataService>();

sensorService.ThresholdExceeded += async (_, args) =>
{
    using var scope = app.Services.CreateScope();
    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

    var notification = args.ToNotification();

    var request = new NotificationRequestDto(
        DataType: notification.dataType,
        ExceedingLevel: notification.exceedingLevel,
        Value: notification.value,
        HappenedAt: notification.HappenedAt,
        IsRead: notification.IsRead,
        UserMessage: notification.userMessage
    );

    Console.WriteLine($"Notification triggered: {args.DataType} value {args.Value}");
    await notificationService.CreateNotificationAsync(request);
};

using (var scope = app.Services.CreateScope())
{
    var seedSensorService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();
    await seedSensorService.GenerateNotificationsFromSeededDataAsync();
}

await app.RunAsync();

// This is needed for integration tests
public partial class Program { }
