using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Where(d => d.FactoryId == factoryId)
            .ToListAsync(cancellationToken);
    }
}
