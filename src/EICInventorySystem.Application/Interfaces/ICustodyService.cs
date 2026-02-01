using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Application.Interfaces;

/// <summary>
/// Service interface for Worker entity operations
/// Workers DO NOT have system access - they are data records only
/// </summary>
public interface IWorkerService
{
    Task<WorkerDto> GetWorkerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<WorkerDto?> GetWorkerByCodeAsync(string workerCode, CancellationToken cancellationToken = default);
    Task<(IEnumerable<WorkerDto> Items, int TotalCount)> GetWorkersAsync(
        int? factoryId = null,
        int? departmentId = null,
        bool? isActive = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    Task<WorkerDto> CreateWorkerAsync(CreateWorkerDto request, int userId, CancellationToken cancellationToken = default);
    Task<WorkerDto> UpdateWorkerAsync(UpdateWorkerDto request, int userId, CancellationToken cancellationToken = default);
    Task<bool> ActivateWorkerAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<bool> DeactivateWorkerAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<WorkerDto> TransferWorkerToDepartmentAsync(int workerId, int newDepartmentId, int userId, CancellationToken cancellationToken = default);
    
    // Autocomplete search for Zero Manual Typing
    Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string searchTerm, int? factoryId = null, int? departmentId = null, int maxResults = 10, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for Operational Custody operations
/// Handles custody issuance, returns, consumption, and aging tracking
/// </summary>
public interface IOperationalCustodyService
{
    // CRUD operations
    Task<OperationalCustodyDto> GetCustodyByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationalCustodyDto?> GetCustodyByNumberAsync(string custodyNumber, CancellationToken cancellationToken = default);
    Task<(IEnumerable<OperationalCustodyDto> Items, int TotalCount)> GetCustodiesAsync(
        int? workerId = null,
        int? itemId = null,
        int? factoryId = null,
        int? departmentId = null,
        int? warehouseId = null,
        CustodyStatus? status = null,
        bool? isOverdue = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    // Issue custody to worker
    Task<OperationalCustodyDto> IssueCustodyAsync(CreateCustodyDto request, int userId, CancellationToken cancellationToken = default);
    
    // Return operations
    Task<OperationalCustodyDto> ReturnCustodyAsync(ReturnCustodyDto request, int userId, CancellationToken cancellationToken = default);
    
    // Consume operations
    Task<OperationalCustodyDto> ConsumeCustodyAsync(ConsumeCustodyDto request, int userId, CancellationToken cancellationToken = default);
    
    // Transfer operations
    Task<OperationalCustodyDto> TransferCustodyAsync(TransferCustodyDto request, int userId, CancellationToken cancellationToken = default);

    // Report operations
    Task<IEnumerable<OperationalCustodyDto>> GetActiveCustodiesByWorkerAsync(int workerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OperationalCustodyDto>> GetOverdueCustodiesAsync(int maxDays = 30, int? factoryId = null, CancellationToken cancellationToken = default);
    Task<CustodyAgingReportDto> GetCustodyAgingReportAsync(int workerId, CancellationToken cancellationToken = default);
    Task<CustodyStatisticsDto> GetCustodyStatisticsAsync(int? factoryId = null, int? departmentId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustodyByWorkerDto>> GetCustodiesByWorkerAsync(int? departmentId = null, int? factoryId = null, CancellationToken cancellationToken = default);

    // Validation
    Task<bool> ValidateCustodyLimitAsync(int workerId, int itemId, decimal quantity, CancellationToken cancellationToken = default);
    Task<decimal> GetWorkerTotalCustodyQuantityAsync(int workerId, int itemId, CancellationToken cancellationToken = default);
}
