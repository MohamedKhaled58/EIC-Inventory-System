using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EICInventorySystem.Infrastructure.Services;

/// <summary>
/// Background service for processing notification queues and sending emails/push notifications
/// </summary>
public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);

    public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_processingInterval, stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get unread notifications older than 1 hour for potential email sending
                var cutoffTime = DateTime.UtcNow.AddHours(-1);
                var pendingNotifications = await context.Notifications
                    .Include(n => n.User)
                    .Where(n => !n.IsRead && n.CreatedAt < cutoffTime && n.Type == "ApprovalRequired")
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var notification in pendingNotifications)
                {
                    // Log for email sending (actual email sending would be configured separately)
                    _logger.LogInformation(
                        "Pending notification for user {UserId}: {Title}",
                        notification.UserId,
                        notification.Title);
                }

                if (pendingNotifications.Any())
                {
                    _logger.LogInformation("Processed {Count} pending notifications", pendingNotifications.Count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notifications");
            }
        }

        _logger.LogInformation("Notification Background Service is stopping.");
    }
}

/// <summary>
/// Background service for monitoring inventory levels and generating alerts
/// </summary>
public class InventoryAlertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InventoryAlertBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public InventoryAlertBackgroundService(IServiceProvider serviceProvider, ILogger<InventoryAlertBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inventory Alert Background Service is starting.");

        // Initial delay to let the application fully start
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get all alerts
                var lowStockAlerts = (await inventoryService.GetLowStockAlertsAsync(cancellationToken: stoppingToken)).ToList();
                var reserveAlerts = (await inventoryService.GetReserveAlertsAsync(cancellationToken: stoppingToken)).ToList();

                // Process critical low stock alerts
                var criticalAlerts = lowStockAlerts.Where(a => a.Severity == "Critical").ToList();
                foreach (var alert in criticalAlerts)
                {
                    // Check if we've already sent a notification for this item in the last 24 hours
                    var existingNotification = await context.Notifications
                        .Where(n => n.EntityType == "InventoryRecord" && 
                                    n.EntityId == alert.ItemId &&
                                    n.Type == "CriticalStock" &&
                                    n.CreatedAt > DateTime.UtcNow.AddHours(-24))
                        .AnyAsync(stoppingToken);

                    if (!existingNotification)
                    {
                        // Send to warehouse managers
                        await notificationService.SendNotificationToRoleAsync(
                            "CentralWarehouseKeeper",
                            $"مخزون حرج: {alert.ItemName}",
                            $"المادة {alert.ItemCode} في مخزن {alert.WarehouseName} وصلت لمستوى حرج. " +
                            $"الكمية الحالية: {alert.CurrentQuantity:N2}, نقطة إعادة الطلب: {alert.ReorderPoint:N2}",
                            "CriticalStock",
                            stoppingToken);

                        // Also notify factory warehouse keepers
                        await notificationService.SendNotificationToRoleAsync(
                            "FactoryWarehouseKeeper",
                            $"مخزون حرج: {alert.ItemName}",
                            $"المادة {alert.ItemCode} في مخزن {alert.WarehouseName} وصلت لمستوى حرج. " +
                            $"الكمية الحالية: {alert.CurrentQuantity:N2}, نقطة إعادة الطلب: {alert.ReorderPoint:N2}",
                            "CriticalStock",
                            stoppingToken);

                        _logger.LogWarning(
                            "Critical stock alert for item {ItemCode} in warehouse {Warehouse}: Current={Current}, ReorderPoint={ReorderPoint}",
                            alert.ItemCode, alert.WarehouseName, alert.CurrentQuantity, alert.ReorderPoint);
                    }
                }

                // Process reserve alerts
                var lowReserveAlerts = reserveAlerts.ToList();
                foreach (var alert in lowReserveAlerts)
                {
                    var existingNotification = await context.Notifications
                        .Where(n => n.EntityType == "CommanderReserve" && 
                                    n.EntityId == alert.ItemId &&
                                    n.CreatedAt > DateTime.UtcNow.AddHours(-24))
                        .AnyAsync(stoppingToken);

                    if (!existingNotification)
                    {
                        await notificationService.SendNotificationToRoleAsync(
                            "ComplexCommander",
                            $"انخفاض احتياطي القائد: {alert.ItemName}",
                            $"احتياطي القائد للمادة {alert.ItemCode} في مخزن {alert.WarehouseName} أقل من الحد الأدنى. " +
                            $"الكمية الحالية: {alert.CurrentReserve:N2}, الحد الأدنى: {alert.MinimumRequired:N2}",
                            "CommanderReserve",
                            stoppingToken);
                    }
                }

                _logger.LogInformation(
                    "Inventory alerts check completed. Low stock: {LowCount}, Critical: {CriticalCount}, Low reserve: {ReserveCount}",
                    lowStockAlerts.Count, criticalAlerts.Count, lowReserveAlerts.Count);

                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing inventory alerts");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        _logger.LogInformation("Inventory Alert Background Service is stopping.");
    }
}

