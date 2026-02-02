using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(
        int? factoryId = null,
        ProjectStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .Include(p => p.Factory)
            .AsNoTracking()
            .AsQueryable();

        if (factoryId.HasValue)
        {
            query = query.Where(p => p.FactoryId == factoryId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                NameArabic = p.NameArabic,
                Code = p.Code,
                Description = p.Description,
                Status = p.Status,
                Budget = p.Budget,
                SpentAmount = p.SpentAmount,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                FactoryId = p.FactoryId,
                FactoryName = p.Factory.Name
            })
            .ToListAsync(cancellationToken);

        return projects;
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .Include(p => p.Factory)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project == null) return null;

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            NameArabic = project.NameArabic,
            Code = project.Code,
            Description = project.Description,
            Status = project.Status,
            Budget = project.Budget,
            SpentAmount = project.SpentAmount,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            FactoryId = project.FactoryId,
            FactoryName = project.Factory.Name
        };
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = dto.Name,
            NameArabic = dto.NameArabic,
            Code = dto.Code,
            Description = dto.Description,
            Budget = dto.Budget,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            FactoryId = dto.FactoryId,
            Status = ProjectStatus.Active,
            SpentAmount = 0
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload to get factory name
        return await GetProjectByIdAsync(project.Id, cancellationToken) 
            ?? throw new InvalidOperationException("Project created but not found");
    }

    public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { dto.Id }, cancellationToken);
        if (project == null) throw new KeyNotFoundException($"Project with ID {dto.Id} not found");

        project.Name = dto.Name;
        project.NameArabic = dto.NameArabic;
        project.Description = dto.Description;
        project.Status = dto.Status;
        project.Budget = dto.Budget;
        project.EndDate = dto.EndDate;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetProjectByIdAsync(project.Id, cancellationToken)
            ?? throw new InvalidOperationException("Project updated but not found");
    }

    public async Task<bool> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
