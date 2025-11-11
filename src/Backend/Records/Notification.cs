using Microsoft.EntityFrameworkCore;

namespace Backend.Records;


[PrimaryKey(nameof(Id))]
public record Notification(Guid Id, string? exceedingLevel, string? dataType, double? value, DateTime? HappenedAt, bool? IsRead, string? userMessage);
