namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a transfer
/// </summary>
public class TransferItem : BaseEntity
{
    public int TransferId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ShippedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Notes { get; private set; }
    public bool IsFromCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }

    // Navigation properties
    public Transfer Transfer { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private TransferItem() { }

    public TransferItem(
        int transferId,
        int itemId,
        decimal quantity,
        decimal unitPrice,
        int createdBy,
        string? notes = null,
        bool isFromCommanderReserve = false,
        decimal commanderReserveQuantity = 0) : base(createdBy)
    {
        TransferId = transferId;
        ItemId = itemId;
        Quantity = quantity;
        ShippedQuantity = 0;
        ReceivedQuantity = 0;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Notes = notes;
        IsFromCommanderReserve = isFromCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
    }

    public void Ship(decimal quantity, int updatedBy)
    {
        if (quantity > (Quantity - ShippedQuantity))
            throw new InvalidOperationException("Cannot ship more than requested quantity");

        ShippedQuantity += quantity;
        Update(updatedBy);
    }

    public void Receive(decimal quantity, int updatedBy)
    {
        if (quantity > (ShippedQuantity - ReceivedQuantity))
            throw new InvalidOperationException("Cannot receive more than shipped quantity");

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

    public void MarkAsFromCommanderReserve(decimal reserveQuantity, int updatedBy)
    {
        IsFromCommanderReserve = true;
        CommanderReserveQuantity = reserveQuantity;
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
