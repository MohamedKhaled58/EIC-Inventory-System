namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Return entity for returning materials to warehouse
/// </summary>
public class Return : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int ProjectId { get; private set; }
    public int? DepartmentId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateTime ReturnDate { get; private set; }
    public string Status { get; private set; } = null!; // "Draft", "Pending", "Approved", "Rejected", "Received", "Cancelled"
    public string? Reason { get; private set; }
    public string? ReasonArabic { get; private set; }
    public int? ApproverId { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public int? ReceiverId { get; private set; }
    public DateTime? ReceivedDate { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public Department? Department { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;
    public User? Approver { get; private set; }
    public User? Receiver { get; private set; }
    public ICollection<ReturnItem> Items { get; private set; } = new List<ReturnItem>();

    private Return() { }

    public Return(
        string number,
        int projectId,
        int? departmentId,
        int warehouseId,
        DateTime returnDate,
        int createdBy,
        string? reason = null,
        string? reasonArabic = null) : base(createdBy)
    {
        Number = number;
        ProjectId = projectId;
        DepartmentId = departmentId;
        WarehouseId = warehouseId;
        ReturnDate = returnDate;
        Status = "Draft";
        Reason = reason;
        ReasonArabic = reasonArabic;
        TotalQuantity = 0;
        TotalValue = 0;
    }

    public void Submit(int updatedBy)
    {
        Status = "Pending";
        Update(updatedBy);
    }

    public void Approve(int approverId, string? approvalNotes = null, int updatedBy = 0)
    {
        ApproverId = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        Status = "Approved";
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, int updatedBy)
    {
        ApproverId = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = rejectionReason;
        Status = "Rejected";
        Update(updatedBy);
    }

    public void Receive(int receiverId, int updatedBy)
    {
        ReceiverId = receiverId;
        ReceivedDate = DateTime.UtcNow;
        Status = "Received";
        Update(updatedBy);
    }

    public void Cancel(int updatedBy)
    {
        Status = "Cancelled";
        Update(updatedBy);
    }

    public void UpdateTotals(decimal totalQuantity, decimal totalValue, int updatedBy)
    {
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        Update(updatedBy);
    }

    public bool IsPending()
    {
        return Status == "Pending";
    }

    public bool IsApproved()
    {
        return Status == "Approved" || Status == "Received";
    }

    public bool IsRejected()
    {
        return Status == "Rejected";
    }

    public bool IsReceived()
    {
        return Status == "Received";
    }
}
