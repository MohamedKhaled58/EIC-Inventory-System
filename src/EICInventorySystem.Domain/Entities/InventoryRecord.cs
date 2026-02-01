namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Inventory record tracking item quantities in a warehouse
/// 
/// Commander Reserve Logic:
/// - For CENTRAL warehouse: CommanderReserveQuantity = Complex Commander Reserve
/// - For FACTORY warehouse: CommanderReserveQuantity = Factory Commander Reserve
/// 
/// Reserve is NOT part of available stock for normal operations.
/// Reserve requires explicit Commander approval with justification.
/// </summary>
public class InventoryRecord : BaseEntity
{
    public int WarehouseId { get; private set; }
    public int ItemId { get; private set; }

    // Quantities
    public decimal TotalQuantity { get; private set; }
    public decimal GeneralQuantity { get; private set; }
    
    /// <summary>
    /// Commander Reserve Quantity
    /// For Central Warehouse: This is Complex Commander Reserve
    /// For Factory Warehouse: This is Factory Commander Reserve
    /// NOT available for normal daily operations
    /// </summary>
    public decimal CommanderReserveQuantity { get; private set; }

    // Allocations
    public decimal GeneralAllocated { get; private set; }
    public decimal ReserveAllocated { get; private set; }

    // Thresholds
    public decimal MinimumReserveRequired { get; private set; }
    public decimal ReorderPoint { get; private set; }

    // Computed properties (not stored in DB)
    /// <summary>
    /// Available for normal operations (General stock minus allocations)
    /// DOES NOT include Commander Reserve
    /// </summary>
    public decimal AvailableQuantity => GeneralQuantity - GeneralAllocated;
    
    /// <summary>
    /// Available reserve (requires Commander approval to use)
    /// </summary>
    public decimal AvailableReserve => CommanderReserveQuantity - ReserveAllocated;
    
    /// <summary>
    /// Total available including reserve (for reporting only, not for normal operations)
    /// </summary>
    public decimal TotalAvailable => AvailableQuantity + AvailableReserve;
    
    /// <summary>
    /// Alias for AvailableQuantity - used by BOQ and Custody services
    /// </summary>
    public decimal AvailableStock => AvailableQuantity;

    // Navigation properties
    public Warehouse Warehouse { get; private set; } = null!;
    public Item Item { get; private set; } = null!;
    public ICollection<InventoryTransaction> Transactions { get; private set; } = new List<InventoryTransaction>();

    private InventoryRecord() { }

    public InventoryRecord(
        int warehouseId,
        int itemId,
        decimal totalQuantity,
        decimal generalQuantity,
        decimal commanderReserveQuantity,
        decimal minimumReserveRequired,
        decimal reorderPoint,
        int createdBy) : base(createdBy)
    {
        WarehouseId = warehouseId;
        ItemId = itemId;
        TotalQuantity = totalQuantity;
        GeneralQuantity = generalQuantity;
        CommanderReserveQuantity = commanderReserveQuantity;
        GeneralAllocated = 0;
        ReserveAllocated = 0;
        MinimumReserveRequired = minimumReserveRequired;
        ReorderPoint = reorderPoint;
    }

    public void AddStock(decimal quantity, decimal generalQuantity, decimal reserveQuantity, int updatedBy)
    {
        TotalQuantity += quantity;
        GeneralQuantity += generalQuantity;
        CommanderReserveQuantity += reserveQuantity;
        Update(updatedBy);
    }

    public void RemoveStock(decimal quantity, decimal generalQuantity, decimal reserveQuantity, int updatedBy)
    {
        TotalQuantity -= quantity;
        GeneralQuantity -= generalQuantity;
        CommanderReserveQuantity -= reserveQuantity;
        Update(updatedBy);
    }

    public void AllocateGeneral(decimal quantity, int updatedBy)
    {
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Insufficient general stock available for allocation");

        GeneralAllocated += quantity;
        Update(updatedBy);
    }

    public void AllocateReserve(decimal quantity, int updatedBy)
    {
        if (quantity > AvailableReserve)
            throw new InvalidOperationException("Insufficient reserve stock available for allocation");

        ReserveAllocated += quantity;
        Update(updatedBy);
    }

    public void ReleaseGeneralAllocation(decimal quantity, int updatedBy)
    {
        if (quantity > GeneralAllocated)
            throw new InvalidOperationException("Cannot release more than allocated");

        GeneralAllocated -= quantity;
        GeneralQuantity -= quantity;
        TotalQuantity -= quantity;
        Update(updatedBy);
    }

    public void ReleaseReserveAllocation(decimal quantity, int updatedBy)
    {
        if (quantity > ReserveAllocated)
            throw new InvalidOperationException("Cannot release more than allocated");

        ReserveAllocated -= quantity;
        CommanderReserveQuantity -= quantity;
        TotalQuantity -= quantity;
        Update(updatedBy);
    }

    public void AdjustGeneral(decimal adjustment, int updatedBy)
    {
        GeneralQuantity += adjustment;
        TotalQuantity += adjustment;
        Update(updatedBy);
    }

    public void AdjustReserve(decimal adjustment, int updatedBy)
    {
        CommanderReserveQuantity += adjustment;
        TotalQuantity += adjustment;
        Update(updatedBy);
    }

    public void UpdateThresholds(decimal minimumReserveRequired, decimal reorderPoint, int updatedBy)
    {
        MinimumReserveRequired = minimumReserveRequired;
        ReorderPoint = reorderPoint;
        Update(updatedBy);
    }

    public bool IsBelowReorderPoint()
    {
        return TotalQuantity < ReorderPoint;
    }

    public bool IsReserveBelowMinimum()
    {
        return CommanderReserveQuantity < MinimumReserveRequired;
    }

    public bool IsCriticalStock()
    {
        return TotalQuantity < (ReorderPoint * 0.5m);
    }

    public decimal GetReservePercentage()
    {
        if (TotalQuantity == 0) return 0;
        return (CommanderReserveQuantity / TotalQuantity) * 100;
    }
}
