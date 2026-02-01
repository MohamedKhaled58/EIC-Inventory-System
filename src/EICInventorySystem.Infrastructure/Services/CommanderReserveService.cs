using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class CommanderReserveService : ICommanderReserveService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public CommanderReserveService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<CommanderReserveDto>> GetCommanderReserveAsync(int? warehouseId = null, int? itemId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Include(ir => ir.Item)
            .Where(ir => ir.CommanderReserveQuantity > 0)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        if (itemId.HasValue)
            query = query.Where(ir => ir.ItemId == itemId.Value);

        var records = await query.ToListAsync(cancellationToken);

        return records.Select(ir => new CommanderReserveDto
        {
            Id = ir.Id,
            WarehouseId = ir.WarehouseId,
            WarehouseName = ir.Warehouse?.Name ?? "",
            WarehouseNameArabic = ir.Warehouse?.NameArabic ?? "",
            ItemId = ir.ItemId,
            ItemCode = ir.Item?.ItemCode ?? "",
            ItemName = ir.Item?.Name ?? "",
            ItemNameArabic = ir.Item?.NameArabic ?? "",
            Unit = ir.Item?.Unit ?? "",
            TotalQuantity = ir.TotalQuantity,
            GeneralQuantity = ir.GeneralQuantity,
            ReserveQuantity = ir.CommanderReserveQuantity,
            MinimumReserveRequired = ir.MinimumReserveRequired,
            ReserveAllocated = ir.ReserveAllocated,
            AvailableReserve = ir.CommanderReserveQuantity - ir.ReserveAllocated,
            ReservePercentage = ir.GetReservePercentage(),
            Status = ir.IsReserveBelowMinimum() ? "Low" : "Normal",
            LastUpdated = ir.UpdatedAt ?? ir.CreatedAt
        });
    }

    public async Task<IEnumerable<CommanderReserveSummaryDto>> GetCommanderReserveSummaryAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Where(ir => ir.CommanderReserveQuantity > 0)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        var grouped = await query
            .GroupBy(ir => new { ir.WarehouseId, ir.Warehouse!.Name, ir.Warehouse.NameArabic })
            .Select(g => new CommanderReserveSummaryDto
            {
                WarehouseId = g.Key.WarehouseId,
                WarehouseName = g.Key.Name,
                WarehouseNameArabic = g.Key.NameArabic,
                TotalItems = g.Count(),
                TotalReserve = g.Sum(ir => ir.CommanderReserveQuantity),
                TotalAllocated = g.Sum(ir => ir.ReserveAllocated),
                AvailableReserve = g.Sum(ir => ir.CommanderReserveQuantity - ir.ReserveAllocated),
                LowReserveItems = g.Count(ir => ir.CommanderReserveQuantity < ir.MinimumReserveRequired),
                CriticalReserveItems = g.Count(ir => ir.CommanderReserveQuantity < ir.MinimumReserveRequired * 0.5m),
                AverageReservePercentage = g.Average(ir => ir.TotalQuantity > 0 ? (ir.CommanderReserveQuantity / ir.TotalQuantity) * 100 : 0)
            })
            .ToListAsync(cancellationToken);

        return grouped;
    }

    public async Task<CommanderReserveRequestDto> CreateRequestAsync(CreateCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        // Get item and warehouse info
        var item = await _context.Items.FindAsync(new object[] { request.ItemId }, cancellationToken);
        var warehouse = await _context.Warehouses.FindAsync(new object[] { request.WarehouseId }, cancellationToken);
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (item == null) throw new InvalidOperationException("Item not found");
        if (warehouse == null) throw new InvalidOperationException("Warehouse not found");

        // Check available reserve
        var inventoryRecord = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == request.WarehouseId && ir.ItemId == request.ItemId, cancellationToken);

        var availableReserve = inventoryRecord != null
            ? inventoryRecord.CommanderReserveQuantity - inventoryRecord.ReserveAllocated
            : 0;

        if (request.RequestedQuantity > availableReserve)
            throw new InvalidOperationException($"Requested quantity ({request.RequestedQuantity}) exceeds available reserve ({availableReserve})");

        // Generate request number
        var requestNumber = $"CRR-{DateTime.UtcNow:yyyyMMdd}-{await GetNextRequestSequenceAsync(cancellationToken):D4}";

        // Allocate the reserve
        inventoryRecord?.AllocateReserve(request.RequestedQuantity, userId);
        await _context.SaveChangesAsync(cancellationToken);

        // Notify commanders
        await _notificationService.SendNotificationToRoleAsync(
            "ComplexCommander",
            $"طلب احتياطي القائد - {requestNumber}",
            $"طلب جديد للحصول على {request.RequestedQuantity} {item.Unit} من {item.Name} من مخزن {warehouse.Name}",
            "ApprovalRequired",
            cancellationToken);

        return new CommanderReserveRequestDto
        {
            Id = 0,
            RequestNumber = requestNumber,
            WarehouseId = request.WarehouseId,
            WarehouseName = warehouse.Name,
            ItemId = request.ItemId,
            ItemCode = item.ItemCode,
            ItemName = item.Name,
            ItemNameArabic = item.NameArabic,
            Unit = item.Unit,
            RequestedQuantity = request.RequestedQuantity,
            AvailableReserve = availableReserve,
            Reason = request.Reason,
            RequesterId = userId,
            RequesterName = user?.FullName ?? "",
            RequisitionId = request.RequisitionId,
            RequestDate = DateTime.UtcNow,
            Status = CommanderReserveRequestStatus.Pending,
            Notes = request.Notes
        };
    }

    public async Task<CommanderReserveRequestDto> ApproveRequestAsync(ApproveCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null || !user.CanApproveCommanderReserve())
            throw new InvalidOperationException("User is not authorized to approve commander reserve requests");

        // In a full implementation, we would have a CommanderReserveRequest entity
        // For now, return the approved status
        return new CommanderReserveRequestDto
        {
            Id = request.Id,
            Status = CommanderReserveRequestStatus.Approved,
            ApprovedDate = DateTime.UtcNow,
            ApprovedById = userId,
            ApprovedByName = user.FullName,
            Notes = request.Notes
        };
    }

    public async Task<CommanderReserveRequestDto> RejectRequestAsync(RejectCommanderReserveRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null || !user.CanApproveCommanderReserve())
            throw new InvalidOperationException("User is not authorized to reject commander reserve requests");

        return new CommanderReserveRequestDto
        {
            Id = request.Id,
            Status = CommanderReserveRequestStatus.Rejected,
            RejectionReason = request.RejectionReason
        };
    }

    public async Task<CommanderReserveReleaseDto> ReleaseReserveAsync(ReleaseCommanderReserveDto request, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.CanAccessCommanderReserve())
            throw new InvalidOperationException("User is not authorized to release commander reserve");

        var record = await _context.InventoryRecords
            .Include(ir => ir.Item)
            .Include(ir => ir.Warehouse)
            .FirstOrDefaultAsync(ir => ir.WarehouseId == request.WarehouseId && ir.ItemId == request.ItemId, cancellationToken);

        if (record == null)
            throw new InvalidOperationException("Inventory record not found");

        if (record.CommanderReserveQuantity - record.ReserveAllocated < request.Quantity)
            throw new InvalidOperationException($"Insufficient available reserve. Available: {record.CommanderReserveQuantity - record.ReserveAllocated}");

        // Release reserve to general stock
        record.AdjustReserve(-request.Quantity, userId);
        record.AdjustGeneral(request.Quantity, userId);

        await _context.SaveChangesAsync(cancellationToken);

        var releaseNumber = $"CRL-{DateTime.UtcNow:yyyyMMdd}-{await GetNextReleaseSequenceAsync(cancellationToken):D4}";

        // Get requisition number if exists
        string? requisitionNumber = null;
        if (request.RequisitionId.HasValue)
        {
            var requisition = await _context.Requisitions.FindAsync(new object[] { request.RequisitionId.Value }, cancellationToken);
            requisitionNumber = requisition?.Number;
        }

        // Get transfer number if exists
        string? transferNumber = null;
        if (request.TransferId.HasValue)
        {
            var transfer = await _context.Transfers.FindAsync(new object[] { request.TransferId.Value }, cancellationToken);
            transferNumber = transfer?.TransferNumber;
        }

        return new CommanderReserveReleaseDto
        {
            Id = 0,
            ReleaseNumber = releaseNumber,
            WarehouseId = request.WarehouseId,
            WarehouseName = record.Warehouse?.Name ?? "",
            ItemId = request.ItemId,
            ItemCode = record.Item?.ItemCode ?? "",
            ItemName = record.Item?.Name ?? "",
            ItemNameArabic = record.Item?.NameArabic ?? "",
            Unit = record.Item?.Unit ?? "",
            Quantity = request.Quantity,
            Reason = request.Reason,
            RequisitionId = request.RequisitionId,
            RequisitionNumber = requisitionNumber,
            TransferId = request.TransferId,
            TransferNumber = transferNumber,
            ReleaseDate = DateTime.UtcNow,
            CommanderId = userId,
            CommanderName = user.FullName,
            Notes = request.Notes
        };
    }

    public async Task<bool> AllocateReserveAsync(int itemId, int warehouseId, decimal quantity, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.CanAccessCommanderReserve())
            throw new InvalidOperationException("User is not authorized to allocate commander reserve");

        var record = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == warehouseId && ir.ItemId == itemId, cancellationToken);

        if (record == null)
            throw new InvalidOperationException("Inventory record not found");

        if (record.GeneralQuantity - record.GeneralAllocated < quantity)
            throw new InvalidOperationException("Insufficient general stock for allocation to reserve");

        // Move from general to reserve
        record.AdjustGeneral(-quantity, userId);
        record.AdjustReserve(quantity, userId);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AdjustReserveAsync(int itemId, int warehouseId, decimal newReserveQuantity, string reason, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || !user.CanAccessCommanderReserve())
            throw new InvalidOperationException("User is not authorized to adjust commander reserve");

        var record = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == warehouseId && ir.ItemId == itemId, cancellationToken);

        if (record == null)
            throw new InvalidOperationException("Inventory record not found");

        var currentReserve = record.CommanderReserveQuantity;
        var difference = newReserveQuantity - currentReserve;

        if (difference > 0)
        {
            // Adding to reserve from general
            if (record.GeneralQuantity - record.GeneralAllocated < difference)
                throw new InvalidOperationException("Insufficient general stock for reserve adjustment");

            record.AdjustGeneral(-difference, userId);
            record.AdjustReserve(difference, userId);
        }
        else if (difference < 0)
        {
            // Reducing reserve to general
            record.AdjustReserve(difference, userId);
            record.AdjustGeneral(-difference, userId);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<int> GetNextRequestSequenceAsync(CancellationToken cancellationToken)
    {
        // Simple sequence generator - in production use a proper sequence table
        var today = DateTime.UtcNow.Date;
        var count = await _context.AuditLogs
            .CountAsync(a => a.Action == "CommanderReserveRequested" && a.Timestamp >= today, cancellationToken);
        return count + 1;
    }

    private async Task<int> GetNextReleaseSequenceAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.AuditLogs
            .CountAsync(a => a.Action == "CommanderReserveReleased" && a.Timestamp >= today, cancellationToken);
        return count + 1;
    }
}
