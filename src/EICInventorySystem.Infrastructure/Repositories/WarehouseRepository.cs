using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.InventoryRecords)
            .Include(w => w.OutgoingTransfers)
            .Include(w => w.IncomingTransfers)
            .FirstOrDefaultAsync(w => w.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.InventoryRecords)
            .Where(w => w.FactoryId == factoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetCentralWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.InventoryRecords)
            .Where(w => w.Type == "Central")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Warehouse>> GetFactoryWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.InventoryRecords)
            .Where(w => w.Type == "Factory")
            .ToListAsync(cancellationToken);
    }

    public async Task<Warehouse?> GetWithInventoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.InventoryRecords)
                .ThenInclude(ir => ir.Item)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Warehouse?> GetWithTransfersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.OutgoingTransfers)
                .ThenInclude(t => t.DestinationWarehouse)
            .Include(w => w.IncomingTransfers)
                .ThenInclude(t => t.SourceWarehouse)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }
}
