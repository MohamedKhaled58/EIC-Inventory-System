using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class RequisitionRepository : Repository<Requisition>, IRequisitionRepository
{
    public RequisitionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Requisition?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await _context.Requisitions
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Number == number, cancellationToken);
    }

    public async Task<IEnumerable<Requisition>> GetPendingAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Where(r => r.Status == RequisitionStatus.Pending);

        if (warehouseId.HasValue)
        {
            query = query.Where(r => r.WarehouseId == warehouseId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
