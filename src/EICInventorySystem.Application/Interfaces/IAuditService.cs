namespace EICInventorySystem.Application.Interfaces;

public interface IAuditService
{
    Task LogActionAsync(
        int userId,
        string action,
        string entityType,
        string entityId,
        string description,
        string? severity = null,
        string? ipAddress = null,
        string? oldValue = null,
        string? newValue = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(
        int? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}

public record AuditLogDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Severity { get; init; }
    public string? IpAddress { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public DateTime CreatedAt { get; init; }
}
