using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
{
    public ReceiptRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Receipt?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Items)
                .ThenInclude(ri => ri.Item)
            .Include(r => r.Warehouse)
            .Include(r => r.Supplier)
            .Include(r => r.PurchaseOrder)
            .Include(r => r.Receiver)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Supplier)
            .Include(r => r.Items)
            .Where(r => r.WarehouseId == warehouseId)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetBySupplierAsync(int supplierId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.SupplierId == supplierId)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetByPurchaseOrderAsync(int purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Warehouse)
            .Include(r => r.Supplier)
            .Include(r => r.Items)
            .Where(r => r.PurchaseOrderId == purchaseOrderId)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Warehouse)
            .Include(r => r.Supplier)
            .Include(r => r.Items)
            .Where(r => r.ReceiptDate >= startDate && r.ReceiptDate <= endDate)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetByStatusAsync(ReceiptStatus status, CancellationToken cancellationToken = default)
    {
        var statusString = status.ToString();
        return await _dbSet
            .Include(r => r.Warehouse)
            .Include(r => r.Supplier)
            .Include(r => r.Items)
            .Where(r => r.Status == statusString)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Receipt>> GetByFactoryAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Warehouse)
            .Include(r => r.Supplier)
            .Include(r => r.Items)
            .Where(r => r.Warehouse.FactoryId == factoryId)
            .OrderByDescending(r => r.ReceiptDate)
            .ToListAsync(cancellationToken);
    }
}
