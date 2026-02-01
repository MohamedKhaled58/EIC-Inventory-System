using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EICInventorySystem.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryRecordDto>> GetInventoryRecordsAsync(int? warehouseId = null, int? itemId = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Include(ir => ir.Item)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        if (itemId.HasValue)
            query = query.Where(ir => ir.ItemId == itemId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            query = status.ToLower() switch
            {
                "low" => query.Where(ir => ir.GeneralQuantity < ir.ReorderPoint),
                "critical" => query.Where(ir => ir.GeneralQuantity < ir.ReorderPoint * 0.5m),
                "normal" => query.Where(ir => ir.GeneralQuantity >= ir.ReorderPoint),
                _ => query
            };
        }

        var records = await query.ToListAsync(cancellationToken);

        return records.Select(ir => new InventoryRecordDto
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
            CommanderReserveQuantity = ir.CommanderReserveQuantity,
            MinimumReserveRequired = ir.MinimumReserveRequired,
            ReorderPoint = ir.ReorderPoint,
            GeneralAllocated = ir.GeneralAllocated,
            ReserveAllocated = ir.ReserveAllocated,
            AvailableQuantity = ir.GeneralQuantity - ir.GeneralAllocated,
            AvailableReserve = ir.CommanderReserveQuantity - ir.ReserveAllocated,
            Status = ir.GeneralQuantity < ir.ReorderPoint * 0.5m ? "Critical" :
                     ir.GeneralQuantity < ir.ReorderPoint ? "Low" : "Normal",
            LastUpdated = ir.UpdatedAt ?? ir.CreatedAt,
            LastUpdatedBy = null
        });
    }

    public async Task<InventoryRecordDto?> GetInventoryRecordAsync(int warehouseId, int itemId, CancellationToken cancellationToken = default)
    {
        var ir = await _context.InventoryRecords
            .Include(r => r.Warehouse)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.WarehouseId == warehouseId && r.ItemId == itemId, cancellationToken);

        if (ir == null) return null;

        return new InventoryRecordDto
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
            CommanderReserveQuantity = ir.CommanderReserveQuantity,
            MinimumReserveRequired = ir.MinimumReserveRequired,
            ReorderPoint = ir.ReorderPoint,
            GeneralAllocated = ir.GeneralAllocated,
            ReserveAllocated = ir.ReserveAllocated,
            AvailableQuantity = ir.GeneralQuantity - ir.GeneralAllocated,
            AvailableReserve = ir.CommanderReserveQuantity - ir.ReserveAllocated,
            Status = ir.GeneralQuantity < ir.ReorderPoint * 0.5m ? "Critical" :
                     ir.GeneralQuantity < ir.ReorderPoint ? "Low" : "Normal",
            LastUpdated = ir.UpdatedAt ?? ir.CreatedAt,
            LastUpdatedBy = null
        };
    }

    public async Task<IEnumerable<InventorySummaryDto>> GetInventorySummaryAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        var grouped = await query
            .GroupBy(ir => new { ir.WarehouseId, ir.Warehouse!.Name, ir.Warehouse.NameArabic })
            .Select(g => new InventorySummaryDto
            {
                WarehouseId = g.Key.WarehouseId,
                WarehouseName = g.Key.Name,
                WarehouseNameArabic = g.Key.NameArabic,
                TotalItems = g.Count(),
                TotalQuantity = g.Sum(ir => ir.TotalQuantity),
                TotalGeneralStock = g.Sum(ir => ir.GeneralQuantity),
                TotalCommanderReserve = g.Sum(ir => ir.CommanderReserveQuantity),
                LowStockItems = g.Count(ir => ir.GeneralQuantity < ir.ReorderPoint),
                CriticalStockItems = g.Count(ir => ir.GeneralQuantity < ir.ReorderPoint * 0.5m),
                LowReserveItems = g.Count(ir => ir.CommanderReserveQuantity < ir.MinimumReserveRequired)
            })
            .ToListAsync(cancellationToken);

        return grouped;
    }

    public async Task<IEnumerable<ItemTransactionHistoryDto>> GetItemTransactionHistoryAsync(int itemId, int? warehouseId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ItemTransactionHistories
            .Include(t => t.Item)
            .Include(t => t.Project)
            .Include(t => t.Department)
            .Where(t => t.ItemId == itemId)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate <= endDate.Value);

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Take(500)
            .ToListAsync(cancellationToken);

        return transactions.Select(t => new ItemTransactionHistoryDto
        {
            Id = t.Id,
            ItemId = t.ItemId,
            ItemCode = t.Item?.ItemCode ?? "",
            ItemName = t.Item?.Name ?? "",
            ItemNameArabic = t.Item?.NameArabic ?? "",
            WarehouseId = warehouseId ?? 0,
            WarehouseName = "",
            TransactionType = t.TransactionType,
            TransactionTypeArabic = GetTransactionTypeArabic(t.TransactionType),
            Quantity = t.Quantity,
            QuantityBefore = 0,
            QuantityAfter = 0,
            GeneralQuantityBefore = 0,
            GeneralQuantityAfter = 0,
            ReserveQuantityBefore = 0,
            ReserveQuantityAfter = 0,
            IsFromCommanderReserve = false,
            ReferenceNumber = t.ReferenceNumber,
            ReferenceType = t.TransactionType,
            TransactionDate = t.TransactionDate,
            PerformedBy = t.WorkerName,
            Notes = t.Notes
        });
    }

    public async Task<IEnumerable<LowStockAlertDto>> GetLowStockAlertsAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Include(ir => ir.Item)
            .Where(ir => ir.GeneralQuantity < ir.ReorderPoint)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        var records = await query.ToListAsync(cancellationToken);

        return records.Select(ir => new LowStockAlertDto
        {
            ItemId = ir.ItemId,
            ItemCode = ir.Item?.ItemCode ?? "",
            ItemName = ir.Item?.Name ?? "",
            ItemNameArabic = ir.Item?.NameArabic ?? "",
            WarehouseId = ir.WarehouseId,
            WarehouseName = ir.Warehouse?.Name ?? "",
            CurrentQuantity = ir.GeneralQuantity,
            ReorderPoint = ir.ReorderPoint,
            Shortage = ir.ReorderPoint - ir.GeneralQuantity,
            Severity = ir.GeneralQuantity < ir.ReorderPoint * 0.5m ? "Critical" : "Warning",
            AlertDate = DateTime.UtcNow
        });
    }

    public async Task<IEnumerable<ReserveAlertDto>> GetReserveAlertsAsync(int? warehouseId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Include(ir => ir.Item)
            .Where(ir => ir.CommanderReserveQuantity < ir.MinimumReserveRequired)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(ir => ir.WarehouseId == warehouseId.Value);

        var records = await query.ToListAsync(cancellationToken);

        return records.Select(ir => new ReserveAlertDto
        {
            ItemId = ir.ItemId,
            ItemCode = ir.Item?.ItemCode ?? "",
            ItemName = ir.Item?.Name ?? "",
            ItemNameArabic = ir.Item?.NameArabic ?? "",
            WarehouseId = ir.WarehouseId,
            WarehouseName = ir.Warehouse?.Name ?? "",
            CurrentReserve = ir.CommanderReserveQuantity,
            MinimumRequired = ir.MinimumReserveRequired,
            Shortage = ir.MinimumReserveRequired - ir.CommanderReserveQuantity,
            AlertDate = DateTime.UtcNow
        });
    }

    public async Task<bool> AdjustStockAsync(StockAdjustmentDto adjustment, int userId, CancellationToken cancellationToken = default)
    {
        var record = await _context.InventoryRecords
            .FirstOrDefaultAsync(ir => ir.WarehouseId == adjustment.WarehouseId && ir.ItemId == adjustment.ItemId, cancellationToken);

        if (record == null)
            throw new InvalidOperationException("Inventory record not found");

        var previousGeneral = record.GeneralQuantity;
        var previousReserve = record.CommanderReserveQuantity;
        var previousTotal = record.TotalQuantity;

        // Calculate adjustments
        var generalDiff = adjustment.NewGeneralQuantity - previousGeneral;
        var reserveDiff = adjustment.NewReserveQuantity - previousReserve;

        if (generalDiff != 0)
            record.AdjustGeneral(generalDiff, userId);

        if (reserveDiff != 0)
            record.AdjustReserve(reserveDiff, userId);

        // Create stock adjustment record
        var adjustmentType = (adjustment.NewQuantity > previousTotal) 
            ? Domain.Enums.AdjustmentType.Addition 
            : Domain.Enums.AdjustmentType.Correction;

        var stockAdjustment = new Domain.Entities.StockAdjustment(
            record.Id,
            adjustment.ItemId,
            adjustment.WarehouseId,
            previousGeneral,
            adjustment.NewGeneralQuantity,
            adjustmentType,
            userId,
            adjustment.Reason,
            null,
            null,
            false
        );

        _context.StockAdjustments.Add(stockAdjustment);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> TransferStockAsync(StockTransferDto transfer, int userId, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var sourceRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == transfer.FromWarehouseId && ir.ItemId == transfer.ItemId, cancellationToken);

            if (sourceRecord == null)
                throw new InvalidOperationException("Source inventory record not found");

            // Validate and deduct from source
            if (transfer.FromCommanderReserve)
            {
                if (sourceRecord.CommanderReserveQuantity - sourceRecord.ReserveAllocated < transfer.Quantity)
                    throw new InvalidOperationException("Insufficient available commander reserve quantity");
                sourceRecord.AdjustReserve(-transfer.Quantity, userId);
            }
            else
            {
                if (sourceRecord.GeneralQuantity - sourceRecord.GeneralAllocated < transfer.Quantity)
                    throw new InvalidOperationException("Insufficient available general stock quantity");
                sourceRecord.AdjustGeneral(-transfer.Quantity, userId);
            }

            // Add to destination
            var destRecord = await _context.InventoryRecords
                .FirstOrDefaultAsync(ir => ir.WarehouseId == transfer.ToWarehouseId && ir.ItemId == transfer.ItemId, cancellationToken);

            if (destRecord == null)
            {
                // Create new inventory record at destination
                var item = await _context.Items.FindAsync(new object[] { transfer.ItemId }, cancellationToken);
                destRecord = new Domain.Entities.InventoryRecord(
                    transfer.ToWarehouseId,
                    transfer.ItemId,
                    transfer.Quantity,
                    transfer.ToCommanderReserve ? 0 : transfer.Quantity,
                    transfer.ToCommanderReserve ? transfer.Quantity : 0,
                    0,
                    item?.ReorderPoint ?? 10,
                    userId
                );
                _context.InventoryRecords.Add(destRecord);
            }
            else
            {
                if (transfer.ToCommanderReserve)
                    destRecord.AdjustReserve(transfer.Quantity, userId);
                else
                    destRecord.AdjustGeneral(transfer.Quantity, userId);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static string GetTransactionTypeArabic(string transactionType) => transactionType switch
    {
        "Receiving" => "استلام",
        "Issuing" => "صرف",
        "Transfer" => "تحويل",
        "Adjustment" => "تسوية",
        "Return" => "مرتجع",
        _ => transactionType
    };
}
