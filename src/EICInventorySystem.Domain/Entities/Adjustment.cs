namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Adjustment entity for inventory corrections
/// </summary>
public class Adjustment : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int WarehouseId { get; private set; }
    public DateTime AdjustmentDate { get; private set; }
    public AdjustmentStatus Status { get; private set; }
    public AdjustmentType Type { get; private set; }
    public string? Reason { get; private set; }
    public string? ReasonArabic { get; private set; }
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public decimal TotalAdjustment { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? ReferenceNumber { get; private set; } // Physical count reference, etc.

    // Navigation properties
    public Warehouse Warehouse { get; private set; } = null!;
    public User? Approver { get; private set; }
    public ICollection<AdjustmentItem> Items { get; private set; } = new List<AdjustmentItem>();

    private Adjustment() { }

    public Adjustment(
        string number,
        int warehouseId,
        DateTime adjustmentDate,
        AdjustmentType type,
        int createdBy,
        string? reason = null,
        string? reasonArabic = null,
        string? referenceNumber = null) : base(createdBy)
    {
        Number = number;
        WarehouseId = warehouseId;
        AdjustmentDate = adjustmentDate;
        Status = AdjustmentStatus.Draft;
        Type = type;
        Reason = reason;
        ReasonArabic = reasonArabic;
        TotalAdjustment = 0;
        TotalValue = 0;
        ReferenceNumber = referenceNumber;
    }

    public void Submit(int updatedBy)
    {
        Status = AdjustmentStatus.Pending;
        Update(updatedBy);
    }

    public void Approve(int approverId, string? approvalNotes = null, int updatedBy = 0)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        Status = AdjustmentStatus.Approved;
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, int updatedBy)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = rejectionReason;
        Status = AdjustmentStatus.Rejected;
        Update(updatedBy);
    }

    public void Complete(int updatedBy)
    {
        Status = AdjustmentStatus.Completed;
        Update(updatedBy);
    }

    public void Cancel(int updatedBy)
    {
        Status = AdjustmentStatus.Cancelled;
        Update(updatedBy);
    }

    public void UpdateTotals(decimal totalAdjustment, decimal totalValue, int updatedBy)
    {
        TotalAdjustment = totalAdjustment;
        TotalValue = totalValue;
        Update(updatedBy);
    }

    public bool IsPending()
    {
        return Status == AdjustmentStatus.Pending;
    }

    public bool IsApproved()
    {
        return Status == AdjustmentStatus.Approved || Status == AdjustmentStatus.Completed;
    }

    public bool IsRejected()
    {
        return Status == AdjustmentStatus.Rejected;
    }

    public bool IsCompleted()
    {
        return Status == AdjustmentStatus.Completed;
    }
}


