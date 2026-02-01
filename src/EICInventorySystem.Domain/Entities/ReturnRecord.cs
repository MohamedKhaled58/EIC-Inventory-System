namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Record of material returns from projects/departments
/// </summary>
public class ReturnRecord
{
    public int Id { get; private set; }
    public int ProjectAllocationId { get; private set; }
    public int ItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public DateTime ReturnDate { get; private set; }
    public int ReturnedBy { get; private set; }
    public string? Reason { get; private set; }
    public string? Condition { get; private set; } // "Good", "Damaged", "Used"
    public string? BatchNumber { get; private set; }

    // Navigation properties
    public ProjectAllocation ProjectAllocation { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ReturnRecord() { }

    public ReturnRecord(
        int projectAllocationId,
        int itemId,
        decimal quantity,
        string unit,
        int returnedBy,
        string? reason = null,
        string? condition = null,
        string? batchNumber = null)
    {
        ProjectAllocationId = projectAllocationId;
        ItemId = itemId;
        Quantity = quantity;
        Unit = unit;
        ReturnDate = DateTime.UtcNow;
        ReturnedBy = returnedBy;
        Reason = reason;
        Condition = condition ?? "Good";
        BatchNumber = batchNumber;
    }
}
