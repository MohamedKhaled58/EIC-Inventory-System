namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a requisition is created
/// </summary>
public class RequisitionCreatedEvent : DomainEvent
{
    public int RequisitionId { get; }
    public string RequisitionNumber { get; }
    public int RequesterId { get; }
    public int? DepartmentId { get; }
    public int? ProjectId { get; }
    public int WarehouseId { get; }
    public decimal TotalQuantity { get; }
    public bool RequiresCommanderReserve { get; }

    public RequisitionCreatedEvent(
        int requisitionId,
        string requisitionNumber,
        int requesterId,
        int? departmentId,
        int? projectId,
        int warehouseId,
        decimal totalQuantity,
        bool requiresCommanderReserve)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        RequesterId = requesterId;
        DepartmentId = departmentId;
        ProjectId = projectId;
        WarehouseId = warehouseId;
        TotalQuantity = totalQuantity;
        RequiresCommanderReserve = requiresCommanderReserve;
    }
}
