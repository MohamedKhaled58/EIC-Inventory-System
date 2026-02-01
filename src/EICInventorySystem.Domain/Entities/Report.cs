namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Represents a generated report
/// </summary>
public class Report : BaseEntity
{
    public string ReportNumber { get; private set; } = string.Empty;
    public ReportType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string TitleArabic { get; private set; } = string.Empty;
    public ReportStatus Status { get; private set; } = ReportStatus.Pending;
    public DateTime GeneratedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int CreatedByUserId { get; private set; }
    public int? WarehouseId { get; private set; }
    public int? FactoryId { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Parameters { get; private set; }
    public string? ResultPath { get; private set; }
    public string FileFormat { get; private set; } = "PDF";
    public long? FileSizeBytes { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navigation properties
    public User? CreatedByUser { get; private set; }
    public Warehouse? Warehouse { get; private set; }
    public Factory? Factory { get; private set; }

    private Report() { }

    public Report(
        string reportNumber,
        ReportType type,
        string title,
        string titleArabic,
        int createdByUserId,
        int? warehouseId = null,
        int? factoryId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? parameters = null,
        string fileFormat = "PDF") : base(createdByUserId)
    {
        ReportNumber = reportNumber;
        Type = type;
        Title = title;
        TitleArabic = titleArabic;
        CreatedByUserId = createdByUserId;
        WarehouseId = warehouseId;
        FactoryId = factoryId;
        StartDate = startDate;
        EndDate = endDate;
        Parameters = parameters;
        FileFormat = fileFormat;
        Status = ReportStatus.Pending;
        GeneratedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(string resultPath, long fileSizeBytes)
    {
        Status = ReportStatus.Completed;
        ResultPath = resultPath;
        FileSizeBytes = fileSizeBytes;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessing()
    {
        Status = ReportStatus.Processing;
    }
}

public enum ReportType
{
    Inventory,
    StockMovement,
    Requisition,
    Transfer,
    CommanderReserve,
    Consumption,
    Receipt,
    Audit,
    Summary
}

public enum ReportStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
