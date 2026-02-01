namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Transfer entity for moving materials between warehouses
/// </summary>
public class Transfer : BaseEntity
{
    public string TransferNumber { get; private set; } = null!;
    public int CreatedByUserId { get; private set; }
    public int SourceWarehouseId { get; private set; }
    public int DestinationWarehouseId { get; private set; }
    public DateTime TransferDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? ReceivedDate { get; private set; }
    public string Status { get; private set; } = null!; // "Draft", "Pending", "Approved", "Rejected", "Shipped", "Received", "Cancelled"
    public string Priority { get; private set; } = null!; // "Low", "Medium", "High", "Critical"
    public string? Notes { get; private set; }
    public string? ReasonArabic { get; private set; }
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public string? RejectionReason { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public decimal ShippedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public bool RequiresCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    public int? CommanderApprovalId { get; private set; }
    public DateTime? CommanderApprovalDate { get; private set; }
    public string? CommanderApprovalNotes { get; private set; }

    // Navigation properties
    public User CreatedByUser { get; private set; } = null!;
    public Warehouse SourceWarehouse { get; private set; } = null!;
    public Warehouse DestinationWarehouse { get; private set; } = null!;
    public User? ApprovedByUser { get; private set; }
    public User? CommanderApprover { get; private set; }
    public ICollection<TransferItem> Items { get; private set; } = new List<TransferItem>();

    private Transfer() { }

    public Transfer(
        string transferNumber,
        int createdByUserId,
        int sourceWarehouseId,
        int destinationWarehouseId,
        DateTime requiredDate,
        string priority,
        int createdBy,
        string? notes = null,
        string? reasonArabic = null) : base(createdBy)
    {
        TransferNumber = transferNumber;
        CreatedByUserId = createdByUserId;
        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        TransferDate = DateTime.UtcNow;
        RequiredDate = requiredDate;
        Status = "Draft";
        Priority = priority;
        Notes = notes;
        ReasonArabic = reasonArabic;
        TotalQuantity = 0;
        TotalValue = 0;
        ShippedQuantity = 0;
        ReceivedQuantity = 0;
        RequiresCommanderReserve = false;
        CommanderReserveQuantity = 0;
    }

    public void Submit(int updatedBy)
    {
        Status = "Pending";
        Update(updatedBy);
    }

    public void Approve(int approverId, string? approvalNotes = null, int updatedBy = 0)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalNotes = approvalNotes;
        Status = "Approved";
        Update(updatedBy);
    }

    public void Reject(int approverId, string rejectionReason, int updatedBy)
    {
        ApprovedBy = approverId;
        ApprovalDate = DateTime.UtcNow;
        RejectionReason = rejectionReason;
        Status = "Rejected";
        Update(updatedBy);
    }

    public void ApproveCommanderReserve(int commanderId, string? approvalNotes = null, int updatedBy = 0)
    {
        CommanderApprovalId = commanderId;
        CommanderApprovalDate = DateTime.UtcNow;
        CommanderApprovalNotes = approvalNotes;
        Update(updatedBy);
    }

    public void Ship(int updatedBy)
    {
        ShippedDate = DateTime.UtcNow;
        Status = "Shipped";
        ShippedQuantity = TotalQuantity;
        Update(updatedBy);
    }

    public void Receive(decimal quantity, int updatedBy)
    {
        ReceivedQuantity += quantity;
        if (ReceivedQuantity >= TotalQuantity)
        {
            ReceivedDate = DateTime.UtcNow;
            Status = "Received";
        }
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

    public void MarkRequiresCommanderReserve(decimal reserveQuantity, int updatedBy)
    {
        RequiresCommanderReserve = true;
        CommanderReserveQuantity = reserveQuantity;
        Update(updatedBy);
    }

    public bool IsPending()
    {
        return Status == "Pending";
    }

    public bool IsApproved()
    {
        return Status == "Approved" || Status == "Shipped" || Status == "Received";
    }

    public bool IsRejected()
    {
        return Status == "Rejected";
    }

    public bool IsCompleted()
    {
        return Status == "Received";
    }

    public decimal GetRemainingQuantity()
    {
        return TotalQuantity - ReceivedQuantity;
    }
}
