namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Purchase order entity for ordering materials from suppliers
/// </summary>
public class PurchaseOrder : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int SupplierId { get; private set; }
    public int? WarehouseId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDeliveryDate { get; private set; }
    public DateTime? ActualDeliveryDate { get; private set; }
    public string Status { get; private set; } = null!; // "Draft", "Pending", "Approved", "Rejected", "Sent", "PartiallyReceived", "Received", "Cancelled"
    public string Priority { get; private set; } = null!; // "Low", "Medium", "High", "Critical"
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    public int? ApproverId { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal ReceivedValue { get; private set; }
    public string? Currency { get; private set; }
    public string PaymentTerms { get; private set; } = null!;
    public string? InvoiceNumber { get; private set; }

    // Navigation properties
    public Supplier Supplier { get; private set; } = null!;
    public Warehouse? Warehouse { get; private set; }
    public User? Approver { get; private set; }
    public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();
    public ICollection<Receipt> Receipts { get; private set; } = new List<Receipt>();

    private PurchaseOrder() { }

    public PurchaseOrder(
        string number,
        int supplierId,
        int? warehouseId,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        string priority,
        string paymentTerms,
        int createdBy,
        string? notes = null,
        string? notesArabic = null,
        string? currency = null) : base(createdBy)
    {
        Number = number;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Status = "Draft";
        Priority = priority;
        Notes = notes;
        NotesArabic = notesArabic;
        TotalQuantity = 0;
        TotalValue = 0;
        ReceivedQuantity = 0;
        ReceivedValue = 0;
        Currency = currency ?? "EGP";
        PaymentTerms = paymentTerms;
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

    public void Send(int updatedBy)
    {
        Status = "Sent";
        Update(updatedBy);
    }

    public void Receive(decimal quantity, decimal value, int updatedBy)
    {
        ReceivedQuantity += quantity;
        ReceivedValue += value;
        if (ReceivedQuantity >= TotalQuantity)
        {
            ActualDeliveryDate = DateTime.UtcNow;
            Status = "Received";
        }
        else
        {
            Status = "PartiallyReceived";
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

    public bool IsPending()
    {
        return Status == "Pending";
    }

    public bool IsApproved()
    {
        return Status == "Approved" || Status == "Sent" || Status == "PartiallyReceived" || Status == "Received";
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

    public decimal GetRemainingValue()
    {
        return TotalValue - ReceivedValue;
    }
}
