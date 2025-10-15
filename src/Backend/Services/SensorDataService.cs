using Backend.DTOs;
using Backend.Events;
using Backend.Models;
using Backend.Plugins.ThresholdChecker;
using Backend.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public interface ISensorDataService
{
    Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync();
    Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(RequestContext requestContext);
    event EventHandler<ThresholdExceededEventArgs>? ThresholdExceeded;
}

public class SensorDataService: ISensorDataService
{
    private readonly AppDbContext _context;
    private readonly IEnumerable<IThresholdChecker> _thresholdCheckers;

    // Event for threshold exceeded
    public event EventHandler<ThresholdExceededEventArgs>? ThresholdExceeded;
    
     public SensorDataService(
        AppDbContext context,
        IEnumerable<IThresholdChecker> thresholdCheckers)
    {
        _context = context;
        _thresholdCheckers = thresholdCheckers;
    }

    private void CheckThresholdAndNotify(double value, Guid userId, DataType dataType, DateTime timestamp)
    {
        var checker = _thresholdCheckers.FirstOrDefault(c => c.SensorType == dataType);
        if (checker == null) return;

        if (checker.IsThresholdExceeded(value, out string message))
        {
            var eventArgs = new ThresholdExceededEventArgs(
                userId: userId,
                dataType: dataType,
                value: value,
                message: message,
                timestamp: timestamp
            );
            OnThresholdExceeded(eventArgs);
        }
    }

    private void OnThresholdExceeded(ThresholdExceededEventArgs e)
    {
        ThresholdExceeded?.Invoke(this, e);
    }

    public async Task<IEnumerable<NoiseData>> GetAllNoiseDataAsync()
    {
        return await _context.NoiseData.ToListAsync();
    }

    public async Task<IEnumerable<SensorDataResponseDto>> GetAggregatedDataAsync(RequestContext requestContext)
    {
        var request = requestContext.Request ?? throw new ArgumentException("Request is not initialized.");
        var dataType = requestContext.DataType ?? throw new ArgumentException("DataType is not initialized.");

        var dataTypeLower = dataType.ToString().ToLower();

        var dataType_split = dataTypeLower + "_data";

        var materializedViewName = request.Granularity switch
        {
            TimeGranularity.Minute => dataType_split + "_minutely",
            TimeGranularity.Hour => dataType_split + "_hourly",
            TimeGranularity.Day => dataType_split + "_daily",
            _ => throw new ArgumentException($"Unsupported scope: {request.Granularity}")
        };

        var aggregateColumnName = request.Function switch
        {
            AggregationFunction.Avg => "avg_" + dataTypeLower,
            AggregationFunction.Sum => "sum_" + dataTypeLower,
            AggregationFunction.Min => "min_" + dataTypeLower,
            AggregationFunction.Max => "max_" + dataTypeLower,
            AggregationFunction.Count => "sample_count",
            _ => throw new ArgumentException($"Unsupported aggregation type: {request.Function}")
        };

        if (request.Field.HasValue)
        {
            aggregateColumnName += "_" + request.Field.Value.ToString().ToLower();
        }

        var startTime = request.StartTime ?? throw new ArgumentException("StartTime is required.");
        var endTime = request.EndTime ?? throw new ArgumentException("EndTime is required.");

        var sql = $@"
            SELECT 
                bucket as Time,
                {aggregateColumnName} as Value
            FROM {materializedViewName}
            WHERE bucket >= {{0}} AND bucket <= {{1}}
            ORDER BY bucket";

        var result = await _context.Database
            .SqlQueryRaw<SensorDataResponseDto>(sql, startTime, endTime)
            .ToListAsync();

        if (result.Count == 0)
        {
            return [];
        }

        return result;
    }
}