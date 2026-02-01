using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.Status != ProjectStatus.Completed && p.Status != ProjectStatus.Cancelled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByFactoryIdAsync(int factoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.FactoryId == factoryId)
            .ToListAsync(cancellationToken);
    }
}
