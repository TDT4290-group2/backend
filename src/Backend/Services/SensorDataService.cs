using Backend.DTOs;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public interface ISensorDataService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(RequestContext requestContext);
}

public class SensorDataService : ISensorDataService
{
    private readonly AppDbContext _context;

    public SensorDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync()
    {
        return await _context.NoiseData.ToListAsync();
    }

    public async Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(RequestContext requestContext)
    {
        var request = requestContext.Request;
        var dataType = requestContext.DataType;
        var dataType_split = dataType + "_data";

        var materializedViewName = request.Granularity switch
        {
            TimeGranularity.Minute => dataType_split + "_minutely",
            TimeGranularity.Hour => dataType_split + "_hourly",
            TimeGranularity.Day => dataType_split + "_daily",
            _ => throw new ArgumentException($"Unsupported scope: {request.Granularity}")
        };
        var aggregateColumnName = request.Function switch
        {
            AggregationFunction.Avg => "avg_" + dataType,
            AggregationFunction.Sum => "sum_" + dataType,
            AggregationFunction.Min => "min_" + dataType,
            AggregationFunction.Max => "max_" + dataType,
            AggregationFunction.Count => "sample_count",
            _ => throw new ArgumentException($"Unsupported aggregation type: {request.Function}")
        };

        if (!request.Fields.IsNullOrEmpty())
        {
            aggregateColumnName += "_" + request.Fields[0];
        }

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