using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(
        int userId,
        string action,
        string entityType,
        string entityId,
        string description,
        string? severity = null,
        string? ipAddress = null,
        string? oldValue = null,
        string? newValue = null,
        CancellationToken cancellationToken = default)
    {
        // Get user name for audit log
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        var userName = user?.Username ?? "Unknown";

        var auditLog = new Domain.Entities.AuditLog(
            userId: userId,
            userName: userName,
            action: action,
            entityType: entityType,
            entityId: int.TryParse(entityId, out var id) ? id : null,
            entityDescription: description,
            previousValues: oldValue,
            newValues: newValue,
            ipAddress: ipAddress,
            userAgent: null,
            module: entityType,
            isSensitiveOperation: severity == "High" || severity == "Critical",
            referenceNumber: null,
            additionalInfo: null);

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(
        int? userId = null,
        string? entityType = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Include(al => al.User)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(al => al.UserId == userId.Value);

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(al => al.EntityType == entityType);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(al => al.Action == action);

        if (startDate.HasValue)
            query = query.Where(al => al.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(al => al.Timestamp <= endDate.Value);

        var logs = await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return logs.Select(al => new AuditLogDto
        {
            Id = al.Id,
            UserId = al.UserId,
            Username = al.User?.Username ?? al.UserName,
            Action = al.Action,
            EntityType = al.EntityType,
            EntityId = al.EntityId?.ToString() ?? "",
            Description = al.EntityDescription ?? "",
            Severity = al.IsSensitiveOperation ? "High" : "Normal",
            IpAddress = al.IpAddress,
            OldValue = al.PreviousValues,
            NewValue = al.NewValues,
            CreatedAt = al.Timestamp
        });
    }
}
