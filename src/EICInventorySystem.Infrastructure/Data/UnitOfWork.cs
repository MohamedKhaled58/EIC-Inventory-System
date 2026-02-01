using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Repositories;

namespace EICInventorySystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        if (_repositories.ContainsKey(type))
        {
            return (IRepository<T>)_repositories[type];
        }

        var repositoryType = typeof(Repository<>).MakeGenericType(type);
        var repository = Activator.CreateInstance(repositoryType, _context);

        if (repository == null)
            throw new InvalidOperationException($"Could not create repository instance for type {type.Name}");

        _repositories[type] = repository;
        return (IRepository<T>)repository;
    }

    public IUserRepository UserRepository => 
        (IUserRepository)_repositories.GetOrAdd(typeof(IUserRepository), 
            () => new Repositories.UserRepository(_context));

    public IFactoryRepository FactoryRepository => 
        (IFactoryRepository)_repositories.GetOrAdd(typeof(IFactoryRepository), 
            () => new Repositories.FactoryRepository(_context));

    public IWarehouseRepository WarehouseRepository => 
        (IWarehouseRepository)_repositories.GetOrAdd(typeof(IWarehouseRepository), 
            () => new Repositories.WarehouseRepository(_context));

    public ISupplierRepository SupplierRepository => 
        (ISupplierRepository)_repositories.GetOrAdd(typeof(ISupplierRepository), 
            () => new Repositories.SupplierRepository(_context));

    public IItemRepository ItemRepository => 
        (IItemRepository)_repositories.GetOrAdd(typeof(IItemRepository), 
            () => new Repositories.ItemRepository(_context));

    public IProjectRepository ProjectRepository => 
        (IProjectRepository)_repositories.GetOrAdd(typeof(IProjectRepository), 
            () => new Repositories.ProjectRepository(_context));

    public IDepartmentRepository DepartmentRepository => 
        (IDepartmentRepository)_repositories.GetOrAdd(typeof(IDepartmentRepository), 
            () => new Repositories.DepartmentRepository(_context));

    public IRequisitionRepository RequisitionRepository => 
        (IRequisitionRepository)_repositories.GetOrAdd(typeof(IRequisitionRepository), 
            () => new Repositories.RequisitionRepository(_context));

    public ITransferRepository TransferRepository => 
        (ITransferRepository)_repositories.GetOrAdd(typeof(ITransferRepository), 
            () => new Repositories.TransferRepository(_context));

    public IReceiptRepository ReceiptRepository => 
        (IReceiptRepository)_repositories.GetOrAdd(typeof(IReceiptRepository), 
            () => new Repositories.ReceiptRepository(_context));

    public IConsumptionRepository ConsumptionRepository => 
        (IConsumptionRepository)_repositories.GetOrAdd(typeof(IConsumptionRepository), 
            () => new Repositories.ConsumptionRepository(_context));

    public IReturnRepository ReturnRepository => 
        (IReturnRepository)_repositories.GetOrAdd(typeof(IReturnRepository), 
            () => new Repositories.ReturnRepository(_context));

    public IAdjustmentRepository AdjustmentRepository => 
        (IAdjustmentRepository)_repositories.GetOrAdd(typeof(IAdjustmentRepository), 
            () => new Repositories.AdjustmentRepository(_context));

    public IAuditLogRepository AuditLogRepository => 
        (IAuditLogRepository)_repositories.GetOrAdd(typeof(IAuditLogRepository), 
            () => new Repositories.AuditLogRepository(_context));

    public INotificationRepository NotificationRepository => 
        (INotificationRepository)_repositories.GetOrAdd(typeof(INotificationRepository), 
            () => new Repositories.NotificationRepository(_context));

    public IReportRepository ReportRepository => 
        (IReportRepository)_repositories.GetOrAdd(typeof(IReportRepository), 
            () => new Repositories.ReportRepository(_context));

    public ISystemSettingsRepository SystemSettingsRepository => 
        (ISystemSettingsRepository)_repositories.GetOrAdd(typeof(ISystemSettingsRepository), 
            () => new Repositories.SystemSettingsRepository(_context));

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

internal static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        value = valueFactory();
        dictionary[key] = value;
        return value;
    }
}
