namespace EICInventorySystem.Application.Common.DTOs;

public record WarehouseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public int? FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
    public string FactoryNameAr { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public record ItemDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryNameAr { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public string UnitAr { get; init; } = string.Empty;
    public decimal MinimumStock { get; init; }
    public decimal ReorderLevel { get; init; }
    public bool IsActive { get; init; }
    public bool IsCritical { get; init; }
    public decimal ReservePercentage { get; init; }
    public decimal? UnitPrice { get; init; }
    
    // Inventory properties (populated when warehouseId is specified)
    public decimal? AvailableQuantity { get; set; }
    public decimal? TotalQuantity { get; set; }
    public decimal? ReservedQuantity { get; set; }
    public decimal? CommanderReserveQuantity { get; set; }
}

public record ItemCategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}

public record ItemWithInventoryDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal CurrentQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
    public decimal ReservedQuantity { get; init; }
    public decimal CommanderReserve { get; init; }
}

public record FactoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public record DepartmentDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public int FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
}

public record ProjectDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public int FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string ManagerName { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
