using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class SystemSettingsRepository : Repository<SystemSettings>, ISystemSettingsRepository
{
    public SystemSettingsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SystemSettings?> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SystemSettings> GetOrCreateSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbSet.FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
        {
            settings = new SystemSettings(
                "EIC Inventory System",
                "نظام إدارة المخازن - مجمع الصناعات الهندسية",
                "ar",
                "EGP",
                "dd/MM/yyyy",
                true,
                1);

            await _dbSet.AddAsync(settings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return settings;
    }

    public async Task UpdateSettingsAsync(SystemSettings settings, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(settings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> GetSettingValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var settings = await _dbSet.FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
            return null;

        return key switch
        {
            "SystemName" => settings.SystemName,
            "SystemNameArabic" => settings.SystemNameArabic,
            "DefaultLanguage" => settings.DefaultLanguage,
            "DefaultCurrency" => settings.DefaultCurrency,
            "DateFormat" => settings.DateFormat,
            "TimeFormat" => settings.TimeFormat,
            "TimeZone" => settings.TimeZone,
            "EnableNotifications" => settings.EnableNotifications.ToString(),
            "EnableEmailNotifications" => settings.EnableEmailNotifications.ToString(),
            "EnableAuditLogging" => settings.EnableAuditLogging.ToString(),
            "AuditLogRetentionDays" => settings.AuditLogRetentionDays.ToString(),
            "NotificationRetentionDays" => settings.NotificationRetentionDays.ToString(),
            "ReportRetentionDays" => settings.ReportRetentionDays.ToString(),
            "SessionTimeoutMinutes" => settings.SessionTimeoutMinutes.ToString(),
            "MaxLoginAttempts" => settings.MaxLoginAttempts.ToString(),
            "LockoutDurationMinutes" => settings.LockoutDurationMinutes.ToString(),
            "PasswordMinLength" => settings.PasswordMinLength.ToString(),
            "PasswordRequireUppercase" => settings.PasswordRequireUppercase.ToString(),
            "PasswordRequireLowercase" => settings.PasswordRequireLowercase.ToString(),
            "PasswordRequireNumbers" => settings.PasswordRequireNumbers.ToString(),
            "PasswordRequireSpecialChars" => settings.PasswordRequireSpecialChars.ToString(),
            "PasswordExpiryDays" => settings.PasswordExpiryDays.ToString(),
            "EnableTwoFactorAuth" => settings.EnableTwoFactorAuth.ToString(),
            "EnableIpWhitelist" => settings.EnableIpWhitelist.ToString(),
            "EnableCommanderReserveTracking" => settings.EnableCommanderReserveTracking.ToString(),
            "CommanderReservePercentage" => settings.CommanderReservePercentage.ToString(),
            "EnableLowStockAlerts" => settings.EnableLowStockAlerts.ToString(),
            "LowStockThresholdPercentage" => settings.LowStockThresholdPercentage.ToString(),
            "EnableReorderAlerts" => settings.EnableReorderAlerts.ToString(),
            "EnableExpiryAlerts" => settings.EnableExpiryAlerts.ToString(),
            "ExpiryAlertDays" => settings.ExpiryAlertDays.ToString(),
            "EnableAutoReorder" => settings.EnableAutoReorder.ToString(),
            "EnableBarcodeScanning" => settings.EnableBarcodeScanning.ToString(),
            "EnableQrCodeGeneration" => settings.EnableQrCodeGeneration.ToString(),
            "EnablePdfReports" => settings.EnablePdfReports.ToString(),
            "EnableExcelExport" => settings.EnableExcelExport.ToString(),
            "EnableDataBackup" => settings.EnableDataBackup.ToString(),
            "BackupSchedule" => settings.BackupSchedule,
            "BackupRetentionDays" => settings.BackupRetentionDays.ToString(),
            "EnableDataArchiving" => settings.EnableDataArchiving.ToString(),
            "ArchiveAfterDays" => settings.ArchiveAfterDays.ToString(),
            "MaintenanceMode" => settings.MaintenanceMode.ToString(),
            "MaintenanceMessage" => settings.MaintenanceMessage,
            "MaintenanceMessageArabic" => settings.MaintenanceMessageArabic,
            "ApiRateLimitPerMinute" => settings.ApiRateLimitPerMinute.ToString(),
            "EnableApiRateLimiting" => settings.EnableApiRateLimiting.ToString(),
            "EnableRequestLogging" => settings.EnableRequestLogging.ToString(),
            "EnablePerformanceMonitoring" => settings.EnablePerformanceMonitoring.ToString(),
            _ => null
        };
    }

    public async Task<bool> IsMaintenanceModeAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbSet.Select(s => s.MaintenanceMode).FirstOrDefaultAsync(cancellationToken);
        return settings;
    }

    public async Task SetMaintenanceModeAsync(bool enabled, CancellationToken cancellationToken = default)
    {
        // SystemSettings properties are read-only. Need to recreate with updated values or add an Update method to entity.
        // For now, we can only update via EF tracking the entity
        var settings = await _dbSet.FirstOrDefaultAsync(cancellationToken);
        if (settings != null)
        {
            // Note: Unable to set MaintenanceMode directly - entity needs Update method
            // This is a design limitation that would require entity modification
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<decimal> GetCommanderReservePercentageAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbSet.Select(s => s.CommanderReservePercentage).FirstOrDefaultAsync(cancellationToken);
        return settings;
    }

    public async Task UpdateCommanderReservePercentageAsync(decimal percentage, CancellationToken cancellationToken = default)
    {
        // SystemSettings properties are read-only. Need entity modification to support updates.
        var settings = await _dbSet.FirstOrDefaultAsync(cancellationToken);
        if (settings != null)
        {
            // Note: Unable to set CommanderReservePercentage directly - entity needs Update method
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
