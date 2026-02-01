namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Transaction history for item receiving and issuing events
/// Tracks all inventory movements with worker, department, and approver details
/// </summary>
public class ItemTransactionHistory : BaseEntity
{
    public int ItemId { get; private set; }
    public int? ProjectId { get; private set; }
    public int? DepartmentId { get; private set; }
    public string TransactionType { get; private set; } = null!; // "Receiving", "Issuing"
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime TransactionDate { get; private set; }

    // Worker details
    public int WorkerId { get; private set; }
    public string WorkerName { get; private set; } = null!;
    public int WorkerDepartmentId { get; private set; }
    public string WorkerDepartmentName { get; private set; } = null!;

    // Approver details
    public int ApproverId { get; private set; }
    public string ApproverName { get; private set; } = null!;
    public string ApproverRole { get; private set; } = null!; // "Commander", "Officer", "Engineer"
    public DateTime ApprovalDate { get; private set; }

    // Usage context
    public string? VehicleNumber { get; private set; } // For vehicle-related projects
    public string? UsageDescription { get; private set; }
    public string? UsageDescriptionArabic { get; private set; }
    public string? ReferenceNumber { get; private set; } // RequisitionNumber, TransferNumber, etc.
    public string? Notes { get; private set; }

    // Navigation properties
    public Item Item { get; private set; } = null!;
    public Project? Project { get; private set; }
    public Department? Department { get; private set; }
    public User Worker { get; private set; } = null!;
    public User Approver { get; private set; } = null!;

    private ItemTransactionHistory() { }

    public ItemTransactionHistory(
        int itemId,
        int? projectId,
        int? departmentId,
        string transactionType,
        decimal quantity,
        decimal unitPrice,
        int workerId,
        string workerName,
        int workerDepartmentId,
        string workerDepartmentName,
        int approverId,
        string approverName,
        string approverRole,
        int createdBy,
        string? vehicleNumber = null,
        string? usageDescription = null,
        string? usageDescriptionArabic = null,
        string? referenceNumber = null,
        string? notes = null) : base(createdBy)
    {
        ItemId = itemId;
        ProjectId = projectId;
        DepartmentId = departmentId;
        TransactionType = transactionType;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        TransactionDate = DateTime.UtcNow;
        WorkerId = workerId;
        WorkerName = workerName;
        WorkerDepartmentId = workerDepartmentId;
        WorkerDepartmentName = workerDepartmentName;
        ApproverId = approverId;
        ApproverName = approverName;
        ApproverRole = approverRole;
        ApprovalDate = DateTime.UtcNow;
        VehicleNumber = vehicleNumber;
        UsageDescription = usageDescription;
        UsageDescriptionArabic = usageDescriptionArabic;
        ReferenceNumber = referenceNumber;
        Notes = notes;
    }

    public void UpdateNotes(string notes, int updatedBy)
    {
        Notes = notes;
        Update(updatedBy);
    }

    public bool IsReceivingTransaction()
    {
        return TransactionType == "Receiving";
    }

    public bool IsIssuingTransaction()
    {
        return TransactionType == "Issuing";
    }

    public bool IsVehicleRelated()
    {
        return !string.IsNullOrEmpty(VehicleNumber);
    }
}
