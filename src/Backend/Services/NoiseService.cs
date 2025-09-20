using backend.Records;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public interface INoiseService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<NoiseData?> GetNoiseDataByIdAsync(Guid id);
    Task<NoiseData> AddNoiseDataAsync(NoiseData data);
    Task DeleteNoiseDataAsync(Guid id);
}

public class NoiseService: INoiseService
{
    private readonly AppDbContext _context;

    public NoiseService(AppDbContext context)
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