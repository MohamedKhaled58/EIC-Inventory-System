namespace EICInventorySystem.Application.Interfaces;

public interface IReportService
{
    Task<byte[]> GenerateInventoryReportAsync(int? warehouseId = null, string format = "pdf", CancellationToken cancellationToken = default);
    Task<byte[]> GenerateRequisitionReportAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default);
    Task<byte[]> GenerateTransferReportAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default);
    Task<byte[]> GenerateStockMovementReportAsync(int? warehouseId = null, DateTime? startDate = null, DateTime? endDate = null, string format = "pdf", CancellationToken cancellationToken = default);
    Task<byte[]> GenerateCommanderReserveReportAsync(int? warehouseId = null, string format = "pdf", CancellationToken cancellationToken = default);
}
