namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when unauthorized access is attempted
/// </summary>
public class UnauthorizedAccessAttemptEvent : DomainEvent
{
    public int UserId { get; }
    public string UserName { get; }
    public string ResourceType { get; }
    public int? ResourceId { get; }
    public string Action { get; }
    public string IpAddress { get; }
    public string? UserAgent { get; }
    public DateTime AttemptTime { get; }

    public UnauthorizedAccessAttemptEvent(
        int userId,
        string userName,
        string resourceType,
        int? resourceId,
        string action,
        string ipAddress,
        string? userAgent)
    {
        UserId = userId;
        UserName = userName;
        ResourceType = resourceType;
        ResourceId = resourceId;
        Action = action;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        AttemptTime = DateTime.UtcNow;
    }
}
