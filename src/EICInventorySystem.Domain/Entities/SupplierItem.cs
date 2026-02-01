using EICInventorySystem.Domain.Common;

namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Represents items supplied by a supplier with pricing and lead time information
/// </summary>
public class SupplierItem : Entity
{
    public int SupplierId { get; private set; }
    public int ItemId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int LeadTimeDays { get; private set; }
    public int MinimumOrderQuantity { get; private set; }
    public int MaximumOrderQuantity { get; private set; }
    public string? SupplierItemCode { get; private set; }
    public bool IsPreferred { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public int? UpdatedBy { get; private set; }

    // Navigation properties
    public Supplier Supplier { get; private set; } = null!;
    public Item Item { get; private set; } = null!;

    private SupplierItem() { }

    public SupplierItem(
        int supplierId,
        int itemId,
        decimal unitPrice,
        int leadTimeDays,
        int minimumOrderQuantity,
        int maximumOrderQuantity,
        string? supplierItemCode,
        bool isPreferred,
        int createdBy)
    {
        SupplierId = supplierId;
        ItemId = itemId;
        UnitPrice = unitPrice;
        LeadTimeDays = leadTimeDays;
        MinimumOrderQuantity = minimumOrderQuantity;
        MaximumOrderQuantity = maximumOrderQuantity;
        SupplierItemCode = supplierItemCode;
        IsPreferred = isPreferred;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(
        decimal unitPrice,
        int leadTimeDays,
        int minimumOrderQuantity,
        int maximumOrderQuantity,
        string? supplierItemCode,
        bool isPreferred,
        int updatedBy)
    {
        UnitPrice = unitPrice;
        LeadTimeDays = leadTimeDays;
        MinimumOrderQuantity = minimumOrderQuantity;
        MaximumOrderQuantity = maximumOrderQuantity;
        SupplierItemCode = supplierItemCode;
        IsPreferred = isPreferred;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetPreferred(bool preferred, int userId)
    {
        IsPreferred = preferred;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}
