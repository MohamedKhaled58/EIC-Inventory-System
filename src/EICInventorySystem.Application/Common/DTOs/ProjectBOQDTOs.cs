using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Application.Common.DTOs;

#region ProjectBOQ DTOs

public record ProjectBOQDto
{
    public int Id { get; init; }
    public string BOQNumber { get; init; } = string.Empty;
    public int ProjectId { get; init; }
    public string ProjectCode { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public string ProjectNameArabic { get; init; } = string.Empty;
    public int FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
    public string FactoryNameArabic { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string WarehouseNameArabic { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public DateTime? RequiredDate { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public DateTime? IssuedDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public BOQStatus Status { get; init; }
    public BOQPriority Priority { get; init; }
    public decimal TotalQuantity { get; init; }
    public decimal IssuedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
    public int TotalItems { get; init; }
    public bool RequiresCommanderReserve { get; init; }
    public decimal CommanderReserveQuantity { get; init; }
    public bool CommanderApproved { get; init; }
    public int? CommanderApprovalId { get; init; }
    public string? CommanderApproverName { get; init; }
    public DateTime? CommanderApprovalDate { get; init; }
    public int? ApprovedById { get; init; }
    public string? ApprovedByName { get; init; }
    public string? ApprovalNotes { get; init; }
    public string? ApprovalNotesArabic { get; init; }
    public bool IsRemainingBOQ { get; init; }
    public int? OriginalBOQId { get; init; }
    public string? OriginalBOQNumber { get; init; }
    public string? PartialIssueReason { get; init; }
    public string? PartialIssueReasonArabic { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
    public List<ProjectBOQItemDto> Items { get; init; } = new();
}

public record ProjectBOQItemDto
{
    public int Id { get; init; }
    public int BOQId { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal IssuedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
    public decimal AvailableStock { get; init; }
    public decimal Shortfall { get; init; }
    public bool IsFromCommanderReserve { get; init; }
    public decimal CommanderReserveQuantity { get; init; }
    public string? PartialIssueReason { get; init; }
    public string? PartialIssueReasonArabic { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record CreateProjectBOQDto
{
    public int ProjectId { get; init; }
    public int FactoryId { get; init; }
    public int WarehouseId { get; init; }
    public DateTime RequiredDate { get; init; }
    public BOQPriority Priority { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
    public List<CreateProjectBOQItemDto> Items { get; init; } = new();
}

public record CreateProjectBOQItemDto
{
    public int ItemId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public bool UseCommanderReserve { get; init; }
    public decimal CommanderReserveQuantity { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record UpdateProjectBOQDto
{
    public int Id { get; init; }
    public DateTime? RequiredDate { get; init; }
    public BOQPriority Priority { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
    public List<UpdateProjectBOQItemDto> Items { get; init; } = new();
}

public record UpdateProjectBOQItemDto
{
    public int Id { get; init; }
    public int ItemId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public bool UseCommanderReserve { get; init; }
    public decimal CommanderReserveQuantity { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record ApproveBOQDto
{
    public int Id { get; init; }
    public string? ApprovalNotes { get; init; }
    public string? ApprovalNotesArabic { get; init; }
}

public record RejectBOQDto
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
    public string RejectionReasonArabic { get; init; } = string.Empty;
}

public record IssueBOQDto
{
    public int Id { get; init; }
    public bool AllowPartialIssue { get; init; }
    public string? PartialIssueReason { get; init; }
    public string? PartialIssueReasonArabic { get; init; }
    public List<IssueBOQItemDto> Items { get; init; } = new();
}

public record IssueBOQItemDto
{
    public int ItemId { get; init; }
    public decimal IssueQuantity { get; init; }
}

public record ApproveCommanderReserveDto
{
    public int Id { get; init; }
    public string? ApprovalNotes { get; init; }
    public string? ApprovalNotesArabic { get; init; }
}

public record BOQStatisticsDto
{
    public int TotalBOQs { get; init; }
    public int DraftBOQs { get; init; }
    public int PendingBOQs { get; init; }
    public int ApprovedBOQs { get; init; }
    public int PartiallyIssuedBOQs { get; init; }
    public int FullyIssuedBOQs { get; init; }
    public int CancelledBOQs { get; init; }
    public decimal TotalRequestedQuantity { get; init; }
    public decimal TotalIssuedQuantity { get; init; }
    public decimal TotalPendingQuantity { get; init; }
}

public record PendingBOQItemsDto
{
    public int ProjectId { get; init; }
    public string ProjectCode { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public string ProjectNameArabic { get; init; } = string.Empty;
    public int BOQId { get; init; }
    public string BOQNumber { get; init; } = string.Empty;
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal RequestedQuantity { get; init; }
    public decimal IssuedQuantity { get; init; }
    public decimal PendingQuantity { get; init; }
    public string? PartialIssueReason { get; init; }
    public string? PartialIssueReasonArabic { get; init; }
    public DateTime? RequiredDate { get; init; }
}

#endregion
