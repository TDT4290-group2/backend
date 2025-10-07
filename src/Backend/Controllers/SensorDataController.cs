using Microsoft.AspNetCore.Mvc;
using Backend.Records;
using Backend.Services;
using Backend.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/sensor")]
public class SensorDataController : ControllerBase
{
    private readonly ISensorDataService _sensorDataService;

    public SensorDataController(ISensorDataService sensorDataService)
    {
        _sensorDataService = sensorDataService;
    }

    [HttpGet("noisedata/all")]
    public async Task<ActionResult<IEnumerable<SensorDataResponseDto>>> GetAllNoiseData()
    {
        var noiseData = await _sensorDataService.GetAllNoiseDataAsync();
        var response = noiseData.Select(nd => new SensorDataResponseDto
        {
            Time = nd.Time,
            Value = nd.LavgQ3
        });
        return Ok(response);
    }

    [HttpGet("{dataType}/{userId}")]
    public async Task<ActionResult<IEnumerable<SensorDataResponseDto>>> GetAggregatedNoiseData([FromBody] SensorDataRequestDto request, [FromRoute] Guid userId, [FromRoute] string dataType)
    {
        var response = await _sensorDataService.GetAggregatedDataAsync(request, userId, dataType);
        return Ok(response);
    }
}