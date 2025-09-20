using Microsoft.EntityFrameworkCore;
using backend.Records;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NoiseData> NoiseData { get; set; }
}