/// <summary>
/// Background service for scheduled report generation
/// </summary>
public class ReportGenerationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReportGenerationBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public ReportGenerationBackgroundService(IServiceProvider serviceProvider, ILogger<ReportGenerationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Report Generation Background Service is starting.");

        // Initial delay
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();

                // Check for scheduled reports
                var now = DateTime.UtcNow;
                
                // Generate daily reports at midnight UTC
                if (now.Hour == 0 && now.Minute < 5)
                {
                    _logger.LogInformation("Generating daily inventory reports...");
                    
                    var warehouses = await context.Warehouses.Where(w => w.IsActive).ToListAsync(stoppingToken);
                    foreach (var warehouse in warehouses)
                    {
                        try
                        {
                            var report = await reportService.GenerateInventoryReportAsync(warehouse.Id, "pdf", stoppingToken);
                            
                            // Store report in file system or blob storage
                            var reportPath = Path.Combine("Reports", "Daily", $"{warehouse.Code}_{now:yyyyMMdd}_inventory.pdf");
                            var directory = Path.GetDirectoryName(reportPath);
                            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                                Directory.CreateDirectory(directory);
                            
                            await File.WriteAllBytesAsync(reportPath, report, stoppingToken);
                            
                            _logger.LogInformation("Generated daily report for warehouse {Warehouse}", warehouse.Name);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to generate report for warehouse {Warehouse}", warehouse.Name);
                        }
                    }
                }

                // Generate weekly reports on Sunday at 1 AM UTC
                if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour == 1 && now.Minute < 5)
                {
                    _logger.LogInformation("Generating weekly stock movement reports...");
                    
                    var startDate = now.AddDays(-7);
                    var endDate = now;

                    try
                    {
                        var report = await reportService.GenerateStockMovementReportAsync(null, startDate, endDate, "pdf", stoppingToken);
                        var reportPath = Path.Combine("Reports", "Weekly", $"stock_movement_{now:yyyyMMdd}.pdf");
                        var directory = Path.GetDirectoryName(reportPath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        
                        await File.WriteAllBytesAsync(reportPath, report, stoppingToken);
                        _logger.LogInformation("Generated weekly stock movement report");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to generate weekly stock movement report");
                    }
                }

                // Generate monthly commander reserve reports on the 1st at 2 AM UTC
                if (now.Day == 1 && now.Hour == 2 && now.Minute < 5)
                {
                    _logger.LogInformation("Generating monthly commander reserve reports...");
                    
                    try
                    {
                        var report = await reportService.GenerateCommanderReserveReportAsync(null, "pdf", stoppingToken);
                        var reportPath = Path.Combine("Reports", "Monthly", $"commander_reserve_{now:yyyyMM}.pdf");
                        var directory = Path.GetDirectoryName(reportPath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        
                        await File.WriteAllBytesAsync(reportPath, report, stoppingToken);
                        _logger.LogInformation("Generated monthly commander reserve report");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to generate monthly commander reserve report");
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in report generation service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Report Generation Background Service is stopping.");
    }
}

/// <summary>
/// Background service for cleaning up old audit logs and notifications
/// </summary>
public class DataCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataCleanupBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public DataCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<DataCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Cleanup Background Service is starting.");

        // Wait until 3 AM to run cleanup
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                
                // Run at 3 AM UTC
                if (now.Hour == 3)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Delete read notifications older than 30 days
                    var notificationCutoff = DateTime.UtcNow.AddDays(-30);
                    var oldNotifications = await context.Notifications
                        .Where(n => n.IsRead && n.CreatedAt < notificationCutoff)
                        .ToListAsync(stoppingToken);

                    if (oldNotifications.Any())
                    {
                        context.Notifications.RemoveRange(oldNotifications);
                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Deleted {Count} old notifications", oldNotifications.Count);
                    }

                    // Archive audit logs older than 90 days (mark as archived, don't delete)
                    var auditCutoff = DateTime.UtcNow.AddDays(-90);
                    var oldAuditCount = await context.AuditLogs
                        .Where(a => a.Timestamp < auditCutoff)
                        .CountAsync(stoppingToken);

                    if (oldAuditCount > 0)
                    {
                        _logger.LogInformation("Found {Count} audit logs older than 90 days for potential archival", oldAuditCount);
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in data cleanup service");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Data Cleanup Background Service is stopping.");
    }
}
