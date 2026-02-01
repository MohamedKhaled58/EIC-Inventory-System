using EICInventorySystem.Application.Common.DTOs;

namespace EICInventorySystem.Application.Interfaces;

public interface ICommanderReserveService
{
    Task<IEnumerable<CommanderReserveDto>> GetCommanderReserveAsync(int? warehouseId = null, int? itemId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CommanderReserveSummaryDto>> GetCommanderReserveSummaryAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
    Task<CommanderReserveRequestDto> CreateRequestAsync(CreateCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default);
    Task<CommanderReserveRequestDto> ApproveRequestAsync(ApproveCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default);
    Task<CommanderReserveRequestDto> RejectRequestAsync(RejectCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default);
    Task<CommanderReserveReleaseDto> ReleaseReserveAsync(ReleaseCommanderReserveDto request, int userId, CancellationToken cancellationToken = default);
    Task<bool> AllocateReserveAsync(int itemId, int warehouseId, decimal quantity, int userId, CancellationToken cancellationToken = default);
    Task<bool> AdjustReserveAsync(int itemId, int warehouseId, decimal newReserveQuantity, string reason, int userId, CancellationToken cancellationToken = default);
}
