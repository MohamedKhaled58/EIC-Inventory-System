namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a return is completed
/// </summary>
public class ReturnCompletedEvent : DomainEvent
{
    public int ReturnId { get; }
    public string ReturnNumber { get; }
    public int ProjectId { get; }
    public int? DepartmentId { get; }
    public int WarehouseId { get; }
    public decimal TotalQuantity { get; }
    public decimal TotalValue { get; }
    public DateTime ReturnDate { get; }
    public int ReceiverId { get; }

    public ReturnCompletedEvent(
        int returnId,
        string returnNumber,
        int projectId,
        int? departmentId,
        int warehouseId,
        decimal totalQuantity,
        decimal totalValue,
        DateTime returnDate,
        int receiverId)
    {
        ReturnId = returnId;
        ReturnNumber = returnNumber;
        ProjectId = projectId;
        DepartmentId = departmentId;
        WarehouseId = warehouseId;
        TotalQuantity = totalQuantity;
        TotalValue = totalValue;
        ReturnDate = returnDate;
        ReceiverId = receiverId;
    }
}
