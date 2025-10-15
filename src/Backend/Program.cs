using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Validation;
using Backend.Observers;
using Backend.Plugins.ThresholdChecker;

var builder = WebApplication.CreateBuilder(args);
var  AllowDevFrontend = "_allowDevFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowDevFrontend,
        policy  =>
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

builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<ValidateFieldForDataTypeFilter>();

// Remove the first ISensorDataService registration and keep only this one
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IThresholdObserver, ThresholdNotificationObserver>();
builder.Services.AddScoped<IThresholdChecker, NoiseThresholdChecker>();  
builder.Services.AddScoped<ISensorDataService, SensorDataService>(sp =>
{
    var service = new SensorDataService(
        sp.GetRequiredService<AppDbContext>(),
        sp.GetRequiredService<IEnumerable<IThresholdChecker>>());
    
    var observer = sp.GetRequiredService<IThresholdObserver>();
    observer.Subscribe(service);
    
    return service;
});

var app = builder.Build();
app.UseCors(AllowDevFrontend);
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

// This is needed for integration tests
public partial class Program { }