namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a return
/// </summary>
public class ReturnItem : BaseEntity
{
    public int ReturnId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Reason { get; private set; }
    public string? ReasonArabic { get; private set; }
    public string Condition { get; private set; } = null!; // "Good", "Damaged", "Used", "Defective"
    public bool IsToCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }

    // Navigation properties
    public Return Return { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ReturnItem() { }

    public ReturnItem(
        int returnId,
        int itemId,
        decimal quantity,
        decimal unitPrice,
        string condition,
        int createdBy,
        string? reason = null,
        string? reasonArabic = null,
        bool isToCommanderReserve = false,
        decimal commanderReserveQuantity = 0) : base(createdBy)
    {
        ReturnId = returnId;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Reason = reason;
        ReasonArabic = reasonArabic;
        Condition = condition;
        IsToCommanderReserve = isToCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
    }

    public void UpdateQuantity(decimal quantity, decimal unitPrice, int updatedBy)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        Update(updatedBy);
    }

    public void MarkAsToCommanderReserve(decimal reserveQuantity, int updatedBy)
    {
        IsToCommanderReserve = true;
        CommanderReserveQuantity = reserveQuantity;
        Update(updatedBy);
    }
}
