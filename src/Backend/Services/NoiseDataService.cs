using Backend.Records;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface INoiseDataService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<NoiseData?> GetNoiseDataByIdAsync(Guid id);
    Task<NoiseData> AddNoiseDataAsync(NoiseData data);
    Task DeleteNoiseDataAsync(Guid id);
}

public class NoiseDataService: INoiseDataService
{
    private readonly AppDbContext _context;

    public NoiseDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync()
    {
        return await _context.NoiseData.ToListAsync();
    }

    public async Task<NoiseData?> GetNoiseDataByIdAsync(Guid id)
    {
        return await _context.NoiseData.FindAsync(id);
    }

    public async Task<NoiseData> AddNoiseDataAsync(NoiseData data)
    {
        _context.NoiseData.Add(data);
        await _context.SaveChangesAsync();
        return data;
    }

    public async Task DeleteNoiseDataAsync(Guid id)
    {
        var data = await _context.NoiseData.FindAsync(id);
        if (data != null)
        {
            _context.NoiseData.Remove(data);
            await _context.SaveChangesAsync();
        }
    }
}