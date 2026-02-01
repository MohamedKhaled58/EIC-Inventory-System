namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a notification is created
/// </summary>
public class NotificationCreatedEvent : DomainEvent
{
    public int NotificationId { get; }
    public int UserId { get; }
    public string Title { get; }
    public string Message { get; }
    public string Type { get; }
    public string? Category { get; }
    public string? ActionUrl { get; }
    public string? ReferenceNumber { get; }
    public string? EntityType { get; }
    public int? EntityId { get; }

    public NotificationCreatedEvent(
        int notificationId,
        int userId,
        string title,
        string message,
        string type,
        string? category,
        string? actionUrl,
        string? referenceNumber,
        string? entityType,
        int? entityId)
    {
        NotificationId = notificationId;
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        Category = category;
        ActionUrl = actionUrl;
        ReferenceNumber = referenceNumber;
        EntityType = entityType;
        EntityId = entityId;
    }
}
