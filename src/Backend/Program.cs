using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Validation;

var builder = WebApplication.CreateBuilder(args);
var AllowDevFrontend = "_allowDevFrontend";
var configuration = builder.Configuration;

var allowedHost = configuration["AllowedHost"] ?? "localhost";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowDevFrontend,
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
            {
                // Allow localhost for development
                if (origin == "http://localhost:5173")
                    return true;

                // Allow configured host and its subdomains
                var uri = new Uri(origin);
                if (uri.Host == allowedHost || uri.Host.EndsWith($".{allowedHost}"))
                    return true;

                return false;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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
builder.Services.AddScoped<INoteDataService, NoteDataService>();

var app = builder.Build();
// Migrate database during startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

app.UseCors(AllowDevFrontend);
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();

// This is needed for integration tests
public partial class Program { }