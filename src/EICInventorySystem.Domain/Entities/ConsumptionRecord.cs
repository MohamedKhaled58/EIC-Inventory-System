namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Record of material consumption by a project
/// </summary>
public class ConsumptionRecord
{
    public int Id { get; private set; }
    public int ProjectAllocationId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public DateTime ConsumptionDate { get; private set; }
    public int ConsumedBy { get; private set; }
    public string? Notes { get; private set; }
    public string? BatchNumber { get; private set; }
    public string? WorkOrderNumber { get; private set; }

    // Navigation properties
    public ProjectAllocation ProjectAllocation { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ConsumptionRecord() { }

    public ConsumptionRecord(
        int projectAllocationId,
        int itemId,
        decimal quantity,
        string unit,
        int consumedBy,
        string? notes = null,
        string? batchNumber = null,
        string? workOrderNumber = null)
    {
        ProjectAllocationId = projectAllocationId;
        ItemId = itemId;
        Quantity = quantity;
        Unit = unit;
        ConsumptionDate = DateTime.UtcNow;
        ConsumedBy = consumedBy;
        Notes = notes;
        BatchNumber = batchNumber;
        WorkOrderNumber = workOrderNumber;
    }
}
