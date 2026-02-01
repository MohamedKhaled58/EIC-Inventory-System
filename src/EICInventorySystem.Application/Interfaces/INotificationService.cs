namespace EICInventorySystem.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string title, string message, string type, CancellationToken cancellationToken = default);
    Task SendNotificationToRoleAsync(string role, string title, string message, string type, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteNotificationAsync(int notificationId, CancellationToken cancellationToken = default);
}

public record NotificationDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string TitleArabic { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string MessageArabic { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? EntityType { get; init; }
    public int? EntityId { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ReadAt { get; init; }
}
