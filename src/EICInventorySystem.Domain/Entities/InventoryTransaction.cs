namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Generic inventory transaction for tracking all inventory movements
/// </summary>
public class InventoryTransaction : BaseEntity
{
    public int InventoryRecordId { get; private set; }
    public string TransactionType { get; private set; } = null!; // "Receipt", "Issue", "TransferOut", "TransferIn", "Adjustment", "Return"
    public decimal Quantity { get; private set; }
    public decimal GeneralQuantity { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public decimal PreviousGeneralQuantity { get; private set; }
    public decimal PreviousReserveQuantity { get; private set; }
    public decimal NewGeneralQuantity { get; private set; }
    public decimal NewReserveQuantity { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public int? ReferenceId { get; private set; }
    public string ReferenceType { get; private set; } = null!; // "Requisition", "Transfer", "Receipt", "Adjustment", "Return", "Consumption"
    public string? ReferenceNumber { get; private set; }
    public string? Notes { get; private set; }
    public bool IsFromCommanderReserve { get; private set; }
    public int? UserId { get; private set; }

    // Navigation properties
    public InventoryRecord InventoryRecord { get; private set; } = null!;
    public User? User { get; private set; }

    private InventoryTransaction() { }

    public InventoryTransaction(
        int inventoryRecordId,
        string transactionType,
        decimal quantity,
        decimal generalQuantity,
        decimal commanderReserveQuantity,
        decimal unitPrice,
        decimal previousGeneralQuantity,
        decimal previousReserveQuantity,
        decimal newGeneralQuantity,
        decimal newReserveQuantity,
        string referenceType,
        int? referenceId,
        string? referenceNumber,
        int createdBy,
        string? notes = null,
        bool isFromCommanderReserve = false,
        int? userId = null) : base(createdBy)
    {
        InventoryRecordId = inventoryRecordId;
        TransactionType = transactionType;
        Quantity = quantity;
        GeneralQuantity = generalQuantity;
        CommanderReserveQuantity = commanderReserveQuantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        PreviousGeneralQuantity = previousGeneralQuantity;
        PreviousReserveQuantity = previousReserveQuantity;
        NewGeneralQuantity = newGeneralQuantity;
        NewReserveQuantity = newReserveQuantity;
        TransactionDate = DateTime.UtcNow;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        IsFromCommanderReserve = isFromCommanderReserve;
        UserId = userId ?? createdBy;
    }

    public bool IsReceipt()
    {
        return TransactionType == "Receipt" || TransactionType == "TransferIn";
    }

    public bool IsIssue()
    {
        return TransactionType == "Issue" || TransactionType == "TransferOut";
    }

    public bool IsAdjustment()
    {
        return TransactionType == "Adjustment";
    }

    public bool IsFromReserve()
    {
        return IsFromCommanderReserve;
    }
}
