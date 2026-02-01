namespace EICInventorySystem.Application.Common.DTOs;

public record RequisitionDto
{
    public int Id { get; init; }
    public string RequisitionNumber { get; init; } = string.Empty;
    public int RequesterId { get; init; }
    public string RequesterName { get; init; } = string.Empty;
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public int SourceWarehouseId { get; init; }
    public string SourceWarehouseName { get; init; } = string.Empty;
    public RequisitionType Type { get; init; }
    public RequisitionStatus Status { get; init; }
    public DateTime RequestDate { get; init; }
    public DateTime? RequiredDate { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public DateTime? IssuedDate { get; init; }
    public DateTime? ReceivedDate { get; init; }
    public int? ApprovedById { get; init; }
    public string? ApprovedByName { get; init; }
    public int? IssuedById { get; init; }
    public string? IssuedByName { get; init; }
    public string? Notes { get; init; }
    public string? RejectionReason { get; init; }
    public bool RequiresCommanderReserve { get; init; }
    public bool CommanderReserveApproved { get; init; }
    public int? CommanderApprovalId { get; init; }
    public DateTime? CommanderApprovalDate { get; init; }
    public List<RequisitionItemDto> Items { get; init; } = new();
}

public record RequisitionItemDto
{
    public int Id { get; init; }
    public int RequisitionId { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal ApprovedQuantity { get; init; }
    public decimal IssuedQuantity { get; init; }
    public decimal ReceivedQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
    public decimal AvailableReserve { get; init; }
    public bool RequiresReserve { get; init; }
    public decimal ReserveQuantity { get; init; }
    public decimal ReserveApprovedQuantity { get; init; }
    public string? Notes { get; init; }
}

public record CreateRequisitionDto
{
    public int SourceWarehouseId { get; init; }
    public RequisitionType Type { get; init; }
    public DateTime? RequiredDate { get; init; }
    public int? DepartmentId { get; init; }
    public int? ProjectId { get; init; }
    public string? Notes { get; init; }
    public List<CreateRequisitionItemDto> Items { get; init; } = new();
}

public record CreateRequisitionItemDto
{
    public int ItemId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public string? Notes { get; init; }
}

public record UpdateRequisitionDto
{
    public int Id { get; init; }
    public DateTime? RequiredDate { get; init; }
    public string? Notes { get; init; }
}

public record ApproveRequisitionDto
{
    public int Id { get; init; }
    public List<ApproveRequisitionItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ApproveRequisitionItemDto
{
    public int ItemId { get; init; }
    public decimal ApprovedQuantity { get; init; }
    public bool UseCommanderReserve { get; init; }
    public decimal ReserveQuantity { get; init; }
}

public record RejectRequisitionDto
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

public record IssueRequisitionDto
{
    public int Id { get; init; }
    public List<IssueRequisitionItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record IssueRequisitionItemDto
{
    public int ItemId { get; init; }
    public decimal IssuedQuantity { get; init; }
}

public record ReceiveRequisitionDto
{
    public int Id { get; init; }
    public List<ReceiveRequisitionItemDto> Items { get; init; } = new();
    public string? Notes { get; init; }
}

public record ReceiveRequisitionItemDto
{
    public int ItemId { get; init; }
    public decimal ReceivedQuantity { get; init; }
}
