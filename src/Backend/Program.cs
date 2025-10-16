using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Validation;

var builder = WebApplication.CreateBuilder(args);
var  AllowDevFrontend = "_allowDevFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowDevFrontend,
        policy  =>
        {
            policy.SetIsOriginAllowed(origin => 
                new Uri(origin).Host == "localhost"
            ).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
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

var app = builder.Build();
app.UseCors(AllowDevFrontend);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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