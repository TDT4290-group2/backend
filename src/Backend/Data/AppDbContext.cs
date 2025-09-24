using Microsoft.EntityFrameworkCore;
using Backend.Records;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NoiseData> NoiseData { get; set; }
    public DbSet<DustData> DustData { get; set; }
    public DbSet<VibrationData> VibrationData { get; set; }
}