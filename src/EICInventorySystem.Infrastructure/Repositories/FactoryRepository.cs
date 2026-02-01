using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class FactoryRepository : Repository<Factory>, IFactoryRepository
{
    public FactoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Factory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Warehouses)
            .Include(f => f.Departments)
            .Include(f => f.Projects)
            .FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Factory>> GetActiveFactoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Warehouses)
            .Where(f => f.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Factory>> GetWithWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Warehouses)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Factory>> GetWithDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Departments)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Factory>> GetWithProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Projects)
            .ToListAsync(cancellationToken);
    }
}
