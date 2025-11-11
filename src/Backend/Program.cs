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
builder.Services.AddScoped<INoteDataService, NoteDataService>();

var app = builder.Build();
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