namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a requisition is approved
/// </summary>
public class RequisitionApprovedEvent : DomainEvent
{
    public int RequisitionId { get; }
    public string RequisitionNumber { get; }
    public int ApproverId { get; }
    public DateTime ApprovalDate { get; }
    public string? ApprovalNotes { get; }
    public bool RequiresCommanderReserve { get; }

    public RequisitionApprovedEvent(
        int requisitionId,
        string requisitionNumber,
        int approverId,
        DateTime approvalDate,
        string? approvalNotes,
        bool requiresCommanderReserve)
    {
        RequisitionId = requisitionId;
        RequisitionNumber = requisitionNumber;
        ApproverId = approverId;
        ApprovalDate = approvalDate;
        ApprovalNotes = approvalNotes;
        RequiresCommanderReserve = requiresCommanderReserve;
    }
}
