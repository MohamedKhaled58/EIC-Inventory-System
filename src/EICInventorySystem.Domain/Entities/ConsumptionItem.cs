namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a consumption record
/// </summary>
public class ConsumptionItem : BaseEntity
{
    public int ConsumptionId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Notes { get; private set; }
    public bool IsFromCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }

    // Navigation properties
    public Consumption Consumption { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ConsumptionItem() { }

    public ConsumptionItem(
        int consumptionId,
        int itemId,
        decimal quantity,
        decimal unitPrice,
        int createdBy,
        string? notes = null,
        bool isFromCommanderReserve = false,
        decimal commanderReserveQuantity = 0) : base(createdBy)
    {
        ConsumptionId = consumptionId;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Notes = notes;
        IsFromCommanderReserve = isFromCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
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
}
