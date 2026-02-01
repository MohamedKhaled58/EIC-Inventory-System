using EICInventorySystem.Application.Common.DTOs;

namespace EICInventorySystem.Application.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryRecordDto>> GetInventoryRecordsAsync(int? warehouseId = null, int? itemId = null, string? status = null, CancellationToken cancellationToken = default);
    Task<InventoryRecordDto?> GetInventoryRecordAsync(int warehouseId, int itemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventorySummaryDto>> GetInventorySummaryAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemTransactionHistoryDto>> GetItemTransactionHistoryAsync(int itemId, int? warehouseId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<LowStockAlertDto>> GetLowStockAlertsAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReserveAlertDto>> GetReserveAlertsAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
    Task<bool> AdjustStockAsync(StockAdjustmentDto adjustment, int userId, CancellationToken cancellationToken = default);
    Task<bool> TransferStockAsync(StockTransferDto transfer, int userId, CancellationToken cancellationToken = default);
}
