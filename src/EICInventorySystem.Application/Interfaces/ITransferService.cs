using EICInventorySystem.Application.Common.DTOs;

namespace EICInventorySystem.Application.Interfaces;

/// <summary>
/// Service for managing transfers between warehouses
/// </summary>
public interface ITransferService
{
    /// <summary>
    /// Get all transfers with optional filters
    /// </summary>
    Task<IEnumerable<TransferDto>> GetTransfersAsync(
        int? fromWarehouseId = null,
        int? toWarehouseId = null,
        TransferStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single transfer by ID
    /// </summary>
    Task<TransferDto?> GetTransferByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single transfer by number
    /// </summary>
    Task<TransferDto?> GetTransferByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transfers pending approval for a user
    /// </summary>
    Task<IEnumerable<TransferDto>> GetPendingApprovalsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new transfer request
    /// </summary>
    Task<TransferDto> CreateTransferAsync(CreateTransferDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approve a transfer
    /// </summary>
    Task<TransferDto> ApproveTransferAsync(ApproveTransferDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject a transfer
    /// </summary>
    Task<TransferDto> RejectTransferAsync(RejectTransferDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ship items for a transfer
    /// </summary>
    Task<TransferDto> ShipTransferAsync(ShipTransferDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Receive items from a transfer
    /// </summary>
    Task<TransferDto> ReceiveTransferAsync(ReceiveTransferDto request, int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a transfer
    /// </summary>
    Task<bool> CancelTransferAsync(int id, int userId, string reason, CancellationToken cancellationToken = default);
}
