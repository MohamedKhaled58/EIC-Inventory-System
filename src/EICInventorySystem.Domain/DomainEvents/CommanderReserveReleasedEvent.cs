namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when Commander's Reserve is released
/// </summary>
public class CommanderReserveReleasedEvent : DomainEvent
{
    public int InventoryRecordId { get; }
    public int ItemId { get; }
    public int WarehouseId { get; }
    public decimal Quantity { get; }
    public string ReferenceNumber { get; }
    public int CommanderId { get; }
    public int RequesterId { get; }
    public string Reason { get; }

    public CommanderReserveReleasedEvent(
        int inventoryRecordId,
        int itemId,
        int warehouseId,
        decimal quantity,
        string referenceNumber,
        int commanderId,
        int requesterId,
        string reason)
    {
        InventoryRecordId = inventoryRecordId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        ReferenceNumber = referenceNumber;
        CommanderId = commanderId;
        RequesterId = requesterId;
        Reason = reason;
    }
}
