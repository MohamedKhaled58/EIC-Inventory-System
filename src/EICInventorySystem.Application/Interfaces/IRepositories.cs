using System.Linq.Expressions;
using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Domain.Entities;

namespace EICInventorySystem.Application.Interfaces;

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of Work interface for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    IUserRepository UserRepository { get; }
    IFactoryRepository FactoryRepository { get; }
    IWarehouseRepository WarehouseRepository { get; }
    ISupplierRepository SupplierRepository { get; }
    IItemRepository ItemRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IRequisitionRepository RequisitionRepository { get; }
    ITransferRepository TransferRepository { get; }
    IReceiptRepository ReceiptRepository { get; }
    IConsumptionRepository ConsumptionRepository { get; }
    IReturnRepository ReturnRepository { get; }
    IAdjustmentRepository AdjustmentRepository { get; }
    IAuditLogRepository AuditLogRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IReportRepository ReportRepository { get; }
    ISystemSettingsRepository SystemSettingsRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    // Alias for SaveChangesAsync to match usage in some commands
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}

// Specific repository interfaces - aligned to match actual implementations

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
}

public interface IFactoryRepository : IRepository<Factory>
{
    Task<IEnumerable<Factory>> GetActiveFactoriesAsync(CancellationToken cancellationToken = default);
}

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IEnumerable<Warehouse>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Warehouse>> GetCentralWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Warehouse>> GetFactoryWarehousesAsync(CancellationToken cancellationToken = default);
    Task<Warehouse?> GetWithInventoryAsync(int id, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetWithTransfersAsync(int id, CancellationToken cancellationToken = default);
}

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default);
}

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
}

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default);
}

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IEnumerable<Department>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default);
}

public interface IRequisitionRepository : IRepository<Requisition>
{
    Task<Requisition?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<IEnumerable<Requisition>> GetPendingAsync(int? warehouseId = null, CancellationToken cancellationToken = default);
}

public interface ITransferRepository : IRepository<Transfer>
{
    Task<IEnumerable<Transfer>> GetByStatusAsync(TransferStatus status, CancellationToken cancellationToken = default);
}

public interface IReceiptRepository : IRepository<Receipt>
{
    Task<IEnumerable<Receipt>> GetByStatusAsync(ReceiptStatus status, CancellationToken cancellationToken = default);
}

public interface IConsumptionRepository : IRepository<Consumption>
{
    Task<IEnumerable<Consumption>> GetByStatusAsync(ConsumptionStatus status, CancellationToken cancellationToken = default);
}

public interface IReturnRepository : IRepository<Return>
{
    Task<IEnumerable<Return>> GetByStatusAsync(ReturnStatus status, CancellationToken cancellationToken = default);
}

public interface IAdjustmentRepository : IRepository<Adjustment>
{
    Task<IEnumerable<Adjustment>> GetByStatusAsync(AdjustmentStatus status, CancellationToken cancellationToken = default);
}

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByPriorityAsync(NotificationPriority priority, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IReportRepository : IRepository<Report>
{
}

public interface ISystemSettingsRepository : IRepository<SystemSettings>
{
}
