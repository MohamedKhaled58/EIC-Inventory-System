using EICInventorySystem.Application.Common.DTOs;

namespace EICInventorySystem.Application.Interfaces;

/// <summary>
/// Service for managing requisitions
/// </summary>
public interface IRequisitionService
{
    /// <summary>
    /// Get all requisitions with optional filters and pagination
    /// </summary>
    Task<(IEnumerable<RequisitionDto> Items, int TotalCount)> GetRequisitionsAsync(
        int? factoryId = null,
        int? warehouseId = null,
        int? departmentId = null,
        int? projectId = null,
        RequisitionStatus? status = null,
        RequisitionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single requisition by ID
    /// </summary>
    Task<RequisitionDto?> GetRequisitionAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single requisition by number
    /// </summary>
    Task<RequisitionDto?> GetRequisitionByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending requisitions for a warehouse
    /// </summary>
    Task<IEnumerable<RequisitionDto>> GetPendingRequisitionsAsync(int? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get requisitions pending approval
    /// </summary>
    Task<IEnumerable<RequisitionDto>> GetRequisitionsForApprovalAsync(int? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get requisitions ready for issuance
    /// </summary>
    Task<IEnumerable<RequisitionDto>> GetRequisitionsForIssuanceAsync(int? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get requisitions ready for receiving
    /// </summary>
    Task<IEnumerable<RequisitionDto>> GetRequisitionsForReceivingAsync(int? departmentId = null, int? projectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get requisition statistics
    /// </summary>
    Task<Application.Queries.RequisitionStatisticsDto> GetRequisitionStatisticsAsync(int? factoryId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new requisition
    /// </summary>
    Task<RequisitionDto> CreateRequisitionAsync(CreateRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing requisition
    /// </summary>
    Task<RequisitionDto> UpdateRequisitionAsync(UpdateRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approve a requisition
    /// </summary>
    Task<RequisitionDto> ApproveRequisitionAsync(ApproveRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject a requisition
    /// </summary>
    Task<RequisitionDto> RejectRequisitionAsync(RejectRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issue items from a requisition
    /// </summary>
    Task<RequisitionDto> IssueRequisitionAsync(IssueRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record receipt of items from a requisition
    /// </summary>
    Task<RequisitionDto> ReceiveRequisitionAsync(ReceiveRequisitionDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a requisition
    /// </summary>
    Task<bool> CancelRequisitionAsync(int id, int userId, string reason, CancellationToken cancellationToken = default);
}
