using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EICInventorySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(ILogger<SettingsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get user settings
    /// </summary>
    [HttpGet("user")]
    public ActionResult<UserSettingsDto> GetUserSettings()
    {
        try
        {
            // Return default settings for now
            return Ok(new UserSettingsDto
            {
                Theme = "light",
                Language = "ar",
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                ItemsPerPage = 25,
                DashboardLayout = "default",
                ShowNotifications = true,
                SoundEnabled = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings");
            return StatusCode(500, new { message = "Error fetching user settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Update user settings
    /// </summary>
    [HttpPut("user")]
    public ActionResult<UserSettingsDto> UpdateUserSettings([FromBody] UserSettingsDto settings)
    {
        try
        {
            // For now, just echo back the settings
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user settings");
            return StatusCode(500, new { message = "Error updating user settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Get system settings (admin only)
    /// </summary>
    [HttpGet("system")]
    public ActionResult<SystemSettingsDto> GetSystemSettings()
    {
        try
        {
            return Ok(new SystemSettingsDto
            {
                CompanyName = "EIC - Engineering Industrial Complex",
                CompanyNameAr = "مجمع الصناعات الهندسية",
                DefaultWarehouse = 1,
                LowStockThreshold = 10,
                CriticalStockThreshold = 5,
                ReorderLeadTimeDays = 7,
                MaxReserveDays = 30,
                AllowNegativeInventory = false,
                RequireApprovalForTransfers = true,
                RequireApprovalForRequisitions = true,
                AutoGenerateRequisitionNumber = true,
                AutoGenerateTransferNumber = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system settings");
            return StatusCode(500, new { message = "Error fetching system settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Update system settings (admin only)
    /// </summary>
    [HttpPut("system")]
    [Authorize(Roles = "ComplexCommander")]
    public ActionResult<SystemSettingsDto> UpdateSystemSettings([FromBody] SystemSettingsDto settings)
    {
        try
        {
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating system settings");
            return StatusCode(500, new { message = "Error updating system settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notification settings
    /// </summary>
    [HttpGet("notifications")]
    public ActionResult<NotificationSettingsDto> GetNotificationSettings()
    {
        try
        {
            return Ok(new NotificationSettingsDto
            {
                EmailNotifications = true,
                LowStockAlerts = true,
                TransferStatusUpdates = true,
                RequisitionStatusUpdates = true,
                DailyReportEmail = false,
                WeeklyReportEmail = true,
                NotificationEmail = ""
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification settings");
            return StatusCode(500, new { message = "Error fetching notification settings", error = ex.Message });
        }
    }

    /// <summary>
    /// Update notification settings
    /// </summary>
    [HttpPut("notifications")]
    public ActionResult<NotificationSettingsDto> UpdateNotificationSettings([FromBody] NotificationSettingsDto settings)
    {
        try
        {
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification settings");
            return StatusCode(500, new { message = "Error updating notification settings", error = ex.Message });
        }
    }
}

public record UserSettingsDto
{
    public string Theme { get; init; } = "light";
    public string Language { get; init; } = "ar";
    public string DateFormat { get; init; } = "dd/MM/yyyy";
    public string TimeFormat { get; init; } = "HH:mm";
    public int ItemsPerPage { get; init; } = 25;
    public string DashboardLayout { get; init; } = "default";
    public bool ShowNotifications { get; init; } = true;
    public bool SoundEnabled { get; init; } = true;
}

public record SystemSettingsDto
{
    public string CompanyName { get; init; } = string.Empty;
    public string CompanyNameAr { get; init; } = string.Empty;
    public int DefaultWarehouse { get; init; }
    public int LowStockThreshold { get; init; }
    public int CriticalStockThreshold { get; init; }
    public int ReorderLeadTimeDays { get; init; }
    public int MaxReserveDays { get; init; }
    public bool AllowNegativeInventory { get; init; }
    public bool RequireApprovalForTransfers { get; init; }
    public bool RequireApprovalForRequisitions { get; init; }
    public bool AutoGenerateRequisitionNumber { get; init; }
    public bool AutoGenerateTransferNumber { get; init; }
}

public record NotificationSettingsDto
{
    public bool EmailNotifications { get; init; }
    public bool LowStockAlerts { get; init; }
    public bool TransferStatusUpdates { get; init; }
    public bool RequisitionStatusUpdates { get; init; }
    public bool DailyReportEmail { get; init; }
    public bool WeeklyReportEmail { get; init; }
    public string NotificationEmail { get; init; } = string.Empty;
}
