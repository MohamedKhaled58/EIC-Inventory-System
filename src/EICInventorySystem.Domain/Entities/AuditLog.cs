namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Comprehensive audit log for all system operations
/// </summary>
public class AuditLog
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Action { get; private set; } = null!; // "Create", "Update", "Delete", "View", "Approve", "Reject"
    public string EntityType { get; private set; } = null!; // "Requisition", "Transfer", "Inventory", etc.
    public int? EntityId { get; private set; }
    public string? EntityDescription { get; private set; }
    public string? PreviousValues { get; private set; } // JSON string
    public string? NewValues { get; private set; } // JSON string
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Module { get; private set; } // "Inventory", "Requisition", "Transfer", etc.
    public bool IsSensitiveOperation { get; private set; }
    public string? ReferenceNumber { get; private set; } // RequisitionNumber, TransferNumber, etc.
    public string? AdditionalInfo { get; private set; } // JSON string for extra context

    // Navigation properties
    public User User { get; private set; } = null!;

    private AuditLog() { }

    public AuditLog(
        int userId,
        string userName,
        string action,
        string entityType,
        int? entityId = null,
        string? entityDescription = null,
        string? previousValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? module = null,
        bool isSensitiveOperation = false,
        string? referenceNumber = null,
        string? additionalInfo = null)
    {
        UserId = userId;
        UserName = userName;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        EntityDescription = entityDescription;
        PreviousValues = previousValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
        Module = module ?? entityType;
        IsSensitiveOperation = isSensitiveOperation;
        ReferenceNumber = referenceNumber;
        AdditionalInfo = additionalInfo;
    }
}
