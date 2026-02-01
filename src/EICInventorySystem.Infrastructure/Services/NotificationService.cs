using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SendNotificationAsync(int userId, string title, string message, string type, CancellationToken cancellationToken = default)
    {
        var notification = new Domain.Entities.Notification(
            userId: userId,
            title: title,
            message: message,
            type: type,
            category: GetCategoryFromType(type),
            actionUrl: null,
            referenceNumber: null,
            entityType: null,
            entityId: null);

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SendNotificationToRoleAsync(string role, string title, string message, string type, CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .Where(u => u.Role == role && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        foreach (var userId in users)
        {
            var notification = new Domain.Entities.Notification(
                userId: userId,
                title: title,
                message: message,
                type: type,
                category: GetCategoryFromType(type),
                actionUrl: null,
                referenceNumber: null,
                entityType: null,
                entityId: null);

            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (unreadOnly)
            query = query.Where(n => !n.IsRead);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            TitleArabic = GetArabicTitle(n.Title, n.Type),
            Message = n.Message,
            MessageArabic = n.Message,
            Type = n.Type,
            EntityType = n.EntityType,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt
        });
    }

    public async Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
        if (notification != null)
        {
            notification.MarkAsRead();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNotificationAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, cancellationToken);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static string GetCategoryFromType(string type) => type switch
    {
        "LowStock" or "CriticalStock" => "Inventory",
        "ApprovalRequired" or "Approved" or "Rejected" => "Requisition",
        "TransferPending" or "TransferCompleted" => "Transfer",
        "CommanderReserve" => "CommanderReserve",
        _ => "System"
    };

    private static string GetArabicTitle(string title, string type) => type switch
    {
        "LowStock" => "تنبيه انخفاض المخزون",
        "CriticalStock" => "تنبيه مخزون حرج",
        "ApprovalRequired" => "مطلوب موافقة",
        "Approved" => "تمت الموافقة",
        "Rejected" => "تم الرفض",
        "TransferPending" => "تحويل قيد الانتظار",
        "TransferCompleted" => "تم التحويل",
        "CommanderReserve" => "احتياطي القائد",
        _ => title
    };
}
