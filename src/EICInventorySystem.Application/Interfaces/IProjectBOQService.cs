using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Application.Interfaces;

/// <summary>
/// Service interface for Project BOQ operations
/// Handles BOQ creation, approval, issuance (full and partial)
/// </summary>
public interface IProjectBOQService
{
    // CRUD operations
    Task<ProjectBOQDto> GetBOQByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto?> GetBOQByNumberAsync(string boqNumber, CancellationToken cancellationToken = default);
    Task<(IEnumerable<ProjectBOQDto> Items, int TotalCount)> GetBOQsAsync(
        int? factoryId = null,
        int? projectId = null,
        int? warehouseId = null,
        BOQStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    Task<ProjectBOQDto> CreateBOQAsync(CreateProjectBOQDto request, int userId, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto> UpdateBOQAsync(UpdateProjectBOQDto request, int userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteBOQAsync(int id, int userId, CancellationToken cancellationToken = default);

    // Workflow operations
    Task<ProjectBOQDto> SubmitBOQAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto> ApproveBOQAsync(ApproveBOQDto request, int userId, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto> RejectBOQAsync(RejectBOQDto request, int userId, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto> ApproveCommanderReserveAsync(ApproveCommanderReserveDto request, int userId, CancellationToken cancellationToken = default);
    Task<ProjectBOQDto> IssueBOQAsync(IssueBOQDto request, int userId, CancellationToken cancellationToken = default);
    Task<bool> CancelBOQAsync(int id, int userId, string? reason = null, string? reasonArabic = null, CancellationToken cancellationToken = default);

    // Query operations
    Task<IEnumerable<ProjectBOQDto>> GetPendingBOQsAsync(int? factoryId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectBOQDto>> GetBOQsForApprovalAsync(int? factoryId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectBOQDto>> GetBOQsForIssuanceAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectBOQDto>> GetPartiallyIssuedBOQsAsync(int? projectId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PendingBOQItemsDto>> GetPendingItemsAsync(int? projectId = null, int? factoryId = null, CancellationToken cancellationToken = default);
    Task<BOQStatisticsDto> GetBOQStatisticsAsync(int? factoryId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    // Validation operations
    Task<bool> ValidateBOQItemsAsync(int boqId, CancellationToken cancellationToken = default);
    Task<decimal> GetAvailableStockForItemAsync(int itemId, int warehouseId, CancellationToken cancellationToken = default);
}
