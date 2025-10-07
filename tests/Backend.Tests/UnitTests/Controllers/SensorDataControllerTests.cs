using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Backend.Controllers;
using Backend.Records;

namespace Backend.Tests.UnitTests.Controllers;

public class NoiseControllerTests
{
    /// <summary>
    /// Verifies that GetAggregatedData returns an empty list when no sensor data (noise, dust, vibration) is available.
    /// </summary>
    /// <remarks>
    /// This test ensures the controller properly handles empty datasets from the service layer
    /// and returns an appropriate HTTP 200 response with an empty collection for noise, dust and vibration data.
    /// </remarks>
    [Fact]
    public async Task GetAggregatedData_ReturnsEmptyList_WhenNoData()
    {

        var mockService = new Mock<ISensorDataService>();

        mockService.Setup(service => service.GetAggregatedDataAsync(It.IsAny<RequestContext>()))
                    .ReturnsAsync(new List<SensorDataResponseDto>());

        var controller = new SensorDataController(mockService.Object);

        var noiseRequest = new SensorDataRequestDto(
            DateTimeOffset.Parse("2025-02-12T10:12:31+00:00"),
            DateTimeOffset.Parse("2025-02-25T16:14:10+00:00"),
            (TimeGranularity)2,
            (AggregationFunction)0,
            Array.Empty<string>()
        );
        var noiseResult = await controller.GetAggregatedData(noiseRequest, Guid.NewGuid(), "noise");
        var okNoiseResult = noiseResult.Result as OkObjectResult;
        Assert.IsType<OkObjectResult>(okNoiseResult);
        var noiseData = okNoiseResult.Value as IEnumerable<SensorDataResponseDto>;
        Assert.NotNull(noiseData);
        Assert.Empty(noiseData);

        var dustRequest = new SensorDataRequestDto(
            DateTimeOffset.Parse("2025-02-12T10:12:31+00:00"),
            DateTimeOffset.Parse("2025-02-25T16:14:10+00:00"),
            (TimeGranularity)2,
            (AggregationFunction)0,
            Array.Empty<string>()
        );
        var dustResult = await controller.GetAggregatedData(dustRequest, Guid.NewGuid(), "dust");
        var okDustResult = dustResult.Result as OkObjectResult;
        Assert.IsType<OkObjectResult>(okDustResult);
        var dustData = okDustResult.Value as IEnumerable<SensorDataResponseDto>;
        Assert.NotNull(dustData);
        Assert.Empty(dustData);

        var vibrationRequest = new SensorDataRequestDto(
            DateTimeOffset.Parse("2025-02-12T10:12:31+00:00"),
            DateTimeOffset.Parse("2025-02-25T16:14:10+00:00"),
            (TimeGranularity)2,
            (AggregationFunction)0,
            ["pm10_stel"]
        );
        var vibrationResult = await controller.GetAggregatedData(vibrationRequest, Guid.NewGuid(), "vibration");
        var okVibrationResult = vibrationResult.Result as OkObjectResult;
        Assert.IsType<OkObjectResult>(okVibrationResult);
        var vibrationData = okVibrationResult.Value as IEnumerable<SensorDataResponseDto>;
        Assert.NotNull(vibrationData);
        Assert.Empty(vibrationData);
    }

    /// <summary>
    /// Verifies that GetAggregatedData returns a list of sensor data (noise, dust, vibration) when data is available.
    /// </summary>
    /// <remarks>
    /// This test ensures the controller properly handles datasets from the service layer
    /// and returns an appropriate HTTP 200 response with the expected collection of sensor data for noise, dust and vibration.
    /// </remarks>
    [Fact]
    public async Task GetAggregatedData_ReturnsDataList_WhenDataExists()
    {
        var mockService = new Mock<ISensorDataService>();
        mockService.Setup(service => service.GetAggregatedDataAsync(It.IsAny<RequestContext>()))
                    .ReturnsAsync(new List<SensorDataResponseDto>
                    {
                        new SensorDataResponseDto { Time = DateTime.Parse("2025-02-12T10:12:31+00:00"), Value = 42.0 },
                        new SensorDataResponseDto { Time = DateTime.Parse("2025-02-13T11:15:45+00:00"), Value = 36.5 }
                    });
                    
        var controller = new SensorDataController(mockService.Object);

        var request = new SensorDataRequestDto(
            DateTimeOffset.Parse("2025-02-12T10:12:31+00:00"),
            DateTimeOffset.Parse("2025-02-25T16:14:10+00:00"),
            (TimeGranularity)2,
            (AggregationFunction)0,
            Array.Empty<string>()
        );
        var result = await controller.GetAggregatedData(request, Guid.NewGuid(), "noise");
        var okResult = result.Result as OkObjectResult;
        Assert.IsType<OkObjectResult>(okResult);

        var data = okResult.Value as IEnumerable<SensorDataResponseDto>;
        Assert.NotNull(data);
        Assert.Equal(2, data.Count());
    }
    


}