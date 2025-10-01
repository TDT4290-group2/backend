// using Backend.DTOs;
// using Backend.Records;
// using Microsoft.EntityFrameworkCore;
// using Npgsql;

// namespace Backend.Services;

// public interface ISensorDataService
// {
//     Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();

//     Task<OneOf<IEnumerable<NoiseData>, IEnumerable<DustData>, IEnumerable<VibrationData>>> GetSensorDataAsync(SensorDataRequestDto request);
// }


// public class SensorDataService : ISensorDataService
// {
//     private readonly AppDbContext _context;

//     public SensorDataService(AppDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync()
//     {
//         return await _context.NoiseData.ToListAsync();
//     }

//     public Task<IEnumerable<NoiseData>> GetNoiseDataAsync()
//     {
//         throw new NotImplementedException();
//     }
// }