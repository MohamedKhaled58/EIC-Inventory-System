using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class RequisitionService : IRequisitionService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public RequisitionService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<(IEnumerable<RequisitionDto> Items, int TotalCount)> GetRequisitionsAsync(
        int? factoryId = null,
        int? warehouseId = null,
        int? departmentId = null,
        int? projectId = null,
        RequisitionStatus? status = null,
        RequisitionType? type = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Department)
            .Include(r => r.Project)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(r => r.WarehouseId == warehouseId.Value);

        if (departmentId.HasValue)
            query = query.Where(r => r.DepartmentId == departmentId.Value);

        if (projectId.HasValue)
            query = query.Where(r => r.ProjectId == projectId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(r => r.RequestDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.RequestDate <= endDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var requisitions = await query
            .OrderByDescending(r => r.RequestDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (requisitions.Select(MapToDto), totalCount);
    }

    public async Task<RequisitionDto?> GetRequisitionAsync(int id, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Department)
            .Include(r => r.Project)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return requisition != null ? MapToDto(requisition) : null;
    }

    public async Task<RequisitionDto?> GetRequisitionByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Department)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(r => r.Number == number, cancellationToken);

        return requisition != null ? MapToDto(requisition) : null;
    }

    public async Task<IEnumerable<RequisitionDto>> GetPendingRequisitionsAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .Where(r => r.Status == RequisitionStatus.Pending);

        if (warehouseId.HasValue)
            query = query.Where(r => r.WarehouseId == warehouseId.Value);

        var requisitions = await query
            .OrderBy(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        return requisitions.Select(MapToDto);
    }

    public async Task<IEnumerable<RequisitionDto>> GetRequisitionsForApprovalAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .Where(r => r.Status == RequisitionStatus.Pending);

        if (warehouseId.HasValue)
            query = query.Where(r => r.WarehouseId == warehouseId.Value);

        var requisitions = await query
            .OrderBy(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        return requisitions.Select(MapToDto);
    }

    public async Task<IEnumerable<RequisitionDto>> GetRequisitionsForIssuanceAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .Where(r => r.Status == RequisitionStatus.Approved);

        if (warehouseId.HasValue)
            query = query.Where(r => r.WarehouseId == warehouseId.Value);

        var requisitions = await query
            .OrderBy(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        return requisitions.Select(MapToDto);
    }

    public async Task<IEnumerable<RequisitionDto>> GetRequisitionsForReceivingAsync(int? departmentId = null, int? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions
            .Include(r => r.Requester)
            .Include(r => r.Warehouse)
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .Where(r => r.Status == RequisitionStatus.PartiallyIssued || r.Status == RequisitionStatus.Approved);

        if (departmentId.HasValue)
            query = query.Where(r => r.DepartmentId == departmentId.Value);

        if (projectId.HasValue)
            query = query.Where(r => r.ProjectId == projectId.Value);

        var requisitions = await query
            .OrderBy(r => r.RequestDate)
            .ToListAsync(cancellationToken);

        return requisitions.Select(MapToDto);
    }

    public async Task<Application.Queries.RequisitionStatisticsDto> GetRequisitionStatisticsAsync(int? factoryId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Requisitions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(r => r.RequestDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(r => r.RequestDate <= endDate.Value);

        var requisitions = await query.ToListAsync(cancellationToken);

        return new Application.Queries.RequisitionStatisticsDto
        {
            TotalRequisitions = requisitions.Count,
            PendingRequisitions = requisitions.Count(r => r.Status == RequisitionStatus.Pending),
            ApprovedRequisitions = requisitions.Count(r => r.Status == RequisitionStatus.Approved),
            RejectedRequisitions = requisitions.Count(r => r.Status == RequisitionStatus.Rejected),
            IssuedRequisitions = requisitions.Count(r => r.Status == RequisitionStatus.PartiallyIssued),
            ReceivedRequisitions = requisitions.Count(r => r.Status == RequisitionStatus.Completed),
            TotalRequestedQuantity = requisitions.Sum(r => r.TotalQuantity),
            TotalApprovedQuantity = requisitions.Sum(r => r.TotalQuantity), // Use TotalQuantity as proxy
            TotalIssuedQuantity = requisitions.Sum(r => r.IssuedQuantity),
            RequisitionsRequiringReserve = requisitions.Count(r => r.RequiresCommanderReserve),
            ReserveRequisitionsApproved = requisitions.Count(r => r.CommanderApprovalId.HasValue)
        };
    }

    public async Task<RequisitionDto> CreateRequisitionAsync(CreateRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses.FindAsync(new object[] { request.SourceWarehouseId }, cancellationToken);
        if (warehouse == null)
            throw new InvalidOperationException("Warehouse not found");

        var number = await GenerateRequisitionNumberAsync(cancellationToken);

        var priority = request.Type == RequisitionType.Emergency ? RequisitionPriority.High : RequisitionPriority.Medium;

        var requisition = new Requisition(
            number: number,
            requesterId: userId,
            departmentId: request.DepartmentId,
            projectId: request.ProjectId,
            warehouseId: request.SourceWarehouseId,
            requiredDate: request.RequiredDate ?? DateTime.UtcNow.AddDays(3),
            priority: priority,
            createdBy: userId,
            reason: request.Notes);

        _context.Requisitions.Add(requisition);
        await _context.SaveChangesAsync(cancellationToken);

        // Add items
        decimal totalQuantity = 0;
        decimal totalValue = 0;
        foreach (var itemRequest in request.Items)
        {
            var item = await _context.Items.FindAsync(new object[] { itemRequest.ItemId }, cancellationToken);
            if (item == null) continue;

            var requisitionItem = new RequisitionItem(
                requisitionId: requisition.Id,
                itemId: itemRequest.ItemId,
                requestedQuantity: itemRequest.RequestedQuantity,
                unitPrice: item.StandardCost,
                createdBy: userId,
                notes: itemRequest.Notes);

            _context.RequisitionItems.Add(requisitionItem);
            totalQuantity += itemRequest.RequestedQuantity;
            totalValue += itemRequest.RequestedQuantity * item.StandardCost;
        }

        requisition.UpdateTotals(totalQuantity, totalValue, userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create requisition");
    }

    public async Task<RequisitionDto> UpdateRequisitionAsync(UpdateRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions.FindAsync(new object[] { request.Id }, cancellationToken);
        if (requisition == null)
            throw new InvalidOperationException("Requisition not found");

        if (requisition.Status != RequisitionStatus.Draft && requisition.Status != RequisitionStatus.Pending)
            throw new InvalidOperationException("Cannot update requisition in current status");

        requisition.Update(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Requisition not found");
    }

    public async Task<RequisitionDto> ApproveRequisitionAsync(ApproveRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .Include(r => r.Requester)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (requisition == null)
            throw new InvalidOperationException("Requisition not found");

        if (!requisition.IsPending())
            throw new InvalidOperationException("Requisition is not pending approval");

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.CanApproveRequisition())
            throw new InvalidOperationException("User is not authorized to approve requisitions");

        requisition.Approve(userId, request.Notes, userId);

        // Update approved quantities using UpdateQuantity method
        foreach (var approveItem in request.Items)
        {
            var item = requisition.Items.FirstOrDefault(i => i.ItemId == approveItem.ItemId);
            if (item != null)
            {
                // UpdateQuantity updates the requested quantity (serves as approval)
                item.UpdateQuantity(approveItem.ApprovedQuantity, item.UnitPrice, userId);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Notify requester
        await _notificationService.SendNotificationAsync(
            requisition.RequesterId,
            $"تم اعتماد طلب الصرف {requisition.Number}",
            $"تم اعتماد طلب الصرف الخاص بك من قبل {user.FullName}",
            "Approved",
            cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Requisition not found");
    }

    public async Task<RequisitionDto> RejectRequisitionAsync(RejectRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Requester)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (requisition == null)
            throw new InvalidOperationException("Requisition not found");

        if (!requisition.IsPending())
            throw new InvalidOperationException("Requisition is not pending approval");

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.CanApproveRequisition())
            throw new InvalidOperationException("User is not authorized to reject requisitions");

        requisition.Reject(userId, request.RejectionReason, userId);
        await _context.SaveChangesAsync(cancellationToken);

        // Notify requester
        await _notificationService.SendNotificationAsync(
            requisition.RequesterId,
            $"تم رفض طلب الصرف {requisition.Number}",
            $"تم رفض طلب الصرف الخاص بك. السبب: {request.RejectionReason}",
            "Rejected",
            cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Requisition not found");
    }

    public async Task<RequisitionDto> IssueRequisitionAsync(IssueRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (requisition == null)
            throw new InvalidOperationException("Requisition not found");

        if (!requisition.IsApproved())
            throw new InvalidOperationException("Requisition is not approved");

        decimal totalIssued = 0;
        foreach (var issueItem in request.Items)
        {
            var requisitionItem = requisition.Items.FirstOrDefault(i => i.ItemId == issueItem.ItemId);
            if (requisitionItem == null) continue;

            // Update inventory
            var inventoryRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == requisition.WarehouseId && ir.ItemId == issueItem.ItemId, cancellationToken);

            if (inventoryRecord != null)
            {
                if (inventoryRecord.GeneralQuantity - inventoryRecord.GeneralAllocated < issueItem.IssuedQuantity)
                    throw new InvalidOperationException($"Insufficient stock for item {requisitionItem.Item?.Name}");

                inventoryRecord.AdjustGeneral(-issueItem.IssuedQuantity, userId);
            }

            requisitionItem.Issue(issueItem.IssuedQuantity, userId);
            totalIssued += issueItem.IssuedQuantity;
        }

        requisition.Issue(totalIssued, userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Requisition not found");
    }

    public async Task<RequisitionDto> ReceiveRequisitionAsync(ReceiveRequisitionDto request, int userId, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (requisition == null)
            throw new InvalidOperationException("Requisition not found");

        // For receiving, we mark items as fully issued if they receive the full amount
        foreach (var receiveItem in request.Items)
        {
            var requisitionItem = requisition.Items.FirstOrDefault(i => i.ItemId == receiveItem.ItemId);
            if (requisitionItem != null)
            {
                // Issue the remaining quantity that was received
                var remainingToIssue = requisitionItem.RequestedQuantity - requisitionItem.IssuedQuantity;
                if (remainingToIssue > 0 && receiveItem.ReceivedQuantity > 0)
                {
                    var issueAmount = Math.Min(remainingToIssue, receiveItem.ReceivedQuantity);
                    requisitionItem.Issue(issueAmount, userId);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetRequisitionAsync(requisition.Id, cancellationToken) ?? throw new InvalidOperationException("Requisition not found");
    }

    public async Task<bool> CancelRequisitionAsync(int id, int userId, string reason, CancellationToken cancellationToken = default)
    {
        var requisition = await _context.Requisitions.FindAsync(new object[] { id }, cancellationToken);
        if (requisition == null)
            return false;

        if (requisition.IsCompleted())
            throw new InvalidOperationException("Cannot cancel a completed requisition");

        requisition.Cancel(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static RequisitionDto MapToDto(Requisition r)
    {
        return new RequisitionDto
        {
            Id = r.Id,
            RequisitionNumber = r.Number,
            RequesterId = r.RequesterId,
            RequesterName = r.Requester?.FullName ?? "",
            DepartmentId = r.DepartmentId,
            DepartmentName = r.Department?.Name,
            ProjectId = r.ProjectId,
            ProjectName = r.Project?.Name,
            SourceWarehouseId = r.WarehouseId,
            SourceWarehouseName = r.Warehouse?.Name ?? "",
            Type = MapPriorityToType(r.Priority),
            Status = r.Status,
            RequestDate = r.RequestDate,
            RequiredDate = r.RequiredDate,
            ApprovedDate = r.ApprovalDate,
            ApprovedById = r.ApproverId,
            ApprovedByName = r.Approver?.FullName,
            Notes = r.Reason,
            RequiresCommanderReserve = r.RequiresCommanderReserve,
            CommanderReserveApproved = r.CommanderApprovalId.HasValue,
            CommanderApprovalId = r.CommanderApprovalId,
            CommanderApprovalDate = r.CommanderApprovalDate,
            Items = r.Items.Select(i => new RequisitionItemDto
            {
                Id = i.Id,
                RequisitionId = i.RequisitionId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.ItemCode ?? "",
                ItemName = i.Item?.Name ?? "",
                ItemNameArabic = i.Item?.NameArabic ?? "",
                Unit = i.Item?.Unit ?? "",
                RequestedQuantity = i.RequestedQuantity,
                ApprovedQuantity = i.RequestedQuantity, // After approval, requested becomes approved
                IssuedQuantity = i.IssuedQuantity,
                ReceivedQuantity = i.IssuedQuantity, // Issued = Received in this model
                Notes = i.Notes
            }).ToList()
        };
    }

    private static RequisitionType MapPriorityToType(RequisitionPriority priority) => priority switch
    {
        RequisitionPriority.High or RequisitionPriority.Critical => RequisitionType.Emergency,
        _ => RequisitionType.Standard
    };

    private async Task<string> GenerateRequisitionNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.Requisitions
            .CountAsync(r => r.RequestDate >= today, cancellationToken);
        return $"REQ-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
    }
}
