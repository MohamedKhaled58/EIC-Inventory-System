namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// System notification for users
/// </summary>
public class Notification
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string Type { get; private set; } = null!; // "Info", "Warning", "Error", "Success", "ApprovalRequired"
    public string? Category { get; private set; } // "Inventory", "Requisition", "Transfer", "System"
    public string? ActionUrl { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? EntityType { get; private set; }
    public int? EntityId { get; private set; }

    public NotificationPriority Priority { get; private set; } = NotificationPriority.Normal;
    public bool IsDeleted { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private Notification() { }

    public Notification(
        int userId,
        string title,
        string message,
        string type,
        NotificationPriority priority = NotificationPriority.Normal,
        string? category = null,
        string? actionUrl = null,
        string? referenceNumber = null,
        string? entityType = null,
        int? entityId = null)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        Priority = priority;
        Category = category;
        ActionUrl = actionUrl;
        IsRead = false;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        ReferenceNumber = referenceNumber;
        EntityType = entityType;
        EntityId = entityId;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
