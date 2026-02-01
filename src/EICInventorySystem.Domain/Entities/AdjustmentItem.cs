namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within an adjustment
/// </summary>
public class AdjustmentItem : BaseEntity
{
    public int AdjustmentId { get; private set; }
    public int ItemId { get; private set; }
    public decimal CurrentQuantity { get; private set; }
    public decimal AdjustedQuantity { get; private set; }
    public decimal AdjustmentAmount { get; private set; } // Positive = increase, Negative = decrease
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Reason { get; private set; }
    public string? ReasonArabic { get; private set; }
    public bool IsGeneralStock { get; private set; }
    public bool IsCommanderReserve { get; private set; }
    public decimal GeneralAdjustment { get; private set; }
    public decimal ReserveAdjustment { get; private set; }

    // Navigation properties
    public Adjustment Adjustment { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private AdjustmentItem() { }

    public AdjustmentItem(
        int adjustmentId,
        int itemId,
        decimal currentQuantity,
        decimal adjustedQuantity,
        decimal unitPrice,
        int createdBy,
        string? reason = null,
        string? reasonArabic = null,
        bool isGeneralStock = true,
        bool isCommanderReserve = false,
        decimal generalAdjustment = 0,
        decimal reserveAdjustment = 0) : base(createdBy)
    {
        AdjustmentId = adjustmentId;
        ItemId = itemId;
        CurrentQuantity = currentQuantity;
        AdjustedQuantity = adjustedQuantity;
        AdjustmentAmount = adjustedQuantity - currentQuantity;
        UnitPrice = unitPrice;
        TotalValue = Math.Abs(AdjustmentAmount) * unitPrice;
        Reason = reason;
        ReasonArabic = reasonArabic;
        IsGeneralStock = isGeneralStock;
        IsCommanderReserve = isCommanderReserve;
        GeneralAdjustment = generalAdjustment;
        ReserveAdjustment = reserveAdjustment;
    }

    public void UpdateQuantity(decimal currentQuantity, decimal adjustedQuantity, decimal unitPrice, int updatedBy)
    {
        CurrentQuantity = currentQuantity;
        AdjustedQuantity = adjustedQuantity;
        AdjustmentAmount = adjustedQuantity - currentQuantity;
        UnitPrice = unitPrice;
        TotalValue = Math.Abs(AdjustmentAmount) * unitPrice;
        Update(updatedBy);
    }

    public bool IsIncrease()
    {
        return AdjustmentAmount > 0;
    }

    public bool IsDecrease()
    {
        return AdjustmentAmount < 0;
    }
}
