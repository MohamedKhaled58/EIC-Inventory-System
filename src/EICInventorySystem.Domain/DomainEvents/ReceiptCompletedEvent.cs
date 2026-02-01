namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a receipt is completed
/// </summary>
public class ReceiptCompletedEvent : DomainEvent
{
    public int ReceiptId { get; }
    public string ReceiptNumber { get; }
    public int SupplierId { get; }
    public int WarehouseId { get; }
    public decimal TotalQuantity { get; }
    public decimal TotalValue { get; }
    public DateTime ReceiptDate { get; }
    public int ReceiverId { get; }

    public ReceiptCompletedEvent(
        int receiptId,
        string receiptNumber,
        int supplierId,
        int warehouseId,
        decimal totalQuantity,
        decimal totalValue,
        DateTime receiptDate,
        int receiverId)
    {
        ReceiptId = receiptId;
        ReceiptNumber = receiptNumber;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        ReceiptDate = receiptDate;
        ReceiverId = receiverId;
    }
}
