using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class AdjustmentRepository : Repository<Adjustment>, IAdjustmentRepository
{
    public AdjustmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Adjustment?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Items)
                .ThenInclude(ai => ai.Item)
            .Include(a => a.Warehouse)
            .Include(a => a.CreatedBy)
            .Include(a => a.ApprovedBy)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Where(a => a.WarehouseId == warehouseId)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Where(a => a.AdjustmentDate >= startDate && a.AdjustmentDate <= endDate)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetByTypeAsync(AdjustmentType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Where(a => a.Type == type)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetByStatusAsync(AdjustmentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetByFactoryAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Where(a => a.Warehouse.FactoryId == factoryId)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Adjustment>> GetPendingApprovalsAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Include(a => a.CreatedBy)
            .Where(a => a.Status == AdjustmentStatus.Pending && 
                       a.Warehouse.FactoryId == factoryId)
            .OrderByDescending(a => a.AdjustmentDate)
            .ToListAsync(cancellationToken);
    }
}
