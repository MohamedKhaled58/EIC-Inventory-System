namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an audit log is created
/// </summary>
public class AuditLogCreatedEvent : DomainEvent
{
    public int AuditLogId { get; }
    public int UserId { get; }
    public string UserName { get; }
    public string Action { get; }
    public string EntityType { get; }
    public int? EntityId { get; }
    public string? EntityDescription { get; }
    public string? IpAddress { get; }
    public string? UserAgent { get; }
    public string? Module { get; }
    public bool IsSensitiveOperation { get; }
    public string? ReferenceNumber { get; }

    public AuditLogCreatedEvent(
        int auditLogId,
        int userId,
        string userName,
        string action,
        string entityType,
        int? entityId,
        string? entityDescription,
        string? ipAddress,
        string? userAgent,
        string? module,
        bool isSensitiveOperation,
        string? referenceNumber)
    {
        AuditLogId = auditLogId;
        UserId = userId;
        UserName = userName;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        EntityDescription = entityDescription;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Module = module;
        IsSensitiveOperation = isSensitiveOperation;
        ReferenceNumber = referenceNumber;
    }
}
