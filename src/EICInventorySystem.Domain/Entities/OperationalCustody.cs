using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Operational Custody - tracks items issued to workers for operational work
/// Custody is deducted from warehouse stock and is fully traceable
/// Can be returned or consumed
/// </summary>
public class OperationalCustody : BaseEntity
{
    public string CustodyNumber { get; private set; } = null!;
    public int WorkerId { get; private set; }
    public int ItemId { get; private set; }
    public int WarehouseId { get; private set; }
    public int FactoryId { get; private set; }
    public int DepartmentId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime? ReturnedDate { get; private set; }
    public CustodyStatus Status { get; private set; }
    public string Purpose { get; private set; } = null!;
    public string PurposeArabic { get; private set; } = null!;
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    
    // Issuing/Return tracking
    public int IssuedById { get; private set; }
    public int? ReturnReceivedById { get; private set; }
    
    // Per-item custody limit (from Item settings or manual)
    public decimal? CustodyLimit { get; private set; }

    // Computed properties
    public decimal RemainingQuantity => Quantity - ReturnedQuantity - ConsumedQuantity;
    public int DaysInCustody => (DateTime.UtcNow - IssuedDate).Days;
    public bool IsOverdue(int maxDays) => DaysInCustody > maxDays && Status == CustodyStatus.Active;

    // Navigation properties
    public Worker Worker { get; private set; } = null!;
    public Item Item { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;
    public Factory Factory { get; private set; } = null!;
    public Department Department { get; private set; } = null!;
    public User IssuedBy { get; private set; } = null!;
    public User? ReturnReceivedBy { get; private set; }

    private OperationalCustody() { }

    public OperationalCustody(
        string custodyNumber,
        int workerId,
        int itemId,
        int warehouseId,
        int factoryId,
        int departmentId,
        decimal quantity,
        string purpose,
        string purposeArabic,
        int issuedById,
        int createdBy,
        decimal? custodyLimit = null,
        string? notes = null,
        string? notesArabic = null) : base(createdBy)
    {
        CustodyNumber = custodyNumber;
        WorkerId = workerId;
        ItemId = itemId;
        WarehouseId = warehouseId;
        FactoryId = factoryId;
        DepartmentId = departmentId;
        Quantity = quantity;
        ReturnedQuantity = 0;
        ConsumedQuantity = 0;
        IssuedDate = DateTime.UtcNow;
        Status = CustodyStatus.Active;
        Purpose = purpose;
        PurposeArabic = purposeArabic;
        IssuedById = issuedById;
        CustodyLimit = custodyLimit;
        Notes = notes;
        NotesArabic = notesArabic;
    }

    /// <summary>
    /// Return items from custody back to warehouse
    /// </summary>
    public void Return(decimal quantity, int receivedById, int updatedBy, string? notes = null)
    {
        if (quantity > RemainingQuantity)
            throw new InvalidOperationException("Cannot return more than remaining custody quantity");

        ReturnedQuantity += quantity;
        ReturnReceivedById = receivedById;
        
        if (!string.IsNullOrEmpty(notes))
            Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";

        if (RemainingQuantity == 0)
        {
            Status = CustodyStatus.FullyReturned;
            ReturnedDate = DateTime.UtcNow;
        }
        else
        {
            Status = CustodyStatus.PartiallyReturned;
        }

        Update(updatedBy);
    }

    /// <summary>
    /// Mark items as consumed (used up, cannot be returned)
    /// </summary>
    public void Consume(decimal quantity, int updatedBy, string? notes = null)
    {
        if (quantity > RemainingQuantity)
            throw new InvalidOperationException("Cannot consume more than remaining custody quantity");

        ConsumedQuantity += quantity;
        
        if (!string.IsNullOrEmpty(notes))
            Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";

        if (RemainingQuantity == 0)
        {
            Status = CustodyStatus.Consumed;
            ReturnedDate = DateTime.UtcNow;
        }

        Update(updatedBy);
    }

    /// <summary>
    /// Transfer custody to another worker
    /// </summary>
    public void TransferToWorker(int newWorkerId, int newDepartmentId, int updatedBy)
    {
        WorkerId = newWorkerId;
        DepartmentId = newDepartmentId;
        Status = CustodyStatus.Transferred;
        Update(updatedBy);
    }
}

