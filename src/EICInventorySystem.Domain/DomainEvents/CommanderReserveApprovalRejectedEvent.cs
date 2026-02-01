namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when Commander's Reserve approval is rejected
/// </summary>
public class CommanderReserveApprovalRejectedEvent : DomainEvent
{
    public int RequisitionId { get; }
    public string RequisitionNumber { get; }
    public int ItemId { get; }
    public decimal RequestedQuantity { get; }
    public int RequesterId { get; }
    public int CommanderId { get; }
    public DateTime RejectionDate { get; }
    public string RejectionReason { get; }

    public CommanderReserveApprovalRejectedEvent(
        int requisitionId,
        string requisitionNumber,
        int itemId,
        decimal requestedQuantity,
        int requesterId,
        int commanderId,
        DateTime rejectionDate,
        string rejectionReason)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        ItemId = itemId;
        RequestedQuantity = requestedQuantity;
        RequesterId = requesterId;
        CommanderId = commanderId;
        RejectionDate = rejectionDate;
        RejectionReason = rejectionReason;
    }
}
