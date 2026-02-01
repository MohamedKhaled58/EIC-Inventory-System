namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a transfer is created
/// </summary>
public class TransferCreatedEvent : DomainEvent
{
    public int TransferId { get; }
    public string TransferNumber { get; }
    public int SourceWarehouseId { get; }
    public int DestinationWarehouseId { get; }
    public DateTime TransferDate { get; }

    public TransferCreatedEvent(
        int transferId,
        string transferNumber,
        int sourceWarehouseId,
        int destinationWarehouseId,
        DateTime transferDate)
    {
        TransferId = transferId;
        TransferNumber = transferNumber;
        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        TransferDate = transferDate;
    }
}
