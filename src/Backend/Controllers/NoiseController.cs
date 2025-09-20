using Microsoft.AspNetCore.Mvc;
using backend.Records;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoiseDataController : ControllerBase
{
    private readonly INoiseService _noiseService;

    public NoiseDataController(INoiseService noiseService)
    {
        _noiseService = noiseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoiseDataResponseDto>>> GetAll()
    {
        var noiseData = await _noiseService.GetAllNoiseDataAsync();
        var response = noiseData.Select(nd => new NoiseDataResponseDto(
            nd.Id,
            nd.LavgQ3,
            nd.Time
        ));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoiseDataResponseDto>> GetById(Guid id)
    {
        var noiseData = await _noiseService.GetNoiseDataByIdAsync(id);
        if (noiseData == null)
        {
            return NotFound();
        }
        return Ok(new NoiseDataResponseDto(
            noiseData.Id,
            noiseData.LavgQ3,
            noiseData.Time
        ));
    }

    [HttpPost]
    public async Task<ActionResult<NoiseDataResponseDto>> Create(CreateNoiseDataDto data)
    {
        var noiseData = new NoiseData(Guid.NewGuid(), data.LavgQ3, TimeOnly.FromDateTime(DateTime.Now));
        var createdData = await _noiseService.AddNoiseDataAsync(noiseData);
        return CreatedAtAction(nameof(GetById), new { id = createdData.Id }, new NoiseDataResponseDto(
            createdData.Id,
            createdData.LavgQ3,
            createdData.Time
        ));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _noiseService.DeleteNoiseDataAsync(id);
        return NoContent();
    }
}