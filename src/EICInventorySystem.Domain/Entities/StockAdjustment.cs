namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Stock adjustment records for inventory corrections
/// </summary>
public class StockAdjustment
{
    public int Id { get; private set; }
    public int InventoryRecordId { get; private set; }
    public int ItemId { get; private set; }
    public int WarehouseId { get; private set; }
    public decimal PreviousQuantity { get; private set; }
    public decimal NewQuantity { get; private set; }
    public decimal AdjustmentAmount { get; private set; }
    public AdjustmentType AdjustmentType { get; private set; }
    public string? AffectedStockType { get; private set; } // "General", "Reserve", "Both"
    public DateTime AdjustmentDate { get; private set; }
    public int AdjustedBy { get; private set; }
    public string? Reason { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public bool RequiresApproval { get; private set; }
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }

    // Navigation properties
    public InventoryRecord InventoryRecord { get; private set; } = null!;
    public Item Item { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;

    private StockAdjustment() { }

    public StockAdjustment(
        int inventoryRecordId,
        int itemId,
        int warehouseId,
        decimal previousQuantity,
        decimal newQuantity,
        AdjustmentType adjustmentType,
        int adjustedBy,
        string? reason = null,
        string? referenceNumber = null,
        string? affectedStockType = null,
        bool requiresApproval = false)
    {
        InventoryRecordId = inventoryRecordId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        PreviousQuantity = previousQuantity;
        NewQuantity = newQuantity;
        AdjustmentAmount = newQuantity - previousQuantity;
        AdjustmentType = adjustmentType;
        AffectedStockType = affectedStockType ?? "Both";
        AdjustmentDate = DateTime.UtcNow;
        AdjustedBy = adjustedBy;
        Reason = reason;
        ReferenceNumber = referenceNumber;
        RequiresApproval = requiresApproval;
    }

    public void Approve(int approvedBy)
    {
        if (!RequiresApproval)
            throw new InvalidOperationException("This adjustment does not require approval");

        ApprovedBy = approvedBy;
        ApprovalDate = DateTime.UtcNow;
    }
}
