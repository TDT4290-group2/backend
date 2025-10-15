using Microsoft.EntityFrameworkCore;

namespace Backend.Records;


[PrimaryKey(nameof(Id), nameof(UserId))]
public record Notification(Guid Id, Guid UserId, string? exceedingLevel, string? dataType, double? value, DateTime? HappenedAt, bool? IsRead, string? userMessage);
