namespace EICInventorySystem.Application.Common.DTOs;

public record InventoryRecordDto
{
    public int Id { get; init; }
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string WarehouseNameArabic { get; init; } = string.Empty;
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal TotalQuantity { get; init; }
    public decimal GeneralQuantity { get; init; }
    public decimal CommanderReserveQuantity { get; init; }
    public decimal MinimumReserveRequired { get; init; }
    public decimal ReorderPoint { get; init; }
    public decimal GeneralAllocated { get; init; }
    public decimal ReserveAllocated { get; init; }
    public decimal AvailableQuantity { get; init; }
    public decimal AvailableReserve { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime LastUpdated { get; init; }
    public string? LastUpdatedBy { get; init; }
}

public record InventorySummaryDto
{
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string WarehouseNameArabic { get; init; } = string.Empty;
    public int TotalItems { get; init; }
    public decimal TotalQuantity { get; init; }
    public decimal TotalGeneralStock { get; init; }
    public decimal TotalCommanderReserve { get; init; }
    public int LowStockItems { get; init; }
    public int CriticalStockItems { get; init; }
    public int LowReserveItems { get; init; }
}

public record ItemTransactionHistoryDto
{
    public int Id { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string TransactionType { get; init; } = string.Empty;
    public string TransactionTypeArabic { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal QuantityBefore { get; init; }
    public decimal QuantityAfter { get; init; }
    public decimal GeneralQuantityBefore { get; init; }
    public decimal GeneralQuantityAfter { get; init; }
    public decimal ReserveQuantityBefore { get; init; }
    public decimal ReserveQuantityAfter { get; init; }
    public bool IsFromCommanderReserve { get; init; }
    public string? ReferenceNumber { get; init; }
    public string? ReferenceType { get; init; }
    public DateTime TransactionDate { get; init; }
    public string? PerformedBy { get; init; }
    public string? Notes { get; init; }
}

public record StockAdjustmentDto
{
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public decimal NewQuantity { get; init; }
    public decimal NewGeneralQuantity { get; init; }
    public decimal NewReserveQuantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Notes { get; init; }
}

public record StockTransferDto
{
    public int ItemId { get; init; }
    public int FromWarehouseId { get; init; }
    public int ToWarehouseId { get; init; }
    public decimal Quantity { get; init; }
    public bool FromCommanderReserve { get; init; }
    public bool ToCommanderReserve { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Notes { get; init; }
}

public record LowStockAlertDto
{
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public decimal CurrentQuantity { get; init; }
    public decimal ReorderPoint { get; init; }
    public decimal Shortage { get; init; }
    public string Severity { get; init; } = string.Empty;
    public DateTime AlertDate { get; init; }
}

public record ReserveAlertDto
{
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public decimal CurrentReserve { get; init; }
    public decimal MinimumRequired { get; init; }
    public decimal Shortage { get; init; }
    public DateTime AlertDate { get; init; }
}
