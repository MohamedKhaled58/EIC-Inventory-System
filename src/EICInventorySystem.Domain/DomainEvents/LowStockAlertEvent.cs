namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when stock is low
/// </summary>
public class LowStockAlertEvent : DomainEvent
{
    public int InventoryRecordId { get; }
    public int ItemId { get; }
    public int WarehouseId { get; }
    public decimal CurrentQuantity { get; }
    public decimal ReorderPoint { get; }
    public bool IsCritical { get; }
    public bool IsReserveLow { get; }
    public decimal CommanderReserveQuantity { get; }
    public decimal MinimumReserveRequired { get; }

    public LowStockAlertEvent(
        int inventoryRecordId,
        int itemId,
        int warehouseId,
        decimal currentQuantity,
        decimal reorderPoint,
        bool isCritical,
        bool isReserveLow,
        decimal commanderReserveQuantity,
        decimal minimumReserveRequired)
    {
        InventoryRecordId = inventoryRecordId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        CurrentQuantity = currentQuantity;
        ReorderPoint = reorderPoint;
        IsCritical = isCritical;
        IsReserveLow = isReserveLow;
        CommanderReserveQuantity = commanderReserveQuantity;
        MinimumReserveRequired = minimumReserveRequired;
    }
}
