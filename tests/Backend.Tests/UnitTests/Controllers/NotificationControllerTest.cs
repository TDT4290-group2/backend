using Backend.Controllers;
using Backend.DTOs;
using Backend.Records;
using Backend.Services;
using Backend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Backend.Tests.UnitTests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _mockService;
    private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _mockService = new Mock<INotificationService>();
        _mockHubContext = new Mock<IHubContext<NotificationHub>>();
        _controller = new NotificationController(_mockService.Object, _mockHubContext.Object);
    }

    [Fact]
    public async Task GetAllNotifications_ReturnsEmptyList_WhenNoneExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllNotificationsAsync())
                    .ReturnsAsync(new List<Notification>());

        // Act
        var result = await _controller.GetAllNotifications();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var notifications = Assert.IsAssignableFrom<IEnumerable<NotificationResponseDto>>(okResult.Value);
        Assert.Empty(notifications);
    }

    [Fact]
    public async Task GetNotificationByDataType_ReturnsNotFound_WhenNoneExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetNotificationsByDataTypeAsync("noise"))
                    .ReturnsAsync(new List<Notification>());

        // Act
        var result = await _controller.GetNotificationByDataType("noise");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetNotificationByDataTypeAndDate_ReturnsNotFound_WhenNoMatch()
    {
        // Arrange
        var date = DateTime.UtcNow;
        _mockService.Setup(s => s.GetNotificationByDataTypeAndDateAsync("dust", date))
                    .ReturnsAsync((Notification?)null);

        // Act
        var result = await _controller.GetNotificationByDataTypeAndDate("dust", date);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateNotificationMessage_ReturnsNotFound_WhenNoMatch()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var request = new NotificationRequestDto(
            DataType: "dust",
            ExceedingLevel: "medium",
            Value: 80.0,
            HappenedAt: date,
            IsRead: true,
            UserMessage: "Updated message"
        );

        _mockService.Setup(s => s.UpdateNotificationMessageAsync("dust", date, request))
                    .ReturnsAsync((Notification?)null);

        // Act
        var result = await _controller.UpdateNotificationMessage("dust", date, request);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
