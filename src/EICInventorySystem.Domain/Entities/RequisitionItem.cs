namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item within a requisition
/// </summary>
public class RequisitionItem : BaseEntity
{
    public int RequisitionId { get; private set; }
    public int ItemId { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Notes { get; private set; }
    public bool IsFromCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }

    // Navigation properties
    public Requisition Requisition { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private RequisitionItem() { }

    public RequisitionItem(
        int requisitionId,
        int itemId,
        decimal requestedQuantity,
        decimal unitPrice,
        int createdBy,
        string? notes = null,
        bool isFromCommanderReserve = false,
        decimal commanderReserveQuantity = 0) : base(createdBy)
    {
        RequisitionId = requisitionId;
        ItemId = itemId;
        RequestedQuantity = requestedQuantity;
        IssuedQuantity = 0;
        UnitPrice = unitPrice;
        TotalValue = requestedQuantity * unitPrice;
        Notes = notes;
        IsFromCommanderReserve = isFromCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
    }

    public void Issue(decimal quantity, int updatedBy)
    {
        if (quantity > (RequestedQuantity - IssuedQuantity))
            throw new InvalidOperationException("Cannot issue more than requested quantity");

        IssuedQuantity += quantity;
        Update(updatedBy);
    }

    public void UpdateQuantity(decimal requestedQuantity, decimal unitPrice, int updatedBy)
    {
        RequestedQuantity = requestedQuantity;
        UnitPrice = unitPrice;
        TotalValue = requestedQuantity * unitPrice;
        Update(updatedBy);
    }

    public void MarkAsFromCommanderReserve(decimal reserveQuantity, int updatedBy)
    {
        IsFromCommanderReserve = true;
        CommanderReserveQuantity = reserveQuantity;
        Update(updatedBy);
    }

    public decimal GetRemainingQuantity()
    {
        return RequestedQuantity - IssuedQuantity;
    }

    public bool IsFullyIssued()
    {
        return IssuedQuantity >= RequestedQuantity;
    }
}
