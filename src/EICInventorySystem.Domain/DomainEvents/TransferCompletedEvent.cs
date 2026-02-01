namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a transfer is completed
/// </summary>
public class TransferCompletedEvent : DomainEvent
{
    public int TransferId { get; }
    public string TransferNumber { get; }
    public int FromWarehouseId { get; }
    public int ToWarehouseId { get; }
    public DateTime CompletionDate { get; }

    public TransferCompletedEvent(
        int transferId,
        string transferNumber,
        int fromWarehouseId,
        int toWarehouseId,
        DateTime completionDate)
    {
        TransferId = transferId;
        TransferNumber = transferNumber;
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        CompletionDate = completionDate;
    }
}
