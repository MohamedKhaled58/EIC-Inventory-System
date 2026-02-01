namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Item/Material entity with comprehensive inventory tracking and historical data capabilities
/// </summary>
public class Item : BaseEntity
{
    public string ItemCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string DescriptionArabic { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string CategoryArabic { get; private set; } = null!;
    public string Unit { get; private set; } = null!;
    public string UnitOfMeasureArabic { get; private set; } = null!;
    public decimal StandardCost { get; private set; }
    public decimal Weight { get; private set; }
    public string WeightUnit { get; private set; } = null!;
    public string? Specifications { get; private set; }
    public string? SpecificationsArabic { get; private set; }
    public string? Barcode { get; private set; }
    public int ReorderPoint { get; private set; }
    public int MinimumStock { get; private set; }
    public int MaximumStock { get; private set; }
    public decimal ReservePercentage { get; private set; } // 20-30% for Commander's Reserve
    public decimal ReservedQuantity { get; private set; } // Dedicated quantity reserved for Commander
    public bool IsActive { get; private set; }
    public bool IsCritical { get; private set; }
    public bool IsVehicleRelated { get; private set; } // Indicates if item is used for vehicle operations

    // Navigation properties
    public ICollection<InventoryRecord> InventoryRecords { get; private set; } = new List<InventoryRecord>();
    public ICollection<RequisitionItem> RequisitionItems { get; private set; } = new List<RequisitionItem>();
    public ICollection<TransferItem> TransferItems { get; private set; } = new List<TransferItem>();
    public ICollection<ReceiptItem> ReceiptItems { get; private set; } = new List<ReceiptItem>();
    public ICollection<ConsumptionItem> ConsumptionItems { get; private set; } = new List<ConsumptionItem>();
    public ICollection<ReturnItem> ReturnItems { get; private set; } = new List<ReturnItem>();
    public ICollection<AdjustmentItem> AdjustmentItems { get; private set; } = new List<AdjustmentItem>();
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; private set; } = new List<PurchaseOrderItem>();
    public ICollection<ItemTransactionHistory> TransactionHistory { get; private set; } = new List<ItemTransactionHistory>();
    public ICollection<Project> AssignedProjects { get; private set; } = new List<Project>();
    public ICollection<Department> AssignedDepartments { get; private set; } = new List<Department>();

    private Item() { }

    public Item(
        string itemCode,
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        string category,
        string categoryArabic,
        string unit,
        string unitOfMeasureArabic,
        decimal standardCost,
        decimal weight,
        string weightUnit,
        int reorderPoint,
        int minimumStock,
        int maximumStock,
        decimal reservePercentage,
        int createdBy,
        string? specifications = null,
        string? specificationsArabic = null,
        string? barcode = null,
        bool isCritical = false,
        bool isVehicleRelated = false) : base(createdBy)
    {
        ItemCode = itemCode;
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Category = category;
        CategoryArabic = categoryArabic;
        Unit = unit;
        UnitOfMeasureArabic = unitOfMeasureArabic;
        StandardCost = standardCost;
        Weight = weight;
        WeightUnit = weightUnit;
        Specifications = specifications;
        SpecificationsArabic = specificationsArabic;
        Barcode = barcode;
        ReorderPoint = reorderPoint;
        MinimumStock = minimumStock;
        MaximumStock = maximumStock;
        ReservePercentage = reservePercentage;
        ReservedQuantity = 0;
        IsActive = true;
        IsCritical = isCritical;
        IsVehicleRelated = isVehicleRelated;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        string category,
        string categoryArabic,
        decimal standardCost,
        decimal weight,
        string weightUnit,
        int reorderPoint,
        int minimumStock,
        int maximumStock,
        decimal reservePercentage,
        int updatedBy,
        string? specifications = null,
        string? specificationsArabic = null,
        bool isCritical = false,
        bool isVehicleRelated = false)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Category = category;
        CategoryArabic = categoryArabic;
        StandardCost = standardCost;
        Weight = weight;
        WeightUnit = weightUnit;
        Specifications = specifications;
        SpecificationsArabic = specificationsArabic;
        ReorderPoint = reorderPoint;
        MinimumStock = minimumStock;
        MaximumStock = maximumStock;
        ReservePercentage = reservePercentage;
        IsCritical = isCritical;
        IsVehicleRelated = isVehicleRelated;
        Update(updatedBy);
    }

    public void Activate(int updatedBy)
    {
        IsActive = true;
        Update(updatedBy);
    }

    public void Deactivate(int updatedBy)
    {
        IsActive = false;
        Update(updatedBy);
    }

    public void MarkAsCritical(int updatedBy)
    {
        IsCritical = true;
        Update(updatedBy);
    }

    public void UnmarkAsCritical(int updatedBy)
    {
        IsCritical = false;
        Update(updatedBy);
    }

    /// <summary>
    /// Updates the reserved quantity for the Commander
    /// </summary>
    public void UpdateReservedQuantity(decimal quantity, int updatedBy)
    {
        if (quantity < 0)
            throw new InvalidOperationException("Reserved quantity cannot be negative");

        ReservedQuantity = quantity;
        Update(updatedBy);
    }

    /// <summary>
    /// Marks the item as vehicle-related for vehicle operations tracking
    /// </summary>
    public void MarkAsVehicleRelated(int updatedBy)
    {
        IsVehicleRelated = true;
        Update(updatedBy);
    }

    /// <summary>
    /// Unmarks the item as vehicle-related
    /// </summary>
    public void UnmarkAsVehicleRelated(int updatedBy)
    {
        IsVehicleRelated = false;
        Update(updatedBy);
    }

    /// <summary>
    /// Gets the total reserved quantity across all warehouses
    /// </summary>
    public decimal GetTotalReservedQuantity()
    {
        return ReservedQuantity;
    }

    /// <summary>
    /// Checks if the item is suitable for vehicle operations
    /// </summary>
    public bool IsSuitableForVehicleOperations()
    {
        return IsVehicleRelated;
    }
}
