using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Validation;
using Backend.Observers;
using Backend.Plugins.ThresholdChecker;
using Backend.Hubs;
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
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddSingleton<IThresholdObserver, ThresholdNotificationObserver>();
builder.Services.AddSingleton<IThresholdChecker>(new ThresholdChecker(DataType.Noise));
builder.Services.AddSingleton<IThresholdChecker>(new ThresholdChecker(DataType.Dust));
builder.Services.AddSingleton<IThresholdChecker>(new ThresholdChecker(DataType.Vibration));
builder.Services.AddSingleton<ISensorDataService>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var thresholdCheckers = sp.GetRequiredService<IEnumerable<IThresholdChecker>>();
    var observer = sp.GetRequiredService<IThresholdObserver>();
    var service = new SensorDataService(scopeFactory, thresholdCheckers);
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
using (var seedScope = app.Services.CreateScope())
{
    var sensorService = seedScope.ServiceProvider.GetRequiredService<ISensorDataService>();
    await sensorService.GenerateNotificationsFromSeededDataAsync();
}

await app.RunAsync();

// This is needed for integration tests
public partial class Program { }
