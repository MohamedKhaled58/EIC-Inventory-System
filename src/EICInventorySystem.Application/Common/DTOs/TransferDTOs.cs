namespace EICInventorySystem.Application.Common.DTOs;

public record TransferDto
{
    public int Id { get; init; }
    public string TransferNumber { get; init; } = string.Empty;
    public int FromWarehouseId { get; init; }
    public string FromWarehouseName { get; init; } = string.Empty;
    public int ToWarehouseId { get; init; }
    public string ToWarehouseName { get; init; } = string.Empty;
    public TransferStatus Status { get; init; }
    public DateTime RequestDate { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public DateTime? ShippedDate { get; init; }
    public DateTime? ReceivedDate { get; init; }
    public int RequesterId { get; init; }
    public string RequesterName { get; init; } = string.Empty;
    public int? ApprovedById { get; init; }
    public string? ApprovedByName { get; init; }
    public int? ShippedById { get; init; }
    public string? ShippedByName { get; init; }
    public int? ReceivedById { get; init; }
    public string? ReceivedByName { get; init; }
    public string? Notes { get; init; }
    public string? RejectionReason { get; init; }
    public List<TransferItemDto> Items { get; init; } = new();
}

public record TransferItemDto
{
    public int Id { get; init; }
    public int TransferId { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal ApprovedQuantity { get; init; }
    public decimal ShippedQuantity { get; init; }
    public decimal ReceivedQuantity { get; init; }
    public bool FromCommanderReserve { get; init; }
    public bool ToCommanderReserve { get; init; }
    public decimal ReserveQuantity { get; init; }
    public string? Notes { get; init; }
}

public record CreateTransferDto
{
    public int FromWarehouseId { get; init; }
    public int ToWarehouseId { get; init; }
    public string? Notes { get; init; }
    public List<CreateTransferItemDto> Items { get; init; } = new();
}

public record CreateTransferItemDto
{
    public int ItemId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public bool FromCommanderReserve { get; init; }
    public bool ToCommanderReserve { get; init; }
    public string? Notes { get; init; }
}

public record ApproveTransferDto
{
    public int Id { get; init; }
    public List<ApproveTransferItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ApproveTransferItemDto
{
    public int ItemId { get; init; }
    public decimal ApprovedQuantity { get; init; }
}

public record RejectTransferDto
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

public record ShipTransferDto
{
    public int Id { get; init; }
    public List<ShipTransferItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ShipTransferItemDto
{
    public int ItemId { get; init; }
    public decimal ShippedQuantity { get; init; }
}

public record ReceiveTransferDto
{
    public int Id { get; init; }
    public List<ReceiveTransferItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ReceiveTransferItemDto
{
    public int ItemId { get; init; }
    public decimal ReceivedQuantity { get; init; }
}
