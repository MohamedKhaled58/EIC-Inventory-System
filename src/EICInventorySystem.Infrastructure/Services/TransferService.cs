using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using EICInventorySystem.Domain.Enums;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class TransferService : ITransferService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public TransferService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<TransferDto>> GetTransfersAsync(
        int? fromWarehouseId = null,
        int? toWarehouseId = null,
        TransferStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Transfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.CreatedByUser)
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .AsQueryable();

        if (fromWarehouseId.HasValue)
            query = query.Where(t => t.SourceWarehouseId == fromWarehouseId.Value);

        if (toWarehouseId.HasValue)
            query = query.Where(t => t.DestinationWarehouseId == toWarehouseId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value.ToString());

        if (startDate.HasValue)
            query = query.Where(t => t.TransferDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.TransferDate <= endDate.Value);

        var transfers = await query
            .OrderByDescending(t => t.TransferDate)
            .Take(500)
            .ToListAsync(cancellationToken);

        return transfers.Select(MapToDto);
    }

    public async Task<TransferDto?> GetTransferByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.CreatedByUser)
            .Include(t => t.ApprovedByUser)
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return transfer != null ? MapToDto(transfer) : null;
    }

    public async Task<TransferDto?> GetTransferByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(t => t.TransferNumber == number, cancellationToken);

        return transfer != null ? MapToDto(transfer) : null;
    }

    public async Task<IEnumerable<TransferDto>> GetPendingApprovalsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
            return Enumerable.Empty<TransferDto>();

        var query = _context.Transfers
            .Include(t => t.SourceWarehouse)
            .Include(t => t.DestinationWarehouse)
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .Where(t => t.Status == "Pending");

        // Filter by warehouse if user is warehouse-specific
        if (user.WarehouseId.HasValue)
            query = query.Where(t => t.SourceWarehouseId == user.WarehouseId.Value);

        var transfers = await query
            .OrderBy(t => t.TransferDate)
            .ToListAsync(cancellationToken);

        return transfers.Select(MapToDto);
    }

    public async Task<TransferDto> CreateTransferAsync(CreateTransferDto request, int userId, CancellationToken cancellationToken = default)
    {
        var fromWarehouse = await _context.Warehouses.FindAsync(new object[] { request.FromWarehouseId }, cancellationToken);
        var toWarehouse = await _context.Warehouses.FindAsync(new object[] { request.ToWarehouseId }, cancellationToken);

        if (fromWarehouse == null || toWarehouse == null)
            throw new InvalidOperationException("Invalid warehouse");

        var number = await GenerateTransferNumberAsync(cancellationToken);

        var transfer = new Transfer(
            transferNumber: number,
            createdByUserId: userId,
            sourceWarehouseId: request.FromWarehouseId,
            destinationWarehouseId: request.ToWarehouseId,
            requiredDate: DateTime.UtcNow.AddDays(7),
            priority: "Medium",
            createdBy: userId,
            notes: request.Notes);

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        // Add items
        decimal totalQuantity = 0;
        decimal totalValue = 0;
        foreach (var itemRequest in request.Items)
        {
            var item = await _context.Items.FindAsync(new object[] { itemRequest.ItemId }, cancellationToken);
            if (item == null) continue;

            var transferItem = new TransferItem(
                transferId: transfer.Id,
                itemId: itemRequest.ItemId,
                quantity: itemRequest.RequestedQuantity,
                unitPrice: item.StandardCost,
                createdBy: userId,
                notes: itemRequest.Notes,
                isFromCommanderReserve: itemRequest.FromCommanderReserve,
                commanderReserveQuantity: itemRequest.FromCommanderReserve ? itemRequest.RequestedQuantity : 0);

            _context.TransferItems.Add(transferItem);
            totalQuantity += itemRequest.RequestedQuantity;
            totalValue += itemRequest.RequestedQuantity * item.StandardCost;
        }

        transfer.UpdateTotals(totalQuantity, totalValue, userId);
        await _context.SaveChangesAsync(cancellationToken);

        // Notify source warehouse manager
        await _notificationService.SendNotificationToRoleAsync(
            "FactoryWarehouseKeeper",
            $"طلب تحويل جديد - {number}",
            $"طلب تحويل جديد من {fromWarehouse.Name} إلى {toWarehouse.Name}",
            "TransferPending",
            cancellationToken);

        return await GetTransferByIdAsync(transfer.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create transfer");
    }

    public async Task<TransferDto> ApproveTransferAsync(ApproveTransferDto request, int userId, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transfer == null)
            throw new InvalidOperationException("Transfer not found");

        if (transfer.Status != "Pending")
            throw new InvalidOperationException("Transfer is not pending approval");

        transfer.Approve(userId, request.Notes, userId);

        // Update approved quantities by updating item quantities
        foreach (var approveItem in request.Items)
        {
            var item = transfer.Items.FirstOrDefault(i => i.ItemId == approveItem.ItemId);
            if (item != null)
            {
                item.UpdateQuantity(approveItem.ApprovedQuantity, item.UnitPrice, userId);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetTransferByIdAsync(transfer.Id, cancellationToken) ?? throw new InvalidOperationException("Transfer not found");
    }

    public async Task<TransferDto> RejectTransferAsync(RejectTransferDto request, int userId, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers.FindAsync(new object[] { request.Id }, cancellationToken);

        if (transfer == null)
            throw new InvalidOperationException("Transfer not found");

        if (transfer.Status != "Pending")
            throw new InvalidOperationException("Transfer is not pending approval");

        transfer.Reject(userId, request.RejectionReason, userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetTransferByIdAsync(transfer.Id, cancellationToken) ?? throw new InvalidOperationException("Transfer not found");
    }

    public async Task<TransferDto> ShipTransferAsync(ShipTransferDto request, int userId, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transfer == null)
            throw new InvalidOperationException("Transfer not found");

        if (transfer.Status != "Approved")
            throw new InvalidOperationException("Transfer is not approved");

        // Deduct from source warehouse
        foreach (var shipItem in request.Items)
        {
            var transferItem = transfer.Items.FirstOrDefault(i => i.ItemId == shipItem.ItemId);
            if (transferItem == null) continue;

            var inventoryRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == transfer.SourceWarehouseId && ir.ItemId == shipItem.ItemId, cancellationToken);

            if (inventoryRecord != null)
            {
                if (transferItem.IsFromCommanderReserve)
                {
                    if (inventoryRecord.CommanderReserveQuantity - inventoryRecord.ReserveAllocated < shipItem.ShippedQuantity)
                        throw new InvalidOperationException($"Insufficient reserve for item {transferItem.Item?.Name}");
                    inventoryRecord.AdjustReserve(-shipItem.ShippedQuantity, userId);
                }
                else
                {
                    if (inventoryRecord.GeneralQuantity - inventoryRecord.GeneralAllocated < shipItem.ShippedQuantity)
                        throw new InvalidOperationException($"Insufficient stock for item {transferItem.Item?.Name}");
                    inventoryRecord.AdjustGeneral(-shipItem.ShippedQuantity, userId);
                }
            }

            transferItem.Ship(shipItem.ShippedQuantity, userId);
        }

        transfer.Ship(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetTransferByIdAsync(transfer.Id, cancellationToken) ?? throw new InvalidOperationException("Transfer not found");
    }

    public async Task<TransferDto> ReceiveTransferAsync(ReceiveTransferDto request, int userId, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers
            .Include(t => t.Items)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transfer == null)
            throw new InvalidOperationException("Transfer not found");

        if (transfer.Status != "Shipped")
            throw new InvalidOperationException("Transfer has not been shipped");

        decimal totalReceived = 0;
        foreach (var receiveItem in request.Items)
        {
            var transferItem = transfer.Items.FirstOrDefault(i => i.ItemId == receiveItem.ItemId);
            if (transferItem == null) continue;

            // Add to destination warehouse
            var inventoryRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == transfer.DestinationWarehouseId && ir.ItemId == receiveItem.ItemId, cancellationToken);

            if (inventoryRecord == null)
            {
                // Create new inventory record
                var item = await _context.Items.FindAsync(new object[] { receiveItem.ItemId }, cancellationToken);
                inventoryRecord = new InventoryRecord(
                    transfer.DestinationWarehouseId,
                    receiveItem.ItemId,
                    receiveItem.ReceivedQuantity,
                    receiveItem.ReceivedQuantity, // generalQuantity
                    0, // commanderReserveQuantity
                    0, // reserveAllocated
                    item?.ReorderPoint ?? 10,
                    userId);
                _context.InventoryRecords.Add(inventoryRecord);
            }
            else
            {
                inventoryRecord.AdjustGeneral(receiveItem.ReceivedQuantity, userId);
            }

            transferItem.Receive(receiveItem.ReceivedQuantity, userId);
            totalReceived += receiveItem.ReceivedQuantity;
        }

        transfer.Receive(totalReceived, userId);
        await _context.SaveChangesAsync(cancellationToken);

        // Notify about completion
        await _notificationService.SendNotificationToRoleAsync(
            "CentralWarehouseKeeper",
            $"اكتمل التحويل - {transfer.TransferNumber}",
            $"تم استلام التحويل {transfer.TransferNumber} بنجاح",
            "TransferCompleted",
            cancellationToken);

        return await GetTransferByIdAsync(transfer.Id, cancellationToken) ?? throw new InvalidOperationException("Transfer not found");
    }

    public async Task<bool> CancelTransferAsync(int id, int userId, string reason, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.Transfers.FindAsync(new object[] { id }, cancellationToken);
        if (transfer == null)
            return false;

        if (transfer.Status == "Completed" || transfer.Status == "Shipped")
            throw new InvalidOperationException("Cannot cancel transfer that has been shipped or completed");

        transfer.Cancel(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static TransferDto MapToDto(Transfer t)
    {
        return new TransferDto
        {
            Id = t.Id,
            TransferNumber = t.TransferNumber,
            FromWarehouseId = t.SourceWarehouseId,
            FromWarehouseName = t.SourceWarehouse?.Name ?? "",
            ToWarehouseId = t.DestinationWarehouseId,
            ToWarehouseName = t.DestinationWarehouse?.Name ?? "",
            Status = ParseTransferStatus(t.Status),
            RequestDate = t.TransferDate,
            ApprovedDate = t.ApprovalDate,
            ShippedDate = t.ShippedDate,
            ReceivedDate = t.ReceivedDate,
            RequesterId = t.CreatedBy,
            RequesterName = t.CreatedByUser?.FullName ?? "",
            ApprovedById = t.ApprovedBy,
            ApprovedByName = t.ApprovedByUser?.FullName,
            Notes = t.Notes,
            RejectionReason = t.RejectionReason,
            Items = t.Items.Select(i => new TransferItemDto
            {
                Id = i.Id,
                TransferId = i.TransferId,
                ItemId = i.ItemId,
                ItemCode = i.Item?.ItemCode ?? "",
                ItemName = i.Item?.Name ?? "",
                ItemNameArabic = i.Item?.NameArabic ?? "",
                Unit = i.Item?.Unit ?? "",
                RequestedQuantity = i.Quantity,
                ApprovedQuantity = i.Quantity,
                ShippedQuantity = i.ShippedQuantity,
                ReceivedQuantity = i.ReceivedQuantity,
                FromCommanderReserve = i.IsFromCommanderReserve,
                ToCommanderReserve = false,
                Notes = i.Notes
            }).ToList()
        };
    }

    private static TransferStatus ParseTransferStatus(string status) => status switch
    {
        "Draft" => TransferStatus.Draft,
        "Pending" => TransferStatus.Pending,
        "Approved" => TransferStatus.Approved,
        "Rejected" => TransferStatus.Rejected,
        "Shipped" => TransferStatus.Shipped,
        "Completed" => TransferStatus.Completed,
        "Received" => TransferStatus.Completed,
        "Cancelled" => TransferStatus.Cancelled,
        _ => TransferStatus.Draft
    };

    private async Task<string> GenerateTransferNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var count = await _context.Transfers
            .CountAsync(t => t.TransferDate >= today, cancellationToken);
        return $"TRF-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";
    }
}
