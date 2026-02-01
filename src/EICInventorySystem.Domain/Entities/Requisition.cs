using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Requisition entity for material requests
/// Supports: BOQ-based requests, Daily operational requests, Emergency requests
/// </summary>
public class Requisition : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int RequesterId { get; private set; }
    public int? DepartmentId { get; private set; }
    public int? ProjectId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public RequisitionStatus Status { get; private set; }
    public RequisitionPriority Priority { get; private set; }
    
    /// <summary>
    /// Source of this requisition: Standard, BOQ, DailyOperational, Emergency
    /// </summary>
    public RequisitionSource Source { get; private set; }
    
    /// <summary>
    /// If Source is BOQ, this links to the ProjectBOQ
    /// </summary>
    public int? BOQId { get; private set; }
    
    public string? Reason { get; private set; }
    public string? ReasonArabic { get; private set; }
    public int? ApproverId { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public bool RequiresCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    public int? CommanderApprovalId { get; private set; }
    public DateTime? CommanderApprovalDate { get; private set; }
    public string? CommanderApprovalNotes { get; private set; }

    // Navigation properties
    public User Requester { get; private set; } = null!;
    public Department? Department { get; private set; }
    public Project? Project { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public User? Approver { get; private set; }
    public User? CommanderApprover { get; private set; }
    public ProjectBOQ? BOQ { get; private set; }
    public ICollection<RequisitionItem> Items { get; private set; } = new List<RequisitionItem>();

    private Requisition() { }

    public Requisition(
        string number,
        int requesterId,
        int? departmentId,
        int? projectId,
        int warehouseId,
        DateTime requiredDate,
        RequisitionPriority priority,
        int createdBy,
        RequisitionSource source = RequisitionSource.Standard,
        int? boqId = null,
        string? reason = null,
        string? reasonArabic = null) : base(createdBy)
    {
        Number = number;
        RequesterId = requesterId;
        DepartmentId = departmentId;
        ProjectId = projectId;
        WarehouseId = warehouseId;
        RequestDate = DateTime.UtcNow;
        RequiredDate = requiredDate;
        Status = RequisitionStatus.Draft;
        Priority = priority;
        Source = source;
        BOQId = boqId;
        Reason = reason;
        ReasonArabic = reasonArabic;
        TotalQuantity = 0;
        TotalValue = 0;
        IssuedQuantity = 0;
        RequiresCommanderReserve = false;
        CommanderReserveQuantity = 0;
    }

    public void Submit(int updatedBy)
    {
        Status = RequisitionStatus.Pending;
        Update(updatedBy);
    }

    public void Approve(int approverId, string? approvalNotes = null, int updatedBy = 0)
    {
        ApproverId = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        Status = RequisitionStatus.Approved;
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, int updatedBy)
    {
        ApproverId = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = rejectionReason;
        Status = RequisitionStatus.Rejected;
        Update(updatedBy);
    }

    public void ApproveCommanderReserve(int commanderId, string? approvalNotes = null, int updatedBy = 0)
    {
        CommanderApprovalId = commanderId;
        CommanderApprovalDate = DateTime.UtcNow;
        CommanderApprovalNotes = approvalNotes;
        Update(updatedBy);
    }

    public void Issue(decimal quantity, int updatedBy)
    {
        IssuedQuantity += quantity;
        if (IssuedQuantity >= TotalQuantity)
        {
            Status = RequisitionStatus.Completed;
        }
        else
        {
            Status = RequisitionStatus.PartiallyIssued;
        }
        Update(updatedBy);
    }

    public void Cancel(int updatedBy)
    {
        Status = RequisitionStatus.Cancelled;
        Update(updatedBy);
    }

    public void UpdateTotals(decimal totalQuantity, decimal totalValue, int updatedBy)
    {
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        Update(updatedBy);
    }

    public void MarkRequiresCommanderReserve(decimal reserveQuantity, int updatedBy)
    {
        RequiresCommanderReserve = true;
        CommanderReserveQuantity = reserveQuantity;
        Update(updatedBy);
    }

    public bool IsPending()
    {
        return Status == RequisitionStatus.Pending;
    }

    public bool IsApproved()
    {
        return Status == RequisitionStatus.Approved || Status == RequisitionStatus.PartiallyIssued || Status == RequisitionStatus.Completed;
    }

    public bool IsRejected()
    {
        return Status == RequisitionStatus.Rejected;
    }

    public bool IsCompleted()
    {
        return Status == RequisitionStatus.Completed;
    }

    public decimal GetRemainingQuantity()
    {
        return TotalQuantity - IssuedQuantity;
    }
}
