namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a transfer is approved
/// </summary>
public class TransferApprovedEvent : DomainEvent
{
    public int TransferId { get; }
    public string TransferNumber { get; }
    public int ApprovedBy { get; }
    public DateTime ApprovalDate { get; }

    public TransferApprovedEvent(
        int transferId,
        string transferNumber,
        int approvedBy,
        DateTime approvalDate)
    {
        TransferId = transferId;
        TransferNumber = transferNumber;
        ApprovedBy = approvedBy;
        ApprovalDate = approvalDate;
    }
}
