using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class OperationalCustodyService : IOperationalCustodyService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public OperationalCustodyService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region CRUD Operations

    public async Task<OperationalCustodyDto> GetCustodyByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var custody = await _context.OperationalCustodies
            .Include(c => c.Worker)
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.Factory)
            .Include(c => c.Department)
            .Include(c => c.IssuedBy)
            .Include(c => c.ReturnReceivedBy)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (custody == null)
            throw new InvalidOperationException("Custody not found");

        return MapToDto(custody);
    }

    public async Task<OperationalCustodyDto?> GetCustodyByNumberAsync(string custodyNumber, CancellationToken cancellationToken = default)
    {
        var custody = await _context.OperationalCustodies
            .Include(c => c.Worker)
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.Factory)
            .Include(c => c.Department)
            .Include(c => c.IssuedBy)
            .FirstOrDefaultAsync(c => c.CustodyNumber == custodyNumber, cancellationToken);

        return custody != null ? MapToDto(custody) : null;
    }

    public async Task<(IEnumerable<OperationalCustodyDto> Items, int TotalCount)> GetCustodiesAsync(
        int? workerId = null,
        int? itemId = null,
        int? factoryId = null,
        int? departmentId = null,
        int? warehouseId = null,
        CustodyStatus? status = null,
        bool? isOverdue = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.OperationalCustodies
            .Include(c => c.Worker)
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.Factory)
            .Include(c => c.Department)
            .Include(c => c.IssuedBy)
            .AsQueryable();

        if (workerId.HasValue)
            query = query.Where(c => c.WorkerId == workerId.Value);

        if (itemId.HasValue)
            query = query.Where(c => c.ItemId == itemId.Value);

        if (factoryId.HasValue)
            query = query.Where(c => c.FactoryId == factoryId.Value);

        if (departmentId.HasValue)
            query = query.Where(c => c.DepartmentId == departmentId.Value);

        if (warehouseId.HasValue)
            query = query.Where(c => c.WarehouseId == warehouseId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(c => c.IssuedDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(c => c.IssuedDate <= endDate.Value);

        // Filter for overdue custodies (default 30 days)
        if (isOverdue.HasValue && isOverdue.Value)
        {
            var overdueDate = DateTime.UtcNow.AddDays(-30);
            query = query.Where(c => c.Status == CustodyStatus.Active && c.IssuedDate < overdueDate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var custodies = await query
            .OrderByDescending(c => c.IssuedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (custodies.Select(c => MapToDto(c)), totalCount);
    }

    #endregion

    #region Issue/Return/Consume/Transfer Operations

    public async Task<OperationalCustodyDto> IssueCustodyAsync(CreateCustodyDto request, int userId, CancellationToken cancellationToken = default)
    {
        // Validate worker
        var worker = await _context.Workers.FindAsync(new object[] { request.WorkerId }, cancellationToken);
        if (worker == null)
            throw new InvalidOperationException("Worker not found");

        if (!worker.IsActive)
            throw new InvalidOperationException("Cannot issue custody to inactive worker");

        // Validate item
        var item = await _context.Items.FindAsync(new object[] { request.ItemId }, cancellationToken);
        if (item == null)
            throw new InvalidOperationException("Item not found");

        // Validate inventory
        var inventory = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == request.WarehouseId && ir.ItemId == request.ItemId, cancellationToken);

        if (inventory == null || inventory.AvailableStock < request.Quantity)
            throw new InvalidOperationException("Insufficient stock in warehouse");

        // Validate custody limit
        if (request.CustodyLimit.HasValue)
        {
            var existingCustody = await GetWorkerTotalCustodyQuantityAsync(request.WorkerId, request.ItemId, cancellationToken);
            if (existingCustody + request.Quantity > request.CustodyLimit.Value)
                throw new InvalidOperationException($"Custody would exceed limit of {request.CustodyLimit.Value}");
        }

        var custodyNumber = await GenerateCustodyNumberAsync(cancellationToken);

        var custody = new OperationalCustody(
            custodyNumber: custodyNumber,
            workerId: request.WorkerId,
            itemId: request.ItemId,
            warehouseId: request.WarehouseId,
            factoryId: request.FactoryId,
            departmentId: request.DepartmentId,
            quantity: request.Quantity,
            purpose: request.Purpose,
            purposeArabic: request.PurposeArabic,
            issuedById: userId,
            createdBy: userId,
            custodyLimit: request.CustodyLimit,
            notes: request.Notes,
            notesArabic: request.NotesArabic);

        _context.OperationalCustodies.Add(custody);

        // Deduct from inventory
        inventory.AdjustGeneral(-request.Quantity, userId);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetCustodyByIdAsync(custody.Id, cancellationToken);
    }

    public async Task<OperationalCustodyDto> ReturnCustodyAsync(ReturnCustodyDto request, int userId, CancellationToken cancellationToken = default)
    {
        var custody = await _context.OperationalCustodies
            .Include(c => c.Item)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (custody == null)
            throw new InvalidOperationException("Custody not found");

        if (custody.Status != CustodyStatus.Active && custody.Status != CustodyStatus.PartiallyReturned)
            throw new InvalidOperationException("Custody is not active");

        if (request.ReturnQuantity > custody.RemainingQuantity)
            throw new InvalidOperationException("Return quantity exceeds remaining custody quantity");

        custody.Return(request.ReturnQuantity, userId, userId, request.Notes);

        // Add back to inventory
        var inventory = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == custody.WarehouseId && ir.ItemId == custody.ItemId, cancellationToken);

        if (inventory != null)
        {
            inventory.AdjustGeneral(request.ReturnQuantity, userId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetCustodyByIdAsync(custody.Id, cancellationToken);
    }

    public async Task<OperationalCustodyDto> ConsumeCustodyAsync(ConsumeCustodyDto request, int userId, CancellationToken cancellationToken = default)
    {
        var custody = await _context.OperationalCustodies.FindAsync(new object[] { request.Id }, cancellationToken);
        if (custody == null)
            throw new InvalidOperationException("Custody not found");

        if (custody.Status != CustodyStatus.Active && custody.Status != CustodyStatus.PartiallyReturned)
            throw new InvalidOperationException("Custody is not active");

        if (request.ConsumeQuantity > custody.RemainingQuantity)
            throw new InvalidOperationException("Consume quantity exceeds remaining custody quantity");

        custody.Consume(request.ConsumeQuantity, userId, request.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetCustodyByIdAsync(custody.Id, cancellationToken);
    }

    public async Task<OperationalCustodyDto> TransferCustodyAsync(TransferCustodyDto request, int userId, CancellationToken cancellationToken = default)
    {
        var custody = await _context.OperationalCustodies.FindAsync(new object[] { request.Id }, cancellationToken);
        if (custody == null)
            throw new InvalidOperationException("Custody not found");

        if (custody.Status != CustodyStatus.Active)
            throw new InvalidOperationException("Can only transfer active custodies");

        var newWorker = await _context.Workers.FindAsync(new object[] { request.NewWorkerId }, cancellationToken);
        if (newWorker == null)
            throw new InvalidOperationException("New worker not found");

        if (!newWorker.IsActive)
            throw new InvalidOperationException("Cannot transfer to inactive worker");

        custody.TransferToWorker(request.NewWorkerId, request.NewDepartmentId, userId);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetCustodyByIdAsync(custody.Id, cancellationToken);
    }

    #endregion

    #region Report Operations

    public async Task<IEnumerable<OperationalCustodyDto>> GetActiveCustodiesByWorkerAsync(int workerId, CancellationToken cancellationToken = default)
    {
        var custodies = await _context.OperationalCustodies
            .Include(c => c.Worker)
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.Factory)
            .Include(c => c.Department)
            .Include(c => c.IssuedBy)
            .Where(c => c.WorkerId == workerId && c.Status == CustodyStatus.Active)
            .OrderByDescending(c => c.IssuedDate)
            .ToListAsync(cancellationToken);

        return custodies.Select(c => MapToDto(c));
    }

    public async Task<IEnumerable<OperationalCustodyDto>> GetOverdueCustodiesAsync(int maxDays = 30, int? factoryId = null, CancellationToken cancellationToken = default)
    {
        var overdueDate = DateTime.UtcNow.AddDays(-maxDays);

        var query = _context.OperationalCustodies
            .Include(c => c.Worker)
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.Factory)
            .Include(c => c.Department)
            .Include(c => c.IssuedBy)
            .Where(c => c.Status == CustodyStatus.Active && c.IssuedDate < overdueDate);

        if (factoryId.HasValue)
            query = query.Where(c => c.FactoryId == factoryId.Value);

        var custodies = await query
            .OrderBy(c => c.IssuedDate)
            .ToListAsync(cancellationToken);

        return custodies.Select(c => MapToDto(c));
    }

    public async Task<CustodyAgingReportDto> GetCustodyAgingReportAsync(int workerId, CancellationToken cancellationToken = default)
    {
        var worker = await _context.Workers
            .Include(w => w.Department)
            .FirstOrDefaultAsync(w => w.Id == workerId, cancellationToken);

        if (worker == null)
            throw new InvalidOperationException("Worker not found");

        var custodies = await _context.OperationalCustodies
            .Include(c => c.Item)
            .Include(c => c.Warehouse)
            .Include(c => c.IssuedBy)
            .Where(c => c.WorkerId == workerId && c.Status == CustodyStatus.Active)
            .ToListAsync(cancellationToken);

        var overdueDate = DateTime.UtcNow.AddDays(-30);
        var overdueCustodies = custodies.Where(c => c.IssuedDate < overdueDate).Count();

        return new CustodyAgingReportDto
        {
            WorkerId = workerId,
            WorkerCode = worker.WorkerCode,
            WorkerName = worker.Name,
            WorkerNameArabic = worker.NameArabic,
            DepartmentName = worker.Department?.Name ?? "",
            DepartmentNameArabic = worker.Department?.NameArabic ?? "",
            TotalCustodies = custodies.Count,
            OverdueCustodies = overdueCustodies,
            TotalQuantity = custodies.Sum(c => c.RemainingQuantity),
            AverageDaysInCustody = custodies.Any() 
                ? (int)custodies.Average(c => (DateTime.UtcNow - c.IssuedDate).TotalDays) 
                : 0,
            MaxDaysInCustody = custodies.Any() 
                ? (int)custodies.Max(c => (DateTime.UtcNow - c.IssuedDate).TotalDays) 
                : 0,
            Custodies = custodies.Select(c => MapToDtoWithWorker(c, worker)).ToList()
        };
    }

    public async Task<CustodyStatisticsDto> GetCustodyStatisticsAsync(int? factoryId = null, int? departmentId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.OperationalCustodies.AsQueryable();

        if (factoryId.HasValue)
            query = query.Where(c => c.FactoryId == factoryId.Value);

        if (departmentId.HasValue)
            query = query.Where(c => c.DepartmentId == departmentId.Value);

        var custodies = await query.ToListAsync(cancellationToken);

        var activeCustodies = custodies.Where(c => c.Status == CustodyStatus.Active).ToList();
        var overdueDate = DateTime.UtcNow.AddDays(-30);

        return new CustodyStatisticsDto
        {
            TotalActiveCustodies = activeCustodies.Count,
            TotalOverdueCustodies = activeCustodies.Count(c => c.IssuedDate < overdueDate),
            TotalQuantityInCustody = activeCustodies.Sum(c => c.RemainingQuantity),
            TotalReturnedQuantity = custodies.Sum(c => c.ReturnedQuantity),
            TotalConsumedQuantity = custodies.Sum(c => c.ConsumedQuantity),
            TotalWorkersWithCustody = activeCustodies.Select(c => c.WorkerId).Distinct().Count(),
            TotalItemsInCustody = activeCustodies.Select(c => c.ItemId).Distinct().Count()
        };
    }

    public async Task<IEnumerable<CustodyByWorkerDto>> GetCustodiesByWorkerAsync(int? departmentId = null, int? factoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Workers
            .Include(w => w.Department)
            .Include(w => w.Custodies.Where(c => c.Status == CustodyStatus.Active))
                .ThenInclude(c => c.Item)
            .Where(w => w.Custodies.Any(c => c.Status == CustodyStatus.Active));

        if (departmentId.HasValue)
            query = query.Where(w => w.DepartmentId == departmentId.Value);

        if (factoryId.HasValue)
            query = query.Where(w => w.FactoryId == factoryId.Value);

        var workers = await query.ToListAsync(cancellationToken);

        return workers.Select(w => new CustodyByWorkerDto
        {
            WorkerId = w.Id,
            WorkerCode = w.WorkerCode,
            WorkerName = w.Name,
            WorkerNameArabic = w.NameArabic,
            MilitaryRank = w.MilitaryRank,
            DepartmentName = w.Department?.Name ?? "",
            DepartmentNameArabic = w.Department?.NameArabic ?? "",
            Custodies = w.Custodies
                .Where(c => c.Status == CustodyStatus.Active)
                .Select(c => MapToDtoWithWorker(c, w))
                .ToList()
        });
    }

    #endregion

    #region Validation

    public async Task<bool> ValidateCustodyLimitAsync(int workerId, int itemId, decimal quantity, CancellationToken cancellationToken = default)
    {
        var existingQuantity = await GetWorkerTotalCustodyQuantityAsync(workerId, itemId, cancellationToken);
        
        // Get item's custody limit (if any)
        var item = await _context.Items.FindAsync(new object[] { itemId }, cancellationToken);
        
        // For now, use a default limit. This can be enhanced to use per-item limits
        decimal limit = 100; // Default limit
        
        return existingQuantity + quantity <= limit;
    }

    public async Task<decimal> GetWorkerTotalCustodyQuantityAsync(int workerId, int itemId, CancellationToken cancellationToken = default)
    {
        return await _context.OperationalCustodies
            .Where(c => c.WorkerId == workerId && c.ItemId == itemId && c.Status == CustodyStatus.Active)
            .SumAsync(c => c.RemainingQuantity, cancellationToken);
    }

    #endregion

    #region Private Methods

    private async Task<string> GenerateCustodyNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.OperationalCustodies
            .CountAsync(c => c.IssuedDate >= today, cancellationToken);
        return $"CUS-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
    }

    private static OperationalCustodyDto MapToDto(OperationalCustody c)
    {
        return MapToDtoWithWorker(c, c.Worker);
    }

    private static OperationalCustodyDto MapToDtoWithWorker(OperationalCustody c, Worker? worker)
    {
        var w = worker;
        return new OperationalCustodyDto
        {
            Id = c.Id,
            CustodyNumber = c.CustodyNumber,
            WorkerId = c.WorkerId,
            WorkerCode = w?.WorkerCode ?? "",
            WorkerName = w?.Name ?? "",
            WorkerNameArabic = w?.NameArabic ?? "",
            WorkerMilitaryRank = w?.MilitaryRank,
            ItemId = c.ItemId,
            ItemCode = c.Item?.ItemCode ?? "",
            ItemName = c.Item?.Name ?? "",
            ItemNameArabic = c.Item?.NameArabic ?? "",
            Unit = c.Item?.Unit ?? "",
            WarehouseId = c.WarehouseId,
            WarehouseName = c.Warehouse?.Name ?? "",
            WarehouseNameArabic = c.Warehouse?.NameArabic ?? "",
            FactoryId = c.FactoryId,
            FactoryName = c.Factory?.Name ?? "",
            FactoryNameArabic = c.Factory?.NameArabic ?? "",
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department?.Name ?? "",
            DepartmentNameArabic = c.Department?.NameArabic ?? "",
            Quantity = c.Quantity,
            ReturnedQuantity = c.ReturnedQuantity,
            ConsumedQuantity = c.ConsumedQuantity,
            RemainingQuantity = c.RemainingQuantity,
            IssuedDate = c.IssuedDate,
            ReturnedDate = c.ReturnedDate,
            Status = c.Status,
            Purpose = c.Purpose,
            PurposeArabic = c.PurposeArabic,
            Notes = c.Notes,
            NotesArabic = c.NotesArabic,
            IssuedById = c.IssuedById,
            IssuedByName = c.IssuedBy?.FullName ?? "",
            ReturnReceivedById = c.ReturnReceivedById,
            ReturnReceivedByName = c.ReturnReceivedBy?.FullName,
            CustodyLimit = c.CustodyLimit,
            DaysInCustody = c.DaysInCustody,
            IsOverdue = c.IsOverdue(30)
        };
    }

    #endregion
}
