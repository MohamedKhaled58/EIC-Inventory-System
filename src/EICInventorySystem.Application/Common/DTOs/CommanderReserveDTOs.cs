namespace EICInventorySystem.Application.Common.DTOs;

public record CommanderReserveDto
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
    public decimal ReserveQuantity { get; init; }
    public decimal MinimumReserveRequired { get; init; }
    public decimal ReserveAllocated { get; init; }
    public decimal AvailableReserve { get; init; }
    public decimal ReservePercentage { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime LastUpdated { get; init; }
}

public record CommanderReserveSummaryDto
{
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string WarehouseNameArabic { get; init; } = string.Empty;
    public int TotalItems { get; init; }
    public decimal TotalReserve { get; init; }
    public decimal TotalAllocated { get; init; }
    public decimal AvailableReserve { get; init; }
    public int LowReserveItems { get; init; }
    public int CriticalReserveItems { get; init; }
    public decimal AverageReservePercentage { get; init; }
}

public record ReleaseCommanderReserveDto
{
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public decimal Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int? RequisitionId { get; init; }
    public int? TransferId { get; init; }
    public string? Notes { get; init; }
}

public record CommanderReserveReleaseDto
{
    public int Id { get; init; }
    public string ReleaseNumber { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int? RequisitionId { get; init; }
    public string? RequisitionNumber { get; init; }
    public int? TransferId { get; init; }
    public string? TransferNumber { get; init; }
    public DateTime ReleaseDate { get; init; }
    public int CommanderId { get; init; }
    public string CommanderName { get; init; } = string.Empty;
    public string? Notes { get; init; }
}

public record CommanderReserveRequestDto
{
    public int Id { get; init; }
    public string RequestNumber { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal AvailableReserve { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int RequesterId { get; init; }
    public string RequesterName { get; init; } = string.Empty;
    public int? RequisitionId { get; init; }
    public string? RequisitionNumber { get; init; }
    public DateTime RequestDate { get; init; }
    public CommanderReserveRequestStatus Status { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public int? ApprovedById { get; init; }
    public string? ApprovedByName { get; init; }
    public string? RejectionReason { get; init; }
    public string? Notes { get; init; }
}

public record CreateCommanderReserveRequestDto
{
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int? RequisitionId { get; init; }
    public string? Notes { get; init; }
}

public record ApproveCommanderReserveRequestDto
{
    public int Id { get; init; }
    public decimal ApprovedQuantity { get; init; }
    public string? Notes { get; init; }
}

public record RejectCommanderReserveRequestDto
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

public enum CommanderReserveRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Released = 3
}
