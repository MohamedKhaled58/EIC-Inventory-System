namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a purchase order
/// </summary>
public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ExpectedDeliveryDate { get; private set; }

    // Navigation properties
    public PurchaseOrder PurchaseOrder { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private PurchaseOrderItem() { }

    public PurchaseOrderItem(
        int purchaseOrderId,
        int itemId,
        decimal quantity,
        decimal unitPrice,
        int createdBy,
        string? notes = null,
        DateTime? expectedDeliveryDate = null) : base(createdBy)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemId = itemId;
        Quantity = quantity;
        ReceivedQuantity = 0;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Notes = notes;
        ExpectedDeliveryDate = expectedDeliveryDate;
    }

    public void Receive(decimal quantity, int updatedBy)
    {
        if (quantity > (Quantity - ReceivedQuantity))
            throw new InvalidOperationException("Cannot receive more than ordered quantity");

        ReceivedQuantity += quantity;
        Update(updatedBy);
    }

    public void UpdateQuantity(decimal quantity, decimal unitPrice, int updatedBy)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Update(updatedBy);
    }

    public decimal GetRemainingQuantity()
    {
        return Quantity - ReceivedQuantity;
    }

    public bool IsFullyReceived()
    {
        return ReceivedQuantity >= Quantity;
    }
}
