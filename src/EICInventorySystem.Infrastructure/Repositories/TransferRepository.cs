using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class TransferRepository : Repository<Transfer>, ITransferRepository
{
    public TransferRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Transfer?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Items)
                .ThenInclude(ti => ti.Item)
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.CreatedByUser)
            .Include(t => t.ApprovedByUser)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetBySourceWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
            .Where(t => t.SourceWarehouseId == warehouseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetByDestinationWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SourceWarehouse)
            .Include(t => t.Items)
            .Where(t => t.DestinationWarehouseId == warehouseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetPendingApprovalsAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
            .Include(t => t.CreatedByUser)
            .Where(t => t.SourceWarehouseId == warehouseId && t.Status == "Pending")
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetByStatusAsync(TransferStatus status, CancellationToken cancellationToken = default)
    {
        var statusString = status.ToString();
        return await _dbSet
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
            .Where(t => t.Status == statusString)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transfer>> GetByFactoryAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
            .Where(t => t.SourceWarehouse.FactoryId == factoryId || t.DestinationWarehouse.FactoryId == factoryId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
