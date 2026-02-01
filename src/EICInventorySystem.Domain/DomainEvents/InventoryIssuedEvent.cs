namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when inventory is issued
/// </summary>
public class InventoryIssuedEvent : DomainEvent
{
    public int InventoryRecordId { get; }
    public int ItemId { get; }
    public int WarehouseId { get; }
    public decimal Quantity { get; }
    public decimal GeneralQuantity { get; }
    public decimal CommanderReserveQuantity { get; }
    public string ReferenceNumber { get; }
    public int UserId { get; }
    public bool IsFromCommanderReserve { get; }

    public InventoryIssuedEvent(
        int inventoryRecordId,
        int itemId,
        int warehouseId,
        decimal quantity,
        decimal generalQuantity,
        decimal commanderReserveQuantity,
        string referenceNumber,
        int userId,
        bool isFromCommanderReserve = false)
    {
        InventoryRecordId = inventoryRecordId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        GeneralQuantity = generalQuantity;
        CommanderReserveQuantity = commanderReserveQuantity;
        ReferenceNumber = referenceNumber;
        UserId = userId;
        IsFromCommanderReserve = isFromCommanderReserve;
    }
}
