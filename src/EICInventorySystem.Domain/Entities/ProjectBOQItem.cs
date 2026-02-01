namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Individual item/line in a Project BOQ
/// Tracks requested vs issued quantities per item
/// Items selected by Part Number ONLY (no free text)
/// </summary>
public class ProjectBOQItem : BaseEntity
{
    public int BOQId { get; private set; }
    public int ItemId { get; private set; }
    public decimal RequestedQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public decimal RemainingQuantity => RequestedQuantity - IssuedQuantity;
    
    // Commander Reserve tracking
    public bool IsFromCommanderReserve { get; private set; }
    public decimal CommanderReserveQuantity { get; private set; }
    
    // Stock availability (set during validation)
    public decimal AvailableStock { get; private set; }
    
    // Partial issue tracking
    public string? PartialIssueReason { get; private set; }
    public string? PartialIssueReasonArabic { get; private set; }
    
    // Notes
    public string? Notes { get; private set; }
    public string? NotesArabic { get; private set; }

    // Navigation properties
    public ProjectBOQ BOQ { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private ProjectBOQItem() { }

    public ProjectBOQItem(
        int boqId,
        int itemId,
        decimal requestedQuantity,
        int createdBy,
        bool isFromCommanderReserve = false,
        decimal commanderReserveQuantity = 0,
        string? notes = null,
        string? notesArabic = null) : base(createdBy)
    {
        BOQId = boqId;
        ItemId = itemId;
        RequestedQuantity = requestedQuantity;
        IssuedQuantity = 0;
        IsFromCommanderReserve = isFromCommanderReserve;
        CommanderReserveQuantity = commanderReserveQuantity;
        AvailableStock = 0;
        Notes = notes;
        NotesArabic = notesArabic;
    }

    public void UpdateQuantity(decimal requestedQuantity, int updatedBy)
    {
        if (requestedQuantity <= 0)
            throw new InvalidOperationException("Requested quantity must be greater than zero");

        RequestedQuantity = requestedQuantity;
        Update(updatedBy);
    }

    public void SetAvailableStock(decimal availableStock)
    {
        AvailableStock = availableStock;
    }

    public void MarkForCommanderReserve(decimal quantity, int updatedBy)
    {
        IsFromCommanderReserve = true;
        CommanderReserveQuantity = quantity;
        Update(updatedBy);
    }

    public void Issue(decimal quantity, int updatedBy, string? partialReason = null, string? partialReasonArabic = null)
    {
        if (quantity > RemainingQuantity)
            throw new InvalidOperationException("Cannot issue more than remaining quantity");

        IssuedQuantity += quantity;

        if (RemainingQuantity > 0 && !string.IsNullOrEmpty(partialReason))
        {
            PartialIssueReason = partialReason;
            PartialIssueReasonArabic = partialReasonArabic;
        }

        Update(updatedBy);
    }

    public void IssueFullQuantity(int updatedBy)
    {
        IssuedQuantity = RequestedQuantity;
        Update(updatedBy);
    }

    /// <summary>
    /// Check if there is sufficient stock to issue this item
    /// </summary>
    public bool HasSufficientStock()
    {
        return AvailableStock >= RemainingQuantity;
    }

    /// <summary>
    /// Get the shortfall quantity (how much more is needed)
    /// </summary>
    public decimal GetShortfall()
    {
        return Math.Max(0, RemainingQuantity - AvailableStock);
    }
    
    /// <summary>
    /// Shortfall property - alias for GetShortfall() for easier binding
    /// </summary>
    public decimal Shortfall => GetShortfall();
}
