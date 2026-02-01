namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Receipt entity for receiving materials from suppliers
/// </summary>
public class Receipt : BaseEntity
{
    public string Number { get; private set; } = null!;
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? PurchaseOrderId { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public string Status { get; private set; } = null!; // "Draft", "Pending", "Received", "Cancelled"
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }
    public int? ReceiverId { get; private set; }
    public DateTime? ReceivedDate { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public DateTime? InvoiceDate { get; private set; }
    public string? DeliveryNoteNumber { get; private set; }

    // Navigation properties
    public Supplier Supplier { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; private set; }
    public User? Receiver { get; private set; }
    public ICollection<ReceiptItem> Items { get; private set; } = new List<ReceiptItem>();

    private Receipt() { }

    public Receipt(
        string number,
        int supplierId,
        int warehouseId,
        int? purchaseOrderId,
        DateTime receiptDate,
        int createdBy,
        string? notes = null,
        string? notesArabic = null,
        string? invoiceNumber = null,
        DateTime? invoiceDate = null,
        string? deliveryNoteNumber = null) : base(createdBy)
    {
        Number = number;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        PurchaseOrderId = purchaseOrderId;
        ReceiptDate = receiptDate;
        Status = "Draft";
        Notes = notes;
        NotesArabic = notesArabic;
        TotalQuantity = 0;
        TotalValue = 0;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DeliveryNoteNumber = deliveryNoteNumber;
    }

    public void Submit(int updatedBy)
    {
        Status = "Pending";
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

    public bool IsReceived()
    {
        return Status == "Received";
    }
}
