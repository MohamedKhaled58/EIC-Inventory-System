namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when an item transaction is recorded
/// </summary>
public class ItemTransactionRecordedEvent : DomainEvent
{
    public int ItemId { get; }
    public int? ProjectId { get; }
    public int? DepartmentId { get; }
    public string TransactionType { get; }
    public decimal Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalValue { get; }
    public int WorkerId { get; }
    public string WorkerName { get; }
    public int WorkerDepartmentId { get; }
    public int ApproverId { get; }
    public string ApproverRole { get; }
    public string? VehicleNumber { get; }
    public string ReferenceNumber { get; }

    public ItemTransactionRecordedEvent(
        int itemId,
        int? projectId,
        int? departmentId,
        string transactionType,
        decimal quantity,
        decimal unitPrice,
        int workerId,
        string workerName,
        int workerDepartmentId,
        int approverId,
        string approverRole,
        string referenceNumber,
        string? vehicleNumber = null)
    {
        ItemId = itemId;
        ProjectId = projectId;
        DepartmentId = departmentId;
        TransactionType = transactionType;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalValue = quantity * unitPrice;
        WorkerId = workerId;
        WorkerName = workerName;
        WorkerDepartmentId = workerDepartmentId;
        ApproverId = approverId;
        ApproverRole = approverRole;
        VehicleNumber = vehicleNumber;
        ReferenceNumber = referenceNumber;
    }
}
