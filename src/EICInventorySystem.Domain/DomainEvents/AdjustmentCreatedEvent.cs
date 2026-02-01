using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an inventory adjustment is created
/// </summary>
public class AdjustmentCreatedEvent : DomainEvent
{
    public int AdjustmentId { get; }
    public string AdjustmentNumber { get; }
    public int WarehouseId { get; }
    public AdjustmentType AdjustmentType { get; }

    public AdjustmentCreatedEvent(
        int adjustmentId,
        string adjustmentNumber,
        int warehouseId,
        AdjustmentType adjustmentType)
    {
        AdjustmentId = adjustmentId;
        AdjustmentNumber = adjustmentNumber;
        WarehouseId = warehouseId;
        AdjustmentType = adjustmentType;
    }
}
