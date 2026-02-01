namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Project allocation entity for tracking items allocated to projects
/// </summary>
public class ProjectAllocation : BaseEntity
{
    public int ProjectId { get; private set; }
    public int ItemId { get; private set; }
    public int WarehouseId { get; private set; }
    public decimal AllocatedQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime AllocationDate { get; private set; }
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    public bool IsFromCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public Item Item { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;

    private ProjectAllocation() { }

    public ProjectAllocation(
        int projectId,
        int itemId,
        int warehouseId,
        decimal allocatedQuantity,
        decimal unitPrice,
        int createdBy,
        string? notes = null,
        string? notesArabic = null,
        bool isFromCommanderReserve = false,
        decimal commanderReserveQuantity = 0) : base(createdBy)
    {
        ProjectId = projectId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        AllocatedQuantity = allocatedQuantity;
        ConsumedQuantity = 0;
        ReturnedQuantity = 0;
        UnitPrice = unitPrice;
        TotalValue = allocatedQuantity * unitPrice;
        AllocationDate = DateTime.UtcNow;
        Notes = notes;
        NotesArabic = notesArabic;
        IsFromCommanderReserve = isFromCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
    }

    public void Consume(decimal quantity, int updatedBy)
    {
        if (quantity > (AllocatedQuantity - ConsumedQuantity - ReturnedQuantity))
            throw new InvalidOperationException("Cannot consume more than allocated quantity");

        ConsumedQuantity += quantity;
        Update(updatedBy);
    }

    public void Return(decimal quantity, int updatedBy)
    {
        if (quantity > (AllocatedQuantity - ConsumedQuantity - ReturnedQuantity))
            throw new InvalidOperationException("Cannot return more than available quantity");

        ReturnedQuantity += quantity;
        Update(updatedBy);
    }

    public void UpdateAllocation(decimal allocatedQuantity, decimal unitPrice, int updatedBy)
    {
        AllocatedQuantity = allocatedQuantity;
        UnitPrice = unitPrice;
        TotalValue = allocatedQuantity * unitPrice;
        Update(updatedBy);
    }

    public decimal GetAvailableQuantity()
    {
        return AllocatedQuantity - ConsumedQuantity - ReturnedQuantity;
    }

    public bool IsFullyConsumed()
    {
        return ConsumedQuantity >= AllocatedQuantity;
    }

    public bool IsFullyReturned()
    {
        return ReturnedQuantity >= AllocatedQuantity;
    }
}
