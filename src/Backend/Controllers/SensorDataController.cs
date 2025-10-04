using Microsoft.AspNetCore.Mvc;
using Backend.Records;
using Backend.Services;
using Backend.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/sensor")]
public class SensorDataController : ControllerBase
{
    private readonly INoiseDataService _noiseDataService;

    public SensorDataController(INoiseDataService noiseDataService)
    {
        _noiseDataService = noiseDataService;
    }

    [HttpGet("noisedata/all")]
    public async Task<ActionResult<IEnumerable<SensorDataResponseDto>>> GetAllNoiseData()
    {
        var noiseData = await _noiseDataService.GetAllNoiseDataAsync();
        var response = noiseData.Select(nd => new SensorDataResponseDto
        {
            Time = nd.Time,
            Value = nd.LavgQ3
        });
        return Ok(response);
    }

    [HttpGet("noisedata/{userId}")]
    public async Task<ActionResult<IEnumerable<SensorDataResponseDto>>> GetAggregatedNoiseData([FromBody] SensorDataRequestDto request, [FromRoute] Guid userId)
    {
        var response = await _noiseDataService.GetAggregatedNoiseDataAsync(request, userId);
        return Ok(response);
    }
}