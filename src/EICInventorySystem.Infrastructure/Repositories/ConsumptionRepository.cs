using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ConsumptionRepository : Repository<Consumption>, IConsumptionRepository
{
    public ConsumptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Consumption?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Item)
            .Include(c => c.Project)
            .Include(c => c.Department)
            .Include(c => c.CreatedBy)
            .Include(c => c.ApprovedBy)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Consumption>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Department)
            .Include(c => c.Items)
            .Where(c => c.ProjectId == projectId)
            .OrderByDescending(c => c.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Consumption>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Project)
            .Include(c => c.Items)
            .Where(c => c.DepartmentId == departmentId)
            .OrderByDescending(c => c.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Consumption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Project)
            .Include(c => c.Department)
            .Include(c => c.Items)
            .Where(c => c.ConsumptionDate >= startDate && c.ConsumptionDate <= endDate)
            .OrderByDescending(c => c.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Consumption>> GetByStatusAsync(ConsumptionStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Project)
            .Include(c => c.Department)
            .Include(c => c.Items)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Consumption>> GetByFactoryAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Project)
            .Include(c => c.Department)
            .Include(c => c.Items)
            .Where(c => (c.Project != null && c.Project.FactoryId == factoryId) || (c.Department != null && c.Department.FactoryId == factoryId))
            .OrderByDescending(c => c.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalConsumptionByItemAsync(int itemId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(c => c.Items)
            .Where(c => c.Status == ConsumptionStatus.Approved);

        if (startDate.HasValue)
        {
            query = query.Where(c => c.ConsumptionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.ConsumptionDate <= endDate.Value);
        }

        var consumptions = await query.ToListAsync(cancellationToken);
        return consumptions
            .SelectMany(c => c.Items)
            .Where(ci => ci.ItemId == itemId)
            .Sum(ci => ci.Quantity);
    }
}
