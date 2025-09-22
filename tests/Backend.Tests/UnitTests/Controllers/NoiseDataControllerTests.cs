using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using backend.Controllers;
using backend.Records;

namespace Backend.Tests.UnitTests.Controllers;

public class NoiseControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoData()
    {

        var mockService = new Mock<INoiseDataService>();
        mockService.Setup(service => service.GetAllNoiseDataAsync()).ReturnsAsync(new List<NoiseData>());
        var controller = new NoiseDataController(mockService.Object);

        var result = await controller.GetAll();
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<OkObjectResult>(okResult);

        var data = okResult.Value as IEnumerable<NoiseDataResponseDto>;
        Assert.NotNull(data);
        Assert.Empty(data);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var mockService = new Mock<INoiseDataService>();
        var guid = new Guid();
        var time = TimeOnly.FromDateTime(DateTime.Now);
        var noiseData = new NoiseData(guid, 70.2, time);
        mockService.Setup(service => service.GetNoiseDataByIdAsync(guid))
                   .ReturnsAsync(noiseData);
        var controller = new NoiseDataController(mockService.Object);

        var result = await controller.GetById(guid);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedNoiseData = Assert.IsType<NoiseDataResponseDto>(okResult.Value);

        Assert.Equal(noiseData.Id, returnedNoiseData.Id);
        Assert.Equal(noiseData.LavgQ3, returnedNoiseData.LavgQ3);
        Assert.Equal(noiseData.Time, returnedNoiseData.Time);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var mockService = new Mock<INoiseDataService>();
        var guid = new Guid();
        mockService.Setup(service => service.GetNoiseDataByIdAsync(guid))
                     .ReturnsAsync((NoiseData?)null);
        var controller = new NoiseDataController(mockService.Object);
        var result = await controller.GetById(guid);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult_WithCreatedNoiseData()
    {
        var mockService = new Mock<INoiseDataService>();
        var guid = Guid.NewGuid();
        var time = TimeOnly.FromDateTime(DateTime.Now);
        var noiseData = new NoiseData(guid, 70.2, time);
        var noiseDataDto = new NoiseDataResponseDto(guid, 70.2, time);

        mockService.Setup(service => service.AddNoiseDataAsync(It.IsAny<NoiseData>()))
               .ReturnsAsync(noiseData);

        var controller = new NoiseDataController(mockService.Object);

        var result = await controller.Create(new CreateNoiseDataDto(70.2, time));

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedNoiseData = Assert.IsType<NoiseDataResponseDto>(createdAtActionResult.Value);

        Assert.Equal(noiseData.Id, returnedNoiseData.Id);
        Assert.Equal(noiseData.LavgQ3, returnedNoiseData.LavgQ3);
        Assert.Equal(noiseData.Time, returnedNoiseData.Time);
        Assert.Equal(nameof(controller.GetById), createdAtActionResult.ActionName);
    }
}