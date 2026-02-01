using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ReturnRepository : Repository<Return>, IReturnRepository
{
    public ReturnRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Return?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Items)
                .ThenInclude(ri => ri.Item)
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .Include(r => r.Approver)
            .Include(r => r.Receiver)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Project)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.DepartmentId == departmentId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Items)
            .Where(r => r.WarehouseId == warehouseId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.ReturnDate >= startDate && r.ReturnDate <= endDate)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByStatusAsync(ReturnStatus status, CancellationToken cancellationToken = default)
    {
        var statusString = status.ToString();
        return await _dbSet
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.Status == statusString)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Return>> GetByFactoryAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Project)
            .Include(r => r.Department)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
            .Where(r => r.Project.FactoryId == factoryId || 
                        (r.Department != null && r.Department.FactoryId == factoryId) || 
                        r.Warehouse.FactoryId == factoryId)
            .OrderByDescending(r => r.ReturnDate)
            .ToListAsync(cancellationToken);
    }
}
