namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// System-wide configuration settings
/// </summary>
public class SystemConfiguration
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Category { get; private set; } = null!; // "General", "Inventory", "Security", "Notifications"
    public string DataType { get; private set; } = null!; // "String", "Int", "Decimal", "Boolean", "Json"
    public string? DescriptionAr { get; private set; }
    public bool IsEncrypted { get; private set; }
    public bool IsReadOnly { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public int? UpdatedBy { get; private set; }

    private SystemConfiguration() { }

    public SystemConfiguration(
        string key,
        string value,
        string category,
        string dataType,
        string? description,
        string? descriptionAr,
        bool isEncrypted,
        bool isReadOnly)
    {
        Key = key;
        Value = value;
        Category = category;
        DataType = dataType;
        Description = description ?? string.Empty;
        DescriptionAr = descriptionAr;
        IsEncrypted = isEncrypted;
        IsReadOnly = isReadOnly;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateValue(string value, int updatedBy)
    {
        Value = value;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public T GetValue<T>()
    {
        if (DataType == "String")
            return (T)(object)Value;
        if (DataType == "Int")
            return (T)(object)int.Parse(Value);
        if (DataType == "Decimal")
            return (T)(object)decimal.Parse(Value);
        if (DataType == "Boolean")
            return (T)(object)bool.Parse(Value);
        if (DataType == "Json")
            return System.Text.Json.JsonSerializer.Deserialize<T>(Value)!;
        
        throw new InvalidOperationException($"Unsupported data type: {DataType}");
    }


    // Predefined configuration keys
    public static class Keys
    {
        // Inventory Settings
        public const string DEFAULT_RESERVE_PERCENTAGE = "INVENTORY.DEFAULT_RESERVE_PERCENTAGE";
        public const string MINIMUM_RESERVE_PERCENTAGE = "INVENTORY.MINIMUM_RESERVE_PERCENTAGE";
        public const string AUTO_ALLOCATE_RESERVE = "INVENTORY.AUTO_ALLOCATE_RESERVE";
        public const string ENABLE_LOW_STOCK_ALERTS = "INVENTORY.ENABLE_LOW_STOCK_ALERTS";
        public const string LOW_STOCK_THRESHOLD_PERCENTAGE = "INVENTORY.LOW_STOCK_THRESHOLD_PERCENTAGE";
        public const string ENABLE_CRITICAL_STOCK_ALERTS = "INVENTORY.ENABLE_CRITICAL_STOCK_ALERTS";
        public const string CRITICAL_STOCK_THRESHOLD_PERCENTAGE = "INVENTORY.CRITICAL_STOCK_THRESHOLD_PERCENTAGE";

        // Requisition Settings
        public const string REQUISITION_AUTO_APPROVE_THRESHOLD = "REQUISITION.AUTO_APPROVE_THRESHOLD";
        public const string REQUISITION_EXPIRY_DAYS = "REQUISITION.EXPIRY_DAYS";
        public const string REQUISITION_REQUIRE_JUSTIFICATION = "REQUISITION.REQUIRE_JUSTIFICATION";
        public const string REQUISITION_MAX_ITEMS = "REQUISITION.MAX_ITEMS";

        // Transfer Settings
        public const string TRANSFER_AUTO_APPROVE_THRESHOLD = "TRANSFER.AUTO_APPROVE_THRESHOLD";
        public const string TRANSFER_EXPIRY_DAYS = "TRANSFER.EXPIRY_DAYS";
        public const string TRANSFER_REQUIRE_JUSTIFICATION = "TRANSFER.REQUIRE_JUSTIFICATION";

        // Project Settings
        public const string PROJECT_DEADLINE_WARNING_DAYS = "PROJECT.DEADLINE_WARNING_DAYS";
        public const string PROJECT_OVERDUE_ALERT_DAYS = "PROJECT.OVERDUE_ALERT_DAYS";
        public const string PROJECT_BUDGET_WARNING_PERCENTAGE = "PROJECT.BUDGET_WARNING_PERCENTAGE";

        // Security Settings
        public const string PASSWORD_MIN_LENGTH = "SECURITY.PASSWORD_MIN_LENGTH";
        public const string PASSWORD_MAX_AGE_DAYS = "SECURITY.PASSWORD_MAX_AGE_DAYS";
        public const string PASSWORD_HISTORY_COUNT = "SECURITY.PASSWORD_HISTORY_COUNT";
        public const string MAX_LOGIN_ATTEMPTS = "SECURITY.MAX_LOGIN_ATTEMPTS";
        public const string LOCKOUT_DURATION_MINUTES = "SECURITY.LOCKOUT_DURATION_MINUTES";
        public const string SESSION_TIMEOUT_MINUTES = "SECURITY.SESSION_TIMEOUT_MINUTES";
        public const string ENABLE_TWO_FACTOR_AUTH = "SECURITY.ENABLE_TWO_FACTOR_AUTH";
        public const string REQUIRE_COMMANDER_APPROVAL_FOR_RESERVE = "SECURITY.REQUIRE_COMMANDER_APPROVAL_FOR_RESERVE";

        // Notification Settings
        public const string ENABLE_EMAIL_NOTIFICATIONS = "NOTIFICATION.ENABLE_EMAIL_NOTIFICATIONS";
        public const string ENABLE_SMS_NOTIFICATIONS = "NOTIFICATION.ENABLE_SMS_NOTIFICATIONS";
        public const string ENABLE_PUSH_NOTIFICATIONS = "NOTIFICATION.ENABLE_PUSH_NOTIFICATIONS";
        public const string NOTIFICATION_RETENTION_DAYS = "NOTIFICATION.RETENTION_DAYS";
        public const string EMAIL_SMTP_SERVER = "NOTIFICATION.EMAIL_SMTP_SERVER";
        public const string EMAIL_SMTP_PORT = "NOTIFICATION.EMAIL_SMTP_PORT";
        public const string EMAIL_SMTP_USERNAME = "NOTIFICATION.EMAIL_SMTP_USERNAME";
        public const string EMAIL_SMTP_PASSWORD = "NOTIFICATION.EMAIL_SMTP_PASSWORD";
        public const string EMAIL_FROM_ADDRESS = "NOTIFICATION.EMAIL_FROM_ADDRESS";
        public const string EMAIL_FROM_NAME = "NOTIFICATION.EMAIL_FROM_NAME";

        // Audit Settings
        public const string AUDIT_LOG_RETENTION_DAYS = "AUDIT.RETENTION_DAYS";
        public const string AUDIT_LOG_LEVEL = "AUDIT.LOG_LEVEL"; // DEBUG, INFO, WARNING, ERROR
        public const string AUDIT_LOG_SENSITIVE_DATA = "AUDIT.LOG_SENSITIVE_DATA";

        // System Settings
        public const string SYSTEM_NAME = "SYSTEM.NAME";
        public const string SYSTEM_NAME_AR = "SYSTEM.NAME_AR";
        public const string SYSTEM_VERSION = "SYSTEM.VERSION";
        public const string SYSTEM_MAINTENANCE_MODE = "SYSTEM.MAINTENANCE_MODE";
        public const string SYSTEM_TIMEZONE = "SYSTEM.TIMEZONE";
        public const string SYSTEM_DATE_FORMAT = "SYSTEM.DATE_FORMAT";
        public const string SYSTEM_DEFAULT_LANGUAGE = "SYSTEM.DEFAULT_LANGUAGE";
    }

    // Default configuration values
    public static class Defaults
    {
        public static readonly Dictionary<string, (string Value, string DataType, string? Description, string? DescriptionAr)> DefaultValues = new()
        {
            // Inventory Defaults
            { Keys.DEFAULT_RESERVE_PERCENTAGE, ("25", "INTEGER", "Default percentage of items allocated to Commander's Reserve", "النسبة الافتراضية للصنف المخصص لاحتياطي القائد") },
            { Keys.MINIMUM_RESERVE_PERCENTAGE, ("15", "INTEGER", "Minimum percentage of items that must be in Commander's Reserve", "الحد الأدنى للنسبة المئوية للصنف الذي يجب أن يكون في احتياطي القائد") },
            { Keys.AUTO_ALLOCATE_RESERVE, ("true", "BOOLEAN", "Automatically allocate items to Commander's Reserve on receipt", "تخصيص الصنف تلقائياً لاحتياطي القائد عند الاستلام") },
            { Keys.ENABLE_LOW_STOCK_ALERTS, ("true", "BOOLEAN", "Enable low stock alert notifications", "تمكين تنبيهات انخفاض المخزون") },
            { Keys.LOW_STOCK_THRESHOLD_PERCENTAGE, ("20", "INTEGER", "Percentage below reorder point to trigger low stock alert", "النسبة المئوية أقل من نقطة إعادة الطلب لتشغيل تنبيه انخفاض المخزون") },
            { Keys.ENABLE_CRITICAL_STOCK_ALERTS, ("true", "BOOLEAN", "Enable critical stock alert notifications", "تمكين تنبيهات المخزون الحرج") },
            { Keys.CRITICAL_STOCK_THRESHOLD_PERCENTAGE, ("10", "INTEGER", "Percentage below reorder point to trigger critical stock alert", "النسبة المئوية أقل من نقطة إعادة الطلب لتشغيل تنبيه المخزون الحرج") },

            // Requisition Defaults
            { Keys.REQUISITION_AUTO_APPROVE_THRESHOLD, ("10000", "DECIMAL", "Maximum value for auto-approval of requisitions", "الحد الأقصى للقيمة للموافقة التلقائية على طلبات الصرف") },
            { Keys.REQUISITION_EXPIRY_DAYS, ("30", "INTEGER", "Number of days before a requisition expires", "عدد الأيام قبل انتهاء صلاحية طلب الصرف") },
            { Keys.REQUISITION_REQUIRE_JUSTIFICATION, ("true", "BOOLEAN", "Require justification for requisitions above threshold", "يتطلب مبرراً لطلبات الصرف فوق الحد") },
            { Keys.REQUISITION_MAX_ITEMS, ("50", "INTEGER", "Maximum number of items per requisition", "الحد الأقصى لعدد الأصناف لكل طلب صرف") },

            // Transfer Defaults
            { Keys.TRANSFER_AUTO_APPROVE_THRESHOLD, ("50000", "DECIMAL", "Maximum value for auto-approval of transfers", "الحد الأقصى للقيمة للموافقة التلقائية على التحويلات") },
            { Keys.TRANSFER_EXPIRY_DAYS, ("14", "INTEGER", "Number of days before a transfer request expires", "عدد الأيام قبل انتهاء صلاحية طلب التحويل") },
            { Keys.TRANSFER_REQUIRE_JUSTIFICATION, ("true", "BOOLEAN", "Require justification for transfers above threshold", "يتطلب مبرراً للتحويلات فوق الحد") },

            // Project Defaults
            { Keys.PROJECT_DEADLINE_WARNING_DAYS, ("7", "INTEGER", "Days before deadline to send warning notification", "الأيام قبل الموعد النهائي لإرسال تنبيه التحذير") },
            { Keys.PROJECT_OVERDUE_ALERT_DAYS, ("1", "INTEGER", "Days after deadline to send overdue alert", "الأيام بعد الموعد النهائي لإرسال تنبيه التأخير") },
            { Keys.PROJECT_BUDGET_WARNING_PERCENTAGE, ("80", "INTEGER", "Budget usage percentage to trigger warning", "نسبة استخدام الميزانية لتشغيل التحذير") },

            // Security Defaults
            { Keys.PASSWORD_MIN_LENGTH, ("8", "INTEGER", "Minimum password length", "الحد الأدنى لطول كلمة المرور") },
            { Keys.PASSWORD_MAX_AGE_DAYS, ("90", "INTEGER", "Maximum password age in days", "الحد الأقصى لعمر كلمة المرور بالأيام") },
            { Keys.PASSWORD_HISTORY_COUNT, ("5", "INTEGER", "Number of previous passwords to remember", "عدد كلمات المرور السابقة التي يجب تذكرها") },
            { Keys.MAX_LOGIN_ATTEMPTS, ("5", "INTEGER", "Maximum failed login attempts before lockout", "الحد الأقصى لمحاولات تسجيل الدخول الفاشلة قبل القفل") },
            { Keys.LOCKOUT_DURATION_MINUTES, ("30", "INTEGER", "Lockout duration in minutes", "مدة القفل بالدقائق") },
            { Keys.SESSION_TIMEOUT_MINUTES, ("60", "INTEGER", "Session timeout in minutes", "مهلة الجلسة بالدقائق") },
            { Keys.ENABLE_TWO_FACTOR_AUTH, ("false", "BOOLEAN", "Enable two-factor authentication", "تمكين المصادقة الثنائية") },
            { Keys.REQUIRE_COMMANDER_APPROVAL_FOR_RESERVE, ("true", "BOOLEAN", "Require commander approval for reserve releases", "يتطلب موافقة القائد لإصدار الاحتياطي") },

            // Notification Defaults
            { Keys.ENABLE_EMAIL_NOTIFICATIONS, ("true", "BOOLEAN", "Enable email notifications", "تمكين إشعارات البريد الإلكتروني") },
            { Keys.ENABLE_SMS_NOTIFICATIONS, ("false", "BOOLEAN", "Enable SMS notifications", "تمكين إشعارات الرسائل القصيرة") },
            { Keys.ENABLE_PUSH_NOTIFICATIONS, ("true", "BOOLEAN", "Enable push notifications", "تمكين الإشعارات الفورية") },
            { Keys.NOTIFICATION_RETENTION_DAYS, ("90", "INTEGER", "Number of days to retain notifications", "عدد الأيام للاحتفاظ بالإشعارات") },
            { Keys.EMAIL_SMTP_SERVER, ("", "STRING", "SMTP server for email notifications", "خادم SMTP لإشعارات البريد الإلكتروني") },
            { Keys.EMAIL_SMTP_PORT, ("587", "INTEGER", "SMTP port for email notifications", "منفذ SMTP لإشعارات البريد الإلكتروني") },
            { Keys.EMAIL_SMTP_USERNAME, ("", "STRING", "SMTP username for email notifications", "اسم مستخدم SMTP لإشعارات البريد الإلكتروني") },
            { Keys.EMAIL_SMTP_PASSWORD, ("", "STRING", "SMTP password for email notifications", "كلمة مرور SMTP لإشعارات البريد الإلكتروني") },
            { Keys.EMAIL_FROM_ADDRESS, ("noreply@eic.mil", "STRING", "From address for email notifications", "عنوان المرسل لإشعارات البريد الإلكتروني") },
            { Keys.EMAIL_FROM_NAME, ("EIC Inventory System", "STRING", "From name for email notifications", "اسم المرسل لإشعارات البريد الإلكتروني") },

            // Audit Defaults
            { Keys.AUDIT_LOG_RETENTION_DAYS, ("365", "INTEGER", "Number of days to retain audit logs", "عدد الأيام للاحتفاظ بسجلات التدقيق") },
            { Keys.AUDIT_LOG_LEVEL, ("INFO", "STRING", "Audit log level (DEBUG, INFO, WARNING, ERROR)", "مستوى سجل التدقيق") },
            { Keys.AUDIT_LOG_SENSITIVE_DATA, ("false", "BOOLEAN", "Log sensitive data in audit trails", "تسجيل البيانات الحساسة في سجلات التدقيق") },

            // System Defaults
            { Keys.SYSTEM_NAME, ("EIC Inventory Command System", "STRING", "System name", "اسم النظام") },
            { Keys.SYSTEM_NAME_AR, ("نظام إدارة مخازن مجمع الصناعات الهندسية", "STRING", "System name in Arabic", "اسم النظام بالعربية") },
            { Keys.SYSTEM_VERSION, ("1.0.0", "STRING", "System version", "إصدار النظام") },
            { Keys.SYSTEM_MAINTENANCE_MODE, ("false", "BOOLEAN", "System maintenance mode", "وضع صيانة النظام") },
            { Keys.SYSTEM_TIMEZONE, ("Africa/Cairo", "STRING", "System timezone", "المنطقة الزمنية للنظام") },
            { Keys.SYSTEM_DATE_FORMAT, ("yyyy-MM-dd", "STRING", "System date format", "تنسيق تاريخ النظام") },
            { Keys.SYSTEM_DEFAULT_LANGUAGE, ("ar", "STRING", "System default language", "اللغة الافتراضية للنظام") }
        };
    }

    public static SystemConfiguration CreateDefault(string key)
    {
        if (!Defaults.DefaultValues.TryGetValue(key, out var config))
            throw new ArgumentException($"No default configuration found for key: {key}");

        return new SystemConfiguration(
            key,
            config.Value,
            GetCategoryFromKey(key),
            config.DataType,
            config.Description,
            config.DescriptionAr,
            IsKeyEncrypted(key),
            IsKeyReadOnly(key));
    }

    private static string GetCategoryFromKey(string key)
    {
        if (key.StartsWith("INVENTORY.")) return "INVENTORY";
        if (key.StartsWith("REQUISITION.")) return "REQUISITION";
        if (key.StartsWith("TRANSFER.")) return "TRANSFER";
        if (key.StartsWith("PROJECT.")) return "PROJECT";
        if (key.StartsWith("SECURITY.")) return "SECURITY";
        if (key.StartsWith("NOTIFICATION.")) return "NOTIFICATION";
        if (key.StartsWith("AUDIT.")) return "AUDIT";
        if (key.StartsWith("SYSTEM.")) return "SYSTEM";
        return "GENERAL";
    }

    private static bool IsKeyEncrypted(string key)
    {
        return key.Contains("PASSWORD") || key.Contains("SECRET") || key.Contains("TOKEN");
    }

    private static bool IsKeyReadOnly(string key)
    {
        return key == Keys.SYSTEM_VERSION;
    }
}
