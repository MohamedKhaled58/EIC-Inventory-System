namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// System-wide configuration settings
/// </summary>
public class SystemSettings : BaseEntity
{
    public string SystemName { get; private set; } = "EIC Inventory System";
    public string SystemNameArabic { get; private set; } = "نظا إدارة المخزون";
    public string DefaultLanguage { get; private set; } = "ar-EG";
    public string DefaultCurrency { get; private set; } = "EGP";
    public string DateFormat { get; private set; } = "yyyy-MM-dd";
    public string TimeFormat { get; private set; } = "HH:mm:ss";
    public string TimeZone { get; private set; } = "Egypt Standard Time";
    
    // Notifications
    public bool EnableNotifications { get; private set; } = true;
    public bool EnableEmailNotifications { get; private set; } = true;
    public int NotificationRetentionDays { get; private set; } = 30;
    
    // Audit & Logging
    public bool EnableAuditLogging { get; private set; } = true;
    public int AuditLogRetentionDays { get; private set; } = 90;
    public bool EnableRequestLogging { get; private set; } = true;
    public bool EnablePerformanceMonitoring { get; private set; } = true;

    // Reports
    public bool EnablePdfReports { get; private set; } = true;
    public bool EnableExcelExport { get; private set; } = true;
    public int ReportRetentionDays { get; private set; } = 365;

    // Security
    public int SessionTimeoutMinutes { get; private set; } = 60;
    public int MaxLoginAttempts { get; private set; } = 5;
    public int LockoutDurationMinutes { get; private set; } = 30;
    public int PasswordMinLength { get; private set; } = 8;
    public bool PasswordRequireUppercase { get; private set; } = true;
    public bool PasswordRequireLowercase { get; private set; } = true;
    public bool PasswordRequireNumbers { get; private set; } = true;
    public bool PasswordRequireSpecialChars { get; private set; } = true;
    public int PasswordExpiryDays { get; private set; } = 90;
    public bool EnableTwoFactorAuth { get; private set; } = false;
    public bool EnableIpWhitelist { get; private set; } = false;
    public int ApiRateLimitPerMinute { get; private set; } = 60;
    public bool EnableApiRateLimiting { get; private set; } = true;

    // Inventory
    public bool EnableCommanderReserveTracking { get; private set; } = true;
    public decimal CommanderReservePercentage { get; private set; } = 10.0m;
    public bool EnableLowStockAlerts { get; private set; } = true;
    public decimal LowStockThresholdPercentage { get; private set; } = 20.0m;
    public bool EnableReorderAlerts { get; private set; } = true;
    public bool EnableAutoReorder { get; private set; } = false;
    public bool EnableExpiryAlerts { get; private set; } = true;
    public int ExpiryAlertDays { get; private set; } = 30;
    public bool EnableBarcodeScanning { get; private set; } = true;
    public bool EnableQrCodeGeneration { get; private set; } = true;

    // Backup & Maintenance
    public bool EnableDataBackup { get; private set; } = true;
    public string BackupSchedule { get; private set; } = "0 0 * * *"; // Daily at midnight
    public int BackupRetentionDays { get; private set; } = 30;
    public bool EnableDataArchiving { get; private set; } = true;
    public int ArchiveAfterDays { get; private set; } = 365;
    public bool MaintenanceMode { get; private set; } = false;
    public string MaintenanceMessage { get; private set; } = "System is under maintenance. Please try again later.";
    public string MaintenanceMessageArabic { get; private set; } = "النظام تت الصيانة. يرجى المحاولة لاحقاً.";

    private SystemSettings() { }

    public SystemSettings(
        string systemName,
        string systemNameArabic,
        string defaultLanguage,
        string defaultCurrency,
        string dateFormat,
        bool enableNotifications,
        int createdBy) : base(createdBy)
    {
        SystemName = systemName;
        SystemNameArabic = systemNameArabic;
        DefaultLanguage = defaultLanguage;
        DefaultCurrency = defaultCurrency;
        DateFormat = dateFormat;
        EnableNotifications = enableNotifications;
    }
}
