namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a receipt
/// </summary>
public class ReceiptItem : BaseEntity
{
    public int ReceiptId { get; private set; }
    public int ItemId { get; private set; }
    public decimal OrderedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public decimal GeneralQuantity { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    public string? BatchNumber { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Receipt Receipt { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ReceiptItem() { }

    public ReceiptItem(
        int receiptId,
        int itemId,
        decimal orderedQuantity,
        decimal unitPrice,
        int createdBy,
        decimal generalQuantity = 0,
        decimal commanderReserveQuantity = 0,
        string? batchNumber = null,
        DateTime? expiryDate = null,
        string? notes = null) : base(createdBy)
    {
        ReceiptId = receiptId;
        ItemId = itemId;
        OrderedQuantity = orderedQuantity;
        ReceivedQuantity = 0;
        UnitPrice = unitPrice;
        TotalValue = orderedQuantity * unitPrice;
        GeneralQuantity = generalQuantity;
        CommanderReserveQuantity = commanderReserveQuantity;
        BatchNumber = batchNumber;
        ExpiryDate = expiryDate;
        Notes = notes;
    }

    public void Receive(decimal quantity, decimal generalQuantity, decimal commanderReserveQuantity, int updatedBy)
    {
        if (quantity > (OrderedQuantity - ReceivedQuantity))
            throw new InvalidOperationException("Cannot receive more than ordered quantity");

        ReceivedQuantity += quantity;
        GeneralQuantity += generalQuantity;
        CommanderReserveQuantity += commanderReserveQuantity;
        TotalValue = ReceivedQuantity * UnitPrice;
        Update(updatedBy);
    }

    public void UpdateQuantity(decimal orderedQuantity, decimal unitPrice, int updatedBy)
    {
        OrderedQuantity = orderedQuantity;
        UnitPrice = unitPrice;
        TotalValue = orderedQuantity * unitPrice;
        Update(updatedBy);
    }

    public decimal GetRemainingQuantity()
    {
        return OrderedQuantity - ReceivedQuantity;
    }

    public bool IsFullyReceived()
    {
        return ReceivedQuantity >= OrderedQuantity;
    }
}
