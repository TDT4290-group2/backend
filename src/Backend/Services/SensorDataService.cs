using Backend.DTOs;
using Backend.Models;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public interface ISensorDataService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(SensorDataRequestDto request, Guid userId, DataType dataType);
}

public class SensorDataService(AppDbContext context) : ISensorDataService
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync()
    {
        return await _context.NoiseData.ToListAsync();
    }

    public async Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(SensorDataRequestDto request, Guid userId, DataType dataType)
    {
        var dataType_split = dataType.ToString().ToLower() + "_data";

        var materializedViewName = request.Granularity switch
            {
                TimeGranularity.Minute => dataType_split + "_minutely",
                TimeGranularity.Hour => dataType_split + "_hourly",
                TimeGranularity.Day => dataType_split + "_daily",
                _ => throw new ArgumentException($"Unsupported scope: {request.Granularity}")
            };
        var aggregateColumnName = request.Function switch
            {
                AggregationFunction.Avg => "avg_" + dataType.ToString().ToLower(),
                AggregationFunction.Sum => "sum_" + dataType.ToString().ToLower(),
                AggregationFunction.Min => "min_" + dataType.ToString().ToLower(),
                AggregationFunction.Max => "max_" + dataType.ToString().ToLower(),
                AggregationFunction.Count => "sample_count",
                _ => throw new ArgumentException($"Unsupported aggregation type: {request.Function}")
            };

        if (request.Field.HasValue)
        {
            aggregateColumnName += "_" + request.Field.Value.ToString().ToLower();
        }

        var sql = $@"
            SELECT 
                bucket as Time,
                {aggregateColumnName} as Value
            FROM {materializedViewName}
            WHERE bucket >= {{0}} AND bucket <= {{1}}
            ORDER BY bucket";

        var result = await _context.Database
            .SqlQueryRaw<SensorDataResponseDto>(sql, request.StartTime, request.EndTime)
            .ToListAsync();

        if (result.Count == 0)
        {
            return [];
        }

        return result;
    }
}