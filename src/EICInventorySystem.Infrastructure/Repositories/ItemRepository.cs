using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Item?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .FirstOrDefaultAsync(i => i.ItemCode == code, cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Where(i => i.Category == category)
            .ToListAsync(cancellationToken);
    }
}
