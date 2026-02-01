using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class ProjectBOQService : IProjectBOQService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ProjectBOQService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    #region CRUD Operations

    public async Task<ProjectBOQDto> GetBOQByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Warehouse)
            .Include(b => b.ApprovedBy)
            .Include(b => b.CommanderApprover)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .Include(b => b.OriginalBOQ)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        return MapToDto(boq);
    }

    public async Task<ProjectBOQDto?> GetBOQByNumberAsync(string boqNumber, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Warehouse)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(b => b.BOQNumber == boqNumber, cancellationToken);

        return boq != null ? MapToDto(boq) : null;
    }

    public async Task<(IEnumerable<ProjectBOQDto> Items, int TotalCount)> GetBOQsAsync(
        int? factoryId = null,
        int? projectId = null,
        int? warehouseId = null,
        BOQStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Warehouse)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .AsQueryable();

        if (factoryId.HasValue)
            query = query.Where(b => b.FactoryId == factoryId.Value);

        if (projectId.HasValue)
            query = query.Where(b => b.ProjectId == projectId.Value);

        if (warehouseId.HasValue)
            query = query.Where(b => b.WarehouseId == warehouseId.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(b => b.CreatedDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.CreatedDate <= endDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var boqs = await query
            .OrderByDescending(b => b.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (boqs.Select(MapToDto), totalCount);
    }

    public async Task<ProjectBOQDto> CreateBOQAsync(CreateProjectBOQDto request, int userId, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { request.ProjectId }, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var factory = await _context.Factories.FindAsync(new object[] { request.FactoryId }, cancellationToken);
        if (factory == null)
            throw new InvalidOperationException("Factory not found");

        var warehouse = await _context.Warehouses.FindAsync(new object[] { request.WarehouseId }, cancellationToken);
        if (warehouse == null)
            throw new InvalidOperationException("Warehouse not found");

        var boqNumber = await GenerateBOQNumberAsync(cancellationToken);

        var boq = new ProjectBOQ(
            boqNumber: boqNumber,
            projectId: request.ProjectId,
            factoryId: request.FactoryId,
            warehouseId: request.WarehouseId,
            requiredDate: request.RequiredDate,
            priority: request.Priority,
            createdBy: userId,
            notes: request.Notes,
            notesArabic: request.NotesArabic);

        _context.ProjectBOQs.Add(boq);
        await _context.SaveChangesAsync(cancellationToken);

        // Add items
        foreach (var itemRequest in request.Items)
        {
            var item = await _context.Items.FindAsync(new object[] { itemRequest.ItemId }, cancellationToken);
            if (item == null) continue;

            var boqItem = new ProjectBOQItem(
                boqId: boq.Id,
                itemId: itemRequest.ItemId,
                requestedQuantity: itemRequest.RequestedQuantity,
                createdBy: userId,
                isFromCommanderReserve: itemRequest.UseCommanderReserve,
                commanderReserveQuantity: itemRequest.CommanderReserveQuantity,
                notes: itemRequest.Notes,
                notesArabic: itemRequest.NotesArabic);

            // Set available stock
            var inventory = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == request.WarehouseId && ir.ItemId == itemRequest.ItemId, cancellationToken);
            if (inventory != null)
            {
                boqItem.SetAvailableStock(inventory.AvailableStock);
            }

            _context.ProjectBOQItems.Add(boqItem);
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload to get items
        boq = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstAsync(b => b.Id == boq.Id, cancellationToken);
        boq.UpdateTotals(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<ProjectBOQDto> UpdateBOQAsync(UpdateProjectBOQDto request, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (boq.Status != BOQStatus.Draft)
            throw new InvalidOperationException("Cannot update BOQ in current status");

        // Update items
        foreach (var itemRequest in request.Items)
        {
            var boqItem = boq.Items.FirstOrDefault(i => i.Id == itemRequest.Id || i.ItemId == itemRequest.ItemId);
            if (boqItem != null)
            {
                boqItem.UpdateQuantity(itemRequest.RequestedQuantity, userId);
                if (itemRequest.UseCommanderReserve)
                {
                    boqItem.MarkForCommanderReserve(itemRequest.CommanderReserveQuantity, userId);
                }
            }
        }

        boq.UpdateTotals(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<bool> DeleteBOQAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (boq == null)
            return false;

        if (boq.Status != BOQStatus.Draft)
            throw new InvalidOperationException("Can only delete draft BOQs");

        _context.ProjectBOQItems.RemoveRange(boq.Items);
        _context.ProjectBOQs.Remove(boq);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    #endregion

    #region Workflow Operations

    public async Task<ProjectBOQDto> SubmitBOQAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (boq.Status != BOQStatus.Draft)
            throw new InvalidOperationException("BOQ is not in draft status");

        if (!boq.Items.Any())
            throw new InvalidOperationException("BOQ must have at least one item");

        boq.Submit(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<ProjectBOQDto> ApproveBOQAsync(ApproveBOQDto request, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Factory)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (boq.Status != BOQStatus.Pending)
            throw new InvalidOperationException("BOQ is not pending approval");

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        boq.Approve(userId, userId, request.ApprovalNotes, request.ApprovalNotesArabic);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<ProjectBOQDto> RejectBOQAsync(RejectBOQDto request, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs.FindAsync(new object[] { request.Id }, cancellationToken);
        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (boq.Status != BOQStatus.Pending)
            throw new InvalidOperationException("BOQ is not pending approval");

        boq.Reject(userId, request.RejectionReason ?? "", request.RejectionReasonArabic ?? "", userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<ProjectBOQDto> ApproveCommanderReserveAsync(ApproveCommanderReserveDto request, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs.FindAsync(new object[] { request.Id }, cancellationToken);
        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (!boq.RequiresCommanderApproval())
            throw new InvalidOperationException("BOQ does not require commander reserve approval");

        boq.ApproveCommanderReserve(userId, userId, request.ApprovalNotes, request.ApprovalNotesArabic);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<ProjectBOQDto> IssueBOQAsync(IssueBOQDto request, int userId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (boq == null)
            throw new InvalidOperationException("BOQ not found");

        if (boq.Status != BOQStatus.Approved && boq.Status != BOQStatus.PartiallyIssued)
            throw new InvalidOperationException("BOQ must be approved before issuance");

        decimal totalIssued = 0;
        bool isPartialIssue = false;

        foreach (var issueItem in request.Items)
        {
            var boqItem = boq.Items.FirstOrDefault(i => i.ItemId == issueItem.ItemId);
            if (boqItem == null) continue;

            // Update inventory
            var inventoryRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == boq.WarehouseId && ir.ItemId == issueItem.ItemId, cancellationToken);

            if (inventoryRecord != null)
            {
                var available = inventoryRecord.AvailableStock;
                if (available < issueItem.IssueQuantity)
                    throw new InvalidOperationException($"Insufficient stock for item {boqItem.Item?.Name}");

                inventoryRecord.AdjustGeneral(-issueItem.IssueQuantity, userId);
            }

            boqItem.Issue(issueItem.IssueQuantity, userId);
            totalIssued += issueItem.IssueQuantity;

            if (boqItem.RemainingQuantity > 0)
                isPartialIssue = true;
        }

        if (isPartialIssue)
        {
            boq.IssuePartially(totalIssued, userId);

            // Create remaining BOQ for shortfall
            if (request.AllowPartialIssue)
            {
                await CreateRemainingBOQAsync(boq, request.PartialIssueReason ?? "", request.PartialIssueReasonArabic ?? "", userId, cancellationToken);
            }
        }
        else
        {
            boq.IssueFullly(userId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetBOQByIdAsync(boq.Id, cancellationToken);
    }

    public async Task<bool> CancelBOQAsync(int id, int userId, string? reason = null, string? reasonArabic = null, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs.FindAsync(new object[] { id }, cancellationToken);
        if (boq == null)
            return false;

        if (boq.Status == BOQStatus.FullyIssued || boq.Status == BOQStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a fully issued or completed BOQ");

        boq.Cancel(userId, reason, reasonArabic);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    #endregion

    #region Query Operations

    public async Task<IEnumerable<ProjectBOQDto>> GetPendingBOQsAsync(int? factoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .Where(b => b.Status == BOQStatus.Pending);

        if (factoryId.HasValue)
            query = query.Where(b => b.FactoryId == factoryId.Value);

        var boqs = await query.OrderBy(b => b.RequiredDate).ToListAsync(cancellationToken);
        return boqs.Select(MapToDto);
    }

    public async Task<IEnumerable<ProjectBOQDto>> GetBOQsForApprovalAsync(int? factoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Items)
            .Where(b => b.Status == BOQStatus.Pending);

        if (factoryId.HasValue)
            query = query.Where(b => b.FactoryId == factoryId.Value);

        var boqs = await query.OrderBy(b => b.Priority).ThenBy(b => b.RequiredDate).ToListAsync(cancellationToken);
        return boqs.Select(MapToDto);
    }

    public async Task<IEnumerable<ProjectBOQDto>> GetBOQsForIssuanceAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Warehouse)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .Where(b => b.Status == BOQStatus.Approved);

        if (warehouseId.HasValue)
            query = query.Where(b => b.WarehouseId == warehouseId.Value);

        var boqs = await query.OrderBy(b => b.Priority).ThenBy(b => b.RequiredDate).ToListAsync(cancellationToken);
        return boqs.Select(MapToDto);
    }

    public async Task<IEnumerable<ProjectBOQDto>> GetPartiallyIssuedBOQsAsync(int? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs
            .Include(b => b.Project)
            .Include(b => b.Factory)
            .Include(b => b.Items)
                .ThenInclude(i => i.Item)
            .Where(b => b.Status == BOQStatus.PartiallyIssued);

        if (projectId.HasValue)
            query = query.Where(b => b.ProjectId == projectId.Value);

        var boqs = await query.OrderBy(b => b.RequiredDate).ToListAsync(cancellationToken);
        return boqs.Select(MapToDto);
    }

    public async Task<IEnumerable<PendingBOQItemsDto>> GetPendingItemsAsync(int? projectId = null, int? factoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQItems
            .Include(i => i.BOQ)
                .ThenInclude(b => b.Project)
            .Include(i => i.Item)
            .Where(i => i.RemainingQuantity > 0 && 
                       (i.BOQ.Status == BOQStatus.Approved || i.BOQ.Status == BOQStatus.PartiallyIssued));

        if (projectId.HasValue)
            query = query.Where(i => i.BOQ.ProjectId == projectId.Value);

        if (factoryId.HasValue)
            query = query.Where(i => i.BOQ.FactoryId == factoryId.Value);

        var items = await query.ToListAsync(cancellationToken);

        return items.Select(i => new PendingBOQItemsDto
        {
            ProjectId = i.BOQ.ProjectId,
            ProjectCode = i.BOQ.Project?.Code ?? "",
            ProjectName = i.BOQ.Project?.Name ?? "",
            ProjectNameArabic = i.BOQ.Project?.NameArabic ?? "",
            BOQId = i.BOQId,
            BOQNumber = i.BOQ.BOQNumber,
            ItemId = i.ItemId,
            ItemCode = i.Item?.ItemCode ?? "",
            ItemName = i.Item?.Name ?? "",
            ItemNameArabic = i.Item?.NameArabic ?? "",
            Unit = i.Item?.Unit ?? "",
            RequestedQuantity = i.RequestedQuantity,
            IssuedQuantity = i.IssuedQuantity,
            PendingQuantity = i.RemainingQuantity,
            PartialIssueReason = i.PartialIssueReason,
            PartialIssueReasonArabic = i.PartialIssueReasonArabic,
            RequiredDate = i.BOQ.RequiredDate
        });
    }

    public async Task<BOQStatisticsDto> GetBOQStatisticsAsync(int? factoryId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectBOQs.AsQueryable();

        if (factoryId.HasValue)
            query = query.Where(b => b.FactoryId == factoryId.Value);

        if (startDate.HasValue)
            query = query.Where(b => b.CreatedDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.CreatedDate <= endDate.Value);

        var boqs = await query.ToListAsync(cancellationToken);

        return new BOQStatisticsDto
        {
            TotalBOQs = boqs.Count,
            DraftBOQs = boqs.Count(b => b.Status == BOQStatus.Draft),
            PendingBOQs = boqs.Count(b => b.Status == BOQStatus.Pending),
            ApprovedBOQs = boqs.Count(b => b.Status == BOQStatus.Approved),
            PartiallyIssuedBOQs = boqs.Count(b => b.Status == BOQStatus.PartiallyIssued),
            FullyIssuedBOQs = boqs.Count(b => b.Status == BOQStatus.FullyIssued),
            CancelledBOQs = boqs.Count(b => b.Status == BOQStatus.Cancelled),
            TotalRequestedQuantity = boqs.Sum(b => b.TotalQuantity),
            TotalIssuedQuantity = boqs.Sum(b => b.IssuedQuantity),
            TotalPendingQuantity = boqs.Where(b => b.Status == BOQStatus.Pending || b.Status == BOQStatus.Approved)
                                        .Sum(b => b.RemainingQuantity)
        };
    }

    public async Task<bool> ValidateBOQItemsAsync(int boqId, CancellationToken cancellationToken = default)
    {
        var boq = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == boqId, cancellationToken);

        return boq?.Items.Any() ?? false;
    }

    public async Task<decimal> GetAvailableStockForItemAsync(int itemId, int warehouseId, CancellationToken cancellationToken = default)
    {
        var inventory = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == warehouseId && ir.ItemId == itemId, cancellationToken);

        return inventory?.AvailableStock ?? 0;
    }

    #endregion

    #region Private Methods

    private async Task<string> GenerateBOQNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.ProjectBOQs
            .CountAsync(b => b.CreatedDate >= today, cancellationToken);
        return $"BOQ-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
    }

    private async Task CreateRemainingBOQAsync(ProjectBOQ originalBOQ, string partialIssueReason, string partialIssueReasonArabic, int userId, CancellationToken cancellationToken)
    {
        var remainingItems = originalBOQ.Items
            .Where(i => i.RemainingQuantity > 0)
            .ToList();

        if (!remainingItems.Any()) return;

        var boqNumber = await GenerateBOQNumberAsync(cancellationToken);

        var remainingBOQ = ProjectBOQ.CreateRemainingBOQ(
            boqNumber,
            originalBOQ,
            partialIssueReason,
            partialIssueReasonArabic,
            userId);

        _context.ProjectBOQs.Add(remainingBOQ);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var item in remainingItems)
        {
            var newItem = new ProjectBOQItem(
                boqId: remainingBOQ.Id,
                itemId: item.ItemId,
                requestedQuantity: item.RemainingQuantity,
                createdBy: userId,
                isFromCommanderReserve: false,
                commanderReserveQuantity: 0,
                notes: null,
                notesArabic: null);

            _context.ProjectBOQItems.Add(newItem);
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        // Update totals
        remainingBOQ = await _context.ProjectBOQs
            .Include(b => b.Items)
            .FirstAsync(b => b.Id == remainingBOQ.Id, cancellationToken);
        remainingBOQ.UpdateTotals(userId);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static ProjectBOQDto MapToDto(ProjectBOQ boq)
    {
        return new ProjectBOQDto
        {
            Id = boq.Id,
            BOQNumber = boq.BOQNumber,
            ProjectId = boq.ProjectId,
            ProjectCode = boq.Project?.Code ?? "",
            ProjectName = boq.Project?.Name ?? "",
            ProjectNameArabic = boq.Project?.NameArabic ?? "",
            FactoryId = boq.FactoryId,
            FactoryName = boq.Factory?.Name ?? "",
            FactoryNameArabic = boq.Factory?.NameArabic ?? "",
            WarehouseId = boq.WarehouseId,
            WarehouseName = boq.Warehouse?.Name ?? "",
            WarehouseNameArabic = boq.Warehouse?.NameArabic ?? "",
            CreatedDate = boq.CreatedDate,
            RequiredDate = boq.RequiredDate,
            ApprovedDate = boq.ApprovedDate,
            IssuedDate = boq.IssuedDate,
            CompletedDate = boq.CompletedDate,
            Status = boq.Status,
            Priority = boq.Priority,
            TotalQuantity = boq.TotalQuantity,
            IssuedQuantity = boq.IssuedQuantity,
            RemainingQuantity = boq.RemainingQuantity,
            TotalItems = boq.Items.Count,
            RequiresCommanderReserve = boq.RequiresCommanderApproval(),
            CommanderReserveQuantity = boq.CommanderReserveQuantity,
            CommanderApproved = boq.CommanderApprovalId.HasValue,
            CommanderApprovalId = boq.CommanderApprovalId,
            CommanderApproverName = boq.CommanderApprover?.FullName,
            CommanderApprovalDate = boq.CommanderApprovalDate,
            ApprovedById = boq.ApprovedById,
            ApprovedByName = boq.ApprovedBy?.FullName,
            ApprovalNotes = boq.ApprovalNotes,
            ApprovalNotesArabic = boq.ApprovalNotesArabic,
            IsRemainingBOQ = boq.IsRemainingBOQ,
            OriginalBOQId = boq.OriginalBOQId,
            OriginalBOQNumber = boq.OriginalBOQ?.BOQNumber,
            PartialIssueReason = boq.PartialIssueReason,
            PartialIssueReasonArabic = boq.PartialIssueReasonArabic,
            Notes = boq.Notes,
            NotesArabic = boq.NotesArabic,
            Items = boq.Items.Select(i => new ProjectBOQItemDto
            {
                Id = i.Id,
                BOQId = i.BOQId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.ItemCode ?? "",
                ItemName = i.Item?.Name ?? "",
                ItemNameArabic = i.Item?.NameArabic ?? "",
                Unit = i.Item?.Unit ?? "",
                RequestedQuantity = i.RequestedQuantity,
                IssuedQuantity = i.IssuedQuantity,
                RemainingQuantity = i.RemainingQuantity,
                AvailableStock = i.AvailableStock,
                Shortfall = i.Shortfall,
                IsFromCommanderReserve = i.IsFromCommanderReserve,
                CommanderReserveQuantity = i.CommanderReserveQuantity,
                PartialIssueReason = i.PartialIssueReason,
                PartialIssueReasonArabic = i.PartialIssueReasonArabic,
                Notes = i.Notes,
                NotesArabic = i.NotesArabic
            }).ToList()
        };
    }

    #endregion
}
