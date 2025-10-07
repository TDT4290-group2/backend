using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Controllers;
using GraphQL.AspNet;
using GraphQL.AspNet.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddGraphQL();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INoiseDataService, NoiseDataService>();

var app = builder.Build();
app.MapControllers();
app.UseGraphQL();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

// This is needed for integration tests
public partial class Program { }