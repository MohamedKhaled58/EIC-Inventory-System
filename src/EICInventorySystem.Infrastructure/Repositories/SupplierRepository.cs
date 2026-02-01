using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.NameArabic)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetByItemCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        // Supplier doesn't have ItemCategories - return suppliers with purchase orders containing items from that category
        return await _dbSet
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetWithPurchaseOrdersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.PurchaseOrders)
                .ThenInclude(po => po.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
