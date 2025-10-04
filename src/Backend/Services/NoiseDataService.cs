using Backend.DTOs;
using Backend.Records;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface INoiseDataService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<IEnumerable<SensorDataResponseDto>> GetAggregatedNoiseDataAsync(SensorDataRequestDto request, Guid userId);
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

    public async Task<IEnumerable<SensorDataResponseDto>> GetAggregatedNoiseDataAsync(SensorDataRequestDto request, Guid userId)
    {
        var materializedViewName = request.Granularity switch
            {
                TimeGranularity.Minute => "noise_data_minutely",
                TimeGranularity.Hour => "noise_data_hourly",
                TimeGranularity.Day => "noise_data_daily",
                _ => throw new ArgumentException($"Unsupported scope: {request.Granularity}")
            };

        var aggregateColumnName = request.Function switch
            {
                AggregationFunction.Avg => "avg_noise",
                AggregationFunction.Sum => "sum_noise",
                AggregationFunction.Min => "min_noise",
                AggregationFunction.Max => "max_noise",
                AggregationFunction.Count => "sample_count",
                _ => throw new ArgumentException($"Unsupported aggregation type: {request.Function}")
            };

        var sql = $@"
            SELECT 
                bucket as Time,
                {aggregateColumnName} as Value
            FROM {materializedViewName}
            WHERE bucket >= {{0}} AND bucket <= {{1}}
            ORDER BY bucket";

        try
        {
            var result = await _context.Database
                .SqlQueryRaw<SensorDataResponseDto>(sql, request.StartTime, request.EndTime)
                .ToListAsync();

            if (!result.Any())
            {
                return new List<SensorDataResponseDto>();
            }

            return result;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements"))
        {
            return new List<SensorDataResponseDto>();
        }
    }
}