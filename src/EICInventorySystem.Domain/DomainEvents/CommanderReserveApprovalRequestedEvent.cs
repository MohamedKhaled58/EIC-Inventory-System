namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when Commander's Reserve approval is requested
/// </summary>
public class CommanderReserveApprovalRequestedEvent : DomainEvent
{
    public int RequisitionId { get; }
    public string RequisitionNumber { get; }
    public int ItemId { get; }
    public decimal RequestedQuantity { get; }
    public int RequesterId { get; }
    public int FactoryId { get; }
    public string Reason { get; }
    public DateTime RequestDate { get; }

    public CommanderReserveApprovalRequestedEvent(
        int requisitionId,
        string requisitionNumber,
        int itemId,
        decimal requestedQuantity,
        int requesterId,
        int factoryId,
        string reason,
        DateTime requestDate)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        ItemId = itemId;
        RequestedQuantity = requestedQuantity;
        RequesterId = requesterId;
        FactoryId = factoryId;
        Reason = reason;
        RequestDate = requestDate;
    }
}
