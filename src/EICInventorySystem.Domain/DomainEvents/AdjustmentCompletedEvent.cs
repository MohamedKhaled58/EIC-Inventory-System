namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an adjustment is completed
/// </summary>
public class AdjustmentCompletedEvent : DomainEvent
{
    public int AdjustmentId { get; }
    public string AdjustmentNumber { get; }
    public int WarehouseId { get; }
    public string Type { get; }
    public decimal TotalAdjustment { get; }
    public decimal TotalValue { get; }
    public DateTime AdjustmentDate { get; }
    public int ApproverId { get; }

    public AdjustmentCompletedEvent(
        int adjustmentId,
        string adjustmentNumber,
        int warehouseId,
        string type,
        decimal totalAdjustment,
        decimal totalValue,
        DateTime adjustmentDate,
        int approverId)
    {
        AdjustmentId = adjustmentId;
        AdjustmentNumber = adjustmentNumber;
        WarehouseId = warehouseId;
        Type = type;
        TotalAdjustment = totalAdjustment;
        TotalValue = totalValue;
        AdjustmentDate = adjustmentDate;
        ApproverId = approverId;
    }
}
