using Microsoft.EntityFrameworkCore;

namespace Backend.Records;


[PrimaryKey(nameof(Id), nameof(UserId))]
public record Notification(Guid Id, Guid UserId, string Title, string Message, DateTime CreatedAt, bool IsRead);