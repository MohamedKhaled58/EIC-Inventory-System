using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Application.Common.DTOs;

#region Worker DTOs

public record WorkerDto
{
    public int Id { get; init; }
    public string WorkerCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameArabic { get; init; } = string.Empty;
    public string? MilitaryRank { get; init; }
    public string? MilitaryRankArabic { get; init; }
    public string? NationalId { get; init; }
    public string? Phone { get; init; }
    public int FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
    public string FactoryNameArabic { get; init; } = string.Empty;
    public int DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentNameArabic { get; init; } = string.Empty;
    public DateTime JoinDate { get; init; }
    public bool IsActive { get; init; }
    public int ActiveCustodyCount { get; init; }
}

public record CreateWorkerDto
{
    public string WorkerCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string NameArabic { get; init; } = string.Empty;
    public string? MilitaryRank { get; init; }
    public string? MilitaryRankArabic { get; init; }
    public string? NationalId { get; init; }
    public string? Phone { get; init; }
    public int FactoryId { get; init; }
    public int DepartmentId { get; init; }
    public DateTime? JoinDate { get; init; }
}

public record UpdateWorkerDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string NameArabic { get; init; } = string.Empty;
    public string? MilitaryRank { get; init; }
    public string? MilitaryRankArabic { get; init; }
    public string? NationalId { get; init; }
    public string? Phone { get; init; }
    public int DepartmentId { get; init; }
}

#endregion

#region Operational Custody DTOs

public record OperationalCustodyDto
{
    public int Id { get; init; }
    public string CustodyNumber { get; init; } = string.Empty;
    public int WorkerId { get; init; }
    public string WorkerCode { get; init; } = string.Empty;
    public string WorkerName { get; init; } = string.Empty;
    public string WorkerNameArabic { get; init; } = string.Empty;
    public string? WorkerMilitaryRank { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemNameArabic { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string WarehouseNameArabic { get; init; } = string.Empty;
    public int FactoryId { get; init; }
    public string FactoryName { get; init; } = string.Empty;
    public string FactoryNameArabic { get; init; } = string.Empty;
    public int DepartmentId { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentNameArabic { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal ReturnedQuantity { get; init; }
    public decimal ConsumedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
    public DateTime IssuedDate { get; init; }
    public DateTime? ReturnedDate { get; init; }
    public CustodyStatus Status { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public string PurposeArabic { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
    public int IssuedById { get; init; }
    public string IssuedByName { get; init; } = string.Empty;
    public int? ReturnReceivedById { get; init; }
    public string? ReturnReceivedByName { get; init; }
    public decimal? CustodyLimit { get; init; }
    public int DaysInCustody { get; init; }
    public bool IsOverdue { get; init; }
}

public record CreateCustodyDto
{
    public int WorkerId { get; init; }
    public int ItemId { get; init; }
    public int WarehouseId { get; init; }
    public int FactoryId { get; init; }
    public int DepartmentId { get; init; }
    public decimal Quantity { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public string PurposeArabic { get; init; } = string.Empty;
    public decimal? CustodyLimit { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record ReturnCustodyDto
{
    public int Id { get; init; }
    public decimal ReturnQuantity { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record ConsumeCustodyDto
{
    public int Id { get; init; }
    public decimal ConsumeQuantity { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record TransferCustodyDto
{
    public int Id { get; init; }
    public int NewWorkerId { get; init; }
    public int NewDepartmentId { get; init; }
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record CustodyAgingReportDto
{
    public int WorkerId { get; init; }
    public string WorkerCode { get; init; } = string.Empty;
    public string WorkerName { get; init; } = string.Empty;
    public string WorkerNameArabic { get; init; } = string.Empty;
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentNameArabic { get; init; } = string.Empty;
    public int TotalCustodies { get; init; }
    public int OverdueCustodies { get; init; }
    public decimal TotalQuantity { get; init; }
    public int AverageDaysInCustody { get; init; }
    public int MaxDaysInCustody { get; init; }
    public List<OperationalCustodyDto> Custodies { get; init; } = new();
}

public record CustodyStatisticsDto
{
    public int TotalActiveCustodies { get; init; }
    public int TotalOverdueCustodies { get; init; }
    public decimal TotalQuantityInCustody { get; init; }
    public decimal TotalReturnedQuantity { get; init; }
    public decimal TotalConsumedQuantity { get; init; }
    public int TotalWorkersWithCustody { get; init; }
    public int TotalItemsInCustody { get; init; }
}

public record CustodyByWorkerDto
{
    public int WorkerId { get; init; }
    public string WorkerCode { get; init; } = string.Empty;
    public string WorkerName { get; init; } = string.Empty;
    public string WorkerNameArabic { get; init; } = string.Empty;
    public string? MilitaryRank { get; init; }
    public string DepartmentName { get; init; } = string.Empty;
    public string DepartmentNameArabic { get; init; } = string.Empty;
    public List<OperationalCustodyDto> Custodies { get; init; } = new();
}

#endregion
