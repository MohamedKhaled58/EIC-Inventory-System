namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a purchase order is created
/// </summary>
public class PurchaseOrderCreatedEvent : DomainEvent
{
    public int PurchaseOrderId { get; }
    public string PurchaseOrderNumber { get; }
    public int SupplierId { get; }
    public int? WarehouseId { get; }
    public decimal TotalQuantity { get; }
    public decimal TotalValue { get; }
    public DateTime OrderDate { get; }
    public DateTime ExpectedDeliveryDate { get; }
    public int CreatedBy { get; }

    public PurchaseOrderCreatedEvent(
        int purchaseOrderId,
        string purchaseOrderNumber,
        int supplierId,
        int? warehouseId,
        decimal totalQuantity,
        decimal totalValue,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        int createdBy)
    {
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNumber = purchaseOrderNumber;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        CreatedBy = createdBy;
    }
}
