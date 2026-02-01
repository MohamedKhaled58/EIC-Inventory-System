namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a requisition is rejected
/// </summary>
public class RequisitionRejectedEvent : DomainEvent
{
    public int RequisitionId { get; }
    public string RequisitionNumber { get; }
    public int RequesterId { get; }
    public int RejecterId { get; }
    public DateTime RejectionDate { get; }
    public string RejectionReason { get; }
    public bool RequiresCommanderReserve { get; }

    public RequisitionRejectedEvent(
        int requisitionId,
        string requisitionNumber,
        int requesterId,
        int rejecterId,
        DateTime rejectionDate,
        string rejectionReason,
        bool requiresCommanderReserve)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        RequesterId = requesterId;
        RejecterId = rejecterId;
        RejectionDate = rejectionDate;
        RejectionReason = rejectionReason;
        RequiresCommanderReserve = requiresCommanderReserve;
    }
}
