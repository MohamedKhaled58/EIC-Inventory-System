using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class WorkerService : IWorkerService
{
    private readonly ApplicationDbContext _context;

    public WorkerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkerDto> GetWorkerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers
            .Include(w => w.Factory)
            .Include(w => w.Department)
            .Include(w => w.Custodies.Where(c => c.Status == CustodyStatus.Active))
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (worker == null)
            throw new InvalidOperationException("Worker not found");

        return MapToDto(worker);
    }

    public async Task<WorkerDto?> GetWorkerByCodeAsync(string workerCode, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers
            .Include(w => w.Factory)
            .Include(w => w.Department)
            .Include(w => w.Custodies.Where(c => c.Status == CustodyStatus.Active))
            .FirstOrDefaultAsync(w => w.WorkerCode == workerCode, cancellationToken);

        return worker != null ? MapToDto(worker) : null;
    }

    public async Task<(IEnumerable<WorkerDto> Items, int TotalCount)> GetWorkersAsync(
        int? factoryId = null,
        int? departmentId = null,
        bool? isActive = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Workers
            .Include(w => w.Factory)
            .Include(w => w.Department)
            .Include(w => w.Custodies.Where(c => c.Status == CustodyStatus.Active))
            .AsQueryable();

        if (factoryId.HasValue)
            query = query.Where(w => w.FactoryId == factoryId.Value);

        if (departmentId.HasValue)
            query = query.Where(w => w.DepartmentId == departmentId.Value);

        if (isActive.HasValue)
            query = query.Where(w => w.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(w => 
                w.WorkerCode.ToLower().Contains(term) ||
                w.Name.ToLower().Contains(term) ||
                w.NameArabic.Contains(term) ||
                (w.NationalId != null && w.NationalId.Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var workers = await query
            .OrderBy(w => w.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (workers.Select(MapToDto), totalCount);
    }

    public async Task<WorkerDto> CreateWorkerAsync(CreateWorkerDto request, int userId, CancellationToken cancellationToken = default)
    {
        // Validate factory exists
        var factory = await _context.Factories.FindAsync(new object[] { request.FactoryId }, cancellationToken);
        if (factory == null)
            throw new InvalidOperationException("Factory not found");

        // Validate department exists
        var department = await _context.Departments.FindAsync(new object[] { request.DepartmentId }, cancellationToken);
        if (department == null)
            throw new InvalidOperationException("Department not found");

        // Check for duplicate worker code
        var existing = await _context.Workers.AnyAsync(w => w.WorkerCode == request.WorkerCode, cancellationToken);
        if (existing)
            throw new InvalidOperationException("Worker code already exists");

        var worker = new Worker(
            workerCode: request.WorkerCode,
            name: request.Name,
            nameArabic: request.NameArabic,
            factoryId: request.FactoryId,
            departmentId: request.DepartmentId,
            createdBy: userId,
            militaryRank: request.MilitaryRank,
            militaryRankArabic: request.MilitaryRankArabic,
            nationalId: request.NationalId,
            phone: request.Phone,
            joinDate: request.JoinDate);

        _context.Workers.Add(worker);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetWorkerByIdAsync(worker.Id, cancellationToken);
    }

    public async Task<WorkerDto> UpdateWorkerAsync(UpdateWorkerDto request, int userId, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (worker == null)
            throw new InvalidOperationException("Worker not found");

        worker.UpdateDetails(
            name: request.Name,
            nameArabic: request.NameArabic,
            departmentId: request.DepartmentId,
            updatedBy: userId,
            militaryRank: request.MilitaryRank,
            militaryRankArabic: request.MilitaryRankArabic,
            nationalId: request.NationalId,
            phone: request.Phone);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetWorkerByIdAsync(worker.Id, cancellationToken);
    }

    public async Task<bool> ActivateWorkerAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers.FindAsync(new object[] { id }, cancellationToken);
        if (worker == null)
            return false;

        worker.Activate(userId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateWorkerAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers
            .Include(w => w.Custodies.Where(c => c.Status == CustodyStatus.Active))
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (worker == null)
            return false;

        if (worker.Custodies.Any())
            throw new InvalidOperationException("Cannot deactivate worker with active custodies");

        worker.Deactivate(userId);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<WorkerDto> TransferWorkerToDepartmentAsync(int workerId, int newDepartmentId, int userId, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers.FindAsync(new object[] { workerId }, cancellationToken);
        if (worker == null)
            throw new InvalidOperationException("Worker not found");

        var department = await _context.Departments.FindAsync(new object[] { newDepartmentId }, cancellationToken);
        if (department == null)
            throw new InvalidOperationException("Department not found");

        worker.TransferToDepartment(newDepartmentId, userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetWorkerByIdAsync(workerId, cancellationToken);
    }

    public async Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string searchTerm, int? factoryId = null, int? departmentId = null, int maxResults = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<WorkerDto>();

        var term = searchTerm.ToLower();
        var query = _context.Workers
            .Include(w => w.Factory)
            .Include(w => w.Department)
            .Where(w => w.IsActive)
            .Where(w => 
                w.WorkerCode.ToLower().Contains(term) ||
                w.Name.ToLower().Contains(term) ||
                w.NameArabic.Contains(term));

        if (factoryId.HasValue)
            query = query.Where(w => w.FactoryId == factoryId.Value);

        if (departmentId.HasValue)
            query = query.Where(w => w.DepartmentId == departmentId.Value);

        var workers = await query
            .Take(maxResults)
            .ToListAsync(cancellationToken);

        return workers.Select(MapToDto);
    }

    private static WorkerDto MapToDto(Worker w)
    {
        return new WorkerDto
        {
            Id = w.Id,
            WorkerCode = w.WorkerCode,
            Name = w.Name,
            NameArabic = w.NameArabic,
            MilitaryRank = w.MilitaryRank,
            MilitaryRankArabic = w.MilitaryRankArabic,
            NationalId = w.NationalId,
            Phone = w.Phone,
            FactoryId = w.FactoryId,
            FactoryName = w.Factory?.Name ?? "",
            FactoryNameArabic = w.Factory?.NameArabic ?? "",
            DepartmentId = w.DepartmentId,
            DepartmentName = w.Department?.Name ?? "",
            DepartmentNameArabic = w.Department?.NameArabic ?? "",
            JoinDate = w.JoinDate,
            IsActive = w.IsActive,
            ActiveCustodyCount = w.Custodies?.Count(c => c.Status == CustodyStatus.Active) ?? 0
        };
    }
}
